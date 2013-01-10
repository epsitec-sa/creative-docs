//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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


		public static IEnumerable<string> GetAlpha2Codes()
		{
			if (Iso3166.list_en1_semic_3_txt == null)
			{
				Iso3166.list_en1_semic_3_txt = Iso3166.DownloadLines ("http://www.iso.org/iso/list-en1-semic-3.txt").ToArray ();
			}

			var codes = from line in Iso3166.list_en1_semic_3_txt.Skip (2)
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

			string key = string.Concat (code, "-", language);
			GeoNamesCountryInformation info;

			if (Iso3166.geoNamesCountryInformationCache.TryGetValue (key, out info))
			{
				return info;
			}

			string uri = string.Format ("http://api.geonames.org/countryInfoCSV?lang={0}&country={1}&username=epsitec", language, code);
			string[] values = Iso3166.DownloadLines (uri).Skip (1).First ().Split ('\t');

			info = new GeoNamesCountryInformation ()
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

			Iso3166.geoNamesCountryInformationCache[key] = info;

			return info;
		}

		private static IEnumerable<string> DownloadLines(string uri)
		{
			string value;

			using (WebClient client = new WebClient ())
			{
				byte[] data = client.DownloadData (uri);
				value = System.Text.Encoding.UTF8.GetString (data);
			}

			return value.Split ('\n').Select (x => x.TrimEnd (' ', '\r'));
		}

		private static Dictionary<string, GeoNamesCountryInformation> geoNamesCountryInformationCache;
		private static string[] list_en1_semic_3_txt;
	}
}