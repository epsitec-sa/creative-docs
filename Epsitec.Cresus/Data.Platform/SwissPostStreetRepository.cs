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
			this.streets = new List<ISwissPostStreetInformation> (SwissPostStreet.GetStreets ());
			this.streetByZip = new Dictionary<SwissPostFullZip, List<ISwissPostStreetInformation>> ();
			this.streetByUserFriendlyStreetName = new Dictionary<string, ISwissPostStreetInformation> ();

			foreach (var street in this.streets)
			{
				List<ISwissPostStreetInformation> list;
				
				if (this.streetByZip.TryGetValue (street.ZipCodeAndAddOn, out list) == false)
				{
					list = new List<ISwissPostStreetInformation> ();
					this.streetByZip[street.ZipCodeAndAddOn] = list;
				}

				list.Add (street);

				var swissPostStreetName    = street.StreetName;
				var userFriendlyStreetName = SwissPostStreet.ConvertToUserFriendlyStreetName (swissPostStreetName);
				var zipStreetKey           = SwissPostStreetRepository.GetZipStreetKey (street.ZipCode, street.ZipCodeAddOn, userFriendlyStreetName);

				//	Make sure that a given user friendly name always maps to the same postal name.
				//	This should be the case, since the data comes from the MAT[CH] Street database.

				if (this.streetByUserFriendlyStreetName.ContainsKey (zipStreetKey))
				{
					var oldName = this.streetByUserFriendlyStreetName[zipStreetKey].StreetName;
					var newName = swissPostStreetName;

					if (oldName != newName)
					{
						System.Diagnostics.Debug.WriteLine (string.Format ("MAT[CH]street not consistent: {0} -> {1} and {2}", zipStreetKey, oldName, newName));
					}
				}

				this.streetByUserFriendlyStreetName[zipStreetKey] = street;
			}
		}

		public IList<ISwissPostStreetInformation> Streets
		{
			get
			{
				return this.streets.AsReadOnly ();
			}
		}

		public static readonly SwissPostStreetRepository Current = new SwissPostStreetRepository ();

		public IEnumerable<ISwissPostStreetInformation> FindStreets(int zipCode, int zipAddOn)
		{
			List<ISwissPostStreetInformation> list;
			
			if (this.streetByZip.TryGetValue (new SwissPostFullZip (zipCode, zipAddOn), out list))
			{
				return list;
			}
			else
			{
				return EmptyList<ISwissPostStreetInformation>.Instance;
			}
		}

		public IEnumerable<ISwissPostStreetInformation> FindStreets(int zipCode, int zipAddOn, string rootName)
		{
			List<ISwissPostStreetInformation> list;

			if (this.streetByZip.TryGetValue (new SwissPostFullZip (zipCode, zipAddOn), out list))
			{
				var upperRootName = TextConverter.ConvertToUpperAndStripAccents (rootName);

				return list.Where (x => x.StreetNameShort == rootName || x.StreetNameRoot == upperRootName);
			}
			else
			{
				return EmptyList<ISwissPostStreetInformation>.Instance;
			}
		}

		/// <summary>
		/// Finds the street information, like <see cref="FindStreetFromStreetName"/>, but also
		/// matches on similar zip codes (e.g. 1401 would map with 1400). This is required, since
		/// Swiss addresses of post boxes can have dedicated zip codes, and this makes street
		/// matching impossible without relaxed zip code matching.
		/// </summary>
		/// <param name="zipCode">The zip code.</param>
		/// <param name="zipAddOn">The zip code complement.</param>
		/// <param name="street">The street.</param>
		/// <returns>The <see cref="MatchLightStreetInformation"/> or <c>null</c>.</returns>
		public ISwissPostStreetInformation FindStreetFromStreetNameRelaxed(int zipCode, int zipAddOn, string street)
		{
			var info = this.FindStreetFromStreetName (zipCode, zipAddOn, street);

			if (info != null)
			{
				return info;
			}

			//	In Lausanne, 1000..1019 all map to the base zip code 1000, for instance:
			
			var baseZipCode = SwissPostZipCodeFoldingRepository.Resolve (zipCode, zipAddOn).BaseZipCode;

			//	Check to see in all derived zip codes if there is a matching street name.

			foreach (var item in SwissPostZipCodeFoldingRepository.FindDerived (baseZipCode))
			{
				var otherZipCode  = item.ZipCode;
				var otherZipAddOn = item.ZipCodeAddOn;

				if (otherZipCode != zipCode)
				{
					info = this.FindStreetFromStreetName (otherZipCode, otherZipAddOn, street);

					if (info != null)
					{
						return info;
					}
				}
			}

			return null;
		}

		public ISwissPostStreetInformation FindStreetFromStreetName(int zipCode, int zipAddOn, string street)
		{
			var normalizedName = SwissPostStreet.NormalizeStreetName (street);
			var matchingInfos  = this.FindStreets (zipCode, zipAddOn).Where (x => x.NormalizedStreetName == normalizedName);

			var found = matchingInfos.FirstOrDefault ();

			if (found != null)
			{
				return found;
			}

			var tokens  = SwissPostStreet.TokenizeStreetName (street).ToArray ();
			matchingInfos = this.FindStreets (zipCode, zipAddOn).Where (x => x.MatchNameWithHeuristics (tokens));

			found = matchingInfos.FirstOrDefault ();

			return found;
		}

		public bool IsStreetKnownRelaxed(int zipCode, int zipAddOn, string street)
		{
			return this.FindStreetFromStreetNameRelaxed (zipCode, zipAddOn, street) != null;
		}

		public bool IsStreetKnown(int zipCode, int zipAddOn, string street)
		{
			return this.FindStreetFromStreetName (zipCode, zipAddOn, street) != null;
		}

		public bool IsBusinessAddressOrPostBox(int zipCode, int zipAddOn, string postBox)
		{
			//	We were not able to match the street based on the given ZIP code. But the
			//	ZIP code might be the one associated with the post box, or it might be a
			//	special ZIP code referring to an administration or a big company.
			//
			//	Examples:
			//
			//	Martin Schwarz, rue de la Plaine 13, Case postale, 1401 Yverdon-les-Bains
			//	Service de la population, Division Etrangers, Av. de Beaulieu 19, 1014 Lausanne

			var info = SwissPostZipCodeFoldingRepository.Resolve (zipCode, zipAddOn);

			return info.ZipCodeType == SwissPostZipType.Company || (string.IsNullOrEmpty (postBox) == false);
		}

		
		internal ISwissPostStreetInformation FindStreetFromUserFriendlyStreetNameDictionary(int zipCode, int zipAddOn, string street)
		{
			ISwissPostStreetInformation result;
			string key = SwissPostStreetRepository.GetZipStreetKey (zipCode, zipAddOn, street);

			if (this.streetByUserFriendlyStreetName.TryGetValue (key, out result))
			{
				return result;
			}
			else
			{
				return null;
			}
		}

		private static string GetZipStreetKey(int zipCode, int zipAddOn, string street)
		{
			return string.Format ("{0:0000}:{1:00}.{2}", zipCode, zipAddOn, street);
		}

		private readonly Dictionary<SwissPostFullZip, List<ISwissPostStreetInformation>> streetByZip;
		private readonly Dictionary<string, ISwissPostStreetInformation> streetByUserFriendlyStreetName;
		private readonly List<ISwissPostStreetInformation> streets;
	}
}
