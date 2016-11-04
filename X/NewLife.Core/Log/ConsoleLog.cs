﻿using System;
using System.Collections.Generic;

namespace NewLife.Log
{
    /// <summary>控制台输出日志</summary>
    public class ConsoleLog : Logger
    {
        private Boolean _UseColor = true;
        /// <summary>是否使用多种颜色，默认使用</summary>
        public Boolean UseColor { get { return _UseColor; } set { _UseColor = value; } }

        /// <summary>写日志</summary>
        /// <param name="level"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        protected override void OnWrite(LogLevel level, String format, params Object[] args)
        {
            var e = WriteLogEventArgs.Current.Set(level).Set(Format(format, args), null);

            if (!UseColor)
            {
                ConsoleWriteLog(e);
                return;
            }
#if !__MOBILE__
            var cc = Console.ForegroundColor;
            switch (level)
            {
                case LogLevel.Warn:
                    cc = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                case LogLevel.Fatal:
                    cc = ConsoleColor.Red;
                    break;
                default:
                    cc = GetColor(e.ThreadID);
                    break;
            }

            var old = Console.ForegroundColor;
            Console.ForegroundColor = cc;
#endif
            ConsoleWriteLog(e);
#if !__MOBILE__
            Console.ForegroundColor = old;
#endif
        }

        private void ConsoleWriteLog(WriteLogEventArgs e)
        {
            var msg = e.ToString();
            Console.WriteLine(msg);
        }

        static Dictionary<Int32, ConsoleColor> dic = new Dictionary<Int32, ConsoleColor>();
        static ConsoleColor[] colors = new ConsoleColor[] {
            ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Magenta, ConsoleColor.White, ConsoleColor.Yellow,
            ConsoleColor.DarkGreen, ConsoleColor.DarkCyan, ConsoleColor.DarkMagenta, ConsoleColor.DarkRed, ConsoleColor.DarkYellow };
        private ConsoleColor GetColor(Int32 threadid)
        {
            if (threadid == 1) return ConsoleColor.Gray;

            // 好像因为dic.TryGetValue也会引发线程冲突，真是悲剧！
            lock (dic)
            {
                ConsoleColor cc;
                var key = threadid;
                if (!dic.TryGetValue(key, out cc))
                {
                    //lock (dic)
                    {
                        //if (!dic.TryGetValue(key, out cc))
                        {
                            cc = colors[dic.Count % colors.Length];
                            dic[key] = cc;
                        }
                    }
                }

                return cc;
            }
        }

        /// <summary>已重载。</summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0} UseColor={1}", GetType().Name, UseColor);
        }
    }
}