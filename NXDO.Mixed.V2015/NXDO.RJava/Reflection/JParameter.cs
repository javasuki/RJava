using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NXDO.RJava.Reflection
{
    /// <summary>
    /// 发现参数属性并提供对参数元数据的访问。
    /// </summary>
    public class JParameter
    {
        internal JParameter()
        {
        }

        /// <summary>
        /// 参数类型(泛型参数时,为擦除后的超类类型)
        /// </summary>
        public JClass ParameterClass
        {
            get;
            internal set;
        }

        /// <summary>
        /// 泛型参数的名称(普通参数无名称)
        /// </summary>
        internal string GenericParamName
        {
            get;
            set;
        }

        /// <summary>
        /// true: 泛型类
        /// </summary>
        internal bool IsGenericClass
        {
            get;
            set;
        }

        /// <summary>
        /// true: 泛型参数
        /// </summary>
        internal bool IsGenericParameter
        {
            get;
            set;
        }
    }
}
