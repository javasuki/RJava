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
// * File Name:	Dog.cs
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
//    [JClass("rt.Dog")]
//    public class Dog : JObjectX
//    {
//        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
//        static Type _tInstanceType = typeof(Dog);
//        protected Dog(IntPtr objInstance, IntPtr clzInstance)
//            : base(objInstance, clzInstance)
//        {
//        }

//        public Dog(string arg0,string[] arg1)
//        {
//            //public ctor(java.lang.String,java.lang.String[])
//            var _pAryValues = new object[] {arg0, arg1};
//            JSuper(_pAryValues);
//        }

//        public Dog(string arg0)
//        {
//            //public ctor(java.lang.String)
//            var _pAryValues = new object[] {arg0};
//            JSuper(_pAryValues);

            
//        }

//        public static void testDemo()
//        {
//            JObjectX.DemoStart();
//        }

//        public void testJIntAry()
//        {
//            short?[] ii = new short?[] { 1, null, 3 };
//            var vvs = new object[] { ii };
//            JObjectX.testArrayValue(ii);
//        }

//        public string[] FldStrAry
//        {
//            get
//            {
//                return JObjectX.JGetFieldValue<string[]>("FldStrAry", _tInstanceType, this);
//            }
//            set
//            {
//                JObjectX.JSetFieldValue("FldStrAry", _tInstanceType, this, value);
//            }
//        }

//        public int[] FldIntAry
//        {
//            get
//            {
//                return JObjectX.JGetFieldValue<int[]>("FldIntAry", _tInstanceType, this);
//            }
//            set
//            {
//                JObjectX.JSetFieldValue("FldIntAry", _tInstanceType, this, value);
//            }
//        }

//        public int?[] FldIObjAry
//        {
//            get
//            {
//                return JObjectX.JGetFieldValue<int?[]>("FldIObjAry", _tInstanceType, this);
//            }
//            set
//            {
//                JObjectX.JSetFieldValue("FldIObjAry", _tInstanceType, this, value);
//            }
//        }

//        public Rt.Dog[] FldDogAry
//        {
//            get
//            {
//                return JObjectX.JGetFieldValue<Rt.Dog[]>("FldDogAry", _tInstanceType, this);
//            }
//            set
//            {
//                JObjectX.JSetFieldValue("FldDogAry", _tInstanceType, this, value);
//            }
//        }

//        public virtual string Name
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<string>("Name", _tInstanceType, this, null);
//            }
//            set
//            {
//                JObjectX.JSetPropertyValue("Name", _tInstanceType, this, value);
//            }
//        }

//        public virtual Rt.Dog[] DogAry
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<Rt.Dog[]>("DogAry", _tInstanceType, this, null);
//            }
//            set
//            {
//                JObjectX.JSetPropertyValue("DogAry", _tInstanceType, this, new object[] { value });
//            }
//        }

//        public virtual int?[] IntegerAry
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<int?[]>("IntegerAry", _tInstanceType, this, null);
//            }
//            set
//            {
//                JObjectX.JSetPropertyValue("IntegerAry", _tInstanceType, this, new object[] { value });
//            }
//        }

//        public virtual string[] StrAry2
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<string[]>("StrAry2", _tInstanceType, this, null);
//            }
//            set
//            {
//                JObjectX.JSetPropertyValue("StrAry2", _tInstanceType, this, new object[] { value });
//            }
//        }

//        public virtual bool[] BoolAry
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<bool[]>("BoolAry", _tInstanceType, this, null);
//            }
//            set
//            {
//                JObjectX.JSetPropertyValue("BoolAry", _tInstanceType, this, new object[] { value });
//            }
//        }

//        public virtual byte[] ByteAry
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<byte[]>("ByteAry", _tInstanceType, this, null);
//            }
//            set
//            {
//                JObjectX.JSetPropertyValue("ByteAry", _tInstanceType, this, new object[] { value });
//            }
//        }

