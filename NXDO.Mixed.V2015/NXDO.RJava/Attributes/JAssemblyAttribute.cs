using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NXDO.RJava.Attributes
{
    /// <summary>
    /// 标识 java class 打包后的程序集 (jar或zip) 文件名称。
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class JAssemblyAttribute : System.Attribute
    {
        static JAssemblyAttribute()
        {
            JarFiles = new SortedList<int,string>();
        }

        internal static SortedList<int,string> JarFiles
        {
            get;
            set;
        }

        /// <summary>
        /// 标识java程序集
        /// </summary>
        /// <param name="jarNames">java程序集名称。<para>支持使用相对路径，可用;分隔多个程序集名称。</para></param>
        public JAssemblyAttribute(string jarNames)
            : this(jarNames, JarFiles.Count)
        {
        }

        /// <summary>
        /// 标识java程序集
        /// </summary>
        /// <param name="jarNames">java程序集名称。<para>支持使用相对路径，可用;分隔多个程序集名称。</para></param>
        /// <param name="loadIndex">加载次序</param>
        public JAssemblyAttribute(string jarNames,int loadIndex)
        {
            if (string.IsNullOrWhiteSpace(jarNames)) return;
            if (JAssemblyAttribute.JarFiles.Values.Contains(jarNames)) return;

            //List<string> jars = new List<string>();
            //FileInfo finfo = new FileInfo(jarName);
            //if(String.IsNullOrWhiteSpace(finfo.Extension))
            //{
            //    jars.Add(".jar");
            //    jars.Add(".zip");
            //}

            if (JAssemblyAttribute.JarFiles.ContainsValue(jarNames)) return;
            bool bHasLoadIndex = JAssemblyAttribute.JarFiles.ContainsKey(loadIndex);
            while (bHasLoadIndex)
            {
                loadIndex += 1;
                bHasLoadIndex = JAssemblyAttribute.JarFiles.ContainsKey(loadIndex);
            }

            JAssemblyAttribute.JarFiles.Add(loadIndex, jarNames);
        }

    }
}
