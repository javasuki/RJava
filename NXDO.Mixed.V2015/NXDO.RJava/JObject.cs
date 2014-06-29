using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

using NXDO.RJava.Attributes;
using NXDO.RJava.Core;

namespace NXDO.RJava
{
    /// <summary>
    /// 提供一个低级别的接口，已由 JObject 实现。
    /// <para>不推荐在用户代码中实现该接口。使用 JActivator.CreateByInterface 方法的接口可继承，以提供低级别的服务。</para>
    /// </summary>
    public interface JIBase
    {
        /// <summary>
        /// 获取当前实例的 JClass。
        /// </summary>
        /// <returns>JClass 实例，表示当前实例的在 jvm 中运行时的确切类型。</returns>
        JClass GetClass();

        /// <summary>
        /// 转换当前接口为 JObject 实例类型。
        /// <para>该实例在 jvm 中运行时的确切类型，由继承接口上的 JEmitAttribute 特性提供。</para>
        /// </summary>
        /// <returns>JObject 实例。</returns>
        JObject AsCast();
    }

    /// <summary>
    /// 支持 java 类层次结构中的所有类，并为派生类提供低级别服务。这是 C#.RJava 中所有类的最终基类；它是 java 类型层次结构的根。
    /// <para>未实现数组的根类型。
    /// <code>JObject jobj = int[]{1,2,3} 无效。</code>
    /// </para>
    /// <para>未实现泛型类上泛型参数的嵌套 eg. MyClass&lt;T&gt; : JObject，T又是一个泛型类型。</para>
    /// </summary>
    [JClass("java.lang.Object")]
    public abstract class JObject : JIBase, IDisposable //: IJObject
    {
        #region ctor
        /// <summary>
        /// JObject 构造函数，与java无任何交互。
        /// </summary>
        public JObject()
        {
        }

        /// <summary>
        /// 继承类必须实现的构造函数，与java 交互并持有 jvm 中的对象实例。
        /// <para>此构造函数由内部框架调用实现，请勿在用户代码中做任何调用。</para>
        /// </summary>
        /// <param name="objectPtr">实例指针</param>
        /// <param name="classPtr">类型指针</param>
        protected JObject(IntPtr objectPtr, IntPtr classPtr)
        {
            this.Handle = objectPtr;
            this._instanceJClass = new JClass(classPtr);
        }
        #endregion

