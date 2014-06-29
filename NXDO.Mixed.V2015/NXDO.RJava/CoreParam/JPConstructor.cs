using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NXDO.RJava.Reflection;

namespace NXDO.RJava.Core
{
    class JPConstructor : JParamValue
    {
        private JPConstructor(IntPtr valPtr, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = valPtr;
        }

        private JPConstructor(IntPtr[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetObjectArray("java.lang.reflect.Constructor", array);
        }

        public static implicit operator JPConstructor(JConstructor c)
        {
            return new JPConstructor(c.Handle, "java.lang.reflect.Constructor");
        }

        public static implicit operator JPConstructor(JConstructor[] array)
        {
            if (array == null)
                array = new JConstructor[] { };

            var valAry = array.Select(jc => jc.Handle).ToArray();
            return new JPConstructor(valAry, "[Ljava.lang.reflect.Constructor;");
        }
    }
}
