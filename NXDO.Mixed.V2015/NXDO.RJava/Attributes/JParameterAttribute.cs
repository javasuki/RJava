using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NXDO.RJava.Attributes
{
    /// <summary>
    /// 标识参数的真实 java 类型。
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class JParameterAttribute : JClassAttribute
    {
        /// <summary>
        /// 设置 java 方法参数调用时对应实际的 java 类型名称
        /// </summary>
        /// <param name="jInterfaceName">java 参数实际的类型名称</param>
        public JParameterAttribute(string jClassName)
            : base(jClassName)
        {
        }
    }
}
