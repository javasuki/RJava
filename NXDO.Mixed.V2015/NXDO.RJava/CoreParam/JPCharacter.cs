using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NXDO.RJava.Core
{
    internal class JPCharacter : JParamValue
    {
        private JPCharacter(char? c, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.ToPrimitiveValue(c);
        }

        private JPCharacter(char[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetPrimitiveArray(array);
        }

        private JPCharacter(char?[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetObjectArray("java.lang.Character", array);
        }



        public static implicit operator JPCharacter(char c)
        {
            return new JPCharacter(c, "char");
        }

        public static implicit operator JPCharacter(char? c)
        {
            return new JPCharacter(c, "java.lang.Character");
        }

        public static implicit operator JPCharacter(char[] array)
        {
            return new JPCharacter(array, "char[]");
        }

        public static implicit operator JPCharacter(char?[] array)
        {
            return new JPCharacter(array, "[Ljava.lang.Character;");
        }
    }
}
