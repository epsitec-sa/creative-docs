//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server
{
	public static class LogFile
	{
		public static void CreateEmptyLogFile()
		{
			System.IO.File.CreateText (LogFile.Filename);
		}

		public static void AppendToLogFile(string message)
		{
			var stream = System.IO.File.AppendText (LogFile.Filename);
			stream.WriteLine (message);
			stream.Close ();
		}


		private static string Filename
		{
			get
			{
				var path = System.Environment.GetFolderPath (System.Environment.SpecialFolder.MyDocuments);
				return System.IO.Path.Combine (path, "assets.log.txt");
			}
		}
	}
}
