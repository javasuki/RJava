using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NXDO.RJava.Core
{
    internal class JPDecimal : JParamValue
    {
        #region ctor
        private JPDecimal(decimal? d, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.ToPrimitiveValue(d);
        }

        private JPDecimal(decimal[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetObjectArray("java.math.BigDecimal", array);
        }

        private JPDecimal(decimal?[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetObjectArray("java.math.BigDecimal", array);
        }
        #endregion

        #region implicit operator
        public static implicit operator JPDecimal(decimal d)
        {
            return new JPDecimal(d, "java.math.BigDecimal");
        }

        public static implicit operator JPDecimal(decimal? d)
        {
            return new JPDecimal(d, "java.math.BigDecimal");
        }

        public static implicit operator JPDecimal(decimal[] array)
        {
            return new JPDecimal(array, "[Ljava.math.BigDecimal;");
        }

        public static implicit operator JPDecimal(decimal?[] array)
        {
            return new JPDecimal(array, "[Ljava.math.BigDecimal;");
        }
        #endregion
    }
}
