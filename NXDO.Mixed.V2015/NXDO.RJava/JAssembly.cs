using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

using NXDO.RJava.Attributes;

namespace NXDO.RJava
{
    /// <summary>
    /// 表示一个或多个 java 程序集，它是一个可自我描述的 JVM 运行时应用程序构造块。
    /// </summary>
    public class JAssembly
    {

        #region static/私有 ctor
        static JAssembly()
        {
            JAssembly.IsJarFileZeroCountThrow = true;
        }

        private JAssembly() 
        {
            JdkVersion = 6; 
        }
        #endregion

        #region 程序集内部一些设置参数，可在JVM建立前设置
        /// <summary>
        /// 是否检查需要加载的JAR文件数，true：0个文件时抛出异常，反之无异常。
        /// <para>默认为true.</para>
        /// <para>提供给生成器使用 JarFinder （dotnet中包装类）</para>
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal static bool IsJarFileZeroCountThrow
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// JDK版本
        /// </summary>
        public static int JdkVersion
        {
            get;
            set;
        }

        #region 建立 java 运行环境
        /// <summary>
        /// java 核心的装载与执行器
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static JRunCore jReflection;

        /// <summary>
        /// 建立 java 运行环境
        /// <para>JObject.JSuper中调用（仅一次初始C++中的JVM环境）</para>
        /// </summary>
        internal static JRunCore JBridgeContext
        {
            get
            {
                if (JAssembly.jReflection != null)
                    return JAssembly.jReflection;

                #region java 类库文件
                SortedList<int, string> lst = new SortedList<int, string>();
                var dotnetAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var dotAsm in dotnetAssemblies)
                {
                    //一旦获取，则对应的静态集合中，已经包含当前程序下定义的所有 jar
                    var jasms = dotAsm.GetCustomAttributes(typeof(JAssemblyAttribute), false);
                    if (jasms.Length == 0) continue;

                    //合并所有程序中定义的 jar (已经去除空字符串)
                    foreach (var data in JAssemblyAttribute.JarFiles)
                    {
                        if (lst.Values.Contains(data.Value)) continue;
                        lst.Add(data.Key, data.Value);
                    }
                }

                if (JAssembly.IsJarFileZeroCountThrow && lst.Count == 0)
                    throw new NotImplementedException("程序集中未发现java程序集加载标识，尝试加载失败。");


                string allJarNames = string.Join(";", GetJarsFullName(lst.Values));
                #endregion

                JRunEnvironment jre = JRunEnvironment.Create(JAssembly.JdkVersion); //单实例对象
                JAssembly.jReflection = jre.LoadBridge(allJarNames, JAssembly.IsJarFileZeroCountThrow); //单实例对象
                return JAssembly.jReflection;
            }
        }                


        
        #region 生成 java 类库文件名 相关方法
        /// <summary>
        /// 获取存在的多个带有PATH的 jar 文件名。
        /// </summary>
        /// <param name="lstNames">来源配置或设置，以；分隔的多个绝对或相对路径的 jar 文件名。</param>
        /// <param name="isAddToAssemblyFiles"></param>
        /// <returns></returns>
        private static List<string> GetJarsFullName(IEnumerable<string> lstNames, bool isAddToAssemblyFiles = true)
        {
            List<string> lstJarNames = new List<string>();
            foreach (var name in lstNames)
            {
                string[] names = name.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (names.Length == 0) continue;
                SetJavaFileNames(names, lstJarNames);
            }

            //if (lstJarNames.Count == 0)
            //    throw new NotImplementedException("程序集中未发现java程序集加载标识，尝试加载失败。");

            DeleteSameFileName(lstJarNames);

            if (isAddToAssemblyFiles)
                lstJarNames.ForEach(f => _JAssemblyFiles.Add(f));

            return lstJarNames;
        }

