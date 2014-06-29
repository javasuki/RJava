using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NXDO.RJava.Core
{
    internal class JPString : JParamValue
    {
        private JPString(string s, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.ToPrimitiveValue(s);
        }

        private JPString(string[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetObjectArray("java.lang.String", array);
        }

        public static implicit operator JPString(string s)
        {
            return new JPString(s, "java.lang.String");
        }

        public static implicit operator JPString(string[] array)
        {
            return new JPString(array, "[Ljava.lang.String;");
        }    
    }

    //public class JStringClass<T> : JStringClass
    //{
    //    internal JStringClass(string s, string javaClassName)
    //        : base(s, javaClassName)
    //    {
    //    }
    //}
}
