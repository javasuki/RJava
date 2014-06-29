using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

using NXDO.RJava.Attributes;
using NXDO.RJava.Core;

namespace NXDO.RJava
{
    /// <summary>
    /// java参数建立的帮助器
    /// </summary>
    class JInvokeHelper
    {
        /// <summary>
        /// 获取构造函数的缓存信息
        /// </summary>
        /// <param name="type">JObject继承类的类型</param>
        /// <param name="values">参数值列表</param>
        /// <returns>java构造函数的参数值列表</returns>
        public static IParamValue[] GetConstructorParams(Type type, object[] values)
        {
            List<IParamValue> lstJPVs = new List<IParamValue>();

            #region 获取 jobject 继承类的 构造函数
            MethodBase initCtorMethod = null;
            int iNext = 2;
            do
            {
                StackFrame frame = new StackFrame(iNext++, true);
                initCtorMethod = frame.GetMethod();
                if (initCtorMethod == null) break;
                if (!initCtorMethod.Name.StartsWith(".ctor")) continue;
                break;
            }
            while (initCtorMethod != null);
            if (initCtorMethod == null)
                throw new MethodAccessException(type.Name + "不存在构造函数。");
            #endregion

            int idx = 0;
            foreach (var pinfo in initCtorMethod.GetParameters())
                lstJPVs.Add(JInvokeHelper.CreateJParamValue(pinfo.ParameterType, values[idx++]));

            return lstJPVs.ToArray();
        }

        /// <summary>
        /// 获取调用方法的缓存信息
        /// </summary>
        /// <param name="type">JObject继承类的类型</param>
        /// <returns>缓存信息对象实例</returns>
        public static JCacheInfo GetCacheMethodParams(Type type)
        {
            #region 获取 jobject 继承类的 方法名称
            MethodBase dotnetMethod = null;
            int iNext = 2;
            bool bFinded = false;
            do
            {
                StackFrame frame = new StackFrame(iNext++, true);
                dotnetMethod = frame.GetMethod();
                if (dotnetMethod == null) break;
                if (type.IsGenericType)
                {
                    if (dotnetMethod.DeclaringType == type.GetGenericTypeDefinition())
                    {
                        bFinded = true;
                        break;
                    }
                }
                else if (dotnetMethod.DeclaringType == type)
                {
                    bFinded = true;
                    break;
                }
            }
            while (dotnetMethod != null);
            if (!bFinded)
                throw new MethodAccessException(type.Name + ",未找到调用链上的方法。");
            #endregion

            MethodInfo mi = (MethodInfo)dotnetMethod;
            bool isVoid = mi.ReturnType == typeof(void);
            bool isStatic = dotnetMethod.IsStatic;

            #region 得到 java 方法名称
            String jMethodName = "";
            if(dotnetMethod.IsDefined(typeof(JMethodAttribute), true)){
                JMethodAttribute jmAttr = dotnetMethod.GetCustomAttributes(typeof(JMethodAttribute), true)[0] as JMethodAttribute;
                if (!string.IsNullOrWhiteSpace(jmAttr.Name))
                    jMethodName = jmAttr.Name;
            }


            if (string.IsNullOrWhiteSpace(jMethodName))
            {
                //判断是否为属性对应的特殊方法名称
                string prefixName = "";
                string tmpMethodName = dotnetMethod.Name;
                if (dotnetMethod.IsSpecialName)
                {
                    tmpMethodName = tmpMethodName.Substring(4);
                    if (dotnetMethod.Name.StartsWith("get_"))                    
                        prefixName = "get";
                    else if (dotnetMethod.Name.StartsWith("set_"))
                        prefixName = "set";
                }
                jMethodName = JInvokeHelper.GetDefaultMethodName(tmpMethodName, prefixName);
            }
            #endregion

            return new JCacheInfo { IsVoid = isVoid, IsStatic = isStatic, Name = jMethodName, Params = dotnetMethod.GetParameters().ToList() };
        }

        /// <summary>
        /// 获取成员变量对应属性的缓存信息
        /// </summary>
        /// <param name="type">JObject继承类的类型</param>
        /// <param name="isSet">是否为设置值</param>
        /// <param name="fieldName">java成员变量名称</param>
        /// <returns>缓存信息对象实例</returns>
        public static JCacheInfo GetCacheFieldParam(Type type, bool isSet, string fieldName)
        {            
            PropertyInfo pinfo = null;
            MethodBase dotnetMethod = null;
            int iNext = 2;
            do
            {
                StackFrame frame = new StackFrame(iNext++, true);
                dotnetMethod = frame.GetMethod();
                if (dotnetMethod == null) break;
                if (!dotnetMethod.IsSpecialName) continue;
                if (!dotnetMethod.DeclaringType.IsSubclassOf(typeof(JObject))) continue;

                BindingFlags bindingFlags = !dotnetMethod.IsStatic ? BindingFlags.IgnoreCase | BindingFlags.Public : BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public;
                type = dotnetMethod.DeclaringType;
                pinfo = type.GetProperty(fieldName);
                if (pinfo != null) break;
            }
            while (dotnetMethod != null);

            if (pinfo == null)
                throw new MethodAccessException("未找到调用链上的属性。");

            MethodInfo mi = (MethodInfo)dotnetMethod;
            bool isStatic = dotnetMethod.IsStatic;
            bool isVoid = mi.ReturnType == typeof(void);

            JCacheInfo jcacheInfo = new JCacheInfo { IsVoid = isVoid, IsStatic = isStatic, Name = fieldName };
            if (isSet)
                jcacheInfo.Params = mi.GetParameters().ToList();
            return jcacheInfo;
        }

