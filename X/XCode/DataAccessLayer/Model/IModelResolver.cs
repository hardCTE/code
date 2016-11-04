﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using XCode.Model;

namespace XCode.DataAccessLayer
{
    /// <summary>模型解析器接口。解决名称大小写、去前缀、关键字等多个问题</summary>
    public interface IModelResolver
    {
        #region 名称处理
        ///// <summary>获取别名。过滤特殊符号，过滤_之类的前缀。另外，避免一个表中的字段别名重名</summary>
        ///// <param name="dc"></param>
        ///// <returns></returns>
        //String GetName(IDataColumn dc);

        ///// <summary>获取别名。过滤特殊符号，过滤_之类的前缀。</summary>
        ///// <param name="name">名称</param>
        ///// <returns></returns>
        //String GetName(String name);

        /// <summary>根据字段名等信息计算索引的名称</summary>
        /// <param name="di"></param>
        /// <returns></returns>
        String GetName(IDataIndex di);

        ///// <summary>去除前缀。默认去除第一个_前面部分，去除tbl和table前缀</summary>
        ///// <param name="name">名称</param>
        ///// <returns></returns>
        //String CutPrefix(String name);

        ///// <summary>自动处理大小写</summary>
        ///// <param name="name">名称</param>
        ///// <returns></returns>
        //String FixWord(String name);

        /// <summary>获取显示名，如果描述不存在，则使用名称，否则使用描述前面部分，句号（中英文皆可）、换行分隔</summary>
        /// <param name="name">名称</param>
        /// <param name="description"></param>
        /// <returns></returns>
        String GetDisplayName(String name, String description);
        #endregion

        #region 模型处理
        /// <summary>连接两个表。
        /// 实际上是猜测它们之间的关系，根据一个字段名是否等于另一个表的表名加某个字段名来判断是否存在关系。</summary>
        /// <param name="table"></param>
        /// <param name="rtable"></param>
        IDataTable Connect(IDataTable table, IDataTable rtable);

        /// <summary>猜测表间关系</summary>
        /// <param name="table"></param>
        /// <param name="rtable"></param>
        /// <param name="rname"></param>
        /// <param name="column"></param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        Boolean GuessRelation(IDataTable table, IDataTable rtable, String rname, IDataColumn column, String name);

        /// <summary>修正数据</summary>
        /// <param name="table"></param>
        IDataTable Fix(IDataTable table);

        /// <summary>修正数据列</summary>
        /// <param name="column"></param>
        IDataColumn Fix(IDataColumn column);
        #endregion

        #region 设置
        ///// <summary>是否ID作为id的格式化，否则使用原名。默认使用ID</summary>
        //Boolean UseID { get; set; }

        ///// <summary>是否自动去除前缀。默认启用</summary>
        //Boolean AutoCutPrefix { get; set; }

        ///// <summary>是否自动去除字段前面的表名。默认启用</summary>
        //Boolean AutoCutTableName { get; set; }

        ///// <summary>是否自动纠正大小写。默认启用</summary>
        //Boolean AutoFixWord { get; set; }

        ///// <summary>要过滤的前缀</summary>
        //String[] FilterPrefixs { get; set; }
        #endregion
    }

    /// <summary>模型解析器。解决名称大小写、去前缀、关键字等多个问题</summary>
    public class ModelResolver : IModelResolver
    {
        #region 名称处理
        ///// <summary>获取别名。过滤特殊符号，过滤_之类的前缀。另外，避免一个表中的字段别名重名</summary>
        ///// <param name="dc"></param>
        ///// <returns></returns>
        //public virtual String GetName(IDataColumn dc)
        //{
        //    var name = dc.ColumnName;
        //    // 对于自增字段，如果强制使用ID，并且字段名以ID结尾，则直接取用ID
        //    if (dc.Identity && UseID && name.EndsWithIgnoreCase("ID")) return "ID";

