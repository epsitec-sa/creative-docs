//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Data.Platform;
using Epsitec.Data.Platform.MatchSort;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Tool.Data.Platform
{
	class Program
	{
		static void Main(string[] args)
		{
			var matchSort = new MatchSortEtl ();
			var zipCodeFoldings = matchSort.GetZipCodeFoldings ();

			Program.WriteZipCodeFoldingsCache (zipCodeFoldings);
			
			var alpha2codes = Iso3166.MaintenanceDownloadAlpha2CodesTextFile ();
			var allCodes    = Iso3166.GetAlpha2Codes (alpha2codes);

			Program.WriteAlpha2CodesCacheFile (alpha2codes);
			Program.WriteCountryInformationCache (allCodes);
		}

		private static void WriteAlpha2CodesCacheFile(string[] alpha2codes)
		{
			if ((alpha2codes != null) &&
				(alpha2codes.Length > 0))
			{
				System.IO.File.WriteAllLines (System.IO.Path.Combine (Program.rootPath, "Data.Platform", "DataFiles", "alpha2codes.txt"), alpha2codes);
			}
		}
		
		private static void WriteCountryInformationCache(IEnumerable<string> allCodes)
		{
			var cache  = Iso3166.MaintenanceCreateCountryInfoCache ();
			var languageCodes = new string[] { "fr", "de", "en", "it" };

			foreach (var languageCode in languageCodes)
			{
				foreach (var countryCode in allCodes)
				{
					var key = Iso3166.GetCountryKey (countryCode, languageCode);
					var info = Iso3166.MaintenanceDownloadCountryInformation (countryCode, languageCode);

					cache[key] = info;
				}
			}

			string json = Iso3166.MaintenanceSerializeCountryInfoCache (cache);

			var zip = new Epsitec.Common.IO.ZipFile ();
			var data = System.Text.Encoding.Default.GetBytes (json);
			zip.AddEntry ("json.txt", data);
			zip.SaveFile (System.IO.Path.Combine (Program.rootPath, "Data.Platform", "DataFiles", "CountryInfo.zip"));
		}

		private static void WriteZipCodeFoldingsCache(IEnumerable<SwissPostZipCodeFolding> foldings)
		{
			var text = string.Join ("\n", foldings.Select (x => x.ToString ()));
			var zip  = new Epsitec.Common.IO.ZipFile ();
			var data = System.Text.Encoding.Default.GetBytes (text);
			zip.AddEntry ("codes.txt", data);
			zip.SaveFile (System.IO.Path.Combine (Program.rootPath, "Data.Platform", "DataFiles", "ZipCodeFolding.zip"));
		}
		
		private static string rootPath = @"S:\Epsitec.Cresus";
	}
}
