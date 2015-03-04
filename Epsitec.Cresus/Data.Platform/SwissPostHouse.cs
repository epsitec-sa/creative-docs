//	Copyright © 2011-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System.Collections.Generic;
using System.Linq;
using Epsitec.Data.Platform.MatchSort;


namespace Epsitec.Data.Platform
{
	internal static class SwissPostHouse
	{
		public static string GetSwissPostHouseCsv()
		{
			var matchClient       = SwissPost.MatchWebClient;
			var swissPostHouseCsv = SwissPostHouse.GetMatchHouseCsvPath ();
			var file              = matchClient.GetMatchSortFile ();

			if (matchClient.IsANewRelease || SwissPostHouse.MustGenerateMatchHouseCsv ())
			{
				MatchSortExtractor.WriteRecordsToFile<SwissPostHouseInformation> (file, SwissPostHouseInformation.GetMatchRecordId (), swissPostHouseCsv);
				return swissPostHouseCsv;
			}
			else
			{
				return swissPostHouseCsv;
			}		
		}

		private static string GetMatchHouseCsvPath()
		{
			string path1 = System.Environment.GetFolderPath (System.Environment.SpecialFolder.ApplicationData);
			return System.IO.Path.Combine (path1, "Epsitec", "swissposthouse.csv");
		}

		private static bool MustGenerateMatchHouseCsv()
		{
			return !System.IO.File.Exists (SwissPostHouse.GetMatchHouseCsvPath ());
		}
	}
}