        #region jclass 相关
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static JClass _class;
        /// <summary>
        /// Class&lt;java.lang.Object&gt; 实例。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static JClass Class
        {
            get
            {
                if (_class == null)
                    _class = JClass.ForName("java.lang.Object");
                return _class;                
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JClass _instanceJClass;
        /// <summary>
        /// 获取当前实例的 JClass。
        /// </summary>
        /// <returns>JClass 实例，表示当前实例的在 jvm 中运行时的确切类型。</returns>
        public virtual JClass GetClass()
        {
            if (_instanceJClass != null)
                return _instanceJClass;

            if (this.GetType() == typeof(JObject))
                _instanceJClass = JObject.Class;
            else
            {
                bool isSub = this.GetType().IsSubclassOf(typeof(JObject));
                if (!isSub)
                    throw new TypeAccessException("当前 Type 不是有效 JObject 的继承类。");

                string jclassName = JClassAttribute.Get(this);
                _instanceJClass = JClass.ForName(jclassName);
            }
            return _instanceJClass;
        }
        #endregion

        #region java持有/调用环境
        /// <summary>
        /// 持有 java 对象实例的句柄。
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal IntPtr Handle
        {
            get;
            set;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal static JRunCore JContext = JAssembly.JBridgeContext;
        #endregion

        #region jsuper
        /// <summary>
        /// 执行 java 构造方法。
        /// </summary>
        /// <param name="values">构造方法参数列表。</param>
        protected void JSuper(params object[] values)
        {
            //不使用缓存
            string jclassName = JClassAttribute.Get(this.GetType());
            this._instanceJClass = JClass.ForName(jclassName);

            var jvalues = JInvokeHelper.GetConstructorParams(this.GetType(), values);
            this.Handle = JContext.JNew(this._instanceJClass.Handle, jvalues);
        }
        #endregion

        JObject JIBase.AsCast()
        {
            return this.GetClass().AsCast(this);
        }

        #region JInvokeMethod
        /// <summary>
        /// 调用无返回值的 java 方法
        /// </summary>
        /// <param name="cahceType">缓存JObject继承类的类型。</param>
        /// <param name="cacheMethodKey">缓存java方法的KEY,不是有效的java方法名称。</param>
        /// <param name="jObjectInstance">JObject继承类实例，静态方法使用null。</param>
        /// <param name="values">方法调用的参数。<code>如果含有数组则： new object[]{ value, array }</code></param>
        protected static void JInvokeMethod(Type cahceType, string cacheMethodKey, JObject jObjectInstance, params object[] values)
        {
            JInvokeMethod<IntPtr>(cahceType, cacheMethodKey, jObjectInstance, values);
        }

        /// <summary>
        /// 调用 java 方法
        /// </summary>
        /// <typeparam name="T">返回值的类型参数</typeparam>
        /// <param name="cahceType">缓存JObject继承类的类型。</param>
        /// <param name="cacheMethodKey">缓存java方法的KEY,不是有效的java方法名称。</param>
        /// <param name="jObjectInstance">JObject继承类实例，静态方法使用null。</param>
        /// <param name="values">方法调用的参数。<code>如果含有数组则： new object[]{ value, array }</code></param>
        /// <returns>T类型的返回值。</returns>
        protected static T JInvokeMethod<T>(Type cahceType, string cacheMethodKey, JObject jObjectInstance, params object[] values)
        {
            JCacheInfo jcache = JCacheInfo.Get(cahceType, cacheMethodKey);
            if (jcache == null)
            {
                jcache = JInvokeHelper.GetCacheMethodParams(cahceType);
                JCacheInfo.Add(cahceType, cacheMethodKey, jcache);
            }

            var invokePtr = JInvokeHelper.GetInvokePtr(jcache.IsStatic, jObjectInstance, cahceType);
            var jvalues = JInvokeHelper.ConvertToJParams(jcache, values);            
            var jr = JContext.JInvoke(invokePtr, jcache.Name, jvalues);
            if (jcache.IsVoid) return default(T);

            return new JMReturn<T>(jr).Value;
        }
        #endregion

        #region JInvokeField
        /// <summary>
        /// 获取或设置 java 成员变量的值。
        /// <para>获取时，必须为某属性的get方法中调用。反之设置为set方法中调用。</para>
        /// </summary>
        /// <typeparam name="T">成员变量对应dotnet的类型</typeparam>
        /// <param name="cahceType">JObject继承类的类型</param>
        /// <param name="jFieldName">java 成员变量的名称，必须与 java 类中保持一致。</param>
        /// <param name="jObjectInstance">JObject继承类的实例。<para>如果是静态成员，则设置成 null。</para></param>
        /// <param name="fieldValue">设置成员变量的值。<para>如果是获取成员变量值，则设置成 null，或使用默认值。</para></param>
        /// <returns>返回成员变量的值。</returns>
        protected static T JInvokeField<T>(Type cahceType, string jFieldName, JObject jObjectInstance, object fieldValue = null)
        {
            bool isSet = fieldValue != null;
            string cacheFielddKey = isSet ? "dotnet@fld_set_" + jFieldName : "dotnet@fld_get_" + jFieldName;
            JCacheInfo jcache = JCacheInfo.Get(cahceType, cacheFielddKey);
            if (jcache == null)
            {
                jcache = JInvokeHelper.GetCacheFieldParam(cahceType, isSet, jFieldName);
                JCacheInfo.Add(cahceType, cacheFielddKey, jcache);
            }

            var invokePtr = JInvokeHelper.GetInvokePtr(jcache.IsStatic, jObjectInstance, cahceType);
            if (!isSet)
            {
                var jr = JContext.JGetField(invokePtr, jFieldName);
                return new JMReturn<T>(jr).Value;
            }

            var jvalues = JInvokeHelper.ConvertToJParams(jcache, new object[] { fieldValue });
            JContext.JSetField(invokePtr, jFieldName, jvalues[0]);
            return default(T);
        }
        #endregion

        #region c# object 相关的固有方法/Dispose
        #region Dispose
        /// <summary>
        /// 释放或重置非托管资源。
        /// </summary>
        public virtual void Dispose()
        {
            if (this.Handle == IntPtr.Zero) return;
            var context = JAssembly.JBridgeContext;
            context.FreeJObject(this.Handle, this.GetClass().Handle);
            this.Handle = IntPtr.Zero;
            GC.Collect();
        }
        #endregion

        #region 重写dotnet原生方法,换成 java 的方法值 (toString/hashCode)
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string _toStrValue;
        /// <summary>
        /// 返回表示当前 java.lang.Object 的 System.String。
        /// <para>java.lang.Class.getClass().getName() + '@' + Integer.toHexString(hashCode())</para>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Handle == IntPtr.Zero) return string.Empty;
            //if (string.IsNullOrWhiteSpace(this._toStrValue))
            {
                var context = JAssembly.JBridgeContext;
                this._toStrValue = context.GetObjectToString(this.Handle);
            }
            return this._toStrValue;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int _hashCode = int.MinValue;
        /// <summary>
        /// 返回表示当前 java.lang.Object 的哈希代码。
        /// </summary>
        /// <returns>哈希代码</returns>
        public override int GetHashCode()
        {
            if (this.Handle == IntPtr.Zero) return int.MinValue;
            if (_hashCode == int.MinValue)
            {
                var context = JAssembly.JBridgeContext;
                _hashCode = context.GetObjectHashcode(this.Handle);
            }
            return _hashCode;
        }
        

        /// <summary>
        /// 确定指定的 java.lang.Object 是否等于当前的 java.lang.Object。
        /// </summary>
        /// <param name="obj">需要进行比较的 java.lang.Object。</param>
        /// <returns>相等为 true；否则为 false。</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is JObject)) return false;
            
            if (this.Handle == IntPtr.Zero) return false;

            var jSub = obj as JObject;
            if (jSub.Handle == IntPtr.Zero) return false;

            var context = JAssembly.JBridgeContext;
            int hashCode = context.GetObjectHashcode(this.Handle);
            int hashCode2 = context.GetObjectHashcode(jSub.Handle);
            return hashCode == hashCode2;
        }
        #endregion

        #region dotnet 原生方法
        /// <summary>
        /// 返回表示当前 System.Object 的 System.String。
        /// <para>dotnet原生方法</para>
        /// </summary>
        /// <returns>表示当前的 System.Object。</returns>
        public virtual string ToStringN()
        {
            return base.ToString();
        }

        /// <summary>
        /// 用作特定类型的哈希函数。
        /// <para>dotnet原生方法</para>
        /// </summary>
        /// <returns>当前 System.Object 的哈希代码。</returns>
        public virtual int GetHashCodeN()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 确定指定的 System.Object 是否等于当前的 System.Object。
        /// </summary>
        /// <param name="obj">与当前的 System.Object 进行比较的 System.Object。</param>
        /// <returns>如果指定的 System.Object 等于当前的 System.Object，则为 true；否则为 false。</returns>
        public bool EqualsN(object obj)
        {
            return base.Equals(obj);
        }
        #endregion
        #endregion

        #region 基本类型装箱/拆箱，为满足 java xxx(object v) 到 c# xxx(JObject v) 包装的类型装箱转换。 函数返回值为 JObject 需要拆箱时。
        /// <summary>
        /// 是否为装箱对象
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal virtual bool IsBoxStruct
        {
            get { return false; }
        }

        #region box
        /// <summary>
        /// bool装箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator JObject(bool value)
        {
            return new JBox<bool>(value);
        }

        /// <summary>
        /// byte装箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator JObject(byte value)
        {
            return new JBox<byte>(value);
        }

        /// <summary>
        /// char装箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator JObject(char value)
        {
            return new JBox<char>(value);
        }

        /// <summary>
        /// short装箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator JObject(short value)
        {
            return new JBox<short>(value);
        }

        /// <summary>
        /// int装箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator JObject(int value)
        {
            return new JBox<int>(value);
        }

        /// <summary>
        /// long装箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator JObject(long value)
        {
            return new JBox<long>(value);
        }

        /// <summary>
        /// float装箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator JObject(float value)
        {
            return new JBox<float>(value);
        }

        /// <summary>
        /// double装箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator JObject(double value)
        {
            return new JBox<double>(value);
        }

        /// <summary>
        /// string装箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator JObject(string value)
        {
            return new JBox<string>(value);
        }
        #endregion

        #region unbox
        /// <summary>
        /// bool拆箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns>dotnet bool值</returns>
        [DebuggerNonUserCode]
        public static implicit operator bool(JObject value)
        {
            if (value != null && value is JUnknown)
            {
                //动态关键字，实现动态时解析的方法调用后的支持
                return (bool)(JUnknown)value;
            }

            JBox box = (JBox)value;
            if (box == null || !box.IsBoxStruct) return false;
            return Convert.ToBoolean(box.BoxValue);
        }

        /// <summary>
        /// byte拆箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns>dotnet byte值</returns>
        [DebuggerNonUserCode]
        public static implicit operator byte(JObject value)
        {
            if (value != null && value is JUnknown)
            {
                //动态关键字，实现动态时解析的方法调用后的支持
                return (byte)(JUnknown)value;
            }

            JBox box = (JBox)value;
            if (box == null || !box.IsBoxStruct) return 0x00;
            return Convert.ToByte(box.BoxValue);
        }

        /// <summary>
        /// char拆箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns>dotnet char值</returns>
        [DebuggerNonUserCode]
        public static implicit operator char(JObject value)
        {
            if (value != null && value is JUnknown)
            {
                //动态关键字，实现动态时解析的方法调用后的支持
                return (char)(JUnknown)value;
            }

            JBox box = (JBox)value;
            if (box == null || !box.IsBoxStruct) return (char)'\0';
            return Convert.ToChar(box.BoxValue);
        }

        /// <summary>
        /// short拆箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns>dotnet short值</returns>
        [DebuggerNonUserCode]
        public static implicit operator short(JObject value)
        {
            if (value != null && value is JUnknown)
            {
                //动态关键字，实现动态时解析的方法调用后的支持
                return (short)(JUnknown)value;
            }

            JBox box = (JBox)value;
            if (box == null || !box.IsBoxStruct) return 0;
            return Convert.ToInt16(box.BoxValue);
        }

        /// <summary>
        /// int拆箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns>dotnet int值</returns>
        //[DebuggerNonUserCode]
        public static implicit operator int(JObject value)
        {
            if (value != null && value is JUnknown)
            {
                //动态关键字，实现动态时解析的方法调用后的支持
                return (int)(JUnknown)value;
            }

            JBox box = (JBox)value;
            if (box == null || !box.IsBoxStruct) return 0;
            return Convert.ToInt32(box.BoxValue);
        }

        /// <summary>
        /// long拆箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns>dotnet long值</returns>
        [DebuggerNonUserCode]
        public static implicit operator long(JObject value)
        {
            if (value != null && value is JUnknown)
            {
                //动态关键字，实现动态时解析的方法调用后的支持
                return (long)(JUnknown)value;
            }

            JBox box = (JBox)value;
            if (box == null || !box.IsBoxStruct) return 0;
            return Convert.ToInt64(box.BoxValue);
        }

        /// <summary>
        /// float拆箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns>dotnet float值</returns>
        [DebuggerNonUserCode]
        public static implicit operator float(JObject value)
        {
            if (value != null && value is JUnknown)
            {
                //动态关键字，实现动态时解析的方法调用后的支持
                return (float)(JUnknown)value;
            }

            JBox box = (JBox)value;
            if (box == null || !box.IsBoxStruct) return 0;
            return Convert.ToSingle(box.BoxValue);
        }

        /// <summary>
        /// double拆箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns>dotnet double值</returns>
        [DebuggerNonUserCode]
        public static implicit operator double(JObject value)
        {
            if (value != null && value is JUnknown)
            {
                //动态关键字，实现动态时解析的方法调用后的支持
                return (double)(JUnknown)value;
            }

            JBox box = (JBox)value;
            if (box == null || !box.IsBoxStruct) return 0;
            return Convert.ToDouble(box.BoxValue);
        }

        /// <summary>
        /// string拆箱
        /// </summary>
        /// <param name="value"></param>
        /// <returns>dotnet string值</returns>
        [DebuggerNonUserCode]
        public static implicit operator string(JObject value)
        {
            if (value != null && value is JUnknown)
            {
                //动态关键字，实现动态时解析的方法调用后的支持
                return (string)(JUnknown)value;
            }

            JBox box = (JBox)value;
            if (box == null || !box.IsBoxStruct) return string.Empty;
            return (string)box.BoxValue;
        }
        #endregion
        #endregion

    }

}
