using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsvHelper.Configuration;
using Epsitec.Common.Types;

namespace Epsitec.Data.Platform.MatchSort.Mapping
{
	internal class SwissPostHouseInformationMap : CsvClassMap<SwissPostHouseInformation>
	{
		public SwissPostHouseInformationMap()
		{
			Map (m => m.HouseCode).Index (1);
			Map (m => m.StreetCode).Index (2);
			Map (m => m.HouseNumber).Index (3).Default (0);
			Map (m => m.HouseLetter).Index (4);
			Map (m => m.OfficialHouse).Index (5);
		}
	}
}
