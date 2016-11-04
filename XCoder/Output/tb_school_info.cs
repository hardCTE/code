/*
 * XCoder v6.8.6152.26226
 * 作者：Administrator/XUDB
 * 时间：2016-11-04 14:34:20
 * 版权：xdb 2016~2016
*/
﻿using System;
using System.Collections.Generic;
using System.Data;
﻿using App.DAL;

namespace MyNameSpace
{
    /// <summary>学校信息表包含各种常用字段</summary>
	/// <remarks>学校信息表（包含各种常用字段）</remarks>
    [Description("学校信息表（包含各种常用字段）")]
    [BindIndex("PRIMARY", true, "key_id,key_str")]
    [BindIndex("idx_mul", false, "idx_code,idx_num")]
    [BindIndex("fk_categoryId", false, "ref_category")]
    [BindTable("tb_school_info", Description = "学校信息表（包含各种常用字段）", ConnName = "MyConnName", DbType = DatabaseType.MySql)]
    public partial class tb_school_infoDal
    {
        #region 属性
        /// <summary>班级（联合主键1，int、非空）</summary>
		public virtual Int32 key_id { get; set; }

        /// <summary>学校（联合主键2，字符可空，最大40）</summary>
		public virtual String key_str { get; set; }

        /// <summary>编码（索引1）</summary>
		public virtual String idx_code { get; set; }

        /// <summary>数字（序号索引2）</summary>
		public virtual Int64 idx_num { get; set; }

        /// <summary>引用的分类Id</summary>
		public virtual Int64 ref_category { get; set; }

        /// <summary>标示（字符char200)</summary>
		public virtual String txt_char { get; set; }

        /// <summary>名称（text8000）</summary>
		public virtual String txt_text { get; set; }

        /// <summary>是否启用（bool替代值enumyn）</summary>
		public virtual Boolean bool_enum { get; set; }

        /// <summary>扩展枚举（可多重选择1,2,3）</summary>
		public virtual String ext_enum { get; set; }

        /// <summary>类型（tinyint2）</summary>
		public virtual SByte num_tinyint { get; set; }

        /// <summary>价格（decimal10,2）</summary>
		public virtual Decimal num_decimal { get; set; }

        /// <summary>注册日期（date）</summary>
		public virtual DateTime dt_date { get; set; }

        /// <summary>修改时间（datetime）</summary>
		public virtual DateTime dt_datetime { get; set; }

