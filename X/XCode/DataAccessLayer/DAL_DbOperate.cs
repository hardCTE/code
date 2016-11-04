﻿using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using NewLife.Collections;
using XCode.Cache;

namespace XCode.DataAccessLayer
{
    partial class DAL
    {
        #region 统计属性
        private Boolean _EnableCache = XCache.Kind != XCache.CacheKinds.关闭缓存;
        /// <summary>是否启用缓存</summary>
        /// <remarks>设为false可清空缓存</remarks>
        public Boolean EnableCache
        {
            get { return _EnableCache; }
            set
            {
                _EnableCache = value;
                if (!_EnableCache) XCache.RemoveAll();
            }
        }

        /// <summary>缓存个数</summary>
        public Int32 CacheCount { get { return XCache.Count; } }

        [ThreadStatic]
        private static Int32 _QueryTimes;
        /// <summary>查询次数</summary>
        public static Int32 QueryTimes { get { return _QueryTimes; } }

        [ThreadStatic]
        private static Int32 _ExecuteTimes;
        /// <summary>执行次数</summary>
        public static Int32 ExecuteTimes { get { return _ExecuteTimes; } }
        #endregion

        #region 使用缓存后的数据操作方法
        //private DictionaryCache<String, SelectBuilder> _PageSplitCache2;
        /// <summary>根据条件把普通查询SQL格式化为分页SQL。</summary>
        /// <remarks>
        /// 因为需要继承重写的原因，在数据类中并不方便缓存分页SQL。
        /// 所以在这里做缓存。
        /// </remarks>
        /// <param name="builder">查询生成器</param>
        /// <param name="startRowIndex">开始行，0表示第一行</param>
        /// <param name="maximumRows">最大返回行数，0表示所有行</param>
        /// <returns>分页SQL</returns>
        public SelectBuilder PageSplit(SelectBuilder builder, Int32 startRowIndex, Int32 maximumRows)
        {
            //var cacheKey = String.Format("{0}_{1}_{2}_{3}", builder, startRowIndex, maximumRows, ConnName);

            //// 一个项目可能同时采用多种数据库，分页缓存不能采用静态
            //if (_PageSplitCache2 == null)
            //{
            //    _PageSplitCache2 = new DictionaryCache<String, SelectBuilder>(StringComparer.OrdinalIgnoreCase);

            //    // Access、SqlCe和SqlServer2000在处理DoubleTop时，最后一页可能导致数据不对，故不能长时间缓存其分页语句
            //    var dt = DbType;
            //    if (dt == DatabaseType.Access || dt == DatabaseType.SqlCe || dt == DatabaseType.SqlServer && Db.ServerVersion.StartsWith("08"))
            //    {
            //        _PageSplitCache2.Expire = 60;
            //    }
            //}
            //return _PageSplitCache2.GetItem(cacheKey, builder, startRowIndex, maximumRows, (k, b, s, m) => Db.PageSplit(b, s, m));

            //2016年7月2日 HUIYUE 取消分页SQL缓存，此部分缓存提升性能不多，但有可能会造成分页数据不准确，感觉得不偿失
            return Db.PageSplit(builder, startRowIndex, maximumRows);
        }

        /// <summary>执行SQL查询，返回记录集</summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="tableNames">所依赖的表的表名</param>
        /// <returns></returns>
        [DebuggerHidden]
        public DataSet Select(String sql, params String[] tableNames)
        {
            CheckBeforeUseDatabase();

            var cacheKey = sql + "_" + ConnName;
            DataSet ds = null;
            if (EnableCache && XCache.TryGetItem(cacheKey, out ds)) return ds;

            Interlocked.Increment(ref _QueryTimes);
            ds = Session.Query(sql);

            if (EnableCache) XCache.Add(cacheKey, ds, tableNames);

            return ds;
        }

        /// <summary>执行SQL查询，返回记录集</summary>
        /// <param name="builder">SQL语句</param>
        /// <param name="startRowIndex">开始行，0表示第一行</param>
        /// <param name="maximumRows">最大返回行数，0表示所有行</param>
        /// <param name="tableNames">所依赖的表的表名</param>
        /// <returns></returns>
        [DebuggerHidden]
        public DataSet Select(SelectBuilder builder, Int32 startRowIndex, Int32 maximumRows, params String[] tableNames)
        {
            builder = PageSplit(builder, startRowIndex, maximumRows);
            if (builder == null) return null;

            return Select(builder.ToString(), tableNames);
        }

        /// <summary>执行SQL查询，返回总记录数</summary>
        /// <param name="sb">查询生成器</param>
        /// <param name="tableNames">所依赖的表的表名</param>
        /// <returns></returns>
        [DebuggerHidden]
        public Int32 SelectCount(SelectBuilder sb, params String[] tableNames)
        {
            CheckBeforeUseDatabase();

            var cacheKey = "";
            var rs = 0;
            if (EnableCache)
            {
                cacheKey = sb + "_SelectCount" + "_" + ConnName;
                if (XCache.TryGetItem(cacheKey, out rs)) return rs;
            }

            Interlocked.Increment(ref _QueryTimes);
            rs = (Int32)Session.QueryCount(sb);

            if (EnableCache) XCache.Add(cacheKey, rs, tableNames);

            return rs;
        }

