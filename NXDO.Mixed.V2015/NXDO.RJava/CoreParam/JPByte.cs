using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NXDO.RJava.Core
{
    internal class JPByte : JParamValue
    {
        private JPByte(byte? b, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.ToPrimitiveValue(b);
        }

        private JPByte(byte[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetPrimitiveArray(array);
        }

        private JPByte(byte?[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetObjectArray("java.lang.Byte", array);
        }

        public static implicit operator JPByte(byte b)
        {
            return new JPByte(b, "byte");
        }

        public static implicit operator JPByte(byte? b)
        {
            return new JPByte(b, "java.lang.Byte");
        }

        public static implicit operator JPByte(byte[] array)
        {
            return new JPByte(array, "byte[]");
        }

        public static implicit operator JPByte(byte?[] array)
        {
            return new JPByte(array, "[Ljava.lang.Byte;");
        }  
    }
}
