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
    
    
    #line 1 "类名Dal.tt"
    public class 类名Dal : XCoder.XCoderBase
    {
        public override string Render()
        {
            
            #line 1 "类名Dal.tt"
            this.Write("/*\r\n * XCoder v");
            
            #line default
            #line hidden
            
            #line 2 "类名Dal.tt"
            this.Write(Version);
            
            #line default
            #line hidden
            
            #line 2 "类名Dal.tt"
            this.Write("\r\n * 作者：");
            
            #line default
            #line hidden
            
            #line 3 "类名Dal.tt"
            this.Write(Environment.UserName + "/" + Environment.MachineName);
            
            #line default
            #line hidden
            
            #line 3 "类名Dal.tt"
            this.Write("\r\n * 时间：");
            
            #line default
            #line hidden
            
            #line 4 "类名Dal.tt"
            this.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            
            #line default
            #line hidden
            
            #line 4 "类名Dal.tt"
            this.Write("\r\n * 版权：hardCTE 2016~");
            
            #line default
            #line hidden
            
            #line 5 "类名Dal.tt"
            this.Write(DateTime.Now.ToString("yyyy"));
            
            #line default
            #line hidden
            
            #line 5 "类名Dal.tt"
            this.Write("\r\n*/\r\n﻿using System;\r\nusing System.Collections.Generic;\r\nusing System.Data;\r\nusin" +
                    "g System.Linq;\r\nusing App.FrameCore;\r\nusing Dapper;\r\n\r\nnamespace ");
            
            #line default
            #line hidden
            
            #line 14 "类名Dal.tt"
            this.Write(Config.NameSpace);
            
            #line default
            #line hidden
            
            #line 14 "类名Dal.tt"
            this.Write("\r\n{");
            
            #line default
            #line hidden
            
            #line 15 "类名Dal.tt"

	var modelName = FormatUtil.ToCodeName(Table.Name);

            
            #line default
            #line hidden
            
            #line 17 "类名Dal.tt"
            this.Write("\r\n\t/// <summary>\r\n    /// ");
            
            #line default
            #line hidden
            
            #line 19 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 19 "类名Dal.tt"
            this.Write(" 数据访问层\r\n    /// </summary>\r\n    public partial class ");
            
            #line default
            #line hidden
            
            #line 21 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 21 "类名Dal.tt"
            this.Write("Dal : DbBase\r\n    {\r\n\t\t#region 定义\r\n\r\n        public ");
            
            #line default
            #line hidden
            
            #line 25 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 25 "类名Dal.tt"
            this.Write("Dal(IDbConnection dbCon = null) : base(dbCon)\r\n        {\r\n\t\t}\r\n\r\n        #endregi" +
                    "on\r\n\r\n\t\t#region 查询\r\n\r\n        #region 按键及索引 查询\r\n");
            
            #line default
            #line hidden
            
            #line 34 "类名Dal.tt"

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
            
            #line 60 "类名Dal.tt"
            this.Write("\r\n\t\t/// <summary>\r\n        /// 根据主键获取实体\r\n        /// </summary>\r\n\t");
            
            #line default
            #line hidden
            
            #line 64 "类名Dal.tt"

	foreach(IDataColumn Field in keyFields){
		String colDescr=Field.Description;
        if(!String.IsNullOrEmpty(colDescr)) colDescr=colDescr.Replace("\r\n"," ").Replace("\\", "\\\\").Replace("'", "").Replace("\"", "");	
	
            
            #line default
            #line hidden
            
            #line 68 "类名Dal.tt"
            this.Write("\t/// <param name=\"");
            
            #line default
            #line hidden
            
            #line 68 "类名Dal.tt"
            this.Write(FormatUtil.ToParamName(Field.Name));
            
            #line default
            #line hidden
            
            #line 68 "类名Dal.tt"
            this.Write("\">");
            
            #line default
            #line hidden
            
            #line 68 "类名Dal.tt"
            this.Write(colDescr);
            
            #line default
            #line hidden
            
            #line 68 "类名Dal.tt"
            this.Write("</param>\r\n\t");
            
            #line default
            #line hidden
            
            #line 69 "类名Dal.tt"
}
            
            #line default
            #line hidden
            
            #line 69 "类名Dal.tt"
            this.Write("\t/// <param name=\"tran\">事务</param>\r\n        /// <returns></returns>\r\n        publ" +
                    "ic virtual ");
            
            #line default
            #line hidden
            
            #line 71 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 71 "类名Dal.tt"
            this.Write(" GetByPk(");
            
            #line default
            #line hidden
            
            #line 71 "类名Dal.tt"
            this.Write(strParas);
            
            #line default
            #line hidden
            
            #line 71 "类名Dal.tt"
            this.Write(", IDbTransaction tran = null)\r\n        {\r\n            const string format = \"SELE" +
                    "CT * FROM {0} WHERE ");
            
            #line default
            #line hidden
            
            #line 73 "类名Dal.tt"
            this.Write(strSqlPara);
            
            #line default
            #line hidden
            
            #line 73 "类名Dal.tt"
            this.Write("\";\r\n\r\n            var sql = string.Format(format, ");
            
            #line default
            #line hidden
            
            #line 75 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 75 "类名Dal.tt"
            this.Write("._.DataBaseTableName);\r\n\r\n            return DbConn.QueryFirst<");
            
            #line default
            #line hidden
            
            #line 77 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 77 "类名Dal.tt"
            this.Write(">(\r\n                sql: sql,\r\n                param: new {");
            
            #line default
            #line hidden
            
            #line 79 "类名Dal.tt"
            this.Write(strSqlObj);
            
            #line default
            #line hidden
            
            #line 79 "类名Dal.tt"
            this.Write("},\r\n                transaction: tran);\r\n        }\r\n");
            
            #line default
            #line hidden
            
            #line 82 "类名Dal.tt"
			
	}else if(di.Unique){

            
            #line default
            #line hidden
            
            #line 84 "类名Dal.tt"
            this.Write("\r\n\t\t/// <summary>\r\n        /// 根据唯一索引获取实体\r\n        /// </summary>\r\n\t");
            
            #line default
            #line hidden
            
            #line 88 "类名Dal.tt"

	foreach(IDataColumn Field in keyFields){
		String colDescr=Field.Description;
        if(!String.IsNullOrEmpty(colDescr)) colDescr=colDescr.Replace("\r\n"," ").Replace("\\", "\\\\").Replace("'", "").Replace("\"", "");	
	
            
            #line default
            #line hidden
            
            #line 92 "类名Dal.tt"
            this.Write("\t/// <param name=\"");
            
            #line default
            #line hidden
            
            #line 92 "类名Dal.tt"
            this.Write(FormatUtil.ToParamName(Field.Name));
            
            #line default
            #line hidden
            
            #line 92 "类名Dal.tt"
            this.Write("\">");
            
            #line default
            #line hidden
            
            #line 92 "类名Dal.tt"
            this.Write(colDescr);
            
            #line default
            #line hidden
            
            #line 92 "类名Dal.tt"
            this.Write("</param>\r\n\t");
            
            #line default
            #line hidden
            
            #line 93 "类名Dal.tt"
}
            
            #line default
            #line hidden
            
            #line 93 "类名Dal.tt"
            this.Write("\t/// <param name=\"tran\">事务</param>\r\n        /// <returns></returns>\r\n        publ" +
                    "ic virtual ");
            
            #line default
            #line hidden
            
            #line 95 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 95 "类名Dal.tt"
            this.Write(" GetByUk");
            
            #line default
            #line hidden
            
            #line 95 "类名Dal.tt"
            this.Write(idxName);
            
            #line default
            #line hidden
            
            #line 95 "类名Dal.tt"
            this.Write("(");
            
            #line default
            #line hidden
            
            #line 95 "类名Dal.tt"
            this.Write(strParas);
            
            #line default
            #line hidden
            
            #line 95 "类名Dal.tt"
            this.Write(", IDbTransaction tran = null)\r\n        {\r\n            const string format = \"SELE" +
                    "CT * FROM {0} WHERE ");
            
            #line default
            #line hidden
            
            #line 97 "类名Dal.tt"
            this.Write(strSqlPara);
            
            #line default
            #line hidden
            
            #line 97 "类名Dal.tt"
            this.Write("\";\r\n\r\n            var sql = string.Format(format, ");
            
            #line default
            #line hidden
            
            #line 99 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 99 "类名Dal.tt"
            this.Write("._.DataBaseTableName);\r\n\r\n            return DbConn.QueryFirst<");
            
            #line default
            #line hidden
            
            #line 101 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 101 "类名Dal.tt"
            this.Write(">(\r\n                sql: sql,\r\n                param: new {");
            
            #line default
            #line hidden
            
            #line 103 "类名Dal.tt"
            this.Write(strSqlObj);
            
            #line default
            #line hidden
            
            #line 103 "类名Dal.tt"
            this.Write("},\r\n                transaction: tran);\r\n        }\r\n");
            
            #line default
            #line hidden
            
            #line 106 "类名Dal.tt"

	}else{

            
            #line default
            #line hidden
            
            #line 108 "类名Dal.tt"
            this.Write("\r\n\t\t/// <summary>\r\n        /// 根据索引获取实体列表\r\n        /// </summary>\r\n\t");
            
            #line default
            #line hidden
            
            #line 112 "类名Dal.tt"

	foreach(IDataColumn Field in keyFields){
		String colDescr=Field.Description;
        if(!String.IsNullOrEmpty(colDescr)) colDescr=colDescr.Replace("\r\n"," ").Replace("\\", "\\\\").Replace("'", "").Replace("\"", "");	
	
            
            #line default
            #line hidden
            
            #line 116 "类名Dal.tt"
            this.Write("\t/// <param name=\"");
            
            #line default
            #line hidden
            
            #line 116 "类名Dal.tt"
            this.Write(FormatUtil.ToParamName(Field.Name));
            
            #line default
            #line hidden
            
            #line 116 "类名Dal.tt"
            this.Write("\">");
            
            #line default
            #line hidden
            
            #line 116 "类名Dal.tt"
            this.Write(colDescr);
            
            #line default
            #line hidden
            
            #line 116 "类名Dal.tt"
            this.Write("</param>\r\n\t");
            
            #line default
            #line hidden
            
            #line 117 "类名Dal.tt"
}
            
            #line default
            #line hidden
            
            #line 117 "类名Dal.tt"
            this.Write("\t/// <param name=\"top\">获取行数(默认为0，即所有)</param>\r\n        /// <param name=\"sort\">排序方" +
                    "式(不包含关键字Order By)</param>\r\n\t\t/// <param name=\"tran\">事务</param>\r\n        /// <ret" +
                    "urns></returns>\r\n        public virtual IEnumerable<");
            
            #line default
            #line hidden
            
            #line 121 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 121 "类名Dal.tt"
            this.Write("> GetByIdx");
            
            #line default
            #line hidden
            
            #line 121 "类名Dal.tt"
            this.Write(idxName);
            
            #line default
            #line hidden
            
            #line 121 "类名Dal.tt"
            this.Write("(");
            
            #line default
            #line hidden
            
            #line 121 "类名Dal.tt"
            this.Write(strParas);
            
            #line default
            #line hidden
            
            #line 121 "类名Dal.tt"
            this.Write(", int top = 0, string sort = null, IDbTransaction tran = null)\r\n        {\r\n      " +
                    "      const string format = \"SELECT * FROM {0} WHERE ");
            
            #line default
            #line hidden
            
            #line 123 "类名Dal.tt"
            this.Write(strSqlPara);
            
            #line default
            #line hidden
            
            #line 123 "类名Dal.tt"
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
            
            #line 138 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 138 "类名Dal.tt"
            this.Write("._.DataBaseTableName,\r\n\t\t\t\tsortClause, limitClause);\r\n\r\n            return DbConn" +
                    ".Query<");
            
            #line default
            #line hidden
            
            #line 141 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 141 "类名Dal.tt"
            this.Write(">(\r\n                sql: sql,\r\n                param: new {");
            
            #line default
            #line hidden
            
            #line 143 "类名Dal.tt"
            this.Write(strSqlObj);
            
            #line default
            #line hidden
            
            #line 143 "类名Dal.tt"
            this.Write("},\r\n                transaction: tran);\r\n        }\r\n");
            
            #line default
            #line hidden
            
            #line 146 "类名Dal.tt"

	}
}

            
            #line default
            #line hidden
            
            #line 149 "类名Dal.tt"
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
            
            #line 163 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 163 "类名Dal.tt"
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
            
            #line 192 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 192 "类名Dal.tt"
            this.Write("._.DataBaseTableName,\r\n                whereClause, sortClause, limitClause);\r\n\r\n" +
                    "            return DbConn.Query<");
            
            #line default
            #line hidden
            
            #line 195 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 195 "类名Dal.tt"
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
            
            #line 231 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 231 "类名Dal.tt"
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
            
            #line 255 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 255 "类名Dal.tt"
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
            
            #line 284 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 284 "类名Dal.tt"
            this.Write("._.DataBaseTableName,\r\n                whereClause, sortClause, limitClause);\r\n\r\n" +
                    "            return DbConn.Query<");
            
            #line default
            #line hidden
            
            #line 287 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 287 "类名Dal.tt"
            this.Write(">(\r\n                sql: sql,\r\n                param: param);\r\n        }\r\n\r\n     " +
                    "   #endregion\r\n\r\n        #endregion\r\n");
            
            #line default
            #line hidden
            
            #line 295 "类名Dal.tt"

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
String strSqlColKvs = String.Join(",", Table.Columns.Where(p => !p.Identity).Select(s => string.Format("{0}=@{1}", s.ColumnName, FormatUtil.ToCodeName(s.Name))));

