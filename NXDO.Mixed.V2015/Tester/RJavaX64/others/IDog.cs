//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

////using NXDO.JBridge;
//using NXDO.JRuntime.JVM;
//using NXDO.JRuntime.Attributes;

//namespace RJavaX64
//{
//    [JClass("rt.Dog")]
//    public class Dog : JObject
//    {
//        private static Type _tInstanceType = typeof(Dog);
//        protected Dog(IntPtr optr, IntPtr cptr)
//            : base(optr,cptr)
//        {
//        }

//        virtual public void ttyy()
//        {
//        }

        

//        [JMethod("main")]
//        public static void JMain(params String[] args)
//        {
//            object[] o = new object[] { args };
//            JObject.JInvokeVoidMethod("main", typeof(Dog), null, o);
//        }
        
//        public Dog(string name)// : base(name)
//        {
//            JSuper(name);
//        }

//        public Dog(string name, String[] values)// : base(name)
//        {
//            var o = new object[] { name, values };
//            JSuper(o);
//        }


//        public void testT<T>(T t) 
//        {
//            JObject.JInvokeVoidMethod("v_ggg", this.GetType(), this, t);
//        }

//        public String[] fldStrAry
//        {
//            get
//            {
//                return JObject.JGetFieldValue<string[]>("fldStrAry", _tInstanceType, this);
//            }
//        }

//        public int[] fldIntAry
//        {
//            get
//            {
//                return JObject.JGetFieldValue<int[]>("fldIntAry", this.GetType(), this);
//            }
//        }

//        public int?[] fldIObjAry
//        {
//            get
//            {
//                return JObject.JGetFieldValue<int?[]>("fldIObjAry", this.GetType(), this);
//            }
//        }

//        public virtual Dog[] fldDogAry
//        {
//            get
//            {
//                return JObject.JGetFieldValue<Dog[]>("fldDogAry", this.GetType(), this);
//            }
//            set
//            {
//                JObject.JSetFieldValue("fldDogAry", this.GetType(), this, value);
//            }
//        }

//        public void Print()
//        {
//            JObject.JInvokeVoidMethod("v_print", this.GetType(), this);
//        }

//        public virtual string Name
//        {
//            protected get
//            {
//                //this.GetLoader().ClearParams();
//                //object o = this.GetLoader().GetValueInvokeMethod(this.JObjectPtr, "getName", ValueTypeFlag.VF_string);
//                //return o.ToString();
//                return JObject.JGetPropertyValue<string>("Name",this.GetType(), this);
//            }
//            set
//            {
//                JObject.JSetPropertyValue("Name", this.GetType(), this, value);
//                //this.GetLoader().ClearParams();
//                //this.GetLoader().FillParamValue(value, true);
//                //this.GetLoader().GetObjectInvokeMethod(this.JObjectPtr, "setName");
//            }
//        }

//        public int Age
//        {
//            set
//            {
//                JObject.JSetPropertyValue("Age", this.GetType(), this, value);
//            }
//        }

//        public int? AgeI
//        {
//            set
//            {
//                JObject.JSetPropertyValue("AgeI", this.GetType(), this, value);
//            }
//        }

//        public Dog[] getDogAry()
//        {
//            return JObject.JInvokeResultMethod<Dog[]>("v_getDogAry", this.GetType(), this);
//        }

//        public void setDogAry(Dog[] args)
//        {
//            var oAry = new object[] { args };
//            JObject.JInvokeVoidMethod("v_setDogAry", this.GetType(), this, oAry);
//        }

//        public Dog[] DogAry
//        {
//            get
//            {
//                return JObject.JGetPropertyValue<Dog[]>("DogAry", this.GetType(), this);
//            }
//            set
//            {
//                JObject.JSetPropertyValue("DogAry", this.GetType(), this, new object[] { value });
//            }
//        }

//        public void SetFriend(Cat cat)
//        {

//            JObject.JInvokeVoidMethod("v_setfriend", this.GetType(), this, cat);
//        }

//        public void setStrAry(params String[] args)
//        {
//            var oAry = new object[] { args };
//            JObject.JInvokeVoidMethod("v_setStrAry", this.GetType(), this, oAry);
//        }

