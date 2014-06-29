using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NXDO.RJava.Core
{
    internal class JPShort : JParamValue
    {
        private JPShort(short? s, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.ToPrimitiveValue(s);
        }

        private JPShort(short[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetPrimitiveArray(array);
        }

        private JPShort(short?[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetObjectArray("java.lang.Short", array);
        }

        public static implicit operator JPShort(short s)
        {
            return new JPShort(s, "short");
        }

        public static implicit operator JPShort(short? s)
        {
            return new JPShort(s, "java.lang.Short");
        }

        public static implicit operator JPShort(short[] array)
        {
            return new JPShort(array, "short[]");
        }

        public static implicit operator JPShort(short?[] array)
        {
            return new JPShort(array, "[Ljava.lang.Short;");
        }
    }
}
