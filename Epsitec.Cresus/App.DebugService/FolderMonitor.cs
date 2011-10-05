//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.DebugService
{
	/// <summary>
	/// The <c>FolderMonitor</c> class is used to monitor files in a folder; whenever a file
	/// is readable (and is no longer in use by anybody else), it gets uploaded to the cloud.
	/// </summary>
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
				if ((file.Exists) &&
					(file.Length > 0))
				{
					if (this.TryProcessFile (file))
					{
						count++;
					}
				}
			}

			return count > 0;
		}

		public void Sleep()
		{
			System.Threading.Thread.Sleep (100);
		}

		
		private bool TryProcessFile(System.IO.FileInfo file)
		{
			try
			{
				var stream = file.Open (System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None);

				byte[] data = new byte[stream.Length];
				stream.Read (data, 0, data.Length);
				stream.Close ();

				this.UploadFile (file.FullName, data);

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

		/// <summary>
		/// Uploads the file to the cloud, using an HTTP POST request.
		/// </summary>
		/// <param name="path">The path used to extract the file name and the session name.</param>
		/// <param name="data">The data.</param>
		private void UploadFile(string path, byte[] data)
		{
			string fileName = System.IO.Path.GetFileName (path);

			if (this.sessionId == null)
			{
				string session  = System.IO.Path.GetFileName (System.IO.Path.GetDirectoryName (path));

				if (session.StartsWith ("dbg-"))
				{
					session = session.Substring (4);
				}
				
				this.sessionId = session;
			}

			System.Console.WriteLine ("{0}/{1} : {2} bytes", this.sessionId, fileName, data.Length);

			var urlParam1  = string.Concat ("session=", System.Web.HttpUtility.UrlEncode (this.sessionId));
			var urlParam2  = string.Concat ("file=", System.Web.HttpUtility.UrlEncode (fileName));
			var encodedUrl = string.Concat (WebUploader.DebugServiceUrl, "log", "?", urlParam1, "&", urlParam2);

			System.Console.WriteLine ("Push file to {0}", encodedUrl);

			WebUploader.Post (encodedUrl, data);
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (System.IO.Directory.Exists (this.path))
			{
				if (this.sessionId != null)
				{
					var totalTicks = System.DateTime.UtcNow.Ticks / 10000;
					var fileName   = string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}\\{1:00000000000000}.END.txt", this.sessionId, totalTicks);
					var data       = System.Text.Encoding.UTF8.GetBytes ("Exited");

					try
					{
						this.UploadFile (fileName, data);
					}
					catch (System.Exception ex)
					{
						System.Diagnostics.Debug.WriteLine ("The debug service folder monitor could not properly upload the 'exit' event.");
						System.Diagnostics.Debug.WriteLine (ex.Message);
						System.Diagnostics.Debug.WriteLine (ex.StackTrace);
					}
				}
				
				System.IO.Directory.Delete (this.path, recursive: true);
			}
		}

		#endregion

		private readonly string path;
		private string sessionId;
	}
}
