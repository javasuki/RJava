using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;


namespace NXDO.RJava
{
    /// <summary>
    /// 缓存dotnet相关方法（成员变量对应属性）的参数信息，返回值等信息。
    /// </summary>
    class JCacheInfo
    {
        /// <summary>
        /// 缓存
        /// </summary>
        static Dictionary<Type, Dictionary<string, JCacheInfo>> dicDatas = new Dictionary<Type,Dictionary<string,JCacheInfo>>();

        /// <summary>
        /// 是否存在缓存
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        static bool HasCache(Type type, string cacheName)
        {
            bool b = dicDatas.ContainsKey(type);
            if (!b) return false;

            var dic = dicDatas[type];
            if (dic == null) return false;

            return dic.ContainsKey(cacheName);
        }

        /// <summary>
        /// 添加缓存信息
        /// </summary>
        /// <param name="type">JObject继承类</param>
        /// <param name="cacheName">缓存的名称 key</param>
        /// <param name="value">缓存信息</param>
        public static void Add(Type type, string cacheName, JCacheInfo value)
        {
            if (!HasCache(type, cacheName))
            {
                if (!dicDatas.ContainsKey(type))
                {
                    Dictionary<string, JCacheInfo> d = new Dictionary<string, JCacheInfo>();
                    d.Add(cacheName, value);
                    dicDatas.Add(type, d);
                }
                else
                {
                    var d = dicDatas[type];
                    d.Add(cacheName, value);
                    dicDatas[type] = d;
                }
            }
            else
            {
                var d = dicDatas[type];
                d.Add(cacheName, value);
                dicDatas[type] = d;
            }
        }

        /// <summary>
        /// 获取缓存信息
        /// </summary>
        /// <param name="type">JObject继承类</param>
        /// <param name="cacheName">缓存的名称 key</param>
        /// <returns>缓存信息</returns>
        public static JCacheInfo Get(Type type, string cacheName)
        {
            if (!HasCache(type, cacheName)) return null;

            var dic = dicDatas[type];
            return dic[cacheName];
        }

        public JCacheInfo()
        {
        }

        /// <summary>
        /// java 方法(成员变量)名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// java 方法(成员变量)是否静态
        /// </summary>
        public bool IsStatic
        {
            get;
            set;
        }

        /// <summary>
        /// java 方法(成员变量)是否无返回值
        /// <para>方法void好理解。设置成员变量时，其实无返回值，则此为true。</para>
        /// </summary>
        public bool IsVoid
        {
            get;
            set;
        }


        /// <summary>
        /// dotnet 参数列表信息
        /// </summary>
        public List<ParameterInfo> Params
        {
            get
            {
                if (_Params == null)
                    _Params = new List<ParameterInfo>();
                return _Params;
            }
            set
            {
                _Params = value;
            }
        }List<ParameterInfo> _Params;
    }
}
