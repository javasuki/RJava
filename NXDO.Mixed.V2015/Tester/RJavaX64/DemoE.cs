using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NXDO.RJava;
using NXDO.RJava.Core;
using NXDO.RJava.Attributes;

namespace RJavaX64
{
    [JClass("rt.DemoE")]
    public class DemoE : JObject
    {
        public int getage()
        {
            return JObject.JInvokeMethod<int>(this.GetType(), "hh", this);
        }

        public static new JClass Class
        {
            get
            {
                return JClass.ForName("rt.DemoE");
            }
        }

        private static Type nCahceType = typeof(DemoE);
        protected DemoE(IntPtr objectPtr, IntPtr classPtr) : 
            base(objectPtr, classPtr)
        {
        }

        public DemoE()
        {
            JSuper(new object[]{});
        }

        [JMethod("InClass")]
        public void InClass(JClass cls)
        {
            JObject.JInvokeMethod(nCahceType, "InClass", this, new object[] { cls });
        }

        [JMethod("InClassAry")]
        public void InClassAry(JClass[] cls){
            JObject.JInvokeMethod(nCahceType, "InClassAry", this, new object[] { cls });
	    }

        [JMethod("AddList")]
        public void AddList(JList<int?> lst)
        {
            JObject.JInvokeMethod(nCahceType, "AddList", this, new object[] { lst });
        }

        [JMethod("AddArrayList")]
        public void AddArrayList(object lst)
        {
            JObject.JInvokeMethod(nCahceType, "AddArrayList", this, new object[] { lst });
        }

        [JMethod("AddArrayListAry")]
        public void AddArrayListAry(object lst)
        {
            JObject.JInvokeMethod(nCahceType, "AddArrayListAry", this, new object[] { lst });
        }

        [JMethod("OOD")]
        public decimal OOD(decimal bd)
        {
            return JObject.JInvokeMethod<decimal>(nCahceType, "OOD", this, new object[] { bd });
        }

        [JMethod("OODAry")]
        public decimal[] OODAry(decimal[] bd)
        {
            //JObject.
            return JObject.JInvokeMethod<decimal[]>(nCahceType, "OODAry", this, new object[] { bd });
        }

        [JMethod("TestEnum")]
        public OEnum TestEnum(OEnum o)
        {
            return JObject.JInvokeMethod<OEnum>(nCahceType, "TestEnum", this, new object[] { o });
	    }

        [JMethod("TestO")]
        public Object TestO(Object o)
        {
            return JObject.JInvokeMethod<Object>(nCahceType, "TestO", this, new object[] { o });
        }

        [JMethod("TestEnumAry")]
        public OEnum[] TestEnumAry(OEnum[] o)
        {
            return JObject.JInvokeMethod<OEnum[]>(nCahceType, "TestEnumAry", this, new object[] { o });
        }

        [JMethod("TestOAry")]
        public Object[] TestOAry(Object[] o)
        {
            return JObject.JInvokeMethod<Object[]>(nCahceType, "TestOAry", this, new object[] { o });
        }

        [JMethod("TestDT")]
        public DateTime? TestDT(DateTime? o)
        {
            return JObject.JInvokeMethod<DateTime>(nCahceType, "TestDT", this, new object[] { o });
        }

        [JMethod("TestDTAry")]
        public DateTime[] TestDTAry(DateTime[] dts)
        {
            return JObject.JInvokeMethod<DateTime[]>(nCahceType, "TestDTAry", this, new object[] { dts });
        }
    }

    [JEnum("rt.OEnum")]
    public enum OEnum
    {
        Field,
	    Property,
	    Mehotd,
	    Ctor
    }

    public class AOO
    {
        public class BOO : IFOO
        {
            public class DOO
            {
            }
        }

        public class COO : IFOO
        {
        }
    }

    public interface IFOO
    {
    }

    public interface vvv
    {
        void ttt();
    }

    public class aaa : vvv
    {
        public virtual void ttt(){
        }
    }

    public class t : JObject, IBigvalue
    {
        public int GetAge2()
        {
            return 0;
        }
    }
}
