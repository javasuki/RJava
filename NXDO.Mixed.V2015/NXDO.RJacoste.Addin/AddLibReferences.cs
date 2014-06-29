using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Resources;
using System.IO;

namespace NXDO.RJacoste.Addin
{
    internal class AddLibReferences
    {
        //private string VsPrjPath;
		private string saveFileName;
        bool isGb2312;
        public AddLibReferences(string saveFileName)
            : this(saveFileName,false)//(string VsPrjPath, string saveFileName)
		{
			//this.VsPrjPath = VsPrjPath;
			
		}

        public AddLibReferences(string saveFileName, bool isGb2312)
        {
            this.saveFileName = saveFileName;
            this.isGb2312 = isGb2312;
        }

		private byte[] GetResourceContent(string sFileName)
		{
			Stream stm = (Assembly.GetAssembly(this.GetType()).GetManifestResourceStream(sFileName));
			byte[] bts = new byte[stm.Length];

			stm.Read(bts, 0, Convert.ToInt32(stm.Length));
			stm.Close();
			return bts;
		}

		public void Save(string resName)
		{
            byte[] bytStr = this.GetResourceContent("NXDO.RJacoste.Addin.Res.dll." + resName);
			FileStream fs = new FileStream(saveFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
			foreach (byte b in bytStr)
			{
				fs.WriteByte(b);
			}
			fs.Flush();
			fs.Close();
		}

		public void Save(string resName,string oldStr,string newStr)
		{
            byte[] bytStr = this.GetResourceContent("NXDO.RJacoste.Addin.dll." + resName);
			string sMemo = Encoding.Default.GetString(bytStr);
			sMemo = sMemo.Replace(oldStr, newStr);

            if (this.isGb2312)
                bytStr = Encoding.GetEncoding("gb2312").GetBytes(sMemo);
            else
			    bytStr = Encoding.Default.GetBytes(sMemo);

			FileStream fs = new FileStream(saveFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
			foreach (byte b in bytStr)
			{
				fs.WriteByte(b);
			}
			fs.Flush();
			fs.Close();
		}

        public void Save(string resName, string[] oldStr, string[] newStr)
        {
            byte[] bytStr = this.GetResourceContent("NXDO.RJacoste.Addin.dll." + resName);
            string sMemo = Encoding.Default.GetString(bytStr);

            for (int i = 0; i < oldStr.Length;i++ )            
                sMemo = sMemo.Replace(oldStr[i], newStr[i]);

            if (this.isGb2312)
                bytStr = Encoding.GetEncoding("gb2312").GetBytes(sMemo);
            else
                bytStr = Encoding.Default.GetBytes(sMemo);

            FileStream fs = new FileStream(saveFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            foreach (byte b in bytStr)
            {
                fs.WriteByte(b);
            }
            fs.Flush();
            fs.Close();
        }

        //public void SaveConfig(string resName )
        //{
        //    byte[] bytStr = this.GetResourceContent("NXDO.Addin.VS2012.Res.config." + resName);
        //    string sMemo = Encoding.UTF8.GetString(bytStr);


        //    if (this.isGb2312)
        //        bytStr = Encoding.GetEncoding("gb2312").GetBytes(sMemo);
        //    else
        //        bytStr = Encoding.UTF8.GetBytes(sMemo);

        //    FileStream fs = new FileStream(saveFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
        //    foreach (byte b in bytStr)
        //    {
        //        fs.WriteByte(b);
        //    }
        //    fs.Flush();
        //    fs.Close();
        //}
    }
}
