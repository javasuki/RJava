//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using System.Reflection;

//using NXDO.JBridge;
//using NXDO.JRuntime.JVM;
//using NXDO.JRuntime.Attributes;

//using Jacoste.Rt;

//namespace RJavaX64
//{    
//    [JClass("rt.Person")]
//    public partial class Person : JObject
//    {
//        protected Person(IntPtr javaObjectPtr, IntPtr javaClassPtr) : 
//            base(javaObjectPtr, javaClassPtr)
//        {
//        }

//        //jarLoader.ClearParams();
//        //jarLoader.GetObjectInvokeMethod(this.JClassPtr, "doStatic", true);
//        public Person()
//        {
//            this.JSuper();
//        }

//        public int? Age
//        {
//            get
//            {
//                return JObject.JGetPropertyValue<int?>("Age", typeof(Person), this);
//            }
//            set
//            {
//                JObject.JSetPropertyValue("Age", typeof(Person), this, value);
//            }
//        }

//        public int? getAge()
//        {
//            return JObject.JInvokeResultMethod<int?>("_v_get_age", this.GetType(), this);
//        }

//        [JField("Flag")]
//        public int Flag
//        {
//            get
//            {
//                return JObject.JGetFieldValue<int>("Flag", typeof(Person),this );
//            }
//            //final Flag,外部只读，如果有SET，跳过JAVA final 功能，反射赋值
//            //set
//            //{
//            //    JObject.JSetFieldValue("Flag",value, typeof(Person),this );
//            //}
//        }

        
//        [JField("Flag2")]
//        public static int Flag2
//        {
//            get
//            {
//                return JObject.JGetFieldValue<int>("Flag2", typeof(Person), null);
//            }
//        }

//        [JField("Flag3")]
//        public static int? Flag3
//        {
//            get
//            {
//                return JObject.JGetFieldValue<int?>("Flag3", typeof(Person), null);
//            }
//        }

//        [JField("Flag4")]
//        public int? Flag4
//        {
//            get
//            {
//                return JObject.JGetFieldValue<int?>("Flag4", typeof(Person), this);
//            }
//            set
//            {
//                JObject.JSetFieldValue("Flag4",  typeof(Person), this,value);
//            }
//        }


//        public Person(int age)
//        {
//            this.JSuper(age);
//        }

//        public Person(int age, Dog dog)
//        {
//            this.JSuper(age,dog);
//        }

//        public virtual Dog Dog
//        {
//            [JMethod]
//            get
//            {
//                return JObject.JGetPropertyValue<Dog>("Dog",this.GetType(), this);
//            }

//            [JMethod]
//            set
//            {
//                JObject.JSetPropertyValue("Dog", this.GetType(), this, value);
//            }
//        }



//        //public virtual int Age
//        //{
//        //    [JMethod, JValue(false)]
//        //    get
//        //    {
//        //        return JObject.JGetPropertyValue<int>("Age", this.GetType(), this, null);
//        //        //this.GetLoader().ClearParams();
//        //        //object o = this.GetLoader().GetValueInvokeMethod(this.JObjectPtr, "getAge", ValueTypeFlag.VF_int);
//        //        //return Convert.ToInt32(o);
//        //    }
//        //    [JMethod, JValue(true)]
//        //    set
//        //    {
//        //        JObject.JSetPropertyValue("Age", value, this.GetType(), this, null);

//        //        //this.GetLoader().ClearParams();
//        //        //this.GetLoader().FillParamValue(value, true);
//        //        //this.GetLoader().GetObjectInvokeMethod(this.JObjectPtr, "setAge");
//        //    }

//        //}

//        public virtual string XXX
//        {
//            get
//            {
//                return JObject.JGetPropertyValue<string>("XXX", this.GetType(), this);
//            }
//        }

//        [JMethod]
//        public virtual void DoPrint()
//        {
//            //this.GetLoader().ClearParams();
//            //this.GetLoader().GetObjectInvokeMethod(this.JObjectPtr, "doPrint");

//            JObject.JInvokeVoidMethod("v_DoPrint",this.GetType(), this);
//        }

//        [JMethod]
//        public virtual void DoString(string dd)
//        {
//            JObject.JInvokeVoidMethod("v_DoString_string",this.GetType(), this, dd);
//            //this.GetLoader().ClearParams();
//            //this.GetLoader().FillParamValue(dd, true);
//            //this.GetLoader().GetObjectInvokeMethod(this.JObjectPtr, "doString");
//        }

//        [JMethod]
//        public void DoDogTest(String dd, Dog dog)
//        {
//            JObject.JInvokeVoidMethod("v_DoDogTest_string_dog",this.GetType(), this, dd, dog);
//        }

//        //[JMethod]
//        //public Dog GetReturnDog(String dd)
//        //{
//        //    return JObject.JInvokeResultMethod<Dog>("dog_GetReturnDog_string",this.GetType(), this, dd);
//        //}

//        //public int getReturnInt(String ss)
//        //{
//        //    return JObject.JInvokeResultMethod<int>("i_getReturnInt_string",this.GetType(), this, ss);
//        //}
        

//        //public String getReturnString(String ss)
//        //{
//        //    return JObject.JInvokeResultMethod<string>("s_getReturnString_string",this.GetType(), this, ss);
//        //}

//        [JMethod]
//        public static void DoStatic()
//        {
//            JObject.JInvokeVoidMethod("_v_DoStatic", typeof(Person), null);
//            //this.GetLoader().ClearParams();
//            //this.GetLoader().GetObjectInvokeMethod(this.JClassPtr, "doStatic", true);
//        }

//        //[JMethod]
//        //public static int DoStatic2()
//        //{
//        //    return JObject.JInvokeResultMethod<int>("_v_DoStatic2", typeof(Person), null);
//        //}

//        //[JMethod]
//        //public static Cat DoStatic3(Dog dog)
//        //{
//        //    return JObject.JInvokeResultMethod<Cat>("_v_DoStatic3", typeof(Person), null, dog);
//        }

//        [JMethod]
//        public virtual int GetAge()
//        {
//            return 0;
//        }


//    }

//    [JClass("rt.Man")]
//    public class Man : Person
//    {
//        protected Man(IntPtr javaObjectPtr, IntPtr javaClassPtr) : 
//            base(javaObjectPtr, javaClassPtr)
//        {
//        }

//        public Man() : base()
//        {
//        }

//        public override void DoString(string dd)
//        {
//            JObject.JInvokeVoidMethod("v_dostring_string", this.GetType(), this, dd);
//        }

        

//        [JField("Name")]
//        public string Name
//        {
//            get
//            {
//                return JObject.JGetFieldValue<string>("Name", this.GetType(), this);
//            }
//            set
//            {
//                JObject.JSetFieldValue("Name", this.GetType(), this, value);
//            }
//        }

//        [JField("MyCat")]
//        public Cat MyCat
//        {
//            get
//            {
//                return JObject.JGetFieldValue<Cat>("MyCat", this.GetType(),this);
//            }
//            set
//            {
//                JObject.JSetFieldValue("MyCat", this.GetType(), this, value);
//            }
//        }

//        [JField("MyCat2")]
//        public static Cat MyCat2
//        {
//            get
//            {
//                return JObject.JGetFieldValue<Cat>("MyCat2", typeof(Man), null);
//            }
//            set
//            {
//                JObject.JSetFieldValue("MyCat2", typeof(Man), null, value);
//            }
//        }
//    }
//}
