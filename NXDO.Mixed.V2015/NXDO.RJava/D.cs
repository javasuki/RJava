using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace NXDO.RJava
{
    class D
    {
        public static void Write(object o){
            Debug.WriteLine(o);
        }

        public static void Write(params object[] os)
        {
            for (int i = 0; i < os.Length; i++)
            {
                string s = new string('\t', i);
                Debug.Write(s);
                Debug.Write(os[i]);
            }
            Debug.Write("\r\n");
        }

        public static void Write(string v)
        {
            Debug.WriteLine(v);
        }

        public static void Write(string format,params object[] os)
        {
            D.Write(string.Format(format, os));
        }
    }
}
