using System;
using XCode.DataAccessLayer;

namespace XCoder.TemplateHelper
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
        public static string GetFieldTypeString(IDataColumn field)
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
    }
}
