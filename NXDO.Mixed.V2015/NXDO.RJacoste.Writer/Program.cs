using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NXDO.RJava;

namespace NXDO.RJacoste.Writer
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isInAddin = false;
            string sMark = "";
            foreach (var s in args)
            {
                if (s.CompareTo("RJava.ForJVM.ByJObject-javasuki(ZhuQi)") == 0)
                {
                    isInAddin = true;
                    continue;
                }
                sMark += sMark.Length == 0 ? s : " " + s;
            }
            if (!isInAddin) return; //不存在标识,退出

            try
            {
                new RunWriter(sMark).execute();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //Console.Read();
        }

        class RunWriter
        {
            string srcToDir;
            public RunWriter(string prjName){
                
                this.alJars = new List<string>();
                this.alJarsName = new List<string>();

                string path = Path.GetDirectoryName(prjName); //prjName可能是目录，则返回它的上层目录
                srcToDir = Path.Combine(path, "nxdo.java");
                if (!Directory.Exists(srcToDir))
                    srcToDir = Path.Combine(prjName, "nxdo.java");

                //this.listDir(path);
                this.listFile(path);
            }

            public void execute(){
                String jarfname = String.Join(";", alJars.ToArray());
                JCSharp jf = new JCSharp(jarfname);
                jf.FillClasses(srcToDir);



                StringBuilder sb = new StringBuilder();
                string sinfo = Path.Combine(Path.GetDirectoryName(srcToDir), "Properties"+ Path.DirectorySeparatorChar +"AssemblyInfo.cs");
                if (!File.Exists(sinfo)) return;

                //读取文件内容
                using (StreamReader sr = new StreamReader(sinfo, System.Text.Encoding.UTF8))
                {
                    sb.Append(sr.ReadToEnd());
                    sr.Close();
                }

                //写文件 JAssembly 注解
                string sbMemo = sb.ToString();
                using (FileStream fs = new FileStream(sinfo, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite))
                {
                    string usingNamesapce = "using NXDO.RJava.Attributes;";
                    if(sbMemo.IndexOf(usingNamesapce) < 0)
                        sb.Insert(0, usingNamesapce + Environment.NewLine);

                    sb.AppendLine();
                    foreach (string jar in alJarsName)
                    {
                        string jasmAttr = "[assembly: JAssembly(\"" + jar + "\")]";
                        if (sbMemo.IndexOf(jasmAttr) > -1) continue;
                        sb.AppendLine(jasmAttr);
                        sbMemo = sb.ToString();
                    }

                    byte[] b1 = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                    fs.Write(b1, 0, b1.Length);
                    fs.Flush();
                }
            }

            #region 查找jar文件
            private List<string> alJars;
            private List<string> alJarsName; //仅保存无PATH的文件名
            private void listDir(string path)
            {
                this.listFile(path);
                DirectoryInfo[] dInfos = new DirectoryInfo(path).GetDirectories();
                foreach (DirectoryInfo di in dInfos)
                {
                    if (di.FullName.ToLower().IndexOf("\\bin\\debug") > -1) continue;
                    if (di.FullName.ToLower().IndexOf("\\bin\\release") > -1) continue;
                    this.listDir(di.FullName);
                    System.Threading.Thread.Sleep(2);
                }
            }

            private void listFile(string path)
            {
                string[] fNames = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).Where(
                                    s => (s.EndsWith(".jar") || s.EndsWith(".zip")) && s.IndexOf("\\bin\\") < 0
                                    ).ToArray();
                foreach (string fname in fNames)
                {
                    string fSimpleName = Path.GetFileName(fname);
                    if (fSimpleName.CompareTo("jxdo.rjava.jar") == 0) continue;
                    if (alJarsName.Contains(fSimpleName)) continue; //多个同名文件,只添加一个.
                    this.alJarsName.Add(fSimpleName);
                    this.alJars.Add(fname);
                    System.Threading.Thread.Sleep(2);
                }
            }
            #endregion
        }
    }
}