        //    #region 先去掉表前缀
        //    var dt = dc.Table;
        //    if (dt != null && AutoCutTableName)
        //    {
        //        // 要去掉的前缀集合
        //        var pfs = new HashSet<String>(StringComparer.OrdinalIgnoreCase);
        //        // 表名、类名，已经包含的下划线前缀，一律过滤
        //        foreach (var item in new String[] { dt.TableName, dt.Name })
        //        {
        //            if (!item.IsNullOrWhiteSpace())
        //            {
        //                pfs.Add(item);
        //                // 如果包括下划线，再分割
        //                if (item.Contains("_"))
        //                {
        //                    foreach (var elm in item.Split("_"))
        //                    {
        //                        if (elm != null && elm.Length >= 2 && !pfs.Contains(elm)) pfs.Add(elm);
        //                    }
        //                }
        //            }
        //        }

        //        foreach (var item in pfs)
        //        {
        //            if (name.Length != item.Length) name = name.TrimStart(item);
        //        }
        //        name = name.TrimStart('_');
        //    }
        //    #endregion

        //    name = GetName(name);
        //    if (dt != null)
        //    {
        //        var lastname = name;
        //        var index = 0;
        //        var cs = dt.Columns;
        //        for (int i = 0; i < cs.Count; i++)
        //        {
        //            var item = cs[i];
        //            if (item != dc && item.ColumnName != dc.ColumnName)
        //            {
        //                // 对于小于当前的采用别名，对于大于当前的，采用字段名，保证同名有优先级
        //                if (lastname.EqualIgnoreCase(item.ID < dc.ID ? item.Name : item.ColumnName))
        //                {
        //                    lastname = name + ++index;
        //                    // 从头开始
        //                    i = -1;
        //                }
        //            }
        //        }
        //        name = lastname;
        //    }
        //    return name;
        //}

        /// <summary>获取别名。过滤特殊符号，过滤_之类的前缀。</summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public virtual String GetName(String name)
        {
            if (String.IsNullOrEmpty(name)) return name;

            name = name.Replace("$", null);
            name = name.Replace("(", null);
            name = name.Replace(")", null);
            name = name.Replace("（", null);
            name = name.Replace("）", null);
            name = name.Replace(" ", null);
            name = name.Replace("　", null);
            name = name.Replace("/", "_");
            name = name.Replace("\\", "_");

            //    //if (name[0] == '_' && (name.Length == 1 || Char.IsLetter(name[1]))) name = name.Substring(1);
            //    // 去除前缀不够严谨，导致出现数字开头的名称，那是非法的
            //    if (name[0] == '_')
            //    {
            //        var str = name.Substring(1);
            //        if (!IsKeyWord(str)) name = str;
            //    }

            //    // 很多时候，这个别名就是表名
            //    name = CutPrefix(name);
            //    if (AutoFixWord) name = FixWord(name);
            //    if (name[0] == '_')
            //    {
            //        var str = name.Substring(1);
            //        if (!IsKeyWord(str)) name = str;
            //    }

            //    //关键字加后缀
            //    //2016.02.12 @宁波-小董，测试发现下面代码在Oracle环境中产生死循环，修改为if
            //    //while (IsKeyWord(name)) name += "_";
            //    if (IsKeyWord(name)) name += "_";

            return name;
        }

        /// <summary>根据字段名等信息计算索引的名称</summary>
        /// <param name="di"></param>
        /// <returns></returns>
        public virtual String GetName(IDataIndex di)
        {
            if (di.Columns == null || di.Columns.Length < 1) return null;

            var sb = new StringBuilder();
            if (di.PrimaryKey)
                sb.Append("PK");
            else if (di.Unique)
                sb.Append("IU");
            else
                sb.Append("IX");

            if (di.Table != null)
            {
                sb.Append("_");
                sb.Append(di.Table.TableName);
            }
            for (int i = 0; i < di.Columns.Length; i++)
            {
                sb.Append("_");
                sb.Append(di.Columns[i]);
            }
            return sb.ToString();
        }

