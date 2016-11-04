﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NewLife.Reflection;
using XCode.DataAccessLayer;

namespace XCode
{
    /// <summary>用于指定数据属性映射关系</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MapAttribute : Attribute
    {
        #region 属性
        /// <summary>数据列</summary>
        public String Name { get; set; }

        /// <summary>目标提供者</summary>
        public MapProvider Provider { get; set; }
        #endregion

        #region 构造
        /// <summary>指定一个表内关联关系</summary>
        /// <param name="column"></param>
        public MapAttribute(String column)
        {
            Name = column;
        }

        /// <summary>指定一个关系</summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="key"></param>
        public MapAttribute(String name, Type type, String key = null)
        {
            Name = name;

            // 区分实体类和提供者
            if (typeof(MapProvider).IsAssignableFrom(type))
                Provider = Activator.CreateInstance(type) as MapProvider;
            else
            {
                if (key.IsNullOrEmpty()) key = EntityFactory.CreateOperate(type)?.Unique?.Name;
                Provider = new MapProvider { EntityType = type, Key = key };
            }
        }
        #endregion

        #region 方法
        #endregion
    }

    /// <summary>映射提供者</summary>
    public class MapProvider
    {
        #region 属性
        /// <summary>实体类型</summary>
        public Type EntityType { get; set; }

        /// <summary>关联键</summary>
        public String Key { get; set; }
        #endregion

        #region 方法
        /// <summary>获取数据源</summary>
        /// <returns></returns>
        public virtual IDictionary<Object, String> GetDataSource()
        {
            var fact = EntityFactory.CreateOperate(EntityType);

            var key = Key;
            var mst = fact.Master?.Name;

            if (key.IsNullOrEmpty()) throw new ArgumentNullException("没有设置关联键", nameof(Key));
            if (mst.IsNullOrEmpty()) throw new ArgumentNullException("没有设置主要字段");

            // 数据较少时，从缓存读取
            var list = fact.Count < 1000 ? fact.FindAllWithCache() : fact.FindAll(null, null, null, 0, 100);

            return list.ToDictionary(e => e[key], e => e[mst] + "");
        }
        #endregion
    }
}