        /// <summary>执行SQL语句，返回受影响的行数</summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="tableNames">受影响的表的表名</param>
        /// <returns></returns>
        [DebuggerHidden]
        public Int32 Execute(String sql, params String[] tableNames)
        {
            CheckBeforeUseDatabase();

            Interlocked.Increment(ref _ExecuteTimes);

            var rs = Session.Execute(sql);

            // 移除所有和受影响表有关的缓存
            if (EnableCache) XCache.Remove(tableNames);

            return rs;
        }

        /// <summary>执行插入语句并返回新增行的自动编号</summary>
        /// <param name="sql"></param>
        /// <param name="tableNames">受影响的表的表名</param>
        /// <returns>新增行的自动编号</returns>
        [DebuggerHidden]
        public Int64 InsertAndGetIdentity(String sql, params String[] tableNames)
        {
            CheckBeforeUseDatabase();

            Interlocked.Increment(ref _ExecuteTimes);

            var rs = Session.InsertAndGetIdentity(sql);

            // 移除所有和受影响表有关的缓存
            if (EnableCache) XCache.Remove(tableNames);

            return rs;
        }

        /// <summary>执行SQL语句，返回受影响的行数</summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="type">命令类型，默认SQL文本</param>
        /// <param name="ps">命令参数</param>
        /// <param name="tableNames">受影响的表的表名</param>
        /// <returns></returns>
        [DebuggerHidden]
        public Int32 Execute(String sql, CommandType type, DbParameter[] ps, params String[] tableNames)
        {
            CheckBeforeUseDatabase();

            Interlocked.Increment(ref _ExecuteTimes);

            var rs = Session.Execute(sql, type, ps);

            // 移除所有和受影响表有关的缓存
            if (EnableCache) XCache.Remove(tableNames);

            return rs;
        }

        /// <summary>执行插入语句并返回新增行的自动编号</summary>
        /// <param name="sql"></param>
        /// <param name="type">命令类型，默认SQL文本</param>
        /// <param name="ps">命令参数</param>
        /// <param name="tableNames">受影响的表的表名</param>
        /// <returns>新增行的自动编号</returns>
        [DebuggerHidden]
        public Int64 InsertAndGetIdentity(String sql, CommandType type, DbParameter[] ps, params String[] tableNames)
        {
            CheckBeforeUseDatabase();

            Interlocked.Increment(ref _ExecuteTimes);

            var rs = Session.InsertAndGetIdentity(sql, type, ps);

            // 移除所有和受影响表有关的缓存
            if (EnableCache) XCache.Remove(tableNames);

            return rs;
        }

        /// <summary>执行CMD，返回记录集</summary>
        /// <param name="cmd">CMD</param>
        /// <param name="tableNames">所依赖的表的表名</param>
        /// <returns></returns>
        [DebuggerHidden]
        public DataSet Select(DbCommand cmd, String[] tableNames)
        {
            CheckBeforeUseDatabase();

            var cacheKey = "";
            DataSet ds = null;
            if (EnableCache)
            {
                cacheKey = cmd.CommandText + "_" + ConnName;
                if (XCache.TryGetItem(cacheKey, out ds)) return ds;
            }

            Interlocked.Increment(ref _QueryTimes);
            ds = Session.Query(cmd);

            if (EnableCache) XCache.Add(cacheKey, ds, tableNames);

            return ds;
        }

        /// <summary>执行CMD，返回受影响的行数</summary>
        /// <param name="cmd"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public Int32 Execute(DbCommand cmd, String[] tableNames)
        {
            CheckBeforeUseDatabase();

            Interlocked.Increment(ref _ExecuteTimes);
            var ret = Session.Execute(cmd);

            // 移除所有和受影响表有关的缓存
            if (EnableCache) XCache.Remove(tableNames);

            return ret;
        }
        #endregion

        #region 事务
        /// <summary>开始事务</summary>
        /// <returns>剩下的事务计数</returns>
        public Int32 BeginTransaction()
        {
            CheckBeforeUseDatabase();
            return Session.BeginTransaction();
        }

        /// <summary>提交事务</summary>
        /// <returns>剩下的事务计数</returns>
        public Int32 Commit() { return Session.Commit(); }

        /// <summary>回滚事务，忽略异常</summary>
        /// <returns>剩下的事务计数</returns>
        public Int32 Rollback() { return Session.Rollback(); }

        /// <summary>添加脏实体会话</summary>
        /// <param name="key">实体会话关键字</param>
        /// <param name="entitySession">事务嵌套处理中，事务真正提交或回滚之前，进行了子事务提交的实体会话</param>
        /// <param name="executeCount">实体操作次数</param>
        /// <param name="updateCount">实体更新操作次数</param>
        /// <param name="directExecuteSQLCount">直接执行SQL语句次数</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal void AddDirtiedEntitySession(String key, IEntitySession entitySession, Int32 executeCount, Int32 updateCount, Int32 directExecuteSQLCount)
        {
            Session.AddDirtiedEntitySession(key, entitySession, executeCount, updateCount, directExecuteSQLCount);
        }

        /// <summary>移除脏实体会话</summary>
        /// <param name="key">实体会话关键字</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal void RemoveDirtiedEntitySession(String key)
        {
            Session.RemoveDirtiedEntitySession(key);
        }

        /// <summary>获取脏实体会话</summary>
        /// <param name="key">实体会话关键字</param>
        /// <param name="session">脏实体会话</param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal Boolean TryGetDirtiedEntitySession(String key, out DirtiedEntitySession session)
        {
            return Session.TryGetDirtiedEntitySession(key, out session);
        }
        #endregion

        #region 队列
        /// <summary>实体队列</summary>
        public EntityQueue Queue { get; private set; }
        #endregion
    }
}