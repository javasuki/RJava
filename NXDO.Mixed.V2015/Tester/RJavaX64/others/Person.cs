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
// * File Name:	Person.cs
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
//    [JClass("rt.Person")]
//    public class Person : JObjectX
//    {
//        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
//        static Type _tInstanceType = typeof(Person);
//        protected Person(IntPtr objInstance, IntPtr clzInstance)
//            : base(objInstance, clzInstance)
//        {
//        }

//        public Person()
//        {
//            //public ctor()
//            JSuper();
//        }

//        public Person(int arg0)
//        {
//            //public ctor(int)
//            var _pAryValues = new object[] {arg0};
//            JSuper(_pAryValues);
//        }

//        public Person(int arg0,Rt.Dog arg1)
//        {
//            //public ctor(int,rt.Dog)
//            var _pAryValues = new object[] {arg0, arg1};
//            JSuper(_pAryValues);
//        }

//        public virtual byte Byte
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<byte>("Byte", _tInstanceType, this, null);
//            }
//        }

//        public virtual char Char
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<char>("Char", _tInstanceType, this, null);
//            }
//        }

//        public virtual Rt.Dog Dog
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<Rt.Dog>("Dog", _tInstanceType, this, null);
//            }
//            set
//            {
//                JObjectX.JSetPropertyValue("Dog", _tInstanceType, this, value);
//            }
//        }

//        public virtual bool Bool
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<bool>("Bool", _tInstanceType, this, null);
//            }
//        }

//        public virtual double D
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<double>("D", _tInstanceType, this, null);
//            }
//        }

//        public virtual long L
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<long>("L", _tInstanceType, this, null);
//            }
//        }

//        public virtual float F
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<float>("F", _tInstanceType, this, null);
//            }
//        }

//        public virtual int I
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<int>("I", _tInstanceType, this, null);
//            }
//        }

//        public virtual short S
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<short>("S", _tInstanceType, this, null);
//            }
//        }

//        public virtual string XXX
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<string>("XXX", _tInstanceType, this, null);
//            }
//        }

//        public virtual int? Age
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<int?>("Age", _tInstanceType, this, null);
//            }
//            set
//            {
//                JObjectX.JSetPropertyValue("Age", _tInstanceType, this, value);
//            }
//        }

//        public virtual bool? Sex
//        {
//            set
//            {
//                JObjectX.JSetPropertyValue("Sex", _tInstanceType, this, value);
//            }
//        }

//        public virtual void DoString(string arg0)
//        {
//            //public void doString(java.lang.String) throws java.lang.Exception
//            var _pAryValues = new object[] {arg0};
//            JObjectX.JInvokeVoidMethod("doString(String)", _tInstanceType, this, _pAryValues);
//        }

//        public virtual void DoPrint()
//        {
//            //public void doPrint()
//            JObjectX.JInvokeVoidMethod("doPrint()", _tInstanceType, this);
//        }

//        public static void DoStatic()
//        {
//            //public static void doStatic()
//            JObjectX.JInvokeVoidMethod("doStatic()", _tInstanceType, null);
//        }

//        public static int DoStatic2()
//        {
//            //public static int doStatic2()
//            return JObjectX.JInvokeResultMethod<int>("doStatic2()", _tInstanceType, null, null);
//        }

//        public virtual void DoDogTest(string arg0,Rt.Dog arg1)
//        {
//            //public void doDogTest(java.lang.String,rt.Dog)
//            var _pAryValues = new object[] {arg0, arg1};
//            JObjectX.JInvokeVoidMethod("doDogTest(String,rt.Dog)", _tInstanceType, this, _pAryValues);
//        }

//        public virtual Rt.Dog GetReturnDog(string arg0)
//        {
//            //public rt.Dog getReturnDog(java.lang.String)
//            var _pAryValues = new object[] {arg0};
//            return JObjectX.JInvokeResultMethod<Rt.Dog>("getReturnDog(String)", _tInstanceType, this, null, _pAryValues);
//        }

//        public virtual int GetReturnInt(string arg0)
//        {
//            //public int getReturnInt(java.lang.String)
//            var _pAryValues = new object[] {arg0};
//            return JObjectX.JInvokeResultMethod<int>("getReturnInt(String)", _tInstanceType, this, null, _pAryValues);
//        }

//        public virtual string GetReturnString(string arg0)
//        {
//            //public java.lang.String getReturnString(java.lang.String)
//            var _pAryValues = new object[] {arg0};
//            return JObjectX.JInvokeResultMethod<string>("getReturnString(String)", _tInstanceType, this, null, _pAryValues);
//        }

//    }
//}
