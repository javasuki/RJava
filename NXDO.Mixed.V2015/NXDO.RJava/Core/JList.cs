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
    /// 表示为 java.util.List&lt;?&gt; 接口声明，并继承了 System.Collections.Generic.IList&lt;T&gt; 接口。
    /// <para>C#,  JList&lt;int&gt; jList = new List&lt;int&gt;().ToJavaList();   //扩展方法</para>
    /// </summary>
    /// <typeparam name="T">元素类型</typeparam>   
    public interface JList<T> : JIBase, IList<T>, IRJavaDefine<JList<T>>, IDisposable
    {
    }

    [JClass("java.util.ArrayList")]
    internal class JavaList<T> : JObject, JList<T>, IParamValue
    {
        public JList<T> GetConvertToDefine(JObject jobject)
        {            
            var unknow = jobject as JUnknown;
            if (unknow == null)
                throw new ArgumentException("提供参数无效，无法与已定义类型之间进行转换。", "jobject");

            this.Clear();
            this.Handle = jobject.Handle;
            this.JClass = jobject.GetClass().Handle;

            var newList = Activator.CreateInstance(this.GetType()) as JList<T>;
            foreach(T t in this){
                newList.Add(t);
            }
            return newList;
        }

        public JavaList()
        {
            JSuper();
            this.JClass = JType.ForName("java.util.List").Handle;
            this.JValue = this.Handle;

            //List为接口，实际初始化了 ArrayList
            //List list = new java.util.ArrayList();
        }

        protected JavaList(IntPtr objectPtr, IntPtr classPtr)
            : base(objectPtr, classPtr)
        {
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JType listClass;
        override  public JType GetClass()
        {
            if (listClass == null)
                listClass = new JClass(this.JClass, "java.util.List");
            return listClass;
        }


        #region IParamValue
        /// <summary>
        /// 如果没有改变，则为 List 接口的类型
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IntPtr JClass
        {
            get;
            internal set;
        }

        /// <summary>
        /// ArrayList 对象实例
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

        #region IList<T>
        public int IndexOf(T item)
        {
            var jpv = JInvokeHelper.CreateJParamValue(typeof(JObject), item);
            var ptr = JObject.JContext.JInvoke(this.Handle, "indexOf", JParamValue.GetParams(jpv));
            return new JMReturn<int>(ptr).Value;
        }

        public void Insert(int index, T item)
        {
            var jpv1 = JInvokeHelper.CreateJParamValue(typeof(int), index);
            var jpv2 = JInvokeHelper.CreateJParamValue(typeof(JObject), item);
            JObject.JContext.JInvoke(this.Handle, "add", JParamValue.GetParams(jpv1, jpv2));
        }

        public void RemoveAt(int index)
        {
            var jpv = JInvokeHelper.CreateJParamValue(typeof(int), index);
            JObject.JContext.JInvoke(this.Handle, "remove", JParamValue.GetParams(jpv));
        }

        public T this[int index]
        {
            get
            {
                var jpv = JInvokeHelper.CreateJParamValue(typeof(int), index);
                var ptr = JObject.JContext.JInvoke(this.Handle, "get", JParamValue.GetParams(jpv));
                return new JMReturn<T>(ptr).Value;
            }
            set
            {
                var jpv1 = JInvokeHelper.CreateJParamValue(typeof(int), index);
                var jpv2 = JInvokeHelper.CreateJParamValue(typeof(JObject), value);
                JObject.JContext.JInvoke(this.Handle, "set", JParamValue.GetParams(jpv1, jpv2));
            }
        }

        public void Add(T item)
        {
            var jpv = JInvokeHelper.CreateJParamValue(typeof(JObject), item);
            JObject.JContext.JInvoke(this.Handle, "add", JParamValue.GetParams(jpv));
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

        public bool Remove(T item)
        {
            var jpv = JInvokeHelper.CreateJParamValue(typeof(object), item);
            var ptr = JObject.JContext.JInvoke(this.Handle, "remove", JParamValue.GetParams(jpv));
            return new JMReturn<bool>(ptr).Value;
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