        ///// <summary>去除前缀。默认去除第一个_前面部分，去除tbl和table前缀</summary>
        ///// <param name="name">名称</param>
        ///// <returns></returns>
        //public virtual String CutPrefix(String name)
        //{
        //    if (String.IsNullOrEmpty(name)) return null;

        //    var old = name;
        //    foreach (var pfx in FilterPrefixs)
        //    {
        //        if (name.StartsWithIgnoreCase(pfx) && name.Length != pfx.Length)
        //        {
        //            var str = name.Substring(pfx.Length);
        //            if (!IsKeyWord(str)) name = str;
        //        }
        //        else if (name.EndsWithIgnoreCase(pfx) && name.Length != pfx.Length)
        //        {
        //            var str = name.Substring(0, name.Length - pfx.Length);
        //            if (!IsKeyWord(str)) name = str;
        //        }
        //    }

        //    // 自动去掉前缀，如果上面有过滤，这里是不能去除的，否则可能过度
        //    if (AutoCutPrefix && name == old)
        //    {
        //        Int32 n = name.IndexOf("_");
        //        // _后至少要有2个字母，并且后一个不能是_
        //        if (n >= 0 && n < name.Length - 2 && name[n + 1] != '_')
        //        {
        //            var str = name.Substring(n + 1);
        //            if (!IsKeyWord(str)) name = str;
        //        }
        //    }

        //    return name;
        //}

        ///// <summary>自动处理大小写</summary>
        ///// <param name="name">名称</param>
        ///// <returns></returns>
        //public virtual String FixWord(String name)
        //{
        //    if (String.IsNullOrEmpty(name)) return null;

        //    if (UseID && name.EqualIgnoreCase("ID")) return "ID";

        //    if (name.Length <= 2) return name;

        //    // 如果包括下划线，特殊处理
        //    if (name.Contains("_"))
        //    {
        //        var ss = name.Split('_');
        //        for (int i = 0; i < ss.Length; i++)
        //        {
        //            if (!ss[i].IsNullOrWhiteSpace()) ss[i] = FixWord(ss[i]);
        //        }
        //        return String.Join("_", ss);
        //    }

        //    Int32 lowerCount = 0;
        //    Int32 upperCount = 0;
        //    foreach (var item in name)
        //    {
        //        if (item >= 'a' && item <= 'z')
        //            lowerCount++;
        //        else if (item >= 'A' && item <= 'Z')
        //            upperCount++;
        //    }

        //    //没有或者只有一个小写字母的，需要修正
        //    //没有大写的，也要修正
        //    if (lowerCount <= 1 || upperCount < 1)
        //    {
        //        name = name.ToLower();
        //        Char c = name[0];
        //        if (c >= 'a' && c <= 'z') c = (Char)(c - 'a' + 'A');
        //        name = c + name.Substring(1);
        //    }
        //    else
        //    {
        //        Char c = name[0];
        //        if (c >= 'a' && c <= 'z')
        //        {
        //            c = (Char)(c - 'a' + 'A');
        //            name = c + name.Substring(1);
        //        }
        //    }

        //    //处理Is开头的，第三个字母要大写
        //    if (name.StartsWith("Is") && name.Length >= 3)
        //    {
        //        Char c = name[2];
        //        if (c >= 'a' && c <= 'z')
        //        {
        //            c = (Char)(c - 'a' + 'A');
        //            name = name.Substring(0, 2) + c + name.Substring(3);
        //        }
        //    }

        //    return name;
        //}

        ///// <summary>代码生成器</summary>
        //private static CSharpCodeProvider _CG = new CSharpCodeProvider();

        ///// <summary>是否关键字</summary>
        ///// <param name="name">名称</param>
        ///// <returns></returns>
        //static Boolean IsKeyWord(String name)
        //{
        //    if (String.IsNullOrEmpty(name)) return false;