        private static string BaseDirectory
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_BaseDirectory))
                {
                    //String ss  = typeof(JAssembly).Assembly.EscapedCodeBase;
                    String sCodebase = typeof(JAssembly).Assembly.CodeBase;
                    if (sCodebase.StartsWith("file:"))
                        sCodebase = sCodebase.Replace("file:///", "");
                    else
                        throw new Exception("探测程序集不支持从远程文件系统中创建JVM环境。");
                    _BaseDirectory = new FileInfo(sCodebase).DirectoryName;
                    //_BaseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                }
                return _BaseDirectory;
            }
        }static string _BaseDirectory;

        /// <summary>
        /// 设置需要加载多个文件名称
        /// </summary>
        /// <param name="names"></param>
        /// <param name="lstJarNames"></param>
        private static void SetJavaFileNames(string[] names, List<string> lstJarNames)
        {
            foreach (string name in names)
            {
                List<string> tmpNames = new List<string>();
                FileInfo finfo = new FileInfo(name);
                if (string.IsNullOrWhiteSpace(finfo.Extension))
                {
                    tmpNames.Add(name + ".jar");
                    tmpNames.Add(name + ".zip");
                }
                else
                    tmpNames.Add(name);

                int iCount = tmpNames.Count;
                for (int i = 0; i < iCount; i++)
                {
                    string tmpName = tmpNames[i];
                    if (tmpName.IndexOf(Path.VolumeSeparatorChar) < 0)
                    {
                        bool hasJlibsPath = !(tmpName.IndexOf("jlibs" + Path.DirectorySeparatorChar) < 0); //是否存在 jlibs 目录
                        if (tmpName.StartsWith(Path.DirectorySeparatorChar + ""))
                        {
                            if (!hasJlibsPath)
                            {
                                iCount += 1;
                                tmpNames.Add("jlibs" + tmpName);
                            }
                            tmpName = tmpName.Substring(1);
                        }
                        else
                        {
                            if (!hasJlibsPath)
                            {
                                iCount += 1;
                                tmpNames.Add("jlibs" + Path.DirectorySeparatorChar + tmpName);
                            }
                        }

                        //不存在卷标分隔符，则为相对路径
                        tmpName = Path.Combine(BaseDirectory, tmpName);
                        if (!File.Exists(tmpName)) continue;
                        lstJarNames.Add(tmpName);
                        continue;
                    }

                    //存在卷标分隔符，则为绝对路径
                    if (!File.Exists(tmpName)) continue;
                    lstJarNames.Add(tmpName);
                }//end 2 for   
            }//end 1 for
        }

        /// <summary>
        /// 只保留不同目录下一个同名文件。
        /// </summary>
        /// <param name="lstJarNames"></param>
        private static void DeleteSameFileName(List<string> lstJarNames)
        {
            List<string> lstFNames = new List<string>();
            for (int i=0; i< lstJarNames.Count; i++)
            {
                string name = lstJarNames[i];
                FileInfo finfo = new FileInfo(name);
                int icount = (from a in lstFNames where name.EndsWith(a) select a).Count();
                if (icount == 0)
                {
                    lstFNames.Add(finfo.Name);
                    continue;
                }

                lstJarNames.RemoveAt(i);
                i -= 1;
            }
        }
        #endregion
        #endregion

        /// <summary>
        /// 获取加载到 jvm 中的类库文件名
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<string> GetJAssemblyFiles()
        {
            return _JAssemblyFiles;

        }static private List<string> _JAssemblyFiles = new List<string>();

        /// <summary>
        /// 加载 java 程序集。
        /// </summary>
        /// <param name="jarNames">java程序集的文件名或路径，以；分隔的多个绝对或相对路径的 jar 文件名。</param>
        public static void LoadFrom(string jarNames)
        {
            var lst = GetJarsFullName(jarNames.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries), false);

            //是否与已加载的jar同名，同名则不再加载
            for (int i = 0; i < lst.Count; i++)
            {
                string name = new FileInfo(lst[i]).Name;
                int icount = (from a in _JAssemblyFiles where a.EndsWith(name) select a).Count();
                if (icount == 0)
                    continue;

                lst.RemoveAt(i);
                i -= 1;
            }

            if (lst.Count == 0) return;
            string allJarNames = string.Join(";", lst);
            lst.ForEach(f => _JAssemblyFiles.Add(f));

            //var jl = JRunEnvironment.Create().Load(allJarNames, JAssembly.IsJarFileZeroCountThrow);
            //if (JAssembly.jLoader == null) JAssembly.jLoader = jl;

            var jrfn = JRunEnvironment.Create().LoadBridge(allJarNames, JAssembly.IsJarFileZeroCountThrow);
            if (JAssembly.jReflection == null) JAssembly.jReflection = jrfn;
        }

        #region 静态释放
        /// <summary>
        /// 释放整个 jvm 资源，仅在系统退出时调用。
        /// </summary>
        public static void Dispose()
        {
            _JAssemblyFiles.Clear();
            if (jReflection == null) return;
            JRunEnvironment.FreeBridge();
            jReflection = null;
        }
        #endregion
    }
}
