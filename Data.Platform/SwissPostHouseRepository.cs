//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

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
					if (this.houseByStreetCode.TryGetValue (house.StreetCode, out var list) == false)
					{
						list = new List<SwissPostHouseInformation> ();
						this.houseByStreetCode[house.StreetCode] = list;
					}

					list.Add (house);
				}
			}
		}

		public IReadOnlyList<SwissPostHouseInformation> FindByStreetCode(int streetCode)
		{
            if (this.houseByStreetCode.TryGetValue (streetCode, out var houses))
            {
                return houses;
            }
            else
            {
                return System.Array.Empty<SwissPostHouseInformation> ();
            }
		}

		public static readonly SwissPostHouseRepository Current = new SwissPostHouseRepository ();


		private readonly Dictionary<int, List<SwissPostHouseInformation>> houseByStreetCode;
	}
}
