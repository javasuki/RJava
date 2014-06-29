using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NXDO.RJava.Attributes;

namespace NXDO.RJava.Core
{
    /// <summary>
    /// 通用性Object参数
    /// </summary>
    class JPObject : JParamValue
    {
        private JPObject(string jParamClassName)
            : base(jParamClassName)
        {
        }

        /// <summary>
        /// 一切java类的根类型名称，java.lang.Object。
        /// </summary>
        public static string JavaBaseObjectClassName = "java.lang.Object";

        public static JPObject Create(bool isArray)
        {
            string javaClassName = !isArray ? JavaBaseObjectClassName : "[L" + JavaBaseObjectClassName + ";";
            var jpo = new JPObject(javaClassName);
            jpo.JValue = IntPtr.Zero;
            return jpo;
        }

        public void Add(JObject jo)
        {
            if (jo == null)
                return;

            //IntPtr ptr = IntPtr.Zero;
            //if (!jo.IsBoxStruct)
            //    ptr = jo.JObjectHolder == null ? IntPtr.Zero : jo.JObjectHolder.ObjectPtr;
            //else
            //    ptr = this.GetJBoxPtr((JBox)jo);
            this.JValue = this.GetJBoxPtr(jo);//ptr;
        }

        public void Add(JObject[] jos)
        {
            int size = jos == null ? 0 : jos.Length;
            IntPtr elemPtr = JParamValueHelper.GetJavaClass(JavaBaseObjectClassName);
            this.JValue = JParamValueHelper.CreateObjectArray(elemPtr, size);
            if (size == 0) return;

            int idx = 0;
            foreach (var jo in jos)
            {
                IntPtr ptr = IntPtr.Zero;
                if (jo != null)
                {
                    ptr = this.GetJBoxPtr((JBox)jo);

                    //if (!jo.IsBoxStruct)
                    //    ptr = jo.JObjectHolder == null ? IntPtr.Zero : jo.JObjectHolder.ObjectPtr;
                    //else
                    //{
                        
                        #region 注释
                        //Type boxType = ((JBox)jo).BoxType;
                        //object boxValue = ((JBox)jo).BoxValue;
                        //if (!boxType.IsEnum)
                        //{
                        //    bool isDate = boxType == typeof(DateTime) || boxType == typeof(DateTime?);
                        //    if (!isDate)
                        //        ptr = JParamValueHelper.ToPrimitiveValue(boxValue, boxType);
                        //    else
                        //    {
                        //        //日期
                        //        DateTime? date = (DateTime?)boxValue;
                        //        if (date.HasValue)
                        //        {
                        //            string sDateVal = date.Value.ToString(JPDateTime.SDateFormat);
                        //            ptr = JParamValueHelper.NewDate(sDateVal, JPDateTime.SDateFormat);
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    var jenumClassPtr = JParamValueHelper.GetJavaClass(JClassAttribute.Get(boxType));
                        //    ptr = JObject.JContext.JGetField(jenumClassPtr, boxValue.ToString());
                        //}
                        #endregion
                    //}
                }
                JParamValueHelper.SetValueObjectArray(this.JValue, idx++, ptr);
            }
        }

        /// <summary>
        /// 获取装箱值对应的 java 值（指针）
        /// </summary>
        /// <param name="jobject"></param>
        /// <returns>java 值（指针）</returns>
        private IntPtr GetJBoxPtr(JObject jobject)
        {
            if (jobject == null) return IntPtr.Zero;
            if (!jobject.IsBoxStruct)
                return jobject.Handle;

            JBox jbox = (JBox)jobject;
            Type boxType = jbox.BoxType;
            object boxValue = jbox.BoxValue;
            if (boxType.IsEnum)  //枚举
                return JObject.JContext.JGetField(
                                    JParamValueHelper.GetJavaClass(JClassAttribute.Get(boxType)),
                                    boxValue.ToString() );

            
            bool isDate = boxType == typeof(DateTime) || boxType == typeof(DateTime?);
            if (!isDate) //基本类型，string，JObject继承类
                return JParamValueHelper.ToPrimitiveValue(boxValue, boxType);

            //日期 DateTime java.util.Date/java.util.Calendar
            IntPtr ptr = IntPtr.Zero;
            DateTime? date = (DateTime?)boxValue;
            if (date.HasValue)
            {
                string sDateVal = date.Value.ToString(JPDateTime.SDateFormat);
                ptr = JParamValueHelper.NewDate(sDateVal, JPDateTime.SDateFormat);
            }
            return ptr;
        }
    }
}
