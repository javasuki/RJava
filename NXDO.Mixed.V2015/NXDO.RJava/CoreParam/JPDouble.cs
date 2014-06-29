using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NXDO.RJava.Core
{
    internal class JPDouble : JParamValue
    {

        private JPDouble(double? d, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.ToPrimitiveValue(d);
        }

        private JPDouble(double[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetPrimitiveArray(array);
        }

        private JPDouble(double?[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetObjectArray("java.lang.Double", array);
        }
       

        public static implicit operator JPDouble(double d)
        {
            return new JPDouble(d, "double");
        }

        public static implicit operator JPDouble(double? d)
        {
            return new JPDouble(d, "java.lang.Double");
        }

        public static implicit operator JPDouble(double[] array)
        {
            return new JPDouble(array, "double[]");
        }

        public static implicit operator JPDouble(double?[] array)
        {
            return new JPDouble(array, "[Ljava.lang.Double;");
        }

    }
}
