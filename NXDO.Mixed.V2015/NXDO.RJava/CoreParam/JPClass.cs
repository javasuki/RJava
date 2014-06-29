using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NXDO.RJava.Core
{
    class JPClass : JParamValue
    {
        private JPClass(IntPtr valPtr, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = valPtr;
        }

        private JPClass(IntPtr[] array, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetObjectArray("java.lang.Class", array);
        }

        public static implicit operator JPClass(JClass c)
        {
            return new JPClass(c.Handle, "java.lang.Class");
        }

        public static implicit operator JPClass(JClass[] array)
        {
            if (array == null)
                array = new JClass[] { };

            var valAry = array.Select(jc => jc.Handle).ToArray();
            return new JPClass(valAry, "[Ljava.lang.Class;");
        }
    }
}
