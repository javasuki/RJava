using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;

namespace NXDO.RJava
{
    /// <summary>
    /// Dotnet类型装箱对象
    /// </summary>
    [DebuggerDisplay("{BoxType} = {BoxValue}")]
    abstract class JBox : JObject//, IConvertible
    {

        /// <summary>
        /// 装箱的类型
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public abstract Type BoxType { get; }

        /// <summary>
        /// 装箱值
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public abstract object BoxValue { get; }


        /// <summary>
        /// 是否为装箱对象
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal override bool IsBoxStruct
        {
            get
            {
                return true;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static Dictionary<Type, ConstructorInfo> dicBoxCtors = new Dictionary<Type, ConstructorInfo>();

        /// <summary>
        /// JObject通用与泛型的装箱
        /// </summary>
        /// <param name="o">需要装箱的值</param>
        /// <returns>JObject</returns>
        public static JObject CreateBox(object o)
        {
            if (o == null) return null;
            
            if (o is JObject) return (JObject)o;
            if (o is JBox) return (JBox)o;

            ConstructorInfo ctor= null;
            Type type = o.GetType();
            if (dicBoxCtors.ContainsKey(type))
                ctor = dicBoxCtors[type];
            else
            {
                ctor = typeof(JBox<>).MakeGenericType(o.GetType()).GetConstructors().First();
                dicBoxCtors.Add(type, ctor);
            }
            return (JBox)ctor.Invoke(new object[] { o });
        }

        /// <summary>
        /// JObject通用数组与泛型数组的装箱
        /// </summary>
        /// <param name="os">需要装箱的数组</param>
        /// <returns>JObject数组</returns>
        public static JObject[] CreateBoxArray(Array os)
        {
            if (os == null) return null;
            List<JObject> lst = new List<JObject>();
            for (int i = 0; i < os.Length; i++)
            {
                lst.Add( JBox.CreateBox(os.GetValue(i)));
            }
            return lst.ToArray();
        }

        #region IConvertible
        ////TODO *****************************
        ////TODO *****************************
        //TypeCode IConvertible.GetTypeCode()
        //{
        //    throw new NotImplementedException();
        //}

        //bool IConvertible.ToBoolean(IFormatProvider provider)
        //{
        //    return (bool)this.BoxValue;
        //}

        //byte IConvertible.ToByte(IFormatProvider provider)
        //{
        //    return (byte)this.BoxValue;
        //}

        //char IConvertible.ToChar(IFormatProvider provider)
        //{
        //    return (char)this.BoxValue;
        //}

        ////TODO *****************************
        ////TODO *****************************
        //DateTime IConvertible.ToDateTime(IFormatProvider provider)
        //{
        //    throw new NotImplementedException();
        //}

        //decimal IConvertible.ToDecimal(IFormatProvider provider)
        //{
        //    return (decimal)this.BoxValue;
        //}

        //double IConvertible.ToDouble(IFormatProvider provider)
        //{
        //    return (double)this.BoxValue;
        //}

        //short IConvertible.ToInt16(IFormatProvider provider)
        //{
        //    return (short)this.BoxValue;
        //}

        //int IConvertible.ToInt32(IFormatProvider provider)
        //{
        //    return (int)this.BoxValue;
        //}

        //long IConvertible.ToInt64(IFormatProvider provider)
        //{
        //    return (long)this.BoxValue;
        //}

        //sbyte IConvertible.ToSByte(IFormatProvider provider)
        //{
        //    return (sbyte)this.BoxValue;
        //}

        //float IConvertible.ToSingle(IFormatProvider provider)
        //{
        //    return (float)this.BoxValue;
        //}

        //string IConvertible.ToString(IFormatProvider provider)
        //{
        //    return (string)this.BoxValue;
        //}

        ////TODO *****************************
        ////TODO *****************************
        //object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        //{
        //    throw new NotImplementedException();
        //}

        //ushort IConvertible.ToUInt16(IFormatProvider provider)
        //{
        //    return (ushort)this.BoxValue;
        //}

        //uint IConvertible.ToUInt32(IFormatProvider provider)
        //{
        //    return (uint)this.BoxValue;
        //}

        //ulong IConvertible.ToUInt64(IFormatProvider provider)
        //{
        //    return (ulong)this.BoxValue;
        //}
        #endregion
    }

    /// <summary>
    /// 装箱对象
    /// </summary>
    [DebuggerNonUserCode]
    class JBox<T> : JBox
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private object boxValue;
        public JBox(T v)
        {
            this.boxValue = v;
        }

        /// <summary>
        /// 装箱的类型
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override Type BoxType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// 装箱值
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override object BoxValue
        {
            get { return this.boxValue; }
        }

        /// <summary>
        /// 重写ToString，便于查看装箱值。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.boxValue == null ? string.Empty : this.boxValue.ToString();
        }
    }
}
