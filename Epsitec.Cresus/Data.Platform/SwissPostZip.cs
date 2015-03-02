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
			var matchClient = new MatchWebClient ();
			var file = matchClient.GetMatchSortFileFromWebsite ();
			var swissPostZipCsv = SwissPostZip.GetMatchZipCsvPath ();
			MatchSortExtractor.WriteRecordsToFile<SwissPostZipInformation> (file, SwissPostZipInformation.GetMatchRecordId (), swissPostZipCsv);
			return swissPostZipCsv;
		}

		private static string GetMatchZipCsvPath()
		{
			string path1 = System.Environment.GetFolderPath (System.Environment.SpecialFolder.ApplicationData);
			return System.IO.Path.Combine (path1, "Epsitec", "swisspostzip.csv");
		}
	}
}
