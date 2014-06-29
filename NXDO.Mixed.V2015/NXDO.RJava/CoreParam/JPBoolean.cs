using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NXDO.RJava.Core
{
    internal class JPBoolean : JParamValue
    {
        private JPBoolean(bool? b, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.ToPrimitiveValue(b);
        }

        private JPBoolean(bool[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetPrimitiveArray(array);
        }

        private JPBoolean(bool?[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetObjectArray("java.lang.Boolean", array);
        }
        
        public static implicit operator JPBoolean(bool b)
        {
            return new JPBoolean(b, "boolean");
        }

        public static implicit operator JPBoolean(bool? b)
        {
            return new JPBoolean(b, "java.lang.Boolean");
        }

        public static implicit operator JPBoolean(bool[] array)
        {
            return new JPBoolean(array, "boolean[]");
        }

        public static implicit operator JPBoolean(bool?[] array)
        {
            return new JPBoolean(array, "[Ljava.lang.Boolean;");
        }     
    }
}