// sql 主键的keyvalue语句（columnname=@OriginalCodeName,columnname2=@OriginalCodeName2)
String strSqlKeyKvs = String.Join(" AND ", PrimaryKeyFields.Select(s => string.Format("{0}=@Original{1}", s.ColumnName, FormatUtil.ToCodeName(s.Name))));
String strSqlKeyObj = String.Join(" , ", PrimaryKeyFields.Select(s => string.Format("Original{0} = {1}", FormatUtil.ToCodeName(s.Name),FormatUtil.ToParamName(s.Name))));

// 主键参数（int id,string codeName）
String strKeyParam = String.Join(",", PrimaryKeyFields.Select(s => string.Format("{0} {1}", FormatUtil.ToFieldTypeString(s), FormatUtil.ToParamName(s.Name))));

            
            #line default
            #line hidden
            
            #line 320 "类名Dal.tt"
            this.Write("\r\n\t\t#region Add\r\n\r\n        /// <summary>\r\n        /// 添加\r\n        /// </summary>\r" +
                    "\n        /// <param name=\"item\">实体</param>\r\n        /// <param name=\"tran\">事务</p" +
                    "aram>\r\n        /// <returns></returns>\r\n        public virtual int Add(");
            
            #line default
            #line hidden
            
            #line 329 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 329 "类名Dal.tt"
            this.Write(" item, IDbTransaction tran = null)\r\n        {\r\n            const string format = " +
                    "@\"INSERT INTO {0}(");
            
            #line default
            #line hidden
            
            #line 331 "类名Dal.tt"
            this.Write(strSqlColumns);
            
            #line default
            #line hidden
            
            #line 331 "类名Dal.tt"
            this.Write(") \r\n\t\t\t\tVALUES(");
            
            #line default
            #line hidden
            
            #line 332 "类名Dal.tt"
            this.Write(strSqlParas);
            
            #line default
            #line hidden
            
            #line 332 "类名Dal.tt"
            this.Write(");\r\n\t\t\t\t");
            
            #line default
            #line hidden
            
            #line 333 "类名Dal.tt"
            this.Write(strSqlLastInsertId);
            
            #line default
            #line hidden
            
            #line 333 "类名Dal.tt"
            this.Write("\";\r\n\r\n            var sql = string.Format(format, ");
            
            #line default
            #line hidden
            
            #line 335 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 335 "类名Dal.tt"
            this.Write("._.DataBaseTableName);\r\n");
            
            #line default
            #line hidden
            
            #line 336 "类名Dal.tt"
