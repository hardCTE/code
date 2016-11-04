﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using NewLife.Log;
using NewLife.Threading;

namespace NewLife.Net
{
    /// <summary>会话集合。带有自动清理不活动会话的功能</summary>
    class SessionCollection : DisposeBase, IDictionary<String, ISocketSession>
    {
        #region 属性
        Dictionary<String, ISocketSession> _dic = new Dictionary<String, ISocketSession>();

        /// <summary>服务端</summary>
        public ISocketServer Server { get; private set; }

        /// <summary>清理周期。单位毫秒，默认1000毫秒。</summary>
        public Int32 ClearPeriod { get; set; }

        /// <summary>清理会话计时器</summary>
        private TimerX clearTimer;
        #endregion

        #region 构造
        public SessionCollection(ISocketServer server)
        {
            Server = server;
            ClearPeriod = 1000;
        }

        protected override void OnDispose(bool disposing)
        {
            base.OnDispose(disposing);

            if (clearTimer != null) clearTimer.Dispose();

            CloseAll();
        }
        #endregion

        #region 主要方法
        /// <summary>添加新会话，并设置会话编号</summary>
        /// <param name="session"></param>
        /// <returns>返回添加新会话是否成功</returns>
        public Boolean Add(ISocketSession session)
        {
            // 估算完成时间，执行过长时提示
            using (var tc = new TimeCost("{0}.Add".F(GetType().Name), 100))
            {
                tc.Log = Server.Log;

                var key = session.Remote.EndPoint + "";
                if (_dic.ContainsKey(key)) return false;
                lock (_dic)
                {
                    if (_dic.ContainsKey(key)) return false;

                    session.OnDisposed += (s, e) => { lock (_dic) { _dic.Remove((s as ISocketSession).Remote.EndPoint + ""); } };
                    _dic.Add(key, session);

                    if (clearTimer == null) clearTimer = new TimerX(RemoveNotAlive, null, ClearPeriod, ClearPeriod);
                }
            }

            return true;
        }

        /// <summary>获取会话，加锁</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ISocketSession Get(String key)
        {
            // 估算完成时间，执行过长时提示
            using (var tc = new TimeCost("{0}.Get".F(GetType().Name), 100))
            {
                tc.Log = Server.Log;

                ISocketSession session = null;
                if (!_dic.TryGetValue(key, out session)) return null;

                // 加锁后获取，本方法平均执行时间143.48ns
                lock (_dic)
                {
                    if (!_dic.TryGetValue(key, out session)) return null;

                    return session;
                }
            }
        }

        /// <summary>关闭所有</summary>
        public void CloseAll()
        {
            if (_dic.Count < 1) return;

            // 必须先复制到数组，因为会话销毁的时候，会自动从集合中删除，从而引起集合枚举失败
            ISocketSession[] ns = null;
            lock (_dic)
            {
                if (_dic.Count < 1) return;

                //_dic.Values.CopyTo(ns, 0);
                ns = _dic.Values.ToArray();
                _dic.Clear();
            }
            foreach (var item in ns)
            {
                if (item != null && !item.Disposed) item.TryDispose();
            }
        }

        /// <summary>移除不活动的会话</summary>
        void RemoveNotAlive(Object state)
        {
            if (_dic.Count < 1) return;

            var timeout = 30;
            if (Server != null) timeout = Server.SessionTimeout;
            var keys = new List<String>();
            var values = new List<ISocketSession>();
            // 估算完成时间，执行过长时提示
            using (var tc = new TimeCost("{0}.RemoveNotAlive".F(GetType().Name), 100))
            {
                tc.Log = Server.Log;

                lock (_dic)
                {
                    if (_dic.Count < 1) return;

                    // 这里可能有问题，曾经见到，_list有元素，但是value为null，这里居然没有进行遍历而直接跳过
                    // 操作这个字典的时候，必须加锁，否则就会数据错乱，成了这个样子，无法枚举
                    foreach (var elm in _dic)
                    {
                        var item = elm.Value;
                        // 判断是否已超过最大不活跃时间
                        if (item == null || item.Disposed || timeout > 0 && IsNotAlive(item, timeout))
                        {
                            keys.Add(elm.Key);
                            values.Add(elm.Value);
                        }
                    }
                    // 从会话集合里删除这些键值，现在在锁内部，操作安全
                    foreach (var item in keys)
                    {
                        _dic.Remove(item);
                    }
                }
            }
            // 已经离开了锁，慢慢释放各个会话
            foreach (var item in values)
            {
                item.WriteLog("超过{0}秒不活跃销毁 {1}", timeout, item);

                //item.Dispose();
                item.TryDispose();
            }
        }

        Boolean IsNotAlive(ISocketSession session, Int32 timeout)
        {
            // 如果有最后时间则判断最后时间，否则判断开始时间
            var time = session.LastTime > DateTime.MinValue ? session.LastTime : session.StartTime;
            return time.AddSeconds(timeout) < DateTime.Now;
        }
        #endregion

        #region 成员
        public void Clear() { _dic.Clear(); }

        public int Count { get { return _dic.Count; } }

        public bool IsReadOnly { get { return (_dic as IDictionary<Int32, ISocketSession>).IsReadOnly; } }

        public IEnumerator<ISocketSession> GetEnumerator() { return _dic.Values.GetEnumerator() as IEnumerator<ISocketSession>; }

        IEnumerator IEnumerable.GetEnumerator() { return _dic.GetEnumerator(); }
        #endregion

        #region IDictionary<String,ISocketSession> 成员

        void IDictionary<String, ISocketSession>.Add(String key, ISocketSession value) { Add(value); }

        bool IDictionary<String, ISocketSession>.ContainsKey(String key) { return _dic.ContainsKey(key); }

        ICollection<String> IDictionary<String, ISocketSession>.Keys { get { return _dic.Keys; } }

        bool IDictionary<String, ISocketSession>.Remove(String key)
        {
            ISocketSession session;
            if (!_dic.TryGetValue(key, out session)) return false;

            //session.Close();
            session.Dispose();

            return _dic.Remove(key);
        }

        bool IDictionary<String, ISocketSession>.TryGetValue(String key, out ISocketSession value) { return _dic.TryGetValue(key, out value); }

        ICollection<ISocketSession> IDictionary<String, ISocketSession>.Values { get { return _dic.Values; } }

        ISocketSession IDictionary<String, ISocketSession>.this[String key] { get { return _dic[key]; } set { _dic[key] = value; } }

        #endregion

        #region ICollection<KeyValuePair<String,ISocketSession>> 成员

        void ICollection<KeyValuePair<String, ISocketSession>>.Add(KeyValuePair<String, ISocketSession> item)
        {
            throw new XException("不支持！请使用Add(ISocketSession session)方法！");
        }

        bool ICollection<KeyValuePair<String, ISocketSession>>.Contains(KeyValuePair<String, ISocketSession> item) { throw new NotImplementedException(); }

        void ICollection<KeyValuePair<String, ISocketSession>>.CopyTo(KeyValuePair<String, ISocketSession>[] array, int arrayIndex) { throw new NotImplementedException(); }

        bool ICollection<KeyValuePair<String, ISocketSession>>.Remove(KeyValuePair<String, ISocketSession> item) { throw new XException("不支持！请直接销毁会话对象！"); }

        #endregion

        #region IEnumerable<KeyValuePair<String,ISocketSession>> 成员
        IEnumerator<KeyValuePair<String, ISocketSession>> IEnumerable<KeyValuePair<String, ISocketSession>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}