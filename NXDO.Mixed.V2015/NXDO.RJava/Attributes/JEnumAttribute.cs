using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NXDO.RJava.Attributes
{
    /// <summary>
    /// 标识 java 枚举的类型名称。
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple=false, Inherited=false)]
    public class JEnumAttribute : JClassAttribute
    {
        /// <summary>
        /// 设置 java 枚举的类型名称
        /// </summary>
        /// <param name="jEnumClassName">java 枚举的类型名称</param>
        public JEnumAttribute(string jEnumClassName)
            : base(jEnumClassName)
        {
        }
    }
}
