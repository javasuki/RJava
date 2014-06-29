using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace NXDO.RJava
{
    /// <summary>
    /// 动态方法参数
    /// </summary>
    public sealed class JDynamic
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        JClass jclass;
        private JDynamic(String javaClassName, Object value)
        {
            jclass = JClass.ForName(javaClassName);
            this.Value = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal JClass Class
        {
            get
            {
                return jclass;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal Object Value
        {
            get;
            private set;
        }


        /// <summary>
        /// 获取传递给动态方法的参数值
        /// </summary>
        /// <param name="javaClassName">显示指定参数的 java 类型名称</param>
        /// <param name="value">参数值</param>
        /// <returns>动态方法参数</returns>
        public static JDynamic Parameter(String javaClassName, Object value)
        {
            return new JDynamic(javaClassName, value);
        }

    }
}
