using System;
using XCode.DataAccessLayer;

namespace TemplateHelper
{
    /// <summary>
    /// 格式化实用类
    /// </summary>
    public static class FormatUtil
    {
        /// <summary>
        /// 转为字段类型字符串
        /// （可空的会携带?）
        /// </summary>
        /// <param name="field">字段信息</param>
        /// <returns></returns>
        public static string ToFieldTypeString(IDataColumn field)
        {
            if (field?.DataType == null)
            {
                return string.Empty;
            }

            if (field.Nullable && !string.Equals(field.DataType.Name, "string", StringComparison.OrdinalIgnoreCase))
            {
                return field.DataType.Name + "?";
            }

            return field.DataType.Name;
        }

        /// <summary>
        /// 将一个名称转换为程序内部使用的名称(合法的变量名称)
        /// Eg:CatName
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ToCodeName(string name)
        {
            name = name ?? string.Empty;
            return name.ToCodeName();
        }

        /// <summary>
        /// 将一个名称转换为私有变量名称(合法的变量名称)
        /// Eg:_abcAbc
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ToPrivateName(string name)
        {
            name = (name ?? string.Empty).ToCodeName();
            if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            {
                return "_" + name;
            }

            return "_" + char.ToLower(name[0]) + name.Substring(1);
        }

        /// <summary>
        /// 将一个名称转换为参数名称(合法的变量名称)
        /// Eg:abcAbc
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ToParamName(string name)
        {
            name = (name ?? string.Empty).ToCodeName();
            if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            {
                return name;
            }

            return char.ToLower(name[0]) + name.Substring(1);
        }

        /// <summary>
        /// 将字符串转为单行显示格式
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static string ToSigleDisplay(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return string.Empty;
            }

            return raw.Replace("\r\n", " ").Replace("\\", "\\\\").Replace("'", "").Replace("\"", "");	
        }
    }
}
