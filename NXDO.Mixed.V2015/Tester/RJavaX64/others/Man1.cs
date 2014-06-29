//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;

//using NXDO.JRuntime.JVM;
//using NXDO.JRuntime.Attributes;

//#region 代码说明
///*
// *******************************************************
// *       此段代码由 NXDO.RJacoste.Addin 产生  
// *******************************************************
// * File Name:	Man1.cs
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
//    [JClass("rt.Man1")]
//    public class Man1 : Rt.Man
//    {
//        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
//        static Type _tInstanceType = typeof(Man1);
//        protected Man1(IntPtr objInstance, IntPtr clzInstance)
//            : base(objInstance, clzInstance)
//        {
//        }

//        public Man1() : base()
//        {
//        }

//        public override void DoString(string arg0)
//        {
//            //public void doString(java.lang.String) throws java.lang.Exception
//            var _pAryValues = new object[] {arg0};
//            JObjectX.JInvokeVoidMethod("doString(String)", _tInstanceType, this, _pAryValues);
//        }

//    }
//}
