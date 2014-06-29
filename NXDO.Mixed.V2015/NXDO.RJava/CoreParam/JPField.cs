using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NXDO.RJava.Reflection;

namespace NXDO.RJava.Core
{
    class JPField : JParamValue
    {
        private JPField(IntPtr valPtr, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = valPtr;
        }

        private JPField(IntPtr[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetObjectArray("java.lang.reflect.Field", array);
        }

        public static implicit operator JPField(JField f)
        {
            return new JPField(f.Handle, "java.lang.reflect.Field");
        }

        public static implicit operator JPField(JField[] array)
        {
            if (array == null)
                array = new JField[] { };

            var valAry = array.Select(jc => jc.Handle).ToArray();
            return new JPField(valAry, "[Ljava.lang.reflect.Field;");
        }
    }
}
