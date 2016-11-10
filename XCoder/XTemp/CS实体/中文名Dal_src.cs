namespace CS实体
{
    using System.Linq;
    using TemplateHelper;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using System.Xml;
    using XCode;
    using XCode.Test;
    using XCode.Transform;
    using XCode.Web;
    using XCode.Model;
    using XCode.Exceptions;
    using XCode.Configuration;
    using XCode.Common;
    using XCode.Sync;
    using XCode.Membership;
    using XCode.DataAccessLayer;
    using XCode.Code;
    using XCode.Cache;
    using System.Windows.Forms;
    using System.Threading.Tasks;
    using System.IO;
    using NewLife;
    using NewLife.Xml;
    using NewLife.Windows;
    using NewLife.Data;
    using NewLife.Web;
    using NewLife.Threading;
    using NewLife.Serialization;
    using NewLife.Security;
    using NewLife.Remoting;
    using NewLife.Reflection;
    using NewLife.Net;
    using NewLife.Model;
    using NewLife.Messaging;
    using NewLife.Log;
    using NewLife.IO;
    using NewLife.Configuration;
    using NewLife.Common;
    using NewLife.Collections;
    using NewLife.IP;
    using NewLife.Agent;
    
    
    #line 1 "中文名Dal.tt"
    public class 中文名Dal : XCoder.XCoderBase
    {
        public override string Render()
        {
            
            #line 1 "中文名Dal.tt"
            this.Write("﻿using System;\r\nusing System.Collections.Generic;\r\nusing System.Data;\r\nusing App." +
                    "FrameCore;\r\nusing Dapper;\r\n\r\nnamespace ");
            
            #line default
            #line hidden
            
            #line 7 "中文名Dal.tt"
            this.Write(Config.NameSpace);
            
            #line default
            #line hidden
            
            #line 7 "中文名Dal.tt"
            this.Write("\r\n{");
            
            #line default
            #line hidden
            
            #line 8 "中文名Dal.tt"

	var modelName = FormatUtil.ToCodeName(Table.Name);

            
            #line default
            #line hidden
            
            #line 10 "中文名Dal.tt"
            this.Write("\r\n\t/// <summary>\r\n    /// ");
            
            #line default
            #line hidden
            
            #line 12 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 12 "中文名Dal.tt"
            this.Write(" 数据访问层\r\n    /// </summary>\r\n    public partial class ");
            
            #line default
            #line hidden
            
            #line 14 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 14 "中文名Dal.tt"
            this.Write("Dal : DbBase\r\n    {\r\n\t\t#region 定义\r\n\r\n        public ");
            
            #line default
            #line hidden
            
            #line 18 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 18 "中文名Dal.tt"
            this.Write("Dal(IDbConnection dbCon = null) : base(dbCon)\r\n        {\r\n\t\t}\r\n\r\n        #endregi" +
                    "on\r\n\r\n\t\t#region 查询\r\n\r\n        #region 按键及索引 查询\r\n");
            
            #line default
            #line hidden
            
            #line 27 "中文名Dal.tt"

foreach(IDataIndex di in Table.Indexes){
	if(di.Columns==null||di.Columns.Length<1)continue;

	var idxName = FormatUtil.ToCodeName(di.Name);
	var keyFields = Table.Columns.Where(p => di.Columns.Contains(p.Name));

	// 函数参数列表
	var paraList = keyFields.Select(p => string.Format("{0} {1}",
			FormatUtil.ToFieldTypeString(p),
			FormatUtil.ToParamName(p.Name)));
	var strParas = String.Join(",", paraList);

	// sql语句中参数
	var sqlParaList = keyFields.Select(p => string.Format("{0}=@{1}",
			p.Name,
			FormatUtil.ToCodeName(p.Name)));
	var strSqlPara = String.Join(" and ", sqlParaList);

	// sql对象
	var sqlObjList = keyFields.Select(p => string.Format("{0} = {1}",
			FormatUtil.ToCodeName(p.Name),
			FormatUtil.ToParamName(p.Name)));
	var strSqlObj = String.Join(" , ", sqlObjList);

	if(di.Name.ToLower() == "primary"){
	
            
            #line default
            #line hidden
            
            #line 53 "中文名Dal.tt"
            this.Write("\r\n\t\t/// <summary>\r\n        /// 根据主键获取实体\r\n        /// </summary>\r\n\t");
            
            #line default
            #line hidden
            
            #line 57 "中文名Dal.tt"

	foreach(IDataColumn Field in keyFields){
		String colDescr=Field.Description;
        if(!String.IsNullOrEmpty(colDescr)) colDescr=colDescr.Replace("\r\n"," ").Replace("\\", "\\\\").Replace("'", "").Replace("\"", "");	
	
            
            #line default
            #line hidden
            
            #line 61 "中文名Dal.tt"
            this.Write("\t/// <param name=\"");
            
            #line default
            #line hidden
            
            #line 61 "中文名Dal.tt"
            this.Write(FormatUtil.ToParamName(Field.Name));
            
            #line default
            #line hidden
            
            #line 61 "中文名Dal.tt"
            this.Write("\">");
            
            #line default
            #line hidden
            
            #line 61 "中文名Dal.tt"
            this.Write(colDescr);
            
            #line default
            #line hidden
            
            #line 61 "中文名Dal.tt"
            this.Write("</param>\r\n\t");
            
            #line default
            #line hidden
            
            #line 62 "中文名Dal.tt"
}
            
            #line default
            #line hidden
            
            #line 62 "中文名Dal.tt"
            this.Write("\t/// <param name=\"tran\">事务</param>\r\n        /// <returns></returns>\r\n        publ" +
                    "ic virtual ");
            
            #line default
            #line hidden
            
            #line 64 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 64 "中文名Dal.tt"
            this.Write(" GetByPk(");
            
            #line default
            #line hidden
            
            #line 64 "中文名Dal.tt"
            this.Write(strParas);
            
            #line default
            #line hidden
            
            #line 64 "中文名Dal.tt"
            this.Write(", IDbTransaction tran = null)\r\n        {\r\n            const string format = \"SELE" +
                    "CT * FROM {0} WHERE ");
            
            #line default
            #line hidden
            
            #line 66 "中文名Dal.tt"
            this.Write(strSqlPara);
            
            #line default
            #line hidden
            
            #line 66 "中文名Dal.tt"
            this.Write("\";\r\n\r\n            var sql = string.Format(format, ");
            
            #line default
            #line hidden
            
            #line 68 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 68 "中文名Dal.tt"
            this.Write("._.DataBaseTableName);\r\n\r\n            return DbConn.QueryFirst<");
            
            #line default
            #line hidden
            
            #line 70 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 70 "中文名Dal.tt"
            this.Write(">(\r\n                sql: sql,\r\n                param: new {");
            
            #line default
            #line hidden
            
            #line 72 "中文名Dal.tt"
            this.Write(strSqlObj);
            
            #line default
            #line hidden
            
            #line 72 "中文名Dal.tt"
            this.Write("},\r\n                transaction: tran);\r\n        }\r\n");
            
            #line default
            #line hidden
            
            #line 75 "中文名Dal.tt"
			
	}else if(di.Unique){

            
            #line default
            #line hidden
            
            #line 77 "中文名Dal.tt"
            this.Write("\r\n\t\t/// <summary>\r\n        /// 根据唯一索引获取实体\r\n        /// </summary>\r\n\t");
            
            #line default
            #line hidden
            
            #line 81 "中文名Dal.tt"

	foreach(IDataColumn Field in keyFields){
		String colDescr=Field.Description;
        if(!String.IsNullOrEmpty(colDescr)) colDescr=colDescr.Replace("\r\n"," ").Replace("\\", "\\\\").Replace("'", "").Replace("\"", "");	
	
            
            #line default
            #line hidden
            
            #line 85 "中文名Dal.tt"
            this.Write("\t/// <param name=\"");
            
            #line default
            #line hidden
            
            #line 85 "中文名Dal.tt"
            this.Write(FormatUtil.ToParamName(Field.Name));
            
            #line default
            #line hidden
            
            #line 85 "中文名Dal.tt"
            this.Write("\">");
            
            #line default
            #line hidden
            
            #line 85 "中文名Dal.tt"
            this.Write(colDescr);
            
            #line default
            #line hidden
            
            #line 85 "中文名Dal.tt"
            this.Write("</param>\r\n\t");
            
            #line default
            #line hidden
            
            #line 86 "中文名Dal.tt"
}
            
            #line default
            #line hidden
            
            #line 86 "中文名Dal.tt"
            this.Write("\t/// <param name=\"tran\">事务</param>\r\n        /// <returns></returns>\r\n        publ" +
                    "ic virtual ");
            
            #line default
            #line hidden
            
            #line 88 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 88 "中文名Dal.tt"
            this.Write(" GetByUk");
            
            #line default
            #line hidden
            
            #line 88 "中文名Dal.tt"
            this.Write(idxName);
            
            #line default
            #line hidden
            
            #line 88 "中文名Dal.tt"
            this.Write("(");
            
            #line default
            #line hidden
            
            #line 88 "中文名Dal.tt"
            this.Write(strParas);
            
            #line default
            #line hidden
            
            #line 88 "中文名Dal.tt"
            this.Write(", IDbTransaction tran = null)\r\n        {\r\n            const string format = \"SELE" +
                    "CT * FROM {0} WHERE ");
            
            #line default
            #line hidden
            
            #line 90 "中文名Dal.tt"
            this.Write(strSqlPara);
            
            #line default
            #line hidden
            
            #line 90 "中文名Dal.tt"
            this.Write("\";\r\n\r\n            var sql = string.Format(format, ");
            
            #line default
            #line hidden
            
            #line 92 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 92 "中文名Dal.tt"
            this.Write("._.DataBaseTableName);\r\n\r\n            return DbConn.QueryFirst<");
            
            #line default
            #line hidden
            
            #line 94 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 94 "中文名Dal.tt"
            this.Write(">(\r\n                sql: sql,\r\n                param: new {");
            
            #line default
            #line hidden
            
            #line 96 "中文名Dal.tt"
            this.Write(strSqlObj);
            
            #line default
            #line hidden
            
            #line 96 "中文名Dal.tt"
            this.Write("},\r\n                transaction: tran);\r\n        }\r\n");
            
            #line default
            #line hidden
            
            #line 99 "中文名Dal.tt"

	}else{

            
            #line default
            #line hidden
            
            #line 101 "中文名Dal.tt"
            this.Write("\r\n\t\t/// <summary>\r\n        /// 根据索引获取实体列表\r\n        /// </summary>\r\n\t");
            
            #line default
            #line hidden
            
            #line 105 "中文名Dal.tt"

	foreach(IDataColumn Field in keyFields){
		String colDescr=Field.Description;
        if(!String.IsNullOrEmpty(colDescr)) colDescr=colDescr.Replace("\r\n"," ").Replace("\\", "\\\\").Replace("'", "").Replace("\"", "");	
	
            
            #line default
            #line hidden
            
            #line 109 "中文名Dal.tt"
            this.Write("\t/// <param name=\"");
            
            #line default
            #line hidden
            
            #line 109 "中文名Dal.tt"
            this.Write(FormatUtil.ToParamName(Field.Name));
            
            #line default
            #line hidden
            
            #line 109 "中文名Dal.tt"
            this.Write("\">");
            
            #line default
            #line hidden
            
            #line 109 "中文名Dal.tt"
            this.Write(colDescr);
            
            #line default
            #line hidden
            
            #line 109 "中文名Dal.tt"
            this.Write("</param>\r\n\t");
            
            #line default
            #line hidden
            
            #line 110 "中文名Dal.tt"
}
            
            #line default
            #line hidden
            
            #line 110 "中文名Dal.tt"
            this.Write("\t/// <param name=\"top\">获取行数(默认为0，即所有)</param>\r\n        /// <param name=\"sort\">排序方" +
                    "式(不包含关键字Order By)</param>\r\n\t\t/// <param name=\"tran\">事务</param>\r\n        /// <ret" +
                    "urns></returns>\r\n        public virtual IEnumerable<");
            
            #line default
            #line hidden
            
            #line 114 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 114 "中文名Dal.tt"
            this.Write("> GetByIdx");
            
            #line default
            #line hidden
            
            #line 114 "中文名Dal.tt"
            this.Write(idxName);
            
            #line default
            #line hidden
            
            #line 114 "中文名Dal.tt"
            this.Write("(");
            
            #line default
            #line hidden
            
            #line 114 "中文名Dal.tt"
            this.Write(strParas);
            
            #line default
            #line hidden
            
            #line 114 "中文名Dal.tt"
            this.Write(", int top = 0, string sort = null, IDbTransaction tran = null)\r\n        {\r\n      " +
                    "      const string format = \"SELECT * FROM {0} WHERE ");
            
            #line default
            #line hidden
            
            #line 116 "中文名Dal.tt"
            this.Write(strSqlPara);
            
            #line default
            #line hidden
            
            #line 116 "中文名Dal.tt"
            this.Write(@" {1} {2}"";

			var sortClause = string.Empty;
            if (!string.IsNullOrWhiteSpace(sort))
            {
                sortClause = ""ORDER BY "" + sort;
            }

            var limitClause = string.Empty;
            if (top > 0)
            {
                limitClause = ""LIMIT "" + top;
            }

            var sql = string.Format(format, 
				");
            
            #line default
            #line hidden
            
            #line 131 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 131 "中文名Dal.tt"
            this.Write("._.DataBaseTableName,\r\n\t\t\t\tsortClause, limitClause);\r\n\r\n            return DbConn" +
                    ".Query<");
            
            #line default
            #line hidden
            
            #line 134 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 134 "中文名Dal.tt"
            this.Write(">(\r\n                sql: sql,\r\n                param: new {");
            
            #line default
            #line hidden
            
            #line 136 "中文名Dal.tt"
            this.Write(strSqlObj);
            
            #line default
            #line hidden
            
            #line 136 "中文名Dal.tt"
            this.Write("},\r\n                transaction: tran);\r\n        }\r\n");
            
            #line default
            #line hidden
            
            #line 139 "中文名Dal.tt"

	}
}

            
            #line default
            #line hidden
            
            #line 142 "中文名Dal.tt"
            this.Write(@"
        #endregion

        #region 自定义查询

        /// <summary>
        /// 自定义条件查询
        /// </summary>
        /// <param name=""where"">自定义条件，where子句（不包含关键字Where）</param>
        /// <param name=""param"">参数（对象属性自动转为sql中的参数，eg：new {Id=10},则执行sql会转为参数对象 @Id,值为10）</param>
        /// <param name=""top"">获取行数(默认为0，即所有)</param>
        /// <param name=""sort"">排序方式(不包含关键字Order By)</param>
        /// <param name=""tran"">事务</param>
        /// <returns></returns>
        public virtual IEnumerable<");
            
            #line default
            #line hidden
            
            #line 156 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 156 "中文名Dal.tt"
            this.Write(@"> GetTopSort(string where, object param = null,
            int top = 0, string sort = null, IDbTransaction tran = null)
        {
            const string format = ""SELECT * FROM {0} {1} {2} {3}"";

            var whereClause = string.Empty;
            if (!string.IsNullOrWhiteSpace(where))
            {
                whereClause = where.Trim();

                if (!whereClause.StartsWith(""where"", StringComparison.OrdinalIgnoreCase))
                {
                    whereClause = ""WHERE "" + whereClause;
                }
            }

            var sortClause = string.Empty;
            if (!string.IsNullOrWhiteSpace(sort))
            {
                sortClause = ""ORDER BY "" + sort;
            }

            var limitClause = string.Empty;
            if (top > 0)
            {
                limitClause = ""LIMIT "" + top;
            }

            var sql = string.Format(format,
                ");
            
            #line default
            #line hidden
            
            #line 185 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 185 "中文名Dal.tt"
            this.Write("._.DataBaseTableName,\r\n                whereClause, sortClause, limitClause);\r\n\r\n" +
                    "            return DbConn.Query<");
            
            #line default
            #line hidden
            
            #line 188 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 188 "中文名Dal.tt"
            this.Write(@">(
                sql: sql,
                param: param,
                transaction: tran);
        }

        #endregion

        #region 分页

        /// <summary>
        /// 分页信息
        /// </summary>
        /// <param name=""pageSize"">每页条数</param>
        /// <param name=""where"">过滤条件</param>
        /// <param name=""param"">参数（对象属性自动转为sql中的参数，eg：new {Id=10},则执行sql会转为参数对象 @Id,值为10）</param>
        /// <returns>
        /// Item1: 总记录数
        /// Item2: 页数
        /// </returns>
        public virtual Tuple<Int64, Int64> GetPageInfo(int pageSize, string where = null, object param = null)
        {
            const string format = @""SELECT COUNT(*) FROM {0} {1}"";

            var whereClause = string.Empty;
            if (!string.IsNullOrWhiteSpace(where))
            {
                whereClause = where.Trim();

                if (!whereClause.StartsWith(""where"", StringComparison.OrdinalIgnoreCase))
                {
                    whereClause = ""WHERE "" + whereClause;
                }
            }

            var sql = string.Format(format,
                ");
            
            #line default
            #line hidden
            
            #line 224 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 224 "中文名Dal.tt"
            this.Write(@"._.DataBaseTableName,
                whereClause);

            var recordCount = DbConn.ExecuteScalar<Int64>(sql, param);

            var pageCount = 1L;
            if (pageSize != 0)
            {
                var lastPageCount = recordCount%pageSize;
                pageCount = recordCount/pageSize + (lastPageCount > 0 ? 1 : 0);
            }

            return new Tuple<long, long>(recordCount, pageCount);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name=""pageIndex"">页索引（从1开始）</param>
        /// <param name=""pageSize"">每页条数</param>
        /// <param name=""where"">过滤条件</param>
        /// <param name=""param"">参数（对象属性自动转为sql中的参数，eg：new {Id=10},则执行sql会转为参数对象 @Id,值为10）</param>
        /// <param name=""sort"">排序方式(不包含关键字Order By)</param>
        /// <returns></returns>
        public virtual IEnumerable<");
            
            #line default
            #line hidden
            
            #line 248 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 248 "中文名Dal.tt"
            this.Write(@"> GetPageList(Int64 pageIndex, int pageSize,
            string where = null, object param = null, string sort = null)
        {
            const string format = ""SELECT * FROM {0} {1} {2} {3};"";

            var whereClause = string.Empty;
            if (!string.IsNullOrWhiteSpace(where))
            {
                whereClause = where.Trim();

                if (!whereClause.StartsWith(""where"", StringComparison.OrdinalIgnoreCase))
                {
                    whereClause = ""WHERE "" + whereClause;
                }
            }

            var sortClause = string.Empty;
            if (!string.IsNullOrWhiteSpace(sort))
            {
                sortClause = ""ORDER BY "" + sort;
            }

            var limitClause = string.Empty;
            if (pageIndex > 0 && pageSize > 0)
            {
                limitClause = $""LIMIT {(pageSize - 1L)*pageSize},{pageSize}"";
            }

            var sql = string.Format(format,
                ");
            
            #line default
            #line hidden
            
            #line 277 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 277 "中文名Dal.tt"
            this.Write("._.DataBaseTableName,\r\n                whereClause, sortClause, limitClause);\r\n\r\n" +
                    "            return DbConn.Query<");
            
            #line default
            #line hidden
            
            #line 280 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 280 "中文名Dal.tt"
            this.Write(">(\r\n                sql: sql,\r\n                param: param);\r\n        }\r\n\r\n     " +
                    "   #endregion\r\n\r\n        #endregion\r\n");
            
            #line default
            #line hidden
            
            #line 288 "中文名Dal.tt"

// sql 列名（a_b,ab_cd）
String strSqlColumns = String.Join(",", Table.Columns.Where(p => !p.Identity).Select(s => s.ColumnName));

// sql 参数（@AB,@AbCd）
String strSqlParas = String.Join(",", Table.Columns.Where(p => !p.Identity).Select(s => "@" + FormatUtil.ToCodeName(s.Name)));

// 自增列
IDataColumn IdentityField = Table.Columns.FirstOrDefault(p => p.Identity);

// 主键列表
IEnumerable<IDataColumn> PrimaryKeyFields = Table.Columns.Where(p => p.PrimaryKey);

// 获取自增Value语句
String strSqlLastInsertId = IdentityField == null ? string.Empty : "SELECT LAST_INSERT_ID();";

// sql keyvalue语句（columnname=@CodeName,columnname2=@CodeName2)
String strSqlKvs = String.Join(",", Table.Columns.Where(p => !p.Identity).Select(s => string.Format("{0}=@{1}", s.ColumnName, FormatUtil.ToCodeName(s.Name))));

// sql 主键的keyvalue语句（columnname=@OriginalCodeName,columnname2=@OriginalCodeName2)
String strSqlKeyKvs = String.Join(" AND ", PrimaryKeyFields.Select(s => string.Format("{0}=@Original{1}", s.ColumnName, FormatUtil.ToCodeName(s.Name))));
String strSqlKeyObj = String.Join(" , ", PrimaryKeyFields.Select(s => string.Format("Original{0} = {1}", FormatUtil.ToCodeName(s.Name),FormatUtil.ToParamName(s.Name))));

// 主键参数（int id,string codeName）
String strKeyParam = String.Join(",", PrimaryKeyFields.Select(s => string.Format("{0} {1}", FormatUtil.ToFieldTypeString(s), FormatUtil.ToParamName(s.Name))));

            
            #line default
            #line hidden
            
            #line 313 "中文名Dal.tt"
            this.Write("\r\n\t\t#region Add\r\n\r\n        /// <summary>\r\n        /// 添加\r\n        /// </summary>\r" +
                    "\n        /// <param name=\"item\">实体</param>\r\n        /// <param name=\"tran\">事务</p" +
                    "aram>\r\n        /// <returns></returns>\r\n        public virtual int Add(");
            
            #line default
            #line hidden
            
            #line 322 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 322 "中文名Dal.tt"
            this.Write(" item, IDbTransaction tran = null)\r\n        {\r\n            const string format = " +
                    "@\"INSERT INTO {0}(");
            
            #line default
            #line hidden
            
            #line 324 "中文名Dal.tt"
            this.Write(strSqlColumns);
            
            #line default
            #line hidden
            
            #line 324 "中文名Dal.tt"
            this.Write(") \r\n\t\t\t\tVALUES(");
            
            #line default
            #line hidden
            
            #line 325 "中文名Dal.tt"
            this.Write(strSqlParas);
            
            #line default
            #line hidden
            
            #line 325 "中文名Dal.tt"
            this.Write(");\r\n\t\t\t\t");
            
            #line default
            #line hidden
            
            #line 326 "中文名Dal.tt"
            this.Write(strSqlLastInsertId);
            
            #line default
            #line hidden
            
            #line 326 "中文名Dal.tt"
            this.Write("\";\r\n\r\n            var sql = string.Format(format, ");
            
            #line default
            #line hidden
            
            #line 328 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 328 "中文名Dal.tt"
            this.Write("._.DataBaseTableName);\r\n\r\n");
            
            #line default
            #line hidden
            
            #line 330 "中文名Dal.tt"
if(IdentityField != null) {
            
            #line default
            #line hidden
            
            #line 330 "中文名Dal.tt"
            this.Write("\r\n\t\t\titem.");
            
            #line default
            #line hidden
            
            #line 331 "中文名Dal.tt"
            this.Write(FormatUtil.ToCodeName(IdentityField.Name));
            
            #line default
            #line hidden
            
            #line 331 "中文名Dal.tt"
            this.Write(" = DbConn.ExecuteScalar<");
            
            #line default
            #line hidden
            
            #line 331 "中文名Dal.tt"
            this.Write(IdentityField.DataType.Name);
            
            #line default
            #line hidden
            
            #line 331 "中文名Dal.tt"
            this.Write(">(sql, param: item, transaction: tran);\r\n");
            
            #line default
            #line hidden
            
            #line 332 "中文名Dal.tt"

}

foreach(IDataColumn Field in Table.Columns.Where(p => p.PrimaryKey))
{
	var fieldName = FormatUtil.ToCodeName(Field.Name);

            
            #line default
            #line hidden
            
            #line 338 "中文名Dal.tt"
            this.Write("\t\t\titem.Original");
            
            #line default
            #line hidden
            
            #line 338 "中文名Dal.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 338 "中文名Dal.tt"
            this.Write(" = item.");
            
            #line default
            #line hidden
            
            #line 338 "中文名Dal.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 338 "中文名Dal.tt"
            this.Write(";\r\n");
            
            #line default
            #line hidden
            
            #line 339 "中文名Dal.tt"

}
            
            #line default
            #line hidden
            
            #line 340 "中文名Dal.tt"
            this.Write(@"
            return 1;
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name=""items"">实体列表</param>
        /// <param name=""tran"">事务</param>
        /// <returns></returns>
        public virtual int Add(IEnumerable<");
            
            #line default
            #line hidden
            
            #line 350 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 350 "中文名Dal.tt"
            this.Write(@"> items, IDbTransaction tran = null)
        {
            var count = 0;
            foreach (var item in items)
            {
                Add(item, tran);
                count++;
            }

            return count;
        }

        #endregion

        #region Update

        /// <summary>
        /// 更新（根据原始主键OriginalXXX更新其它字段信息）
        /// </summary>
        /// <param name=""item"">实体对象</param>
        /// <param name=""tran"">事务</param>
        /// <returns></returns>
        public virtual int Update(");
            
            #line default
            #line hidden
            
            #line 372 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 372 "中文名Dal.tt"
            this.Write(" item, IDbTransaction tran = null)\r\n        {\r\n            const string format = " +
                    "@\"UPDATE {0} \r\n\t\t\t\t\tSET ");
            
            #line default
            #line hidden
            
            #line 375 "中文名Dal.tt"
            this.Write(strSqlKvs);
            
            #line default
            #line hidden
            
            #line 375 "中文名Dal.tt"
            this.Write(" \r\n\t\t\t\t\tWHERE ");
            
            #line default
            #line hidden
            
            #line 376 "中文名Dal.tt"
            this.Write(strSqlKeyKvs);
            
            #line default
            #line hidden
            
            #line 376 "中文名Dal.tt"
            this.Write(";\";\r\n\r\n            var sql = string.Format(format, ");
            
            #line default
            #line hidden
            
            #line 378 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 378 "中文名Dal.tt"
            this.Write(@"._.DataBaseTableName);

            return DbConn.Execute(sql, param: item, transaction: tran);
        }

        /// <summary>
        /// 更新（根据原始主键OriginalXXX更新包含的字段列表）
        /// </summary>
        /// <param name=""item"">仅更新的字段、OriginalXXX字段</param>
        /// <param name=""nameList"">包含的name列表</param>
        /// <param name=""tran"">事务</param>
        /// <returns></returns>
        public virtual int Update(");
            
            #line default
            #line hidden
            
            #line 390 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 390 "中文名Dal.tt"
            this.Write(" item, IList<string> nameList, IDbTransaction tran = null)\r\n        {\r\n          " +
                    "  if (nameList == null)\r\n            {\r\n                return Update(item, tran" +
                    ");\r\n            }\r\n\r\n            var curFieldList = ");
            
            #line default
            #line hidden
            
            #line 397 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 397 "中文名Dal.tt"
            this.Write(@"._.AllFields.Where(f => nameList.Contains(f.Name) && !f.IsReadonly);
            if (!curFieldList.Any())
            {
                return 0;
            }

            const string format = ""UPDATE {0} SET {1} WHERE {2};"";

            var setClause = curFieldList.Aggregate(string.Empty,
                (raw, p) => $""{raw},{p.ColumnName}=@{p.Name}"",
                last => last.Trim(','));

            var originalKeys = ");
            
            #line default
            #line hidden
            
            #line 409 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 409 "中文名Dal.tt"
            this.Write(@"._.AllFields.Where(p => p.IsPrimaryKey && p.IsReadonly);
            var whereClause = originalKeys.Aggregate(string.Empty,
                (raw, p) => $""{raw} and {p.ColumnName}=@{p.Name}"",
                last => last.Trim().Substring(4));

            var sql = string.Format(format,
                ");
            
            #line default
            #line hidden
            
            #line 415 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 415 "中文名Dal.tt"
            this.Write(@"._.DataBaseTableName,
                setClause, whereClause);

            return DbConn.Execute(sql, param: item, transaction: tran);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name=""items"">实体对象集合</param>
        /// <param name=""tran"">事务</param>
        /// <returns></returns>
        public virtual int Update(IEnumerable<");
            
            #line default
            #line hidden
            
            #line 427 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 427 "中文名Dal.tt"
            this.Write(@"> items, IDbTransaction tran = null)
        {
            var count = 0;
            foreach (var item in items)
            {
                Update(item, tran);
                count++;
            }

            return count;
        }

        /// <summary>
        /// 自定义更新
        /// </summary>
        /// <param name=""item"">实体对象（仅更新的字段、Where字段）</param>
        /// <param name=""strSet"">set语句（不含set关键字，可以用sql参数，Eg：cloumn_name=@CloumnName）</param>
        /// <param name=""strWhere"">where语句（不含where关键字，可以用sql参数，Eg：id=@Id）</param>
        /// <param name=""tran"">事务</param>
        /// <returns></returns>
        public virtual int Update(");
            
            #line default
            #line hidden
            
            #line 447 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 447 "中文名Dal.tt"
            this.Write(@" item, string strSet, string strWhere, IDbTransaction tran = null)
        {
            const string format = ""UPDATE {0} SET {1} {2};"";

            if (string.IsNullOrWhiteSpace(strSet))
            {
                return 0;
            }

            var whereClause = string.Empty;
            if (!string.IsNullOrWhiteSpace(strWhere))
            {
                whereClause = strWhere.Trim();

                if (!whereClause.StartsWith(""where"", StringComparison.OrdinalIgnoreCase))
                {
                    whereClause = ""WHERE "" + whereClause;
                }
            }

            var sql = string.Format(format,
                ");
            
            #line default
            #line hidden
            
            #line 468 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 468 "中文名Dal.tt"
            this.Write("._.DataBaseTableName,\r\n                strSet, whereClause);\r\n\r\n            retur" +
                    "n DbConn.Execute(sql, param: item, transaction: tran);\r\n        }\r\n\r\n        #en" +
                    "dregion\r\n\r\n        #region Remove\r\n");
            
            #line default
            #line hidden
            
            #line 477 "中文名Dal.tt"

// 含有主键时，支持主键删除功能
if(PrimaryKeyFields.Count() > 0)
{

            
            #line default
            #line hidden
            
            #line 481 "中文名Dal.tt"
            this.Write("\r\n\t    /// <summary>\r\n        /// 按主键删除\r\n        /// </summary>");
            
            #line default
            #line hidden
            
            #line 484 "中文名Dal.tt"

	foreach(IDataColumn Field in Table.Columns.Where(p => p.PrimaryKey))
	{
		String colDescr = FormatUtil.ToSigleDisplay(Field.Description);
            
            #line default
            #line hidden
            
            #line 487 "中文名Dal.tt"
            this.Write("\r\n\t\t/// <param name=\"");
            
            #line default
            #line hidden
            
            #line 488 "中文名Dal.tt"
            this.Write(FormatUtil.ToParamName(Field.Name));
            
            #line default
            #line hidden
            
            #line 488 "中文名Dal.tt"
            this.Write("\">");
            
            #line default
            #line hidden
            
            #line 488 "中文名Dal.tt"
            this.Write(colDescr);
            
            #line default
            #line hidden
            
            #line 488 "中文名Dal.tt"
            this.Write("</param>\r\n\t");
            
            #line default
            #line hidden
            
            #line 489 "中文名Dal.tt"
}
            
            #line default
            #line hidden
            
            #line 489 "中文名Dal.tt"
            this.Write("/// <param name=\"tran\">事务</param>\r\n        /// <returns></returns>\r\n        publi" +
                    "c virtual int RemoveByPk(");
            
            #line default
            #line hidden
            
            #line 491 "中文名Dal.tt"
            this.Write(strKeyParam);
            
            #line default
            #line hidden
            
            #line 491 "中文名Dal.tt"
            this.Write(", IDbTransaction tran = null)\r\n        {\r\n            const string format = @\"DEL" +
                    "ETE FROM {0} WHERE ");
            
            #line default
            #line hidden
            
            #line 493 "中文名Dal.tt"
            this.Write(strSqlKeyKvs);
            
            #line default
            #line hidden
            
            #line 493 "中文名Dal.tt"
            this.Write(";\";\r\n\r\n            var sql = string.Format(format, ");
            
            #line default
            #line hidden
            
            #line 495 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 495 "中文名Dal.tt"
            this.Write("._.DataBaseTableName);\r\n\r\n            return DbConn.Execute(sql, param: new {");
            
            #line default
            #line hidden
            
            #line 497 "中文名Dal.tt"
            this.Write(strSqlKeyObj);
            
            #line default
            #line hidden
            
            #line 497 "中文名Dal.tt"
            this.Write("}, transaction: tran);\r\n        }\r\n\t");
            
            #line default
            #line hidden
            
            #line 499 "中文名Dal.tt"

	// 主键 只有一列时，支持批量删除功能
	if(PrimaryKeyFields.Count() == 1)
	{
		String keyParam = FormatUtil.ToParamName(PrimaryKeyFields.First().Name) + "s";
	
            
            #line default
            #line hidden
            
            #line 504 "中文名Dal.tt"
            this.Write("\r\n\t\t/// <summary>\r\n        /// 按主键批量删除\r\n        /// </summary>\r\n        /// <para" +
                    "m name=\"");
            
            #line default
            #line hidden
            
            #line 508 "中文名Dal.tt"
            this.Write(keyParam);
            
            #line default
            #line hidden
            
            #line 508 "中文名Dal.tt"
            this.Write("\">");
            
            #line default
            #line hidden
            
            #line 508 "中文名Dal.tt"
            this.Write(FormatUtil.ToSigleDisplay(PrimaryKeyFields.First().Description));
            
            #line default
            #line hidden
            
            #line 508 "中文名Dal.tt"
            this.Write("列表</param>\r\n        /// <param name=\"tran\">事务</param>\r\n        /// <returns></ret" +
                    "urns>\r\n        public virtual int RemoveByPks(IEnumerable<");
            
            #line default
            #line hidden
            
            #line 511 "中文名Dal.tt"
            this.Write(FormatUtil.ToFieldTypeString(PrimaryKeyFields.First()));
            
            #line default
            #line hidden
            
            #line 511 "中文名Dal.tt"
            this.Write("> ");
            
            #line default
            #line hidden
            
            #line 511 "中文名Dal.tt"
            this.Write(keyParam);
            
            #line default
            #line hidden
            
            #line 511 "中文名Dal.tt"
            this.Write(", IDbTransaction tran = null)\r\n        {\r\n            const string format = @\"DEL" +
                    "ETE FROM {0} WHERE ");
            
            #line default
            #line hidden
            
            #line 513 "中文名Dal.tt"
            this.Write(strSqlKeyKvs);
            
            #line default
            #line hidden
            
            #line 513 "中文名Dal.tt"
            this.Write(";\";\r\n\r\n            var sql = string.Format(format, ");
            
            #line default
            #line hidden
            
            #line 515 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 515 "中文名Dal.tt"
            this.Write("._.DataBaseTableName);\r\n\r\n            return DbConn.Execute(sql, param: ");
            
            #line default
            #line hidden
            
            #line 517 "中文名Dal.tt"
            this.Write(keyParam);
            
            #line default
            #line hidden
            
            #line 517 "中文名Dal.tt"
            this.Write(".Select(p => new {Original");
            
            #line default
            #line hidden
            
            #line 517 "中文名Dal.tt"
            this.Write(FormatUtil.ToCodeName(PrimaryKeyFields.First().Name));
            
            #line default
            #line hidden
            
            #line 517 "中文名Dal.tt"
            this.Write(" = p}), transaction: tran);\r\n        }\r\n\t");
            
            #line default
            #line hidden
            
            #line 519 "中文名Dal.tt"
}
}

            
            #line default
            #line hidden
            
            #line 521 "中文名Dal.tt"
            this.Write("\r\n\r\n");
            
            #line default
            #line hidden
            
            #line 523 "中文名Dal.tt"

// 按照索引 删除
foreach(IDataIndex di in Table.Indexes)
{
	if(di.Columns==null||di.Columns.Length<1)continue;



}

            
            #line default
            #line hidden
            
            #line 532 "中文名Dal.tt"
            this.Write(@"        


        /// <summary>
        /// 根据索引删除 idx_ip(ip)
        /// </summary>
        /// <param name=""ip""></param>
        /// <param name=""tran""></param>
        /// <returns></returns>
        public virtual int RemoveByIp(string ip, IDbTransaction tran = null)
        {
            const string format = @""DELETE FROM {0} WHERE ip=@Ip;"";

            var sql = string.Format(format, TbIpBlackList.__.DataBaseTableName);

            return DbConn.Execute(sql, param: new {Ip = ip}, transaction: tran);
        }

        /// <summary>
        /// 根据索引批量删除 idex_ip(ip)
        /// </summary>
        /// <param name=""ips""></param>
        /// <param name=""tran""></param>
        /// <returns></returns>
        public virtual int RemoveByIps(IEnumerable<string> ips, IDbTransaction tran = null)
        {
            const string format = @""DELETE FROM {0} WHERE ip=@Ip;"";

            var sql = string.Format(format, ");
            
            #line default
            #line hidden
            
            #line 560 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 560 "中文名Dal.tt"
            this.Write(@"._.DataBaseTableName);

            return DbConn.Execute(sql, param: ips.Select(p => new {Ip = p}), transaction: tran);
        }

        /// <summary>
        /// 自定义条件删除
        /// </summary>
        /// <param name=""where"">自定义条件，where子句（不包含关键字Where）</param>
        /// <param name=""param"">参数（对象属性自动转为sql中的参数，eg：new {Id=10},则执行sql会转为参数对象 @Id,值为10）</param>
        /// <param name=""tran"">事务</param>
        /// <returns></returns>
        public virtual int Remove(string where, object param = null, IDbTransaction tran = null)
        {
            const string format = @""DELETE FROM {0} {1};"";

            var whereClause = string.Empty;
            if (!string.IsNullOrWhiteSpace(where))
            {
                whereClause = where.Trim();

                if (!whereClause.StartsWith(""where"", StringComparison.OrdinalIgnoreCase))
                {
                    whereClause = ""WHERE "" + whereClause;
                }
            }

            var sql = string.Format(format,
                ");
            
            #line default
            #line hidden
            
            #line 588 "中文名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 588 "中文名Dal.tt"
            this.Write("._.DataBaseTableName,\r\n                whereClause);\r\n\r\n            return DbConn" +
                    ".Execute(sql, param: param, transaction: tran);\r\n        }\r\n\r\n        #endregion" +
                    "\r\n    }\r\n}");
            
            #line default
            #line hidden
            return this.Output.ToString();
        }
    }
    
    #line default
    #line hidden
}