        //    // 特殊处理item，还有NewLife和System这两个最重要的命名空间
        //    if (name.EqualIgnoreCase("item", "System", "NewLife")) return true;

        //    // 只要有大写字母，就不是关键字
        //    if (name.Any(c => c >= 'A' && c <= 'Z')) return false;

        //    // 只是判断是否合法变量，而不是判断是否真的关键字
        //    return !_CG.IsValidIdentifier(name);
        //}

        /// <summary>获取显示名，如果描述不存在，则使用名称，否则使用描述前面部分，句号（中英文皆可）、换行分隔</summary>
        /// <param name="name">名称</param>
        /// <param name="description"></param>
        /// <returns></returns>
        public virtual String GetDisplayName(String name, String description)
        {
            if (String.IsNullOrEmpty(description)) return name;

            name = description.Trim();
            var p = name.IndexOfAny(new Char[] { '.', '。', '\r', '\n' });
            // p=0表示符号在第一位，不考虑
            if (p > 0) name = name.Substring(0, p).Trim();

            name = name.Replace("$", null);
            name = name.Replace("(", null);
            name = name.Replace(")", null);
            name = name.Replace("（", null);
            name = name.Replace("）", null);
            name = name.Replace(" ", null);
            name = name.Replace("　", null);
            name = name.Replace("/", "_");
            name = name.Replace("\\", "_");
            if (name[0] == '_') name = name.Substring(1);

            return name;
        }
        #endregion

        #region 模型处理
        /// <summary>连接两个表。
        /// 实际上是猜测它们之间的关系，根据一个字段名是否等于另一个表的表名加某个字段名来判断是否存在关系。</summary>
        /// <param name="table"></param>
        /// <param name="rtable"></param>
        public virtual IDataTable Connect(IDataTable table, IDataTable rtable)
        {
            foreach (var dc in table.Columns)
            {
                if (dc.PrimaryKey || dc.Identity) continue;

                if (GuessRelation(table, rtable, rtable.TableName, dc, dc.ColumnName)) continue;
                if (!dc.ColumnName.EqualIgnoreCase(dc.Name))
                {
                    if (GuessRelation(table, rtable, rtable.TableName, dc, dc.Name)) continue;
                }

                if (rtable.TableName.EqualIgnoreCase(rtable.Name)) continue;

                // 如果表2的别名和名称不同，还要继续
                if (GuessRelation(table, rtable, rtable.Name, dc, dc.ColumnName)) continue;
                if (!dc.ColumnName.EqualIgnoreCase(dc.Name))
                {
                    if (GuessRelation(table, rtable, rtable.Name, dc, dc.Name)) continue;
                }
            }

            return table;
        }

        /// <summary>猜测表间关系</summary>
        /// <param name="table"></param>
        /// <param name="rtable"></param>
        /// <param name="rname"></param>
        /// <param name="column"></param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public virtual Boolean GuessRelation(IDataTable table, IDataTable rtable, String rname, IDataColumn column, String name)
        {
            if (table == null || rtable == null || rname == null || column == null || name == null) return false;
            if (name.Length <= rtable.TableName.Length || !name.StartsWithIgnoreCase(rtable.TableName)) return false;

            var key = name.Substring(rtable.TableName.Length);
            var dc = rtable.GetColumn(key);
            // 猜测两表关联关系时，两个字段的类型也必须一致
            if (dc == null || dc.DataType != column.DataType) return false;

            // 建立关系
            var dr = table.CreateRelation();
            dr.Column = column.ColumnName;
            dr.RelationTable = rtable.TableName;
            dr.RelationColumn = dc.ColumnName;
            // 表关系这里一般是多对一，比如管理员的RoleID=>Role+Role.ID，对于索引来说，不是唯一的
            dr.Unique = false;
            // 当然，如果这个字段column有唯一索引，那么，这里也是唯一的。这就是典型的一对一
            if (column.PrimaryKey || column.Identity)
                dr.Unique = true;
            else
            {
                var di = table.GetIndex(column.ColumnName);
                if (di != null && di.Unique) dr.Unique = true;
            }

            dr.Computed = true;
            if (table.GetRelation(dr) == null) table.Relations.Add(dr);

            // 给另一方建立关系
            if (rtable.GetRelation(dc.ColumnName, table.TableName, column.ColumnName) != null) return true;

            dr = rtable.CreateRelation();
            dr.Column = dc.ColumnName;
            dr.RelationTable = table.TableName;
            dr.RelationColumn = column.ColumnName;
            // 那么这里就是唯一的啦
            dr.Unique = true;
            // 当然，如果字段dc不是主键，也没有唯一索引，那么关系就不是唯一的。这就是典型的多对多
            if (!dc.PrimaryKey && !dc.Identity)
            {
                var di = rtable.GetIndex(dc.ColumnName);
                // 没有索引，或者索引不是唯一的
                if (di == null || !di.Unique) dr.Unique = false;
            }

            dr.Computed = true;
            if (rtable.GetRelation(dr) == null) rtable.Relations.Add(dr);

            return true;
        }

