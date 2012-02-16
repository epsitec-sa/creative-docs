//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			IEnumerable<SwissPostStreetInformation> streets = SwissPostStreet.GetStreets ();

			this.streetByZip = new Dictionary<string, List<SwissPostStreetInformation>> ();

			foreach (var street in streets)
			{
				List<SwissPostStreetInformation> list;

				if (this.streetByZip.TryGetValue (street.ZipCode, out list) == false)
				{
					list = new List<SwissPostStreetInformation> ();
					this.streetByZip[street.ZipCode] = list;
				}

				list.Add (street);
			}
		}


		public static readonly SwissPostStreetRepository Current = new SwissPostStreetRepository ();


		public IEnumerable<SwissPostStreetInformation> FindStreets(int zipCode)
		{
			List<SwissPostStreetInformation> list;
			string key = zipCode.ToString ("####");

			if (this.streetByZip.TryGetValue (key, out list))
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
			string key = zipCode.ToString ("####");

			if (this.streetByZip.TryGetValue (key, out list))
			{
				var upperRootName = TextConverter.ConvertToUpperAndStripAccents (rootName);

				return list.Where (x => x.StreetNameShort == rootName || x.StreetNameRoot == upperRootName);
			}
			else
			{
				return EmptyList<SwissPostStreetInformation>.Instance;
			}
		}

		private readonly Dictionary<string, List<SwissPostStreetInformation>> streetByZip;
	}
}
