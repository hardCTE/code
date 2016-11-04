﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using NewLife;
using NewLife.Log;

namespace XCode.DataAccessLayer
{
    class SqlServer : RemoteDb
    {
        #region 属性
        /// <summary>返回数据库类型。外部DAL数据库类请使用Other</summary>
        public override DatabaseType DbType { get { return DatabaseType.SqlServer; } }

        /// <summary>工厂</summary>
        public override DbProviderFactory Factory { get { return SqlClientFactory.Instance; } }

        /// <summary>是否SQL2005及以上</summary>
        public Boolean IsSQL2005 { get { return Version.Major > 8; } }

        private Version _Version;
        /// <summary>是否SQL2005及以上</summary>
        public Version Version
        {
            get
            {
                if (_Version == null)
                {
                    //if (String.IsNullOrEmpty(ConnectionString)) return _Version = new Version();

                    var session = CreateSession();
                    try
                    {
                        //取数据库版本
                        if (!session.Opened) session.Open();
                        var ver = session.Conn.ServerVersion;
                        session.AutoClose();

                        _Version = new Version(ver);
                    }
                    catch (Exception ex)
                    {
                        XTrace.WriteLine("查询[{0}]的版本时出错，将按MSSQL2000进行分页处理！{1}", ConnName, ex);
                        _Version = new Version();
                    }
                    finally { session.Dispose(); }
                }
                return _Version;
            }
        }

        private String _DataPath;
        /// <summary>数据目录</summary>
        public String DataPath { get { return _DataPath; } set { _DataPath = value; } }

        const String Application_Name = "Application Name";
        protected override void OnSetConnectionString(XDbConnectionStringBuilder builder)
        {
            String str = null;
            // 获取数据目录，用于反向工程创建数据库
            if (builder.TryGetAndRemove("DataPath", out str) && !String.IsNullOrEmpty(str)) DataPath = str;

            base.OnSetConnectionString(builder);

            if (!builder.ContainsKey(Application_Name))
            {
                String name = Runtime.IsWeb ? HostingEnvironment.SiteName : AppDomain.CurrentDomain.FriendlyName;
                builder[Application_Name] = String.Format("XCode_{0}_{1}", name, ConnName);
            }
        }
        #endregion

        #region 方法
        /// <summary>创建数据库会话</summary>
        /// <returns></returns>
        protected override IDbSession OnCreateSession() { return new SqlServerSession(); }

        /// <summary>创建元数据对象</summary>
        /// <returns></returns>
        protected override IMetaData OnCreateMetaData() { return new SqlServerMetaData(); }

        public override bool Support(string providerName)
        {
            providerName = providerName.ToLower();
            if (providerName.Contains("system.data.sqlclient")) return true;
            if (providerName.Contains("sql2012")) return true;
            if (providerName.Contains("sql2008")) return true;
            if (providerName.Contains("sql2005")) return true;
            if (providerName.Contains("sql2000")) return true;
            if (providerName == "sqlclient") return true;
            if (providerName.Contains("mssql")) return true;
            if (providerName.Contains("sqlserver")) return true;

            return false;
        }
        #endregion

        #region 分页
        /// <summary>构造分页SQL</summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="startRowIndex">开始行，0表示第一行</param>
        /// <param name="maximumRows">最大返回行数，0表示所有行</param>
        /// <param name="keyColumn">唯一键。用于not in分页</param>
        /// <returns>分页SQL</returns>
        public override String PageSplit(String sql, Int32 startRowIndex, Int32 maximumRows, String keyColumn)
        {
            // 从第一行开始，不需要分页
            if (startRowIndex <= 0 && maximumRows < 1) return sql;

            // 指定了起始行，并且是SQL2005及以上版本，使用RowNumber算法
            if (startRowIndex > 0 && IsSQL2005)
            {
                //return PageSplitRowNumber(sql, startRowIndex, maximumRows, keyColumn);
                SelectBuilder builder = new SelectBuilder();
                builder.Parse(sql);
                return MSPageSplit.PageSplit(builder, startRowIndex, maximumRows, IsSQL2005).ToString();
            }

            // 如果没有Order By，直接调用基类方法
            // 先用字符串判断，命中率高，这样可以提高处理效率
            if (!sql.Contains(" Order "))
            {
                if (!sql.ToLower().Contains(" order ")) return base.PageSplit(sql, startRowIndex, maximumRows, keyColumn);
            }
            //// 使用正则进行严格判断。必须包含Order By，并且它右边没有右括号)，表明有order by，且不是子查询的，才需要特殊处理
            //MatchCollection ms = Regex.Matches(sql, @"\border\s*by\b([^)]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            //if (ms == null || ms.Count < 1 || ms[0].Index < 1)
            String sql2 = sql;
            String orderBy = CheckOrderClause(ref sql2);
            if (String.IsNullOrEmpty(orderBy))
            {
                return base.PageSplit(sql, startRowIndex, maximumRows, keyColumn);
            }
            // 已确定该sql最外层含有order by，再检查最外层是否有top。因为没有top的order by是不允许作为子查询的
            if (Regex.IsMatch(sql, @"^[^(]+\btop\b", RegexOptions.Compiled | RegexOptions.IgnoreCase))
            {
                return base.PageSplit(sql, startRowIndex, maximumRows, keyColumn);
            }
            //String orderBy = sql.Substring(ms[0].Index);

            // 从第一行开始，不需要分页
            if (startRowIndex <= 0)
            {
                if (maximumRows < 1)
                    return sql;
                else
                    return String.Format("Select Top {0} * From {1} {2}", maximumRows, CheckSimpleSQL(sql2), orderBy);
                //return String.Format("Select Top {0} * From {1} {2}", maximumRows, CheckSimpleSQL(sql.Substring(0, ms[0].Index)), orderBy);
            }

            #region Max/Min分页
            // 如果要使用max/min分页法，首先keyColumn必须有asc或者desc
            String kc = keyColumn.ToLower();
            if (kc.EndsWith(" desc") || kc.EndsWith(" asc") || kc.EndsWith(" unknown"))
            {
                String str = PageSplitMaxMin(sql, startRowIndex, maximumRows, keyColumn);
                if (!String.IsNullOrEmpty(str)) return str;
                keyColumn = keyColumn.Substring(0, keyColumn.IndexOf(" "));
            }
            #endregion

            sql = CheckSimpleSQL(sql2);

            if (String.IsNullOrEmpty(keyColumn)) throw new ArgumentNullException("keyColumn", "分页要求指定主键列或者排序字段！");

            if (maximumRows < 1)
                sql = String.Format("Select * From {1} Where {2} Not In(Select Top {0} {2} From {1} {3}) {3}", startRowIndex, sql, keyColumn, orderBy);
            else
                sql = String.Format("Select Top {0} * From {1} Where {2} Not In(Select Top {3} {2} From {1} {4}) {4}", maximumRows, sql, keyColumn, startRowIndex, orderBy);
            return sql;
        }

