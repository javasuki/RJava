using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NXDO.RJava.Extension;

namespace NXDO.RJava
{
    /// <summary>
    /// 获取 dotnet 基本类型与其它类型对应的 JClass。
    /// <para>其它类型，包括 string，DateTime，JObject继承子类等。</para>
    /// </summary>
    /// <typeparam name="T">dotnet类型</typeparam>
    public static class JClass<T>
    {
        /// <summary>
        /// dotnet 类型所表示的 JClass 实例。
        /// </summary>
        public static JClass Class
        {
            get
            {
                return typeof(T).ToJavaClass();
            }
        }
    }
}
