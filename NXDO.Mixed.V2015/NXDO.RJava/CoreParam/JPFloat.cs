using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NXDO.RJava.Core
{
    internal class JPFloat : JParamValue
    {

        private JPFloat(float? f, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.ToPrimitiveValue(f);
        }

        private JPFloat(float[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetPrimitiveArray(array);
        }

        private JPFloat(float?[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetObjectArray("java.lang.Float", array);
        }  

        public static implicit operator JPFloat(float f)
        {
            return new JPFloat(f, "float");
        }

        public static implicit operator JPFloat(float? f)
        {
            return new JPFloat(f, "java.lang.Float");
        }

        public static implicit operator JPFloat(float[] array)
        {
            return new JPFloat(array, "float[]");
        }

        public static implicit operator JPFloat(float?[] array)
        {
            return new JPFloat(array, "[Ljava.lang.Float;");
        }        
    }
}
