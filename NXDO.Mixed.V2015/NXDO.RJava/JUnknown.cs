using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using NXDO.RJava.Core;

namespace NXDO.RJava
{
    /// <summary>
    /// java明确类型，dotnet中类型不明确的，以 JObject，object 为返回值或参数的包装器。
    /// <para>仅当方法返回值或参数显示定义为 JObject，object 时，才有效。</para>
    /// </summary>
    [DebuggerDisplay("java = {jclassName}, hasValue = {IsNullValue}")]
    [DebuggerTypeProxy(typeof(JObject))]
    class JUnknown : JObject, IParamValue
    {
        //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        //IntPtr ptr;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string jclassName;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool IsNullValue;

        public JUnknown(IntPtr ptr, String javaClassName)
            : base(ptr, NXDO.RJava.JClass.ForName(javaClassName).Handle)
        {
            this.JValue = ptr;
            this.jclassName = javaClassName;
            this.IsNullValue = ptr != IntPtr.Zero;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JClass _jclass;
        public override JClass GetClass()
        {
            if (_jclass == null)
            {
                if (string.IsNullOrWhiteSpace(this.jclassName))
                    _jclass = JObject.Class;
                _jclass = NXDO.RJava.JClass.ForName(this.jclassName);
            }
            return _jclass;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IntPtr JClass
        {
            get { return this.GetClass().Handle; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IntPtr JValue
        {
            get;
            set;
        }

        public void ChangeJavaClass(IntPtr javaClassPtr)
        {
            _jclass = new JClass(javaClassPtr);
            this.jclassName = _jclass.FullName;
        }


        #region unbox
        /// <summary>
        /// bool拆箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns>dotnet bool值</returns>
        [DebuggerNonUserCode]
        public static implicit operator bool(JUnknown value)
        {
            return new JMReturn<bool>(IntPtr.Zero, value).Value;
        }

        /// <summary>
        /// byte拆箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns>dotnet byte值</returns>
        [DebuggerNonUserCode]
        public static implicit operator byte(JUnknown value)
        {
            return new JMReturn<byte>(IntPtr.Zero, value).Value;
        }

        /// <summary>
        /// char拆箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns>dotnet char值</returns>
        [DebuggerNonUserCode]
        public static implicit operator char(JUnknown value)
        {
            return new JMReturn<char>(IntPtr.Zero, value).Value;
        }

        /// <summary>
        /// short拆箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns>dotnet short值</returns>
        [DebuggerNonUserCode]
        public static implicit operator short(JUnknown value)
        {
            return new JMReturn<short>(IntPtr.Zero, value).Value;
        }

        /// <summary>
        /// int拆箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns>dotnet int值</returns>
        [DebuggerNonUserCode]
        public static implicit operator int(JUnknown value)
        {
            return new JMReturn<int>(IntPtr.Zero, value).Value;
        }

        /// <summary>
        /// long拆箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns>dotnet long值</returns>
        [DebuggerNonUserCode]
        public static implicit operator long(JUnknown value)
        {
            return new JMReturn<long>(IntPtr.Zero, value).Value;
        }

        /// <summary>
        /// float拆箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns>dotnet float值</returns>
        [DebuggerNonUserCode]
        public static implicit operator float(JUnknown value)
        {
            return new JMReturn<float>(IntPtr.Zero, value).Value;
        }

        /// <summary>
        /// double拆箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns>dotnet double值</returns>
        [DebuggerNonUserCode]
        public static implicit operator double(JUnknown value)
        {
            return new JMReturn<double>(IntPtr.Zero, value).Value;
        }

        /// <summary>
        /// string拆箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns>dotnet string值</returns>
        [DebuggerNonUserCode]
        public static implicit operator string(JUnknown value)
        {
            return new JMReturn<string>(IntPtr.Zero, value).Value;
        }
        #endregion
    }

    #region JUnknowArray,注释，java不支持 new ArrayList<?>[] 数组
    ///// <summary>
    ///// 作为参数传入到 java 方法中，已知 java 类型的数组
    ///// </summary>
    //[DebuggerDisplay("java = {jclassName}, size = {size}")]
    //class JUnknowArray : JObject, IParamValue
    //{
    //    private JUnknowArray(int size, string elemName)
    //    {
    //        this.size = size;
    //        this.jclassName = "[L" + elemName + ";";
    //    }

    //    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    //    string jclassName;
    //    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    //    int size;

    //    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    //    private JClass _jclass;
    //    public override JClass GetClass()
    //    {
    //        if (_jclass == null)            
    //            _jclass = new JClass(this.JClass);
    //        return _jclass;
    //    }

    //    public void ChangeJavaClass(IntPtr javaClassPtr)
    //    {
    //        this.JClass = javaClassPtr;
    //        this._jclass = new JClass(this.JClass);
    //    }

    //    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    //    public IntPtr JClass
    //    {
    //        get;
    //        internal set;
    //    }

    //    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    //    public IntPtr JValue
    //    {
    //        get;
    //        set;
    //    }

    //    public static JUnknowArray CreateArray(string javaElemArrayClassName, JIBase[] objs)
    //    {
    //        int size = objs.Length;
    //        JUnknowArray jAry = new JUnknowArray(size, javaElemArrayClassName);

            
    //        var cls = NXDO.RJava.JClass.ForName(javaElemArrayClassName);
    //        jAry.JValue = JParamValueHelper.CreateObjectArray(cls.Handle, size);
    //        if (size == 0) return jAry;

    //        int idx = 0;
    //        foreach (var jo in objs)
    //        {
    //            var ptr = (jo as JObject).Handle;
    //            JParamValueHelper.SetValueObjectArray(jAry.JValue, idx++, ptr);
    //        }

    //        jAry.JClass = JParamValueHelper.GetJavaClass("[L" + javaElemArrayClassName + ";");
    //        return jAry;
    //    }
    //}
    #endregion
}
