using System;

namespace Tool.CustomAttribute
{
    public class DbHelp : Attribute
    {
        /// <summary>
        /// 表中字段名字
        /// </summary>
        public string PropertyName { get; set; }
        
        /// <summary>
        /// 表中字段类型
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        /// 是否为主键
        /// </summary>
        /// <returns></returns>
        public bool IsKey { get; set; }

        /// <summary>
        /// 是否可为空
        /// </summary>
        public bool CanBeNull { get; set; }

        public DbHelp( string propertyName, Type propertyType, bool isKey, bool canBeNull)
        {
            PropertyName = propertyName;
            PropertyType = propertyType;
            IsKey = isKey;
            CanBeNull = canBeNull;
        }
    }
}