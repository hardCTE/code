﻿﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using XCode;
using XCode.Configuration;
using XCode.DataAccessLayer;

namespace XCode.Membership
{
    /// <summary>角色</summary>
    [Serializable]
    [DataObject]
    [Description("角色")]
    [BindIndex("IU_Role_Name", true, "Name")]
    [BindTable("Role", Description = "角色", ConnName = "Membership", DbType = DatabaseType.SqlServer)]
    public abstract partial class Role<TEntity> : IRole
    {
        #region 属性
        private Int32 _ID;
        /// <summary>编号</summary>
        [DisplayName("编号")]
        [Description("编号")]
        [DataObjectField(true, true, false, 10)]
        [BindColumn(1, "ID", "编号", null, "int", 10, 0, false)]
        public virtual Int32 ID
        {
            get { return _ID; }
            set { if (OnPropertyChanging(__.ID, value)) { _ID = value; OnPropertyChanged(__.ID); } }
        }

        private String _Name;
        /// <summary>名称</summary>
        [DisplayName("名称")]
        [Description("名称")]
        [DataObjectField(false, false, false, 50)]
        [BindColumn(2, "Name", "名称", null, "nvarchar(50)", 0, 0, true, Master=true)]
        public virtual String Name
        {
            get { return _Name; }
            set { if (OnPropertyChanging(__.Name, value)) { _Name = value; OnPropertyChanged(__.Name); } }
        }

        private Boolean _IsSystem;
        /// <summary>是否系统角色。系统角色用于业务系统开发使用，禁止修改名称或删除</summary>
        [DisplayName("是否系统角色")]
        [Description("是否系统角色。系统角色用于业务系统开发使用，禁止修改名称或删除")]
        [DataObjectField(false, false, true, 1)]
        [BindColumn(3, "IsSystem", "是否系统角色。系统角色用于业务系统开发使用，禁止修改名称或删除", null, "bit", 0, 0, false)]
        public virtual Boolean IsSystem
        {
            get { return _IsSystem; }
            set { if (OnPropertyChanging(__.IsSystem, value)) { _IsSystem = value; OnPropertyChanged(__.IsSystem); } }
        }

        private String _Remark;
        /// <summary>说明</summary>
        [DisplayName("说明")]
        [Description("说明")]
        [DataObjectField(false, false, true, 50)]
        [BindColumn(4, "Remark", "说明", null, "nvarchar(50)", 0, 0, true)]
        public virtual String Remark
        {
            get { return _Remark; }
            set { if (OnPropertyChanging(__.Remark, value)) { _Remark = value; OnPropertyChanged(__.Remark); } }
        }

        private String _Permission;
        /// <summary>权限。对不同资源的权限，逗号分隔，每个资源的权限子项竖线分隔</summary>
        [DisplayName("权限")]
        [Description("权限。对不同资源的权限，逗号分隔，每个资源的权限子项竖线分隔")]
        [DataObjectField(false, false, true, 500)]
        [BindColumn(5, "Permission", "权限。对不同资源的权限，逗号分隔，每个资源的权限子项竖线分隔", null, "nvarchar(500)", 0, 0, true)]
        public virtual String Permission
        {
            get { return _Permission; }
            set { if (OnPropertyChanging(__.Permission, value)) { _Permission = value; OnPropertyChanged(__.Permission); } }
        }
        #endregion

        #region 获取/设置 字段值
        /// <summary>
        /// 获取/设置 字段值。
        /// 一个索引，基类使用反射实现。
        /// 派生实体类可重写该索引，以避免反射带来的性能损耗
        /// </summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        public override Object this[String name]
        {
            get
            {
                switch (name)
                {
                    case __.ID : return _ID;
                    case __.Name : return _Name;
                    case __.IsSystem : return _IsSystem;
                    case __.Remark : return _Remark;
                    case __.Permission : return _Permission;
                    default: return base[name];
                }
            }
            set
            {
                switch (name)
                {
                    case __.ID : _ID = Convert.ToInt32(value); break;
                    case __.Name : _Name = Convert.ToString(value); break;
                    case __.IsSystem : _IsSystem = Convert.ToBoolean(value); break;
                    case __.Remark : _Remark = Convert.ToString(value); break;
                    case __.Permission : _Permission = Convert.ToString(value); break;
                    default: base[name] = value; break;
                }
            }
        }
        #endregion

        #region 字段名
        /// <summary>取得角色字段信息的快捷方式</summary>
        partial class _
        {
            ///<summary>编号</summary>
            public static readonly Field ID = FindByName(__.ID);

            ///<summary>名称</summary>
            public static readonly Field Name = FindByName(__.Name);

            ///<summary>是否系统角色。系统角色用于业务系统开发使用，禁止修改名称或删除</summary>
            public static readonly Field IsSystem = FindByName(__.IsSystem);

            ///<summary>说明</summary>
            public static readonly Field Remark = FindByName(__.Remark);

            ///<summary>权限。对不同资源的权限，逗号分隔，每个资源的权限子项竖线分隔</summary>
            public static readonly Field Permission = FindByName(__.Permission);

            static Field FindByName(String name) { return Meta.Table.FindByName(name); }
        }

        /// <summary>取得角色字段名称的快捷方式</summary>
        partial class __
        {
            ///<summary>编号</summary>
            public const String ID = "ID";

            ///<summary>名称</summary>
            public const String Name = "Name";

            ///<summary>是否系统角色。系统角色用于业务系统开发使用，禁止修改名称或删除</summary>
            public const String IsSystem = "IsSystem";

            ///<summary>说明</summary>
            public const String Remark = "Remark";

            ///<summary>权限。对不同资源的权限，逗号分隔，每个资源的权限子项竖线分隔</summary>
            public const String Permission = "Permission";

        }
        #endregion
    }

    /// <summary>角色接口</summary>
    public partial interface IRole
    {
        #region 属性
        /// <summary>编号</summary>
        Int32 ID { get; set; }

        /// <summary>名称</summary>
        String Name { get; set; }

        /// <summary>是否系统角色。系统角色用于业务系统开发使用，禁止修改名称或删除</summary>
        Boolean IsSystem { get; set; }

        /// <summary>说明</summary>
        String Remark { get; set; }

        /// <summary>权限。对不同资源的权限，逗号分隔，每个资源的权限子项竖线分隔</summary>
        String Permission { get; set; }
        #endregion

        #region 获取/设置 字段值
        /// <summary>获取/设置 字段值。</summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        Object this[String name] { get; set; }
        #endregion
    }
}