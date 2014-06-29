using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace NXDO.RJava.Attributes
{
    /// <summary>
    /// 标识 java 的类型名称。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited= true)]
    public class JClassAttribute : System.Attribute
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static Dictionary<Type, String> dicJavaClassNames;
        //static List<Type> selfAttributes;
        static JClassAttribute()
        {
            dicJavaClassNames = new Dictionary<Type, string>();
            //selfAttributes = new List<Type>();
        }

        /// <summary>
        /// 设置 java 类型名称
        /// </summary>
        /// <param name="jclassName">java 类型名称</param>
        public JClassAttribute(string jclassName)
        {
            if (string.IsNullOrWhiteSpace(jclassName))
                throw new ArgumentNullException("jclassName");
            this.ClassName = jclassName;
        }

        /// <summary>
        /// java 类型名称
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal string ClassName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取类上的 JClassAttribute 注解的 java 类型名称
        /// </summary>
        /// <param name="type">JObject继承类实例</param>
        /// <returns>java 类型名称</returns>
        internal static string Get(JObject jobject)
        {
            return JClassAttribute.Get(jobject.GetType());
        }

        static Type JClassAttributeType = typeof(JClassAttribute);
        static Type JInterfaceAttributeType = typeof(JInterfaceAttribute);
        static Type JEnumAttributeType = typeof(JEnumAttribute);

        //在调用方法时，才获取到参数上的注解，在 JInvokeHelper.CreateJParamValue 时获取。
        //static Type JParamAttributeType = typeof(JParameterAttribute);

        /// <summary>
        /// 获取类上的 JClassAttribute 注解的 java 类型名称
        /// </summary>
        /// <param name="type">类</param>
        /// <returns>java 类型名称</returns>
        internal static string Get(Type type)
        {
            bool bExists = dicJavaClassNames.ContainsKey(type);
            if (bExists)
                return dicJavaClassNames[type];

            bool isEnum = type.IsEnum;
            bool isInterface = type.IsInterface;
            bool isSubof = type.IsSubclassOf(typeof(JObject));
            if (isEnum || isInterface || isSubof)
            {
                Type tAttrType = null;
                if (isEnum)
                    tAttrType = JEnumAttributeType;
                else if (isInterface)
                    tAttrType = JInterfaceAttributeType;
                else
                    tAttrType = JClassAttributeType;

                object[] jClsAttrs = type.GetCustomAttributes(tAttrType, true);
                if (jClsAttrs.Length == 0)
                    throw new NullReferenceException(type.Name + "，缺少 " + tAttrType.Name + " 的特性。");

                JClassAttribute jca = jClsAttrs[0] as JClassAttribute;
                if (string.IsNullOrWhiteSpace(jca.ClassName))
                    throw new NullReferenceException(type.Name + "，" + tAttrType.Name + " 缺少 java 类型名称。");

                dicJavaClassNames.Add(type, jca.ClassName);
                return jca.ClassName;
            }
            else
                throw new NotSupportedException("不支持用户代码中实现的继承子类。");

        }
    }
}
