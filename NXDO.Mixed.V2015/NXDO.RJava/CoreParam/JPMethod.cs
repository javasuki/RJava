using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NXDO.RJava.Reflection;

namespace NXDO.RJava.Core
{
    class JPMethod : JParamValue
    {
        private JPMethod(IntPtr valPtr, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = valPtr;
        }

        private JPMethod(IntPtr[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetObjectArray("java.lang.reflect.Method", array);
        }

        public static implicit operator JPMethod(JMethod m)
        {
            return new JPMethod(m.Handle, "java.lang.reflect.Method");
        }

        public static implicit operator JPMethod(JMethod[] array)
        {
            if (array == null)
                array = new JMethod[] { };

            var valAry = array.Select(jc => jc.Handle).ToArray();
            return new JPMethod(valAry, "[Ljava.lang.reflect.Method;");
        }
    }
}
