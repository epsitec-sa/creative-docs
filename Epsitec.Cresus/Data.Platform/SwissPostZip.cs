//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Net;
using System.Collections.Generic;
using System.Linq;

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
			foreach (var line in SwissPostZip.GetZipPlusFile ())
			{
				yield return new SwissPostZipInformation (line);
			}
		}

		private static IEnumerable<string> GetZipPlusFile()
		{
			string uri = "https://match.post.ch/download?file=10001&tid=11&rol=0";
			string file = SwissPostZip.DownloadZippedTextFile (uri);

			return Epsitec.Common.IO.StringLineExtractor.GetLines (file);
		}

		private static string DownloadZippedTextFile(string uri)
		{
			using (WebClient client = new WebClient ())
			{
				using (var stream = client.OpenRead (uri))
				{
					var zipFile = new Epsitec.Common.IO.ZipFile ();
					zipFile.LoadFile (stream);
					var zipEntry = zipFile.Entries.First ();
					return System.Text.Encoding.Default.GetString (zipEntry.Data);
				}
			}
		}
	}
}
