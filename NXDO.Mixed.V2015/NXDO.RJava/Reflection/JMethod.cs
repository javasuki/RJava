using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NXDO.RJava.Core;

namespace NXDO.RJava.Reflection
{
    /// <summary>
    /// 发现 java 方法的属性并提供对 java 方法元数据的访问。
    /// </summary>
    [DebuggerDisplay("Name = {Name}, IsPublic = {IsPublic}")]
    public sealed class JMethod
    {
        internal JMethod(IntPtr handle, string methodName)
        {
            this.Handle = handle;
            this.Name = methodName;

            bool[] boolAry = JRunEnvironment.ReflectionHelper.GetAccessModifier(handle);
            this.IsAbstract = boolAry[0];
            this.IsFinal = boolAry[1];
            this.IsPrivate = boolAry[2];
            this.IsPublic = boolAry[3];
            this.IsStatic = boolAry[4];

            List<JParameter> lstParams = new List<JParameter>();
            var ptrs = JRunEnvironment.ReflectionHelper.GetMethodParams(handle);
            foreach (var ptr in ptrs)
            {
                lstParams.Add(new JMReturn<JParamInfoInternal>(ptr).Value.GetJParameter());
            }
            this.Params = lstParams.ToArray();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal JParameter[] Params;
        /// <summary>
        /// 获取指定的方法或构造函数的参数。
        /// </summary>
        /// <returns>JParameter 类型的数组，包含与此 JMethod 实例所反射的方法（或构造函数）的签名匹配的信息。</returns>
        public JParameter[] GetParameters()
        {
            return Params;
        }

        /// <summary>
        /// java 方法指针
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
                    var declaringHandle = JRunEnvironment.ReflectionHelper.GetMethodDeclaringClass(this.Handle);
                    if (declaringHandle == IntPtr.Zero) return null;
                    string declaringName = JRunEnvironment.ReflectionHelper.GetClassName(declaringHandle);
                    declaringJClass = new JClass(declaringHandle, declaringName);
                }
                return declaringJClass;
            }
        }

        private IntPtr invokeJava(JObject jobject, object[] parameters, ref bool resultIsArray)
        {
            IntPtr objPtr = jobject != null ? jobject.Handle : IntPtr.Zero;
            return invokeJavaByPtr(objPtr, parameters, ref resultIsArray);
        }

        internal IntPtr invokeJavaByPtr(IntPtr handle, object[] parameters, ref bool resultIsArray)
        {
            List<IParamValue> lstJPVs = new List<IParamValue>();
            if (parameters == null) parameters = new object[] { };
            int idx = 0;
            foreach (object joVal in parameters)
            {
                bool isArray = false; 
                Type type = typeof(JObject);
                if (joVal != null)
                {
                    isArray = joVal.GetType().IsArray;
                    if (isArray)
                        type = typeof(JObject[]);
                }

                var jparam = this.Params[idx++];
                var val = JInvokeHelper.CreateJParamValue(type, joVal);
                val.ChangeJavaClass(!isArray ? jparam.ParameterClass.Handle : jparam.ParameterClass.GetArrayHandle());
                lstJPVs.Add(val);
            }

            IntPtr ptr = JRunEnvironment.ReflectionHelper.InvokeMethod(handle, this.Handle, lstJPVs.ToArray());
            resultIsArray = JRunEnvironment.ReflectionHelper.CheckIsArray(ptr);
            return ptr;
        }

        /// <summary>
        /// 使用指定的参数调用当前实例所表示的方法或构造函数。
        /// </summary>
        /// <param name="jobject">对其调用方法或构造函数的对象。如果方法是静态的，则忽略此参数。</param>
        /// <param name="parameters">调用的方法或构造函数的参数列表。如果没有参数，则此应为 null。</param>
        /// <returns>包含被调用方法的返回值；如果方法的返回类型是 void，则为 null。</returns>
        public object Invoke(JObject jobject, object[] parameters)
        {            
            bool resultIsArray = false;
            var ptr = this.invokeJava(jobject, parameters, ref resultIsArray);
            if(!resultIsArray)
                return new JMReturn<JObject>(ptr).Value;
            return new JMReturn<JObject[]>(ptr).Value;
        }


        /// <summary>
        /// 使用指定的参数调用当前实例所表示的方法或构造函数。
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="jobject">对其调用方法或构造函数的对象。如果方法是静态的，则忽略此参数。</param>
        /// <param name="parameters">调用的方法或构造函数的参数列表。如果没有参数，则此应为 null。</param>
        /// <returns>包含被调用方法的返回值；如果方法的返回类型是 void，则为 null。</returns>
        public T Invoke<T>(JObject jobject, object[] parameters)
        {
            bool resultIsArray = false;
            var ptr = this.invokeJava(jobject, parameters, ref resultIsArray);
            return new JMReturn<T>(ptr).Value;
        }
    }
}
