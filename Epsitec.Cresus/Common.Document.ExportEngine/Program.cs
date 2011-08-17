//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Document.PDF;

using System.Collections.Generic;
using System.Linq;

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
			try
			{
				System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
				string[] args = System.Environment.GetCommandLineArgs ();

				System.Diagnostics.Debug.WriteLine ("ExportEngine: version " + typeof (Program).AssemblyQualifiedName.Split ('=')[1].Split (',')[0]);
				System.Diagnostics.Debug.WriteLine ("ExportEngine: operating on file " + args[1]);

				string source = System.IO.File.ReadAllText (args[1], System.Text.Encoding.Default);

				System.Diagnostics.Debug.WriteLine ("----------------------------------------------------------------------");
				System.Diagnostics.Debug.WriteLine (source);

				System.IO.File.Delete (args[1]);

				ImageSurface image = ImageSurface.Deserialize (source);

				Export.ExecuteProcessImageAndCreatePdfStream (image);

				source = ImageSurface.Serialize (image);

				System.Diagnostics.Debug.WriteLine ("----------------------------------------------------------------------");
				System.Diagnostics.Debug.WriteLine (source);
				System.Diagnostics.Debug.WriteLine ("----------------------------------------------------------------------");

				System.IO.File.WriteAllText (args[1], source, System.Text.Encoding.Default);

				System.Diagnostics.Debug.WriteLine (string.Format ("ExportEngine: executed in {0} ms", watch.ElapsedMilliseconds));
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine ("ExportEngine: threw exception " + ex.Message);
				System.Diagnostics.Debug.WriteLine (ex.StackTrace);
			}
		}
	}
}
