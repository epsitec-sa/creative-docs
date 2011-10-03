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
					if (this.TryProcessFile (file))
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

		private bool TryProcessFile(System.IO.FileInfo file)
		{
			try
			{
				var stream = file.Open (System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None);

				byte[] data = new byte[stream.Length];
				stream.Read (data, 0, data.Length);
				stream.Close ();

				this.ProcessFileData (file.Name, data);

				file.Delete ();

				return true;
			}
			catch
			{
				return false;
			}
		}

		private void ProcessFileData(string fileName, byte[] data)
		{
			System.Console.WriteLine ("{0} : {1} bytes", fileName, data.Length);
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (System.IO.Directory.Exists (this.path))
			{
				System.IO.Directory.Delete (this.path);
			}
		}

		#endregion

		private readonly string path;
	}
}
