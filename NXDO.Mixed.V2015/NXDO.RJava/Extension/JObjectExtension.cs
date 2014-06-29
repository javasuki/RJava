using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using NXDO.RJava.Core;

namespace NXDO.RJava.Extension
{
    /// <summary>
    /// JObject 扩展
    /// </summary>
    public static class JObjectExtension
    {
        #region 返回值 JUnknown 的转换
        /// <summary>
        /// 将 JObject 转换成 java 所对应 dotnet 的值。
        /// <para>转换后，相关实例可能被 JVM 释放。</para>
        /// </summary>
        /// <typeparam name="T">dotnet 转换类型</typeparam>
        /// <param name="jobject">需要转换的值</param>
        /// <returns>T类型的值</returns>
        public static T ToDotValue<T>(this JObject jobject)
        {
            if (jobject == null)
                return default(T);

            bool isJBasePtr = jobject is JUnknown;
            if (!isJBasePtr)
                throw new InvalidCastException("无法转换当前类型，请检查自方法调用以来其返回值为 JObject 类型。");

            var jobjectReturn = jobject as JUnknown;
            return new JMReturn<T>(IntPtr.Zero, jobjectReturn).Value;
        }

        /// <summary>
        /// 将 object 转换成 java 所对应 dotnet 的值。
        /// <para>转换后，相关实例可能被 JVM 释放。</para>
        /// </summary>
        /// <typeparam name="T">dotnet 转换类型</typeparam>
        /// <param name="jobject">需要转换的值</param>
        /// <returns>T类型的值</returns>
        public static T ToDotValue<T>(this object jobject)
        {
            return ToDotValue<T>((JUnknown)jobject);
        }

        /// <summary>
        /// 将 JObject迭代器 转换成 java 所对应 dotnet 的迭代器。
        /// <para>转换后，相关实例可能被 JVM 释放。</para>
        /// </summary>
        /// <typeparam name="T">dotnet 转换类型</typeparam>
        /// <param name="jobjects">需要转换的迭代器</param>
        /// <returns>T类型的迭代器</returns>
        public static IEnumerable<T> ToDotValue<T>(this IEnumerable<JObject> jobjects)
        {
            foreach (JObject jr in jobjects)
                yield return JObjectExtension.ToDotValue<T>(jr);
        }

        /// <summary>
        /// 将 object 迭代器 转换成 java 所对应 dotnet 的迭代器。
        /// <para>转换后，相关实例可能被 JVM 释放。</para>
        /// </summary>
        /// <typeparam name="T">dotnet 转换类型</typeparam>
        /// <param name="objects">需要转换的迭代器</param>
        /// <returns>T类型的迭代器</returns>
        public static IEnumerable<T> ToDotValue<T>(this IEnumerable<object> objects)
        {
            foreach (object jr in objects)
                yield return JObjectExtension.ToDotValue<T>((JUnknown)jr);
        }
        #endregion

        #region for Dynamic
        /// <summary>
        /// 转换成动态包装实例，可使用 dynamic 定义当前变量。
        /// <para>动态方法调用时，如果最佳参数匹配无法推导出方法，可使用 JDynamic.Get() 显示指定 java 参数的类型。</para>
        /// </summary>
        /// <param name="jobj">JObject 实例。</param>
        /// <returns>动态包装实例。</returns>
        public static JDynamicObject ToRJavaDynamic(this JObject jobj)
        {
            return (JDynamicObject)jobj;
        }
        #endregion

        #region ToRJavaDefine 反射建立 JObject 实例，转换成已经定义的类或接口
        /// <summary>
        /// 转换 JObject 为已知的 java 包装类型。比方，通过反射建立的 JObject，存在对应的类或接口，则可通过此进行转换。
        /// <example><para>例：</para></example>
        /// <code>
        ///     <para>var jclz = JClass.ForName("java.util.ArrayList");</para>
        ///     <para>var jobj = JActivator.CreateInstance(jclz);</para>
        ///     <para>jclz.GetMethod("add", JObject.Class)Invoke(jobj, new object[] { 1 });</para>
        ///     <para>//已上代码等同于 ArrayList.add(1);</para>
        ///     <para>JList&lt;int&gt; jaryList = jobj.ToRJavaDefine&lt;JList&lt;int&gt;&gt;();</para>
        ///     <para>其中：JList&lt;int&gt; 是已知的，并对应了 java.util.List 的接口。</para>
        /// </code>         
        /// </summary>
        /// <typeparam name="T">包装类型，支持接口与类。
        /// </typeparam>
        /// <param name="jobj">需要转换的实例。</param>
        /// <returns>T类型的实例，或T接口的实现实例。</returns>
        public static T ToRJavaDefine<T>(this JObject jobj) where T : IRJavaDefine<T>
        {
            return JObjectExtension.ToRJavaDefine<T>(jobj, typeof(JObject).Assembly);
        }

