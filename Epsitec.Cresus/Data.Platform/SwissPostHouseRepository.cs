//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Types.Collections;

using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using Epsitec.Data.Platform.MatchSort;

namespace Epsitec.Data.Platform
{
	public sealed class SwissPostHouseRepository
	{
		private SwissPostHouseRepository()
		{
			using (var stream = System.IO.File.OpenText (SwissPostHouse.GetSwissPostHouseCsv ()))
			{
				var csv   = new CsvReader (stream, MatchSortLoader.ConfigureSwissPostReader<SwissPostHouseInformation> ());
				var houses = csv.GetRecords<SwissPostHouseInformation> ().ToList ();
			
				this.houseByStreetCode   = new Dictionary<int, List<SwissPostHouseInformation>> ();

				foreach (var house in houses)
				{
					List<SwissPostHouseInformation> list;

					if (this.houseByStreetCode.TryGetValue (house.StreetCode, out list) == false)
					{
						list = new List<SwissPostHouseInformation> ();
						this.houseByStreetCode[house.StreetCode] = list;
					}

					list.Add (house);
				}
			}
		}

		public List<SwissPostHouseInformation> FindByStreetCode(int streetCode)
		{
			List<SwissPostHouseInformation> houses;

			this.houseByStreetCode.TryGetValue (streetCode, out houses);

			return houses;
		}

		public static readonly SwissPostHouseRepository Current = new SwissPostHouseRepository ();


		private readonly Dictionary<int, List<SwissPostHouseInformation>> houseByStreetCode;
	}
}
