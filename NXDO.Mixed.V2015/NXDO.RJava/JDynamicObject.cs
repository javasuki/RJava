using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using System.Runtime.CompilerServices;
using RBinder = System.Reflection.Binder;
using CBinder = Microsoft.CSharp.RuntimeBinder.Binder;

using NXDO.RJava.Extension;

namespace NXDO.RJava
{
    /// <summary>
    /// 包装 JObject 的动态行为的实例。
    /// </summary>
    [DebuggerDisplay("java = {jclassName}")]
    public sealed class JDynamicObject : System.Dynamic.DynamicObject
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        JObject jobject;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string jclassName;

        /// <summary>
        /// 当前包装的 java 类型
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        JClass jclass;

        internal JDynamicObject(JObject instanceObj, JClass instanceOfClass)
        {
            this.jobject = instanceObj;
            this.jclass = instanceOfClass;
            this.jclassName = instanceOfClass.FullName;
        }

        private string GetMethodName(InvokeMemberBinder binder)
        {
            return JInvokeHelper.GetDefaultMethodName(binder.Name);
        }

        private IntPtr invokeJavaMethod(string methodName, object[] args, ref bool isArray)
        {
            //TODO，保持参数匹配
            int iArgsSize = args.Length;
            var methods = this.jclass.GetOptimalMethods(methodName, iArgsSize);
            if (methods.Count == 0)
                throw new MemberAccessException("没有找到最佳匹配的方法:" + methodName);
            else if (methods.Count == 1)
            {
                var m1 = methods[0];
                return m1.invokeJavaByPtr(!m1.IsStatic ?  this.jobject.Handle : this.jobject.GetClass().Handle, args, ref isArray);
            }

            //同名方法,参数个数相同
            for (int i = 0; i < methods.Count; i++)
            {
                var prms = methods[i].Params;
                if (iArgsSize != prms.Length)
                    throw new MethodAccessException("重载方法 " + methodName + " 参数不确定，无法执行调用。");

                int idx = 0;
                bool isSameType = true;
                foreach (var pp in prms)
                {
                    object oVal = args[idx];
                    if (oVal is JDynamic)
                    {
                        var jdyp = oVal as JDynamic;
                        if(jdyp.Class != pp.ParameterClass)
                            isSameType = false;
                    }
                    else if (oVal != null)
                    {
                        Type dotType = oVal.GetType();
                        if (dotType.ToJavaClass() != pp.ParameterClass)
                            isSameType = false;
                    }

                    if (!isSameType) break;
                }

                if (isSameType)
                {
                    var m1 = methods[i];
                    return m1.invokeJavaByPtr(!m1.IsStatic ? this.jobject.Handle : this.jobject.GetClass().Handle, args, ref isArray);
                }                
            }

            throw new NotSupportedException("在重载方法匹配参数时，未找到最佳方法，无法完成调用。");
            //return IntPtr.Zero;
        }

        /// <summary>
        /// 执行动态方法(实例或静态)。
        /// </summary>
        /// <param name="binder">动态操作绑定实例。</param>
        /// <param name="args">动态方法的参数</param>
        /// <param name="result">方法执行后的返回结果</param>
        /// <returns>是否执行成功。</returns>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            string methodName = binder.Name; // this.GetMethodName(binder);
            try
            {
                bool resultIsArray = false;
                var ptr = this.invokeJavaMethod(methodName, args, ref resultIsArray);

                if (!resultIsArray)
                    result = new NXDO.RJava.Core.JMReturn<JObject>(ptr).Value;
                else
                    result = new NXDO.RJava.Core.JMReturn<JObject[]>(ptr).Value;

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// dynamic 关键字
        /// </summary>
        /// <returns></returns>
        public dynamic GetDynamic()
        {
            return this;
        }

        #region 隐式转换        
        public static implicit operator JObject(JDynamicObject value)
        {
            if (value == null) return null;
            return value.jobject;
        }

        public static implicit operator JDynamicObject(JObject value)
        {
            if (value == null) return null;
            if (value.Handle == IntPtr.Zero) return null;
            return new JDynamicObject(value, value.GetClass());
        }
        #endregion
    }
}
