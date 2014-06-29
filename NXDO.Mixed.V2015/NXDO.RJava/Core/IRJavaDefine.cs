using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NXDO.RJava.Core
{
    /// <summary>
    /// 支持 JObject 转换成程序集内已定义的实现某接口的类（包含内部类）实例。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRJavaDefine<T>
    {
        /// <summary>
        /// 获取转换后的一个已知类型或接口。
        /// </summary>
        /// <param name="jobject">JObject 实例</param>
        /// <returns>某一个已知类型或接口。</returns>
        T GetConvertToDefine(JObject jobject);
    }
}