        /// <summary>时间戳</summary>
		public virtual DateTime dt_timestamp { get; set; }
        #endregion

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
                {
                    case __.key_id : return key_id;
                    case __.key_str : return key_str;
                    case __.idx_code : return idx_code;
                    case __.idx_num : return idx_num;
                    case __.ref_category : return ref_category;
                    case __.txt_char : return txt_char;
                    case __.txt_text : return txt_text;
                    case __.bool_enum : return bool_enum;
                    case __.ext_enum : return ext_enum;
                    case __.num_tinyint : return num_tinyint;
                    case __.num_decimal : return num_decimal;
                    case __.dt_date : return dt_date;
                    case __.dt_datetime : return dt_datetime;
                    case __.dt_timestamp : return dt_timestamp;
                    default: return null;
                }
            }
            set
            {
                switch (name)
                {
                    case __.key_id : key_id = Convert.ToInt32(value); break;
                    case __.key_str : key_str = Convert.ToString(value); break;
                    case __.idx_code : idx_code = Convert.ToString(value); break;
                    case __.idx_num : idx_num = Convert.ToInt64(value); break;
                    case __.ref_category : ref_category = Convert.ToInt64(value); break;
                    case __.txt_char : txt_char = Convert.ToString(value); break;
                    case __.txt_text : txt_text = Convert.ToString(value); break;
                    case __.bool_enum : bool_enum = Convert.ToBoolean(value); break;
                    case __.ext_enum : ext_enum = Convert.ToString(value); break;
                    case __.num_tinyint : num_tinyint = Convert.ToSByte(value); break;
                    case __.num_decimal : num_decimal = Convert.ToDecimal(value); break;
                    case __.dt_date : dt_date = Convert.ToDateTime(value); break;
                    case __.dt_datetime : dt_datetime = Convert.ToDateTime(value); break;
                    case __.dt_timestamp : dt_timestamp = Convert.ToDateTime(value); break;
                    default: break;
				}
            }
        }
        #endregion

        #region 字段信息

		/// <summary>取得学校信息表包含各种常用字段字段名称的快捷方式</summary>
        public partial class __
        {
			/// <summary>
            /// 数据库表名
            /// </summary>
            public const string DataBaseTableName = "tb_school_info";

            ///<summary>班级（联合主键1，int、非空）</summary>
            public const String key_id = "key_id";

            ///<summary>学校（联合主键2，字符可空，最大40）</summary>
            public const String key_str = "key_str";

            ///<summary>编码（索引1）</summary>
            public const String idx_code = "idx_code";

            ///<summary>数字（序号索引2）</summary>
            public const String idx_num = "idx_num";

            ///<summary>引用的分类Id</summary>
            public const String ref_category = "ref_category";

            ///<summary>标示（字符char200)</summary>
            public const String txt_char = "txt_char";

            ///<summary>名称（text8000）</summary>
            public const String txt_text = "txt_text";

            ///<summary>是否启用（bool替代值enumyn）</summary>
            public const String bool_enum = "bool_enum";

            ///<summary>扩展枚举（可多重选择1,2,3）</summary>
            public const String ext_enum = "ext_enum";

            ///<summary>类型（tinyint2）</summary>
            public const String num_tinyint = "num_tinyint";

            ///<summary>价格（decimal10,2）</summary>
            public const String num_decimal = "num_decimal";

            ///<summary>注册日期（date）</summary>
            public const String dt_date = "dt_date";

            ///<summary>修改时间（datetime）</summary>
            public const String dt_datetime = "dt_datetime";

            ///<summary>时间戳</summary>
            public const String dt_timestamp = "dt_timestamp";

        }

        /// <summary>取得学校信息表包含各种常用字段字段信息的快捷方式</summary>
        public partial class _
        {
            ///<summary>班级（联合主键1，int、非空）</summary>
            public static readonly Field key_id = new Field
            {
                Name = __.key_id,
				ColumnName = "key_id",
                DisplayName = "班级联合主键1，int、非空",
                Description = "班级（联合主键1，int、非空）",
                DataType = DbType.Int32,
                DefaultValue = null,
                IsPrimaryKey = true,
                IsReadonly = false,
                IsNullable = false,
                Length = 10,
                Precision = 10,
                Scale = 0
			};

            ///<summary>学校（联合主键2，字符可空，最大40）</summary>
            public static readonly Field key_str = new Field
            {
                Name = __.key_str,
				ColumnName = "key_str",
                DisplayName = "学校联合主键2，字符可空，最大40",
                Description = "学校（联合主键2，字符可空，最大40）",
                DataType = DbType.String,
                DefaultValue = "",
                IsPrimaryKey = true,
                IsReadonly = false,
                IsNullable = false,
                Length = 40,
                Precision = 0,
                Scale = 0
			};

            ///<summary>编码（索引1）</summary>
            public static readonly Field idx_code = new Field
            {
                Name = __.idx_code,
				ColumnName = "idx_code",
                DisplayName = "编码索引1",
                Description = "编码（索引1）",
                DataType = DbType.String,
                DefaultValue = "",
                IsPrimaryKey = false,
                IsReadonly = false,
                IsNullable = true,
                Length = 50,
                Precision = 0,
                Scale = 0
			};

            ///<summary>数字（序号索引2）</summary>
            public static readonly Field idx_num = new Field
            {
                Name = __.idx_num,
				ColumnName = "idx_num",
                DisplayName = "数字序号索引2",
                Description = "数字（序号索引2）",
                DataType = DbType.Int64,
                DefaultValue = null,
                IsPrimaryKey = false,
                IsReadonly = false,
                IsNullable = false,
                Length = 19,
                Precision = 19,
                Scale = 0
			};

            ///<summary>引用的分类Id</summary>
            public static readonly Field ref_category = new Field
            {
                Name = __.ref_category,
				ColumnName = "ref_category",
                DisplayName = "引用的分类Id",
                Description = "引用的分类Id",
                DataType = DbType.Int64,
                DefaultValue = null,
                IsPrimaryKey = false,
                IsReadonly = false,
                IsNullable = false,
                Length = 19,
                Precision = 19,
                Scale = 0
			};

            ///<summary>标示（字符char200)</summary>
            public static readonly Field txt_char = new Field
            {
                Name = __.txt_char,
				ColumnName = "txt_char",
                DisplayName = "标示字符char200",
                Description = "标示（字符char200)",
                DataType = DbType.String,
                DefaultValue = "",
                IsPrimaryKey = false,
                IsReadonly = false,
                IsNullable = true,
                Length = 200,
                Precision = 0,
                Scale = 0
			};

            ///<summary>名称（text8000）</summary>
            public static readonly Field txt_text = new Field
            {
                Name = __.txt_text,
				ColumnName = "txt_text",
                DisplayName = "名称text8000",
                Description = "名称（text8000）",
                DataType = DbType.String,
                DefaultValue = "",
                IsPrimaryKey = false,
                IsReadonly = false,
                IsNullable = true,
                Length = 65535,
                Precision = 0,
                Scale = 0
			};

            ///<summary>是否启用（bool替代值enumyn）</summary>
            public static readonly Field bool_enum = new Field
            {
                Name = __.bool_enum,
				ColumnName = "bool_enum",
                DisplayName = "是否启用bool替代值enumyn",
                Description = "是否启用（bool替代值enumyn）",
                DataType = DbType.Boolean,
                DefaultValue = "false",
                IsPrimaryKey = false,
                IsReadonly = false,
                IsNullable = false,
                Length = 1,
                Precision = 0,
                Scale = 0
			};

            ///<summary>扩展枚举（可多重选择1,2,3）</summary>
            public static readonly Field ext_enum = new Field
            {
                Name = __.ext_enum,
				ColumnName = "ext_enum",
                DisplayName = "扩展枚举可多重选择1,2,3",
                Description = "扩展枚举（可多重选择1,2,3）",
                DataType = DbType.String,
                DefaultValue = "",
                IsPrimaryKey = false,
                IsReadonly = false,
                IsNullable = true,
                Length = 5,
                Precision = 0,
                Scale = 0
			};

            ///<summary>类型（tinyint2）</summary>
            public static readonly Field num_tinyint = new Field
            {
                Name = __.num_tinyint,
				ColumnName = "num_tinyint",
                DisplayName = "类型tinyint2",
                Description = "类型（tinyint2）",
                DataType = DbType.SByte,
                DefaultValue = null,
                IsPrimaryKey = false,
                IsReadonly = false,
                IsNullable = true,
                Length = 3,
                Precision = 3,
                Scale = 0
			};

            ///<summary>价格（decimal10,2）</summary>
            public static readonly Field num_decimal = new Field
            {
                Name = __.num_decimal,
				ColumnName = "num_decimal",
                DisplayName = "价格decimal10,2",
                Description = "价格（decimal10,2）",
                DataType = DbType.Decimal,
                DefaultValue = null,
                IsPrimaryKey = false,
                IsReadonly = false,
                IsNullable = true,
                Length = 10,
                Precision = 10,
                Scale = 2
			};

            ///<summary>注册日期（date）</summary>
            public static readonly Field dt_date = new Field
            {
                Name = __.dt_date,
				ColumnName = "dt_date",
                DisplayName = "注册日期date",
                Description = "注册日期（date）",
                DataType = DbType.DateTime,
                DefaultValue = null,
                IsPrimaryKey = false,
                IsReadonly = false,
                IsNullable = true,
                Length = 0,
                Precision = 0,
                Scale = 0
			};

            ///<summary>修改时间（datetime）</summary>
            public static readonly Field dt_datetime = new Field
            {
                Name = __.dt_datetime,
				ColumnName = "dt_datetime",
                DisplayName = "修改时间datetime",
                Description = "修改时间（datetime）",
                DataType = DbType.DateTime,
                DefaultValue = null,
                IsPrimaryKey = false,
                IsReadonly = false,
                IsNullable = true,
                Length = 0,
                Precision = 0,
                Scale = 0
			};

            ///<summary>时间戳</summary>
            public static readonly Field dt_timestamp = new Field
            {
                Name = __.dt_timestamp,
				ColumnName = "dt_timestamp",
                DisplayName = "时间戳",
                Description = "时间戳",
                DataType = DbType.DateTime,
                DefaultValue = null,
                IsPrimaryKey = false,
                IsReadonly = false,
                IsNullable = true,
                Length = 0,
                Precision = 0,
                Scale = 0
			};

        }

        #endregion
    }
}