﻿using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace XCode.DataAccessLayer
{
    /// <summary>数据关系</summary>
    [Serializable]
    [DisplayName("关系模型")]
    [Description("关系模型")]
    [XmlRoot("Relation")]
    class XRelation : SerializableDataMember, IDataRelation, ICloneable
    {
        #region 属性
        /// <summary>数据列</summary>
        [XmlAttribute]
        [DisplayName("数据列")]
        [Description("数据列")]
        public String Column { get; set; }

        /// <summary>引用表</summary>
        [XmlAttribute]
        [DisplayName("引用表")]
        [Description("引用表")]
        public String RelationTable { get; set; }

        /// <summary>引用列</summary>
        [XmlAttribute]
        [DisplayName("引用列")]
        [Description("引用列")]
        public String RelationColumn { get; set; }

        /// <summary>是否唯一</summary>
        [XmlAttribute]
        [DisplayName("唯一")]
        [Description("唯一")]
        public Boolean Unique { get; set; }

        /// <summary>是否计算出来的，而不是数据库内置的</summary>
        [XmlAttribute]
        [DisplayName("计算")]
        [Description("是否计算出来的，而不是数据库内置的")]
        public Boolean Computed { get; set; }

        /// <summary>表</summary>
        [XmlIgnore]
        public IDataTable Table { get; set; }
        #endregion

        #region ICloneable 成员
        /// <summary>克隆</summary>
        /// <returns></returns>
        object ICloneable.Clone()
        {
            return Clone(Table);
        }

        /// <summary>克隆</summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public IDataRelation Clone(IDataTable table)
        {
            var field = base.MemberwiseClone() as XRelation;
            field.Table = table;

            return field;
        }
        #endregion

        #region 辅助
        /// <summary>已重载。</summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}=>{1}.{2} {3}", Column, RelationTable, RelationColumn, Unique ? "U" : "");
        }
        #endregion
    }
}