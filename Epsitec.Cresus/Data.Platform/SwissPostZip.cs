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
		public static IEnumerable<SwissPostZipInformation> GetZips()
		{
			var matchClient = new MatchWebClient ();
			string file = matchClient.GetMatchSortFileFromWebsite ();
			return MatchSortLoader.LoadRecords<SwissPostZipInformation> (file, SwissPostZipInformation.GetMatchRecordId ());
		}
	}
}
