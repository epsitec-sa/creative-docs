using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsvHelper.Configuration;
using Epsitec.Data.Platform.MatchSort.Rows;

namespace Epsitec.Data.Platform.MatchSort.Mapping
{
	internal class MatchSortMetaDataMap : CsvClassMap<MatchSortMetaData>
	{
		public MatchSortMetaDataMap()
		{
			Map (m => m.Validity).ConvertUsing (row => new DateTime (Convert.ToInt32 (row.GetField (1).Substring (0, 4)),
																	 Convert.ToInt32 (row.GetField (1).Substring (4, 2)),
																	 Convert.ToInt32 (row.GetField (1).Substring (6, 2))
																	 )
												);
			Map (m => m.RandomKey).Index (2);
		}
	}
}
