using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using NXDO.RJava.Core;
using NXDO.RJava.Attributes;

namespace NXDO.RJava.Reflection
{
    [JClass("jxdo.rjava.JParamInfo")]
    class JParamInfoInternal : JObject
    {
        protected JParamInfoInternal(IntPtr objectPtr, IntPtr classPtr)
            : base(objectPtr, classPtr)
        {
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isGetGenericParamName = false;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string sGenericParamName;
        public string GenericParamName
        {
            get
            {
                if (!isGetGenericParamName)
                {
                    isGetGenericParamName = true;
                    var ptr = JObject.JContext.JInvoke(this.Handle, "getGenericParamName", JParamValue.GetParams());
                    sGenericParamName = new JMReturn<string>(ptr).Value;
                }
                return sGenericParamName;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isGetTypeClass = false;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private JClass cTypeClass;
        public JClass TypeClass
        {
            get
            {
                if (!isGetTypeClass)
                {
                    isGetTypeClass = true;
                    var ptr = JObject.JContext.JInvoke(this.Handle, "getTypeClass", JParamValue.GetParams());
                    cTypeClass = new JClass(ptr);
                }
                return cTypeClass;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isGetIsGeneric = false;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isGeneric;
        public bool IsGenericParameter
        {
            get
            {

                if (!isGetIsGeneric)
                {
                    isGetIsGeneric = true;
                    var ptr = JObject.JContext.JGetField(this.Handle, "IsGeneric");
                    isGeneric = new JMReturn<bool>(ptr).Value;
                }
                return isGeneric;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isGetIsGenericType = false;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool isGenericType;
        public bool IsGenericType
        {
            get
            {
                if (!isGetIsGenericType)
                {
                    isGetIsGenericType = true;
                    var ptr = JObject.JContext.JGetField(this.Handle, "IsGenericType");
                    isGenericType = new JMReturn<bool>(ptr).Value;
                }
                return isGenericType;
            }
        }

        public JParameter GetJParameter()
        {
            var jpr = new JParameter();
            jpr.ParameterClass = this.TypeClass;
            jpr.GenericParamName = this.GenericParamName;
            jpr.IsGenericClass = this.IsGenericType;
            jpr.IsGenericParameter = this.IsGenericParameter;
            return jpr;
        }
    }

}
