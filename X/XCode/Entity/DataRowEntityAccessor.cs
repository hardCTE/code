﻿using System;
using System.Collections.Generic;
using System.Data;
using NewLife.Reflection;
using XCode.Configuration;
using XCode.DataAccessLayer;

namespace XCode
{
    /// <summary>在数据行和实体类之间映射数据的接口</summary>
    public interface IDataRowEntityAccessor
    {
        /// <summary>加载数据表。无数据时返回空集合而不是null。</summary>
        /// <param name="dt">数据表</param>
        /// <returns>实体数组</returns>
        IEntityList LoadData(DataTable dt);

        ///// <summary>从一个数据行对象加载数据。不加载关联对象。</summary>
        ///// <param name="dr">数据行</param>
        ///// <param name="entity">实体对象</param>
        //void LoadData(DataRow dr, IEntity entity);

        ///// <summary>从数据读写器加载数据。无数据时返回空集合而不是null。</summary>
        ///// <param name="dr">数据读写器</param>
        ///// <returns>实体数组</returns>
        //IEntityList LoadData(IDataReader dr);

        ///// <summary>从一个数据行对象加载数据。不加载关联对象。</summary>
        ///// <param name="dr">数据读写器</param>
        ///// <param name="entity">实体对象</param>
        //void LoadData(IDataReader dr, IEntity entity);

        ///// <summary>把数据复制到数据行对象中。</summary>
        ///// <param name="entity">实体对象</param>
        ///// <param name="dr">数据行</param>
        //DataRow ToData(IEntity entity, ref DataRow dr);
    }

    /// <summary>在数据行和实体类之间映射数据接口的提供者</summary>
    public interface IDataRowEntityAccessorProvider
    {
        /// <summary>创建实体类的数据行访问器</summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        IDataRowEntityAccessor CreateDataRowEntityAccessor(Type entityType);
    }

    class DataRowEntityAccessorProvider : IDataRowEntityAccessorProvider
    {
        /// <summary>创建实体类的数据行访问器</summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public IDataRowEntityAccessor CreateDataRowEntityAccessor(Type entityType)
        {
            return new DataRowEntityAccessor(entityType);
        }
    }

    class DataRowEntityAccessor : IDataRowEntityAccessor
    {
        #region 属性
        private Type _EntityType;
        /// <summary>实体类</summary>
        public Type EntityType { get { return _EntityType; } set { _EntityType = value; } }

        private IEntityOperate _Factory;
        /// <summary>实体操作者</summary>
        public IEntityOperate Factory
        {
            get { return _Factory ?? (_Factory = EntityFactory.CreateOperate(EntityType)); }
            set { _Factory = value; }
        }

        private Dictionary<String, FieldItem> _FieldItems;
        /// <summary>字段名-字段字典</summary>
        public Dictionary<String, FieldItem> FieldItems
        {
            get
            {
                if (_FieldItems == null)
                {
                    var dic = new Dictionary<String, FieldItem>(StringComparer.OrdinalIgnoreCase);
                    foreach (var item in Factory.Fields)
                    {
                        if (!dic.ContainsKey(item.ColumnName)) dic.Add(item.ColumnName, item);
                    }
                    _FieldItems = dic;
                }
                return _FieldItems;
            }
        }

        public DataRowEntityAccessor(Type type) { EntityType = type; }
        #endregion

        #region 存取
        /// <summary>加载数据表。无数据时返回空集合而不是null。</summary>
        /// <param name="dt">数据表</param>
        /// <returns>实体数组</returns>
        public IEntityList LoadData(DataTable dt)
        {
            // 准备好实体列表
            var list = typeof(EntityList<>).MakeGenericType(EntityType).CreateInstance(dt.Rows.Count) as IEntityList;
            if (dt == null || dt.Rows.Count < 1) return list;

            // 对应数据表中字段的实体字段
            var ps = new List<FieldItem>();
            // 数据表中找不到对应的实体字段的数据字段
            var exts = new List<String>();
            foreach (DataColumn item in dt.Columns)
            {
                var name = item.ColumnName;
                FieldItem fi = null;
                if (FieldItems.TryGetValue(name, out fi))
                    ps.Add(fi);
                else
                    exts.Add(name);
            }

            // 遍历每一行数据，填充成为实体
            foreach (DataRow dr in dt.Rows)
            {
                // 由实体操作者创建实体对象，因为实体操作者可能更换
                var entity = Factory.Create();
                //LoadData(dr, entity, ps, exts);
                foreach (var item in ps)
                    SetValue(entity, item.Name, item.Type, dr[item]);

                foreach (var item in exts)
                    SetValue(entity, item, null, dr[item]);

                list.Add(entity);
            }
            return list;
        }

        ///// <summary>从一个数据行对象加载数据。不加载关联对象。</summary>
        ///// <param name="dr">数据行</param>
        ///// <param name="entity">实体对象</param>
        //public void LoadData(DataRow dr, IEntity entity)
        //{
        //    if (dr == null) return;

        //    var ps = new List<FieldItem>();
        //    var exts = new List<String>();
        //    foreach (DataColumn item in dr.Table.Columns)
        //    {
        //        var name = item.ColumnName;
        //        FieldItem fi = null;
        //        if (FieldItems.TryGetValue(name, out fi))
        //            ps.Add(fi);
        //        else
        //            exts.Add(name);
        //    }

        //    LoadData(dr, entity, ps, exts);
        //}

        ///// <summary>从数据读写器加载数据。无数据时返回空集合而不是null。</summary>
        ///// <param name="dr">数据读写器</param>
        ///// <returns>实体数组</returns>
        //public IEntityList LoadData(IDataReader dr)
        //{
        //    // 准备好实体列表
        //    var list = typeof(EntityList<>).MakeGenericType(EntityType).CreateInstance() as IEntityList;
        //    if (dr == null) return list;

