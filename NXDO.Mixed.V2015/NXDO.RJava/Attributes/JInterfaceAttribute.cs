using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NXDO.RJava.Attributes
{
    /// <summary>
    /// 标识 java 接口的类型名称。
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class JInterfaceAttribute : JClassAttribute
    {
        /// <summary>
        /// 设置 java 接口的类型名称
        /// </summary>
        /// <param name="jInterfaceName">java 接口的类型名称</param>
        public JInterfaceAttribute(string jInterfaceName)
            : base(jInterfaceName)
        {
        }
    }

    /// <summary>
    /// 标识 java 类型名称，仅用于接口的实现。
    /// <para>标识的 java 类型必须具无参的公共默认构造函数。</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class JEmitAttribute : JInterfaceAttribute
    {
        /// <summary>
        /// 标识 java 类型名称，用于接口实现
        /// </summary>
        /// <param name="javaClassName"></param>
        public JEmitAttribute(string javaClassName)
            : base(javaClassName)
        {
        }
    }
}
