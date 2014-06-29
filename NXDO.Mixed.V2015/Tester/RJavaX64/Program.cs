using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using NXDO.RJava;
using NXDO.RJava.Core;
using NXDO.RJava.Reflection;
using NXDO.RJava.Extension;


namespace RJavaX64
{
    class Program
    {
        public static void test(params string[] str)
        {
            Console.WriteLine("invoke");
        }

        static void Main(string[] args)
        {
            

            //var jc = JClass.ForName("rt.Hello");
            //dynamic dy = jc.NewInstance().ToRJavaDynamic();
            NXDO.RJava.JAssembly.LoadFrom("jforloaded.jar");

            var ival = JActivator.CreateByInterface<IBigvalue>();
            var tt = ival.GetAge2();
            var jc = ival.GetClass();

            //JObject jo = ival;

            dynamic dy = JActivator.CreateDynamic("rt.Hello");
            dy.print();
            dy.print("xx");
            dy.print(1);
        }

        private void t123()
        {
            //var ttt = new IfaceTest();
            //var ittest = new TTest();
            //Console.WriteLine("c# : age = " + ittest.Age);
            //ttt.TFace(ittest);

            //var nobj = new Java.Rt.NClass();
            //var nobj2 = new Java.Rt.NClass.AClass(nobj);
            //var nobj3 = new Java.Rt.NClass.AClass.BClass(nobj2);

            //nobj3.Ttt();


            ////AOO.COO
            //ParameterInfo pi = null;
            //var gt = typeof(List<>);
            //bool b0 = gt.IsGenericParameter;
            //bool b1 = gt.IsGenericType;
            //bool b2 = gt.IsGenericTypeDefinition;
            //Type[] ts = gt.GetGenericArguments();


            //var jdemoe = JClass.ForName("rt.DemoE");
            //var joDemoE = JActivator.CreateInstance(jdemoe);
            
            //var m = jdemoe.GetMethod("SetTValue");
            //var ps = m.GetParameters();
            ////m.Invoke(joDemoE, new object[] { 1 });

            ////var jc = JClass.ForName("java.util.ArrayList");
            //JObject jo = JActivator.CreateInstance("java.util.ArrayList");
            ////JClass jClass = jo.GetClass();
            ////JMethod jMethod = jClass.GetMethod("add", JClass<int>.Class, JObject.Class);
            ////jMethod.Invoke(jo, new object[] { 1 });
            ////object jresult = jMethod.Invoke(jo, new object[] { "xxx" });

            //dynamic dyJO = joDemoE.ToRJavaDynamic();
            //dyJO.SetTValue(2);
        }



        private void testdic()
        {
            var jc = JClass.ForName("java.util.Map$Entry");

            var dics = new Dictionary<int,string>();
            dics.Add(1, "xx");
            dics.Add(2, "yy");
            JDictionary<int, string> jd = dics.ToJavaDictionary();
            foreach (var kvp in jd)
            {
                Console.WriteLine(kvp.Value);
            }

            string s0 = jd[1];
            string s1 = jd[2];
            jd[1] = "zz";
        }

