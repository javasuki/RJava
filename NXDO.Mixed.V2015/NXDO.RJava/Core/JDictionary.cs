using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using JType = NXDO.RJava.JClass;
using NXDO.RJava.Attributes;
using NXDO.RJava.Extension;

namespace NXDO.RJava.Core
{
    /// <summary>
    /// 表示为 java.util.Map&lt;K, V&gt; 接口声明，并继承了 System.Collections.Generic.IDictionary&lt;TKey, TValue&gt; 接口。
    /// <para>(JDK1.6) java.util.Dictionary&lt;K, V&gt; 已过期，推荐使用 java.util.Map&lt;K, V&gt;。</para>
    /// <para>.</para>
    /// <para>C#, JDictionary&lt;int, string&gt; jDictionary = new Dictionary&lt;int,string&gt;().ToJavaDictionary();</para>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface JDictionary<TKey, TValue> : JIBase, IDictionary<TKey, TValue>, IRJavaDefine<JDictionary<TKey, TValue>>, IDisposable
    {
        /// <summary>
        /// 确定 JDictionary&lt;TKey,TValue&gt; 是否包含具有指定元素。
        /// <para>java.util.Map&lt;K,V&gt;.containsValue(Object value)</para>
        /// </summary>
        /// <param name="value">指定元素</param>
        /// <returns>如果包含带有该值的元素，则为 true；否则，为false。</returns>
        bool ContainsValue(TValue value);
    }

    [JClass("java.util.HashMap")]
    internal class JavaDictionary<TKey, TValue> : JObject, JDictionary<TKey, TValue>, IParamValue
    {
        #region ctor/getclass
        public JavaDictionary()
        {
            JSuper();
            this.JClass = JType.ForName("java.util.Map").Handle;
            this.JValue = this.Handle;

            //Map为接口，实际初始化了 HashMap
            //Map map = new java.util.HashMap();
        }

        protected JavaDictionary(IntPtr objectPtr, IntPtr classPtr)
            : base(objectPtr, classPtr)
        {
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JType dicClass;
        override  public JType GetClass()
        {
            if (dicClass == null)
                dicClass = new JClass(this.JClass, "java.util.Map");
            return dicClass;
        }
        #endregion

        public JDictionary<TKey, TValue> GetConvertToDefine(JObject jobject)
        {
            var unknow = jobject as JUnknown;
            if (unknow == null)
                throw new ArgumentException("提供参数无效，无法与已定义类型之间进行转换。", "jobject");

            var self = Activator.CreateInstance(this.GetType()) as JavaDictionary<TKey, TValue>;

            this.Clear();
            this.Handle = jobject.Handle;
            this.JClass = jobject.GetClass().Handle;

            foreach (var kv in this)
            {
                self.Add(kv);
            }
            return self;
        }

        #region IParamValue
        public void ChangeJavaClass(IntPtr javaClassPtr)
        {
            this.JClass = javaClassPtr;
        }

        /// <summary>
        /// 如果没有改变，则为 Map 接口的类型
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IntPtr JClass
        {
            get;
            internal set;
        }

        /// <summary>
        /// HashMap 对象实例
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IntPtr JValue
        {
            get;
            set;
        }
        #endregion

        public bool ContainsValue(TValue value)
        {
            var jpv1 = JInvokeHelper.CreateJParamValue(typeof(JObject), value);
            var ptr = JObject.JContext.JInvoke(this.Handle, "containsValue", JParamValue.GetParams(jpv1));
            return new JMReturn<bool>(ptr).Value;
        }

        #region IDictionary<TKey, TValue>
        public void Add(TKey key, TValue value)
        {
            var jpv1 = JInvokeHelper.CreateJParamValue(typeof(JObject), key);
            var jpv2 = JInvokeHelper.CreateJParamValue(typeof(JObject), value);
            JObject.JContext.JInvoke(this.Handle, "put", JParamValue.GetParams(jpv1, jpv2));
        }

        public bool ContainsKey(TKey key)
        {
            var jpv1 = JInvokeHelper.CreateJParamValue(typeof(JObject), key);
            var ptr = JObject.JContext.JInvoke(this.Handle, "containsKey", JParamValue.GetParams(jpv1));
            return new JMReturn<bool>(ptr).Value;
        }

        public ICollection<TKey> Keys
        {
            get 
            {
                List<TKey> keys = new List<TKey>();
                var setKeysHandle = JObject.JContext.JInvoke(this.Handle, "keySet", JParamValue.GetParams());
                var aryPtr = JObject.JContext.JInvoke(setKeysHandle, "toArray", JParamValue.GetParams());
                var joAry = new JMReturn<JObject[]>(aryPtr).Value;
                foreach (var jo in joAry)
                {
                    keys.Add(jo.ToDotValue<TKey>());
                }
                return keys;
            }
        }

        public bool Remove(TKey key)
        {
            var jpv1 = JInvokeHelper.CreateJParamValue(typeof(JObject), key);
            JObject.JContext.JInvoke(this.Handle, "remove", JParamValue.GetParams(jpv1));
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue); ;
            var jpv1 = JInvokeHelper.CreateJParamValue(typeof(JObject), key);
            var ptr = JObject.JContext.JInvoke(this.Handle, "get", JParamValue.GetParams(jpv1));
            if (ptr == null)
                return false;
            value = new NXDO.RJava.Core.JMReturn<TValue>(ptr).Value;
            return true;
        }

