//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.DebugService
{
	public class FolderMonitor : System.IDisposable
	{
		public FolderMonitor(string path)
		{
			this.path = path;
		}


		public bool Process()
		{
			int count = 0;

			foreach (var file in System.IO.Directory.EnumerateFileSystemEntries (this.path).Select (x => new System.IO.FileInfo (x)))
			{
				if (file.Exists)
				{
					if (FolderMonitor.TryProcessFile (file))
					{
						count++;
					}
				}
			}

			if (count == 0)
			{
				System.Threading.Thread.Sleep (100);
			}

			return count > 0;
		}

		
		private static bool TryProcessFile(System.IO.FileInfo file)
		{
			try
			{
				var stream = file.Open (System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None);

				byte[] data = new byte[stream.Length];
				stream.Read (data, 0, data.Length);
				stream.Close ();

				FolderMonitor.UploadFile (file.FullName, data);

				file.Delete ();

				return true;
			}
			catch (System.Exception ex)
			{
				System.Console.ForegroundColor = System.ConsoleColor.Red;
				System.Console.WriteLine (ex.Message);
				System.Console.ResetColor ();

				return false;
			}
		}
		
		private static void UploadFile(string path, byte[] data)
		{
			string fileName = System.IO.Path.GetFileName (path);
			string session  = System.IO.Path.GetFileName (System.IO.Path.GetDirectoryName (path));

			if (session.StartsWith ("dbg-"))
			{
				session = session.Substring (4);
			}

			System.Console.WriteLine ("{0}/{1} : {2} bytes", session, fileName, data.Length);

			var urlParam1  = string.Concat ("session=", System.Web.HttpUtility.UrlEncode (session));
			var urlParam2  = string.Concat ("file=", System.Web.HttpUtility.UrlEncode (fileName));
			var encodedUrl = string.Concat (WebUploader.DebugServiceUrl, "log", "?", urlParam1, "&", urlParam2);

			System.Console.WriteLine ("Push file to {0}", encodedUrl);

			WebUploader.Upload (encodedUrl, data);
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (System.IO.Directory.Exists (this.path))
			{
				System.IO.Directory.Delete (this.path, recursive: true);
			}
		}

		#endregion

		private readonly string path;
	}
}