        /// <summary>
        /// 获取静态/实例方法(成员变量)使用不同的指针值
        /// </summary>
        /// <param name="isStatic"></param>
        /// <param name="jObjectInstance"></param>
        /// <param name="jObjectType"></param>
        /// <returns>方法或成员变量调用指针值</returns>
        public static IntPtr GetInvokePtr(bool isStatic, JObject jObjectInstance, Type jObjectType)
        {
            IntPtr invokePtr = IntPtr.Zero;
            if (!isStatic)
            {
                if (jObjectInstance == null)
                    throw new ArgumentNullException("jInstance");

                if (jObjectInstance.Handle == IntPtr.Zero)
                    throw new ArgumentNullException("JObject未初始化过或者已被 jvm GC 释放!.");


                //实例方法使用 java 实例的指针值
                invokePtr = jObjectInstance.Handle;
            }
            else
                //静态方法使用 java 类型的指针值
                invokePtr = JClass.ForName(JClassAttribute.Get(jObjectType)).Handle;

            return invokePtr;
        }

        #region 创建传递到 java 方法的参数类型与参数值
        /// <summary>
        /// 转换成 java 可用的参数对象
        /// </summary>
        /// <param name="info">java 缓存信息</param>
        /// <param name="values">参数值</param>
        /// <returns>java 可用的参数值列表</returns>
        public static IParamValue[] ConvertToJParams(JCacheInfo info, object[] values)
        {
            List<IParamValue> lstJPVs = new List<IParamValue>();
            int idx = 0;
            foreach (var pinfo in info.Params)
                lstJPVs.Add(JInvokeHelper.CreateJParamValue(pinfo/*pinfo.ParameterType*/, values[idx++]));
            return lstJPVs.ToArray();
        }

