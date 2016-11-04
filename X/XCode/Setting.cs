﻿using System;
using System.ComponentModel;
using NewLife;
using NewLife.Configuration;
using NewLife.Xml;
using XCode.Cache;
using XCode.DataAccessLayer;

namespace XCode
{
    /// <summary>XCode设置</summary>
    [DisplayName("XCode设置")]
    [XmlConfigFile(@"Config\XCode.config", 15000)]
    public class Setting : XmlConfig<Setting>
    {
        #region 属性
        /// <summary>是否启用调试。默认启用</summary>
        [Description("调试")]
        public Boolean Debug { get; set; }

        /// <summary>是否输出SQL语句，默认启用</summary>
        [Description("是否输出SQL语句，默认启用")]
        public Boolean ShowSQL { get; set; }

        /// <summary>设置SQL输出的单独目录，默认为空，SQL输出到当前日志中。生产环境建议输出到站点外单独的SqlLog目录</summary>
        [Description("设置SQL输出的单独目录，默认为空，SQL输出到当前日志中。生产环境建议输出到站点外单独的SqlLog目录")]
        public String SQLPath { get; set; }

        /// <summary>跟踪SQL执行时间，大于该阀值将输出日志，默认500毫秒</summary>
        [Description("跟踪SQL执行时间，大于该阀值将输出日志，默认500毫秒")]
        public Int32 TraceSQLTime { get; set; } = 500;

        /// <summary>连接名映射#，表名映射@，把实体类中的Test2和Test3连接名映射到Test去</summary>
        [Description("连接名映射#，表名映射@，把实体类中的Test2和Test3连接名映射到Test去")]
        public String ConnMaps { get; set; }

        /// <summary>是否启用动态代码调试，把动态生成的实体类代码和程序集输出到临时目录，默认不启用</summary>
        [Description("是否启用动态代码调试，把动态生成的实体类代码和程序集输出到临时目录，默认不启用")]
        public Boolean CodeDebug { get; set; }

        /// <summary>实体类首次访问数据库时，是否执行数据初始化，默认true执行，导数据时建议关闭</summary>
        [Description("实体类首次访问数据库时，是否执行数据初始化，默认true执行，导数据时建议关闭")]
        public Boolean InitData { get; set; }

        /// <summary>事务调试。打开时输出事务回滚日志，默认关闭</summary>
        [Description("事务调试。打开时输出事务回滚日志，默认关闭")]
        public Boolean TransactionDebug { get; set; }

        /// <summary>SQLite数据库默认目录。没有设置连接字符串的连接默认创建SQLite连接，数据库放在该目录</summary>
        [Description("SQLite数据库默认目录。没有设置连接字符串的连接默认创建SQLite连接，数据库放在该目录")]
        public String SQLiteDbPath { get; set; }

        /// <summary>缓存</summary>
        [Description("缓存")]
        public CacheSetting Cache { get; set; }

        /// <summary>反向工程</summary>
        [Description("反向工程")]
        public NegativeSetting Negative { get; set; }

        ///// <summary>模型</summary>
        //[Description("模型")]
        //public ModelSetting Model { get; set; }

        /// <summary>Oracle设置</summary>
        [Description("Oracle设置")]
        public OracleSetting Oracle { get; set; }
        #endregion

        #region 方法
        /// <summary>实例化设置</summary>
        public Setting()
        {
            Debug = true;
            ShowSQL = true;
            SQLPath = "";
            ConnMaps = "Conn2#Conn,Table3@Table";
            InitData = true;

            Cache = new CacheSetting();
            Negative = new NegativeSetting();
            //Model = new ModelSetting();
            Oracle = new OracleSetting();
        }

        /// <summary>新建时调用</summary>
        protected override void OnNew()
        {
            Debug = Config.GetConfig<Boolean>("XCode.Debug", true);
            ShowSQL = Config.GetConfig<Boolean>("XCode.ShowSQL", Debug);
            SQLPath = Config.GetConfig<String>("XCode.SQLPath");
            ConnMaps = Config.GetConfig<String>("XCode.ConnMaps");
            TraceSQLTime = Config.GetConfig<Int32>("XCode.TraceSQLTime");

            Cache.Init();
            Negative.Init();
        }

        /// <summary>加载后检查默认值</summary>
        protected override void OnLoaded()
        {
            var dbpath = SQLiteDbPath;
            if (dbpath.IsNullOrEmpty())
            {
                dbpath = ".";
                if (Runtime.IsWeb)
                {
                    if (!Environment.CurrentDirectory.Contains("iisexpress") ||
                        !Environment.CurrentDirectory.Contains("Web"))
                        dbpath = "..\\Data";
                    else
                        dbpath = "~\\App_Data";
                }
                SQLiteDbPath = dbpath;
            }

            base.OnLoaded();
        }
        #endregion
    }

    /// <summary>Oracle设置</summary>
    public class OracleSetting
    {
        #region 属性
        /// <summary>是否限制只能访问拥有者的信息，默认true</summary>
        [Description("是否限制只能访问拥有者的信息，默认true")]
        public Boolean UseOwner { get; set; }

        /// <summary>是否忽略大小写，如果不忽略则在表名字段名外面加上双引号，默认true</summary>
        [Description("是否忽略大小写，如果不忽略则在表名字段名外面加上双引号，默认true")]
        public Boolean IgnoreCase { get; set; }
        #endregion

        /// <summary>初始化</summary>
        public OracleSetting()
        {
            UseOwner = true;
            IgnoreCase = true;
        }
    }
}