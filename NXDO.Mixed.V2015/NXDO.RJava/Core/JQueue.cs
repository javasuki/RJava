using System;
using System.Collections;
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
    /// 表示为 java.util.Queue&lt;?&gt; 接口声明, 对象的先进先出集合。
    /// <para>C#, JQueue&lt;int&gt; jQueue = new Queue&lt;int&gt;().ToJavaQueue();</para>
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>
    public interface JQueue<T> : JIBase, IEnumerable<T>, IEnumerable, IRJavaDefine<JQueue<T>>, IDisposable
    {
        /// <summary>
        /// 获取集合中包含的元素数。
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 从集合中移除所有对象。
        /// </summary>
        void Clear();

        /// <summary>
        /// 确定某元素是否在集合中。
        /// </summary>
        /// <param name="item">元素</param>
        /// <returns>如果在集合找到 item，则为 true；否则为 false。</returns>
        bool Contains(T item);

        /// <summary>
        /// 从指定数组索引开始将集合元素复制到现有一维 System.Array 中。
        /// </summary>
        /// <param name="array">作为从集合复制的元素的目标位置的一维 System.Array</param>
        /// <param name="arrayIndex">array 中从零开始的索引，将在此处开始复制。</param>
        void CopyTo(T[] array, int arrayIndex);

        /// <summary>
        /// 移除并返回位于集合开始处的对象。
        /// </summary>
        /// <returns></returns>
        T Dequeue();

        /// <summary>
        /// 将对象添加到集合的结尾处。
        /// </summary>
        /// <param name="item">要添加到集合的对象。</param>
        void Enqueue(T item);

        /// <summary>
        /// 返回位于集合开始处的对象但不将其移除。
        /// </summary>
        /// <returns>位于集合的开头的对象。</returns>
        T Peek();

        /// <summary>
        /// 将集合元素复制到新数组。
        /// </summary>
        /// <returns>包含从集合复制的元素的新数组。</returns>
        T[] ToArray();
    }

    [JClass("java.util.LinkedList")]
    internal class JavaQueue<T> : JObject, JQueue<T>, IParamValue
    {
        public JQueue<T> GetConvertToDefine(JObject jobject)
        {            
            var unknow = jobject as JUnknown;
            if (unknow == null)
                throw new ArgumentException("提供参数无效，无法与已定义类型之间进行转换。", "jobject");
            
            var self = Activator.CreateInstance(this.GetType()) as JavaQueue<T>;

            this.Clear();
            this.Handle = jobject.Handle;
            this.JClass = jobject.GetClass().Handle;            
            foreach(T t in this)
            {
                self.Enqueue(t);
            }
            return self;
        }

        public JavaQueue()
        {
            JSuper();
            this.JClass = JType.ForName("java.util.Queue").Handle;
            this.JValue = this.Handle;
        }

        protected JavaQueue(IntPtr objectPtr, IntPtr classPtr)
            : base(objectPtr, classPtr)
        {
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JType listClass;
        override  public JType GetClass()
        {
            if (listClass == null)
                listClass = new JClass(this.JClass, "java.util.Queue");
            return listClass;
        }


        #region IParamValue
        /// <summary>
        /// 如果没有改变，则为 Queue 接口的类型
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IntPtr JClass
        {
            get;
            internal set;
        }

        /// <summary>
        /// LinkedList 对象实例
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IntPtr JValue
        {
            get;
            set;
        }

        public void ChangeJavaClass(IntPtr javaClassPtr)
        {
            this.JClass = javaClassPtr;
        }
        #endregion

        #region JQueue
        public int Count
        {
            get
            {
                var ptr = JObject.JContext.JInvoke(this.Handle, "size", JParamValue.GetParams());
                return new JMReturn<int>(ptr).Value;
            }
        }

        public void Clear()
        {
            JObject.JContext.JInvoke(this.Handle, "clear", JParamValue.GetParams());
        }

        public bool Contains(T item)
        {
            var jpv = JInvokeHelper.CreateJParamValue(typeof(JObject), item);
            var ptr = JObject.JContext.JInvoke(this.Handle, "contains", JParamValue.GetParams(jpv));
            return new JMReturn<bool>(ptr).Value;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            var ptr = JObject.JContext.JInvoke(this.Handle, "toArray", JParamValue.GetParams());
            JObject[] rt = new JMReturn<JObject[]>(ptr).Value;
            var lst = rt.ToDotValue<T>().ToList();
            lst.CopyTo(array, arrayIndex);       
        }

        public T Dequeue()
        {
            //移除并返回位于 System.Collections.Generic.Queue<T> 开始处的对象。
            var ptr = JObject.JContext.JInvoke(this.Handle, "poll", JParamValue.GetParams());
            return new JMReturn<T>(ptr).Value;
        }

        public void Enqueue(T item)
        {
            //将对象添加到 System.Collections.Generic.Queue<T> 的结尾处。
            var jpv = JInvokeHelper.CreateJParamValue(typeof(JObject), item);
            JObject.JContext.JInvoke(this.Handle, "offer", JParamValue.GetParams(jpv));
        }

        public T Peek()
        {
            //返回位于 System.Collections.Generic.Queue<T> 开始处的对象但不将其移除。
            var ptr = JObject.JContext.JInvoke(this.Handle, "peek", JParamValue.GetParams());
            return new JMReturn<T>(ptr).Value;
        }

        public T[] ToArray()
        {
            var ptr = JObject.JContext.JInvoke(this.Handle, "toArray", JParamValue.GetParams());
            JObject[] rt = new JMReturn<JObject[]>(ptr).Value;
            var lst = rt.ToDotValue<T>().ToList();
            return lst.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            var itorHandle = JObject.JContext.JInvoke(this.Handle, "iterator", JParamValue.GetParams());
            while (true)
            {
                var hasNextPtr = JObject.JContext.JInvoke(itorHandle, "hasNext", JParamValue.GetParams());
                bool hasNext = new JMReturn<bool>(hasNextPtr).Value;
                if (!hasNext) break;

                var resultHandle = JObject.JContext.JInvoke(itorHandle, "next", JParamValue.GetParams());
                yield return new JMReturn<T>(resultHandle).Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
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
