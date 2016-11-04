﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using NewLife.Configuration;
using NewLife.Log;
using NewLife.Model;
using NewLife.Reflection;

namespace NewLife.Agent
{
    /// <summary>服务程序基类</summary>
    /// <typeparam name="TService">服务类型</typeparam>
    public abstract class AgentServiceBase<TService> : AgentServiceBase, IAgentService
         where TService : AgentServiceBase<TService>, new()
    {
        #region 构造
        static AgentServiceBase()
        {
            if (_Instance == null) _Instance = new TService();
        }
        #endregion

        #region 静态辅助函数
        /// <summary>服务主函数</summary>
        public static void ServiceMain()
        {
            //提升进程优先级
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            var service = Instance as TService;

            // 根据配置修改服务名
            var name = Setting.Current.ServiceName;
            if (!String.IsNullOrEmpty(name)) Instance.ServiceName = name;

            var Args = Environment.GetCommandLineArgs();

            if (Args.Length > 1)
            {
                #region 命令
                String cmd = Args[1].ToLower();
                if (cmd == "-s")  //启动服务
                {
                    var ServicesToRun = new ServiceBase[] { service };

                    try
                    {
                        ServiceBase.Run(ServicesToRun);
                    }
                    catch (Exception ex)
                    {
                        XTrace.WriteException(ex);
                    }
                    return;
                }
                else if (cmd == "-i") //安装服务
                {
                    service.Install(true);
                    return;
                }
                else if (cmd == "-u") //卸载服务
                {
                    service.Install(false);
                    return;
                }
                else if (cmd == "-start") //启动服务
                {
                    service.ControlService(true);
                    return;
                }
                else if (cmd == "-stop") //停止服务
                {
                    service.ControlService(false);
                    return;
                }
                else if (cmd == "-run") //循环执行任务
                {
                    var service2 = new TService();
                    service2.StartWork();
                    Console.ReadKey(true);
                    return;
                }
                else if (cmd == "-step") //单步执行任务
                {
                    var service2 = new TService();
                    for (int i = 0; i < service2.ThreadCount; i++)
                    {
                        service2.Work(i);
                    }
                    return;
                }
                #endregion
            }
            else
            {
                Console.Title = service.DisplayName;

                #region 命令行
                XTrace.UseConsole();

                //输出状态
                service.ShowStatus();

                while (true)
                {
                    //输出菜单
                    service.ShowMenu();
                    Console.Write("请选择操作（-x是命令行参数）：");

                    //读取命令
                    var key = Console.ReadKey();
                    if (key.KeyChar == '0') break;
                    Console.WriteLine();
                    Console.WriteLine();

                    switch ((Int32)(key.KeyChar - '0'))
                    {
                        case 1:
                            //输出状态
                            service.ShowStatus();

                            break;
                        case 2:
                            if (service.IsInstalled() == true)
                                service.Install(false);
                            else
                                service.Install(true);
                            break;
                        case 3:
                            if (service.IsRunning() == true)
                                service.ControlService(false);
                            else
                                service.ControlService(true);
                            break;
                        case 4:
                            #region 单步调试
                            try
                            {
                                var count = Instance.ThreadCount;
                                Int32 n = 0;
                                if (count > 1)
                                {
                                    Console.Write("请输入要调试的任务（任务数：{0}）：", count);
                                    ConsoleKeyInfo k = Console.ReadKey();
                                    Console.WriteLine();
                                    n = k.KeyChar - '0';
                                }

                                Console.WriteLine("正在单步调试……");
                                if (n < 0 || n > count - 1)
                                {
                                    for (int i = 0; i < count; i++)
                                    {
                                        service.Work(i);
                                    }
                                }
                                else
                                {
                                    service.Work(n);
                                }
                                Console.WriteLine("单步调试完成！");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                            #endregion
                            break;
                        case 5:
                            #region 循环调试
                            try
                            {
                                Console.WriteLine("正在循环调试……");
                                service.StartWork();

                                Console.WriteLine("任意键结束循环调试！");
                                Console.ReadKey(true);

                                service.StopWork();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                            #endregion
                            break;
                        case 6:
                            #region 附加服务
                            Console.WriteLine("正在附加服务调试……");
                            service.StartAttachServers();

                            Console.WriteLine("任意键结束附加服务调试！");
                            Console.ReadKey(true);

                            service.StopAttachServers();
                            #endregion
                            break;
                        case 7:
                            if (WatchDogs.Length > 0) CheckWatchDog();
                            break;
                        //case 8:
                        //    RunUI();
                        //    break;
                        default:
                            break;
                    }
                }
                #endregion
            }
        }

        /// <summary>显示状态</summary>
        protected virtual void ShowStatus()
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            var service = Instance as IAgentService;
            var name = service.ServiceName;

            if (name != service.DisplayName)
                Console.WriteLine("服务：{0}({1})", service.DisplayName, name);
            else
                Console.WriteLine("服务：{0}", name);
            Console.WriteLine("描述：{0}", service.Description);
            Console.Write("状态：");
            if (service.IsInstalled() == null)
                Console.WriteLine("未知");
            else if (service.IsInstalled() == false)
                Console.WriteLine("未安装");
            else
            {
                if (service.IsRunning() == null)
                    Console.WriteLine("未知");
                else if (service.IsRunning() == false)
                    Console.WriteLine("未启动");
                else
                    Console.WriteLine("运行中");
            }

            var asm = AssemblyX.Create(Assembly.GetExecutingAssembly());
            Console.WriteLine();
            Console.WriteLine("核心：{0}", asm.Version);
            //Console.WriteLine("文件：{0}", asm.FileVersion);
            Console.WriteLine("发布：{0:yyyy-MM-dd HH:mm:ss}", asm.Compile);

            var asm2 = AssemblyX.Create(Assembly.GetEntryAssembly());
            if (asm2 != asm)
            {
                Console.WriteLine();
                Console.WriteLine("程序：{0}", asm2.Version);
                Console.WriteLine("发布：{0:yyyy-MM-dd HH:mm:ss}", asm2.Compile);
            }

            Console.ForegroundColor = color;
        }

        /// <summary>显示菜单</summary>
        protected virtual void ShowMenu()
        {
            var service = Instance as IAgentService;

            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine();
            Console.WriteLine("1 显示状态");

            if (service.IsInstalled() == true)
            {
                if (service.IsRunning() == true)
                {
                    Console.WriteLine("3 停止服务 -stop");
                }
                else
                {
                    Console.WriteLine("2 卸载服务 -u");

                    Console.WriteLine("3 启动服务 -start");
                }
            }
            else
            {
                Console.WriteLine("2 安装服务 -i");
            }

            if (service.IsRunning() != true)
            {
                Console.WriteLine("4 单步调试 -step");
                Console.WriteLine("5 循环调试 -run");

                var dic = Config.GetConfigByPrefix("XAgent.AttachServer.");
                if (dic != null && dic.Count > 0)
                {
                    var dic2 = new Dictionary<String, String>();
                    foreach (var item in dic2)
                    {
                        if (!item.Key.IsNullOrWhiteSpace() && !item.Value.IsNullOrWhiteSpace()) dic2.Add(item.Key, item.Value);
                    }
                    dic = dic2;

                    if (dic != null && dic.Count > 0)
                    {
                        Console.WriteLine("6 附加服务调试");
                        foreach (var item in dic)
                        {
                            Console.WriteLine("{0,10} = {1}", item.Key, item.Value);
                        }
                    }
                }
            }

            if (WatchDogs.Length > 0)
            {
                Console.WriteLine("7 看门狗保护服务 {0}", String.Join(",", WatchDogs));
            }

            Console.WriteLine("0 退出");

            Console.ForegroundColor = color;
        }
        #endregion

        #region 服务控制
        private Thread[] _Threads;
        /// <summary>线程组</summary>
        private Thread[] Threads { get { return _Threads ?? (_Threads = new Thread[ThreadCount]); } set { _Threads = value; } }

        private Dictionary<String, IServer> _AttachServers;
        /// <summary>附加服务</summary>
        public Dictionary<String, IServer> AttachServers { get { return _AttachServers ?? (_AttachServers = new Dictionary<string, IServer>()); } /*set { _AttachServers = value; }*/ }

        /// <summary>服务启动事件</summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            StartWork();

            // 处理附加服务
            try
            {
                StartAttachServers();
            }
            catch (Exception ex)
            {
                WriteLine(ex.ToString());
            }
        }

        /// <summary>服务停止事件</summary>
        protected override void OnStop()
        {
            StopWork();

            try
            {
                StopAttachServers();
            }
            catch (Exception ex)
            {
                WriteLine(ex.ToString());
            }
        }

        private void StartAttachServers()
        {
            var dic = Config.GetConfigByPrefix("XAgent.AttachServer.");
            if (dic != null && dic.Count > 0)
            {
                // 实例化
                foreach (var item in dic)
                {
                    if (!item.Key.IsNullOrWhiteSpace() && !item.Value.IsNullOrWhiteSpace())
                    {
                        WriteLine("");
                        WriteLine("正在加载：{0} = {1}", item.Key, item.Value);
                        var type = item.Value.GetTypeEx();
                        if (type != null)
                        {
                            var service = type.CreateInstance() as IServer;
                            if (service != null) AttachServers[item.Key] = service;
                        }
                    }
                }

                // 加载配置。【服务名.属性】的配置方式
                foreach (var item in AttachServers)
                {
                    if (item.Value != null)
                    {
                        var type = item.Value.GetType();
                        // 遍历所有属性，查找指定的设置项
                        foreach (var pi in type.GetProperties(true))
                        {
                            var name = String.Format("XAgent.{0}.{1}", item.Key, pi.Name);
                            Object value = null;
                            // 读取配置，并赋值
                            if (Config.TryGetConfig(name, pi.PropertyType, out value))
                            {
                                WriteLine("配置：{0} = {1}", name, value);
                                //PropertyInfoX.Create(pi).SetValue(item.Value, value);
                                item.Value.SetValue(pi, value);
                            }
                        }
                    }
                }

                // 启动
                foreach (var item in AttachServers)
                {
                    if (item.Value != null)
                    {
                        WriteLine("启动：{0}", item.Key);
                        item.Value.Start();
                    }
                }
            }
        }

        private void StopAttachServers()
        {
            if (AttachServers != null)
            {
                foreach (var item in AttachServers.Values)
                {
                    if (item != null) item.Stop();
                }
            }
        }

        /// <summary>销毁资源</summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (AttachServers != null)
            {
                //foreach (var item in AttachServers.Values)
                //{
                //    if (item != null && item is IDisposable) (item as IDisposable).Dispose();
                //}
                AttachServers.Values.TryDispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>开始循环工作</summary>
        public virtual void StartWork()
        {
            //依赖服务检测
            this.PreStartWork();

            WriteLine("服务启动");

            try
            {
                for (int i = 0; i < ThreadCount; i++)
                {
                    StartWork(i);
                }

                // 启动服务管理线程
                StartManagerThread();

                //// 显示用户界面交互窗体
                //Interactive.ShowForm();
            }
            catch (Exception ex)
            {
                WriteLine(ex.ToString());
            }
        }

        /// <summary>开始循环工作</summary>
        /// <param name="index">线程序号</param>
        public virtual void StartWork(Int32 index)
        {
            if (index < 0 || index >= ThreadCount) throw new ArgumentOutOfRangeException("index");

            // 可以通过设置任务的时间间隔小于0来关闭指定任务
            var ts = Setting.Current.Intervals.SplitAsInt();
            Int32 time = ts[0];
            // 使用专用的时间间隔
            if (index < ts.Length) time = ts[index];
            if (time < 0) return;

            Threads[index] = new Thread(workWaper);
            //String name = "XAgent_" + index;
            String name = "A" + index;
            if (ThreadNames != null && ThreadNames.Length > index && !String.IsNullOrEmpty(ThreadNames[index]))
                name = ThreadNames[index];
            Threads[index].Name = name;
            Threads[index].IsBackground = true;
            Threads[index].Priority = ThreadPriority.AboveNormal;
            Threads[index].Start(index);
        }

        /// <summary>线程包装</summary>
        /// <param name="data">线程序号</param>
        private void workWaper(Object data)
        {
            Int32 index = (Int32)data;

            // 旧异常
            Exception oldEx = null;

            while (true)
            {
                Boolean isContinute = false;
                Active[index] = DateTime.Now;

                try
                {
                    isContinute = Work(index);

                    oldEx = null;
                }
                catch (ThreadAbortException) //线程被取消
                {
                    WriteLine("线程" + index + "被取消！");
                    break;
                }
                catch (ThreadInterruptedException) //线程中断错误
                {
                    WriteLine("线程" + index + "中断错误！");
                    break;
                }
                catch (Exception ex) //确保拦截了所有的异常，保证服务稳定运行
                {
                    // 避免同样的异常信息连续出现，造成日志膨胀
                    if (oldEx == null || oldEx.GetType() != ex.GetType() || oldEx.Message != ex.Message)
                    {
                        oldEx = ex;

                        WriteLine(ex.ToString());
                    }
                }
                Active[index] = DateTime.Now;

                //检查服务是否正在重启
                if (IsShutdowning)
                {
                    WriteLine("服务准备重启，" + Thread.CurrentThread.Name + "退出！");
                    break;
                }

                var ts = Setting.Current.Intervals.SplitAsInt();
                Int32 time = ts[0];
                //使用专用的时间间隔
                if (index < ts.Length) time = ts[index];

                //如果有数据库连接错误，则将等待间隔放大十倍
                //if (hasdberr) time *= 10;
                if (oldEx != null) time *= 10;

                if (!isContinute) Thread.Sleep(time * 1000);
            }
        }

        /// <summary>核心工作方法。调度线程会定期调用该方法</summary>
        /// <param name="index">线程序号</param>
        /// <returns>是否立即开始下一步工作。某些任务能达到满负荷，线程可以不做等待</returns>
        public virtual Boolean Work(Int32 index) { return false; }

        /// <summary>停止循环工作</summary>
        /// <remarks>
        /// 只能停止循环而已，如果已经有一批任务在处理，
        /// 则内部需要捕获ThreadAbortException异常，否则无法停止任务处理。
        /// </remarks>
        public virtual void StopWork()
        {
            WriteLine("服务停止");

            //停止服务管理线程
            StopManagerThread();

            //if (threads != null && threads.IsAlive) threads.Abort();
            if (Threads != null)
            {
                foreach (var item in Threads)
                {
                    try
                    {
                        if (item != null && item.IsAlive) item.Abort();
                    }
                    catch (Exception ex)
                    {
                        WriteLine(ex.ToString());
                    }
                }
            }

            //Interactive.Hide();
        }

        /// <summary>停止循环工作</summary>
        /// <param name="index">线程序号</param>
        public virtual void StopWork(Int32 index)
        {
            if (index < 0 || index >= ThreadCount) throw new ArgumentOutOfRangeException("index");

            var item = Threads[index];
            try
            {
                if (item != null && item.IsAlive) item.Abort();
            }
            catch (Exception ex)
            {
                WriteLine(ex.ToString());
            }
        }
        #endregion

        #region 服务维护线程
        /// <summary>服务管理线程</summary>
        private Thread ManagerThread;

        /// <summary>开始服务管理线程</summary>
        public void StartManagerThread()
        {
            ManagerThread = new Thread(ManagerThreadWaper);
            //ManagerThread.Name = "XAgent_Manager";
            ManagerThread.Name = "AM";
            ManagerThread.IsBackground = true;
            ManagerThread.Priority = ThreadPriority.Highest;
            ManagerThread.Start();
        }

        /// <summary>停止服务管理线程</summary>
        public void StopManagerThread()
        {
            if (ManagerThread == null) return;
            if (ManagerThread.IsAlive)
            {
                try
                {
                    ManagerThread.Abort();
                }
                catch (Exception ex)
                {
                    WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>服务管理线程封装</summary>
        /// <param name="data"></param>
        protected virtual void ManagerThreadWaper(Object data)
        {
            while (true)
            {
                try
                {
                    CheckActive();

                    //如果某一项检查需要重启服务，则返回true，这里跳出循环，等待服务重启
                    if (CheckMemory()) break;
                    if (CheckThread()) break;
                    if (CheckAutoRestart()) break;

                    // 检查看门狗
                    //CheckWatchDog();
                    if (WatchDogs.Length > 0) Task.Factory.StartNew(CheckWatchDog);

                    Thread.Sleep(60 * 1000);
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    WriteLine(ex.ToString());
                }
            }
        }

        private DateTime[] _Active;
        /// <summary>活动时间</summary>
        public DateTime[] Active
        {
            get
            {
                if (_Active == null)
                {
                    _Active = new DateTime[ThreadCount];
                    for (int i = 0; i < ThreadCount; i++)
                    {
                        _Active[i] = DateTime.Now;
                    }
                }
                return _Active;
            }
            set { _Active = value; }
        }

        /// <summary>检查是否有工作线程死亡</summary>
        protected virtual void CheckActive()
        {
            if (Threads == null || Threads.Length < 1) return;

            //检查已经停止了的工作线程
            for (int i = 0; i < ThreadCount; i++)
            {
                if (Threads[i] != null && !Threads[i].IsAlive)
                {
                    WriteLine(Threads[i].Name + "处于停止状态，准备重新启动！");

                    StartWork(i);
                }
            }

            //是否检查最大活动时间
            var max = Setting.Current.MaxActive;
            if (max <= 0) return;

            for (int i = 0; i < ThreadCount; i++)
            {
                var ts = DateTime.Now - Active[i];
                if (ts.TotalSeconds > max)
                {
                    WriteLine(Threads[i].Name + "已经" + ts.TotalSeconds + "秒没有活动了，准备重新启动！");

                    StopWork(i);
                    //等待线程结束
                    Threads[i].Join(5000);
                    StartWork(i);
                }
            }
        }

        /// <summary>检查内存是否超标</summary>
        /// <returns>是否超标重启</returns>
        protected virtual Boolean CheckMemory()
        {
            var max = Setting.Current.MaxMemory;
            if (max <= 0) return false;

            var p = Process.GetCurrentProcess();
            long cur = p.WorkingSet64 + p.PrivateMemorySize64;
            cur = cur / 1024 / 1024;
            if (cur > max)
            {
                WriteLine("当前进程占用内存" + cur + "M，超过阀值" + max + "M，准备重新启动！");

                Restart();

                return true;
            }

            return false;
        }

        /// <summary>检查服务进程的总线程数是否超标</summary>
        /// <returns></returns>
        protected virtual Boolean CheckThread()
        {
            var max = Setting.Current.MaxThread;
            if (max <= 0) return false;

            var p = Process.GetCurrentProcess();
            if (p.Threads.Count > max)
            {
                WriteLine("当前进程总线程" + p.Threads.Count + "个，超过阀值" + max + "个，准备重新启动！");

                Restart();

                return true;
            }

            return false;
        }

        /// <summary>服务开始时间</summary>
        private DateTime Start = DateTime.Now;

        /// <summary>检查自动重启</summary>
        /// <returns></returns>
        protected virtual Boolean CheckAutoRestart()
        {
            var f = Setting.Current.AutoRestart;
            if (f <= 0) return false;

            var ts = DateTime.Now - Start;
            if (ts.TotalMinutes > f)
            {
                WriteLine("服务已运行" + ts.TotalMinutes + "分钟，达到预设重启时间（" + f + "分钟），准备重启！");

                Restart();

                return true;
            }

            return false;
        }

        /// <summary>是否正在重启</summary>
        private Boolean IsShutdowning = false;

        /// <summary>重启服务</summary>
        public void Restart()
        {
            WriteLine("重启服务！");

            //在临时目录生成重启服务的批处理文件
            String filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "重启.bat");
            if (File.Exists(filename)) File.Delete(filename);

            File.AppendAllText(filename, "net stop " + ServiceName);
            File.AppendAllText(filename, Environment.NewLine);
            File.AppendAllText(filename, "ping 127.0.0.1 -n 5 > nul ");
            File.AppendAllText(filename, Environment.NewLine);
            File.AppendAllText(filename, "net start " + ServiceName);

            //准备重启服务，等待所有工作线程返回
            IsShutdowning = true;
            for (int i = 0; i < 10; i++)
            {
                Boolean b = false;
                foreach (Thread item in Threads)
                {
                    if (item.IsAlive)
                    {
                        b = true;
                        break;
                    }
                }
                if (!b) break;
                Thread.Sleep(1000);
            }

            //执行重启服务的批处理
            //RunCmd(filename, false, false);
            var p = new Process();
            var si = new ProcessStartInfo();
            si.FileName = filename;
            si.UseShellExecute = true;
            si.CreateNoWindow = true;
            p.StartInfo = si;

            p.Start();

            //if (File.Exists(filename)) File.Delete(filename);
        }
        #endregion

        #region 看门狗
        private static String[] _WatchDogs;
        /// <summary>看门狗要保护的服务</summary>
        public static String[] WatchDogs
        {
            get
            {
                if (_WatchDogs == null)
                {
                    //_WatchDogs = Config.GetConfigSplit<String>("XAgent.WatchLog", null);
                    _WatchDogs = Setting.Current.WatchDog != null ? Setting.Current.WatchDog.Split() : new String[0];
                }
                return _WatchDogs;
            }
        }

        /// <summary>检查看门狗。</summary>
        /// <remarks>
        /// XAgent看门狗功能由管理线程完成，每分钟一次。
        /// 检查指定的任务是否已经停止，如果已经停止，则启动它。
        /// </remarks>
        public static void CheckWatchDog()
        {
            //var ss = Config.GetConfigSplit<String>("XAgent.WatchLog", null);
            var ss = WatchDogs;
            if (ss == null || ss.Length < 1) return;

            foreach (var item in ss)
            {
                // 注意：IsServiceRunning返回三种状态，null表示未知
                if (ServiceHelper.IsServiceRunning(item) == false)
                {
                    WriteLine("发现服务{0}被关闭，准备启动！", item);

                    ServiceHelper.RunCmd("net start " + item, false, true);
                }
            }
        }
        #endregion
    }
}