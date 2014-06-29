using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NXDO.RJava;
using NXDO.RJava.Attributes;

namespace RJavaX64
{
    [JClass("rt.Demo")]
    public class Demo : JObject
    {
        private static Type nCahceType = typeof(Demo);
        protected Demo(IntPtr objectPtr, IntPtr classPtr) : 
            base(objectPtr,classPtr)
        {
        }

        public Demo()
        {
            JSuper(new object[]{});
        }

        #region bool
        public Demo(bool b)
        {
            JSuper(new object[] { b });
        }

        public Demo(bool? b)
        {
            JSuper(new object[] { b });
        }

        public Demo(bool[] b)
        {
            JSuper(new object[] { b });
        }

        public Demo(bool?[] b)
        {
            JSuper(new object[] { b });
        }
        #endregion

        #region jobject
        public Demo(JObject d)
        {
            JSuper(new object[] { d });
        }

        public Demo(Demo d)
        {
            JSuper(new object[] { d });
        }

        public Demo(Demo[] d)
        {
            JSuper(new object[] { d });
        }
        #endregion

        public static int FieldSI
        {
            get
            {
                return JObject.JInvokeField<int>(nCahceType, "FieldSI", null);
            }
            set
            {
                JObject.JInvokeField<int>(nCahceType, "FieldSI", null, value);
            }
        }

        public int Age
        {
            get
            {
                return JObject.JInvokeMethod<int>(nCahceType, "get_xx", this);
            }
            set
            {
                JObject.JInvokeMethod<int>(nCahceType, "set_xx", this, new object[] { value });
            }
        }

        [JMethod("SayObject")]
        public JObject SayObject(JObject jo)
        {
            return JObject.JInvokeMethod<JObject>(nCahceType, "SayObject_object", this, new object[] { jo });
        }

        [JMethod("SayObjectAry")]
        public object[] SayObjectAry(object[] c)
        {
            //JObject[] jj = new JObject[c.Length];
            //for(int i=0;i<jj.Length; i++)
            //{
            //    jj[i] = (JObject)c[i];
            //}
            return JObject.JInvokeMethod<object[]>(nCahceType, "SayObjectAry", this, new object[] { c });
        }

        [JMethod("Say")]
        public bool[] Say(bool[] c)
        {
            return JObject.JInvokeMethod<bool[]>(nCahceType, "Say_double[]bool", this, new object[] { c });
        }

        [JMethod("Say")]
        public bool?[] Say(bool?[] c)
        {
            return JObject.JInvokeMethod<bool?[]>(nCahceType, "Say_Double[]bool", this, new object[] { c });
        }

        [JMethod("Say")]
        public byte[] Say(byte[] c)
        {
            return JObject.JInvokeMethod<byte[]>(nCahceType, "Say_double[]byte", this, new object[] { c });
        }

        [JMethod("Say")]
        public byte?[] Say(byte?[] c)
        {
            return JObject.JInvokeMethod<byte?[]>(nCahceType, "Say_Double[]byte", this, new object[] { c });
        }

        [JMethod("Say")]
        public char[] Say(char[] c)
        {
            return JObject.JInvokeMethod<char[]>(nCahceType, "Say_double[]char", this, new object[] { c });
        }

        [JMethod("Say")]
        public char?[] Say(char?[] c)
        {
            return JObject.JInvokeMethod<char?[]>(nCahceType, "Say_Double[]char", this, new object[] { c });
        }


        [JMethod("Say")]
        public short[] Say(short[] c)
        {
            return JObject.JInvokeMethod<short[]>(nCahceType, "Say_double[]short", this, new object[] { c });
        }

        [JMethod("Say")]
        public short?[] Say(short?[] c)
        {
            return JObject.JInvokeMethod<short?[]>(nCahceType, "Say_Double[]short", this, new object[] { c });
        }

        [JMethod("Say")]
        public int[] Say(int[] c)
        {
            return JObject.JInvokeMethod<int[]>(nCahceType, "Say_double[]int", this, new object[] { c });
        }

        [JMethod("Say")]
        public int?[] Say(int?[] c)
        {
            return JObject.JInvokeMethod<int?[]>(nCahceType, "Say_Double[]int", this, new object[] { c });
        }

        [JMethod("Say")]
        public long[] Say(long[] c)
        {
            return JObject.JInvokeMethod<long[]>(nCahceType, "Say_double[]long", this, new object[] { c });
        }