        //    // 先移到第一行，要取字段名等信息
        //    if (!dr.Read()) return list;

        //    var ps = new List<FieldItem>();
        //    var exts = new List<String>();
        //    for (int i = 0; i < dr.FieldCount; i++)
        //    {
        //        var name = dr.GetName(i);
        //        FieldItem fi = null;
        //        if (FieldItems.TryGetValue(name, out fi))
        //            ps.Add(fi);
        //        else
        //            exts.Add(name);
        //    }

        //    // 遍历每一行数据，填充成为实体
        //    do
        //    {
        //        // 由实体操作者创建实体对象，因为实体操作者可能更换
        //        var entity = Factory.Create();
        //        foreach (var item in ps)
        //        {
        //            SetValue(entity, item.Name, item.Type, dr[item]);
        //        }

        //        foreach (var item in exts)
        //        {
        //            SetValue(entity, item, null, dr[item]);
        //        }

        //        list.Add(entity);
        //    } while (dr.Read());
        //    return list;
        //}

        ///// <summary>从一个数据读写器加载数据。不加载关联对象。</summary>
        ///// <param name="dr">数据读写器</param>
        ///// <param name="entity">实体对象</param>
        //public void LoadData(IDataReader dr, IEntity entity)
        //{
        //    if (dr == null) return;

        //    // IDataReader的GetSchemaTable方法太浪费资源了
        //    for (int i = 0; i < dr.FieldCount; i++)
        //    {
        //        var name = dr.GetName(i);
        //        Type type = null;

        //        FieldItem fi = null;
        //        if (FieldItems.TryGetValue(name, out fi))
        //        {
        //            name = fi.Name;
        //            type = fi.Type;
        //        }

        //        SetValue(entity, name, type, dr.GetValue(i));
        //    }
        //}

        ///// <summary>把数据复制到数据行对象中。</summary>
        ///// <param name="entity">实体对象</param>
        ///// <param name="dr">数据行</param>
        //public DataRow ToData(IEntity entity, ref DataRow dr)
        //{
        //    if (dr == null) return null;

        //    var ps = new List<String>();
        //    foreach (var fi in Factory.AllFields)
        //    {
        //        // 检查dr中是否有该属性的列。考虑到Select可能是不完整的，此时，只需要局部填充
        //        if (dr.Table.Columns.Contains(fi.ColumnName))
        //        {
        //            dr[fi.ColumnName] = entity[fi.Name];
        //        }

        //        ps.Add(fi.ColumnName);
        //    }

        //    // 扩展属性也写入
        //    if (entity.Extends != null && entity.Extends.Count > 0)
        //    {
        //        foreach (var item in entity.Extends)
        //        {
        //            try
        //            {
        //                if (!ps.Contains(item.Key) && dr.Table.Columns.Contains(item.Key))
        //                    dr[item.Key] = item.Value;
        //            }
        //            catch { }
        //        }
        //    }
        //    return dr;
        //}
        #endregion

        #region 方法
        static String[] TrueString = new String[] { "true", "y", "yes", "1" };
        static String[] FalseString = new String[] { "false", "n", "no", "0" };

        //private void LoadData(DataRow dr, IEntity entity, List<FieldItem> ps, List<String> exts)
        //{
        //    if (dr == null) return;

        //    foreach (var item in ps)
        //    {
        //        SetValue(entity, item.Name, item.Type, dr[item]);
        //    }

        //    foreach (var item in exts)
        //    {
        //        SetValue(entity, item, null, dr[item]);
        //    }
        //}

        private void SetValue(IEntity entity, String name, Type type, Object value)
        {
            // 注意：name并不一定是实体类的成员，随便读取原数据可能会造成不必要的麻烦
            Object oldValue = null;
            if (type != null)
                // 仅对精确匹配的字段进行读取旧值
                oldValue = entity[name];
            else
            {
                type = value.GetType();
                // 如果扩展数据里面有该字段也读取旧值
                if (entity.Extends.ContainsKey(name)) oldValue = entity.Extends[name];
            }

            // 不处理相同数据的赋值
            if (Object.Equals(value, oldValue)) return;

            if (type == typeof(String))
            {
                // 不处理空字符串对空字符串的赋值
                if (value != null && String.IsNullOrEmpty(value.ToString()))
                {
                    if (oldValue == null || String.IsNullOrEmpty(oldValue.ToString())) return;
                }
            }
            else if (type == typeof(Boolean))
            {
                // 处理字符串转为布尔型
                if (value != null && value.GetType() == typeof(String))
                {
                    var vs = value.ToString();
                    if (String.IsNullOrEmpty(vs))
                        value = false;
                    else
                    {
                        if (Array.IndexOf(TrueString, vs.ToLower()) >= 0)
                            value = true;
                        else if (Array.IndexOf(FalseString, vs.ToLower()) >= 0)
                            value = false;
                        else if (DAL.Debug)
                            DAL.WriteLog("无法把字符串{0}转为布尔型！", vs);
                    }
                }
            }
            else if (type == typeof(Guid))
            {
                if (!(value is Guid))
                {
                    if (value is Byte[])
                        value = new Guid((Byte[])value);
                    else if (value is String)
                        value = new Guid((String)value);
                }
            }

            // 不影响脏数据的状态
            var ds = entity.Dirtys;
            Boolean? b = null;
            if (ds.ContainsKey(name)) b = ds[name];

            entity[name] = value == DBNull.Value ? null : value;

            if (b != null)
                ds[name] = b.Value;
            else
                ds.Remove(name);
        }
        #endregion
    }
}