//        public virtual char[] CharAry
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<char[]>("CharAry", _tInstanceType, this, null);
//            }
//            set
//            {
//                JObjectX.JSetPropertyValue("CharAry", _tInstanceType, this, new object[] { value });
//            }
//        }

//        public virtual short[] ShortAry
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<short[]>("ShortAry", _tInstanceType, this, null);
//            }
//            set
//            {
//                JObjectX.JSetPropertyValue("ShortAry", _tInstanceType, this, new object[] { value });
//            }
//        }

//        public virtual int[] IntAry
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<int[]>("IntAry", _tInstanceType, this, null);
//            }
//            set
//            {
//                JObjectX.JSetPropertyValue("IntAry", _tInstanceType, this, new object[] { value });
//            }
//        }

//        public virtual long[] LongAry
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<long[]>("LongAry", _tInstanceType, this, null);
//            }
//            set
//            {
//                JObjectX.JSetPropertyValue("LongAry", _tInstanceType, this, new object[] { value });
//            }
//        }

//        public virtual float[] FloatAry
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<float[]>("FloatAry", _tInstanceType, this, null);
//            }
//            set
//            {
//                JObjectX.JSetPropertyValue("FloatAry", _tInstanceType, this, new object[] { value });
//            }
//        }

//        public double[] DoubleAry
//        {
//            get
//            {
//                return JObjectX.JGetPropertyValue<double[]>("DoubleAry", _tInstanceType, this, null);
//            }
//            set
//            {
//                JObjectX.JSetPropertyValue("DoubleAry", _tInstanceType, this, new object[] { value });
//            }
//        }

//        public virtual int Age
//        {
//            set
//            {
//                JObjectX.JSetPropertyValue("Age", _tInstanceType, this, value);
//            }
//        }

//        public virtual int? AgeI
//        {
//            set
//            {
//                JObjectX.JSetPropertyValue("AgeI", _tInstanceType, this, value);
//            }
//        }

//        [JMethod("main")]
//        public static void JDogMain(string[] arg0)
//        {
//            //public static void main(java.lang.String[]) throws java.lang.Throwable
//            var _pAryValues = new object[] {arg0};
//            JObjectX.JInvokeVoidMethod("main(String[])", _tInstanceType, null, _pAryValues);
//        }

//        public virtual void Print()
//        {
//            //public void print()
//            JObjectX.JInvokeVoidMethod("print()", _tInstanceType, this);
//        }

//        public virtual void TestT<T>(T arg0)
//        {
//            //public <T> void testT(T)
//            var _pAryValues = new object[] {arg0};
//            JObjectX.JInvokeVoidMethod("testT(T)", _tInstanceType, this, _pAryValues);
//        }

//        public virtual void TestTAry<T>(params T[] arg0)
//        {
//            //public <T> void testTAry(T[])
//            var _pAryValues = new object[] {arg0};
//            JObjectX.JInvokeVoidMethod("testTAry(T[])", _tInstanceType, this, _pAryValues);
//        }

//        public virtual T[] GetTestTAry<T>()
//        {
//            //public <T> T[] getTestTAry()
//            return JObjectX.JInvokeResultMethod<T[]>("getTestTAry()", _tInstanceType, this, typeof(T));
//        }

//        [JMethod("GetRunT")]
//        public virtual T GetRunT<T>(T arg0)
//        {
//            //public <T> T GetRunT(T)
//            var _pAryValues = new object[] {arg0};
//            return JObjectX.JInvokeResultMethod<T>("GetRunT(T)", _tInstanceType, this, typeof(T), _pAryValues);
//        }

//        public virtual int DoGetintMethod()
//        {
//            //public int doGetintMethod()
//            return JObjectX.JInvokeResultMethod<int>("doGetintMethod()", _tInstanceType, this, null);
//        }

//        public virtual void SetStrAry(params string[] arg0)
//        {
//            //public void setStrAry(java.lang.String[])
//            var _pAryValues = new object[] {arg0};
//            JObjectX.JInvokeVoidMethod("setStrAry(String[])", _tInstanceType, this, _pAryValues);
//        }

//    }
//}