        [JMethod("Say")]
        public long?[] Say(long?[] c)
        {
            return JObject.JInvokeMethod<long?[]>(nCahceType, "Say_Double[]long", this, new object[] { c });
        }

        [JMethod("Say")]
        public float[] Say(float[] c)
        {
            return JObject.JInvokeMethod<float[]>(nCahceType, "Say_double[]float", this, new object[] { c });
        }

        [JMethod("Say")]
        public float?[] Say(float?[] c)
        {
            return JObject.JInvokeMethod<float?[]>(nCahceType, "Say_Double[]float", this, new object[] { c });
        }

        [JMethod("Say")]
        public double?[] Say(double?[] c)
        {
            return JObject.JInvokeMethod<double?[]>(nCahceType, "Say_Double[]", this, new object[] { c });
        }

        [JMethod("Say")]
        public double[] Say(double[] c)
        {
            return JObject.JInvokeMethod<double[]>(nCahceType, "Say_double[]", this, new object[] { c });
        }


        [JMethod("Say")]
        public string[] Say(string[] c)
        {
            return JObject.JInvokeMethod<string[]>(nCahceType, "Say_double[]string", this, new object[] { c });
        }

        [JMethod("Say")]
        public Demo[] Say(Demo[] c)
        {
            return JObject.JInvokeMethod<Demo[]>(nCahceType, "Say_Demo[]Demo", this, new object[] { c });
        }

        public Demo(string s)
        {
            this.JSuper(new object[] { s });
        }

        public Demo(string[] s)
        {
            this.JSuper(new object[] { s });
        }

        [JMethod("DBool")]
        public bool DBool(bool b){
            return JObject.JInvokeMethod<bool>(nCahceType, "DBool", this, new object[] { b });
	    }

        [JMethod("DBool")]
        public bool? DBool(bool? b)
        {
            return JObject.JInvokeMethod<bool?>(nCahceType, "DBool2", this, new object[] { b });
	    }

        [JMethod("DBoolArray")]
        public void DBoolArray(bool[] bs)
        {
            JObject.JInvokeMethod(nCahceType, "DBoolArray", this, new object[] { bs });
        }

        [JMethod("DBoolArray")]
        public void DBoolArray(bool?[] bs)
        {
            JObject.JInvokeMethod(nCahceType, "DBoolArray2", this, new object[] { bs });
        }

        [JMethod("DString")]
        public void DString(String s)
        {
            JObject.JInvokeMethod(nCahceType, "DString", this, new object[] { s });
        }

        [JMethod("DStringAry")]
        public void DStringAry(String[] ss)
        {
            JObject.JInvokeMethod(nCahceType, "DStringAry", this, new object[] { ss });
        }

        [JMethod("DDemo")]
        public void DDemo(Demo o)
        {
            JObject.JInvokeMethod(nCahceType, "DDemo", this, new object[] { o });
        }

        [JMethod("DDemoAry")]
        public void DDemoAry(Demo[] os)
        {
            JObject.JInvokeMethod(nCahceType, "DDemoAry", this, new object[] { os });
        }


        [JMethod("DObject")]
        public void DObject(JObject o)
        {
            JObject.JInvokeMethod(nCahceType, "DObject", this, new object[] { o });
        }

        [JMethod("DObjectAry")]
        public void DObjectAry(JObject[] os)
        {
            JObject.JInvokeMethod(nCahceType, "DObjectAry", this, new object[] { os });
        }

        [JMethod("DTObj")]
        public void DTObj<T>(T o)
        {
            JObject.JInvokeMethod(nCahceType, "DTObj", this, new object[] { o });
        }

        [JMethod("DTObjAry")]
        public void DTObjAry<T>(T[] os)
        {
            JObject.JInvokeMethod(nCahceType, "DTObjAry", this, new object[] { os });
        }


        [JMethod("TestStatic")]
        public static void TestStatic()
        {
            JObject.JInvokeMethod(nCahceType, "ts", null);
        }





        [JMethod("SayT")]
        public T SayT<T>(T t)
        {
            return JObject.JInvokeMethod<T>(nCahceType, "st", this, new object[] { t });
        }

        [JMethod("SayT")]
        public T[] SayT<T>(T[] t)
        {
            return JObject.JInvokeMethod<T[]>(nCahceType, "stary", this, new object[] { t });
        }
    }
}
