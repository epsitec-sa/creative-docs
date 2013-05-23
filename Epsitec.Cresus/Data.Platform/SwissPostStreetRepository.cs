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
			this.streetByUserFriendlyStreetName = new Dictionary<string, SwissPostStreetInformation> ();

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

				if (this.streetByUserFriendlyStreetName.ContainsKey (zipStreetKey))
				{
					var oldName = this.streetByUserFriendlyStreetName[zipStreetKey].StreetName;
					var newName = swissPostStreetName;

					if (oldName != newName)
					{
						System.Diagnostics.Debug.WriteLine ("MAT[CH]street not consistent: {0} -> {1} and {2}", zipStreetKey, oldName, newName);
					}
				}

				this.streetByUserFriendlyStreetName[zipStreetKey] = street;
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

		/// <summary>
		/// Finds the street information, like <see cref="FindStreetFromStreetName"/>, but also
		/// matches on similar zip codes (e.g. 1401 would map with 1400). This is required, since
		/// Swiss addresses of post boxes can have dedicated zip codes, and this makes street
		/// matching impossible without relaxed zip code matching.
		/// </summary>
		/// <param name="zipCode">The zip code.</param>
		/// <param name="street">The street.</param>
		/// <returns>The <see cref="SwissPostStreetInformation"/> or <c>null</c>.</returns>
		public SwissPostStreetInformation FindStreetFromStreetNameRelaxed(int zipCode, string street)
		{
			var info = this.FindStreetFromStreetName (zipCode, street);

			if (info != null)
			{
				return info;
			}

			//	In Lausanne, 1000..1019 all map to the base zip code 1000, for instance:
			
			var baseZipCode = SwissPostZipCodeFoldingRepository.Resolve (zipCode).BaseZipCode;

			//	Check to see in all derived zip codes if there is a matching street name.

			foreach (var item in SwissPostZipCodeFoldingRepository.FindDerived (baseZipCode))
			{
				var otherZipCode = item.ZipCode;

				if (otherZipCode != zipCode)
				{
					info = this.FindStreetFromStreetName (otherZipCode, street);

					if (info != null)
					{
						return info;
					}
				}
			}

			return null;
		}

		public SwissPostStreetInformation FindStreetFromStreetName(int zipCode, string street)
		{
			var normalizedName = SwissPostStreet.NormalizeStreetName (street);
			var matchingInfos  = this.FindStreets (zipCode).Where (x => x.NormalizedStreetName == normalizedName);

			var found = matchingInfos.FirstOrDefault ();

			if (found != null)
			{
				return found;
			}

			var tokens  = SwissPostStreet.TokenizeStreetName (street).ToArray ();
			matchingInfos = this.FindStreets (zipCode).Where (x => x.MatchNameWithHeuristics (tokens));

			found = matchingInfos.FirstOrDefault ();

			return found;
		}

		public bool IsStreetKnownRelaxed(int zipCode, string street)
		{
			return this.FindStreetFromStreetNameRelaxed (zipCode, street) != null;
		}

		public bool IsStreetKnown(int zipCode, string street)
		{
			return this.FindStreetFromStreetName (zipCode, street) != null;
		}

		internal SwissPostStreetInformation FindStreetFromUserFriendlyStreetNameDictionary(int zipCode, string street)
		{
			SwissPostStreetInformation result;
			string key = SwissPostStreetRepository.GetZipStreetKey (zipCode, street);

			if (this.streetByUserFriendlyStreetName.TryGetValue (key, out result))
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
		private readonly Dictionary<string, SwissPostStreetInformation> streetByUserFriendlyStreetName;
		private readonly List<SwissPostStreetInformation> streets;
	}
}
