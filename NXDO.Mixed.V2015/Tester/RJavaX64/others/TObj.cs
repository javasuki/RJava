//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;

//using NXDO.JRuntime.JVM;
//using NXDO.JRuntime.Attributes;

//#region ����˵��
///*
// *******************************************************
// *       �˶δ����� NXDO.RJacoste.Addin ����  
// *******************************************************
// * File Name:	TObj.cs
// * Date Time:	Fri Feb 14 16:25:58 CST 2014
// * Anthor:	jjsiki(ZhuQi) javasuki@hotmail.com
// * Blog:		http://blog.csdn.net/javasuki
// * Generator:	RJacoste(jxdo.bil2java.JarFinder) For VS2012
// * Version:		1.0.0.0
// * -----------------------------------------------------
// */
//#endregion

//namespace Jacoste.Rt
//{
//    public interface IObj : JInterface
//    {
//        void Settt(int i);
//    }

//    [JClass("rt.TObj")]
//    public class TObj<T> : JObjectX, IObj
//    {
//        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
//        static Type _tInstanceType = typeof(TObj<>);
//        protected TObj(IntPtr objInstance, IntPtr clzInstance)
//            : base(objInstance, clzInstance)
//        {
//        }

//        public TObj()
//        {
//            //public ctor()
//            JSuper();
//        }

//        public virtual T Value
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<T>("Value", _tInstanceType, this, typeof(T));
//            }
//            set
//            {
//                JObjectX.JSetPropertyValue("Value", _tInstanceType, this, value);
//            }
//        }



//        [JMethod("main")]
//        public static void JTObjMain(string[] arg0)
//        {
//            //public static void main(java.lang.String[]) throws java.lang.Throwable
//            var _pAryValues = new object[] {arg0};
//            JObjectX.JInvokeVoidMethod("main(String[])", _tInstanceType, null, _pAryValues);
//        }


//        public void Settt(int i)
//        {
//            JObjectX.JInvokeVoidMethod("settt(int)", _tInstanceType, this, i);
//        }
//    }
//}
