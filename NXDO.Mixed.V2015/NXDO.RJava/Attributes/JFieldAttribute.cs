using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace NXDO.RJava.Attributes
{
    /// <summary>
    /// 属性标识为 java 类的成员变量。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class JFieldAttribute : System.Attribute
    {
        /// <summary>
        /// 以首字母小写名称，标识 java 的成员变量。
        /// </summary>
        public JFieldAttribute()
        {
        }

        /// <summary>
        /// 以指定名称，标识 java 的成员变量。
        /// </summary>
        public JFieldAttribute(string jfieldName)
        {
            this.FieldName = jfieldName;
        }

        /// <summary>
        /// 成员变量名称。
        /// </summary>
        public string FieldName
        {
            get;
            internal set;
        }

        ///// <summary>
        ///// 变量的值标识
        ///// </summary>
        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        //internal JValueAttribute FieldJValue
        //{
        //    get;
        //    set;
        //}
    }
}
