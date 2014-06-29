using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace NXDO.RJacoste.Addin
{
	class AxImage : System.Windows.Forms.AxHost
	{
		private AxImage()
			: base("")
        {
        }

		/// <summary>
		/// µÃµ½Í¼Æ¬
		/// </summary>
		/// <param name="Image"></param>
		/// <returns></returns>
		public static stdole.StdPicture StdPicture(String picResName)
		{
			System.Drawing.Image Image = null;
			using (Stream stm = (Assembly.GetAssembly(typeof(AxImage)).GetManifestResourceStream(picResName)))
			{
				Image = System.Drawing.Image.FromStream(stm);
			}	
			//System.Drawing.Bitmap myBitmap = new System.Drawing.Bitmap(Image,16,16);
			//myBitmap.MakeTransparent(System.Drawing.Color.White );
			return (stdole.StdPicture)(System.Windows.Forms.AxHost.GetIPictureDispFromPicture(Image));
		}
	}
}