        public override SelectBuilder PageSplit(SelectBuilder builder, int startRowIndex, int maximumRows)
        {
            return MSPageSplit.PageSplit(builder, startRowIndex, maximumRows, IsSQL2005, b => CreateSession().QueryCount(b));
        }
        #endregion

        #region 数据库特性
        /// <summary>当前时间函数</summary>
        public override String DateTimeNow { get { return "getdate()"; } }

        /// <summary>最小时间</summary>
        public override DateTime DateTimeMin { get { return SqlDateTime.MinValue.Value; } }

        /// <summary>长文本长度</summary>
        public override Int32 LongTextLength { get { return 4000; } }

        /// <summary>获取Guid的函数</summary>
        public override String NewGuid { get { return "newid()"; } }

        /// <summary>格式化时间为SQL字符串</summary>
        /// <param name="dateTime">时间值</param>
        /// <returns></returns>
        public override String FormatDateTime(DateTime dateTime) { return "{ts'" + dateTime.ToFullString() + "'}"; }
        //public override String FormatDateTime(DateTime dateTime) { return "{ts" + String.Format("'{0:yyyy-MM-dd HH:mm:ss}'", dateTime) + "}"; }

        /// <summary>格式化名称，如果是关键字，则格式化后返回，否则原样返回</summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public override String FormatName(String name)
        {
            // SqlServer数据库名和表名可以用横线。。。
            if (name.Contains("-")) return "[{0}]".F(name);

            return base.FormatName(name);
        }

        /// <summary>格式化关键字</summary>
        /// <param name="keyWord">关键字</param>
        /// <returns></returns>
        public override String FormatKeyWord(String keyWord)
        {
            //if (String.IsNullOrEmpty(keyWord)) throw new ArgumentNullException("keyWord");
            if (String.IsNullOrEmpty(keyWord)) return keyWord;

            if (keyWord.StartsWith("[") && keyWord.EndsWith("]")) return keyWord;

            return String.Format("[{0}]", keyWord);
        }

        /// <summary>系统数据库名</summary>
        public override String SystemDatabaseName { get { return "master"; } }

        public override string FormatValue(IDataColumn field, object value)
        {
            TypeCode code = Type.GetTypeCode(field.DataType);
            Boolean isNullable = field.Nullable;

            if (code == TypeCode.String)
            {
                // 热心网友 Hannibal 在处理日文网站时发现插入的日文为乱码，这里加上N前缀
                if (value == null) return isNullable ? "null" : "''";
                //云飞扬：这里注释掉，空串返回''而不是null字符
                //if (String.IsNullOrEmpty(value.ToString()) && isNullable) return "null";

                // 这里直接判断原始数据类型有所不妥，如果原始数据库不是当前数据库，那么这里的判断将会失效
                // 一个可行的办法就是给XField增加一个IsUnicode属性，但如此一来，XField就稍微变大了
                // 目前暂时影响不大，后面看情况决定是否增加吧
                //if (field.RawType == "ntext" ||
                //    !String.IsNullOrEmpty(field.RawType) && (field.RawType.StartsWith("nchar") || field.RawType.StartsWith("nvarchar")))

                // 为了兼容旧版本实体类
                if (field.IsUnicode || IsUnicode(field.RawType))
                    return "N'" + value.ToString().Replace("'", "''") + "'";
                else
                    return "'" + value.ToString().Replace("'", "''") + "'";
            }
            //else if (field.DataType == typeof(Guid))
            //{
            //    if (value == null) return isNullable ? "null" : "''";

            //    return String.Format("'{0}'", value);
            //}

            return base.FormatValue(field, value);
        }
        #endregion
    }

