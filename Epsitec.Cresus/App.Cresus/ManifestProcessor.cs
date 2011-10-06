//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus
{
	public class ManifestProcessor
	{
		public ManifestProcessor(byte[] data)
		{
			this.data = data;
			
			var lines  = System.Text.Encoding.UTF8.GetString (data).Split ('\n');
			var tuples = from line in lines.Select (x => x.Trim (' ', '\r', '\t', (char)0xfeff))
						 where !string.IsNullOrEmpty (line) && !line.StartsWith ("#")
						 let pos = line.IndexOf (':')
						 select new
						 {
							 Key   = pos < 0 ? line : line.Substring (0, pos).Trim (),
							 Value = pos < 0 ? "" : line.Substring (pos+1).Trim ()
						 };

			this.manifest = tuples.ToDictionary (x => x.Key, x => x.Value);
		}

		public bool ProbeForUpdate()
		{
			string packageId;
			string updateServiceUri;
			
			if ((this.manifest.TryGetValue ("PackageID", out packageId)) &&
				(this.manifest.TryGetValue ("UpdateServiceURI", out updateServiceUri)))
			{
				try
				{
					var dir   = ManifestProcessor.GetPackageFolderPath (packageId);
					var reply = Epsitec.DebugService.WebUploader.Post (updateServiceUri, this.data);

					if (reply.StartsWith ("update "))
					{
						string[] args = reply.Split (' ');
						return this.DownloadUpdate (args[1], args[2], dir);
					}
				}
				catch
				{
				}
			}

			return false;
		}

		public System.IO.FileInfo ProbeForReplacementPackage()
		{
			string packageId;
			string packageRevision;

			if ((this.manifest.TryGetValue ("PackageID", out packageId)) &&
				(this.manifest.TryGetValue ("PackageRevision", out packageRevision)))
			{
				var probe = ManifestProcessor.GetPackageFolderPath (packageId);
				int revision = int.Parse (packageRevision, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);

				if (System.IO.Directory.Exists (probe))
				{
					var files = System.IO.Directory.GetFileSystemEntries (probe, "*.package", System.IO.SearchOption.TopDirectoryOnly);
					var entries = from file in files
								  select new
								  {
									  Revision = ManifestProcessor.GetFileRevision (file),
									  Path = file
								  };

					var newest = entries.Where (x => x.Revision > revision).OrderByDescending (x => x.Revision).FirstOrDefault ();

					if (newest != null)
					{
						return new System.IO.FileInfo (newest.Path);
					}
				}
			}
			
			return null;
		}

		private static string GetPackageFolderPath(string packageId)
		{
			var folder = System.Environment.GetFolderPath (System.Environment.SpecialFolder.LocalApplicationData);
			var probe  = System.IO.Path.Combine (folder, "Cresus Packages", packageId);
			return probe;
		}

		private static int GetFileRevision(string path)
		{
			var name = System.IO.Path.GetFileNameWithoutExtension (path);
			int result;

			int.TryParse (name, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out result);

			return result;
		}

		private bool DownloadUpdate(string packageName, string downloadUri, string packagePath)
		{
			try
			{
				if (!System.IO.Directory.Exists (packagePath))
				{
					System.IO.Directory.CreateDirectory (packagePath);
				}

				var client = new System.Net.WebClient ();
				
				client.Proxy = new System.Net.WebProxy ();
			
				var data = client.DownloadData (downloadUri);

				System.IO.File.WriteAllBytes (System.IO.Path.Combine (packagePath, packageName), data);

				return true;
			}
			catch
			{
			}

			return false;
		}


		private readonly byte[]					data;
		private readonly Dictionary<string, string>	manifest;
	}
}
