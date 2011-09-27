//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Data.Platform
{
	public static class SwissPostZip
	{
		public static IEnumerable<SwissPostZipInformation> GetZips()
		{
			foreach (var line in SwissPostZip.GetZipPlusFile ())
			{
				var args = line.Split ('\t');

				yield return new SwissPostZipInformation ()
				{
					OnrpCode       = args[0],
					ZipType        = args[1],
					ZipCode        = args[2],
					ZipComplement  = args[3],
					ShortName      = args[4],
					LongName       = args[5],
					Canton         = args[6],
					LanguageCode1  = args[7],
					LanguageCode2  = args[8],
					MatchSort      = args[9],
					DistributionBy = args[10],
					ComunityCode   = args[11],
					ValidSince     = args[12],
				};
			}
		}

		private static IEnumerable<string> GetZipPlusFile()
		{
			string uri = "https://match.post.ch/download?file=10001&tid=11&rol=0";
			string file = SwissPostZip.DownloadZippedTextFile (uri);

			int pos = 0;

			while (pos < file.Length)
			{
				int end = file.IndexOf ('\n', pos);

				if (end < 0)
				{
					end = file.Length;
				}

				if (pos < end)
				{
					string line = file.Substring (pos, end-pos-1);
					
					yield return line;
				}

				pos = end+1;
			}
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
