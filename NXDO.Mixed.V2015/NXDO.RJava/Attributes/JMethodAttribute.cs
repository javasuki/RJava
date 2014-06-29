using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace NXDO.RJava.Attributes
{
    /// <summary>
    /// 标识 java 方法真实名称
    /// <para>java 方法真实名称的首字母为小写时，可省略。</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class JMethodAttribute : System.Attribute
    {
        /// <summary>
        /// 标识 java 为首字母小写的方法。
        /// <para>java 方法真实名称的首字母为小写时，可省略。</para>
        /// </summary>
        public JMethodAttribute()
        {
            //this.ParamJValues = new List<JValueAttribute>();
        }

        /// <summary>
        /// 标识 java 方法。
        /// </summary>
        /// <param name="jMethodName">java 方法真实名称。</param>
        public JMethodAttribute(string jMethodName) : this()
        {
            this.Name = jMethodName;
        }

        /// <summary>
        /// 获取java方法名称。
        /// </summary>
        public string Name
        {
            get;
            internal set;
        }


        ///// <summary>
        ///// 方法的参数列表设置
        ///// </summary>
        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        //internal List<JValueAttribute> ParamJValues
        //{
        //    get;
        //    private set;
        //}

        ///// <summary>
        ///// 空表示void,反之具有返回值
        ///// </summary>
        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        //internal JValueAttribute ReturnJValue
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 是否为静态方法。
        ///// </summary>
        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        //internal bool IsStatic
        //{
        //    get;
        //    set;
        //}

        ///// <summary>
        ///// 是否为泛型方法 (仅赋值,未使用)
        ///// </summary>
        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        //internal bool IsGenericMethod
        //{
        //    get;
        //    set;
        //}
    }
}
