using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Linq;

using EnvDTE;
using EnvDTE80;

using System.Reflection;

namespace NXDO.RJacoste.Addin
{
    class FindJar
    {
        private DTE2 _applicationObject;
		private Project prj;
		private OutputWindowPane dg;
		private ArrayList alJars;
        public FindJar(DTE2 _applicationObject, Project prj, OutputWindowPane outWin)
		{
			this._applicationObject = _applicationObject;
			this.prj = prj;
			this.dg = outWin;
			this.alJars = new ArrayList();
		}        

        public void Execute()
        {
            //System.Threading.Thread th = new System.Threading.Thread(new ThreadStart(doExecute));
            //th.Start();
            ////doStart();
            doExecute();
        }

        private void doExecute()
        {
            string path = Path.GetDirectoryName(prj.FullName);
            this.listDir(path);

            dg.OutputString("\nNXDO.RJacoste.Addin for VS2012(jom - JObject Mapping) [1.0.0.0]\n\tAuthor:\t\t朱琦 (javasuki) \n\tBlog:\t\thttp://blog.csdn.net/javasuki \n\tEMail:\t\tjavasuki@hotmail.com\n\tCompany:\t版权所有(C) [itking software] 2008. 保留所有权利.\n\n");


            //取得当前工程的类型

            //if (prj.Kind != PrjKind.prjKindCSharpProject)
            
            //if(prj.Kind != VSLangProj.PrjKind.prjKindCSharpProject)
            {
                //dg.OutputString("\t 你的工程项目目前没有得到插件的支持,请使用C#工程类型..." + System.Environment.NewLine);
                //return;

                ////此属性仅在 Visual C# 项目中受支持。取消 (ORM生成的CLASS中,只给标识赋值而)没有使用的警告
                //Configuration cfg = Connect.CurrentProject.ConfigurationManager.ActiveConfiguration;
                //string s = cfg.Properties.Item("NoWarn").Value.ToString();
                //if (string.IsNullOrEmpty(s)) cfg.Properties.Item("NoWarn").Value = "0414";
                //else if (s.IndexOf("0414") < 0) cfg.Properties.Item("NoWarn").Value += ";0414";
            }


            dg.OutputString("[ 开始搜索可生成 （jom）JObject Mapping 的 jar 文件 ]..." + System.Environment.NewLine);

            if (alJars.Count == 0)
                dg.OutputString("\t没有找到任何jar文件..." + System.Environment.NewLine);

            foreach (string fname in alJars)
            {
                //FileInfo finfo = new FileInfo(fname);
                dg.OutputString("\t找到jar: " + fname.Replace(path + Path.DirectorySeparatorChar, "") + System.Environment.NewLine);
            }
            dg.OutputString("[ 搜索 jar 文件结束, " + DateTime.Now.ToLongTimeString() + " ]" + System.Environment.NewLine);


            if (alJars.Count == 0) return;


            #region //在项目中创建一个新的文件夹,保存生成的 jobject 文件
            //在项目中创建一个新的文件夹
            string newDirFullPath = string.Empty;
            string newDirShortName = "nxdo.java";
            newDirFullPath = Path.Combine(path, newDirShortName);
            if (!Directory.Exists(newDirFullPath))
            {
                Directory.CreateDirectory(newDirFullPath);

                //注意参数,必须是文件夹的名称,而不是FullPath
                Connect.CurrentProject.ProjectItems.AddFolder(newDirShortName, Constants.vsProjectItemKindPhysicalFolder);
            }
            #endregion

            #region //给工程添加类库的引用,满足编译时需要的DLL
            //给工程添加类库的引用
            string refDir = Path.Combine(path, "lib.refs");
            if (!Directory.Exists(refDir)) Directory.CreateDirectory(refDir);
            string refJVMFile = Path.Combine(refDir, "NXDO.RJava.Core.dll");
            string refRunFile = Path.Combine(refDir, "NXDO.RJava.V2015.dll");
            if (!File.Exists(refJVMFile))
                new AddLibReferences(refJVMFile).Save("NXDO.RJava.Core.dll");

            if (!File.Exists(refRunFile))
            {
                new AddLibReferences(refRunFile).Save("NXDO.RJava.V2015.dll");
                new AddLibReferences(refRunFile.Replace(".dll", ".xml")).Save("NXDO.RJava.V2015.xml");
            }

            //if (!File.Exists(refAntlr3File))
            //    new AddLibReferences(refAntlr3File).Save("Antlr3.Runtime.dll");

            //检查是否已经引用了
            bool bRefJvmExisit = false;
            bool bRefRunFile = false;

            //VSProject vsprj = (VSProject)Connect.CurrentProject.Object;
            dynamic vsprj = Connect.CurrentProject.Object;
            for (int i = 1; i <= vsprj.References.Count; i++)
            {
                string refName = vsprj.References.Item(i).Name + ".dll";
                if (refJVMFile.ToLower().EndsWith(refName.ToLower()))
                    bRefJvmExisit = true;

                if (refRunFile.ToLower().EndsWith(refName.ToLower()))
                    bRefRunFile = true;

                if (bRefJvmExisit && bRefRunFile) break;
            }


            if (!bRefJvmExisit)
            {
                dg.OutputString(Environment.NewLine + "自动添加程序集引用: NXDO.RJava.Core.dll" + System.Environment.NewLine);
                vsprj.References.Add(refJVMFile);	//没有引用,则添加引用

            }

            if (!bRefRunFile)
            {
                dg.OutputString(Environment.NewLine + "自动添加程序集引用: NXDO.RJava.V2015.dll" + System.Environment.NewLine);
                vsprj.References.Add(refRunFile);	//没有引用,则添加引用
            }
            #endregion

            //当前插件所在目录
            string runAddinPath = typeof(FindJar).Assembly.CodeBase.Replace("file:///", "");
            runAddinPath = new FileInfo(runAddinPath).DirectoryName;

            #region //检查是否存在 NXDO.RJacoste.Writer.exe,与所需要的DLL/jar
            if (1 == 1)
            {
                string exeFileName = Path.Combine(runAddinPath, "NXDO.RJacoste.Writer.exe");
                if(!File.Exists(exeFileName))
                    new AddLibReferences(exeFileName).Save("NXDO.RJacoste.Writer.exe");

                //NXDO.RJacoste.Writer.exe 运行所需要的DLL/jar
                string dllJvmFile2 = Path.Combine(runAddinPath, "NXDO.RJava.Core.dll");
                string dllRunFile2 = Path.Combine(runAddinPath, "NXDO.RJava.V2015.dll");
                string jarRunFile2 = Path.Combine(runAddinPath, "jxdo.rjava.jar");

                if (!File.Exists(dllJvmFile2))
                    new AddLibReferences(dllJvmFile2).Save("NXDO.RJava.Core.dll");
                if (!File.Exists(dllRunFile2))
                    new AddLibReferences(dllRunFile2).Save("NXDO.RJava.V2015.dll");
                if (!File.Exists(jarRunFile2))
                    new AddLibReferences(jarRunFile2).Save("jxdo.rjava.jar");
            }
            #endregion

            #region //添加jar文件到工程中，确保可复制到（工作编译后的运行目录下）
            if (1 == 1)
            {
                string jarRunFile3 = Path.Combine(path, "jxdo.rjava.jar");
                if (!File.Exists(jarRunFile3))
                {
                    new AddLibReferences(jarRunFile3).Save("jxdo.rjava.jar");

                    EnvDTE.ProjectItem jarFileItem = Connect.CurrentProject.ProjectItems.AddFromFile(jarRunFile3);
                    jarFileItem.Properties.Item("CopyToOutputDirectory").Value = 2;
                }
            }
            #endregion

            dg.OutputString("\n");
            dg.OutputString("===开始生成 Time:" + DateTime.Now.ToLongTimeString() + "===\n\n");

            //调用exe,生成代码
            try
            {
                String runExePath = Path.Combine(runAddinPath, "NXDO.RJacoste.Writer.exe");
                var prci = new System.Diagnostics.ProcessStartInfo(runExePath, "RJava.ForJVM.ByJObject-javasuki(ZhuQi) " + prj.FullName);
                prci.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                prci.WorkingDirectory = runAddinPath;
                var prc = System.Diagnostics.Process.Start(prci);
                prc.WaitForExit();
            }
            catch { }

            foreach (var csName in Directory.GetFiles(newDirFullPath, "*.cs"))
            {
                Connect.CurrentProject.ProjectItems.AddFromFile(csName);
            }

            dg.OutputString("\n===结束生成 Time:" + DateTime.Now.ToLongTimeString() + "===\n");

            Connect.CurrentProject.Save();
        }

        #region 查找jar文件
        private void listDir(string path)
        {
            this.listFile(path);
            DirectoryInfo[] dInfos = new DirectoryInfo(path).GetDirectories();
            foreach (DirectoryInfo di in dInfos)
            {
                this.listDir(di.FullName);
                System.Threading.Thread.Sleep(2);
            }
        }

        private void listFile(string path)
        {
            string[] fNames = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                                    .Where(s => s.EndsWith(".jar") || s.EndsWith(".zip")).ToArray();
            foreach (string fname in fNames)
            {
                if (new FileInfo(fname).Name.CompareTo("jxdo.rjava.jar") == 0) continue;
                if (!fname.ToLower().EndsWith(".jar"))
                {
                    System.Threading.Thread.Sleep(2);
                    continue;
                }

                this.alJars.Add(fname);
                System.Threading.Thread.Sleep(2);
            }
        }
        #endregion
    }
}
