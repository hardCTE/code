using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace TemplateHelper
{
    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 非法字符
        /// </summary>
        private static char[] _cleanChars = new char[] { ' ', '_', '-', '.' };

        /// <summary>
        /// 如果生成的名称为保留字则添加的后缀
        /// </summary>
        public const string ApendCsharpReserved = "Value";

        /// <summary>
        /// C#保留字
        /// </summary>
        public static Regex CSharpReserved = new Regex("^(ABSTRACT|AS|BASE|BOOL|BREAK|BYTE|CASE|CATCH|CHAR|CHECKED|CLASS|CONST|CONTINUE|DECIMAL|DEFAULT|DELEGATE|DO|DOUBLE|ELSE|ENUM|EVENT|EXPLICIT|EXTERN|FALSE|FINALLY|FIXED|FLOAT|FOR|FOREACH|GET|GOTO|IF|IMPLICIT|IN|INT|INTERFACE|INTERNAL|IS|LOCK|LONG|NAMESPACE|NEW|NULL|OBJECT|OPERATOR|OUT|OVERRIDE|PARAMS|PARTIAL|PRIVATE|PROTECTED|PUBLIC|READONLY|REF|RETURN|SBYTE|SEALED|SET|SHORT|SIZEOF|STACKALLOC|STATIC|STRING|STRUCT|SWITCH|THIS|THROW|TRUE|TRY|TYPEOF|UINT|ULONG|UNCHECKED|UNSAFE|USHORT|USING|VALUE|VIRTUAL|VOID|VOLATILE|WHERE|WHILE|YIELD)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);


        /// <summary>
        /// 清除非法字符,并且非法字符之后的第一个字符为大写
        /// </summary>
        /// <param name="value">字符串</param>
        /// <returns>字符串</returns>
        public static string Clean(this string value)
        {
            var query = from str in value.Split(_cleanChars, StringSplitOptions.RemoveEmptyEntries)
                        let item = str.ConvertPascalCase().Trim()
                        select item;

            return String.Join("", query);
        }

        /// <summary>
        /// 转换关键字，如果为关键字，则在字符串末尾添加字符‘Value'
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ConvertKeyWord(this string value)
        {
            if (CSharpReserved.IsMatch(value))
            {
                value += ApendCsharpReserved;
            }

            return value;
        }

        /// <summary>
        /// 转化为合法的变量名(首字母必须是以[a-zA-Z]开头)；如果不是合法的变量名，开头加关键字'CG'
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ConvertLegalName(this string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (Char.IsLetter(value, 0))
                return value;
            return "CG" + value;
        }

        /// <summary>
        /// 使用camel命名法
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertCamelCase(this string value)
        {
            return char.ToLower(value[0]) + value.Substring(1).ConvertToLowerString();
        }

        private static string ConvertToLowerString(this string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.All(char.IsUpper) ? value.ToLower() : value;
        }

        /// <summary>
        /// 使用Pascal命名法
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertPascalCase(this string value)
        {
            return char.ToUpper(value[0]) + value.Substring(1).ConvertToLowerString();
        }

        /// <summary>
        /// 将一个名称转换为程序内部使用的名称(合法的变量名称)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ToCodeName(this string name)
        {
            return name.Clean().ConvertKeyWord().ConvertLegalName();
        }

        /// <summary>
        /// 获得复数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string MakePlural(this string name)
        {
            Regex plural1 = new Regex("(?<keep>[^aeiou])y$");
            Regex plural2 = new Regex("(?<keep>[aeiou]y)$");
            Regex plural3 = new Regex("(?<keep>[sxzh])$");
            Regex plural4 = new Regex("(?<keep>[^sxzhy])$");

            if (plural1.IsMatch(name))
                return plural1.Replace(name, "${keep}ies");
            else if (plural2.IsMatch(name))
                return plural2.Replace(name, "${keep}s");
            else if (plural3.IsMatch(name))
                return plural3.Replace(name, "${keep}es");
            else if (plural4.IsMatch(name))
                return plural4.Replace(name, "${keep}s");

            return name;
        }

        /// <summary>
        /// 获得单数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string MakeSingle(this string name)
        {
            Regex plural1 = new Regex("(?<keep>[^aeiou])ies$");
            Regex plural2 = new Regex("(?<keep>[aeiou]y)s$");
            Regex plural3 = new Regex("(?<keep>[sxzh])es$");
            Regex plural4 = new Regex("(?<keep>[^sxzhyu])s$");

            if (plural1.IsMatch(name))
                return plural1.Replace(name, "${keep}y");
            else if (plural2.IsMatch(name))
                return plural2.Replace(name, "${keep}");
            else if (plural3.IsMatch(name))
                return plural3.Replace(name, "${keep}");
            else if (plural4.IsMatch(name))
                return plural4.Replace(name, "${keep}");

            return name;
        }

        /// <summary>
        /// Trim字符串,删除指定前后字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string TrimString(this string source, string start, string end)
        {
            string ret = source;
            if (!string.IsNullOrEmpty(start))
            {
                if (source.IndexOf(start) == 0)
                    ret = source.Substring(start.Length);
            }
            if (!string.IsNullOrEmpty(end))
            {
                if (source.LastIndexOf(end) != -1 && source.LastIndexOf(end) == source.Length - end.Length)
                    ret = ret.Substring(0, ret.Length - end.Length);
            }
            return ret;
        }


        /// <summary>
        /// 实体类名称
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static string GetEOClassName(this string name)
        {
            return name + "EO";
        }

        /// <summary>
        /// 逻辑类名称
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static string GetMOClassName(this string name)
        {
            return name + "MO";
        }

        /// <summary>
        /// 存储过程操作名称
        /// </summary>
        /// <param name="name">存储过程对象</param>
        /// <returns></returns>
        public static string GetP0ClassName(this string name)
        {
            return name + "PO";
        }

        /// <summary>
        /// 枚举类型名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetEnumName(this string name)
        {
            return name + "Enum";
        }

        /// <summary>
        /// 获得映射的实体类的属性名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetPropertyName(this string name)
        {
            return name.ConvertPascalCase();
        }

        /// <summary>
        /// 获得映射的实体类的字段名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetFieldName(this string name)
        {
            return "_" + name.ConvertCamelCase();
        }

        /// <summary>
        /// 获得映射的实体类的参数名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetParameterName(this string name)
        {
            return name.ConvertCamelCase();
        }

        /// <summary>
        /// 获取主键列映射的实体类的属性名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetPropertyNameForPrimaryColumn(this string name)
        {
            return "Original" + GetPropertyName(name);
        }

        /// <summary>
        /// 获取主键列映射的实体类的字段名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetFieldNameForPrimaryColumn(this string name)
        {
            return "_original" + GetPropertyName(name);
        }

        /// <summary>
        /// 获取外键列映射的实体类的属性名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetPropertyNameForForeignKey(this string name)
        {
            return name.GetPropertyName() + "Source";
        }

        /// <summary>
        /// 获取外键列映射的实体类的字段名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetFieldNameForForeignKey(this string name)
        {
            return (name.GetPropertyName() + "Source").GetFieldName();
        }
    }
}
