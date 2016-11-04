﻿using System;
using System.ComponentModel;
using System.IO;
using NewLife.Configuration;
using NewLife.Log;
using NewLife.Xml;

namespace NewLife
{
    /// <summary>核心设置</summary>
    [DisplayName("核心设置")]
#if !__MOBILE__
    [XmlConfigFile(@"Config\Core.config", 15000)]
#endif
    public class Setting : XmlConfig<Setting>
    {
        #region 属性
        /// <summary>是否启用全局调试。默认为不启用</summary>
        [Description("全局调试。XTrace.Debug")]
        public Boolean Debug { get; set; }

        /// <summary>日志等级，只输出大于等于该级别的日志</summary>
        [Description("日志等级，只输出大于等于该级别的日志")]
        public LogLevel LogLevel { get; set; }

        /// <summary>文件日志目录</summary>
        [Description("文件日志目录")]
        public String LogPath { get; set; }

        /// <summary>网络日志。本地子网日志广播255.255.255.255:514</summary>
        [Description("网络日志。本地子网日志广播255.255.255.255:514")]
        public String NetworkLog { get; set; }

        /// <summary>临时目录</summary>
        [Description("临时目录")]
        public String TempPath { get; set; }

        /// <summary>扩展插件存放目录</summary>
        [Description("扩展插件存放目录")]
        public String PluginPath { get; set; }

        /// <summary>扩展插件服务器。将从该网页上根据关键字分析链接并下载插件</summary>
        [Description("扩展插件服务器。将从该网页上根据关键字分析链接并下载插件")]
        public String PluginServer { get; set; }

        /// <summary>下载扩展插件的缓存目录。默认位于系统盘的X\Cache</summary>
        [Description("下载扩展插件的缓存目录。默认位于系统盘的X\\Cache")]
        public String PluginCache { get; set; }

        /// <summary>网络调试</summary>
        [Description("网络调试")]
        public Boolean NetDebug { get; set; }

        /// <summary>多线程调试</summary>
        [Description("多线程调试")]
        public Boolean ThreadDebug { get; set; }

        /// <summary>网页压缩文件</summary>
        [Description("网页压缩文件")]
        public String WebCompressFiles { get; set; }

        /// <summary>语音提示。默认true</summary>
        [Description("语音提示。默认true")]
        public Boolean SpeechTip { get; set; }
        #endregion

        #region 方法
        /// <summary>实例化</summary>
        public Setting()
        {
            Debug = true;
            SpeechTip = true;
        }

        /// <summary>新建时调用</summary>
        protected override void OnNew()
        {
            Debug = Config.GetConfig<Boolean>("NewLife.Debug", true);
            NetDebug = Config.GetConfig<Boolean>("NewLife.Net.Debug", false);
            LogLevel = Config.GetConfig<LogLevel>("NewLife.LogLevel", Debug ? LogLevel.Debug : LogLevel.Info);

            LogPath = Config.GetConfig<String>("NewLife.LogPath", Runtime.IsWeb ? "../Log" : "Log");
            TempPath = Config.GetConfig<String>("NewLife.TempPath", "XTemp");
            NetworkLog = "";
            //PluginServer = "http://x.newlifex.com/";
            PluginServer = "ftp://ftp.newlifex.com/x/";
            //PluginPath = Runtime.IsWeb ? "Bin" : "Plugins";
            PluginPath = "Plugins";
            ThreadDebug = Config.GetMutilConfig<Boolean>(false, "NewLife.Thread.Debug", "ThreadPoolDebug");
            WebCompressFiles = Config.GetMutilConfig<String>(".aspx,.axd,.js,.css", "NewLife.Web.CompressFiles", "NewLife.CommonEntity.CompressFiles");
        }

        /// <summary>加载完成后</summary>
        protected override void OnLoaded()
        {
#if !__MOBILE__
            if (PluginCache.IsNullOrWhiteSpace())
            {
                // 兼容Linux Mono
                var sys = Environment.SystemDirectory;
                if (sys.IsNullOrEmpty()) sys = "/";
                PluginCache = Path.GetPathRoot(sys).CombinePath("X", "Cache");
            }
#endif
            if (PluginServer.IsNullOrWhiteSpace()||PluginServer.EqualIgnoreCase("http://x.newlifex.com/")) PluginServer = "ftp://ftp.newlifex.com/x/";

            base.OnLoaded();
        }

        /// <summary>获取插件目录</summary>
        /// <returns></returns>
        public String GetPluginPath()
        {
            //if (Runtime.IsWeb)
            //    return "Bin".GetFullPath();
            //else
            return PluginPath.GetBasePath();
        }
        #endregion
    }
}