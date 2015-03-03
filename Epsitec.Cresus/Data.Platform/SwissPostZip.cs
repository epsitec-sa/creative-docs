//	Copyright © 2011-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Samuel LOUP

using System.Collections.Generic;
using System.Linq;
using Epsitec.Data.Platform.MatchSort;


namespace Epsitec.Data.Platform
{
	/// <summary>
	/// The <c>SwissPostZip</c> class is used internally to retrieve the ZIP codes from the
	/// Internet.
	/// </summary>
	internal static class SwissPostZip
	{
		public static string GetSwissPostZipCsv()
		{
			var matchClient     = new MatchWebClient ();
			var swissPostZipCsv = SwissPostZip.GetMatchZipCsvPath ();

			if (SwissPostZip.MustUpdateOrCreateZipCsv (matchClient))
			{
				var file = matchClient.GetMatchSortFile ();
				var meta = MatchSortLoader.GetFileMetaData (file);
				SwissPostZip.WriteMatchZipMetaData (meta.Validity);
				MatchSortExtractor.WriteRecordsToFile<SwissPostZipInformation> (file, SwissPostZipInformation.GetMatchRecordId (), swissPostZipCsv);
				return swissPostZipCsv;
			} 
			else
			{
				return swissPostZipCsv;
			}
		}

		public static bool MustUpdateOrCreateZipCsv (MatchWebClient client)
		{
			var swissPostZipMeta = SwissPostZip.GetMatchZipMetaPath ();
			if(System.IO.File.Exists (swissPostZipMeta))
			{
				var fileMetaData = SwissPostZip.ReadMatchZipMetaData ();
				/* If validity date is outdated*/
				int result = System.DateTime.Compare (fileMetaData, System.DateTime.Now);
				if (result < 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			} 
			else
			{
				return true;
			}
		}

		private static string GetMatchZipCsvPath()
		{
			string path1 = System.Environment.GetFolderPath (System.Environment.SpecialFolder.ApplicationData);
			return System.IO.Path.Combine (path1, "Epsitec", "swisspostzip.csv");
		}

		private static string GetMatchZipMetaPath()
		{
			string path1 = System.Environment.GetFolderPath (System.Environment.SpecialFolder.ApplicationData);
			return System.IO.Path.Combine (path1, "Epsitec", "swisspostzip.meta");
		}

		private static System.DateTime ReadMatchZipMetaData()
		{
			var swissPostZipMeta = SwissPostZip.GetMatchZipMetaPath ();
			string date = System.IO.File.ReadAllText (swissPostZipMeta);
			return System.Convert.ToDateTime (date);
		}

		private static void WriteMatchZipMetaData(System.DateTime releaseDate)
		{
			var date = releaseDate.ToShortDateString ();
			var swissPostZipMeta = SwissPostZip.GetMatchZipMetaPath ();
			System.IO.File.WriteAllText (swissPostZipMeta, date);
		}
	}
}