        /// <summary>
        /// 转换 JObject 为已知的 java 包装类型。比方，通过反射建立的 JObject，存在对应的类或接口，则可通过此进行转换。
        /// <example><para>例：</para></example>
        /// <code>
        ///     <para>var jclz = JClass.ForName("java.util.ArrayList");</para>
        ///     <para>var jobj = JActivator.CreateInstance(jclz);</para>
        ///     <para>jclz.GetMethod("add", JObject.Class)Invoke(jobj, new object[] { 1 });</para>
        ///     <para>//已上代码等同于 ArrayList.add(1);</para>
        ///     <para>JList&lt;int&gt; jaryList = jobj.ToRJavaDefine&lt;JList&lt;int&gt;&gt;();</para>
        ///     <para>其中：JList&lt;int&gt; 是已知的，并对应了 java.util.List 的接口。</para>
        /// </code>         
        /// </summary>
        /// <typeparam name="T">包装类型，支持接口与类。
        /// </typeparam>
        /// <param name="jobj">需要转换的实例。</param>
        /// <param name="searchAssembly">搜索 T 所在的程序集。仅当T是接口并且是用户自定义实现时，需要提供此参数。</param>
        /// <returns>T类型的实例，或T接口的实现实例。</returns>
        public static T ToRJavaDefine<T>(this JObject jobj, Assembly searchAssembly) where T : IRJavaDefine<T>
        {
            //注释，说明代码， t1 可从 t2 分配，而直接使用原始泛型定义类型，则无法分配。
            //var t1 = typeof(IConvertCoreDefine<JList<int>>); //JavaList<int> 实现了 IConvertCoreDefine<JList<int>> 接口
            //var t2 = typeof(JavaList<int>);
            //bool b = t1.IsAssignableFrom(t2);
            if (jobj == null) return default(T);
            if (searchAssembly == null) return default(T);

            bool isSelfDefine = false;
            Type findType = null;
            var tType = typeof(T);            
            if (tType.IsClass)
                findType = tType;
            else
            {
                var jobjType = typeof(JObject);
                var t1 = typeof(IRJavaDefine<T>);

                bool tTypeIsGeneric = tType.IsGenericType;
                var tGTypes = tTypeIsGeneric ? tType.GetGenericArguments() : Type.EmptyTypes;
                if (tTypeIsGeneric)
                {
                    var tSelfDefineType = tType.GetGenericTypeDefinition();
                    if (tSelfDefineType == typeof(JList<>))
                    {
                        isSelfDefine = true;
                        findType = typeof(JavaList<>).MakeGenericType(tGTypes);
                    }
                    else if (tSelfDefineType == typeof(JDictionary<,>))
                    {
                        isSelfDefine = true;
                        findType = typeof(JDictionary<,>).MakeGenericType(tGTypes);
                    }
                    else if (tSelfDefineType == typeof(JStack<>))
                    {
                        isSelfDefine = true;
                        findType = typeof(JStack<>).MakeGenericType(tGTypes);
                    }
                    else if (tSelfDefineType == typeof(JQueue<>))
                    {
                        isSelfDefine = true;
                        findType = typeof(JQueue<>).MakeGenericType(tGTypes);
                    }
                }

                if (findType == null)
                {
                    var type = typeof(T).GetGenericTypeDefinition();
                    var objTypes = searchAssembly.GetTypes().Where(
                        objType =>
                        {
                            #region 搜索实现了 IConvertCoreDefine<T> 接口的类
                            if (objType.IsInterface) return false;
                            if (!objType.IsClass) return false;
                            if (!objType.IsSubclassOf(jobjType)) return false;
                            if (objType.IsGenericType != tTypeIsGeneric) return false;
                            if (tType.IsGenericType)
                            {
                                Type mkGType = null;
                                try
                                {
                                    mkGType = objType.MakeGenericType(tGTypes);
                                }
                                catch
                                {
                                    return false;
                                }
                                return t1.IsAssignableFrom(mkGType);
                            }
                            return t1.IsAssignableFrom(objType);
                            #endregion
                        }).Select(objType => objType.MakeGenericType(tGTypes));
                    if (objTypes.Count() == 0) return default(T);
                    findType = objTypes.First();
                }
            }

            if (findType.GetConstructor(Type.EmptyTypes) == null && !isSelfDefine /*用户自定义的*/)
                throw new ArgumentException("泛型参数所对应类型不具备无参的公共构造函数。", "T");
            var iDefineObject = Activator.CreateInstance(findType) as IRJavaDefine<T>;
            return (T)(iDefineObject.GetConvertToDefine(jobj));
        }
        #endregion

    }

}
