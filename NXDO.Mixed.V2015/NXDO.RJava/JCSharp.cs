using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NXDO.RJava;
using NXDO.RJava.Attributes;


namespace NXDO.RJava
{
    [JClass("jxdo.rjava.JCSharp")]
    internal class JCSharp : JObject
    {
        static JCSharp()
        {
            JAssembly.IsJarFileZeroCountThrow = false;
        }

        protected JCSharp(IntPtr objInstance, IntPtr clzInstance)
            : base(objInstance, clzInstance)
        {
        }

        public JCSharp(String jarNames)
        {
            JSuper(jarNames);
        }

        public void FillClasses(String directoryName)
        {
            JObject.JInvokeMethod(typeof(JCSharp), "fillClasses(java.lang.String)", this, new object[] { directoryName });
        }
    }
}
