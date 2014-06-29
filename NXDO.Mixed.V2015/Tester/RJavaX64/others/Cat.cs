//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using NXDO.JRuntime.Attributes;
//using NXDO.JRuntime.JVM;

//namespace RJavaX64
//{
//    [JClass("jfor2.Cat")]
//    public class Cat : JObject
//    {
//        protected Cat(IntPtr javaObjectPtr, IntPtr javaClassPtr)
//            : base(javaObjectPtr, javaClassPtr)
//        {
//        }

//        public Cat()
//        {
//            JSuper();
//        }

//        [JMethod("Say")]
//        public void Say()
//        {
//            JObject.JInvokeVoidMethod("v_say", this.GetType(), this);
//        }
//    }
//}