    /// <summary>SqlServer数据库</summary>
    internal class SqlServerSession : RemoteDbSession
    {
        #region 查询
        /// <summary>快速查询单表记录数，稍有偏差</summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override Int64 QueryCountFast(string tableName)
        {
            tableName = tableName.Trim().Trim('[', ']').Trim();

            var n = 0L;
            if (QueryIndex().TryGetValue(tableName, out n)) return n;

            String sql = String.Format("select rows from sysindexes where id = object_id('{0}') and indid in (0,1)", tableName);
            return ExecuteScalar<Int64>(sql);
        }

        Dictionary<String, Int64> _index;
        DateTime _next;

        Dictionary<String, Int64> QueryIndex()
        {
            if (_index == null)
            {
                _next = DateTime.Now.AddSeconds(10);
                return _index = QueryIndex_();
            }

            // 检查更新
            if (_next < DateTime.Now)
            {
                // 先改时间，让别的线程先用着旧的
                _next = DateTime.Now.AddSeconds(10);
                //// 同一个会话里面，不担心分表分库的问题，倒是有可能有冲突
                //ThreadPool.QueueUserWorkItem(s => _index = QueryIndex_());

                _index = QueryIndex_();
            }

            // 直接返回旧的
            return _index;
        }

        Dictionary<String, Int64> QueryIndex_()
        {
            var ds = Query("select object_name(id) as objname,rows from sysindexes where indid in (0,1) and status in (0,2066)");
            var dic = new Dictionary<String, Int64>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                dic.Add(dr[0] + "", (Int32)dr[1]);
            }
            return dic;
        }

