//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Collections;
using Epsitec.Common.Types.Converters;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Data.Platform
{
	public sealed class SwissPostStreetRepository
	{
		private SwissPostStreetRepository()
		{
			this.streets = new List<SwissPostStreetInformation> (SwissPostStreet.GetStreets ());
			this.streetByZip = new Dictionary<int, List<SwissPostStreetInformation>> ();
			this.userFriendlyStreetNameToSwissPostStreetMap = new Dictionary<string, string> ();

			foreach (var street in this.streets)
			{
				List<SwissPostStreetInformation> list;

				if (this.streetByZip.TryGetValue (street.ZipCode, out list) == false)
				{
					list = new List<SwissPostStreetInformation> ();
					this.streetByZip[street.ZipCode] = list;
				}

				list.Add (street);

				var swissPostStreetName    = street.StreetName;
				var userFriendlyStreetName = SwissPostStreet.ConvertToUserFriendlyStreetName (swissPostStreetName);
				var zipStreetKey           = SwissPostStreetRepository.GetZipStreetKey (street.ZipCode, userFriendlyStreetName);

				//	Make sure that a given user friendly name always maps to the same postal name.
				//	This should be the case, since the data comes from the MAT[CH] Street database.

				if (this.userFriendlyStreetNameToSwissPostStreetMap.ContainsKey (zipStreetKey))
				{
					var oldName = this.userFriendlyStreetNameToSwissPostStreetMap[zipStreetKey];
					var newName = swissPostStreetName;

					if (oldName != newName)
					{
						System.Diagnostics.Debug.WriteLine ("MAT[CH]street not consistent: {0} -> {1} and {2}", zipStreetKey, oldName, newName);
					}
				}

				this.userFriendlyStreetNameToSwissPostStreetMap[zipStreetKey] = swissPostStreetName;
			}
		}

		public IList<SwissPostStreetInformation> Streets
		{
			get
			{
				return this.streets.AsReadOnly ();
			}
		}

		public static readonly SwissPostStreetRepository Current = new SwissPostStreetRepository ();


		public IEnumerable<SwissPostStreetInformation> FindStreets(int zipCode)
		{
			List<SwissPostStreetInformation> list;
			
			if (this.streetByZip.TryGetValue (zipCode, out list))
			{
				return list;
			}
			else
			{
				return EmptyList<SwissPostStreetInformation>.Instance;
			}
		}

		public IEnumerable<SwissPostStreetInformation> FindStreets(int zipCode, string rootName)
		{
			List<SwissPostStreetInformation> list;
			
			if (this.streetByZip.TryGetValue (zipCode, out list))
			{
				var upperRootName = TextConverter.ConvertToUpperAndStripAccents (rootName);

				return list.Where (x => x.StreetNameShort == rootName || x.StreetNameRoot == upperRootName);
			}
			else
			{
				return EmptyList<SwissPostStreetInformation>.Instance;
			}
		}

		public string MapUserFriendlyStreetNameToSwissPostStreet(int zipCode, string street)
		{
			string key = SwissPostStreetRepository.GetZipStreetKey (zipCode, street);
			string result;

			if (this.userFriendlyStreetNameToSwissPostStreetMap.TryGetValue (key, out result))
			{
				return result;
			}
			else
			{
				return null;
			}
		}

		
		private static string GetZipStreetKey(int zipCode, string street)
		{
			return string.Format ("{0:0000}.{1}", zipCode, street);
		}

		private readonly Dictionary<int, List<SwissPostStreetInformation>> streetByZip;
		private readonly Dictionary<string, string> userFriendlyStreetNameToSwissPostStreetMap;
		private readonly List<SwissPostStreetInformation> streets;
	}
}
