using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsvHelper.Configuration;
using Epsitec.Common.Types;

namespace Epsitec.Data.Platform.MatchSort.Mapping
{
	internal class SwissPostStreetInformationMap : CsvClassMap<SwissPostStreetInformation>
	{
		public SwissPostStreetInformationMap()
		{
			Map (m => m.StreetCode).Index (1);
			Map (m => m.BasicPostCode).Index (2);
			Map (m => m.StreetNameShort).Index (3);
			Map (m => m.StreetName).Index (4);
			// 5 Transposition de la désignation de rue abrégée
			// 6 Transposition longue
			Map (m => m.StreetNameType).Index (7);
			Map (m => m.LanguageCode).ConvertUsing (row =>
				InvariantConverter.ParseInt<SwissPostLanguageCode> (row.GetField (8)));
			// 9 Officiel
			// 10 Adresse entièrement case ??
		}
	}
}