        /// <summary>执行插入语句并返回新增行的自动编号</summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="type">命令类型，默认SQL文本</param>
        /// <param name="ps">命令参数</param>
        /// <returns>新增行的自动编号</returns>
        public override Int64 InsertAndGetIdentity(string sql, CommandType type = CommandType.Text, params DbParameter[] ps)
        {
            return ExecuteScalar<Int64>("SET NOCOUNT ON;" + sql + ";Select SCOPE_IDENTITY()", type, ps);
        }
        #endregion
    }

    /// <summary>SqlServer元数据</summary>
    class SqlServerMetaData : RemoteDbMetaData
    {
        #region 属性
        /// <summary>是否SQL2005</summary>
        public Boolean IsSQL2005 { get { return (Database as SqlServer).IsSQL2005; } }

        public Version Version { get { return (Database as SqlServer).Version; } }

        /// <summary>0级类型</summary>
        public String level0type { get { return IsSQL2005 ? "SCHEMA" : "USER"; } }
        #endregion

        #region 构架
        /// <summary>取得所有表构架</summary>
        /// <returns></returns>
        protected override List<IDataTable> OnGetTables(ICollection<String> names)
        {
            #region 查表说明、字段信息、索引信息
            var session = Database.CreateSession();

            //一次性把所有的表说明查出来
            DataTable DescriptionTable = null;

            //var old = session.ShowSQL;
            //session.ShowSQL = false;
            try
            {
                DescriptionTable = session.Query(DescriptionSql).Tables[0];
            }
            catch { }
            //session.ShowSQL = old;

            var dt = GetSchema(_.Tables, null);
            if (dt == null || dt.Rows == null || dt.Rows.Count < 1) return null;

            //session.ShowSQL = false;
            try
            {
                AllFields = session.Query(SchemaSql).Tables[0];
                AllIndexes = session.Query(IndexSql).Tables[0];
            }
            catch { }
            //session.ShowSQL = old;
            #endregion

            // 列出用户表
            DataRow[] rows = dt.Select(String.Format("({0}='BASE TABLE' Or {0}='VIEW') AND TABLE_NAME<>'Sysdiagrams'", "TABLE_TYPE"));
            rows = OnGetTables(names, rows);
            if (rows == null || rows.Length < 1) return null;

            List<IDataTable> list = GetTables(rows);
            if (list == null || list.Count < 1) return list;

            // 修正备注
            foreach (IDataTable item in list)
            {
                DataRow[] drs = DescriptionTable == null ? null : DescriptionTable.Select("n='" + item.TableName + "'");
                item.Description = drs == null || drs.Length < 1 ? "" : drs[0][1].ToString();
            }

            return list;
        }

        private DataTable AllFields = null;
        private DataTable AllIndexes = null;

        protected override void FixField(IDataColumn field, DataRow dr)
        {
            base.FixField(field, dr);

            var rows = AllFields == null ? null : AllFields.Select("表名='" + field.Table.TableName + "' And 字段名='" + field.ColumnName + "'", null);
            if (rows != null && rows.Length > 0)
            {
                var dr2 = rows[0];

                field.Identity = GetDataRowValue<Boolean>(dr2, "标识");
                field.PrimaryKey = GetDataRowValue<Boolean>(dr2, "主键");
                field.NumOfByte = GetDataRowValue<Int32>(dr2, "占用字节数");
                field.Description = GetDataRowValue<String>(dr2, "字段说明");
            }

            // 整理默认值
            if (!String.IsNullOrEmpty(field.Default))
            {
                field.Default = Trim(field.Default, "(", ")");
                field.Default = Trim(field.Default, "\"", "\"");
                field.Default = Trim(field.Default, "\'", "\'");
                field.Default = Trim(field.Default, "N\'", "\'");
                field.Default = field.Default.Replace("''", "'");

                // 处理类似CONVERT([datetime],'1753-1-1',(0))的时间默认值
                if (field.DataType == typeof(DateTime))
                {
                    var def = field.Default;
                    if (def.StartsWithIgnoreCase("CONVERT("))
                    {
                        var ss = def.Split(",");
                        if (ss.Length >= 3)
                        {
                            def = ss[1].Trim().Trim('\'').Trim();
                            DateTime dt;
                            if (DateTime.TryParse(def, out dt))
                            {
                                if (dt == Database.DateTimeMin || dt.Date == Database.DateTimeMin.Date)
                                {
                                    //def = Database.DateTimeNow;
                                    //def = Database.DateTimeMin.ToString("yyyy-MM-dd");
                                    field.Default = def;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override List<IDataIndex> GetIndexes(IDataTable table)
        {
            List<IDataIndex> list = base.GetIndexes(table);
            if (list != null && list.Count > 0)
            {
                foreach (IDataIndex item in list)
                {
                    DataRow[] drs = AllIndexes == null ? null : AllIndexes.Select("name='" + item.Name + "'");
                    if (drs != null && drs.Length > 0)
                    {
                        item.Unique = GetDataRowValue<Boolean>(drs[0], "is_unique");
                        item.PrimaryKey = GetDataRowValue<Boolean>(drs[0], "is_primary_key");
                    }
                }
            }
            return list;
        }

        public override string CreateTableSQL(IDataTable table)
        {
            String sql = base.CreateTableSQL(table);

            var pks = table.PrimaryKeys;
            if (String.IsNullOrEmpty(sql) || pks == null || pks.Length < 2) return sql;

            // 处理多主键
            var sb = new StringBuilder();
            foreach (var item in pks)
            {
                if (sb.Length > 0) sb.Append(",");
                sb.Append(FormatName(item.ColumnName));
            }
            sql += "; " + Environment.NewLine;
            sql += String.Format("Alter Table {0} Add Constraint PK_{1} Primary Key Clustered({2})", FormatName(table.TableName), table.TableName, sb.ToString());
            return sql;
        }

        public override string FieldClause(IDataColumn field, bool onlyDefine)
        {
            if (!String.IsNullOrEmpty(field.RawType) && field.RawType.Contains("char(-1)"))
            {
                if (IsSQL2005)
                    field.RawType = field.RawType.Replace("char(-1)", "char(MAX)");
                else
                    field.RawType = field.RawType.Replace("char(-1)", "char(" + (Int32.MaxValue / 2) + ")");
            }

            return base.FieldClause(field, onlyDefine);
        }

        protected override string GetFieldConstraints(IDataColumn field, Boolean onlyDefine)
        {
            // 非定义时（修改字段），主键字段没有约束
            if (!onlyDefine && field.PrimaryKey) return null;

            String str = base.GetFieldConstraints(field, onlyDefine);

            // 非定义时，自增字段没有约束
            if (onlyDefine && field.Identity) str = " IDENTITY(1,1)" + str;

            return str;
        }

        protected override string GetFormatParam(IDataColumn field, DataRow dr)
        {
            String str = base.GetFormatParam(field, dr);
            if (String.IsNullOrEmpty(str)) return str;

            // 这个主要来自于float，因为无法取得其精度
            if (str == "(0)") return null;
            return str;
        }

        protected override string GetFormatParamItem(IDataColumn field, DataRow dr, string item)
        {
            String pi = base.GetFormatParamItem(field, dr, item);
            if (field.DataType == typeof(String) && pi == "-1" && IsSQL2005) return "MAX";
            return pi;
        }

        protected override string GetFieldDefault(IDataColumn field, bool onlyDefine)
        {
            if (!onlyDefine) return null;

            return base.GetFieldDefault(field, onlyDefine);
        }
        #endregion

        #region 取得字段信息的SQL模版
        private String _SchemaSql = "";
        /// <summary>构架SQL</summary>
        public virtual String SchemaSql
        {
            get
            {
                if (String.IsNullOrEmpty(_SchemaSql))
                {
                    var sb = new StringBuilder();
                    sb.Append("SELECT ");
                    sb.Append("表名=d.name,");
                    sb.Append("字段序号=a.colorder,");
                    sb.Append("字段名=a.name,");
                    sb.Append("标识=case when COLUMNPROPERTY( a.id,a.name,'IsIdentity')=1 then Convert(Bit,1) else Convert(Bit,0) end,");
                    sb.Append("主键=case when exists(SELECT 1 FROM sysobjects where xtype='PK' and name in (");
                    sb.Append("SELECT name FROM sysindexes WHERE id = a.id AND indid in(");
                    sb.Append("SELECT indid FROM sysindexkeys WHERE id = a.id AND colid=a.colid");
                    sb.Append("))) then Convert(Bit,1) else Convert(Bit,0) end,");
                    sb.Append("类型=b.name,");
                    sb.Append("占用字节数=a.length,");
                    sb.Append("长度=COLUMNPROPERTY(a.id,a.name,'PRECISION'),");
                    sb.Append("小数位数=isnull(COLUMNPROPERTY(a.id,a.name,'Scale'),0),");
                    sb.Append("允许空=case when a.isnullable=1 then Convert(Bit,1)else Convert(Bit,0) end,");
                    sb.Append("默认值=isnull(e.text,''),");
                    sb.Append("字段说明=isnull(g.[value],'')");
                    sb.Append("FROM syscolumns a ");
                    sb.Append("left join systypes b on a.xtype=b.xusertype ");
                    sb.Append("inner join sysobjects d on a.id=d.id  and d.xtype='U' ");
                    sb.Append("left join syscomments e on a.cdefault=e.id ");
                    if (IsSQL2005)
                    {
                        sb.Append("left join sys.extended_properties g on a.id=g.major_id and a.colid=g.minor_id and g.name = 'MS_Description'  ");
                    }
                    else
                    {
                        sb.Append("left join sysproperties g on a.id=g.id and a.colid=g.smallid  ");
                    }
                    sb.Append("order by a.id,a.colorder");
                    _SchemaSql = sb.ToString();
                }
                return _SchemaSql;
            }
        }

        private String _IndexSql;
        public virtual String IndexSql
        {
            get
            {
                if (_IndexSql == null)
                {
                    if (IsSQL2005)
                        _IndexSql = "select ind.* from sys.indexes ind inner join sys.objects obj on ind.object_id = obj.object_id where obj.type='u'";
                    else
                        _IndexSql = "select IndexProperty(obj.id, ind.name,'IsUnique') as is_unique, ObjectProperty(object_id(ind.name),'IsPrimaryKey') as is_primary_key,ind.* from sysindexes ind inner join sysobjects obj on ind.id = obj.id where obj.type='u'";
                }
                return _IndexSql;
            }
        }

        private readonly String _DescriptionSql2000 = "select b.name n, a.value v from sysproperties a inner join sysobjects b on a.id=b.id where a.smallid=0";
        private readonly String _DescriptionSql2005 = "select b.name n, a.value v from sys.extended_properties a inner join sysobjects b on a.major_id=b.id and a.minor_id=0 and a.name = 'MS_Description'";
        /// <summary>取表说明SQL</summary>
        public virtual String DescriptionSql { get { return IsSQL2005 ? _DescriptionSql2005 : _DescriptionSql2000; } }
        #endregion

        #region 数据定义
        //public override object SetSchema(DDLSchema schema, params object[] values)
        //{
        //    IDbSession session = Database.CreateSession();

        //    Object obj = null;
        //    String dbname = String.Empty;
        //    String databaseName = String.Empty;
        //    switch (schema)
        //    {
        //        case DDLSchema.DropDatabase:
        //            databaseName = values == null || values.Length < 1 ? null : (String)values[0];
        //            if (String.IsNullOrEmpty(databaseName)) databaseName = session.DatabaseName;
        //            values = new Object[] { databaseName, values == null || values.Length < 2 ? null : values[1] };

        //            dbname = session.DatabaseName;
        //            session.DatabaseName = SystemDatabaseName;
        //            try
        //            {
        //                var sb = new StringBuilder();
        //                sb.AppendLine("use master");
        //                sb.AppendLine(";");
        //                sb.AppendLine("declare   @spid   varchar(20),@dbname   varchar(20)");
        //                sb.AppendLine("declare   #spid   cursor   for");
        //                sb.AppendFormat("select   spid=cast(spid   as   varchar(20))   from   master..sysprocesses   where   dbid=db_id('{0}')", dbname);
        //                sb.AppendLine();
        //                sb.AppendLine("open   #spid");
        //                sb.AppendLine("fetch   next   from   #spid   into   @spid");
        //                sb.AppendLine("while   @@fetch_status=0");
        //                sb.AppendLine("begin");
        //                sb.AppendLine("exec('kill   '+@spid)");
        //                sb.AppendLine("fetch   next   from   #spid   into   @spid");
        //                sb.AppendLine("end");
        //                sb.AppendLine("close   #spid");
        //                sb.AppendLine("deallocate   #spid");

        //                Int32 count = 0;
        //                try { count = session.Execute(sb.ToString()); }
        //                catch { }
        //                obj = session.Execute(String.Format("Drop Database {0}", FormatName(dbname))) > 0;
        //                //sb.AppendFormat("Drop Database [{0}]", dbname);
        //            }
        //            finally { session.DatabaseName = dbname; }
        //            return obj;
        //        case DDLSchema.TableExist:
        //            return TableExist((IDataTable)values[0]);
        //        default:
        //            break;
        //    }
        //    return base.SetSchema(schema, values);
        //}

        public override string CreateDatabaseSQL(string dbname, string file)
        {
            String dataPath = (Database as SqlServer).DataPath;

            if (String.IsNullOrEmpty(file))
            {
                if (String.IsNullOrEmpty(dataPath)) return String.Format("CREATE DATABASE [{0}]", FormatName(dbname));

                file = dbname + ".mdf";
            }

            String logfile = String.Empty;

            if (!Path.IsPathRooted(file))
            {
                if (!String.IsNullOrEmpty(dataPath)) file = Path.Combine(dataPath, file);

                if (!Path.IsPathRooted(file)) file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
            }
            if (String.IsNullOrEmpty(Path.GetExtension(file))) file += ".mdf";
            file = new FileInfo(file).FullName;

            logfile = Path.ChangeExtension(file, ".ldf");
            logfile = new FileInfo(logfile).FullName;

            var dir = Path.GetDirectoryName(file);
            if (!String.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var sb = new StringBuilder();

            sb.AppendFormat("CREATE DATABASE {0} ON  PRIMARY", FormatName(dbname));
            sb.AppendLine();
            sb.AppendFormat(@"( NAME = N'{0}', FILENAME = N'{1}', SIZE = 10 , MAXSIZE = UNLIMITED, FILEGROWTH = 10%)", dbname, file);
            sb.AppendLine();
            sb.Append("LOG ON ");
            sb.AppendLine();
            sb.AppendFormat(@"( NAME = N'{0}_Log', FILENAME = N'{1}', SIZE = 10 , MAXSIZE = UNLIMITED, FILEGROWTH = 10%)", dbname, logfile);
            sb.AppendLine();

            return sb.ToString();
        }

        public override string DatabaseExistSQL(string dbname)
        {
            return String.Format("SELECT * FROM sysdatabases WHERE name = N'{0}'", dbname);
        }

        /// <summary>使用数据架构确定数据库是否存在，因为使用系统视图可能没有权限</summary>
        /// <param name="dbname"></param>
        /// <returns></returns>
        protected override Boolean DatabaseExist(string dbname)
        {
            var dt = GetSchema(_.Databases, new String[] { dbname });
            return dt != null && dt.Rows != null && dt.Rows.Count > 0;
        }

        protected override Boolean DropDatabase(string databaseName)
        {
            //return base.DropDatabase(databaseName);

            // SQL语句片段，断开该数据库所有链接
            var sb = new StringBuilder();
            sb.AppendLine("use master");
            sb.AppendLine(";");
            sb.AppendLine("declare   @spid   varchar(20),@dbname   varchar(20)");
            sb.AppendLine("declare   #spid   cursor   for");
            sb.AppendFormat("select   spid=cast(spid   as   varchar(20))   from   master..sysprocesses   where   dbid=db_id('{0}')", databaseName);
            sb.AppendLine();
            sb.AppendLine("open   #spid");
            sb.AppendLine("fetch   next   from   #spid   into   @spid");
            sb.AppendLine("while   @@fetch_status=0");
            sb.AppendLine("begin");
            sb.AppendLine("exec('kill   '+@spid)");
            sb.AppendLine("fetch   next   from   #spid   into   @spid");
            sb.AppendLine("end");
            sb.AppendLine("close   #spid");
            sb.AppendLine("deallocate   #spid");

            var count = 0;
            var session = Database.CreateSession();
            try { count = session.Execute(sb.ToString()); }
            catch { }
            return session.Execute(String.Format("Drop Database {0}", FormatName(databaseName))) > 0;
        }

        public override string TableExistSQL(String tableName)
        {
            if (IsSQL2005)
                return String.Format("select * from sysobjects where xtype='U' and name='{0}'", tableName);
            else
                return String.Format("SELECT * FROM sysobjects WHERE id = OBJECT_ID(N'[dbo].{0}') AND OBJECTPROPERTY(id, N'IsUserTable') = 1", FormatName(tableName));
        }

        /// <summary>使用数据架构确定数据表是否存在，因为使用系统视图可能没有权限</summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public Boolean TableExist(IDataTable table)
        {
            var dt = GetSchema(_.Tables, new String[] { null, null, table.TableName, null });
            return dt != null && dt.Rows != null && dt.Rows.Count > 0;
        }

        protected override string RenameTable(string tableName, string tempTableName)
        {
            if (Version.Major >= 8)
                return String.Format("EXECUTE sp_rename N'{0}', N'{1}', 'OBJECT' ", tableName, tempTableName);
            else
                return base.RenameTable(tableName, tempTableName);
        }

        protected override string ReBuildTable(IDataTable entitytable, IDataTable dbtable)
        {
            //return String.Format("SET IDENTITY_INSERT {1} ON;{0};SET IDENTITY_INSERT {1} OFF", base.ReBuildTable(entitytable, dbtable), Database.FormatName(entitytable.TableName));
            var sql = base.ReBuildTable(entitytable, dbtable);
            if (String.IsNullOrEmpty(sql)) return sql;

            // 特殊处理带标识列的表，需要增加SET IDENTITY_INSERT
            if (!entitytable.Columns.Any(e => e.Identity)) return sql;

            var tableName = Database.FormatName(entitytable.TableName);
            var ss = sql.Split("; " + Environment.NewLine);
            //var list = new List<String>(ss);
            //list.Insert(1, String.Format("SET IDENTITY_INSERT {0} ON", tableName));
            //list.Insert(list.Count - 1, String.Format("SET IDENTITY_INSERT {0} OFF", tableName));
            //return String.Join("; " + Environment.NewLine, list.ToArray());
            for (int i = 0; i < ss.Length; i++)
            {
                if (ss[i].StartsWithIgnoreCase("Insert Into"))
                {
                    ss[i] = String.Format("SET IDENTITY_INSERT {1} ON;{0};SET IDENTITY_INSERT {1} OFF", ss[i], tableName);
                    break;
                }
            }
            return String.Join("; " + Environment.NewLine, ss);
        }

        public override string AddTableDescriptionSQL(IDataTable table)
        {
            return String.Format("EXEC dbo.sp_addextendedproperty @name=N'MS_Description', @value=N'{1}' , @level0type=N'{2}',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{0}'", table.TableName, table.Description, level0type);
        }

        public override string DropTableDescriptionSQL(IDataTable table)
        {
            return String.Format("EXEC dbo.sp_dropextendedproperty @name=N'MS_Description', @level0type=N'{1}',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{0}'", table.TableName, level0type);
        }

        public override string AddColumnSQL(IDataColumn field)
        {
            return String.Format("Alter Table {0} Add {1}", FormatName(field.Table.TableName), FieldClause(field, true));
        }

        public override string AlterColumnSQL(IDataColumn field, IDataColumn oldfield)
        {
            // 创建为自增，重建表
            if (field.Identity && !oldfield.Identity)
            {
                //return DropColumnSQL(oldfield) + ";" + Environment.NewLine + AddColumnSQL(field);
                return ReBuildTable(field.Table, oldfield.Table);
            }
            // 类型改变，必须重建表
            if (IsColumnTypeChanged(field, oldfield)) return ReBuildTable(field.Table, oldfield.Table);

            var sql = String.Format("Alter Table {0} Alter Column {1}", FormatName(field.Table.TableName), FieldClause(field, false));
            var pk = DeletePrimaryKeySQL(field);
            if (field.PrimaryKey)
            {
                // 如果没有主键删除脚本，表明没有主键
                //if (String.IsNullOrEmpty(pk))
                if (!oldfield.PrimaryKey)
                {
                    // 增加主键约束
                    pk = String.Format("Alter Table {0} ADD CONSTRAINT PK_{0} PRIMARY KEY {2}({1}) ON [PRIMARY]", FormatName(field.Table.TableName), FormatName(field.ColumnName), field.Identity ? "CLUSTERED" : "");
                    sql += ";" + Environment.NewLine + pk;
                }
            }
            else
            {
                // 字段声明没有主键，但是主键实际存在，则删除主键
                //if (!String.IsNullOrEmpty(pk))
                if (oldfield.PrimaryKey)
                {
                    sql += ";" + Environment.NewLine + pk;
                }
            }

            // 需要提前删除相关默认值
            if (oldfield.Default != null)
            {
                var df = DropDefaultSQL(oldfield);
                if (!String.IsNullOrEmpty(df))
                {
                    sql = df + ";" + Environment.NewLine + sql;

                    // 如果还有默认值，加上
                    if (field.Default != null)
                    {
                        df = AddDefaultSQLWithNoCheck(field);
                        if (!String.IsNullOrEmpty(df)) sql += ";" + Environment.NewLine + df;
                    }
                }
            }
            // 需要提前删除相关索引
            foreach (var di in oldfield.Table.Indexes)
            {
                // 如果包含该字段
                if (di.Columns.Contains(oldfield.ColumnName, StringComparer.OrdinalIgnoreCase))
                {
                    var dis = DropIndexSQL(di);
                    if (!String.IsNullOrEmpty(dis)) sql = dis + ";" + Environment.NewLine + sql;
                }
            }
            // 如果还有索引，则加上
            foreach (var di in field.Table.Indexes)
            {
                // 如果包含该字段
                if (di.Columns.Contains(field.ColumnName, StringComparer.OrdinalIgnoreCase))
                {
                    var cis = CreateIndexSQL(di);
                    if (!String.IsNullOrEmpty(cis)) sql += ";" + Environment.NewLine + cis;
                }
            }

            return sql;
        }

        public override string DropIndexSQL(IDataIndex index)
        {
            return String.Format("Drop Index {1}.{0}", FormatName(index.Name), FormatName(index.Table.TableName));
        }

        public override string DropColumnSQL(IDataColumn field)
        {
            //删除默认值
            String sql = DropDefaultSQL(field);
            if (!String.IsNullOrEmpty(sql)) sql += ";" + Environment.NewLine;

            //删除主键
            String sql2 = DeletePrimaryKeySQL(field);
            if (!String.IsNullOrEmpty(sql2)) sql += sql2 + ";" + Environment.NewLine;

            sql += base.DropColumnSQL(field);
            return sql;
        }

        public override string AddColumnDescriptionSQL(IDataColumn field)
        {
            String sql = String.Format("EXEC dbo.sp_addextendedproperty @name=N'MS_Description', @value=N'{1}' , @level0type=N'{3}',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{0}', @level2type=N'COLUMN',@level2name=N'{2}'", field.Table.TableName, field.Description, field.ColumnName, level0type);
            return sql;
        }

        public override string DropColumnDescriptionSQL(IDataColumn field)
        {
            return String.Format("EXEC dbo.sp_dropextendedproperty @name=N'MS_Description', @level0type=N'{2}',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{0}', @level2type=N'COLUMN',@level2name=N'{1}'", field.Table.TableName, field.ColumnName, level0type);
        }

        public override string AddDefaultSQL(IDataColumn field)
        {
            var sql = DropDefaultSQL(field);
            if (!String.IsNullOrEmpty(sql)) sql += ";" + Environment.NewLine;

            var sql2 = AddDefaultSQLWithNoCheck(field);
            if (String.IsNullOrEmpty(sql2))
                return sql;
            else
                return sql + ";" + Environment.NewLine + sql2;
        }

        string AddDefaultSQLWithNoCheck(IDataColumn field)
        {
            var tc = Type.GetTypeCode(field.DataType);

            var dv = field.Default;
            if (CheckAndGetDefault(field, ref dv))
            {
                //if (String.IsNullOrEmpty(dv)) return null;
                if (dv == null) return null;
                return String.Format("Alter Table {0} Add CONSTRAINT DF_{0}_{1} DEFAULT {2} FOR {1}", field.Table.TableName, field.ColumnName, dv);
            }

            if (tc == TypeCode.String)
                return String.Format("Alter Table {0} Add CONSTRAINT DF_{0}_{1} DEFAULT N'{2}' FOR {1}", field.Table.TableName, field.ColumnName, field.Default);
            //else if (tc == TypeCode.DateTime)
            //{
            //    String dv = CheckAndGetDefault(field, field.Default);
            //    sql += String.Format("Alter Table {0} Add CONSTRAINT DF_{0}_{1} DEFAULT {2} FOR {1}", field.Table.Name, field.Name, dv);
            //}
            else
                return String.Format("Alter Table {0} Add CONSTRAINT DF_{0}_{1} DEFAULT {2} FOR {1}", field.Table.TableName, field.ColumnName, field.Default);
        }

        public override string DropDefaultSQL(IDataColumn field)
        {
            //if (String.IsNullOrEmpty(field.Default)) return String.Empty;
            // 默认值有可能是空字符串
            if (field.Default == null) return String.Empty;

            String sql = null;
            if (IsSQL2005)
                sql = String.Format("select b.name from sys.tables a inner join sys.default_constraints b on a.object_id=b.parent_object_id inner join sys.columns c on a.object_id=c.object_id and b.parent_column_id=c.column_id where a.name='{0}' and c.name='{1}'", field.Table.TableName, field.ColumnName);
            else
                sql = String.Format("select b.name from syscolumns a inner join sysobjects b on a.cdefault=b.id inner join sysobjects c on a.id=c.id where a.name='{1}' and c.name='{0}' and b.xtype='D'", field.Table.TableName, field.ColumnName);

            var ds = Database.CreateSession().Query(sql);
            if (ds == null || ds.Tables == null || ds.Tables[0].Rows.Count < 1) return null;

            var sb = new StringBuilder();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                var name = dr[0].ToString();
                if (sb.Length > 0) sb.AppendLine(";");
                sb.AppendFormat("Alter Table {0} Drop CONSTRAINT {1}", FormatName(field.Table.TableName), name);
            }
            return sb.ToString();
        }

        String DeletePrimaryKeySQL(IDataColumn field)
        {
            if (!field.PrimaryKey) return String.Empty;

            if (field.Table.Indexes == null || field.Table.Indexes.Count < 1) return String.Empty;

            IDataIndex di = null;
            foreach (IDataIndex item in field.Table.Indexes)
            {
                if (Array.IndexOf(item.Columns, field.ColumnName) >= 0)
                {
                    di = item;
                    break;
                }
            }
            if (di == null) return String.Empty;

            return String.Format("Alter Table {0} Drop CONSTRAINT {1}", FormatName(field.Table.TableName), di.Name);
        }

        public override String DropDatabaseSQL(String dbname)
        {
            var sb = new StringBuilder();
            sb.AppendLine("use master");
            sb.AppendLine(";");
            sb.AppendLine("declare   @spid   varchar(20),@dbname   varchar(20)");
            sb.AppendLine("declare   #spid   cursor   for");
            sb.AppendFormat("select   spid=cast(spid   as   varchar(20))   from   master..sysprocesses   where   dbid=db_id('{0}')", dbname);
            sb.AppendLine();
            sb.AppendLine("open   #spid");
            sb.AppendLine("fetch   next   from   #spid   into   @spid");
            sb.AppendLine("while   @@fetch_status=0");
            sb.AppendLine("begin");
            sb.AppendLine("exec('kill   '+@spid)");
            sb.AppendLine("fetch   next   from   #spid   into   @spid");
            sb.AppendLine("end");
            sb.AppendLine("close   #spid");
            sb.AppendLine("deallocate   #spid");
            sb.AppendLine(";");
            sb.AppendFormat("Drop Database {0}", FormatName(dbname));
            return sb.ToString();
        }
        #endregion

        #region 辅助函数
        /// <summary>除去字符串两端成对出现的符号</summary>
        /// <param name="str"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static String Trim(String str, String prefix, String suffix)
        {
            while (!String.IsNullOrEmpty(str))
            {
                if (!str.StartsWith(prefix)) return str;
                if (!str.EndsWith(suffix)) return str;

                str = str.Substring(prefix.Length, str.Length - suffix.Length - prefix.Length);
            }
            return str;
        }
        #endregion
    }
}