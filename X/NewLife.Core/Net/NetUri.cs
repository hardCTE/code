﻿using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace NewLife.Net
{
    /// <summary>协议类型</summary>
    public enum NetType : byte
    {
        /// <summary>未知协议</summary>
        Unknown = 0,

        /// <summary>传输控制协议</summary>
        Tcp = 6,

        /// <summary>用户数据报协议</summary>
        Udp = 17
    }

    /// <summary>网络资源标识，指定协议、地址、端口、地址族（IPv4/IPv6）</summary>
    /// <remarks>
    /// 仅序列化<see cref="Type"/>和<see cref="EndPoint"/>，其它均是配角！
    /// 有可能<see cref="Host"/>代表主机域名，而<see cref="Address"/>指定主机IP地址。
    /// </remarks>
    public class NetUri //: IAccessor
    {
        #region 属性
        private NetType _Type;
        /// <summary>协议类型</summary>
        public NetType Type { get { return _Type; } set { _Type = value; _Protocol = value.ToString(); } }

        [NonSerialized]
        private String _Protocol;
        /// <summary>协议</summary>
        [XmlIgnore]
        public String Protocol
        {
            get { return _Protocol; }
            set
            {
                _Protocol = value;
                if (String.IsNullOrEmpty(value))
                    _Type = NetType.Unknown;
                else
                {
                    try
                    {
                        _Type = (NetType)Enum.Parse(typeof(NetType), value, true);
                        // 规范化名字
                        _Protocol = _Type.ToString();
                    }
                    catch { _Type = NetType.Unknown; }
                }
            }
        }

        /// <summary>地址</summary>
        [XmlIgnore]
        public IPAddress Address { get { return EndPoint.Address; } set { EndPoint.Address = value; _Host = value + ""; } }

        private String _Host;
        /// <summary>主机</summary>
        public String Host { get { return _Host; } set { _Host = value; _EndPoint = null; /*只清空，避免这里耗时 try { EndPoint.Address = ParseAddress(value); } catch { }*/ } }

        private Int32 _Port;
        /// <summary>端口</summary>
        public Int32 Port { get { return _Port = EndPoint.Port; } set { _Port = EndPoint.Port = value; } }

        [NonSerialized]
        private IPEndPoint _EndPoint;
        /// <summary>终结点</summary>
        [XmlIgnore]
        public IPEndPoint EndPoint
        {
            get
            {
                // Host每次改变都会清空
                if (_EndPoint == null) _EndPoint = new IPEndPoint(ParseAddress(_Host) ?? IPAddress.Any, _Port);
                return _EndPoint;
            }
            set
            {
                // 考虑到序列化问题，Host可能是域名，而Address只是地址
                _EndPoint = value;
                if (value != null)
                {
                    _Host = _EndPoint.Address + "";
                    _Port = _EndPoint.Port;
                }
                else
                {
                    _Host = null;
                    _Port = 0;
                }
            }
        }
        #endregion

        #region 扩展属性
        /// <summary>是否Tcp协议</summary>
        [XmlIgnore]
        public Boolean IsTcp { get { return Type == NetType.Tcp; } }

        /// <summary>是否Udp协议</summary>
        [XmlIgnore]
        public Boolean IsUdp { get { return Type == NetType.Udp; } }
        #endregion

        #region 构造
        /// <summary>实例化</summary>
        public NetUri() { }

        /// <summary>实例化</summary>
        /// <param name="uri"></param>
        public NetUri(String uri) { Parse(uri); }

        /// <summary>实例化</summary>
        /// <param name="protocol"></param>
        /// <param name="endpoint"></param>
        public NetUri(NetType protocol, IPEndPoint endpoint)
        {
            Type = protocol;
            EndPoint = endpoint;
        }

        /// <summary>实例化</summary>
        /// <param name="protocol"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public NetUri(NetType protocol, IPAddress address, Int32 port)
        {
            Type = protocol;
            Address = address;
            Port = port;
        }

        /// <summary>实例化</summary>
        /// <param name="protocol"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public NetUri(NetType protocol, String host, Int32 port)
        {
            Type = protocol;
            Host = host;
            Port = port;
        }
        #endregion

        #region 方法
        static readonly String Sep = "://";

        /// <summary>分析</summary>
        /// <param name="uri"></param>
        public NetUri Parse(String uri)
        {
            if (uri.IsNullOrWhiteSpace()) return this;

            // 分析协议
            var p = uri.IndexOf(Sep);
            if (p >= 0)
            {
                Protocol = uri.Substring(0, p);
                uri = uri.Substring(p + Sep.Length);
            }

            // 分析端口
            p = uri.LastIndexOf(":");
            if (p >= 0)
            {
                var pt = uri.Substring(p + 1);
                Int32 port = 0;
                if (Int32.TryParse(pt, out port))
                {
                    Port = port;
                    uri = uri.Substring(0, p);
                }
            }

            Host = uri;

            return this;
        }

        /// <summary>克隆</summary>
        /// <returns></returns>
        public NetUri Clone()
        {
            return new NetUri().CopyFrom(this);
        }

        /// <summary>从另一个对象复制</summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public NetUri CopyFrom(NetUri uri)
        {
            if (uri == null) return this;

            Protocol = uri.Protocol;
            Host = uri.Host;
            Port = uri.Port;

            return this;
        }

        /// <summary>从另一个对象复制</summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public NetUri CopyFrom(Uri uri)
        {
            if (uri == null) return this;

            Host = uri.Host;
            Port = uri.Port;

            return this;
        }
        #endregion

        #region 辅助
        /// <summary>分析地址</summary>
        /// <param name="hostname">主机地址</param>
        /// <returns></returns>
        public static IPAddress ParseAddress(String hostname)
        {
            if (String.IsNullOrEmpty(hostname)) return null;

            try
            {
                IPAddress addr = null;
                if (IPAddress.TryParse(hostname, out addr)) return addr;

                var hostAddresses = Dns.GetHostAddresses(hostname);
                if (hostAddresses == null || hostAddresses.Length < 1) return null;

                return hostAddresses.FirstOrDefault(d => d.AddressFamily == AddressFamily.InterNetwork || d.AddressFamily == AddressFamily.InterNetworkV6);
            }
            catch (SocketException ex)
            {
                throw new XException("解析主机" + hostname + "的地址失败！" + ex.Message, ex);
            }
        }

        /// <summary>已重载。</summary>
        /// <returns></returns>
        public override string ToString()
        {
            var p = Type == NetType.Unknown ? "" : Protocol;
            if (Port > 0)
                return String.Format("{0}://{1}:{2}", p, Host, Port);
            else
                return String.Format("{0}://{1}", p, Host);
        }
        #endregion

        #region 重载运算符
        /// <summary>重载类型转换，字符串直接转为NetUri对象</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator NetUri(String value)
        {
            return new NetUri(value);
        }

        /// <summary>是否相等的地址</summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public Boolean Equals(NetUri uri)
        {
            return Type == uri.Type && Port == uri.Port && Address == uri.Address;
        }
        #endregion

        #region IAccessor 成员

        //bool IAccessor.Read(IReader reader) { return false; }

        //bool IAccessor.ReadComplete(IReader reader, bool success)
        //{
        //    // 因为反序列化仅给字段复制，重新设置一下，保证Protocol等属性有值
        //    ProtocolType = ProtocolType;
        //    EndPoint = EndPoint;

        //    return success;
        //}

        //bool IAccessor.Write(IWriter writer) { return false; }

        //bool IAccessor.WriteComplete(IWriter writer, bool success) { return success; }

        #endregion
    }
}