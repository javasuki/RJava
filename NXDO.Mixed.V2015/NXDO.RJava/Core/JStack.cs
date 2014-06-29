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
    /// 表示为 java.util.Stack&lt;?&gt; 类型声明, 大小可变的后进先出 (LIFO) 集合。
    /// <para>C#, JStack&lt;int&gt; jStack = new Stack&lt;int&gt;().ToJavaStack();</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface JStack<T> : JIBase, IEnumerable<T>, IEnumerable, IRJavaDefine<JStack<T>>, IDisposable
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
        /// 返回位于集合顶部的对象但不将其移除。
        /// </summary>
        /// <returns>位于集合顶部的对象</returns>
        T Peek();

        /// <summary>
        /// 移除并返回位于集合顶部的对象。
        /// </summary>
        /// <returns>位于集合顶部的对象</returns>
        T Pop();

        /// <summary>
        /// 将对象插入集合的顶部。
        /// </summary>
        /// <param name="item">要推入到集合中的对象。</param>
        void Push(T item);

        /// <summary>
        /// 将集合元素复制到新数组。
        /// </summary>
        /// <returns>包含从集合复制的元素的新数组。</returns>
        T[] ToArray();
    }

    [JClass("java.util.Stack")]
    internal class JavaStack<T> : JObject, JStack<T>, IParamValue
    {
        public JStack<T> GetConvertToDefine(JObject jobject)
        {
            var unknow = jobject as JUnknown;
            if (unknow == null)
                throw new ArgumentException("提供参数无效，无法与已定义类型之间进行转换。", "jobject");

            var self = Activator.CreateInstance(this.GetType()) as JavaStack<T>;

            this.Clear();
            this.Handle = jobject.Handle;
            this.JClass = jobject.GetClass().Handle;

            var lst = this.ToList();
            lst.Reverse();
            foreach (T t in lst)
            {
                self.Add(t);
            }

            return self;
        }

        public JavaStack()
        {
            JSuper();
            this.JClass = JType.ForName("java.util.Stack").Handle;
            this.JValue = this.Handle;
        }

        protected JavaStack(IntPtr objectPtr, IntPtr classPtr)
            : base(objectPtr, classPtr)
        {
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JType listClass;
        override  public JType GetClass()
        {
            if (listClass == null)
                listClass = new JClass(this.JClass, "java.util.Stack");
            return listClass;
        }

        #region IParamValue
        /// <summary>
        /// Stack 的类型
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IntPtr JClass
        {
            get;
            internal set;
        }

        /// <summary>
        /// Stack 对象实例
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

        #region JStack
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

        public T Peek()
        {
            var ptr = JObject.JContext.JInvoke(this.Handle, "peek", JParamValue.GetParams());
            return new JMReturn<T>(ptr).Value;
        }

        public T Pop()
        {
            var ptr = JObject.JContext.JInvoke(this.Handle, "pop", JParamValue.GetParams());
            return new JMReturn<T>(ptr).Value;
        }

        public void Push(T item)
        {
            var jpv = JInvokeHelper.CreateJParamValue(typeof(JObject), item);
            JObject.JContext.JInvoke(this.Handle, "push", JParamValue.GetParams(jpv));
        }

        public T[] ToArray()
        {
            var ptr = JObject.JContext.JInvoke(this.Handle, "toArray", JParamValue.GetParams());
            JObject[] rt = new JMReturn<JObject[]>(ptr).Value;
            return rt.ToDotValue<T>().ToArray();
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

        internal void Add(T item)
        {
            var jpv = JInvokeHelper.CreateJParamValue(typeof(JObject), item);
            JObject.JContext.JInvoke(this.Handle, "add", JParamValue.GetParams(jpv));
        }

        public override void Dispose()
        {
            this.Clear();
            base.Dispose();
        }
    }
}
