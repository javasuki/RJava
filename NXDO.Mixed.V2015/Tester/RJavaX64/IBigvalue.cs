using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NXDO.RJava.Attributes;

namespace RJavaX64
{
    [JEmit("rt.bigvalue")]
    public interface IBigvalue : NXDO.RJava.JIBase
    {
        int GetAge2();
    }
}
