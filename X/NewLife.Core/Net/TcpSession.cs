﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NewLife.Net
{
    /// <summary>增强TCP客户端</summary>
    public class TcpSession : SessionBase, ISocketSession
    {
        #region 属性
        /// <summary>会话编号</summary>
        public Int32 ID { get; internal set; }

        /// <summary>收到空数据时抛出异常并断开连接。默认true</summary>
        public Boolean DisconnectWhenEmptyData { get; set; }

        ///// <summary>会话数据流，供用户程序使用。可用于解决Tcp粘包的问题。</summary>
        //public Stream Stream { get; set; }

        ISocketServer _Server;
        /// <summary>Socket服务器。当前通讯所在的Socket服务器，其实是TcpServer/UdpServer。该属性决定本会话是客户端会话还是服务的会话</summary>
        ISocketServer ISocketSession.Server { get { return _Server; } }

        /// <summary>自动重连次数，默认3。发生异常断开连接时，自动重连服务端。</summary>
        public Int32 AutoReconnect { get; set; }
        #endregion

        #region 构造
        /// <summary>实例化增强TCP</summary>
        public TcpSession()
        {
            Name = GetType().Name;
            Local = new NetUri(NetType.Tcp, IPAddress.Any, 0);
            Remote = new NetUri(NetType.Tcp, IPAddress.Any, 0);

            DisconnectWhenEmptyData = true;
            AutoReconnect = 3;
        }

        /// <summary>使用监听口初始化</summary>
        /// <param name="listenPort"></param>
        public TcpSession(Int32 listenPort)
            : this()
        {
            Port = listenPort;
        }

        /// <summary>用TCP客户端初始化</summary>
        /// <param name="client"></param>
        public TcpSession(Socket client)
            : this()
        {
            if (client == null) return;

            Client = client;
            var socket = client;
            if (socket.LocalEndPoint != null) Local.EndPoint = (IPEndPoint)socket.LocalEndPoint;
            if (socket.RemoteEndPoint != null) Remote.EndPoint = (IPEndPoint)socket.RemoteEndPoint;
        }

        internal TcpSession(ISocketServer server, Socket client)
            : this(client)
        {
            Active = true;
            _Server = server;
            Name = server.Name;
        }
        #endregion

        #region 方法
        /// <summary>打开</summary>
        protected override Boolean OnOpen()
        {
            // 服务端会话没有打开
            if (_Server != null) return false;

            if (Client == null || !Client.IsBound)
            {
                // 根据目标地址适配本地IPv4/IPv6
                if (Remote != null && !Remote.Address.IsAny())
                {
                    Local.Address = Local.Address.GetRightAny(Remote.Address.AddressFamily);
                }

                Client = NetHelper.CreateTcp(Local.EndPoint.Address.IsIPv4());
                //Client.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
                Client.Bind(Local.EndPoint);
                CheckDynamic();

                WriteLog("Open {0}", this);
            }

            // 打开端口前如果已设定远程地址，则自动连接
            if (Remote == null || Remote.EndPoint.IsAny()) return false;

            try
            {
                Client.Connect(Remote.EndPoint);
            }
            catch (Exception ex)
            {
                if (!Disposed && !ex.IsDisposed()) OnError("Connect", ex);
                if (ThrowException) throw;

                return false;
            }

            _Reconnect = 0;

            return true;
        }

        /// <summary>关闭</summary>
        protected override Boolean OnClose(String reason)
        {
            var client = Client;
            if (client != null)
            {
                Client = null;
                WriteLog("Close {0} {1}", reason, this);

                // 提前关闭这个标识，否则Close时可能触发自动重连机制
                Active = false;
                try
                {
                    // 温和一点关闭连接
                    //Client.Shutdown();
                    client.Close();

                    // 如果是服务端，这个时候就是销毁
                    if (_Server != null) Dispose();
                }
                catch (Exception ex)
                {
                    if (!ex.IsDisposed()) OnError("Close", ex);
                    if (ThrowException) throw;

                    return false;
                }
            }
            //Stream = null;

            return true;
        }
        #endregion

        #region 发送
        /// <summary>发送数据</summary>
        /// <remarks>
        /// 目标地址由<seealso cref="SessionBase.Remote"/>决定
        /// </remarks>
        /// <param name="buffer">缓冲区</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">数量</param>
        /// <returns>是否成功</returns>
        public override Boolean Send(Byte[] buffer, Int32 offset = 0, Int32 count = -1)
        {
            if (!Open()) return false;

            if (count < 0) count = buffer.Length - offset;

            if (StatSend != null) StatSend.Increment(count);
            if (Log != null && Log.Enable && LogSend) WriteLog("Send [{0}]: {1}", count, buffer.ToHex(0, Math.Min(count, 32)));

            try
            {
                // 修改发送缓冲区
                if (Client.SendBufferSize < count) Client.SendBufferSize = count;

                if (count == 0)
                    Client.Send(new Byte[0]);
                else
                    Client.Send(buffer, offset, count, SocketFlags.None);
            }
            catch (Exception ex)
            {
                if (!ex.IsDisposed())
                {
                    OnError("Send", ex);

                    // 发送异常可能是连接出了问题，需要关闭
                    Close("发送出错");
                    Reconnect();

                    if (ThrowException) throw;
                }

                return false;
            }

            LastTime = DateTime.Now;

            return true;
        }

        internal override bool OnSendAsync(SocketAsyncEventArgs se) { return Client.SendAsync(se); }
        #endregion

        #region 接收
        /// <summary>接收数据</summary>
        /// <returns>收到的数据。如果没有数据返回0长度数组，如果出错返回null</returns>
        public override Byte[] Receive()
        {
            if (!Open()) return null;

            var size = 1024 * 2;

            //// 报文模式调整缓冲区大小。还差这么多数据就足够一个报文
            //var ps = Stream as PacketStream;
            //if (ps != null && ps.Size > 0) size = ps.Size;

            var buf = new Byte[size];

            var count = Receive(buf, 0, buf.Length);
            if (count < 0) return null;
            if (count == 0) return new Byte[0];

            LastTime = DateTime.Now;
            //if (StatReceive != null) StatReceive.Increment(count);

            if (count == buf.Length) return buf;

            return buf.ReadBytes(0, count);
        }

        /// <summary>读取指定长度的数据，一般是一帧</summary>
        /// <param name="buffer">缓冲区</param>
        /// <param name="offset">偏移</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public override Int32 Receive(Byte[] buffer, Int32 offset = 0, Int32 count = -1)
        {
            if (!Open()) return -1;

            if (count < 0) count = buffer.Length - offset;

            var rs = 0;
            try
            {
                //if (count > 0) rs = Stream.Read(buffer, offset, count);
                if (count > 0) rs = Client.Receive(buffer, offset, count, SocketFlags.None);
            }
            catch (Exception ex)
            {
                if (!ex.IsDisposed())
                {
                    OnError("Receive", ex);

                    // 发送异常可能是连接出了问题，需要关闭
                    Close("同步接收出错");
                    Reconnect();

                    if (ThrowException) throw;
                }

                return -1;
            }

            LastTime = DateTime.Now;
            if (StatReceive != null) StatReceive.Increment(rs);

            return rs;
        }

        internal override bool OnReceiveAsync(SocketAsyncEventArgs se)
        {
            var client = Client;
            if (client == null || !Active || Disposed) throw new ObjectDisposedException(GetType().Name);
            
            return client.ReceiveAsync(se);
        }

        /// <summary>处理收到的数据</summary>
        /// <param name="data"></param>
        /// <param name="remote"></param>
        internal override void OnReceive(Byte[] data, IPEndPoint remote)
        {
            if (data.Length == 0 && DisconnectWhenEmptyData)
            {
                Close("收到空数据");
                Dispose();
            }

            OnReceive(data, data.Length);
        }

        /// <summary>处理收到的数据</summary>
        /// <param name="data"></param>
        /// <param name="count"></param>
        protected virtual void OnReceive(Byte[] data, Int32 count)
        {
#if !__MOBILE__
            // 更新全局远程IP地址
            NewLife.Web.WebHelper.UserHost = Remote.EndPoint.ToString();
#endif
            // 分析处理
            var e = new ReceivedEventArgs();
            e.Data = data;
            e.Length = count;
            e.UserState = Remote.EndPoint;

            if (Log.Enable && LogReceive) WriteLog("Recv [{0}]: {1}", e.Length, e.Data.ToHex(0, Math.Min(e.Length, 32)));

            RaiseReceive(this, e);
        }
        #endregion

        #region 粘包处理
        /// <summary>用于粘包处理的数据流缓冲区，指针位于末端，便于附加数据。前面是7位压缩编码整数表示的报文长度</summary>
        private MemoryStream Packet { get; set; }

        /// <summary>所期望的报文大小</summary>
        private Int32 PacketSize { get; set; }

        private Byte[] CheckPacket(Byte[] data, ref Int32 count)
        {
            var ms = Packet;

            // 检查上一次接收数据是否有剩余
            if (ms == null || ms.Length == 0)
            {
                // 空包，并且上次没有剩余
                if (count <= 0) return data;

                // 先转为数据流
                ms = new MemoryStream(data, 0, count);

                var len = ms.ReadEncodedInt();
                // 如果长度足够整包，可以返回，剩余部分留下
                if (len <= ms.Length - ms.Position)
                {
                    WriteLog("得到报文{0}字节", len);
                    count = len;
                    data = ReadPacket(ms, len);
                }
                else
                {
                    WriteLog("数据包大小{0}字节不能满足报文大小{1}字节", count, len);
                    count = -1;

                    PacketSize = len;
                    Packet = new MemoryStream();
                    Packet.Write(ms);
                }
            }
            else
            {
                // 如果上一次有剩余，则附加data到后面
                if (data != null && data.Length > 0 && count > 0)
                {
                    WriteLog("附加数据包{0}字节到上一次剩余数据包{1}字节", count, PacketSize);
                    ms.Write(data, 0, count);
                }

                var len = PacketSize;
                // 如果长度足够整包，可以返回，剩余部分留下
                if (len <= ms.Length)
                {
                    WriteLog("凑够报文{0}字节", len);
                    ms.Position = 0;
                    count = len;
                    data = ReadPacket(ms, len);
                }
                else
                {
                    WriteLog("仍然无法满足报文大小{0}字节", len);
                    count = -1;
                }
            }

            return data;
        }

        Byte[] ReadPacket(Stream ms, Int32 len)
        {
            var data = ms.ReadBytes(len);

            // 剩余部分先读取长度，然后数据放到Packet里面
            if (ms.Position < ms.Length)
            {
                PacketSize = ms.ReadEncodedInt();
                Packet = new MemoryStream();
                Packet.Write(ms);
            }
            else
            {
                PacketSize = 0;
                Packet = null;
            }

            return data;
        }
        #endregion

        #region 自动重连
        /// <summary>重连次数</summary>
        private Int32 _Reconnect;
        void Reconnect()
        {
            if (Disposed) return;
            // 如果重连次数达到最大重连次数，则退出
            if (Interlocked.Increment(ref _Reconnect) > AutoReconnect) return;

            WriteLog("Reconnect {0}", this);

            Open();
        }
        #endregion

        #region 辅助
        private String _LogPrefix;
        /// <summary>日志前缀</summary>
        public override String LogPrefix
        {
            get
            {
                if (_LogPrefix == null)
                {
                    var name = _Server == null ? "" : _Server.Name;
                    _LogPrefix = "{0}[{1}].".F(name, ID);
                }
                return _LogPrefix;
            }
            set { _LogPrefix = value; }
        }

        /// <summary>已重载。</summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Remote != null && !Remote.EndPoint.IsAny())
            {
                if (_Server == null)
                    return String.Format("{0}=>{1}", Local, Remote.EndPoint);
                else
                    return String.Format("{0}<={1}", Local, Remote.EndPoint);
            }
            else
                return Local.ToString();
        }
        #endregion
    }
}