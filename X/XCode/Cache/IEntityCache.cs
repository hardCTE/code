﻿using System;

namespace XCode.Cache
{
    /// <summary>缓存基接口</summary>
    public interface IEntityCacheBase
    {
        /// <summary>连接名</summary>
        String ConnName { get; set; }

        /// <summary>表名</summary>
        String TableName { get; set; }
    }

    /// <summary>实体缓存接口</summary>
    public interface IEntityCache : IEntityCacheBase
    {
        /// <summary>实体集合。因为涉及一个转换，数据量大时很耗性能，建议不要使用。</summary>
        EntityList<IEntity> Entities { get; }

        /// <summary>根据指定项查找</summary>
        /// <param name="name">属性名</param>
        /// <param name="value">属性值</param>
        /// <returns></returns>
        IEntity Find(String name, Object value);

        /// <summary>根据指定项查找</summary>
        /// <param name="name">属性名</param>
        /// <param name="value">属性值</param>
        /// <returns></returns>
        EntityList<IEntity> FindAll(String name, Object value);

        /// <summary>检索与指定谓词定义的条件匹配的所有元素。</summary>
        /// <param name="match">条件</param>
        /// <returns></returns>
        EntityList<IEntity> FindAll(Predicate<IEntity> match);

        /// <summary>清除缓存</summary>
        void Clear(String reason = null);
    }

    /// <summary>单对象缓存接口</summary>
    public interface ISingleEntityCache : IEntityCacheBase
    {
        /// <summary>过期时间。单位是秒，默认60秒</summary>
        Int32 Expire { get; set; }

        /// <summary>最大实体数。默认10000</summary>
        Int32 MaxEntity { get; set; }

        /// <summary>缓存到期时自动保存，默认true</summary>
        Boolean AutoSave { get; set; }

        /// <summary>允许缓存空对象，默认false</summary>
        Boolean AllowNull { get; set; }

        /// <summary>在数据修改时保持缓存，不再过期，独占数据库时默认打开，否则默认关闭</summary>
        Boolean HoldCache { get; set; }

        /// <summary>是否在使用缓存</summary>
        Boolean Using { get; }

        /// <summary>获取数据</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IEntity this[Object key] { get; }

        /// <summary>根据从键获取实体数据</summary>
        /// <param name="slaveKey"></param>
        /// <returns></returns>
        IEntity GetItemWithSlaveKey(String slaveKey);

        /// <summary>初始化单对象缓存，服务端启动时预载入实体记录集</summary>
        /// <remarks>注意事项：
        /// <para>调用方式：TEntity.Meta.Factory.Session.SingleCache.Initialize()，不要使用TEntity.Meta.Session.SingleCache.Initialize()；
        /// 因为Factory的调用会联级触发静态构造函数，确保单对象缓存设置成功</para>
        /// <para>服务端启动时，如果使用异步方式初始化单对象缓存，请将同一数据模型（ConnName）下的实体类型放在同一异步方法内执行，否则实体类型的架构检查抛异常</para>
        /// </remarks>
        void Initialize();

        /// <summary>是否包含指定主键</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Boolean ContainsKey(Object key);

        /// <summary>是否包含指定从键</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Boolean ContainsSlaveKey(String key);

        /// <summary>向单对象缓存添加项</summary>
        /// <param name="key"></param>
        /// <param name="value">实体对象</param>
        /// <returns></returns>
        Boolean Add(Object key, IEntity value);

        /// <summary>向单对象缓存添加项</summary>
        /// <param name="value">实体对象</param>
        /// <returns></returns>
        Boolean Add(IEntity value);

        /// <summary>移除指定项</summary>
        /// <param name="entity"></param>
        /// <param name="save">是否自动保存实体对象</param>
        void Remove(IEntity entity, Boolean save);

        /// <summary>移除指定项</summary>
        /// <param name="key">键值</param>
        /// <param name="save">是否自动保存实体对象</param>
        void RemoveKey(Object key, Boolean save = true);

        /// <summary>清除所有数据</summary>
        /// <param name="reason">清除缓存原因</param>
        void Clear(String reason);
    }

    /// <summary></summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public interface ISingleEntityCache<TKey, TEntity> : ISingleEntityCache where TEntity : Entity<TEntity>, new()
    {
        /// <summary>获取数据</summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TEntity this[TKey key] { get; }

        /// <summary>获取缓存主键的方法，默认方法为获取实体主键值</summary>
        Func<TEntity, TKey> GetKeyMethod { get; set; }

        /// <summary>查找数据的方法</summary>
        Func<TKey, TEntity> FindKeyMethod { get; set; }
    }
}