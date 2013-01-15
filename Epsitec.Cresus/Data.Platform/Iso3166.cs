//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Epsitec.Data.Platform
{
	/// <summary>
	/// The <c>Iso3166</c> class retrieves country information as defined by ISO.
	/// The ISO-3166 standard defines the two-letter codes (such as <c>CH</c> or
	/// <c>FR</c>).
	/// </summary>
	public static class Iso3166
	{
		static Iso3166()
		{
			Iso3166.geoNamesCountryInformationCache = new Dictionary<string, GeoNamesCountryInformation> ();
			Iso3166.list_en1_semic_3_txt = null;
		}


		public static IEnumerable<string> GetAlpha2Codes(string[] cache = null)
		{
			if (Iso3166.list_en1_semic_3_txt == null)
			{
				Iso3166.list_en1_semic_3_txt = Iso3166.LoadAlpha2Codes ();
			}

			var source = cache ?? Iso3166.list_en1_semic_3_txt;

			var codes = from line in source.Skip (2)
						let pos = line.IndexOf (';')
						where pos > 0
						select line.Substring (pos+1);

			return codes;
		}

		public static IEnumerable<GeoNamesCountryInformation> GetCountries(string language)
		{
			return from code in Iso3166.GetAlpha2Codes ()
				   select Iso3166.GetCountryInformation (code, language);
		}

		public static GeoNamesCountryInformation GetCountryInformation(string code, string language)
		{
			//	We are querying the GeoNames server, which provides up to 30'000 replies
			//	par day with our free account.

			if (Iso3166.geoNamesCountryInformationCache.Count == 0)
			{
				Iso3166.geoNamesCountryInformationCache = Iso3166.LoadCountryInformation ();
			}

			string key = Iso3166.GetCountryKey (code, language);
			GeoNamesCountryInformation info;

			if (Iso3166.geoNamesCountryInformationCache.TryGetValue (key, out info))
			{
				return info;
			}

			info = Iso3166.MaintenanceDownloadCountryInformation (code, language);

			Iso3166.geoNamesCountryInformationCache[key] = info;

			return info;
		}

		public static string GetCountryKey(string code, string language)
		{
			return string.Concat (code, "-", language);
		}

		
		
		public static string[] MaintenanceDownloadAlpha2CodesTextFile()
		{
			return Iso3166.DownloadLines ("http://www.iso.org/iso/list-en1-semic-3.txt").ToArray ();
		}

		public static GeoNamesCountryInformation MaintenanceDownloadCountryInformation(string code, string language)
		{
			var uri    = string.Format ("http://api.geonames.org/countryInfoCSV?lang={0}&country={1}&username=epsitec", language, code);
			var values = Iso3166.DownloadLines (uri).Skip (1).First ().Split ('\t');

			var info = new GeoNamesCountryInformation ()
			{
				IsoAlpha2  = values[0],
				IsoAlpha3  = values[1],
				IsoNumeric = values[2],
				FipsCode   = values[3],
				Name       = values[4],
				Capital    = values[5],
				Continent  = values[8],
				Languages  = values[9],
				Currency   = values[10]
			};

			return info;
		}

		public static Dictionary<string, GeoNamesCountryInformation> MaintenanceCreateCountryInfoCache()
		{
			return new Dictionary<string, GeoNamesCountryInformation> ();
		}

		public static string MaintenanceSerializeCountryInfoCache(Dictionary<string, GeoNamesCountryInformation> cache)
		{
			return ServiceStack.Text.JsonSerializer.SerializeToString (cache);
		}

		
		
		private static string[] LoadAlpha2Codes()
		{
			var assembly = System.Reflection.Assembly.GetExecutingAssembly ();
			var path     = "Epsitec.Data.Platform.DataFiles.alpha2codes.txt";

			using (var stream = assembly.GetManifestResourceStream (path))
			{
				return Epsitec.Common.IO.StringLineExtractor.GetLines (stream).ToArray ();
			}
		}

		private static Dictionary<string, GeoNamesCountryInformation> LoadCountryInformation()
		{
			var assembly = System.Reflection.Assembly.GetExecutingAssembly ();
			var path     = "Epsitec.Data.Platform.DataFiles.CountryInfo.zip";
			var json     = Epsitec.Common.IO.ZipFile.DecompressTextFile (assembly, path, System.Text.Encoding.Default);

			return ServiceStack.Text.JsonSerializer.DeserializeFromString<Dictionary<string, GeoNamesCountryInformation>> (json);
		}

		private static IEnumerable<string> DownloadLines(string uri)
		{
			return Epsitec.Common.IO.Web.DownloadLines (uri, System.Text.Encoding.UTF8);
		}
		
		private static Dictionary<string, GeoNamesCountryInformation> geoNamesCountryInformationCache;
		private static string[] list_en1_semic_3_txt;
	}
}