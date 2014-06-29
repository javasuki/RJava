using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NXDO.RJava.Core
{
    internal class JPLong : JParamValue
    {
        private JPLong(long? l, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.ToPrimitiveValue(l);
        }

        private JPLong(long[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetPrimitiveArray(array);
        }

        private JPLong(long?[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetObjectArray("java.lang.Long", array);
        }

        internal static JPLong ToGenericArray(long[] array, string javaClassName)
        {
            var lst = new List<long?>();
            foreach (long v in array)
            {
                lst.Add((long?)v);
            }
            return new JPLong(lst.ToArray(), javaClassName);
        }

        public static implicit operator JPLong(long l)
        {
            return new JPLong(l, "long");
        }

        public static implicit operator JPLong(long? l)
        {
            return new JPLong(l, "java.lang.Long");
        }

        public static implicit operator JPLong(long[] array)
        {
            return new JPLong(array, "long[]");
        }

        public static implicit operator JPLong(long?[] array)
        {
            return new JPLong(array, "[Ljava.lang.Long;");
        }
    }
}
