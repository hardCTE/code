using System;
using System.Collections.Generic;
using System.Data;
﻿using App.DAL;

namespace <#=Config.NameSpace#>
{<#
    String tdis=Table.DisplayName;
    if(!String.IsNullOrEmpty(tdis)) tdis=tdis.Replace("\r\n"," ").Replace("\\", "\\\\").Replace("'", "").Replace("\"", "");
    String tdes=Table.Description;
    if(!String.IsNullOrEmpty(tdes)) tdes=tdes.Replace("\r\n"," ").Replace("\\", "\\\\").Replace("'", "").Replace("\"", "");
    if(String.IsNullOrEmpty(tdis)) tdis=tdes;
    #>
    /// <summary><#=tdis#></summary><# if(tdis!=tdes){#>
	/// <remarks><#=tdes#></remarks><#}#>
    [Description("<#=tdes#>")]<#
foreach(IDataIndex di in Table.Indexes){if(di.Columns==null||di.Columns.Length<1)continue;#>
    [BindIndex("<#=di.Name#>", <#=di.Unique.ToString().ToLower()#>, "<#=String.Join(",", di.Columns)#>")]<#
}
foreach(IDataRelation dr in Table.Relations){#>
    [BindRelation("<#=dr.Column#>", <#=dr.Unique.ToString().ToLower()#>, "<#=dr.RelationTable#>", "<#=dr.RelationColumn#>")]<#}#>
    [BindTable("<#=Table.TableName#>", Description = "<#=tdes#>", ConnName = "<#=Table.ConnName ?? Config.EntityConnName#>", DbType = DatabaseType.<#=Table.DbType#><#if(Table.IsView){#>, IsView = true<#}#>)]
    public partial class <#=Table.Name#>Dal
    {<#
if(Table.Columns.Count>0)
{
#>
        #region 属性<#
    foreach(IDataColumn Field in Table.Columns)
    {
        String des=Field.Description;
        if(!String.IsNullOrEmpty(des)) des=des.Replace("\r\n"," ").Replace("\\", "\\\\").Replace("'", "").Replace("\"", "");
        String dis = Field.DisplayName;
        if(!String.IsNullOrEmpty(dis)) dis=dis.Replace("\r\n"," ").Replace("'", " ").Replace("\"", "");
#>
        /// <summary><#=des#></summary>
		public virtual <#=Field.DataType==null?"":Field.DataType.Name#> <#=Field.Name#> { get; set; }
<#
    }
#>        #endregion

        #region 获取/设置 字段值
        /// <summary>
        /// 获取/设置 字段值。
        /// 一个索引，基类使用反射实现。
        /// 派生实体类可重写该索引，以避免反射带来的性能损耗
        /// </summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        public virtual Object this[String name]
        {
            get
            {
                switch (name)
                {<#
    foreach(IDataColumn Field in Table.Columns)
    {
#>
                    case __.<#=Field.Name#> : return <#=Field.Name#>;<#
    }
#>
                    default: return null;
                }
            }
            set
            {
                switch (name)
                {<#
    Type conv=typeof(Convert);
    foreach(IDataColumn Field in Table.Columns)
    { 
        if(conv.GetMethod("To"+Field.DataType.Name, new Type[]{typeof(Object)})!=null){
#>
                    case __.<#=Field.Name#> : <#=Field.Name#> = Convert.To<#=Field.DataType.Name#>(value); break;<#
        }else{
#>
                    case __.<#=Field.Name#> : <#=Field.Name#> = (<#=Field.DataType.Name#>)value; break;<#
        }
    }
#>
                    default: break;
				}
            }
        }
        #endregion
<#
}
#>
        #region 字段信息

		/// <summary>取得<#=tdis#>字段名称的快捷方式</summary>
        public partial class __
        {
			/// <summary>
            /// 数据库表名
            /// </summary>
            public const string DataBaseTableName = "<#=Table.TableName#>";
<#
foreach(IDataColumn Field in Table.GetAllColumns(Tables, true))
{
    String des=Field.Description;
    if(!String.IsNullOrEmpty(des)) des=des.Replace("\r\n"," ");
#>
            ///<summary><#=des#></summary>
            public const String <#=Field.Name#> = "<#=Field.Name#>";
<#
}
#>
        }

        /// <summary>取得<#=tdis#>字段信息的快捷方式</summary>
        public partial class _
        {<#
foreach(IDataColumn Field in Table.GetAllColumns(Tables, true))
{
    String des=Field.Description;
    if(!String.IsNullOrEmpty(des)) des=des.Replace("\r\n"," ");
#>
            ///<summary><#=des#></summary>
            public static readonly Field <#=Field.Name#> = new Field
            {
                Name = __.<#=Field.Name#>,
				ColumnName = "<#=Field.Name#>",
                DisplayName = "<#=Field.DisplayName#>",
                Description = "<#=(""+Field.Description).Replace("\\", "\\\\")#>",
                DataType = DbType.<#=Field.DataType.Name#>,
                DefaultValue = <#=Field.Default==null?"null":"\""+Field.Default.Replace("\\", "\\\\")+"\""#>,
                IsPrimaryKey = <#=Field.PrimaryKey.ToString().ToLower()#>,
                IsReadonly = false,
                IsNullable = <#=Field.Nullable.ToString().ToLower()#>,
                Length = <#=Field.Length#>,
                Precision = <#=Field.Precision#>,
                Scale = <#=Field.Scale#>
			};
<#
}
#>
        }

        #endregion
    }
}