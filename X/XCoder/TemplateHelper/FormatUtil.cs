﻿using System;
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
        /// 将一个名称转换为程序内部使用的名称(合法的变量名称)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ToPrivatePropertyName(string name)
        {
            name = (name ?? string.Empty).ToCodeName();
            if (string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            return char.ToUpper(name[0]) + name.Substring(1);
        }
    }
}
