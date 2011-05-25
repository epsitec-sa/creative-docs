//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Document.PDF;

namespace Common.Document.ExportEngine
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[System.STAThread]
		static void Main()
		{
			string[] args = System.Environment.GetCommandLineArgs ();
			string source = System.IO.File.ReadAllText (args[1], System.Text.Encoding.Default);

			ImageSurface image = ImageSurface.Deserialize (source);
			
			Export.ExecuteProcessImageAndCreatePdfStream (image);

			System.IO.File.WriteAllText (args[1], ImageSurface.Serialize (image), System.Text.Encoding.Default);
		}
	}
}
