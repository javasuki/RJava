using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NXDO.RJava.Attributes;

namespace NXDO.RJava.Extension
{
    /// <summary>
    /// JClass扩展，获取基本类型与其它类型对应的 JClass。
    /// <para>其它类型，包括 string，DateTime，JObject继承子类等。</para>
    /// </summary>
    public static class JClassExtension
    {
        /// <summary>
        /// 获取 dotnet 类型所对应 java 类型的申明。
        /// </summary>
        /// <param name="type">dotnet 类型</param>
        /// <returns>dotnet 类型所表示的 JClass 实例。</returns>
        public static JClass ToJavaClass(this Type type)
        {
            #region 基本类型
            if (type == typeof(bool))
                return JClass.ForName("boolean");
            else if (type == typeof(byte))
                return JClass.ForName("byte");
            else if (type == typeof(char))
                return JClass.ForName("char");

            else if (type == typeof(Int16))
                return JClass.ForName("short");
            else if (type == typeof(Int32))
                return JClass.ForName("int");
            else if (type == typeof(Int64))
                return JClass.ForName("long");

            else if (type == typeof(Single))
                return JClass.ForName("float");
            else if (type == typeof(double))
                return JClass.ForName("double");
            #endregion

            #region 基本的引用类型
            else if (type == typeof(bool?))
                return JClass.ForName("java.lang.Boolean");
            else if (type == typeof(byte?))
                return JClass.ForName("java.lang.Byte");
            else if (type == typeof(char?))
                return JClass.ForName("java.lang.Character");

            else if (type == typeof(short?))
                return JClass.ForName("java.lang.Short");
            else if (type == typeof(int?))
                return JClass.ForName("java.lang.Integer");
            else if (type == typeof(long?))
                return JClass.ForName("java.lang.Long");

            else if (type == typeof(float?))
                return JClass.ForName("java.lang.Float");
            else if (type == typeof(double?))
                return JClass.ForName("java.lang.Double");
            #endregion

            #region 基本类型 Array
            else if (type == typeof(bool[]))
                return JClass.ForName("boolean[]");
            else if (type == typeof(byte[]))
                return JClass.ForName("byte[]");
            else if (type == typeof(char[]))
                return JClass.ForName("char[]");

            else if (type == typeof(Int16[]))
                return JClass.ForName("short[]");
            else if (type == typeof(Int32[]))
                return JClass.ForName("int[]");
            else if (type == typeof(Int64[]))
                return JClass.ForName("long[]");

            else if (type == typeof(Single[]))
                return JClass.ForName("float[]");
            else if (type == typeof(double[]))
                return JClass.ForName("double[]");
            #endregion

            #region 基本的引用类型 Array
            else if (type == typeof(bool?[]))
                return JClass.ForName("[Ljava.lang.Boolean;");
            else if (type == typeof(byte?[]))
                return JClass.ForName("[Ljava.lang.Byte;");
            else if (type == typeof(char?[]))
                return JClass.ForName("[Ljava.lang.Character;");

            else if (type == typeof(short?[]))
                return JClass.ForName("[Ljava.lang.Short;");
            else if (type == typeof(int?[]))
                return JClass.ForName("[Ljava.lang.Integer;");
            else if (type == typeof(long?[]))
                return JClass.ForName("[Ljava.lang.Long;");

            else if (type == typeof(float?[]))
                return JClass.ForName("[Ljava.lang.Float;");
            else if (type == typeof(double?[]))
                return JClass.ForName("[Ljava.lang.Double;");
            #endregion

            #region string/date/JObject
            else if (type == typeof(string))
                return JClass.ForName("java.lang.String");

            else if (type == typeof(DateTime))
                return JClass.ForName("java.util.Date");

            else if (type == typeof(JObject) || type == typeof(object))
                return JClass.ForName("java.lang.Object");

            else if (type.IsSubclassOf(typeof(JObject)))
                return JClass.ForName(JClassAttribute.Get(type));


            #endregion 


            #region string/date/JObject 数组
            else if (type == typeof(string[]))
                return JClass.ForName("[Ljava.lang.String;");

            else if (type == typeof(DateTime))
                return JClass.ForName("[Ljava.util.Date;");

            else if (type.IsArray)
            {
                var eType = type.GetElementType();
                if (eType == typeof(JObject) || eType == typeof(object))
                    return JClass.ForName("[Ljava.lang.Object;");

                else if (eType.IsSubclassOf(typeof(JObject)))
                    return JClass.ForName("[L" + JClassAttribute.Get(type) + ";");
            }
            #endregion

            return null;
        }
    }
}
