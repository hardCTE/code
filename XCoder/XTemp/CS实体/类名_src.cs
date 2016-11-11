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
    
    
    #line 1 "类名.tt"
    public class 类名 : XCoder.XCoderBase
    {
        public override string Render()
        {
            
            #line 1 "类名.tt"
            this.Write("/*\r\n * XCoder v");
            
            #line default
            #line hidden
            
            #line 2 "类名.tt"
            this.Write(Version);
            
            #line default
            #line hidden
            
            #line 2 "类名.tt"
            this.Write("\r\n * 作者：");
            
            #line default
            #line hidden
            
            #line 3 "类名.tt"
            this.Write(Environment.UserName + "/" + Environment.MachineName);
            
            #line default
            #line hidden
            
            #line 3 "类名.tt"
            this.Write("\r\n * 时间：");
            
            #line default
            #line hidden
            
            #line 4 "类名.tt"
            this.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            
            #line default
            #line hidden
            
            #line 4 "类名.tt"
            this.Write("\r\n * 版权：hardCTE 2016~");
            
            #line default
            #line hidden
            
            #line 5 "类名.tt"
            this.Write(DateTime.Now.ToString("yyyy"));
            
            #line default
            #line hidden
            
            #line 5 "类名.tt"
            this.Write("\r\n*/\r\n﻿using System;\r\nusing System.Collections.Generic;\r\nusing System.Data;\r\nusin" +
                    "g App.FrameCore;\r\n\r\nnamespace ");
            
            #line default
            #line hidden
            
            #line 12 "类名.tt"
            this.Write(Config.NameSpace);
            
            #line default
            #line hidden
            
            #line 12 "类名.tt"
            this.Write("\r\n{");
            
            #line default
            #line hidden
            
            #line 13 "类名.tt"

    String tdis=FormatUtil.ToSigleDisplay(Table.DisplayName);
    String tdes=FormatUtil.ToSigleDisplay(Table.Description);
    if(String.IsNullOrEmpty(tdis)) tdis=tdes;

	var modelName = TemplateHelper.FormatUtil.ToCodeName(Table.Name);
    
            
            #line default
            #line hidden
            
            #line 19 "类名.tt"
            this.Write("\r\n    /// <summary>");
            
            #line default
            #line hidden
            
            #line 20 "类名.tt"
            this.Write(tdis);
            
            #line default
            #line hidden
            
            #line 20 "类名.tt"
            this.Write("</summary>");
            
            #line default
            #line hidden
            
            #line 20 "类名.tt"
 if(tdis!=tdes){
            
            #line default
            #line hidden
            
            #line 20 "类名.tt"
            this.Write("\r\n\t/// <remarks>");
            
            #line default
            #line hidden
            
            #line 21 "类名.tt"
            this.Write(tdes);
            
            #line default
            #line hidden
            
            #line 21 "类名.tt"
            this.Write("</remarks>");
            
            #line default
            #line hidden
            
            #line 21 "类名.tt"
}
            
            #line default
            #line hidden
            
            #line 21 "类名.tt"
            this.Write("\r\n    ///[Description(\"");
            
            #line default
            #line hidden
            
            #line 22 "类名.tt"
            this.Write(tdes);
            
            #line default
            #line hidden
            
            #line 22 "类名.tt"
            this.Write("\")]");
            
            #line default
            #line hidden
            
            #line 22 "类名.tt"

foreach(IDataIndex di in Table.Indexes){if(di.Columns==null||di.Columns.Length<1)continue;
            
            #line default
            #line hidden
            
            #line 23 "类名.tt"
            this.Write("\r\n    ///[BindIndex(\"");
            
            #line default
            #line hidden
            
            #line 24 "类名.tt"
            this.Write(di.Name);
            
            #line default
            #line hidden
            
            #line 24 "类名.tt"
            this.Write("\", ");
            
            #line default
            #line hidden
            
            #line 24 "类名.tt"
            this.Write(di.Unique.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 24 "类名.tt"
            this.Write(", \"");
            
            #line default
            #line hidden
            
            #line 24 "类名.tt"
            this.Write(String.Join(",", di.Columns));
            
            #line default
            #line hidden
            
            #line 24 "类名.tt"
            this.Write("\")]");
            
            #line default
            #line hidden
            
            #line 24 "类名.tt"

}
foreach(IDataRelation dr in Table.Relations){
            
            #line default
            #line hidden
            
            #line 26 "类名.tt"
            this.Write("\r\n    ///[BindRelation(\"");
            
            #line default
            #line hidden
            
            #line 27 "类名.tt"
            this.Write(dr.Column);
            
            #line default
            #line hidden
            
            #line 27 "类名.tt"
            this.Write("\", ");
            
            #line default
            #line hidden
            
            #line 27 "类名.tt"
            this.Write(dr.Unique.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 27 "类名.tt"
            this.Write(", \"");
            
            #line default
            #line hidden
            
            #line 27 "类名.tt"
            this.Write(dr.RelationTable);
            
            #line default
            #line hidden
            
            #line 27 "类名.tt"
            this.Write("\", \"");
            
            #line default
            #line hidden
            
            #line 27 "类名.tt"
            this.Write(dr.RelationColumn);
            
            #line default
            #line hidden
            
            #line 27 "类名.tt"
            this.Write("\")]");
            
            #line default
            #line hidden
            
            #line 27 "类名.tt"
}
            
            #line default
            #line hidden
            
            #line 27 "类名.tt"
            this.Write("\r\n    ///[BindTable(\"");
            
            #line default
            #line hidden
            
            #line 28 "类名.tt"
            this.Write(Table.TableName);
            
            #line default
            #line hidden
            
            #line 28 "类名.tt"
            this.Write("\", Description = \"");
            
            #line default
            #line hidden
            
            #line 28 "类名.tt"
            this.Write(tdes);
            
            #line default
            #line hidden
            
            #line 28 "类名.tt"
            this.Write("\", ConnName = \"");
            
            #line default
            #line hidden
            
            #line 28 "类名.tt"
            this.Write(Table.ConnName ?? Config.EntityConnName);
            
            #line default
            #line hidden
            
            #line 28 "类名.tt"
            this.Write("\", DbType = DatabaseType.");
            
            #line default
            #line hidden
            
            #line 28 "类名.tt"
            this.Write(Table.DbType);
            
            #line default
            #line hidden
            
            #line 28 "类名.tt"
if(Table.IsView){
            
            #line default
            #line hidden
            
            #line 28 "类名.tt"
            this.Write(", IsView = true");
            
            #line default
            #line hidden
            
            #line 28 "类名.tt"
}
            
            #line default
            #line hidden
            
            #line 28 "类名.tt"
            this.Write(")]\r\n    public partial class ");
            
            #line default
            #line hidden
            
            #line 29 "类名.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 29 "类名.tt"
            this.Write(" : TableModelBase\r\n    {");
            
            #line default
            #line hidden
            
            #line 30 "类名.tt"

if(Table.Columns.Count>0)
{

            
            #line default
            #line hidden
            
            #line 33 "类名.tt"
            this.Write("\r\n        #region 属性");
            
            #line default
            #line hidden
            
            #line 34 "类名.tt"

    foreach(IDataColumn Field in Table.Columns)
    {
        String des = FormatUtil.ToSigleDisplay(Field.Description);
		var fieldType = TemplateHelper.FormatUtil.ToFieldTypeString(Field);
		var fieldName = TemplateHelper.FormatUtil.ToCodeName(Field.Name);

		if(Field.PrimaryKey)
		{
            
            #line default
            #line hidden
            
            #line 42 "类名.tt"
            this.Write("\r\n\t\t/// <summary>Original");
            
            #line default
            #line hidden
            
            #line 43 "类名.tt"
            this.Write(des);
            
            #line default
            #line hidden
            
            #line 43 "类名.tt"
            this.Write("</summary>\r\n\t\tpublic virtual ");
            
            #line default
            #line hidden
            
            #line 44 "类名.tt"
            this.Write(fieldType);
            
            #line default
            #line hidden
            
            #line 44 "类名.tt"
            this.Write(" Original");
            
            #line default
            #line hidden
            
            #line 44 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 44 "类名.tt"
            this.Write(" { get; set; }\r\n\t\t");
            
            #line default
            #line hidden
            
            #line 45 "类名.tt"
}

            
            #line default
            #line hidden
            
            #line 46 "类名.tt"
            this.Write("\r\n        /// <summary>");
            
            #line default
            #line hidden
            
            #line 47 "类名.tt"
            this.Write(des);
            
            #line default
            #line hidden
            
            #line 47 "类名.tt"
            this.Write("</summary>\r\n\t\tpublic virtual ");
            
            #line default
            #line hidden
            
            #line 48 "类名.tt"
            this.Write(fieldType);
            
            #line default
            #line hidden
            
            #line 48 "类名.tt"
            this.Write(" ");
            
            #line default
            #line hidden
            
            #line 48 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 48 "类名.tt"
            this.Write(" { get; set; }\r\n");
            
            #line default
            #line hidden
            
            #line 49 "类名.tt"

    }

            
            #line default
            #line hidden
            
            #line 51 "类名.tt"
            this.Write(@"        #endregion

        #region 实现抽象类方法

		/// <summary>
        /// 数据库表名
        /// </summary>
        public override string DataBaseTableName => _.DataBaseTableName;

        /// <summary>
        /// 获取模型所有字段
        /// </summary>
        /// <returns></returns>
        public override IList<Field> GetAllFields()
        {
            return _.AllFields;
        }

        /// <summary>
        /// 获取/设置 字段值。
        /// 一个索引
        /// 派生实体类可重写该索引
        /// </summary>
        /// <param name=""name"">字段名</param>
        /// <returns></returns>
        public override Object this[String name]
        {
            get
            {
                switch (name)
                {");
            
            #line default
            #line hidden
            
            #line 81 "类名.tt"

    foreach(IDataColumn Field in Table.Columns)
    {
		var fieldName = TemplateHelper.FormatUtil.ToCodeName(Field.Name);

		if(Field.PrimaryKey){
            
            #line default
            #line hidden
            
            #line 86 "类名.tt"
            this.Write("\r\n\t\t\t\t\tcase __.Original");
            
            #line default
            #line hidden
            
            #line 87 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 87 "类名.tt"
            this.Write(" : return Original");
            
            #line default
            #line hidden
            
            #line 87 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 87 "类名.tt"
            this.Write(";");
            
            #line default
            #line hidden
            
            #line 87 "类名.tt"

		}

            
            #line default
            #line hidden
            
            #line 89 "类名.tt"
            this.Write("\r\n                    case __.");
            
            #line default
            #line hidden
            
            #line 90 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 90 "类名.tt"
            this.Write(" : return ");
            
            #line default
            #line hidden
            
            #line 90 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 90 "类名.tt"
            this.Write(";");
            
            #line default
            #line hidden
            
            #line 90 "类名.tt"

    }

            
            #line default
            #line hidden
            
            #line 92 "类名.tt"
            this.Write("\r\n                    default: return null;\r\n                }\r\n            }\r\n  " +
                    "          set\r\n            {\r\n                switch (name)\r\n                {");
            
            #line default
            #line hidden
            
            #line 99 "类名.tt"

    Type conv=typeof(Convert);
    foreach(IDataColumn Field in Table.Columns)
    {
		var fieldName = TemplateHelper.FormatUtil.ToCodeName(Field.Name);

		if(Field.PrimaryKey){
			if(conv.GetMethod("To"+Field.DataType.Name, new Type[]{typeof(Object)})!=null){
		
            
            #line default
            #line hidden
            
            #line 107 "类名.tt"
            this.Write("\r\n\t\t\t\t\tcase __.Original");
            
            #line default
            #line hidden
            
            #line 108 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 108 "类名.tt"
            this.Write(" : Original");
            
            #line default
            #line hidden
            
            #line 108 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 108 "类名.tt"
            this.Write(" = Convert.To");
            
            #line default
            #line hidden
            
            #line 108 "类名.tt"
            this.Write(Field.DataType.Name);
            
            #line default
            #line hidden
            
            #line 108 "类名.tt"
            this.Write("(value); break;");
            
            #line default
            #line hidden
            
            #line 108 "类名.tt"

			}else{
		
            
            #line default
            #line hidden
            
            #line 110 "类名.tt"
            this.Write("\r\n\t\t\t\t\tcase __.Original");
            
            #line default
            #line hidden
            
            #line 111 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 111 "类名.tt"
            this.Write(" : Original");
            
            #line default
            #line hidden
            
            #line 111 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 111 "类名.tt"
            this.Write(" = (");
            
            #line default
            #line hidden
            
            #line 111 "类名.tt"
            this.Write(Field.DataType.Name);
            
            #line default
            #line hidden
            
            #line 111 "类名.tt"
            this.Write(")value; break;");
            
            #line default
            #line hidden
            
            #line 111 "类名.tt"

			}
		}

        if(conv.GetMethod("To"+Field.DataType.Name, new Type[]{typeof(Object)})!=null){

            
            #line default
            #line hidden
            
            #line 116 "类名.tt"
            this.Write("\r\n                    case __.");
            
            #line default
            #line hidden
            
            #line 117 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 117 "类名.tt"
            this.Write(" : ");
            
            #line default
            #line hidden
            
            #line 117 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 117 "类名.tt"
            this.Write(" = Convert.To");
            
            #line default
            #line hidden
            
            #line 117 "类名.tt"
            this.Write(Field.DataType.Name);
            
            #line default
            #line hidden
            
            #line 117 "类名.tt"
            this.Write("(value); break;");
            
            #line default
            #line hidden
            
            #line 117 "类名.tt"

        }else{

            
            #line default
            #line hidden
            
            #line 119 "类名.tt"
            this.Write("\r\n                    case __.");
            
            #line default
            #line hidden
            
            #line 120 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 120 "类名.tt"
            this.Write(" : ");
            
            #line default
            #line hidden
            
            #line 120 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 120 "类名.tt"
            this.Write(" = (");
            
            #line default
            #line hidden
            
            #line 120 "类名.tt"
            this.Write(Field.DataType.Name);
            
            #line default
            #line hidden
            
            #line 120 "类名.tt"
            this.Write(")value; break;");
            
            #line default
            #line hidden
            
            #line 120 "类名.tt"

        }
    }

            
            #line default
            #line hidden
            
            #line 123 "类名.tt"
            this.Write("\r\n                    default: break;\r\n\t\t\t\t}\r\n            }\r\n        }\r\n        #" +
                    "endregion\r\n");
            
            #line default
            #line hidden
            
            #line 129 "类名.tt"

}

            
            #line default
            #line hidden
            
            #line 131 "类名.tt"
            this.Write("\r\n        #region 字段信息\r\n\r\n\t\t/// <summary>取得");
            
            #line default
            #line hidden
            
            #line 134 "类名.tt"
            this.Write(tdis);
            
            #line default
            #line hidden
            
            #line 134 "类名.tt"
            this.Write("字段名称的快捷方式</summary>\r\n        public partial class __\r\n        {");
            
            #line default
            #line hidden
            
            #line 136 "类名.tt"

foreach(IDataColumn Field in Table.GetAllColumns(Tables, true))
{
	var fieldName = TemplateHelper.FormatUtil.ToCodeName(Field.Name);

    String des = FormatUtil.ToSigleDisplay(Field.Description);

	if(Field.PrimaryKey){

            
            #line default
            #line hidden
            
            #line 144 "类名.tt"
            this.Write("\r\n\t\t\t///<summary>原始主键，");
            
            #line default
            #line hidden
            
            #line 145 "类名.tt"
            this.Write(des);
            
            #line default
            #line hidden
            
            #line 145 "类名.tt"
            this.Write("</summary>\r\n            public const String Original");
            
            #line default
            #line hidden
            
            #line 146 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 146 "类名.tt"
            this.Write(" = \"Original");
            
            #line default
            #line hidden
            
            #line 146 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 146 "类名.tt"
            this.Write("\";\r\n");
            
            #line default
            #line hidden
            
            #line 147 "类名.tt"
}
            
            #line default
            #line hidden
            
            #line 147 "类名.tt"
            this.Write("\r\n            ///<summary>");
            
            #line default
            #line hidden
            
            #line 148 "类名.tt"
            this.Write(des);
            
            #line default
            #line hidden
            
            #line 148 "类名.tt"
            this.Write("</summary>\r\n            public const String ");
            
            #line default
            #line hidden
            
            #line 149 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 149 "类名.tt"
            this.Write(" = \"");
            
            #line default
            #line hidden
            
            #line 149 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 149 "类名.tt"
            this.Write("\";\r\n");
            
            #line default
            #line hidden
            
            #line 150 "类名.tt"

}

            
            #line default
            #line hidden
            
            #line 152 "类名.tt"
            this.Write("\r\n        }\r\n\r\n        /// <summary>取得");
            
            #line default
            #line hidden
            
            #line 155 "类名.tt"
            this.Write(tdis);
            
            #line default
            #line hidden
            
            #line 155 "类名.tt"
            this.Write("字段信息的快捷方式</summary>\r\n        public partial class _\r\n        {\r\n\t\t\t/// <summary>\r" +
                    "\n            /// 数据库表名\r\n            /// </summary>\r\n            public const str" +
                    "ing DataBaseTableName = \"");
            
            #line default
            #line hidden
            
            #line 161 "类名.tt"
            this.Write(Table.TableName);
            
            #line default
            #line hidden
            
            #line 161 "类名.tt"
            this.Write("\";\r\n");
            
            #line default
            #line hidden
            
            #line 162 "类名.tt"

foreach(IDataColumn Field in Table.GetAllColumns(Tables, true))
{
	var fieldName = TemplateHelper.FormatUtil.ToCodeName(Field.Name);
    String des=Field.Description;
    if(!String.IsNullOrEmpty(des)) des=des.Replace("\r\n"," ");

	if(Field.PrimaryKey){

            
            #line default
            #line hidden
            
            #line 170 "类名.tt"
            this.Write("\r\n            ///<summary>原始主键,");
            
            #line default
            #line hidden
            
            #line 171 "类名.tt"
            this.Write(des);
            
            #line default
            #line hidden
            
            #line 171 "类名.tt"
            this.Write("</summary>\r\n            public static readonly Field Original");
            
            #line default
            #line hidden
            
            #line 172 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 172 "类名.tt"
            this.Write(" = new Field\r\n            {\r\n                Name = __.Original");
            
            #line default
            #line hidden
            
            #line 174 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 174 "类名.tt"
            this.Write(",\r\n\t\t\t\tColumnName = \"");
            
            #line default
            #line hidden
            
            #line 175 "类名.tt"
            this.Write(Field.Name);
            
            #line default
            #line hidden
            
            #line 175 "类名.tt"
            this.Write("\",\r\n                DisplayName = \"");
            
            #line default
            #line hidden
            
            #line 176 "类名.tt"
            this.Write(Field.DisplayName);
            
            #line default
            #line hidden
            
            #line 176 "类名.tt"
            this.Write("\",\r\n                Description = \"");
            
            #line default
            #line hidden
            
            #line 177 "类名.tt"
            this.Write((""+Field.Description).Replace("\\", "\\\\"));
            
            #line default
            #line hidden
            
            #line 177 "类名.tt"
            this.Write("\",\r\n                DataType = DbType.");
            
            #line default
            #line hidden
            
            #line 178 "类名.tt"
            this.Write(Field.DataType.Name);
            
            #line default
            #line hidden
            
            #line 178 "类名.tt"
            this.Write(",\r\n                DefaultValue = ");
            
            #line default
            #line hidden
            
            #line 179 "类名.tt"
            this.Write(Field.Default==null?"null":"\""+Field.Default.Replace("\\", "\\\\")+"\"");
            
            #line default
            #line hidden
            
            #line 179 "类名.tt"
            this.Write(",\r\n                IsPrimaryKey = ");
            
            #line default
            #line hidden
            
            #line 180 "类名.tt"
            this.Write(Field.PrimaryKey.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 180 "类名.tt"
            this.Write(",\r\n\t\t\t\tIdentity = ");
            
            #line default
            #line hidden
            
            #line 181 "类名.tt"
            this.Write(Field.Identity.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 181 "类名.tt"
            this.Write(",\r\n                IsReadonly = true,\r\n                IsNullable = ");
            
            #line default
            #line hidden
            
            #line 183 "类名.tt"
            this.Write(Field.Nullable.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 183 "类名.tt"
            this.Write(",\r\n                Length = ");
            
            #line default
            #line hidden
            
            #line 184 "类名.tt"
            this.Write(Field.Length);
            
            #line default
            #line hidden
            
            #line 184 "类名.tt"
            this.Write(",\r\n                Precision = ");
            
            #line default
            #line hidden
            
            #line 185 "类名.tt"
            this.Write(Field.Precision);
            
            #line default
            #line hidden
            
            #line 185 "类名.tt"
            this.Write(",\r\n                Scale = ");
            
            #line default
            #line hidden
            
            #line 186 "类名.tt"
            this.Write(Field.Scale);
            
            #line default
            #line hidden
            
            #line 186 "类名.tt"
            this.Write("\r\n\t\t\t};\r\n");
            
            #line default
            #line hidden
            
            #line 188 "类名.tt"
}
            
            #line default
            #line hidden
            
            #line 188 "类名.tt"
            this.Write("\r\n            ///<summary>");
            
            #line default
            #line hidden
            
            #line 189 "类名.tt"
            this.Write(des);
            
            #line default
            #line hidden
            
            #line 189 "类名.tt"
            this.Write("</summary>\r\n            public static readonly Field ");
            
            #line default
            #line hidden
            
            #line 190 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 190 "类名.tt"
            this.Write(" = new Field\r\n            {\r\n                Name = __.");
            
            #line default
            #line hidden
            
            #line 192 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 192 "类名.tt"
            this.Write(",\r\n\t\t\t\tColumnName = \"");
            
            #line default
            #line hidden
            
            #line 193 "类名.tt"
            this.Write(Field.Name);
            
            #line default
            #line hidden
            
            #line 193 "类名.tt"
            this.Write("\",\r\n                DisplayName = \"");
            
            #line default
            #line hidden
            
            #line 194 "类名.tt"
            this.Write(Field.DisplayName);
            
            #line default
            #line hidden
            
            #line 194 "类名.tt"
            this.Write("\",\r\n                Description = \"");
            
            #line default
            #line hidden
            
            #line 195 "类名.tt"
            this.Write((""+Field.Description).Replace("\\", "\\\\"));
            
            #line default
            #line hidden
            
            #line 195 "类名.tt"
            this.Write("\",\r\n                DataType = DbType.");
            
            #line default
            #line hidden
            
            #line 196 "类名.tt"
            this.Write(Field.DataType.Name);
            
            #line default
            #line hidden
            
            #line 196 "类名.tt"
            this.Write(",\r\n                DefaultValue = ");
            
            #line default
            #line hidden
            
            #line 197 "类名.tt"
            this.Write(Field.Default==null?"null":"\""+Field.Default.Replace("\\", "\\\\")+"\"");
            
            #line default
            #line hidden
            
            #line 197 "类名.tt"
            this.Write(",\r\n                IsPrimaryKey = ");
            
            #line default
            #line hidden
            
            #line 198 "类名.tt"
            this.Write(Field.PrimaryKey.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 198 "类名.tt"
            this.Write(",\r\n\t\t\t\tIdentity = ");
            
            #line default
            #line hidden
            
            #line 199 "类名.tt"
            this.Write(Field.Identity.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 199 "类名.tt"
            this.Write(",\r\n                IsReadonly = false,\r\n                IsNullable = ");
            
            #line default
            #line hidden
            
            #line 201 "类名.tt"
            this.Write(Field.Nullable.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 201 "类名.tt"
            this.Write(",\r\n                Length = ");
            
            #line default
            #line hidden
            
            #line 202 "类名.tt"
            this.Write(Field.Length);
            
            #line default
            #line hidden
            
            #line 202 "类名.tt"
            this.Write(",\r\n                Precision = ");
            
            #line default
            #line hidden
            
            #line 203 "类名.tt"
            this.Write(Field.Precision);
            
            #line default
            #line hidden
            
            #line 203 "类名.tt"
            this.Write(",\r\n                Scale = ");
            
            #line default
            #line hidden
            
            #line 204 "类名.tt"
            this.Write(Field.Scale);
            
            #line default
            #line hidden
            
            #line 204 "类名.tt"
            this.Write("\r\n\t\t\t};\r\n");
            
            #line default
            #line hidden
            
            #line 206 "类名.tt"

}

            
            #line default
            #line hidden
            
            #line 208 "类名.tt"
            this.Write("\r\n\t\t\t///<summary>所有字段列表</summary>\r\n\t\t\tpublic static readonly IList<Field> AllFiel" +
                    "ds = new List<Field>\r\n            {");
            
            #line default
            #line hidden
            
            #line 211 "类名.tt"

foreach(IDataColumn Field in Table.GetAllColumns(Tables, true))
{
	var fieldName = TemplateHelper.FormatUtil.ToCodeName(Field.Name);
	if(Field.PrimaryKey){
            
            #line default
            #line hidden
            
            #line 215 "类名.tt"
            this.Write("\r\n\t\t\t\tOriginal");
            
            #line default
            #line hidden
            
            #line 216 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 216 "类名.tt"
            this.Write(",");
            
            #line default
            #line hidden
            
            #line 216 "类名.tt"
}
            
            #line default
            #line hidden
            
            #line 216 "类名.tt"
            this.Write("\t\t\t\r\n\t\t\t\t");
            
            #line default
            #line hidden
            
            #line 217 "类名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 217 "类名.tt"
            this.Write(",");
            
            #line default
            #line hidden
            
            #line 217 "类名.tt"

}

            
            #line default
            #line hidden
            
            #line 219 "类名.tt"
            this.Write("\r\n            };\r\n\r\n        }\r\n\r\n        #endregion\r\n    }\r\n}");
            
            #line default
            #line hidden
            return this.Output.ToString();
        }
    }
    
    #line default
    #line hidden
}