        public ICollection<TValue> Values
        {
            get
            {
                List<TValue> values = new List<TValue>();
                var valuesHandle = JObject.JContext.JInvoke(this.Handle, "values", JParamValue.GetParams());
                var aryPtr = JObject.JContext.JInvoke(valuesHandle, "toArray", JParamValue.GetParams());
                var joAry = new JMReturn<JObject[]>(aryPtr).Value;
                foreach (var jo in joAry)
                {
                    values.Add(jo.ToDotValue<TValue>());
                }
                return values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue v = default(TValue);
                TryGetValue(key, out v);
                return v;
            }
            set
            {
                var jpv1 = JInvokeHelper.CreateJParamValue(typeof(JObject), key);
                var jpv2 = JInvokeHelper.CreateJParamValue(typeof(JObject), value);
                JObject.JContext.JInvoke(this.Handle, "put", JParamValue.GetParams(jpv1, jpv2));
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            JObject.JContext.JInvoke(this.Handle, "clear", JParamValue.GetParams());
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            //键，值都不存在，为 false
            if (!this.ContainsKey(item.Key)) return false;
            if (!this.ContainsValue(item.Value)) return false;

            TValue v1 = this[item.Key];
            TValue v2 = item.Value;
            return v1.Equals(v2);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            List<KeyValuePair<TKey, TValue>> lst = new List<KeyValuePair<TKey, TValue>>();
            var setHandle = JObject.JContext.JInvoke(this.Handle, "entrySet", JParamValue.GetParams());
            var ptr = JObject.JContext.JInvoke(setHandle, "toArray", JParamValue.GetParams());
            JObject[] entries = new JMReturn<JObject[]>(ptr).Value;
            foreach (var jo in entries)
            {
                var k = JObject.JContext.JInvoke(jo.Handle, "getKey", JParamValue.GetParams());
                var v = JObject.JContext.JInvoke(jo.Handle, "getValue", JParamValue.GetParams());
                var kvp = new KeyValuePair<TKey, TValue>(new JMReturn<TKey>(k).Value, new JMReturn<TValue>(v).Value);
                lst.Add(kvp);
            }
            lst.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                var ptr = JObject.JContext.JInvoke(this.Handle, "size", JParamValue.GetParams());
                return new JMReturn<int>(ptr).Value;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return this.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            // Set<Entry<Integer, String>> sets = map.entrySet();
            var setsHandle = JObject.JContext.JInvoke(this.Handle, "entrySet", JParamValue.GetParams());

            //Iterator<Entry<Integer, String>> itor = sets.iterator();
            var itorHandle = JObject.JContext.JInvoke(setsHandle, "iterator", JParamValue.GetParams());     

            //itor.hasNext()
            while (true)
            {
                var hasNextPtr = JObject.JContext.JInvoke(itorHandle, "hasNext", JParamValue.GetParams());
                bool hasNext = new JMReturn<bool>(hasNextPtr).Value;
                if (!hasNext) break;

                //Entry<Integer, String> r = itor.next();
                var resultHandle = JObject.JContext.JInvoke(itorHandle, "next", JParamValue.GetParams());

                //r.getKey, r.getValue
                var k = JObject.JContext.JInvoke(resultHandle, "getKey", JParamValue.GetParams());
                var v = JObject.JContext.JInvoke(resultHandle, "getValue", JParamValue.GetParams());
                yield return new KeyValuePair<TKey, TValue>(new JMReturn<TKey>(k).Value, new JMReturn<TValue>(v).Value);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion

        public override void Dispose()
        {
            this.Clear();
            base.Dispose();
        }
    }
}