if(IdentityField != null) {
            
            #line default
            #line hidden
            
            #line 336 "类名Dal.tt"
            this.Write("\r\n\t\t\titem.");
            
            #line default
            #line hidden
            
            #line 337 "类名Dal.tt"
            this.Write(FormatUtil.ToCodeName(IdentityField.Name));
            
            #line default
            #line hidden
            
            #line 337 "类名Dal.tt"
            this.Write(" = DbConn.ExecuteScalar<");
            
            #line default
            #line hidden
            
            #line 337 "类名Dal.tt"
            this.Write(IdentityField.DataType.Name);
            
            #line default
            #line hidden
            
            #line 337 "类名Dal.tt"
            this.Write(">(sql, param: item, transaction: tran);\r\n");
            
            #line default
            #line hidden
            
            #line 338 "类名Dal.tt"
}else{
            
            #line default
            #line hidden
            
            #line 338 "类名Dal.tt"
            this.Write("\r\n\t\t\tDbConn.ExecuteScalar(sql, param: item, transaction: tran);\r\n");
            
            #line default
            #line hidden
            
            #line 340 "类名Dal.tt"
}

foreach(IDataColumn Field in Table.Columns.Where(p => p.PrimaryKey))
{
	var fieldName = FormatUtil.ToCodeName(Field.Name);

            
            #line default
            #line hidden
            
            #line 345 "类名Dal.tt"
            this.Write("\t\t\titem.Original");
            
            #line default
            #line hidden
            
            #line 345 "类名Dal.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 345 "类名Dal.tt"
            this.Write(" = item.");
            
            #line default
            #line hidden
            
            #line 345 "类名Dal.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 345 "类名Dal.tt"
            this.Write(";\r\n");
            
            #line default
            #line hidden
            
            #line 346 "类名Dal.tt"

}
            
            #line default
            #line hidden
            
            #line 347 "类名Dal.tt"
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
            
            #line 357 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 357 "类名Dal.tt"
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
            
            #line 379 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 379 "类名Dal.tt"
            this.Write(" item, IDbTransaction tran = null)\r\n        {\r\n            const string format = " +
                    "@\"UPDATE {0} \r\n\t\t\t\t\tSET ");
            
            #line default
            #line hidden
            
            #line 382 "类名Dal.tt"
            this.Write(strSqlColKvs);
            
            #line default
            #line hidden
            
            #line 382 "类名Dal.tt"
            this.Write(" \r\n\t\t\t\t\tWHERE ");
            
            #line default
            #line hidden
            
            #line 383 "类名Dal.tt"
            this.Write(strSqlKeyKvs);
            
            #line default
            #line hidden
            
            #line 383 "类名Dal.tt"
            this.Write(";\";\r\n\r\n            var sql = string.Format(format, ");
            
            #line default
            #line hidden
            
            #line 385 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 385 "类名Dal.tt"
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
            
            #line 397 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 397 "类名Dal.tt"
            this.Write(" item, IList<string> nameList, IDbTransaction tran = null)\r\n        {\r\n          " +
                    "  if (nameList == null)\r\n            {\r\n                return Update(item, tran" +
                    ");\r\n            }\r\n\r\n            var curFieldList = ");
            
            #line default
            #line hidden
            
            #line 404 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 404 "类名Dal.tt"
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
            
            #line 416 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 416 "类名Dal.tt"
            this.Write(@"._.AllFields.Where(p => p.IsPrimaryKey && p.IsReadonly);
            var whereClause = originalKeys.Aggregate(string.Empty,
                (raw, p) => $""{raw} and {p.ColumnName}=@{p.Name}"",
                last => last.Trim().Substring(4));

            var sql = string.Format(format,
                ");
            
            #line default
            #line hidden
            
            #line 422 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 422 "类名Dal.tt"
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
            
            #line 434 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 434 "类名Dal.tt"
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
            
            #line 454 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 454 "类名Dal.tt"
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
            
            #line 475 "类名Dal.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 475 "类名Dal.tt"
            this.Write("._.DataBaseTableName,\r\n                strSet, whereClause);\r\n\r\n            retur" +
                    "n DbConn.Execute(sql, param: item, transaction: tran);\r\n        }\r\n\r\n        #en" +
                    "dregion\r\n\r\n\t\t#region Remove\r\n\r\n\t\t");
            
            #line default
            #line hidden
            
            #line 1 "内嵌_删除.tt"
            this.Write("/*\r\n * XCoder v");
            
            #line default
            #line hidden
            
            #line 2 "内嵌_删除.tt"
            this.Write(Version);
            
            #line default
            #line hidden
            
            #line 2 "内嵌_删除.tt"
            this.Write("\r\n * 作者：");
            
            #line default
            #line hidden
            
            #line 3 "内嵌_删除.tt"
            this.Write(Environment.UserName + "/" + Environment.MachineName);
            
            #line default
            #line hidden
            
            #line 3 "内嵌_删除.tt"
            this.Write("\r\n * 时间：");
            
            #line default
            #line hidden
            
            #line 4 "内嵌_删除.tt"
            this.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            
            #line default
            #line hidden
            
            #line 4 "内嵌_删除.tt"
            this.Write("\r\n * 版权：hardCTE 2016~");
            
            #line default
            #line hidden
            
            #line 5 "内嵌_删除.tt"
            this.Write(DateTime.Now.ToString("yyyy"));
            
            #line default
            #line hidden
            
            #line 5 "内嵌_删除.tt"
            this.Write("\r\n*/\r\n﻿\t#region 按键及索引 删除\r\n");
            
            #line default
            #line hidden
            
            #line 8 "内嵌_删除.tt"

//var modelName = FormatUtil.ToCodeName(Table.Name);

foreach(IDataIndex di in Table.Indexes){
	if(di.Columns == null || di.Columns.Length < 1) continue;

	var idxName = FormatUtil.ToCodeName(di.Name);
	var keyFields = Table.Columns.Where(p => di.Columns.Contains(p.Name));

	// 函数参数列表（int id,string codeName）
	var strParas = String.Join(", ", keyFields.Select(p => string.Format("{0} {1}", FormatUtil.ToFieldTypeString(p), FormatUtil.ToParamName(p.Name))));

	// sql语句中参数 keyvalue语句（columnname=@CodeName AND columnname2=@CodeName2)
	var strSqlKvs = String.Join(" AND ", keyFields.Select(p => string.Format("{0}=@Original{1}", p.ColumnName, FormatUtil.ToCodeName(p.Name))));

	// sql对象
	var strSqlObj = String.Join(", ", keyFields.Select(p => string.Format("Original{0} = {1}", FormatUtil.ToCodeName(p.Name), FormatUtil.ToParamName(p.Name))));

	// 删除依据
	var strCondition = di.Name.ToLower() == "primary" ? "主键" : "索引";
	var strConditionName = di.Name.ToLower() == "primary" ? "Pk" : "Idx" + idxName;
	
	
            
            #line default
            #line hidden
            
            #line 30 "内嵌_删除.tt"
            this.Write("\r\n\t\t/// <summary>\r\n        /// 根据");
            
            #line default
            #line hidden
            
            #line 32 "内嵌_删除.tt"
            this.Write(strCondition);
            
            #line default
            #line hidden
            
            #line 32 "内嵌_删除.tt"
            this.Write("删除\r\n        /// </summary>\r\n\t");
            
            #line default
            #line hidden
            
            #line 34 "内嵌_删除.tt"

	foreach(IDataColumn Field in keyFields){
            
            #line default
            #line hidden
            
            #line 35 "内嵌_删除.tt"
            this.Write("\t\r\n\t\t/// <param name=\"");
            
            #line default
            #line hidden
            
            #line 36 "内嵌_删除.tt"
            this.Write(FormatUtil.ToParamName(Field.Name));
            
            #line default
            #line hidden
            
            #line 36 "内嵌_删除.tt"
            this.Write("\">");
            
            #line default
            #line hidden
            
            #line 36 "内嵌_删除.tt"
            this.Write(FormatUtil.ToSigleDisplay(Field.Description));
            
            #line default
            #line hidden
            
            #line 36 "内嵌_删除.tt"
            this.Write("</param>");
            
            #line default
            #line hidden
            
            #line 36 "内嵌_删除.tt"
}
            
            #line default
            #line hidden
            
            #line 36 "内嵌_删除.tt"
            this.Write("\r\n\t\t/// <param name=\"tran\">事务</param>\r\n        /// <returns></returns>\r\n\t\tpublic " +
                    "virtual int RemoveBy");
            
            #line default
            #line hidden
            
            #line 39 "内嵌_删除.tt"
            this.Write(strConditionName);
            
            #line default
            #line hidden
            
            #line 39 "内嵌_删除.tt"
            this.Write("(");
            
            #line default
            #line hidden
            
            #line 39 "内嵌_删除.tt"
            this.Write(strParas);
            
            #line default
            #line hidden
            
            #line 39 "内嵌_删除.tt"
            this.Write(", IDbTransaction tran = null)\r\n        {\r\n            const string format = @\"DEL" +
                    "ETE FROM {0} WHERE ");
            
            #line default
            #line hidden
            
            #line 41 "内嵌_删除.tt"
            this.Write(strSqlKvs);
            
            #line default
            #line hidden
            
            #line 41 "内嵌_删除.tt"
            this.Write(";\";\r\n\r\n            var sql = string.Format(format, ");
            
            #line default
            #line hidden
            
            #line 43 "内嵌_删除.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 43 "内嵌_删除.tt"
            this.Write("._.DataBaseTableName);\r\n\r\n            return DbConn.Execute(sql, param: new {");
            
            #line default
            #line hidden
            
            #line 45 "内嵌_删除.tt"
            this.Write(strSqlObj);
            
            #line default
            #line hidden
            
            #line 45 "内嵌_删除.tt"
            this.Write("}, transaction: tran);\r\n        }\r\n\t\r\n\t");
            
            #line default
            #line hidden
            
            #line 48 "内嵌_删除.tt"

	// 只有一列时，支持批量删除功能
	if(keyFields.Count() == 1)
	{
		String keyParam = FormatUtil.ToParamName(keyFields.First().Name) + "s";
            
            #line default
            #line hidden
            
            #line 52 "内嵌_删除.tt"
            this.Write("\r\n\t\t/// <summary>\r\n        /// 根据");
            
            #line default
            #line hidden
            
            #line 54 "内嵌_删除.tt"
            this.Write(strCondition);
            
            #line default
            #line hidden
            
            #line 54 "内嵌_删除.tt"
            this.Write("批量删除\r\n        /// </summary>\r\n        /// <param name=\"");
            
            #line default
            #line hidden
            
            #line 56 "内嵌_删除.tt"
            this.Write(keyParam);
            
            #line default
            #line hidden
            
            #line 56 "内嵌_删除.tt"
            this.Write("\">");
            
            #line default
            #line hidden
            
            #line 56 "内嵌_删除.tt"
            this.Write(FormatUtil.ToSigleDisplay(keyFields.First().Description));
            
            #line default
            #line hidden
            
            #line 56 "内嵌_删除.tt"
            this.Write("列表</param>\r\n        /// <param name=\"tran\">事务</param>\r\n        /// <returns></ret" +
                    "urns>\r\n        public virtual int RemoveBy");
            
            #line default
            #line hidden
            
            #line 59 "内嵌_删除.tt"
            this.Write(strConditionName);
            
            #line default
            #line hidden
            
            #line 59 "内嵌_删除.tt"
            this.Write("s(IEnumerable<");
            
            #line default
            #line hidden
            
            #line 59 "内嵌_删除.tt"
            this.Write(FormatUtil.ToFieldTypeString(keyFields.First()));
            
            #line default
            #line hidden
            
            #line 59 "内嵌_删除.tt"
            this.Write("> ");
            
            #line default
            #line hidden
            
            #line 59 "内嵌_删除.tt"
            this.Write(keyParam);
            
            #line default
            #line hidden
            
            #line 59 "内嵌_删除.tt"
            this.Write(", IDbTransaction tran = null)\r\n        {\r\n            const string format = @\"DEL" +
                    "ETE FROM {0} WHERE ");
            
            #line default
            #line hidden
            
            #line 61 "内嵌_删除.tt"
            this.Write(strSqlKvs);
            
            #line default
            #line hidden
            
            #line 61 "内嵌_删除.tt"
            this.Write(";\";\r\n\r\n            var sql = string.Format(format, ");
            
            #line default
            #line hidden
            
            #line 63 "内嵌_删除.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 63 "内嵌_删除.tt"
            this.Write("._.DataBaseTableName);\r\n\r\n            return DbConn.Execute(sql, param: ");
            
            #line default
            #line hidden
            
            #line 65 "内嵌_删除.tt"
            this.Write(keyParam);
            
            #line default
            #line hidden
            
            #line 65 "内嵌_删除.tt"
            this.Write(".Select(p => new {Original");
            
            #line default
            #line hidden
            
            #line 65 "内嵌_删除.tt"
            this.Write(FormatUtil.ToCodeName(keyFields.First().Name));
            
            #line default
            #line hidden
            
            #line 65 "内嵌_删除.tt"
            this.Write(" = p}), transaction: tran);\r\n        }\r\n\t");
            
            #line default
            #line hidden
            
            #line 67 "内嵌_删除.tt"
}
}

            
            #line default
            #line hidden
            
            #line 69 "内嵌_删除.tt"
            this.Write(@"
		#endregion

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
            
            #line 95 "内嵌_删除.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 95 "内嵌_删除.tt"
            this.Write("._.DataBaseTableName,\r\n                whereClause);\r\n\r\n            return DbConn" +
                    ".Execute(sql, param: param, transaction: tran);\r\n        }");
            
            #line default
            #line hidden
            
            #line 485 "类名Dal.tt"
            this.Write("\r\n\r\n        #endregion\r\n    }\r\n}");
            
            #line default
            #line hidden
            return this.Output.ToString();
        }
    }
    
    #line default
    #line hidden
}