        /// <summary>修正数据</summary>
        /// <param name="table"></param>
        public virtual IDataTable Fix(IDataTable table)
        {
            if (table.Name.IsNullOrEmpty()) table.Name = GetName(table.TableName);

            // 根据单字段索引修正对应的关系
            FixRelationBySingleIndex(table);

            // 给所有关系字段建立索引
            CreateIndexForRelation(table);

            // 从索引中修正主键
            FixPrimaryByIndex(table);

            // 最后修复主键
            if (table.PrimaryKeys.Length < 1)
            {
                // 自增作为主键，没办法，如果没有主键，整个实体层都会面临大问题！
                var dc = table.Columns.FirstOrDefault(c => c.Identity);
                if (dc != null) dc.PrimaryKey = true;
            }

            // 给非主键的自增字段建立唯一索引
            CreateUniqueIndexForIdentity(table);

            // 索引应该具有跟字段一样的唯一和主键约束
            FixIndex(table);

            //// 修正可能错误的别名
            //foreach (var dc in table.Columns)
            //{
            //    dc.Fix();
            //}
            foreach (var di in table.Indexes)
            {
                di.Fix();
            }

            // 修正可能的主字段
            if (!table.Columns.Any(e => e.Master))
            {
                var f = table.Columns.FirstOrDefault(e => e.Name.EqualIgnoreCase("Name", "Title"));
                if (f != null) f.Master = true;
            }

            return table;
        }
        /// <summary>修正数据列</summary>
        /// <param name="column"></param>
        public virtual IDataColumn Fix(IDataColumn column)
        {
            if (column.Name.IsNullOrEmpty()) column.Name = GetName(column.ColumnName);

            return column;
        }
        /// <summary>根据单字段索引修正对应的关系</summary>
        /// <param name="table"></param>
        protected virtual void FixRelationBySingleIndex(IDataTable table)
        {
            // 给所有单字段索引建立关系，特别是一对一关系
            foreach (var item in table.Indexes)
            {
                if (item.Columns == null || item.Columns.Length != 1) continue;

                var dr = table.GetRelation(item.Columns[0]);
                if (dr == null) continue;

                dr.Unique = item.Unique;
                // 跟关系有关联的索引
                dr.Computed = item.Computed;
            }
        }

        /// <summary>给所有关系字段建立索引</summary>
        /// <param name="table"></param>
        protected virtual void CreateIndexForRelation(IDataTable table)
        {
            foreach (var dr in table.Relations)
            {
                // 跳过主键
                var dc = table.GetColumn(dr.Column);
                if (dc == null || dc.PrimaryKey) continue;

                if (table.GetIndex(dr.Column) == null)
                {
                    var di = table.CreateIndex();
                    di.Columns = new String[] { dr.Column };
                    // 这两个的关系，唯一性
                    di.Unique = dr.Unique;
                    di.Computed = true;
                    table.Indexes.Add(di);
                }
            }
        }

