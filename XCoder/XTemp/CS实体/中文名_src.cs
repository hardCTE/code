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
    
    
    #line 1 "中文名.cs"
    public class 中文名 : XCoder.XCoderBase
    {
        public override string Render()
        {
            
            #line 1 "中文名.cs"
            this.Write("/*\r\n * XCoder v");
            
            #line default
            #line hidden
            
            #line 2 "中文名.cs"
            this.Write(Version);
            
            #line default
            #line hidden
            
            #line 2 "中文名.cs"
            this.Write("\r\n * 作者：");
            
            #line default
            #line hidden
            
            #line 3 "中文名.cs"
            this.Write(Environment.UserName + "/" + Environment.MachineName);
            
            #line default
            #line hidden
            
            #line 3 "中文名.cs"
            this.Write("\r\n * 时间：");
            
            #line default
            #line hidden
            
            #line 4 "中文名.cs"
            this.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            
            #line default
            #line hidden
            
            #line 4 "中文名.cs"
            this.Write("\r\n * 版权：xdb 2016~");
            
            #line default
            #line hidden
            
            #line 5 "中文名.cs"
            this.Write(DateTime.Now.ToString("yyyy"));
            
            #line default
            #line hidden
            
            #line 5 "中文名.cs"
            this.Write("\r\n*/\r\n﻿using System;\r\nusing System.Collections.Generic;\r\nusing System.Data;\r\n﻿usi" +
                    "ng App.DAL;\r\n\r\nnamespace ");
            
            #line default
            #line hidden
            
            #line 12 "中文名.cs"
            this.Write(Config.NameSpace);
            
            #line default
            #line hidden
            
            #line 12 "中文名.cs"
            this.Write("\r\n{");
            
            #line default
            #line hidden
            
            #line 13 "中文名.cs"

    String tdis=Table.DisplayName;
    if(!String.IsNullOrEmpty(tdis)) tdis=tdis.Replace("\r\n"," ").Replace("\\", "\\\\").Replace("'", "").Replace("\"", "");
    String tdes=Table.Description;
    if(!String.IsNullOrEmpty(tdes)) tdes=tdes.Replace("\r\n"," ").Replace("\\", "\\\\").Replace("'", "").Replace("\"", "");
    if(String.IsNullOrEmpty(tdis)) tdis=tdes;
    
            
            #line default
            #line hidden
            
            #line 19 "中文名.cs"
            this.Write("\r\n    /// <summary>");
            
            #line default
            #line hidden
            
            #line 20 "中文名.cs"
            this.Write(tdis);
            
            #line default
            #line hidden
            
            #line 20 "中文名.cs"
            this.Write("</summary>");
            
            #line default
            #line hidden
            
            #line 20 "中文名.cs"
 if(tdis!=tdes){
            
            #line default
            #line hidden
            
            #line 20 "中文名.cs"
            this.Write("\r\n\t/// <remarks>");
            
            #line default
            #line hidden
            
            #line 21 "中文名.cs"
            this.Write(tdes);
            
            #line default
            #line hidden
            
            #line 21 "中文名.cs"
            this.Write("</remarks>");
            
            #line default
            #line hidden
            
            #line 21 "中文名.cs"
}
            
            #line default
            #line hidden
            
            #line 21 "中文名.cs"
            this.Write("\r\n    [Description(\"");
            
            #line default
            #line hidden
            
            #line 22 "中文名.cs"
            this.Write(tdes);
            
            #line default
            #line hidden
            
            #line 22 "中文名.cs"
            this.Write("\")]");
            
            #line default
            #line hidden
            
            #line 22 "中文名.cs"

foreach(IDataIndex di in Table.Indexes){if(di.Columns==null||di.Columns.Length<1)continue;
            
            #line default
            #line hidden
            
            #line 23 "中文名.cs"
            this.Write("\r\n    [BindIndex(\"");
            
            #line default
            #line hidden
            
            #line 24 "中文名.cs"
            this.Write(di.Name);
            
            #line default
            #line hidden
            
            #line 24 "中文名.cs"
            this.Write("\", ");
            
            #line default
            #line hidden
            
            #line 24 "中文名.cs"
            this.Write(di.Unique.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 24 "中文名.cs"
            this.Write(", \"");
            
            #line default
            #line hidden
            
            #line 24 "中文名.cs"
            this.Write(String.Join(",", di.Columns));
            
            #line default
            #line hidden
            
            #line 24 "中文名.cs"
            this.Write("\")]");
            
            #line default
            #line hidden
            
            #line 24 "中文名.cs"

}
foreach(IDataRelation dr in Table.Relations){
            
            #line default
            #line hidden
            
            #line 26 "中文名.cs"
            this.Write("\r\n    [BindRelation(\"");
            
            #line default
            #line hidden
            
            #line 27 "中文名.cs"
            this.Write(dr.Column);
            
            #line default
            #line hidden
            
            #line 27 "中文名.cs"
            this.Write("\", ");
            
            #line default
            #line hidden
            
            #line 27 "中文名.cs"
            this.Write(dr.Unique.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 27 "中文名.cs"
            this.Write(", \"");
            
            #line default
            #line hidden
            
            #line 27 "中文名.cs"
            this.Write(dr.RelationTable);
            
            #line default
            #line hidden
            
            #line 27 "中文名.cs"
            this.Write("\", \"");
            
            #line default
            #line hidden
            
            #line 27 "中文名.cs"
            this.Write(dr.RelationColumn);
            
            #line default
            #line hidden
            
            #line 27 "中文名.cs"
            this.Write("\")]");
            
            #line default
            #line hidden
            
            #line 27 "中文名.cs"
}
            
            #line default
            #line hidden
            
            #line 27 "中文名.cs"
            this.Write("\r\n    [BindTable(\"");
            
            #line default
            #line hidden
            
            #line 28 "中文名.cs"
            this.Write(Table.TableName);
            
            #line default
            #line hidden
            
            #line 28 "中文名.cs"
            this.Write("\", Description = \"");
            
            #line default
            #line hidden
            
            #line 28 "中文名.cs"
            this.Write(tdes);
            
            #line default
            #line hidden
            
            #line 28 "中文名.cs"
            this.Write("\", ConnName = \"");
            
            #line default
            #line hidden
            
            #line 28 "中文名.cs"
            this.Write(Table.ConnName ?? Config.EntityConnName);
            
            #line default
            #line hidden
            
            #line 28 "中文名.cs"
            this.Write("\", DbType = DatabaseType.");
            
            #line default
            #line hidden
            
            #line 28 "中文名.cs"
            this.Write(Table.DbType);
            
            #line default
            #line hidden
            
            #line 28 "中文名.cs"
if(Table.IsView){
            
            #line default
            #line hidden
            
            #line 28 "中文名.cs"
            this.Write(", IsView = true");
            
            #line default
            #line hidden
            
            #line 28 "中文名.cs"
}
            
            #line default
            #line hidden
            
            #line 28 "中文名.cs"
            this.Write(")]\r\n    public partial class ");
            
            #line default
            #line hidden
            
            #line 29 "中文名.cs"
            this.Write(Table.Name);
            
            #line default
            #line hidden
            
            #line 29 "中文名.cs"
            this.Write("Dal\r\n    {");
            
            #line default
            #line hidden
            
            #line 30 "中文名.cs"

if(Table.Columns.Count>0)
{

            
            #line default
            #line hidden
            
            #line 33 "中文名.cs"
            this.Write("\r\n        #region 属性");
            
            #line default
            #line hidden
            
            #line 34 "中文名.cs"

    foreach(IDataColumn Field in Table.Columns)
    {
        String des=Field.Description;
        if(!String.IsNullOrEmpty(des)) des=des.Replace("\r\n"," ").Replace("\\", "\\\\").Replace("'", "").Replace("\"", "");
        String dis = Field.DisplayName;
        if(!String.IsNullOrEmpty(dis)) dis=dis.Replace("\r\n"," ").Replace("'", " ").Replace("\"", "");

            
            #line default
            #line hidden
            
            #line 41 "中文名.cs"
            this.Write("\r\n        /// <summary>");
            
            #line default
            #line hidden
            
            #line 42 "中文名.cs"
            this.Write(des);
            
            #line default
            #line hidden
            
            #line 42 "中文名.cs"
            this.Write("</summary>\r\n\t\tpublic virtual ");
            
            #line default
            #line hidden
            
            #line 43 "中文名.cs"
            this.Write(Field.DataType==null?"":Field.DataType.Name);
            
            #line default
            #line hidden
            
            #line 43 "中文名.cs"
            this.Write(" ");
            
            #line default
            #line hidden
            
            #line 43 "中文名.cs"
            this.Write(Field.Name);
            
            #line default
            #line hidden
            
            #line 43 "中文名.cs"
            this.Write(" { get; set; }\r\n");
            
            #line default
            #line hidden
            
            #line 44 "中文名.cs"

    }

            
            #line default
            #line hidden
            
            #line 46 "中文名.cs"
            this.Write(@"        #endregion

        #region 获取/设置 字段值
        /// <summary>
        /// 获取/设置 字段值。
        /// 一个索引，基类使用反射实现。
        /// 派生实体类可重写该索引，以避免反射带来的性能损耗
        /// </summary>
        /// <param name=""name"">字段名</param>
        /// <returns></returns>
        public virtual Object this[String name]
        {
            get
            {
                switch (name)
                {");
            
            #line default
            #line hidden
            
            #line 61 "中文名.cs"

    foreach(IDataColumn Field in Table.Columns)
    {

            
            #line default
            #line hidden
            
            #line 64 "中文名.cs"
            this.Write("\r\n                    case __.");
            
            #line default
            #line hidden
            
            #line 65 "中文名.cs"
            this.Write(Field.Name);
            
            #line default
            #line hidden
            
            #line 65 "中文名.cs"
            this.Write(" : return ");
            
            #line default
            #line hidden
            
            #line 65 "中文名.cs"
            this.Write(Field.Name);
            
            #line default
            #line hidden
            
            #line 65 "中文名.cs"
            this.Write(";");
            
            #line default
            #line hidden
            
            #line 65 "中文名.cs"

    }

            
            #line default
            #line hidden
            
            #line 67 "中文名.cs"
            this.Write("\r\n                    default: return null;\r\n                }\r\n            }\r\n  " +
                    "          set\r\n            {\r\n                switch (name)\r\n                {");
            
            #line default
            #line hidden
            
            #line 74 "中文名.cs"

    Type conv=typeof(Convert);
    foreach(IDataColumn Field in Table.Columns)
    { 
        if(conv.GetMethod("To"+Field.DataType.Name, new Type[]{typeof(Object)})!=null){

            
            #line default
            #line hidden
            
            #line 79 "中文名.cs"
            this.Write("\r\n                    case __.");
            
            #line default
            #line hidden
            
            #line 80 "中文名.cs"
            this.Write(Field.Name);
            
            #line default
            #line hidden
            
            #line 80 "中文名.cs"
            this.Write(" : ");
            
            #line default
            #line hidden
            
            #line 80 "中文名.cs"
            this.Write(Field.Name);
            
            #line default
            #line hidden
            
            #line 80 "中文名.cs"
            this.Write(" = Convert.To");
            
            #line default
            #line hidden
            
            #line 80 "中文名.cs"
            this.Write(Field.DataType.Name);
            
            #line default
            #line hidden
            
            #line 80 "中文名.cs"
            this.Write("(value); break;");
            
            #line default
            #line hidden
            
            #line 80 "中文名.cs"

        }else{

            
            #line default
            #line hidden
            
            #line 82 "中文名.cs"
            this.Write("\r\n                    case __.");
            
            #line default
            #line hidden
            
            #line 83 "中文名.cs"
            this.Write(Field.Name);
            
            #line default
            #line hidden
            
            #line 83 "中文名.cs"
            this.Write(" : ");
            
            #line default
            #line hidden
            
            #line 83 "中文名.cs"
            this.Write(Field.Name);
            
            #line default
            #line hidden
            
            #line 83 "中文名.cs"
            this.Write(" = (");
            
            #line default
            #line hidden
            
            #line 83 "中文名.cs"
            this.Write(Field.DataType.Name);
            
            #line default
            #line hidden
            
            #line 83 "中文名.cs"
            this.Write(")value; break;");
            
            #line default
            #line hidden
            
            #line 83 "中文名.cs"

        }
    }

            
            #line default
            #line hidden
            
            #line 86 "中文名.cs"
            this.Write("\r\n                    default: break;\r\n\t\t\t\t}\r\n            }\r\n        }\r\n        #" +
                    "endregion\r\n");
            
            #line default
            #line hidden
            
            #line 92 "中文名.cs"

}

            
            #line default
            #line hidden
            
            #line 94 "中文名.cs"
            this.Write("\r\n        #region 字段信息\r\n\r\n\t\t/// <summary>取得");
            
            #line default
            #line hidden
            
            #line 97 "中文名.cs"
            this.Write(tdis);
            
            #line default
            #line hidden
            
            #line 97 "中文名.cs"
            this.Write("字段名称的快捷方式</summary>\r\n        public partial class __\r\n        {\r\n\t\t\t/// <summary>" +
                    "\r\n            /// 数据库表名\r\n            /// </summary>\r\n            public const st" +
                    "ring DataBaseTableName = \"");
            
            #line default
            #line hidden
            
            #line 103 "中文名.cs"
            this.Write(Table.TableName);
            
            #line default
            #line hidden
            
            #line 103 "中文名.cs"
            this.Write("\";\r\n");
            
            #line default
            #line hidden
            
            #line 104 "中文名.cs"

foreach(IDataColumn Field in Table.GetAllColumns(Tables, true))
{
    String des=Field.Description;
    if(!String.IsNullOrEmpty(des)) des=des.Replace("\r\n"," ");

            
            #line default
            #line hidden
            
            #line 109 "中文名.cs"
            this.Write("\r\n            ///<summary>");
            
            #line default
            #line hidden
            
            #line 110 "中文名.cs"
            this.Write(des);
            
            #line default
            #line hidden
            
            #line 110 "中文名.cs"
            this.Write("</summary>\r\n            public const String ");
            
            #line default
            #line hidden
            
            #line 111 "中文名.cs"
            this.Write(Field.Name);
            
            #line default
            #line hidden
            
            #line 111 "中文名.cs"
            this.Write(" = \"");
            
            #line default
            #line hidden
            
            #line 111 "中文名.cs"
            this.Write(Field.Name);
            
            #line default
            #line hidden
            
            #line 111 "中文名.cs"
            this.Write("\";\r\n");
            
            #line default
            #line hidden
            
            #line 112 "中文名.cs"

}

            
            #line default
            #line hidden
            
            #line 114 "中文名.cs"
            this.Write("\r\n        }\r\n\r\n        /// <summary>取得");
            
            #line default
            #line hidden
            
            #line 117 "中文名.cs"
            this.Write(tdis);
            
            #line default
            #line hidden
            
            #line 117 "中文名.cs"
            this.Write("字段信息的快捷方式</summary>\r\n        public partial class _\r\n        {");
            
            #line default
            #line hidden
            
            #line 119 "中文名.cs"

foreach(IDataColumn Field in Table.GetAllColumns(Tables, true))
{
    String des=Field.Description;
    if(!String.IsNullOrEmpty(des)) des=des.Replace("\r\n"," ");

            
            #line default
            #line hidden
            
            #line 124 "中文名.cs"
            this.Write("\r\n            ///<summary>");
            
            #line default
            #line hidden
            
            #line 125 "中文名.cs"
            this.Write(des);
            
            #line default
            #line hidden
            
            #line 125 "中文名.cs"
            this.Write("</summary>\r\n            public static readonly Field ");
            
            #line default
            #line hidden
            
            #line 126 "中文名.cs"
            this.Write(Field.Name);
            
            #line default
            #line hidden
            
            #line 126 "中文名.cs"
            this.Write(" = new Field\r\n            {\r\n                Name = __.");
            
            #line default
            #line hidden
            
            #line 128 "中文名.cs"
            this.Write(Field.Name);
            
            #line default
            #line hidden
            
            #line 128 "中文名.cs"
            this.Write(",\r\n\t\t\t\tColumnName = \"");
            
            #line default
            #line hidden
            
            #line 129 "中文名.cs"
            this.Write(Field.Name);
            
            #line default
            #line hidden
            
            #line 129 "中文名.cs"
            this.Write("\",\r\n                DisplayName = \"");
            
            #line default
            #line hidden
            
            #line 130 "中文名.cs"
            this.Write(Field.DisplayName);
            
            #line default
            #line hidden
            
            #line 130 "中文名.cs"
            this.Write("\",\r\n                Description = \"");
            
            #line default
            #line hidden
            
            #line 131 "中文名.cs"
            this.Write((""+Field.Description).Replace("\\", "\\\\"));
            
            #line default
            #line hidden
            
            #line 131 "中文名.cs"
            this.Write("\",\r\n                DataType = DbType.");
            
            #line default
            #line hidden
            
            #line 132 "中文名.cs"
            this.Write(Field.DataType.Name);
            
            #line default
            #line hidden
            
            #line 132 "中文名.cs"
            this.Write(",\r\n                DefaultValue = ");
            
            #line default
            #line hidden
            
            #line 133 "中文名.cs"
            this.Write(Field.Default==null?"null":"\""+Field.Default.Replace("\\", "\\\\")+"\"");
            
            #line default
            #line hidden
            
            #line 133 "中文名.cs"
            this.Write(",\r\n                IsPrimaryKey = ");
            
            #line default
            #line hidden
            
            #line 134 "中文名.cs"
            this.Write(Field.PrimaryKey.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 134 "中文名.cs"
            this.Write(",\r\n                IsReadonly = false,\r\n                IsNullable = ");
            
            #line default
            #line hidden
            
            #line 136 "中文名.cs"
            this.Write(Field.Nullable.ToString().ToLower());
            
            #line default
            #line hidden
            
            #line 136 "中文名.cs"
            this.Write(",\r\n                Length = ");
            
            #line default
            #line hidden
            
            #line 137 "中文名.cs"
            this.Write(Field.Length);
            
            #line default
            #line hidden
            
            #line 137 "中文名.cs"
            this.Write(",\r\n                Precision = ");
            
            #line default
            #line hidden
            
            #line 138 "中文名.cs"
            this.Write(Field.Precision);
            
            #line default
            #line hidden
            
            #line 138 "中文名.cs"
            this.Write(",\r\n                Scale = ");
            
            #line default
            #line hidden
            
            #line 139 "中文名.cs"
            this.Write(Field.Scale);
            
            #line default
            #line hidden
            
            #line 139 "中文名.cs"
            this.Write("\r\n\t\t\t};\r\n");
            
            #line default
            #line hidden
            
            #line 141 "中文名.cs"

}

            
            #line default
            #line hidden
            
            #line 143 "中文名.cs"
            this.Write("\r\n        }\r\n\r\n        #endregion\r\n    }\r\n}");
            
            #line default
            #line hidden
            return this.Output.ToString();
        }
    }
    
    #line default
    #line hidden
}
