﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using NewLife.Common;
using NewLife.Log;
using NewLife.Net;
using NewLife.Net.IO;
using NewLife.Net.Proxy;
using NewLife.Net.Stress;
using NewLife.Reflection;
using NewLife.Remoting;
using NewLife.Security;
using NewLife.Serialization;
using NewLife.Xml;
using XCode.DataAccessLayer;
using XCode.Membership;
using XCode.Transform;

namespace Test
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;

            //XTrace.Log = new NetworkLog();
            XTrace.UseConsole();
#if DEBUG
            XTrace.Debug = true;
#endif
            while (true)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
#if !DEBUG
                try
                {
#endif
                Test4();
#if !DEBUG
                }
                catch (Exception ex)
                {
                    XTrace.WriteException(ex);
                }
#endif

                sw.Stop();
                Console.WriteLine("OK! 耗时 {0}", sw.Elapsed);
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.C) break;
            }
        }

        static void Test1()
        {
            var svr = new NetServer<WebSocketSession>();
            svr.ProtocolType = NetType.Tcp;
            svr.Port = 8888;
            svr.Received += Svr_Received;
            svr.Log = XTrace.Log;
            svr.LogSend = true;
            svr.LogReceive = true;
            svr.Start();

            Console.ReadKey();
        }

        private static void Svr_Received(Object sender, ReceivedEventArgs e)
        {
            Console.WriteLine(e.ToStr());

            var ss = sender as INetSession;
            ss.Send("收到 {0}".F(DateTime.Now));
        }

        class A
        {
            public Int32 ID { get; set; }
            public String Name { get; set; }

            public Int32 Add(Int32 m)
            {
                return ID + m;
            }
        }

        static void Test3()
        {
            var uri = new NetUri("udp://x2:3389");

            Console.WriteLine(uri);
            Console.WriteLine(uri.Type);
            Console.WriteLine(uri.EndPoint);
            Console.WriteLine(uri.Address);
            Console.WriteLine(uri.Host);
            Console.WriteLine(uri.Port);

            var xml = uri.ToXml();
            Console.WriteLine(xml);

            uri = xml.ToXmlEntity<NetUri>();
            Console.WriteLine(uri);
        }

        static void Test4()
        {
            ApiTest.Main();
        }

        static Statistics stat = new Statistics();
        static void Test5()
        {
            var svr = new HttpReverseProxy();
            svr.RemoteServer.Host = "www.cnblogs.com";
            svr.Port = 888;
            svr.Start();

            while (true)
            {
                Console.Title = "在线 {0:n0} {1}".F(svr.SessionCount, svr.StatSession);
                Thread.Sleep(500);
            }
        }

        static void OnRequest(HttpListener svr, HttpListenerContext context)
        {
            svr.GetContextAsync().ContinueWith(t => OnRequest(svr, t.Result));

            stat.Increment();

            var buf = DateTime.Now.ToFullString().GetBytes();
            context.Response.ContentLength64 = buf.Length;
            context.Response.OutputStream.Write(buf);
            context.Response.OutputStream.Close();
        }

        static void Test7()
        {
            //Console.Write("请输入表达式：");
            //var code = Console.ReadLine();

            //var rs = ScriptEngine.Execute(code, new Dictionary<String, Object> { { "a", 222 }, { "b", 333 } });
            ////Console.WriteLine(rs);

            //var se = ScriptEngine.Create(code);
            //var fm = code.Replace("a", "{0}").Replace("b", "{1}");
            //for (int i = 1; i <= 9; i++)
            //{
            //    for (int j = 1; j <= i; j++)
            //    {
            //        Console.Write(fm + "={2}\t", j, i, se.Invoke(i, j));
            //    }
            //    Console.WriteLine();
            //}

            var se = ScriptEngine.Create("Test.Program.TestMath(k)");
            if (se.Method == null)
            {
                se.Parameters.Add("k", typeof(Double));
                se.Compile();
            }

            var fun = (DM)(Object)Delegate.CreateDelegate(typeof(DM), se.Method as MethodInfo);

            var timer = 1000000;
            var k = 123;
            CodeTimer.ShowHeader();
            CodeTimer.TimeLine("原生", timer, n => TestMath(k));
            CodeTimer.TimeLine("动态", timer, n => se.Invoke(k));
            CodeTimer.TimeLine("动态2", timer, n => fun(k));
        }
        public static Double TestMath(Double k)
        {
            //var bts = File.ReadAllBytes(Assembly.GetExecutingAssembly().Location);
            return Math.Sin(k) * Math.Log10(k) * Math.Exp(k);
        }
        delegate Object DM(Double k);

        static SysConfig Load()
        {
            var filename = SysConfig._.ConfigFile;
            if (filename.IsNullOrWhiteSpace()) return null;
            filename = filename.GetFullPath();
            if (!File.Exists(filename)) return null;

            try
            {
                var config = filename.ToXmlFileEntity<SysConfig>();
                if (config == null) return null;

                //config.OnLoaded();

                //// 第一次加载，建立定时重载定时器
                //if (timer == null && _.ReloadTime > 0) timer = new TimerX(s => Current = null, null, _.ReloadTime * 1000, _.ReloadTime * 1000);

                return config;
            }
            catch (Exception ex) { XTrace.WriteException(ex); return null; }
        }

        static void Test9()
        {
            var user = UserX.FindAllWithCache()[0];
            Console.WriteLine(user.RoleName);
            Console.Clear();

            //var bn = new Binary();
            //bn.EnableTrace();
            var bn = new Xml();
            bn.Write(user);

            var sw = new Stopwatch();
            sw.Start();

            var buf = bn.GetBytes();
            Console.WriteLine(buf.ToHex());
            Console.WriteLine(bn.GetString());

            var ms = new MemoryStream(buf);
            //bn = new Binary();
            bn.Stream = ms;
            //bn.EnableTrace();
            var u = bn.Read<UserX>();

            foreach (var item in UserX.Meta.AllFields)
            {
                if (user[item.Name] == u[item.Name])
                    Console.WriteLine("{0} {1} <=> {2} 通过", item.Name, user[item.Name], u[item.Name]);
                else
                    Console.WriteLine("{0} {1} <=> {2} 失败", item.Name, user[item.Name], u[item.Name]);
            }

            //var hi = HardInfo.Current;
            //sw.Stop();
            //Console.WriteLine(sw.Elapsed);
            //Console.WriteLine(hi);

            //var ci = new ComputerInfo();
            //Console.WriteLine(ci);
        }

        static void Test10()
        {
            NetHelper.ShowTcpParameters();
            Console.WriteLine("k键设置最优Tcp参数，其它键开始测试：");
            var key = Console.ReadKey();
            if (key.KeyChar == 'k') NetHelper.SetTcpMax();

            TcpStress.Main();
        }

        static void Test13()
        {
            var file = @"E:\BaiduYunDownload\xiaomi.db";
            var file2 = Path.ChangeExtension(file, "sqlite");
            DAL.AddConnStr("src", "Data Source=" + file, null, "sqlite");
            DAL.AddConnStr("des", "Data Source=" + file2, null, "sqlite");

            if (!File.Exists(file2))
            {
                var et = new EntityTransform();
                et.SrcConn = "src";
                et.DesConn = "des";
                //et.PartialTableNames.Add("xiaomi");
                //et.PartialCount = 1000000;

                et.Transform();
            }

            var sw = new Stopwatch();

            var dal = DAL.Create("src");
            var eop = dal.CreateOperate(dal.Tables[0].TableName);
            sw.Start();
            var count = eop.Count;
            sw.Stop();
            XTrace.WriteLine("{0} 耗时 {1}ms", count, sw.ElapsedMilliseconds);
            sw.Reset(); sw.Start();
            count = eop.FindCount();
            sw.Stop();
            XTrace.WriteLine("{0} 耗时 {1}ms", count, sw.ElapsedMilliseconds);

            var entity = eop.Create();
            entity["username"] = "Stone";
            entity.Save();
            count = eop.FindCount();
            Console.WriteLine(count);

            entity.Delete();
            count = eop.FindCount();
            Console.WriteLine(count);
        }

        static void Test14()
        {
            Console.Clear();
            //XTrace.Log.Level = LogLevel.Info;
            var server = new FileServer();
            server.Log = XTrace.Log;
            server.Start();

            var count = 0;
            Action<Object> func = s =>
            {
                count++;
                var client = new FileClient();
                try
                {
                    client.Log = XTrace.Log;
                    if (s + "" == "Test.exe")
                        client.Connect("127.0.0.1", server.Port);
                    else
                        client.Connect("::1", server.Port);
                    client.SendFile(s + "");
                }
                finally
                {
                    count--;
                    client.Dispose();
                }
            };

            Task.Factory.StartNew(func, "Test.exe");
            Task.Factory.StartNew(func, "NewLife.Core.dll");
            Task.Factory.StartNew(func, "NewLife.Net.dll");

            var file = @"F:\MS\cn_visual_studio_ultimate_2013_with_update_4_x86_dvd_5935081.iso";
            if (File.Exists(file))
                Task.Factory.StartNew(func, file);

            Thread.Sleep(500);
            while (count > 0) Thread.Sleep(200);
            server.Dispose();
        }

        static void Test15()
        {
            //"我是超级大石头！".Speak();

            var tcp = new TcpSession();
            tcp.Log = XTrace.Log;
            tcp.Remote = "tcp://127.0.0.1:8";
            //tcp.MessageDgram = true;
            tcp.AutoReconnect = 0;
            //tcp.Send("我是大石头！");
            tcp.Open();
            //tcp.Stream = new PacketStream(tcp.Stream);
            //var ms = new MemoryStream();
            for (int i = 0; i < 10; i++)
            {
                tcp.Send("我是大石头{0}！".F(i + 1));
                //var buf = "我是大石头{0}！".F(i + 1).GetBytes();
                //ms.WriteEncodedInt(buf.Length);
                //ms.Write(buf);
            }
            //ms.Position = 0;
            //tcp.Client.GetStream().Write(ms);

            while (tcp.Active)
            {
                var str = tcp.ReceiveString();
                Console.WriteLine(str);
            }

            //NetHelper.Debug = true;
            //var server = new StunServer();
            //server.Start();

            //Console.WriteLine(NetHelper.MyIP().GetAddress());
            //Console.WriteLine(IPAddress.Any.GetAddress());
            //Console.WriteLine(IPAddress.Loopback.GetAddress());

            //var buf = NetHelper.MyIP().GetAddressBytes();
            //buf[3] = 33;
            //Console.WriteLine(new IPAddress(buf).GetAddress());

            //var ip = NetHelper.ParseAddress("dg.newlifex.com");
            //Console.WriteLine(ip.GetAddress());
            //Console.WriteLine(Ip.GetAddress(ip.ToString()));

            //var client = new StunClient();
            //var rs = client.Query();
            //if (rs != null)
            //{
            //    //if (rs != null && rs.Type == StunNetType.Blocked && rs.Public != null) rs.Type = StunNetType.Symmetric;
            //    XTrace.WriteLine("网络类型：{0} {1}", rs.Type, rs.Type.GetDescription());
            //    XTrace.WriteLine("公网地址：{0} {1}", rs.Public, Ip.GetAddress(rs.Public.Address.ToString()));
            //}
        }

        static List<UdpServer> Clients = new List<UdpServer>();
        private static void Test16()
        {
            for (int i = 0; i < 2000; i++)
            {
                var client = new UdpServer();
                Clients.Add(client);

                //client.Log = XTrace.Log;
                //client.LogSend = true;
                //client.LogReceive = true;
                client.Remote = "udp://192.168.0.12:89";
                client.Received += (s, e) => XTrace.WriteLine("{0} {1}", (s as UdpServer).Name, e.ToStr());
                //client.ReceiveAsync();
            }

            for (int i = 0; i < 100; i++)
            {
                foreach (var client in Clients)
                {
                    client.Send("Hello NewLife!");
                }
                Thread.Sleep(10000);
            }
        }
    }
}