        /// <summary>从索引中修正主键</summary>
        /// <param name="table"></param>
        protected virtual void FixPrimaryByIndex(IDataTable table)
        {
            var pks = table.PrimaryKeys;
            if (pks == null || pks.Length < 1)
            {
                // 在索引中找唯一索引作为主键
                var di = table.Indexes.FirstOrDefault(e => e.PrimaryKey && e.Columns.Length == 1);
                // 在索引中找唯一索引作为主键
                if (di == null) di = table.Indexes.FirstOrDefault(e => e.Unique && e.Columns.Length == 1);
                // 如果还没有主键，把第一个索引作为主键
                if (di == null) di = table.Indexes.FirstOrDefault(e => e.Columns.Length == 1);

                if (di != null)
                {
                    var pks2 = table.GetColumns(di.Columns);
                    if (pks2.Length > 0) Array.ForEach<IDataColumn>(pks2, dc => dc.PrimaryKey = true);
                }
            }
        }

        /// <summary>给非主键的自增字段建立唯一索引</summary>
        /// <param name="table"></param>
        protected virtual void CreateUniqueIndexForIdentity(IDataTable table)
        {
            foreach (var dc in table.Columns)
            {
                if (dc.Identity && !dc.PrimaryKey)
                {
                    var di = table.GetIndex(dc.ColumnName);
                    if (di == null)
                    {
                        di = table.CreateIndex();
                        di.Columns = new String[] { dc.ColumnName };
                        di.Computed = true;
                    }
                    // 不管是不是原来有的索引，都要唯一
                    di.Unique = true;
                }
            }
        }

        /// <summary>索引应该具有跟字段一样的唯一和主键约束</summary>
        /// <param name="table"></param>
        protected virtual void FixIndex(IDataTable table)
        {
            // 主要针对MSSQL2000
            foreach (var di in table.Indexes)
            {
                if (di.Columns == null) continue;

                var dcs = table.GetColumns(di.Columns);
                if (dcs == null || dcs.Length <= 0) continue;

                if (!di.Unique) di.Unique = dcs.All(dc => dc.Identity);
                if (!di.PrimaryKey) di.PrimaryKey = dcs.All(dc => dc.PrimaryKey);
            }
        }

        ///// <summary>修正数据列</summary>
        ///// <param name="column"></param>
        //public virtual IDataColumn Fix(IDataColumn column)
        //{
        //    if (column.Name.IsNullOrEmpty()) column.Name = GetName(column);

        //    return column;
        //}
        #endregion

        #region 静态实例
        /// <summary>当前名称解析器</summary>
        public static IModelResolver Current { get { return XCodeService.Container.ResolveInstance<IModelResolver>(); } }
        #endregion

        #region 设置
        ///// <summary>是否ID作为id的格式化，否则使用原名。默认使用ID</summary>
        //public Boolean UseID { get; set; }

        ///// <summary>是否自动去除前缀。默认启用</summary>
        //public Boolean AutoCutPrefix { get; set; }

        ///// <summary>是否自动去除字段前面的表名。默认启用</summary>
        //public Boolean AutoCutTableName { get; set; }

        ///// <summary>是否自动纠正大小写。默认启用</summary>
        //public Boolean AutoFixWord { get; set; }

        ///// <summary>要过滤的前缀</summary>
        //public String[] FilterPrefixs { get; set; }
        #endregion

        #region 构造
        /// <summary>实例化一个默认解析器</summary>
        public ModelResolver()
        {
            //var set = Setting.Current.Model;
            //UseID = set.UseID;
            //AutoCutPrefix = set.AutoCutPrefix;
            //AutoCutTableName = set.AutoCutTableName;
            //AutoFixWord = set.AutoFixWord;
            //FilterPrefixs = set.FilterPrefixs.Split();
        }
        #endregion
    }
}