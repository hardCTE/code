﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NewLife.Log;
using NewLife.Reflection;
using NewLife.Threading;
using XCode.DataAccessLayer;

namespace XCode.Cache
{
    /// <summary>单对象缓存</summary>
    /// <remarks>
    /// 用一个值为实体的字典作为缓存（键一般就是主键），适用于单表大量互相没有关系的数据。
    /// 同时，AutoSave能够让缓存项在过期时自动保存数据，该特性特别适用于点击计数等场合。
    /// </remarks>
    /// <typeparam name="TKey">键值类型</typeparam>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public class SingleEntityCache<TKey, TEntity> : CacheBase<TEntity>, ISingleEntityCache<TKey, TEntity> where TEntity : Entity<TEntity>, new()
    {
        #region 属性
        /// <summary>过期时间。单位是秒，默认60秒</summary>
        public Int32 Expire { get; set; }

        /// <summary>最大实体数。默认10000</summary>
        public Int32 MaxEntity { get; set; }

        /// <summary>缓存到期时自动保存，默认true</summary>
        public Boolean AutoSave { get; set; }

        /// <summary>允许缓存空对象，默认false</summary>
        public Boolean AllowNull { get; set; }

        #region 主键
        /// <summary>获取缓存主键的方法，默认方法为获取实体主键值</summary>
        public Func<TEntity, TKey> GetKeyMethod { get; set; }

        /// <summary>查找数据的方法</summary>
        public Func<TKey, TEntity> FindKeyMethod { get; set; }
        #endregion

        #region 从键
        /// <summary>从键是否区分大小写</summary>
        public Boolean SlaveKeyIgnoreCase { get; set; }

        /// <summary>根据从键查找数据的方法</summary>
        public Func<String, TEntity> FindSlaveKeyMethod { get; set; }

        /// <summary>获取缓存从键的方法，默认为空</summary>
        public Func<TEntity, String> GetSlaveKeyMethod { get; set; }
        #endregion

        /// <summary>初始化缓存的方法，默认为空</summary>
        public Action InitializeMethod { get; set; }

        /// <summary>在数据修改时保持缓存，不再过期，独占数据库时默认打开，否则默认关闭</summary>
        /// <remarks>独占模式也需要用到定时器，否则无法自动保存</remarks>
        public Boolean HoldCache { get; set; }

        /// <summary>是否在使用缓存</summary>
        public Boolean Using { get; set; }
        #endregion

        #region 构造、检查过期缓存
        TimerX _Timer = null;

        /// <summary>实例化一个实体缓存</summary>
        public SingleEntityCache()
        {
            Expire = Setting.Current.Cache.SingleCacheExpire;
            MaxEntity = 10000;
            AutoSave = true;

            var fi = Entity<TEntity>.Meta.Unique;
            if (fi != null) GetKeyMethod = entity => (TKey)entity[Entity<TEntity>.Meta.Unique.Name];
            FindKeyMethod = key => Entity<TEntity>.FindByKey(key);

            SlaveKeyIgnoreCase = false;
            HoldCache = Setting.Current.Cache.Alone;

            // 启动一个定时器，用于定时清理过期缓存。因为比较耗时，最后一个参数采用线程池
            _Timer = new TimerX(CheckExpire, null, Expire * 1000, Expire * 1000);
            _Timer.Async = true;
        }

        /// <summary>子类重载实现资源释放逻辑时必须首先调用基类方法</summary>
        /// <param name="disposing">从Dispose调用（释放所有资源）还是析构函数调用（释放非托管资源）。
        /// 因为该方法只会被调用一次，所以该参数的意义不太大。</param>
        protected override void OnDispose(bool disposing)
        {
            base.OnDispose(disposing);

            try
            {
                Clear("资源释放");
            }
            catch (Exception ex)
            {
                XTrace.WriteException(ex);
            }

            if (_Timer != null) _Timer.Dispose();
        }

        /// <summary>定期检查实体，如果过期，则触发保存</summary>
        void CheckExpire(Object state)
        {
            var hold = HoldCache;
            // 独占缓存不删除缓存，仅判断自动保存
            if (hold && !AutoSave) return;

            // 加锁后把缓存集合拷贝到数组中，避免后面遍历的时候出现多线程冲突
            CacheItem[] cs = null;
            if (Entities.Count <= 0) return;
            lock (Entities)
            {
                if (Entities.Count <= 0) return;

                cs = new CacheItem[Entities.Count];
                Entities.Values.CopyTo(cs, 0);
            }

            if (cs == null || cs.Length < 0) return;

            var list = new List<CacheItem>();
            foreach (var item in cs)
            {
                // 是否过期
                // 单对象缓存每次缓存的时候，设定一个将来的过期时间，然后以后只需要比较过期时间和当前时间就可以了
                if (item.Expired)
                {
                    if (item.Entity != null)
                    {
                        // 自动保存
                        if (AutoSave && item.NextSave <= DateTime.Now)
                        {
                            // 捕获异常，不影响别人
                            try
                            {
                                // 需要在原连接名表名里面更新对象
                                AutoUpdate(item, "定时检查过期");
                            }
                            catch { }
                        }
                        if (!hold) item.Entity = null;
                    }
                    if (!hold) list.Add(item);
                }
            }
            // 独占缓存不删除缓存
            if (hold) return;

            // 从缓存中删除，必须加锁
            if (list.Count > 0)
            {
                lock (Entities)
                {
                    foreach (var item in list)
                    {
                        if (Entities.ContainsKey(item.Key))
                        {
                            Entities.Remove(item.Key);
                            if (Debug) DAL.WriteLog("定时检查，删除超时Key={0}", item.Key);
                        }
                    }

                    //Using = Entities.Count > 0;
                }
                if (SlaveEntities.Count > 0)
                {
                    lock (SlaveEntities)
                    {
                        foreach (var item in list)
                        {
                            if (!item.SlaveKey.IsNullOrWhiteSpace()) SlaveEntities.Remove(item.SlaveKey);
                        }
                    }
                }
            }
        }
        #endregion

        #region 缓存对象
        /// <summary>缓存对象</summary>
        class CacheItem
        {
            public SingleEntityCache<TKey, TEntity> sc;

            /// <summary>键</summary>
            public TKey Key;

            /// <summary>从键</summary>
            public String SlaveKey;

            /// <summary>实体</summary>
            public TEntity Entity;

            private DateTime _ExpireTime;
            /// <summary>缓存过期时间</summary>
            public DateTime ExpireTime { get { return _ExpireTime; } set { _ExpireTime = value; NextSave = value; } }

            /// <summary>下一次保存的时间</summary>
            public DateTime NextSave;

            /// <summary>是否已经过期</summary>
            public Boolean Expired { get { return ExpireTime <= DateTime.Now; } }

            public void SetEntity(TEntity entity)
            {
                // 如果原来有对象，则需要自动保存
                if (Entity != null && Entity != entity) sc.AutoUpdate(this, "设置新的缓存对象");

                Entity = entity;
                ExpireTime = DateTime.Now.AddSeconds(sc.Expire);
            }
        }
        #endregion

        #region 单对象缓存
        private Object _SyncRoot = new Object();

        /// <summary>单对象缓存</summary>
        private Dictionary<TKey, CacheItem> Entities = new Dictionary<TKey, CacheItem>();

        private Dictionary<String, CacheItem> _SlaveEntities;
        /// <summary>单对象缓存，从键查询使用</summary>
        private Dictionary<String, CacheItem> SlaveEntities
        {
            get
            {
                if (_SlaveEntities == null)
                {
                    Dictionary<String, CacheItem> dic;
                    if (SlaveKeyIgnoreCase)
                    {
                        dic = new Dictionary<String, CacheItem>(StringComparer.OrdinalIgnoreCase);
                    }
                    else
                    {
                        dic = new Dictionary<String, CacheItem>();
                    }
                    if (Interlocked.CompareExchange<Dictionary<String, CacheItem>>(ref _SlaveEntities, dic, null) != null)
                    {
                        dic = null;
                    }
                }
                return _SlaveEntities;
            }
        }
        #endregion

        #region 统计
        /// <summary>总次数</summary>
        public Int32 Total;

        /// <summary>命中</summary>
        public Int32 Shoot;

        /// <summary>第一次命中，加锁之前</summary>
        public Int32 Shoot1;

        /// <summary>第二次命中，加锁之后</summary>
        public Int32 Shoot2;

        /// <summary>无效次数，不允许空但是查到对象又为空</summary>
        public Int32 Invalid;

        /// <summary>下一次显示时间</summary>
        public DateTime NextShow;

        /// <summary>显示统计信息</summary>
        public void ShowStatics()
        {
            if (Total > 0)
            {
                var sb = new StringBuilder();
                var name = "<{0},{1}>({2})".F(typeof(TKey).Name, typeof(TEntity).Name, Entities.Count);
                sb.AppendFormat("单对象缓存{0,-20}", name);
                sb.AppendFormat("总次数{0,7:n0}", Total);
                if (Shoot > 0) sb.AppendFormat("，命中{0,7:n0}（{1,6:P02}）", Shoot, (Double)Shoot / Total);
                // 一级命中和总命中相等时不显示
                if (Shoot1 > 0 && Shoot1 != Shoot) sb.AppendFormat("，一级命中{0,7:n0}（{1,6:P02}）", Shoot1, (Double)Shoot1 / Total);
                if (Shoot2 > 0) sb.AppendFormat("，二级命中{0}（{1,6:P02}）", Shoot2, (Double)Shoot2 / Total);
                if (Invalid > 0) sb.AppendFormat("，无效次数{0}（{1,6:P02}）", Invalid, (Double)Invalid / Total);

                XTrace.WriteLine(sb.ToString());
            }
        }
        #endregion

        #region 获取数据
        /// <summary>根据主键获取实体数据</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TEntity this[TKey key] { get { return GetItem(key); } set { Add(key, value); } }

        private TEntity GetItem(TKey key) { return GetItem(Entities, key); }

        private TEntity GetItem<TKey2>(IDictionary<TKey2, CacheItem> dic, TKey2 key)
        {
            // 为空的key，直接返回null，不进行缓存查找
            if (key == null) return null;

            // 更新统计信息
            XCache.CheckShowStatics(ref NextShow, ref Total, ShowStatics);

            // 如果找到项，返回
            CacheItem item = null;
            // 如果TryGetValue获取成功，item为空说明同一时间别的线程已做删除操作
            if (dic.TryGetValue(key, out item) && item != null)
            {
                Interlocked.Increment(ref Shoot1);
                // 下面的GetData里会判断过期并处理
                return GetData(item);
            }

            ClearUp();

            lock (_SyncRoot)
            {
                // 再次尝试获取
                if (dic.TryGetValue(key, out item) && item != null)
                {
                    Interlocked.Increment(ref Shoot2);
                    return GetData(item);
                }

                // 开始更新数据，然后加入缓存
                TEntity entity = null;
                if (dic == Entities)
                    entity = Invoke(FindKeyMethod, (TKey)(Object)key);
                else
                    entity = Invoke(FindSlaveKeyMethod, key + "");

                if (entity == null && !AllowNull)
                    Interlocked.Increment(ref Invalid);
                else
                    TryAdd(entity);

                return entity;
            }
        }

        /// <summary>尝试向两个字典加入数据</summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private Boolean TryAdd(TEntity entity)
        {
            if (!Using)
            {
                Using = true;
                if (Debug) DAL.WriteLog("单对象缓存首次使用 {0} {1}", typeof(TEntity).FullName, XTrace.GetCaller(1, 16));
            }

            var item = new CacheItem { sc = this };

            String slaveKey = null;
            if (GetSlaveKeyMethod != null) slaveKey = GetSlaveKeyMethod(entity);

            var mkey = GetKeyMethod(entity);
            item.Key = mkey;
            item.SlaveKey = slaveKey;
            item.SetEntity(entity);

            var success = false;
            lock (Entities)
            {
                if (!Entities.ContainsKey(mkey))
                {
                    Entities.Add(mkey, item);
                    success = true;
                }
            }
            if (success && !slaveKey.IsNullOrWhiteSpace())
            {
                lock (SlaveEntities)
                {
                    // 新增或更新
                    SlaveEntities[slaveKey] = item;
                }
            }
            return success;
        }

        /// <summary>根据从键获取实体数据</summary>
        /// <param name="slaveKey"></param>
        /// <returns></returns>
        public TEntity GetItemWithSlaveKey(String slaveKey) { return GetItem(SlaveEntities, slaveKey); }

        /// <summary>内部处理返回对象。
        /// 把对象传进来，而不是只传键值然后查找，是为了避免别的线程移除该项
        /// </summary>
        /// <remarks>此方法只做更新操作，不再进行缓存新增操作</remarks>
        /// <param name="item"></param>
        /// <returns></returns>
        private TEntity GetData(CacheItem item)
        {
            if (item == null) return null;

            // 未过期，直接返回
            //if (HoldCache || item.ExpireTime > DateTime.Now)
            // 这里不能判断独占缓存，否则将失去自动保存的机会
            if (!item.Expired)
            {
                Interlocked.Increment(ref Shoot);
                return item.Entity;
            }

            // 自动保存
            AutoUpdate(item, "获取缓存过期");

            // 判断别的线程是否已更新
            if (HoldCache || !item.Expired) return item.Entity;

            // 更新过期缓存，在原连接名表名里面获取
            var entity = Invoke(FindKeyMethod, item.Key);
            if (entity != null || AllowNull)
                item.SetEntity(entity);
            else
                Interlocked.Increment(ref Invalid);

            return entity;
        }

        /// <summary>清理缓存队列</summary>
        private void ClearUp()
        {
            var list = new List<CacheItem>();
            lock (Entities)
            {
                //队列满时，移除最老的一个
                while (Entities.Count >= MaxEntity)
                {
                    var first = RemoveFirst();
                    if (first != null && !first.SlaveKey.IsNullOrWhiteSpace()) { list.Add(first); }
                }
            }
            if (list.Count < 1) return;
            lock (SlaveEntities)
            {
                foreach (var item in list)
                {
                    SlaveEntities.Remove(item.SlaveKey);
                }
            }
        }

        /// <summary>移除第一个缓存项</summary>
        private CacheItem RemoveFirst()
        {
            var keyFirst = Entities.Keys.FirstOrDefault();
            if (keyFirst == null) return null;

            CacheItem item = null;
            if (!Entities.TryGetValue(keyFirst, out item)) return null;

            if (Debug) DAL.WriteLog("单实体缓存{0}超过最大数量限制{1}，准备移除第一项{2}", typeof(TEntity).FullName, MaxEntity, keyFirst);

            Entities.Remove(keyFirst);

            //自动保存
            if (item != null) AutoUpdate(item, "缓存达到最大数移除第一项");

            return item;
        }

        /// <summary>自动更新，最主要是在原连接名和表名里面更新对象</summary>
        /// <param name="item"></param>
        /// <param name="reason"></param>
        private void AutoUpdate(CacheItem item, String reason)
        {
            if (AutoSave && item != null && item.Entity != null)
            {
                item.NextSave = DateTime.Now.AddSeconds(Expire);
                Invoke<CacheItem, Object>(e =>
                {
                    var rs = e.Entity.Update();

                    if (Debug && rs > 0) DAL.WriteLog("单对象缓存AutoSave {0}/{1} {2}", Entity<TEntity>.Meta.TableName, Entity<TEntity>.Meta.ConnName, reason);

                    return null;
                }, item);
            }
        }
        #endregion

        #region 方法
        /// <summary>初始化单对象缓存，服务端启动时预载入实体记录集</summary>
        /// <remarks>注意事项：
        /// <para>调用方式：TEntity.Meta.Factory.Session.SingleCache.Initialize()，不要使用TEntity.Meta.Session.SingleCache.Initialize()；
        /// 因为Factory的调用会联级触发静态构造函数，确保单对象缓存设置成功</para>
        /// <para>服务端启动时，如果使用异步方式初始化单对象缓存，请将同一数据模型（ConnName）下的实体类型放在同一异步方法内执行，否则实体类型的架构检查抛异常</para>
        /// </remarks>
        public void Initialize()
        {
            if (HoldCache && InitializeMethod != null) { InitializeMethod(); }
        }

        /// <summary>是否包含指定键</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Boolean ContainsKey(TKey key) { return Entities.ContainsKey(key); }

        /// <summary>是否包含指定从键</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Boolean ContainsSlaveKey(String key) { return SlaveEntities.ContainsKey(key); }

        ///// <summary>尝试从单对象缓存中获取与指定的主键关联的实体对象</summary>
        ///// <param name="key">主键</param>
        ///// <param name="entity">返回的实体对象</param>
        ///// <returns>是否查找成功</returns>
        //internal Boolean TryGetItem(TKey key, out TEntity entity)
        //{
        //    CacheItem item;
        //    if (Entities.TryGetValue(key, out item) && item != null)
        //    {
        //        entity = item.Entity;
        //        return true;
        //    }
        //    else
        //    {
        //        entity = null;
        //        return false;
        //    }
        //}

        ///// <summary>更新一个实体对象到单对象缓存</summary>
        ///// <param name="entity"></param>
        //internal void Update(TEntity entity)
        //{
        //    var key = GetKeyMethod(entity);

        //    CacheItem item;
        //    if (Entities.TryGetValue(key, out item) && item != null)
        //        item.SetEntity(entity);
        //    else
        //        Add(entity);
        //}

        /// <summary>向单对象缓存添加项</summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        internal void Add(TEntity entity)
        {
            if (entity == null) return;

            //// 加入缓存的实体对象，需要标记来自数据库
            //entity.MarkDb(true);
            CacheItem item;
            if (Entities.TryGetValue(GetKeyMethod(entity), out item) && item != null)
                item.SetEntity(entity);
            else
                Add(GetKeyMethod(entity), entity);
        }

        /// <summary>向单对象缓存添加项</summary>
        /// <param name="key"></param>
        /// <param name="entity">数值</param>
        /// <returns></returns>
        Boolean Add(TKey key, TEntity entity)
        {
            // 如果找到项，返回
            CacheItem item = null;
            if (Entities.TryGetValue(key, out item) && item != null && !item.Expired) return false;

            // 加锁
            lock (_SyncRoot)
            {
                // 如果已存在并且过期，则复制
                // TryGetValue获取成功，Item为空说明同一时间另外线程做了删除操作
                if (Entities.TryGetValue(key, out item) && item != null)
                {
                    if (!item.Expired) return false;

                    item.SetEntity(entity);

                    return false;
                }
                else
                {
                    return TryAdd(entity);
                }
            }
        }

        /// <summary>移除指定项</summary>
        /// <param name="key">键值</param>
        /// <param name="save">是否自动保存实体对象</param>
        public void RemoveKey(TKey key, Boolean save = true)
        {
            CacheItem item = null;
            if (!Entities.TryGetValue(key, out item)) return;
            lock (Entities)
            {
                if (!Entities.TryGetValue(key, out item)) return;

                if (save) AutoUpdate(item, "移除缓存" + key);

                Entities.Remove(key);

                //Using = Entities.Count > 0;
            }
            if (item != null && !item.SlaveKey.IsNullOrWhiteSpace())
            {
                lock (SlaveEntities)
                {
                    SlaveEntities.Remove(item.SlaveKey);
                }
            }
        }

        /// <summary>根据主键移除指定项</summary>
        /// <param name="entity"></param>
        /// <param name="save">是否自动保存实体对象</param>
        public void Remove(TEntity entity, Boolean save = true)
        {
            if (entity == null) return;

            var key = GetKeyMethod(entity);
            RemoveKey(key, save);
        }

        /// <summary>清除所有数据</summary>
        /// <param name="reason">清除缓存原因</param>
        public void Clear(String reason = null)
        {
            if (Debug) DAL.WriteLog("清空单对象缓存：{0} 原因：{1} Using = false", typeof(TEntity).FullName, reason);

            if (AutoSave)
            {
                // 加锁处理自动保存
                lock (Entities)
                {
                    foreach (var key in Entities)
                    {
                        AutoUpdate(key.Value, "清空缓存 " + reason);
                    }
                }
            }

            lock (Entities)
            {
                Entities.Clear();
            }
            lock (SlaveEntities)
            {
                SlaveEntities.Clear();
            }

            Using = false;
        }
        #endregion

        #region ISingleEntityCache 成员
        /// <summary>获取数据</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IEntity ISingleEntityCache.this[object key] { get { return GetItem((TKey)key); } }

        /// <summary>根据从键获取实体数据</summary>
        /// <param name="slaveKey"></param>
        /// <returns></returns>
        IEntity ISingleEntityCache.GetItemWithSlaveKey(String slaveKey) { return GetItemWithSlaveKey(slaveKey); }

        /// <summary>是否包含指定主键</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Boolean ISingleEntityCache.ContainsKey(Object key) { return ContainsKey((TKey)key); }

        /// <summary>移除指定项</summary>
        /// <param name="key">键值</param>
        /// <param name="save">是否自动保存实体对象</param>
        void ISingleEntityCache.RemoveKey(Object key, Boolean save) { RemoveKey((TKey)key, save); }

        /// <summary>移除指定项</summary>
        /// <param name="entity"></param>
        /// <param name="save">是否自动保存实体对象</param>
        void ISingleEntityCache.Remove(IEntity entity, Boolean save) { Remove(entity as TEntity, save); }

        /// <summary>向单对象缓存添加项</summary>
        /// <param name="key"></param>
        /// <param name="value">实体对象</param>
        /// <returns></returns>
        Boolean ISingleEntityCache.Add(Object key, IEntity value)
        {
            var entity = value as TEntity;
            if (entity == null) { return false; }
            return Add((TKey)key, entity);
        }

        /// <summary>向单对象缓存添加项</summary>
        /// <param name="value">实体对象</param>
        /// <returns></returns>
        Boolean ISingleEntityCache.Add(IEntity value)
        {
            var entity = value as TEntity;
            if (entity == null) { return false; }
            var key = GetKeyMethod(entity);
            return Add(key, entity);
        }
        #endregion

        #region 辅助
        internal SingleEntityCache<TKey, TEntity> CopySettingFrom(ISingleEntityCache ec)
        {
            this.Expire = ec.Expire;
            this.MaxEntity = ec.MaxEntity;
            this.AutoSave = ec.AutoSave;
            this.AllowNull = ec.AllowNull;
            this.HoldCache = ec.HoldCache;

            //this.GetKeyMethod = ec.GetKeyMethod;
            //this.FindKeyMethod = ec.FindKeyMethod;

            //this.SlaveKeyIgnoreCase = ec.SlaveKeyIgnoreCase;
            //this.GetSlaveKeyMethod = ec.GetSlaveKeyMethod;
            //this.FindSlaveKeyMethod = ec.FindSlaveKeyMethod;

            //this.InitializeMethod = ec.InitializeMethod;

            return this;
        }

        /// <summary>已重载。</summary>
        /// <returns></returns>
        public override string ToString()
        {
            //return base.ToString();
            return "SingleEntityCache<{0}, {1}>[{2}]".F(typeof(TKey).Name, typeof(TEntity).Name, Entities.Count);
        }
        #endregion
    }
}