using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NXDO.RJava.Core;

namespace NXDO.RJava.Reflection
{
    /// <summary>
    /// 发现 java 类构造函数的属性并提供对 &lt;init&gt; 构造函数元数据的访问权。
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    public sealed class JConstructor
    {
        internal JConstructor(IntPtr handle, string ctorName)
        {
            this.Handle = handle;
            this.Name = ctorName;

            bool[] boolAry = JRunEnvironment.ReflectionHelper.GetCtorModifier(handle);
            this.IsAbstract = boolAry[0];
            this.IsFinal = boolAry[1];
            this.IsPrivate = boolAry[2];
            this.IsPublic = boolAry[3];
            this.IsStatic = boolAry[4];
        }

        /// <summary>
        /// java 构造函数指针
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal IntPtr Handle
        {
            get;
            private set;
        }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        #region 相关访问性
        /// <summary>
        /// 获取一个值，该值指示此方法是否为抽象方法。
        /// </summary>
        public bool IsAbstract
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取一个值，该值指示此方法是否为 final。
        /// </summary>
        public bool IsFinal
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取一个值，该值指示此成员是否是私有的。
        /// </summary>
        public bool IsPrivate
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取一个值，该值指示这是否是一个公共方法。
        /// </summary>
        public bool IsPublic
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取一个值，该值指示方法是否为 static。
        /// </summary>
        public bool IsStatic
        {
            get;
            private set;
        }
        #endregion

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JClass declaringJClass;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isRunGetDeclaringJClass;
        /// <summary>
        ///  获取声明该成员的 JClass 类。
        /// </summary>
        public JClass DeclaringClass
        {
            get
            {
                if (!isRunGetDeclaringJClass)
                {
                    isRunGetDeclaringJClass = true;
                    var declaringHandle = JRunEnvironment.ReflectionHelper.GetConstructorDeclaringClass(this.Handle);
                    if (declaringHandle == IntPtr.Zero) return null;
                    string declaringName = JRunEnvironment.ReflectionHelper.GetClassName(declaringHandle);
                    declaringJClass = new JClass(declaringHandle, declaringName);
                }
                return declaringJClass;
            }
        }

        /// <summary>
        /// 调用具有指定参数的实例所反映的构造函数，并为参数提供值。
        /// </summary>
        /// <param name="parameters">与此构造函数的参数的个数、顺序和类型（受默认联编程序的约束）相匹配的值数组。</param>
        /// <returns>返回与构造函数关联的类的实例。</returns>
        public JObject Invoke(object[] parameters)
        {
            return this.Invoke<JObject>(parameters);
        }

        /// <summary>
        /// 调用具有指定参数的实例所反映的构造函数，并为参数提供值。
        /// </summary>
        /// <typeparam name="T">JObject 继承类的类型。</typeparam>
        /// <param name="parameters">与此构造函数的参数的个数、顺序和类型（受默认联编程序的约束）相匹配的值数组。</param>
        /// <returns>返回与指定T类型关联的类的实例。</returns>
        public T Invoke<T>(object[] parameters) where T : JObject
        {
            List<IParamValue> lstJPVs = new List<IParamValue>();
            if (parameters == null) parameters = new object[] { };
            foreach (object joVal in parameters)
            {
                Type type = typeof(JObject);
                if (joVal != null)
                {
                    bool isArray = joVal.GetType().IsArray;
                    if (isArray)
                        type = typeof(JObject[]);
                }
                var val = JInvokeHelper.CreateJParamValue(type, joVal);
                lstJPVs.Add(val);
            }

            IntPtr ptr = JRunEnvironment.ReflectionHelper.InvokeConstructor(this.Handle, lstJPVs.ToArray());
            return new JMReturn<T>(ptr).Value;
        }
    }
}
