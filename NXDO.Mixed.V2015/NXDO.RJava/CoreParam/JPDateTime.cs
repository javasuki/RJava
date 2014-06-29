using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NXDO.RJava.Core
{
    class JPDateTime : JParamValue
    {
        public static string JavaDateClassName = "java.util.Date";
        public static string SDateFormat = "yyyy-MM-dd HH:mm:ss";

        private JPDateTime(DateTime? date, string jParamClassName)
            : base(jParamClassName)
        {
            if (!date.HasValue) return;
            string sDateVal = date.Value.ToString(SDateFormat);
            this.JValue = JParamValueHelper.NewDate(sDateVal, SDateFormat);
        }

        private JPDateTime(Array array, string jParamClassName, string jElemClassName)
            : base(jParamClassName)
        {
            int size = array == null ? 0 : array.Length;
            var elemPtr = JParamValueHelper.GetJavaClass(jElemClassName);
            this.JValue = JParamValueHelper.CreateObjectArray(elemPtr, size);
            if (size == 0) return;

            #region 转换成指针数组
            int idx = 0;
            foreach (DateTime? v in array)
            {
                IntPtr ptr = IntPtr.Zero;
                if (v != null)
                {
                    string sDateVal = v.Value.ToString(SDateFormat);
                    ptr = JParamValueHelper.NewDate(sDateVal, SDateFormat);
                }

                JParamValueHelper.SetValueObjectArray(this.JValue, idx++, ptr);
            }
            #endregion
        }

        public static JPDateTime Create(DateTime? date, string jParamClassName)
        {
            return new JPDateTime(date, jParamClassName);
        }

        public static JPDateTime Create(Array array, string jParamClassName)
        {
            string jcn = jParamClassName;
            if (!jcn.StartsWith("[L"))
                jcn = "[L" + jcn;
            if (!jcn.EndsWith(";"))
                jcn += ";";

            return new JPDateTime(array, jcn, jParamClassName);
        }
    }
}