        private void testList()
        {

            
            DateTime dt = new DateTime(2012, 1, 1);
            string ss = dt.ToString();

            var type = typeof(Program);
            //Type.GetType
            //type.GetInterfaces()


            var lst = new List<int?>();
            lst.Add(12);
            lst.Add(34);


            JList<int?> jlst = lst.ToJavaList();
            var jarraylist = JClass.AsCast("java.util.ArrayList", jlst);

            var clazz = JClass.ForName("java.util.ArrayList");
            var jobj = JActivator.CreateInstance(clazz);
            var method = clazz.GetMethod("add", JClass<JObject>.Class);
            var isOK = method.Invoke<bool>(jobj, new object[] { 1 });
            isOK = method.Invoke<bool>(jobj, new object[] { "cc" });


            

            var ulst = jobj.ToRJavaDefine<JList<JObject>>();
            int i = ulst[0].ToDotValue<int>();
            string s = ulst[1].ToDotValue<string>();

            Console.WriteLine("");
            Console.WriteLine(">>> 按下任意键结束。");
            Console.ReadKey(true);
            JAssembly.Dispose();

            ////JObject.Class
            
            //DemoE demoE = new DemoE();
            //demoE.InClass(DemoE.Class);
            //demoE.InClassAry(new JClass[]{demoE.GetClass(), JClass.ForName("int")});
            //var demoClass = demoE.GetClass();
            //demoE.AddList(jlst);
            //demoE.AddArrayList( jarraylist );
            //demoE.TestO( JObject.Class.AsCast(jarraylist));

            //var lst2 = jlst.ToList();
            
            ////dynamic jArylist = JActivator.CreateDynamic(DemoE.Class, false);//"java.util.ArrayList");
            ////object vv = jArylist.TestO(1);
            ////int vvv = vv.ToDotValue<int>();
            ////var cls = JClass.ForName("java.util.ArrayList");
            ////var icls = cls.GetInterface("java.util.List");

            //var demo = new DemoE();
            ////dynamic dyDemo = demo.ToRJavaDynamic();
            
            //var dClazz = DemoE.Class;
            //var fld = dClazz.GetField("vvv");
            ////fld.SetValue(null, new int[] { 1, 2, 3 });
            //JObject[] jAry = (JObject[])fld.GetValue(null);
            //int[] iAry = fld.GetValue<int[]>(null);
            
            //JConstructor[] ctor = dClazz.GetConstructors();
            ////var dClass = ctor[0].Invoke<

            
            //var obj = demo.TestO(1);
            //var jobj = (JObject)obj;
            //var clazz = jobj.GetClass();

            
            //var m = demo.GetClass().GetMethod("TestDT", new[]{ JClass<DateTime>.Class });
            //var o = m.Invoke<DateTime>(demo, new object[]{ new DateTime(2013, 1, 1) });

            //var dAry = demo.TestDTAry(new DateTime[] { dt, new DateTime(2013, 1, 1) });
            //var objAry = demo.TestOAry(new object[] { dt, new DateTime(2013, 12, 12), new DateTime(2013, 1, 1) });
            //var dnullAry = objAry.ToDotValue<DateTime?>();

            ////var oDT = demo.TestO(dt);
            ////var dtC = oDT.ToJavaValue<DateTime>();
            ////var dtr = demo.TestDT(null);

            ////var jr = demo.TestO(OEnum.Property);
            ////OEnum or = jr.ToJavaValue<OEnum>();

            ////var jrs = demo.TestOAry(new object[] { OEnum.Property, OEnum.Field, OEnum.Mehotd });
            ////var ors = jrs.ToJavaValue<OEnum>();

            ////OEnum o = demo.TestEnum(OEnum.Field);

            ////OEnum[] oary = demo.TestEnumAry(new OEnum[] { OEnum.Field, OEnum.Mehotd });

            

            
        }

        void ttt2()
        {
            var demo = new Demo();
            //var bbary = demo.Say(new bool[] { true,false,false });
            //var bbary2 = demo.Say(new bool?[] { false, true, null });

            //var byary = demo.Say(new byte[] { 0x01,0x02 });
            //var byary2 = demo.Say(new byte?[] { 0x01,0x02, null });

            //var bcary = demo.Say(new char[] { 'a','b' });
            //var bcary2 = demo.Say(new char?[] { 'a','c', null });

            //var bsary = demo.Say(new short[] { (short)1, (short)2 });
            //var bsary2 = demo.Say(new short?[] { (short)1, (short)2, null });

            //var biary = demo.Say(new int[] { 100,200 });
            //var biary2 = demo.Say(new int?[] { 100, 200, null });

            //var blary = demo.Say(new long[] { 101, 201 });
            //var blary2 = demo.Say(new long?[] { 101, 201, null });

            //var bfary = demo.Say(new float[] { 101.1F, 202.2F });
            //var bfary2 = demo.Say(new float?[] { 101.1F, 202.2F, null });

            //var ary = demo.Say(new double[] { 1D, 2D, 3D });
            //var ary2 = demo.Say(new double?[] { 1D,2D, null });

            //var sary = demo.Say(new string[] { "xx", "yy", null, string.Empty });
            //var demos = demo.Say(new Demo[] { demo, demo, null, new Demo("gg") });

            //var r1 = demo.SayObject(demo);
            //var demo2 = r1.ToCastJValue<Demo>();

            //var r2 = demo.SayObject(100);
            //var int2 = r2.ToCastJValue<int>();

            var os1 = demo.SayObjectAry(new object[] { 1, 2, 4, (int?)null, "abc", 'a', new Demo("gg") });
            var i1 = os1[0].ToDotValue<int>();
            //var i2 = os1[1].ToJavaValue<int>();
            //var i3 = os1[2].ToJavaValue<int>();
            //var i4 = os1[3].ToJavaValue<int?>();
            //var s5 = os1[4].ToJavaValue<string>();
            //var c5 = os1[5].ToJavaValue<char>();
            //var d6 = os1[6].ToJavaValue<Demo>();


            var os2 = demo.SayObjectAry(new JObject[] { demo, demo, null, new Demo("gg") });
            foreach (Demo o in os2.ToDotValue<Demo>())
                Console.WriteLine((object)o);

            
            Console.WriteLine("");
            Console.WriteLine(">>> 按下任意键结束。");
            Console.ReadKey(true);
            JAssembly.Dispose();
        }

