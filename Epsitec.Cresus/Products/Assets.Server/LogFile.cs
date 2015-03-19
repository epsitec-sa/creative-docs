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
			try
			{
				System.IO.File.Delete (LogFile.Filename);

				using (var stream = System.IO.File.CreateText (LogFile.Filename))
				{
					stream.Close ();
				}
			}
			catch (System.Exception ex)
			{
			}
		}

		public static void AppendToLogFile(string message)
		{
			try
			{
				using (var stream = System.IO.File.AppendText (LogFile.Filename))
				{
					stream.WriteLine (message);
					stream.Close ();
				}
			}
			catch (System.Exception ex)
			{
			}
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
