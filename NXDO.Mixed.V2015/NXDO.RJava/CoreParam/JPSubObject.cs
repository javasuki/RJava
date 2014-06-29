using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NXDO.RJava.Attributes;

namespace NXDO.RJava.Core
{
    internal class JPSubObject : JParamValue
    {
        private JPSubObject(IntPtr ptr, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = ptr;
        }

        private JPSubObject(IntPtr[] ptrs, string jParamClassName, string jElemClassName)
            : base(jParamClassName)
        {
            this.JValue = JParamValueHelper.GetObjectArray(jElemClassName, ptrs);
        }


        public static JPSubObject Create(JObject jobject, string jParamClassName)
        {
            if(jobject != null)
                return new JPSubObject(jobject.Handle, jParamClassName);
            return new JPSubObject(IntPtr.Zero, jParamClassName);
        }

        public static JPSubObject Create(JObject[] array, string jParamClassName)
        {
            string jcn = jParamClassName;
            if (!jcn.StartsWith("[L"))
                jcn = "[L" + jcn;
            if (!jcn.EndsWith(";"))
                jcn += ";";

            List<IntPtr> lstPtrs = new List<IntPtr>();
            foreach (JObject jo in array)
            {
                if (jo == null)
                    lstPtrs.Add(IntPtr.Zero);
                else
                    lstPtrs.Add(jo.Handle);
            }
            return new JPSubObject(lstPtrs.ToArray(), jcn, jParamClassName);            
        }      
    }
}