        /// <summary>
        /// 创建传递到 java 方法的参数类型与参数值
        /// </summary>
        /// <param name="valType">c#参数类型</param>
        /// <param name="value">c#参数值</param>
        /// <returns>java方法的参数值列表</returns>
        internal static IParamValue CreateJParamValue(object oMemberType/*Type valType*/, object value)
        {
            object val = value is JDynamic ? ((JDynamic)value).Value : value;
            Type valType = null;

            String sParamClassName = "";
            if (oMemberType is Type)
                valType = (Type)oMemberType;
            else if (oMemberType is ParameterInfo)
            {
                ParameterInfo paramInfo = (ParameterInfo)oMemberType;
                valType = paramInfo.ParameterType;

                var objects = paramInfo.GetCustomAttributes(typeof(JParameterAttribute), true);
                sParamClassName =  (objects.Length > 0) ? (objects[0] as JParameterAttribute).ClassName : "";
            }

            bool isArray = valType.IsArray;
            Type paramType = isArray ? valType.GetElementType() : valType;
            bool isGenericParameter = paramType.IsGenericParameter;
            bool isEnum = paramType.IsEnum;

            if (val != null)
            {
                //已经是参数值的包装对象
                if (value is IParamValue)
                {
                    var jipValue = value as IParamValue;
                    if (!string.IsNullOrWhiteSpace(sParamClassName))
                        jipValue.ChangeJavaClass(JClass.ForName(sParamClassName).Handle);
                    return jipValue;
                }

                //if (isArray)
                //{
                //    //未实现
                //    throw new NotSupportedException("通过 JClass.AsCast 转换的参数类型数组，目前还未实现。");
                //}
            }

            #region 反射类型
            if (paramType == typeof(JClass))
                return !isArray ? (JPClass)((JClass)val) : (JPClass)(JClass[])val;
            else if (paramType == typeof(NXDO.RJava.Reflection.JConstructor))
                return !isArray ? (JPConstructor)((NXDO.RJava.Reflection.JConstructor)val) : (JPConstructor)(NXDO.RJava.Reflection.JConstructor[])val;
            else if (paramType == typeof(NXDO.RJava.Reflection.JMethod))
                return !isArray ? (JPMethod)((NXDO.RJava.Reflection.JMethod)val) : (JPMethod)(NXDO.RJava.Reflection.JMethod[])val;
            else if (paramType == typeof(NXDO.RJava.Reflection.JField))
                return !isArray ? (JPField)((NXDO.RJava.Reflection.JField)val) : (JPField)(NXDO.RJava.Reflection.JField[])val;
            #endregion

            #region 基本类型
            //bool
            else if (paramType == typeof(bool))
                return !isArray ? (JPBoolean)(bool)val : (JPBoolean)(bool[])val;
            else if (paramType == typeof(bool?))
                return !isArray ? (JPBoolean)(bool?)val : (JPBoolean)(bool?[])val;

            //byte
            else if (paramType == typeof(byte))
                return !isArray ? (JPByte)(byte)val : (JPByte)(byte[])val;
            else if (paramType == typeof(byte?))
                return !isArray ? (JPByte)(byte?)val : (JPByte)(byte?[])val;

            //char
            else if (paramType == typeof(char))
                return !isArray ? (JPCharacter)(char)val : (JPCharacter)(char[])val;
            else if (paramType == typeof(char?))
                return !isArray ? (JPCharacter)(char?)val : (JPCharacter)(char?[])val;

            //short
            else if (paramType == typeof(short))
                return !isArray ? (JPShort)(short)val : (JPShort)(short[])val;
            else if (paramType == typeof(short?))
                return !isArray ? (JPShort)(short?)val : (JPShort)(short?[])val;

            //int
            else if (paramType == typeof(int))
                return !isArray ? (JPInt)(int)val : (JPInt)(int[])val;
            else if (paramType == typeof(int?))
                return !isArray ? (JPInt)(int?)val : (JPInt)(int?[])val;

            //long
            else if (paramType == typeof(long))
                return !isArray ? (JPLong)(long)val : (JPLong)(long[])val;
            else if (paramType == typeof(long?))
                return !isArray ? (JPLong)(long?)val : (JPLong)(long?[])val;

            //float
            else if (paramType == typeof(float))
                return !isArray ? (JPFloat)(float)val : (JPFloat)(float[])val;
            else if (paramType == typeof(float?))
                return !isArray ? (JPFloat)(float?)val : (JPFloat)(float?[])val;

            //double
            else if (paramType == typeof(double))
                return !isArray ? (JPDouble)(double)val : (JPDouble)(double[])val;
            else if (paramType == typeof(double?))
                return !isArray ? (JPDouble)(double?)val : (JPDouble)(double?[])val;
            #endregion

            //decimal
            else if (paramType == typeof(decimal))
                return !isArray ? (JPDecimal)(decimal)val : (JPDecimal)(decimal[])val;
            else if (paramType == typeof(decimal?))
                return !isArray ? (JPDecimal)(decimal?)val : (JPDecimal)(decimal?[])val;

            #region string/ jobject子类
            //string 
            else if (paramType == typeof(string))
                return !isArray ? (JPString)(string)val : (JPString)(string[])val;

            //JObject 继承类
            else if (paramType.IsSubclassOf(typeof(JObject)))
                return !isArray ? JPSubObject.Create((JObject)val, JClassAttribute.Get(paramType)) :
                                  JPSubObject.Create((JObject[])val, JClassAttribute.Get(paramType));
            #endregion

            

            #region 枚举类型
            else if (isEnum)
            {
                return !isArray ? JPEnum.Create((Enum)val, JClassAttribute.Get(paramType)) :
                                  JPEnum.Create((Array)val, JClassAttribute.Get(paramType));
            }
            #endregion

            else if (paramType == typeof(DateTime) || paramType == typeof(DateTime?))
            {
                return !isArray ? JPDateTime.Create((DateTime?)val, JPDateTime.JavaDateClassName) :
                                  JPDateTime.Create((Array)val, JPDateTime.JavaDateClassName);
            }
            else if (paramType.IsInterface)
            {
                String jInterfaceClassName = JClassAttribute.Get(paramType);
                return !isArray ? JPSubObject.Create((JObject)val, jInterfaceClassName) :
                                  JPSubObject.Create((JObject[])val, jInterfaceClassName);
            }

            #region Object通用类型,泛型
            //JObject 根类, java 方法 xxx(object val), c#方法 xxx(JObject val)
            else if (paramType == typeof(JObject) || paramType == typeof(object) || isGenericParameter)
            {
                var jpObj = JPObject.Create(isArray);
                if (!isArray)
                    jpObj.Add(JBox.CreateBox(val));
                else
                    jpObj.Add(JBox.CreateBoxArray((Array)val));
                return jpObj;
            }
            #endregion

            return null;
        }
        #endregion

        #region 获取 java 默认的方法名称
        /// <summary>
        /// 获取默认的方法名称。
        /// <para>如果某方法未标识 JMethod 特性。</para>
        /// </summary>
        /// <param name="methodName">方法名称</param>
        /// <param name="prefix">方法前缀（主要针对属性 setXXX,getXXX）</param>
        /// <returns></returns>
        internal static string GetDefaultMethodName(string methodName, string prefix = "")
        {
            //prefix = set/get
            //methodName = age
            //return = setAge/getAge

            //prefix = ""
            //methodName = DoSomething
            //return = doSomething
            return (string.IsNullOrWhiteSpace(prefix) ?
                            methodName[0].ToString().ToLower() :
                            prefix + methodName[0].ToString().ToUpper()) +
                       (methodName.Length > 1 ? methodName.Substring(1) : "");
        }
        #endregion
    }
}
