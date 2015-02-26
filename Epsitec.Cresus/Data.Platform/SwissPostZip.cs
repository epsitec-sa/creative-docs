//	Copyright © 2011-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Net;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Data.Platform.MatchSort;
using System.IO;

namespace Epsitec.Data.Platform
{
	/// <summary>
	/// The <c>SwissPostZip</c> class is used internally to retrieve the ZIP codes from the
	/// Internet.
	/// </summary>
	internal static class SwissPostZip
	{
		public static IEnumerable<SwissPostZipInformation> GetZips()
		{
			string uri = "https://match.post.ch/download?file=10177&tid=36&rol=0";
			string file = SwissPostZip.DownloadFileToTemp (uri);
			return MatchSortLoader.LoadRecords<SwissPostZipInformation> (file, "01");
		}

		private static IEnumerable<string> GetZipPlusFile()
		{
			string uri = "https://match.post.ch/download?file=10001&tid=11&rol=0";
			string file = SwissPostZip.DownloadZippedTextFile (uri);

			return Epsitec.Common.IO.StringLineExtractor.GetLines (file);
		}

		private static string DownloadFileToTemp (string uri)
		{
			var filename = Path.GetTempFileName ();
			using (WebClient client = new WebClient ())
			{
				using (var stream = client.OpenRead (uri))
				{
					try
					{
						var zipFile = new Epsitec.Common.IO.ZipFile ();
						zipFile.LoadFile (stream);
						var zipEntry = zipFile.Entries.First ();

						using (StreamWriter sw = new StreamWriter (filename))
						{
							sw.Write (System.Text.Encoding.Default.GetString (zipEntry.Data));
						}
					}
					catch
					{
						throw new System.Exception ("Error during file download");
					}
				}
			}
			return filename;
		}

		private static string DownloadZippedTextFile(string uri)
		{
			using (WebClient client = new WebClient ())
			{
				using (var stream = client.OpenRead (uri))
				{
					try
					{
						var zipFile = new Epsitec.Common.IO.ZipFile ();
						zipFile.LoadFile (stream);
						var zipEntry = zipFile.Entries.First ();
						return System.Text.Encoding.Default.GetString (zipEntry.Data);
					}
					catch
					{
						System.Diagnostics.Trace.WriteLine ("match.post.ch: server did not return a valid MAT[CH]zip file.");
						
						var assembly = System.Reflection.Assembly.GetExecutingAssembly ();
						var resource = "Epsitec.Data.Platform.DataFiles.MatchStreetZip.zip";
						return Epsitec.Common.IO.ZipFile.DecompressTextFile (assembly, resource);
					}
				}
			}
		}
	}
}
