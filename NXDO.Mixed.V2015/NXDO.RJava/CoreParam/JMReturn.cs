using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NXDO.RJava.Core
{
    /// <summary>
    /// 转换 java 方法的返回值。
    /// <para>返回类型不明确的，使用根对象 JObject 或 object。</para>
    /// </summary>
    /// <typeparam name="T">转换成的类型</typeparam>
    internal class JMReturn<T> //: JUnknown //, IJReturn
    {
        static Type JObjectType = typeof(JObject);
        /// <summary>
        /// 转换java方法的返回值
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="jobjectReturn">有值时：作为JObjectReturn已返回到C#用户代码，再次通过 ToJavaValue 方法进行转换，全部以java对象做处理</param>
        public JMReturn(IntPtr ptr, JUnknown jobjectReturn = null) //: base(ptr, typeof(T), typeof(JObject))
        {
            if (jobjectReturn == null)
            {
                this.ResultPtr = ptr;
                this.ElemType = typeof(T);
                this.IsArray = this.ElemType.IsArray;
                if (this.IsArray)
                    this.ElemType = this.ElemType.GetElementType();
                this.IsSubclassOf = this.ElemType.IsSubclassOf(JMReturn<T>.JObjectType);
                return;
            }

            this.JResultReturn = jobjectReturn;
            this.ResultPtr = jobjectReturn.Handle;
            this.ElemType = typeof(T);
            this.IsArray = this.ElemType.IsArray;
            if (this.IsArray)
                this.ElemType = this.ElemType.GetElementType();
            this.IsSubclassOf = this.ElemType.IsSubclassOf(JMReturn<T>.JObjectType);
        }

        #region 属性
        private IntPtr ResultPtr
        {
            get;
            set;
        }

        private bool IsArray
        {
            get;
            set;
        }

        private Type ElemType
        {
            get;
            set;
        }

        private bool IsSubclassOf
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// 再次通过 ToDotValue 进行转换的原始对象
        /// </summary>
        private JUnknown JResultReturn
        {
            get;
            set;
        }

        public T Value
        {
            get
            {
                if (this.ResultPtr == IntPtr.Zero)
                    return default(T);

                if (this.ElemType == JObjectType || this.ElemType == typeof(Object))
                {
                    string jclassName = JReturnValueHelper.GetClassName(this.ResultPtr);
                    //bool isPrimitive = JReturnValueHelper.CheckIsPrimitive(this.ResultPtr);

                    Object jo = null;
                    if (!IsArray)
                        return (T)((object)new JUnknown(this.ResultPtr, jclassName));
                    else
                    {
                        int size = JReturnValueHelper.GetArraySize(this.ResultPtr);
                        var ary = new JUnknown[size];
                        for (int i = 0; i < size; i++)
                        {
                            IntPtr aryValuePtr = JReturnValueHelper.GetArrayElem(this.ResultPtr, i);
                            ary[i] = new JUnknown(aryValuePtr, jclassName);
                        }
                        jo = ary;
                    }
                    return (T)jo;
                }
                else
                {
                    if (!IsArray)
                        return JReturnValueHelper.GetObjectValue<T>(this.ResultPtr, this.IsSubclassOf);
                    else
                        return JReturnValueHelper.GetObjectArray<T>(this.ResultPtr, this.IsSubclassOf);
                }

            }
        }
    }
}
