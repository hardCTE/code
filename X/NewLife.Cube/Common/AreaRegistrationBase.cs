﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.WebPages;
using NewLife.Cube.Precompiled;
using NewLife.Log;
using NewLife.Reflection;
using XCode.Membership;

namespace NewLife.Cube
{
    /// <summary>区域注册基类</summary>
    /// <remarks>
    /// 提供以下功能：
    /// 1，区域名称。从类名中截取。其中DisplayName特性作为菜单中文名。
    /// 2，静态构造注册一次视图引擎、绑定提供者、过滤器
    /// 3，注册区域默认路由
    /// </remarks>
    public abstract class AreaRegistrationBase : AreaRegistration
    {
        /// <summary>区域名称</summary>
        public override String AreaName { get; }

        /// <summary>预编译引擎集合。便于外部设置属性</summary>
        public static PrecompiledViewAssembly[] PrecompiledEngines { get; private set; }

        /// <summary>实例化区域注册</summary>
        public AreaRegistrationBase()
        {
            AreaName = GetType().Name.TrimEnd("AreaRegistration");
        }

        static AreaRegistrationBase()
        {
            XTrace.WriteLine("{0} Start 初始化魔方 {0}", new String('=', 32));

            //// 注册视图引擎
            //RazorViewEngineX.Register(ViewEngines.Engines);

            // 遍历所有引用了AreaRegistrationBase的程序集
            var list = new List<PrecompiledViewAssembly>();
            foreach (var asm in FindAllArea())
            {
                XTrace.WriteLine("注册区域视图程序集：{0}", asm.FullName);

                var pva = new PrecompiledViewAssembly(asm);
                list.Add(pva);
            }
            PrecompiledEngines = list.ToArray();

            var engine = new CompositePrecompiledMvcEngine(PrecompiledEngines);
            XTrace.WriteLine("注册复合预编译引擎，共有视图程序集{0}个", list.Count);
            //ViewEngines.Engines.Insert(0, engine);
            // 预编译引擎滞后，让其它引擎先工作
            ViewEngines.Engines.Add(engine);

            // StartPage lookups are done by WebPages. 
            VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);

            // 注册绑定提供者
            EntityModelBinderProvider.Register();

            // 注册过滤器
            XTrace.WriteLine("注册过滤器：{0}", typeof(MvcHandleErrorAttribute).FullName);
            XTrace.WriteLine("注册过滤器：{0}", typeof(EntityAuthorizeAttribute).FullName);
            var filters = GlobalFilters.Filters;
            filters.Add(new MvcHandleErrorAttribute());
            filters.Add(new EntityAuthorizeAttribute() { IsGlobal = true });

            // 从数据库或者资源文件加载模版页面的例子
            //HostingEnvironment.RegisterVirtualPathProvider(new ViewPathProvider());

            //var routes = RouteTable.Routes;
            //routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Virtual",
            //    url: "{*viewName}",
            //    defaults: new { controller = "Frontend", action = "Default" },
            //    constraints: new { controller = "Frontend", action = "Default" }
            //);

            XTrace.WriteLine("{0} End   初始化魔方 {0}", new String('=', 32));
        }

        /// <summary>遍历所有引用了AreaRegistrationBase的程序集</summary>
        /// <returns></returns>
        static List<Assembly> FindAllArea()
        {
            var list = new List<Assembly>();
            foreach (var item in typeof(AreaRegistrationBase).GetAllSubclasses(true))
            {
                var asm = item.Assembly;
                if (!list.Contains(asm))
                {
                    list.Add(asm);
                    //yield return asm;
                }
            }

            // 为了能够实现模板覆盖，程序集相互引用需要排序，父程序集在前
            list.Sort((x, y) =>
            {
                if (x == y) return 0;
                if (x != null && y == null) return 1;
                if (x == null && y != null) return -1;

                return x.GetReferencedAssemblies().Any(e => e.FullName == y.FullName) ? 1 : -1;
            });

            return list;
        }

        /// <summary>注册区域</summary>
        /// <param name="context"></param>
        public override void RegisterArea(AreaRegistrationContext context)
        {
            XTrace.WriteLine("开始注册权限管理区域[{0}]", AreaName);

            // 注册本区域默认路由
            context.MapRoute(
                AreaName,
                AreaName + "/{controller}/{action}/{id}",
                new { controller = "Index", action = "Index", id = UrlParameter.Optional },
                new[] { GetType().Namespace + ".Controllers" }
            );

            // 所有已存在文件的请求都交给Mvc处理，比如Admin目录
            //routes.RouteExistingFiles = true;

            // 自动检查并添加菜单
            Task.Factory.StartNew(ScanController).LogException();
        }

        /// <summary>自动扫描控制器，并添加到菜单</summary>
        /// <remarks>默认操作当前注册区域的下一级Controllers命名空间</remarks>
        protected virtual void ScanController()
        {
            //// 延迟几秒钟等其它地方初始化完成
            //Thread.Sleep(3000);
            var mf = ManageProvider.Menu;
            if (mf == null) return;

            XTrace.WriteLine("初始化[{0}]的菜单体系", AreaName);
            mf.ScanController(AreaName, GetType().Assembly, GetType().Namespace + ".Controllers");

            // 更新区域名称为友好中文名
            var menu = mf.Root.FindByPath(AreaName);
            if (menu != null && menu.DisplayName.IsNullOrEmpty())
            {
                var dis = GetType().GetDisplayName();
                var des = GetType().GetDescription();

                if (!dis.IsNullOrEmpty()) menu.DisplayName = dis;
                if (!des.IsNullOrEmpty()) menu.Remark = des;

                menu.Save();
            }
        }
    }
}