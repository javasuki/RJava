using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using NXDO.RJava.Core;
using NXDO.RJava.Attributes;

namespace NXDO.RJava
{
    /// <summary>
    /// 包含特定的方法，用以在本地创建对象类型。此类不能被继承。
    /// </summary>
    public sealed class JActivator
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static JRunCore JContext;
        static JActivator()
        {
            //保证启动JVM
            JContext = JAssembly.JBridgeContext;
        }

        #region CreateInstance
        /// <summary>
        /// 使用无参数构造函数，创建 java 到 dotnet 对象引用实例。
        /// </summary>
        /// <param name="jClassName">首选 java 类型的名称。</param>
        /// <returns>对新创建 java 对象的引用。</returns>
        public static JObject CreateInstance(string jClassName)
        {
            return JActivator.CreateInstance(JClass.ForName(jClassName));
        }

        /// <summary>
        /// 使用无参数构造函数，创建 java 到 dotnet 对象引用实例。
        /// </summary>
        /// <param name="jClass">要创建的 JClass 类型引用实例</param>
        /// <returns>对新创建 java 对象的引用。</returns>
        public static JObject CreateInstance(JClass jClass)
        {
            var objPtr = JContext.JNew(jClass.Handle, new JParamValue[] { });
            return new JMReturn<JObject>(objPtr).Value;
        }

        /// <summary>
        /// 使用无参数构造函数，创建指定泛型类型参数所指定类型的 java 到 dotnet 对象引用实例。
        /// </summary>
        /// <typeparam name="T">要创建的 dotnet 类型</typeparam>
        /// <returns>对新创建 java 对象的引用。</returns>
        public static T CreateInstance<T>() where T : JObject
        {
            if (typeof(T) == typeof(JObject))
                throw new ArgumentException("泛型参数必须明确指定为 JObject 的继承类。", "T");

            string jclassName = JClassAttribute.Get(typeof(T));
            var clsPtr = JClass.ForName(jclassName).Handle;
            var objPtr = JContext.JNew(clsPtr, new JParamValue[] { });
            return new JMReturn<T>(objPtr).Value;
        }
        #endregion

        /// <summary>
        /// 建立一个实现指定接口的对象实例。
        /// </summary>
        /// <typeparam name="T">指定接口类型</typeparam>
        /// <returns>实现了该接口的对象实例</returns>
        public static T CreateByInterface<T>() where T : class
        {
            return new JEmitInterface<T>().Create();
        }

        /// <summary>
        /// 建立动态包装对象。
        /// </summary>
        /// <param name="jClassName">首选 java 类型的名称。</param>
        /// <returns>java 动态包装对象</returns>
        public static JDynamicObject CreateDynamic(string jClassName)
        {
            return JActivator.CreateDynamic(JClass.ForName(jClassName));
        }

        /// <summary>
        /// 建立动态包装对象。
        /// </summary>
        /// <param name="jClass">要创建的 JClass 类型引用实例</param>
        /// <returns>java 动态包装对象</returns>
        public static JDynamicObject CreateDynamic(JClass jClass)
        {
            JObject jo = JActivator.CreateInstance(jClass);
            return new JDynamicObject(jo, jClass);
        }
    }
}
