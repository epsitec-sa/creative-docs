using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsvHelper.Configuration;
using Epsitec.Common.Types;

namespace Epsitec.Data.Platform.MatchSort.Mapping
{
	internal class SwissPostZipInformationMap : CsvClassMap<SwissPostZipInformation>
	{
		public SwissPostZipInformationMap()
		{
			Map (m => m.OnrpCode).Index (1);
			Map (m => m.CommunityCode).Index (2);
			Map (m => m.ZipType).ConvertUsing (row =>
				InvariantConverter.ParseInt<SwissPostZipType> (row.GetField (3)));
			Map (m => m.ZipCode).Index (4);
			Map (m => m.ZipCodeAddOn).Index (5);
			Map (m => m.RootZipCode).Index (6);
			Map (m => m.ShortName).Index (7);
			Map (m => m.LongName).Index (8);
			Map (m => m.Canton).Index (9);
			Map (m => m.LanguageCode1).ConvertUsing (row =>
				InvariantConverter.ParseInt<SwissPostLanguageCode> (row.GetField (10)));
			Map (m => m.LanguageCode2).ConvertUsing (row =>
				InvariantConverter.ParseInt<SwissPostLanguageCode> (row.GetField (11)));
			Map (m => m.DeliveryOnrp).Index (12);
			Map (m => m.ValidSince).ConvertUsing (row => new DateTime (Convert.ToInt32 (row.GetField (13).Substring (0, 4)),
															/**/       Convert.ToInt32 (row.GetField (13).Substring (4, 2)),
															/**/       Convert.ToInt32 (row.GetField (13).Substring (6, 2))
															/**/      )
											/**/);
			Map (m => m.DeliveryZipCode).Index (14);
			Map (m => m.OnlyOfficial).Index (15);
		}
	}
}