        void ttt()
        {
            //Demo.TestStatic();
            int ibox = 0;
            //JObject jbox = ibox;

            //Demo.FieldSI = 3000;
            //int fsi = Demo.FieldSI;
            //int fsi2 = Demo.FieldSI;
            //Demo.FieldSI = 500;

            var demoProp = new Demo();
            //demoProp.DObject((int)1);
            //demoProp.DTObj<int?>((int?)555);
            //demoProp.DTObj<Demo>(demoProp);

            //demoProp.DTObjAry<int?>(new int?[] { 1, 2, null, 5 });
            //demoProp.DTObjAry<Demo>(new Demo[] { demoProp, demoProp, null, demoProp });

            //demoProp.DDemo(demoProp);
            //demoProp.DDemoAry(new Demo[] { demoProp, null });
            //demoProp.DObject((int)1);
            //demoProp.DObject((string)"xx");
            //Console.WriteLine();

            //demoProp.DObjectAry(new JObject[] { 1, "xx", (int?)2, (int?)null, 2.0, demoProp });
            object returnvalue = null;
            returnvalue = demoProp.DBool(true);
            returnvalue = demoProp.DBool(false);

            demoProp.DBool((bool?)true);
            demoProp.DBool((bool?)false);
            //demoProp.DBool(null);
            //demoProp.DBoolArray((bool?[])null);
            //demoProp.DBoolArray(new bool?[] { true, false, null });

            //demoProp.DString("xxx");
            //demoProp.DStringAry(new string[]{"xxx", "vvv"});
            //demoProp.DStringAry(null);

            ////demoProp.SayT<int?>(5);
            ////demoProp.SayT<int?>(new int?[] {  });
            //int joInt = demoProp.SayT<JObject>(5);
            //string joStr = demoProp.SayT<JObject>("xxx");

            //JObject joDemo = demoProp.SayT<JObject>(new Demo("vvv"));
            //var jorDemo = joDemo.ToCast<Demo>();

            //string[] joAryRs2 = demoProp.SayT<String>(new string[] { "string", "xxx" });


            JObject[] joAryRs = demoProp.SayT<JObject>(new JObject[] { new Demo("gg"), new Demo("gg") }); 
            foreach(var o in joAryRs){
                var joDemos = o.ToDotValue<Demo>();
            }
            //int agettt = demoProp.Age;
            //demoProp.Age = 2;
            //agettt = demoProp.Age;

            
            demoProp.SayObject(null);
            demoProp.SayObject(5);
            demoProp.SayObject(true);
            demoProp.SayObject('a');
            demoProp.SayObject(1.0F);
            demoProp.SayObject(1.1D);
            demoProp.SayObject("vvb");
            demoProp.SayObject(new Demo("vvv"));

            ////new Demo(true);
            ////new Demo((bool?)null);
            ////new Demo(new bool[] { true, false });
            ////new Demo(new bool?[] { true, false, null });
            //JObject dm0 = new Demo();
            //new Demo(dm0);

            //var dm1 = new Demo();
            //dm1.SayObject(new JObject[] { dm0, dm1 });

            //var dm2 = new Demo();
            //new Demo(dm1);
            //new Demo(new Demo[] { dm1,dm2 });

            //var demo = new Demo(new string[]{"cc","tt"});
            //Demo.DemoReturn<Demo>(demo);
            //Demo.DemoReturnArray<Demo>(demo,demo);
            ////Demo.DemoArray(demo);
            ////Demo.DemoT<Demo>(demo, demo);
            //Demo.DemoTArray<Demo>(demo, demo);

            //Console.WriteLine("");
            //Console.WriteLine(">>> 按下任意键结束。");
            //Console.ReadKey(true);

            //JAssembly.Dispose();
            //if (1 == 1) return;

            //Array ary = (Array)(new int[] { 2, 3, 4 });
            //Type tt = ary.GetType();
            //if (tt.IsArray)
            //{
            //    Type ttt = tt.GetElementType();
            //}

            
        }
    }
}
