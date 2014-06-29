using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NXDO.RJava.Core;

namespace NXDO.RJava.Reflection
{
    /// <summary>
    /// 发现 java 字段特性并提供对 java 字段元数据的访问权。
    /// </summary>
    [DebuggerDisplay("Name = {Name}, IsPublic = {IsPublic}")]
    public class JField
    {
        internal JField(IntPtr handle, string fieldName)
        {
            this.Handle = handle;
            this.Name = fieldName;

            bool[] boolAry = JRunEnvironment.ReflectionHelper.GetFieldModifier(handle);
            this.IsFinal = boolAry[0];
            this.IsPrivate = boolAry[1];
            this.IsPublic = boolAry[2];
            this.IsStatic = boolAry[3];
        }

        /// <summary>
        /// java 方法指针
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal IntPtr Handle
        {
            get;
            private set;
        }

        /// <summary>
        /// 成员名称
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        #region 标志属性
        /// <summary>
        /// 获取一个值，该值指示此方法是否为 final。
        /// </summary>
        public bool IsFinal
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取一个值，该值指示此成员是否是私有的。
        /// </summary>
        public bool IsPrivate
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取一个值，该值指示这是否是一个公共方法。
        /// </summary>
        public bool IsPublic
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取一个值，该值指示方法是否为 static。
        /// </summary>
        public bool IsStatic
        {
            get;
            private set;
        }
        #endregion

        IntPtr getJavaValue(JObject obj, ref bool isArray)
        {
            IntPtr ownerPtr = obj == null ? IntPtr.Zero : obj.Handle;
            IntPtr ptr = JRunEnvironment.ReflectionHelper.GetFieldValue(this.Handle, ownerPtr);
            isArray = JRunEnvironment.ReflectionHelper.CheckIsArray(ptr);
            return ptr;
        }

        /// <summary>
        /// 返回给定对象支持的字段的值。
        /// </summary>
        /// <param name="obj">其字段所属的对象。静态字段时，此值设为 null。</param>
        /// <returns>字段的值</returns>
        public object GetValue(JObject obj)
        {
            bool resultIsArray = false;
            var ptr = this.getJavaValue(obj, ref resultIsArray);
            if (!resultIsArray)
                return new JMReturn<JObject>(ptr).Value;
            return new JMReturn<JObject[]>(ptr).Value;
        }

        /// <summary>
        /// 返回给定对象支持的字段的值。
        /// </summary>
        /// <typeparam name="T">字段值的类型。</typeparam>
        /// <param name="obj">其字段所属的对象。静态字段时，此值设为 null。</param>
        /// <returns>T类型的字段值。</returns>
        public T GetValue<T>(JObject obj)
        {
            bool resultIsArray = false;
            var ptr = this.getJavaValue(obj, ref resultIsArray);
            return new JMReturn<T>(ptr).Value;
        }

        /// <summary>
        /// 设置给定对象支持的字段值。
        /// <para>TODO:目前不支持数组传入。</para>
        /// </summary>
        /// <param name="obj">其字段所属的对象。静态字段时，此值设为 null。</param>
        /// <param name="value">分配给字段的值。</param>
        public void SetValue(JObject obj, Object value)
        {
            IntPtr ownerPtr = obj != null ? obj.Handle : IntPtr.Zero;

            bool isArray = false;
            Type type = typeof(JObject);
            if (value != null)
            {
                isArray = value.GetType().IsArray;
                if (isArray)
                    type = typeof(JObject[]);
            }
            var val = JInvokeHelper.CreateJParamValue(type, value);

            JRunEnvironment.ReflectionHelper.SetFieldValue(this.Handle, ownerPtr, val);
        }
    }
}
