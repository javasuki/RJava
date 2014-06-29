using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NXDO.RJava.Core
{
    /// <summary>
    /// int/Integer (数组)类型参数
    /// </summary>
    internal class JPInt : JParamValue
    {
        #region ctor
        private JPInt(int? i, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.ToPrimitiveValue(i);
        }

        private JPInt(int[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetPrimitiveArray(array);
        }

        private JPInt(int?[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetObjectArray("java.lang.Integer", array);
        }
        #endregion


        #region implicit operator
        public static implicit operator JPInt(int i)
        {
            return new JPInt(i, "int");
        }

        public static implicit operator JPInt(int? i)
        {
            return new JPInt(i, "java.lang.Integer");
        }

        public static implicit operator JPInt(int[] array)
        {
            return new JPInt(array, "int[]");
        }

        public static implicit operator JPInt(int?[] array)
        {
            return new JPInt(array, "[Ljava.lang.Integer;");
        }
        #endregion
    }
}
