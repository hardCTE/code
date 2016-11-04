﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using NewLife.Collections;
using NewLife.Web;
using XCode.Cache;

namespace XCode.Membership
{
    /// <summary>日志</summary>
    [Serializable]
    [ModelCheckMode(ModelCheckModes.CheckTableWhenFirstUse)]
    public class Log : Log<Log> { }

    /// <summary>日志</summary>
    public partial class Log<TEntity> : Entity<TEntity> where TEntity : Log<TEntity>, new()
    {
        #region 对象操作
        static Log()
        {
            Meta.Table.DataTable.InsertOnly = true;
        }

        /// <summary>已重载。记录当前管理员</summary>
        /// <param name="isNew"></param>
        public override void Valid(bool isNew)
        {
            base.Valid(isNew);

            if (isNew)
            {
                // 自动设置当前登录用户
                if (!Dirtys[__.UserName])
                {
                    var user = ManageProvider.User;
                    if (user != null)
                    {
                        UserName = user.ToString();
                    }
                }
            }

            // 处理过长的备注
            if (!String.IsNullOrEmpty(Remark) && Remark.Length > 500)
            {
                Remark = Remark.Substring(0, 500);
            }
        }

        /// <summary></summary>
        /// <returns></returns>
        protected override int OnUpdate()
        {
            throw new Exception("禁止修改日志！");
        }

        /// <summary></summary>
        /// <returns></returns>
        protected override int OnDelete()
        {
            throw new Exception("禁止删除日志！");
        }
        #endregion

        #region 扩展属性
        /// <summary>创建人名称</summary>
        [XmlIgnore, ScriptIgnore]
        [DisplayName("创建人")]
        [Map("CreateUserID")]
        public String CreateUserName { get { return ManageProvider.Provider.FindByID(CreateUserID) + ""; } }

        /// <summary>物理地址</summary>
        //[BindRelation("CreateIP")]
        [DisplayName("物理地址")]
        public String CreateAddress { get { return CreateIP.IPToAddress(); } }
        #endregion

        #region 扩展查询
        /// <summary>查询</summary>
        /// <param name="key"></param>
        /// <param name="adminid"></param>
        /// <param name="category"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="orderClause"></param>
        /// <param name="startRowIndex"></param>
        /// <param name="maximumRows"></param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static EntityList<TEntity> Search(String key, Int32 adminid, String category, DateTime start, DateTime end, String orderClause, Int32 startRowIndex, Int32 maximumRows)
        {
            if (String.IsNullOrEmpty(orderClause)) orderClause = _.ID.Desc();
            return FindAll(SearchWhere(key, adminid, category, start, end), orderClause, null, startRowIndex, maximumRows);
        }

        /// <summary>查询</summary>
        /// <param name="key"></param>
        /// <param name="adminid"></param>
        /// <param name="category"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="orderClause"></param>
        /// <param name="startRowIndex"></param>
        /// <param name="maximumRows"></param>
        /// <returns></returns>
        public static Int32 SearchCount(String key, Int32 adminid, String category, DateTime start, DateTime end, String orderClause, Int32 startRowIndex, Int32 maximumRows)
        {
            String where = SearchWhere(key, adminid, category, start, end);
            return FindCount(where, null, null, 0, 0);
        }

        /// <summary>查询</summary>
        /// <param name="key"></param>
        /// <param name="adminid"></param>
        /// <param name="category"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static String SearchWhere(String key, Int32 adminid, String category, DateTime start, DateTime end)
        {
            var exp = new WhereExpression();
            if (!String.IsNullOrEmpty(key)) exp &= (_.Action == key | _.Remark.Contains(key));
            if (!String.IsNullOrEmpty(category) && category != "全部") exp &= _.Category == category;
            if (adminid > 0) exp &= _.CreateUserID == adminid;
            if (start > DateTime.MinValue) exp &= _.CreateTime >= start;
            if (end > DateTime.MinValue) exp &= _.CreateTime < end.Date.AddDays(1);

            return exp;
        }

        /// <summary>查询</summary>
        /// <param name="key"></param>
        /// <param name="adminid"></param>
        /// <param name="category"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static EntityList<TEntity> Search(String key, Int32 adminid, String category, DateTime start, DateTime end, Pager p)
        {
            return FindAll(SearchWhere(key, adminid, category, start, end), p);
        }
        #endregion

        #region 扩展操作
        static FieldCache<TEntity> CategoryCache = new FieldCache<TEntity>(_.Category);

        /// <summary>查找所有类别名</summary>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static EntityList<TEntity> FindAllCategory()
        {
            return CategoryCache.Entities;
        }

        /// <summary>获取所有类别名称</summary>
        /// <returns></returns>
        public static IDictionary<String, String> FindAllCategoryName()
        {
            return CategoryCache.FindAllName();
        }
        #endregion

        #region 业务
        /// <summary>创建日志</summary>
        /// <param name="category"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TEntity Create(String category, String action)
        {
            var entity = new TEntity();

            entity.Category = category;
            entity.Action = action;

            return entity;
        }

        /// <summary>创建日志</summary>
        /// <param name="type">类型</param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TEntity Create(Type type, String action)
        {
            var name = type.GetDisplayName() ?? type.GetDescription() ?? type.Name;

            return Create(name, action);
        }

        /// <summary>创建</summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        ILog ILog.Create(Type type, String action) { return Create(type, action); }

        /// <summary>写日志</summary>
        /// <param name="type">类型</param>
        /// <param name="action">操作</param>
        /// <param name="remark">备注</param>
        public void WriteLog(Type type, String action, String remark)
        {
            var log = Create(type, action);
            if (log != null)
            {
                log.Remark = remark;
                log.Save();
            }
        }

        /// <summary>已重载。</summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3:yyyy-MM-dd HH:mm:ss} {4}", Category, Action, UserName ?? CreateUserName, CreateTime, Remark);
        }
        #endregion
    }

    public partial interface ILog
    {
        /// <summary>创建</summary>
        /// <param name="type"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        ILog Create(Type type, String action);

        /// <summary>写日志</summary>
        /// <param name="type">类型</param>
        /// <param name="action">操作</param>
        /// <param name="remark">备注</param>
        void WriteLog(Type type, String action, String remark);

        /// <summary>保存</summary>
        /// <returns></returns>
        Int32 Save();

        /// <summary>异步保存</summary>
        /// <returns></returns>
        Boolean SaveAsync();
    }
}