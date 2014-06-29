using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NXDO.RJava.Attributes;

namespace NXDO.RJava.Core
{
    class JPEnum : JParamValue
    {
        private JPEnum(Enum v, string jParamClassName)
            : base(jParamClassName)
        {
            this.JValue = JObject.JContext.JGetField(this.JClass, v.ToString());
        }

        private JPEnum(Array array, string jParamClassName, string jElemClassName)
            : base(jParamClassName)
        {
            int size = array == null ? 0 : array.Length;
            var elemPtr = JParamValueHelper.GetJavaClass(jElemClassName);
            this.JValue = JParamValueHelper.CreateObjectArray(elemPtr, size);
            if (size == 0) return;

            #region 转换成指针数组
            int idx = 0;
            foreach (Object v in array)
            {
                IntPtr ptr = IntPtr.Zero;
                if (v != null)
                {
                    Enum e = (Enum)v;
                    ptr = JObject.JContext.JGetField(elemPtr, e.ToString());
                }

                JParamValueHelper.SetValueObjectArray(this.JValue, idx++, ptr);
            }
            #endregion
        }

        public static JPEnum Create(Enum v, string jParamClassName)
        {
            return new JPEnum(v, jParamClassName);
        }

        public static JPEnum Create(Array array, string jParamClassName)
        {
            string jcn = jParamClassName;
            if (!jcn.StartsWith("[L"))
                jcn = "[L" + jcn;
            if (!jcn.EndsWith(";"))
                jcn += ";";

            return new JPEnum(array, jcn, jParamClassName);
        }
    }
}