//        public void setStrAry2(String[] args)
//        {
//            var oAry = new object[] { args };
//            JObject.JInvokeVoidMethod("v_setfriend2", this.GetType(), this, oAry);

//            //args.GetType().IsArray
//            //this.GetLoader().ClearParams();
//            //this.GetLoader().FillParamValue(args, true, true, "[Ljava.lang.String;");
//        }

//        public String[] getStrAry2()
//        {
//            return JObject.JInvokeResultMethod<String[]>("v_getStrAry2", this.GetType(), this);
//        }

//        public void setFloatAry(float[] args)
//        {
//            var oAry = new object[] { args };
//            JObject.JInvokeVoidMethod("v_setfloat2", this.GetType(), this, oAry);
//        }



//        public float[] FloatAry
//        {
//            set
//            {
//                var oAry = new object[] { value };
//                JObject.JSetPropertyValue("FloatAry",this.GetType(), this, oAry);
//            }
//            get
//            {
//                return JObject.JGetPropertyValue<float[]>("FloatAry", this.GetType(), this);
//            }
//        }

//        public int?[] IntegerAry
//        {
//            set
//            {
//                var oAry = new object[] { value };
//                JObject.JSetPropertyValue("IntegerAry", this.GetType(), this, oAry);
//            }
//            get
//            {
//                return JObject.JGetPropertyValue<int?[]>("IntegerAry", this.GetType(), this);
//            }
//        }

//        public float[] getFloatAry()
//        {
//            return JObject.JInvokeResultMethod<float[]>("v_getFloatAry", this.GetType(), this);
//        }

//        public void setBoolAry(bool[] args)
//        {
//            var oAry = new object[] { args };
//            JObject.JInvokeVoidMethod("v_setbool", this.GetType(), this, oAry);
//        }

//        public bool[] getBoolAry()
//        {
//            return JObject.JInvokeResultMethod<bool[]>("v_getBoolAry", this.GetType(), this);
//        }

//        public void setByteAry(byte[] args)
//        {
//            var oAry = new object[] { args };
//            JObject.JInvokeVoidMethod("v_setbyte", this.GetType(), this, oAry);
//        }

//        public byte[] getByteAry()
//        {
//            return JObject.JInvokeResultMethod<byte[]>("v_getByteAry", this.GetType(), this);
//        }

//        public void setCharAry(char[] args)
//        {
//            var oAry = new object[] { args };
//            JObject.JInvokeVoidMethod("v_setchar", this.GetType(), this, oAry);
//        }

//        public char[] getCharAry()
//        {
//            return JObject.JInvokeResultMethod<char[]>("v_getCharAry", this.GetType(), this);
//        }

//        public void setShortAry(short[] args)
//        {
//            var oAry = new object[] { args };
//            JObject.JInvokeVoidMethod("v_setshort", this.GetType(), this, oAry);
//        }

//        public short[] getShortAry()
//        {
//            return JObject.JInvokeResultMethod<short[]>("v_getShortAry", this.GetType(), this);
//        }

//        public void setIntAry(int[] args)
//        {
//            var oAry = new object[] { args };
//            JObject.JInvokeVoidMethod("v_setint", this.GetType(), this, oAry);
//        }

//        public int[] getIntAry()
//        {
//            return JObject.JInvokeResultMethod<int[]>("v_getIntAry", this.GetType(), this);
//        }

//        public int?[] getIntegerAry()
//        {
//            return JObject.JInvokeResultMethod<int?[]>("v_getIntegerAry", this.GetType(), this);
//        }

//        public void setLongAry(long[] args)
//        {
//            var oAry = new object[] { args };
//            JObject.JInvokeVoidMethod("v_setlong", this.GetType(), this, oAry);
//        }

//        public long[] getLongAry()
//        {
//            return JObject.JInvokeResultMethod<long[]>("v_getLongAry", this.GetType(), this);
//        }

//        public void setDoubleAry(double[] args)
//        {
//            var oAry = new object[] { args };
//            JObject.JInvokeVoidMethod("v_setdouble", this.GetType(), this, oAry);
//        }

//        public double[] getDoubleAry()
//        {
//            return JObject.JInvokeResultMethod<double[]>("v_getDoubleAry", this.GetType(), this);
//        }
//    }
//}
