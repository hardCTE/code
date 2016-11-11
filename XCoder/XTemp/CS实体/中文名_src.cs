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
    
    
    #line 1 "中文名.tt"
    public class 中文名 : XCoder.XCoderBase
    {
        public override string Render()
        {
            
            #line 1 "中文名.tt"
            this.Write("﻿using System;\r\nusing System.Collections.Generic;\r\nusing System.Data;\r\nusing App." +
                    "FrameCore;\r\n\r\nnamespace ");
            
            #line default
            #line hidden
            
            #line 6 "中文名.tt"
            this.Write(Config.NameSpace);
            
            #line default
            #line hidden
            
            #line 6 "中文名.tt"
            this.Write("\r\n{");
            
            #line default
            #line hidden
            
            #line 7 "中文名.tt"

    String tdis=FormatUtil.ToSigleDisplay(Table.DisplayName);
    String tdes=FormatUtil.ToSigleDisplay(Table.Description);
    if(String.IsNullOrEmpty(tdis)) tdis=tdes;

	var modelName = TemplateHelper.FormatUtil.ToCodeName(Table.Name);
    
            
            #line default
            #line hidden
            
            #line 13 "中文名.tt"
            this.Write("\r\n    /// <summary>");
            
            #line default
            #line hidden
            
            #line 14 "中文名.tt"
            this.Write(tdis);
            
            #line default
            #line hidden
            
            #line 14 "中文名.tt"
            this.Write("</summary>");
            
            #line default
            #line hidden
            
            #line 14 "中文名.tt"
 if(tdis!=tdes){
            
            #line default
            #line hidden
            
            #line 14 "中文名.tt"
            this.Write("\r\n\t/// <remarks>");
            
            #line default
            #line hidden
            
            #line 15 "中文名.tt"
            this.Write(tdes);
            
            #line default
            #line hidden
            
            #line 15 "中文名.tt"
            this.Write("</remarks>");
            
            #line default
            #line hidden
            
            #line 15 "中文名.tt"
}
            
            #line default
            #line hidden
            
            #line 15 "中文名.tt"
            this.Write("\r\n    ///[Description(\"");
            
            #line default
            #line hidden
            
            #line 16 "中文名.tt"
            this.Write(tdes);
            
            #line default
            #line hidden
            
            #line 16 "中文名.tt"
            this.Write("\")]");
            
            #line default
            #line hidden
            
            #line 16 "中文名.tt"

foreach(IDataIndex di in Table.Indexes){if(di.Columns==null||di.Columns.Length<1)continue;
            
            #line default
            #line hidden
            
            #line 17 "中文名.tt"
            this.Write("\r\n    ///[BindIndex(\"");
            
            #line default
            #line hidden
            
            #line 18 "中文名.tt"
            this.Write(di.Name);
            
            #line default
            #line hidden
            
            #line 18 "中文名.tt"
            this.Write("\", ");
            
            #line default
            #line hidden
            
            #line 18 "中文名.tt"
            this.Write(di.Unique.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 18 "中文名.tt"
            this.Write(", \"");
            
            #line default
            #line hidden
            
            #line 18 "中文名.tt"
            this.Write(String.Join(",", di.Columns));
            
            #line default
            #line hidden
            
            #line 18 "中文名.tt"
            this.Write("\")]");
            
            #line default
            #line hidden
            
            #line 18 "中文名.tt"

}
foreach(IDataRelation dr in Table.Relations){
            
            #line default
            #line hidden
            
            #line 20 "中文名.tt"
            this.Write("\r\n    ///[BindRelation(\"");
            
            #line default
            #line hidden
            
            #line 21 "中文名.tt"
            this.Write(dr.Column);
            
            #line default
            #line hidden
            
            #line 21 "中文名.tt"
            this.Write("\", ");
            
            #line default
            #line hidden
            
            #line 21 "中文名.tt"
            this.Write(dr.Unique.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 21 "中文名.tt"
            this.Write(", \"");
            
            #line default
            #line hidden
            
            #line 21 "中文名.tt"
            this.Write(dr.RelationTable);
            
            #line default
            #line hidden
            
            #line 21 "中文名.tt"
            this.Write("\", \"");
            
            #line default
            #line hidden
            
            #line 21 "中文名.tt"
            this.Write(dr.RelationColumn);
            
            #line default
            #line hidden
            
            #line 21 "中文名.tt"
            this.Write("\")]");
            
            #line default
            #line hidden
            
            #line 21 "中文名.tt"
}
            
            #line default
            #line hidden
            
            #line 21 "中文名.tt"
            this.Write("\r\n    ///[BindTable(\"");
            
            #line default
            #line hidden
            
            #line 22 "中文名.tt"
            this.Write(Table.TableName);
            
            #line default
            #line hidden
            
            #line 22 "中文名.tt"
            this.Write("\", Description = \"");
            
            #line default
            #line hidden
            
            #line 22 "中文名.tt"
            this.Write(tdes);
            
            #line default
            #line hidden
            
            #line 22 "中文名.tt"
            this.Write("\", ConnName = \"");
            
            #line default
            #line hidden
            
            #line 22 "中文名.tt"
            this.Write(Table.ConnName ?? Config.EntityConnName);
            
            #line default
            #line hidden
            
            #line 22 "中文名.tt"
            this.Write("\", DbType = DatabaseType.");
            
            #line default
            #line hidden
            
            #line 22 "中文名.tt"
            this.Write(Table.DbType);
            
            #line default
            #line hidden
            
            #line 22 "中文名.tt"
if(Table.IsView){
            
            #line default
            #line hidden
            
            #line 22 "中文名.tt"
            this.Write(", IsView = true");
            
            #line default
            #line hidden
            
            #line 22 "中文名.tt"
}
            
            #line default
            #line hidden
            
            #line 22 "中文名.tt"
            this.Write(")]\r\n    public partial class ");
            
            #line default
            #line hidden
            
            #line 23 "中文名.tt"
            this.Write(modelName);
            
            #line default
            #line hidden
            
            #line 23 "中文名.tt"
            this.Write(" : TableModelBase\r\n    {");
            
            #line default
            #line hidden
            
            #line 24 "中文名.tt"

if(Table.Columns.Count>0)
{

            
            #line default
            #line hidden
            
            #line 27 "中文名.tt"
            this.Write("\r\n        #region 属性");
            
            #line default
            #line hidden
            
            #line 28 "中文名.tt"

    foreach(IDataColumn Field in Table.Columns)
    {
        String des = FormatUtil.ToSigleDisplay(Field.Description);
		var fieldType = TemplateHelper.FormatUtil.ToFieldTypeString(Field);
		var fieldName = TemplateHelper.FormatUtil.ToCodeName(Field.Name);

		if(Field.PrimaryKey)
		{
            
            #line default
            #line hidden
            
            #line 36 "中文名.tt"
            this.Write("\r\n\t\t/// <summary>Original");
            
            #line default
            #line hidden
            
            #line 37 "中文名.tt"
            this.Write(des);
            
            #line default
            #line hidden
            
            #line 37 "中文名.tt"
            this.Write("</summary>\r\n\t\tpublic virtual ");
            
            #line default
            #line hidden
            
            #line 38 "中文名.tt"
            this.Write(fieldType);
            
            #line default
            #line hidden
            
            #line 38 "中文名.tt"
            this.Write(" Original");
            
            #line default
            #line hidden
            
            #line 38 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 38 "中文名.tt"
            this.Write(" { get; set; }\r\n\t\t");
            
            #line default
            #line hidden
            
            #line 39 "中文名.tt"
}

            
            #line default
            #line hidden
            
            #line 40 "中文名.tt"
            this.Write("\r\n        /// <summary>");
            
            #line default
            #line hidden
            
            #line 41 "中文名.tt"
            this.Write(des);
            
            #line default
            #line hidden
            
            #line 41 "中文名.tt"
            this.Write("</summary>\r\n\t\tpublic virtual ");
            
            #line default
            #line hidden
            
            #line 42 "中文名.tt"
            this.Write(fieldType);
            
            #line default
            #line hidden
            
            #line 42 "中文名.tt"
            this.Write(" ");
            
            #line default
            #line hidden
            
            #line 42 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 42 "中文名.tt"
            this.Write(" { get; set; }\r\n");
            
            #line default
            #line hidden
            
            #line 43 "中文名.tt"

    }

            
            #line default
            #line hidden
            
            #line 45 "中文名.tt"
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
            
            #line 75 "中文名.tt"

    foreach(IDataColumn Field in Table.Columns)
    {
		var fieldName = TemplateHelper.FormatUtil.ToCodeName(Field.Name);

		if(Field.PrimaryKey){
            
            #line default
            #line hidden
            
            #line 80 "中文名.tt"
            this.Write("\r\n\t\t\t\t\tcase __.Original");
            
            #line default
            #line hidden
            
            #line 81 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 81 "中文名.tt"
            this.Write(" : return Original");
            
            #line default
            #line hidden
            
            #line 81 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 81 "中文名.tt"
            this.Write(";");
            
            #line default
            #line hidden
            
            #line 81 "中文名.tt"

		}

            
            #line default
            #line hidden
            
            #line 83 "中文名.tt"
            this.Write("\r\n                    case __.");
            
            #line default
            #line hidden
            
            #line 84 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 84 "中文名.tt"
            this.Write(" : return ");
            
            #line default
            #line hidden
            
            #line 84 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 84 "中文名.tt"
            this.Write(";");
            
            #line default
            #line hidden
            
            #line 84 "中文名.tt"

    }

            
            #line default
            #line hidden
            
            #line 86 "中文名.tt"
            this.Write("\r\n                    default: return null;\r\n                }\r\n            }\r\n  " +
                    "          set\r\n            {\r\n                switch (name)\r\n                {");
            
            #line default
            #line hidden
            
            #line 93 "中文名.tt"

    Type conv=typeof(Convert);
    foreach(IDataColumn Field in Table.Columns)
    {
		var fieldName = TemplateHelper.FormatUtil.ToCodeName(Field.Name);

		if(Field.PrimaryKey){
			if(conv.GetMethod("To"+Field.DataType.Name, new Type[]{typeof(Object)})!=null){
		
            
            #line default
            #line hidden
            
            #line 101 "中文名.tt"
            this.Write("\r\n\t\t\t\t\tcase __.Original");
            
            #line default
            #line hidden
            
            #line 102 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 102 "中文名.tt"
            this.Write(" : Original");
            
            #line default
            #line hidden
            
            #line 102 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 102 "中文名.tt"
            this.Write(" = Convert.To");
            
            #line default
            #line hidden
            
            #line 102 "中文名.tt"
            this.Write(Field.DataType.Name);
            
            #line default
            #line hidden
            
            #line 102 "中文名.tt"
            this.Write("(value); break;");
            
            #line default
            #line hidden
            
            #line 102 "中文名.tt"

			}else{
		
            
            #line default
            #line hidden
            
            #line 104 "中文名.tt"
            this.Write("\r\n\t\t\t\t\tcase __.Original");
            
            #line default
            #line hidden
            
            #line 105 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 105 "中文名.tt"
            this.Write(" : Original");
            
            #line default
            #line hidden
            
            #line 105 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 105 "中文名.tt"
            this.Write(" = (");
            
            #line default
            #line hidden
            
            #line 105 "中文名.tt"
            this.Write(Field.DataType.Name);
            
            #line default
            #line hidden
            
            #line 105 "中文名.tt"
            this.Write(")value; break;");
            
            #line default
            #line hidden
            
            #line 105 "中文名.tt"

			}
		}

        if(conv.GetMethod("To"+Field.DataType.Name, new Type[]{typeof(Object)})!=null){

            
            #line default
            #line hidden
            
            #line 110 "中文名.tt"
            this.Write("\r\n                    case __.");
            
            #line default
            #line hidden
            
            #line 111 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 111 "中文名.tt"
            this.Write(" : ");
            
            #line default
            #line hidden
            
            #line 111 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 111 "中文名.tt"
            this.Write(" = Convert.To");
            
            #line default
            #line hidden
            
            #line 111 "中文名.tt"
            this.Write(Field.DataType.Name);
            
            #line default
            #line hidden
            
            #line 111 "中文名.tt"
            this.Write("(value); break;");
            
            #line default
            #line hidden
            
            #line 111 "中文名.tt"

        }else{

            
            #line default
            #line hidden
            
            #line 113 "中文名.tt"
            this.Write("\r\n                    case __.");
            
            #line default
            #line hidden
            
            #line 114 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 114 "中文名.tt"
            this.Write(" : ");
            
            #line default
            #line hidden
            
            #line 114 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 114 "中文名.tt"
            this.Write(" = (");
            
            #line default
            #line hidden
            
            #line 114 "中文名.tt"
            this.Write(Field.DataType.Name);
            
            #line default
            #line hidden
            
            #line 114 "中文名.tt"
            this.Write(")value; break;");
            
            #line default
            #line hidden
            
            #line 114 "中文名.tt"

        }
    }

            
            #line default
            #line hidden
            
            #line 117 "中文名.tt"
            this.Write("\r\n                    default: break;\r\n\t\t\t\t}\r\n            }\r\n        }\r\n        #" +
                    "endregion\r\n");
            
            #line default
            #line hidden
            
            #line 123 "中文名.tt"

}

            
            #line default
            #line hidden
            
            #line 125 "中文名.tt"
            this.Write("\r\n        #region 字段信息\r\n\r\n\t\t/// <summary>取得");
            
            #line default
            #line hidden
            
            #line 128 "中文名.tt"
            this.Write(tdis);
            
            #line default
            #line hidden
            
            #line 128 "中文名.tt"
            this.Write("字段名称的快捷方式</summary>\r\n        public partial class __\r\n        {");
            
            #line default
            #line hidden
            
            #line 130 "中文名.tt"

foreach(IDataColumn Field in Table.GetAllColumns(Tables, true))
{
	var fieldName = TemplateHelper.FormatUtil.ToCodeName(Field.Name);

    String des = FormatUtil.ToSigleDisplay(Field.Description);

	if(Field.PrimaryKey){

            
            #line default
            #line hidden
            
            #line 138 "中文名.tt"
            this.Write("\r\n\t\t\t///<summary>原始主键，");
            
            #line default
            #line hidden
            
            #line 139 "中文名.tt"
            this.Write(des);
            
            #line default
            #line hidden
            
            #line 139 "中文名.tt"
            this.Write("</summary>\r\n            public const String Original");
            
            #line default
            #line hidden
            
            #line 140 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 140 "中文名.tt"
            this.Write(" = \"Original");
            
            #line default
            #line hidden
            
            #line 140 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 140 "中文名.tt"
            this.Write("\";\r\n");
            
            #line default
            #line hidden
            
            #line 141 "中文名.tt"
}
            
            #line default
            #line hidden
            
            #line 141 "中文名.tt"
            this.Write("\r\n            ///<summary>");
            
            #line default
            #line hidden
            
            #line 142 "中文名.tt"
            this.Write(des);
            
            #line default
            #line hidden
            
            #line 142 "中文名.tt"
            this.Write("</summary>\r\n            public const String ");
            
            #line default
            #line hidden
            
            #line 143 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 143 "中文名.tt"
            this.Write(" = \"");
            
            #line default
            #line hidden
            
            #line 143 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 143 "中文名.tt"
            this.Write("\";\r\n");
            
            #line default
            #line hidden
            
            #line 144 "中文名.tt"

}

            
            #line default
            #line hidden
            
            #line 146 "中文名.tt"
            this.Write("\r\n        }\r\n\r\n        /// <summary>取得");
            
            #line default
            #line hidden
            
            #line 149 "中文名.tt"
            this.Write(tdis);
            
            #line default
            #line hidden
            
            #line 149 "中文名.tt"
            this.Write("字段信息的快捷方式</summary>\r\n        public partial class _\r\n        {\r\n\t\t\t/// <summary>\r" +
                    "\n            /// 数据库表名\r\n            /// </summary>\r\n            public const str" +
                    "ing DataBaseTableName = \"");
            
            #line default
            #line hidden
            
            #line 155 "中文名.tt"
            this.Write(Table.TableName);
            
            #line default
            #line hidden
            
            #line 155 "中文名.tt"
            this.Write("\";\r\n");
            
            #line default
            #line hidden
            
            #line 156 "中文名.tt"

foreach(IDataColumn Field in Table.GetAllColumns(Tables, true))
{
	var fieldName = TemplateHelper.FormatUtil.ToCodeName(Field.Name);
    String des=Field.Description;
    if(!String.IsNullOrEmpty(des)) des=des.Replace("\r\n"," ");

	if(Field.PrimaryKey){

            
            #line default
            #line hidden
            
            #line 164 "中文名.tt"
            this.Write("\r\n            ///<summary>原始主键,");
            
            #line default
            #line hidden
            
            #line 165 "中文名.tt"
            this.Write(des);
            
            #line default
            #line hidden
            
            #line 165 "中文名.tt"
            this.Write("</summary>\r\n            public static readonly Field Original");
            
            #line default
            #line hidden
            
            #line 166 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 166 "中文名.tt"
            this.Write(" = new Field\r\n            {\r\n                Name = __.Original");
            
            #line default
            #line hidden
            
            #line 168 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 168 "中文名.tt"
            this.Write(",\r\n\t\t\t\tColumnName = \"");
            
            #line default
            #line hidden
            
            #line 169 "中文名.tt"
            this.Write(Field.Name);
            
            #line default
            #line hidden
            
            #line 169 "中文名.tt"
            this.Write("\",\r\n                DisplayName = \"");
            
            #line default
            #line hidden
            
            #line 170 "中文名.tt"
            this.Write(Field.DisplayName);
            
            #line default
            #line hidden
            
            #line 170 "中文名.tt"
            this.Write("\",\r\n                Description = \"");
            
            #line default
            #line hidden
            
            #line 171 "中文名.tt"
            this.Write((""+Field.Description).Replace("\\", "\\\\"));
            
            #line default
            #line hidden
            
            #line 171 "中文名.tt"
            this.Write("\",\r\n                DataType = DbType.");
            
            #line default
            #line hidden
            
            #line 172 "中文名.tt"
            this.Write(Field.DataType.Name);
            
            #line default
            #line hidden
            
            #line 172 "中文名.tt"
            this.Write(",\r\n                DefaultValue = ");
            
            #line default
            #line hidden
            
            #line 173 "中文名.tt"
            this.Write(Field.Default==null?"null":"\""+Field.Default.Replace("\\", "\\\\")+"\"");
            
            #line default
            #line hidden
            
            #line 173 "中文名.tt"
            this.Write(",\r\n                IsPrimaryKey = ");
            
            #line default
            #line hidden
            
            #line 174 "中文名.tt"
            this.Write(Field.PrimaryKey.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 174 "中文名.tt"
            this.Write(",\r\n\t\t\t\tIdentity = ");
            
            #line default
            #line hidden
            
            #line 175 "中文名.tt"
            this.Write(Field.Identity.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 175 "中文名.tt"
            this.Write(",\r\n                IsReadonly = true,\r\n                IsNullable = ");
            
            #line default
            #line hidden
            
            #line 177 "中文名.tt"
            this.Write(Field.Nullable.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 177 "中文名.tt"
            this.Write(",\r\n                Length = ");
            
            #line default
            #line hidden
            
            #line 178 "中文名.tt"
            this.Write(Field.Length);
            
            #line default
            #line hidden
            
            #line 178 "中文名.tt"
            this.Write(",\r\n                Precision = ");
            
            #line default
            #line hidden
            
            #line 179 "中文名.tt"
            this.Write(Field.Precision);
            
            #line default
            #line hidden
            
            #line 179 "中文名.tt"
            this.Write(",\r\n                Scale = ");
            
            #line default
            #line hidden
            
            #line 180 "中文名.tt"
            this.Write(Field.Scale);
            
            #line default
            #line hidden
            
            #line 180 "中文名.tt"
            this.Write("\r\n\t\t\t};\r\n");
            
            #line default
            #line hidden
            
            #line 182 "中文名.tt"
}
            
            #line default
            #line hidden
            
            #line 182 "中文名.tt"
            this.Write("\r\n            ///<summary>");
            
            #line default
            #line hidden
            
            #line 183 "中文名.tt"
            this.Write(des);
            
            #line default
            #line hidden
            
            #line 183 "中文名.tt"
            this.Write("</summary>\r\n            public static readonly Field ");
            
            #line default
            #line hidden
            
            #line 184 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 184 "中文名.tt"
            this.Write(" = new Field\r\n            {\r\n                Name = __.");
            
            #line default
            #line hidden
            
            #line 186 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 186 "中文名.tt"
            this.Write(",\r\n\t\t\t\tColumnName = \"");
            
            #line default
            #line hidden
            
            #line 187 "中文名.tt"
            this.Write(Field.Name);
            
            #line default
            #line hidden
            
            #line 187 "中文名.tt"
            this.Write("\",\r\n                DisplayName = \"");
            
            #line default
            #line hidden
            
            #line 188 "中文名.tt"
            this.Write(Field.DisplayName);
            
            #line default
            #line hidden
            
            #line 188 "中文名.tt"
            this.Write("\",\r\n                Description = \"");
            
            #line default
            #line hidden
            
            #line 189 "中文名.tt"
            this.Write((""+Field.Description).Replace("\\", "\\\\"));
            
            #line default
            #line hidden
            
            #line 189 "中文名.tt"
            this.Write("\",\r\n                DataType = DbType.");
            
            #line default
            #line hidden
            
            #line 190 "中文名.tt"
            this.Write(Field.DataType.Name);
            
            #line default
            #line hidden
            
            #line 190 "中文名.tt"
            this.Write(",\r\n                DefaultValue = ");
            
            #line default
            #line hidden
            
            #line 191 "中文名.tt"
            this.Write(Field.Default==null?"null":"\""+Field.Default.Replace("\\", "\\\\")+"\"");
            
            #line default
            #line hidden
            
            #line 191 "中文名.tt"
            this.Write(",\r\n                IsPrimaryKey = ");
            
            #line default
            #line hidden
            
            #line 192 "中文名.tt"
            this.Write(Field.PrimaryKey.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 192 "中文名.tt"
            this.Write(",\r\n\t\t\t\tIdentity = ");
            
            #line default
            #line hidden
            
            #line 193 "中文名.tt"
            this.Write(Field.Identity.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 193 "中文名.tt"
            this.Write(",\r\n                IsReadonly = false,\r\n                IsNullable = ");
            
            #line default
            #line hidden
            
            #line 195 "中文名.tt"
            this.Write(Field.Nullable.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 195 "中文名.tt"
            this.Write(",\r\n                Length = ");
            
            #line default
            #line hidden
            
            #line 196 "中文名.tt"
            this.Write(Field.Length);
            
            #line default
            #line hidden
            
            #line 196 "中文名.tt"
            this.Write(",\r\n                Precision = ");
            
            #line default
            #line hidden
            
            #line 197 "中文名.tt"
            this.Write(Field.Precision);
            
            #line default
            #line hidden
            
            #line 197 "中文名.tt"
            this.Write(",\r\n                Scale = ");
            
            #line default
            #line hidden
            
            #line 198 "中文名.tt"
            this.Write(Field.Scale);
            
            #line default
            #line hidden
            
            #line 198 "中文名.tt"
            this.Write("\r\n\t\t\t};\r\n");
            
            #line default
            #line hidden
            
            #line 200 "中文名.tt"

}

            
            #line default
            #line hidden
            
            #line 202 "中文名.tt"
            this.Write("\r\n\t\t\t///<summary>所有字段列表</summary>\r\n\t\t\tpublic static readonly IList<Field> AllFiel" +
                    "ds = new List<Field>\r\n            {");
            
            #line default
            #line hidden
            
            #line 205 "中文名.tt"

foreach(IDataColumn Field in Table.GetAllColumns(Tables, true))
{
	var fieldName = TemplateHelper.FormatUtil.ToCodeName(Field.Name);
	if(Field.PrimaryKey){
            
            #line default
            #line hidden
            
            #line 209 "中文名.tt"
            this.Write("\r\n\t\t\t\tOriginal");
            
            #line default
            #line hidden
            
            #line 210 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 210 "中文名.tt"
            this.Write(",");
            
            #line default
            #line hidden
            
            #line 210 "中文名.tt"
}
            
            #line default
            #line hidden
            
            #line 210 "中文名.tt"
            this.Write("\t\t\t\r\n\t\t\t\t");
            
            #line default
            #line hidden
            
            #line 211 "中文名.tt"
            this.Write(fieldName);
            
            #line default
            #line hidden
            
            #line 211 "中文名.tt"
            this.Write(",");
            
            #line default
            #line hidden
            
            #line 211 "中文名.tt"

}

            
            #line default
            #line hidden
            
            #line 213 "中文名.tt"
            this.Write("\r\n            };\r\n\r\n        }\r\n\r\n        #endregion\r\n    }\r\n}");
            
            #line default
            #line hidden
            return this.Output.ToString();
        }
    }
    
    #line default
    #line hidden
}
