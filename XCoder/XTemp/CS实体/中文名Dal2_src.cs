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
    
    
    #line 1 "中文名Dal2.tt"
    public class 中文名Dal2 : XCoder.XCoderBase
    {
        public override string Render()
        {
            
            #line 1 "中文名Dal2.tt"
            this.Write("﻿\t\t#region 删除\r\n\r\n        #region 按键及索引 删除\r\n");
            
            #line default
            #line hidden
            
            #line 4 "中文名Dal2.tt"

var modelName = FormatUtil.ToCodeName(Table.Name);

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
            
            #line 26 "中文名Dal2.tt"
            this.Write("\r\n\t\t/// <summary>\r\n        /// 根据");
            
            #line default
            #line hidden
            
            #line 28 "中文名Dal2.tt"
            this.Write(strCondition);
            
            #line default
            #line hidden
            
            #line 28 "中文名Dal2.tt"
            this.Write("删除\r\n        /// </summary>\r\n\t");
            
            #line default
            #line hidden
            
            #line 30 "中文名Dal2.tt"

	foreach(IDataColumn Field in keyFields){
            
            #line default
            #line hidden
            
            #line 31 "中文名Dal2.tt"
            this.Write("\t\r\n\t\t/// <param name=\"");
            
            #line default
            #line hidden
            
            #line 32 "中文名Dal2.tt"
            this.Write(FormatUtil.ToParamName(Field.Name));
            
            #line default
            #line hidden
            
            #line 32 "中文名Dal2.tt"
            this.Write("\">");
            
            #line default
            #line hidden
            
            #line 32 "中文名Dal2.tt"
            this.Write(FormatUtil.ToSigleDisplay(Field.Description));
            
            #line default
            #line hidden
            
            #line 32 "中文名Dal2.tt"
            this.Write("</param>");
            
            #line default
            #line hidden
            
            #line 32 "中文名Dal2.tt"
}
            
            #line default
            #line hidden
            
            #line 32 "中文名Dal2.tt"
            this.Write("\r\n\t\t/// <param name=\"tran\">事务</param>\r\n        /// <returns></returns>\r\n\t\tpublic " +
                    "virtual int RemoveBy");
            
            #line default
            #line hidden
            
            #line 35 "中文名Dal2.tt"
            this.Write(strConditionName);
            
            #line default
            #line hidden
            
            #line 35 "中文名Dal2.tt"
            this.Write("(");
            
            #line default
            #line hidden
            
            #line 35 "中文名Dal2.tt"
            this.Write(strParas);
            
            #line default
            #line hidden
            
            #line 35 "中文名Dal2.tt"
            this.Write(", IDbTransaction tran = null)\r\n        {\r\n            const string format = @\"DEL" +
                    "ETE FROM {0} WHERE ");
            
            #line default
            #line hidden
            
            #line 37 "中文名Dal2.tt"
            this.Write(strSqlKvs);
            
            #line default
            #line hidden
            
            #line 37 "中文名Dal2.tt"
            this.Write(";\";\r\n\r\n            var sql = string.Format(format, ");
            
            #line default
            #line hidden
            
            #line 39 "中文名Dal2.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 39 "中文名Dal2.tt"
            this.Write("._.DataBaseTableName);\r\n\r\n            return DbConn.Execute(sql, param: new {");
            
            #line default
            #line hidden
            
            #line 41 "中文名Dal2.tt"
            this.Write(strSqlObj);
            
            #line default
            #line hidden
            
            #line 41 "中文名Dal2.tt"
            this.Write("}, transaction: tran);\r\n        }\r\n\t\r\n\t");
            
            #line default
            #line hidden
            
            #line 44 "中文名Dal2.tt"

	// 只有一列时，支持批量删除功能
	if(keyFields.Count() == 1)
	{
		String keyParam = FormatUtil.ToParamName(keyFields.First().Name) + "s";
            
            #line default
            #line hidden
            
            #line 48 "中文名Dal2.tt"
            this.Write("\r\n\t\t/// <summary>\r\n        /// 根据");
            
            #line default
            #line hidden
            
            #line 50 "中文名Dal2.tt"
            this.Write(strCondition);
            
            #line default
            #line hidden
            
            #line 50 "中文名Dal2.tt"
            this.Write("批量删除\r\n        /// </summary>\r\n        /// <param name=\"");
            
            #line default
            #line hidden
            
            #line 52 "中文名Dal2.tt"
            this.Write(keyParam);
            
            #line default
            #line hidden
            
            #line 52 "中文名Dal2.tt"
            this.Write("\">");
            
            #line default
            #line hidden
            
            #line 52 "中文名Dal2.tt"
            this.Write(FormatUtil.ToSigleDisplay(keyFields.First().Description));
            
            #line default
            #line hidden
            
            #line 52 "中文名Dal2.tt"
            this.Write("列表</param>\r\n        /// <param name=\"tran\">事务</param>\r\n        /// <returns></ret" +
                    "urns>\r\n        public virtual int RemoveBy");
            
            #line default
            #line hidden
            
            #line 55 "中文名Dal2.tt"
            this.Write(strConditionName);
            
            #line default
            #line hidden
            
            #line 55 "中文名Dal2.tt"
            this.Write("s(IEnumerable<");
            
            #line default
            #line hidden
            
            #line 55 "中文名Dal2.tt"
            this.Write(FormatUtil.ToFieldTypeString(keyFields.First()));
            
            #line default
            #line hidden
            
            #line 55 "中文名Dal2.tt"
            this.Write("> ");
            
            #line default
            #line hidden
            
            #line 55 "中文名Dal2.tt"
            this.Write(keyParam);
            
            #line default
            #line hidden
            
            #line 55 "中文名Dal2.tt"
            this.Write(", IDbTransaction tran = null)\r\n        {\r\n            const string format = @\"DEL" +
                    "ETE FROM {0} WHERE ");
            
            #line default
            #line hidden
            
            #line 57 "中文名Dal2.tt"
            this.Write(strSqlKvs);
            
            #line default
            #line hidden
            
            #line 57 "中文名Dal2.tt"
            this.Write(";\";\r\n\r\n            var sql = string.Format(format, ");
            
            #line default
            #line hidden
            
            #line 59 "中文名Dal2.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 59 "中文名Dal2.tt"
            this.Write("._.DataBaseTableName);\r\n\r\n            return DbConn.Execute(sql, param: ");
            
            #line default
            #line hidden
            
            #line 61 "中文名Dal2.tt"
            this.Write(keyParam);
            
            #line default
            #line hidden
            
            #line 61 "中文名Dal2.tt"
            this.Write(".Select(p => new {Original");
            
            #line default
            #line hidden
            
            #line 61 "中文名Dal2.tt"
            this.Write(FormatUtil.ToCodeName(keyFields.First().Name));
            
            #line default
            #line hidden
            
            #line 61 "中文名Dal2.tt"
            this.Write(" = p}), transaction: tran);\r\n        }\r\n\t");
            
            #line default
            #line hidden
            
            #line 63 "中文名Dal2.tt"
}
}

            
            #line default
            #line hidden
            
            #line 65 "中文名Dal2.tt"
            this.Write("\r\n        #endregion");
            
            #line default
            #line hidden
            return this.Output.ToString();
        }
    }
    
    #line default
    #line hidden
}
