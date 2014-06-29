using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using Prs = System.Diagnostics.Process;

namespace NXDO.RJacoste.Addin
{
	/// <summary>用于实现外接程序的对象。</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2
	{
        private CommandBarControl cmdUnload;
        private CommandBarControl cmdReload;
        private CommandBarEvents menuJacosteHandler;

		/// <summary>实现外接程序对象的构造函数。请将您的初始化代码置于此方法内。</summary>
		public Connect()
		{
		}

		/// <summary>实现 IDTExtensibility2 接口的 OnConnection 方法。接收正在加载外接程序的通知。</summary>
		/// <param term='application'>宿主应用程序的根对象。</param>
		/// <param term='connectMode'>描述外接程序的加载方式。</param>
		/// <param term='addInInst'>表示此外接程序的对象。</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
            _applicationObject = (DTE2)application;
            _addInInstance = (AddIn)addInInst;


            CommandBars cmdBars = (CommandBars)(_applicationObject.DTE.CommandBars);
            CommandBar vsBarProject = cmdBars["Project"];

            int idxMenuAddRef = 1;
            int iMenuCount = vsBarProject.Controls.Count;
            for (int i = 1; i <= vsBarProject.Controls.Count; i++)
            {
                if (vsBarProject.Controls[i].Caption.StartsWith("卸载项目")) cmdUnload = vsBarProject.Controls[i];
                if (vsBarProject.Controls[i].Caption.StartsWith("重新加载项目")) cmdReload = vsBarProject.Controls[i];
                if (vsBarProject.Controls[i].Caption.StartsWith("添加 Web 引用")) idxMenuAddRef = i;// cmdReload = vsBarProject.Controls[i];				
            }

            #region 建立菜单
            // O/R Mapping
            CommandBarPopup menuCode = vsBarProject.Controls.Add(MsoControlType.msoControlPopup, Missing.Value, Missing.Value, iMenuCount, true) as CommandBarPopup;
            menuCode.Caption = "NXDO.RJacoste 实用工具集";
            menuCode.TooltipText = "NXDO.RJacoste 工具集";
            //(menuCode as CommandBarButton).Style = MsoButtonStyle.msoButtonIconAndCaption;
            //(menuCode as CommandBarButton).Picture = AxImage.StdPicture("NXDO.Component.Addin.Res.image.VSAddin.bmp");

            CommandBarControl menuItemClass = menuCode.Controls.Add(MsoControlType.msoControlButton, 1, "", 1, true);
            menuItemClass.Tag = "Java 的 C# Class";
            menuItemClass.Caption = "生成 jar 的 c# 包装类";
            menuItemClass.TooltipText = "生成 jar 的 c# 包装类";
            menuJacosteHandler = (CommandBarEvents)_applicationObject.DTE.Events.get_CommandBarEvents(menuItemClass);
            menuJacosteHandler.Click += new _dispCommandBarControlEvents_ClickEventHandler(EventAction);
            CommandBarButton cmdItemClass = (CommandBarButton)menuItemClass;
            cmdItemClass.Style = MsoButtonStyle.msoButtonIconAndCaption;
            cmdItemClass.Picture = AxImage.StdPicture("NXDO.RJacoste.Addin.Res.FormIco.png");

            //CommandBar vsBarProject2 = cmdBars["Web Project Folder"];
            #endregion
		}

        internal static string ProjectName;
        internal static Project CurrentProject;
        private void EventAction(object commandBarControl, ref bool handled, ref bool cancelDefault)
        {
            Project prj = (Project)((Array)_applicationObject.ActiveSolutionProjects).GetValue(0);
            OutputWindowPane outMsg = GetOutputWindow(_applicationObject);


            Connect.CurrentProject = prj;
            Connect.ProjectName = prj.Name;
            CommandBarControl menu = (CommandBarControl)commandBarControl;

            if (menu.Caption.IndexOf("生成 jar 的 c# 包装类") > -1)
            {
                new FindJar(_applicationObject, prj, outMsg).Execute();
            }
        }

        #region 得到输出Win
        private OutputWindowPane GetOutputWindow(DTE2 dte)
        {
            //获得输出窗口 
            OutputWindow ow = dte.ToolWindows.OutputWindow;
            OutputWindowPane owP = null;

            int iCount = ow.OutputWindowPanes.Count;
            for (int i = 1; i <= iCount; i++)
            {
                //MessageBox.Show(ow.OutputWindowPanes.Item(i).Name);
                if (ow.OutputWindowPanes.Item(i).Name.CompareTo("NXDO.Addin") == 0)
                {
                    owP = ow.OutputWindowPanes.Item(i);
                    owP.Clear();
                    owP.Activate();
                    return owP;
                }
            }

            //创建属于自己的输出类型 (哪个下拉框中的文字)
            owP = ow.OutputWindowPanes.Add("NXDO.Addin");
            owP.Activate();
            return owP;
        }
        #endregion

		/// <summary>实现 IDTExtensibility2 接口的 OnDisconnection 方法。接收正在卸载外接程序的通知。</summary>
		/// <param term='disconnectMode'>描述外接程序的卸载方式。</param>
		/// <param term='custom'>特定于宿主应用程序的参数数组。</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
		}

		/// <summary>实现 IDTExtensibility2 接口的 OnAddInsUpdate 方法。当外接程序集合已发生更改时接收通知。</summary>
		/// <param term='custom'>特定于宿主应用程序的参数数组。</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>实现 IDTExtensibility2 接口的 OnStartupComplete 方法。接收宿主应用程序已完成加载的通知。</summary>
		/// <param term='custom'>特定于宿主应用程序的参数数组。</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref Array custom)
		{
		}

		/// <summary>实现 IDTExtensibility2 接口的 OnBeginShutdown 方法。接收正在卸载宿主应用程序的通知。</summary>
		/// <param term='custom'>特定于宿主应用程序的参数数组。</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref Array custom)
		{
		}
		
		private DTE2 _applicationObject;
		private AddIn _addInInstance;
	}
}