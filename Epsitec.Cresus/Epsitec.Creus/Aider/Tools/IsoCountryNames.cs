//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Data.Platform;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Tools
{
	internal sealed class IsoCountryNames
	{
		private IsoCountryNames()
		{
			this.map = Iso3166.GetCountries ("FR").ToDictionary (c => c.IsoAlpha2, c => c.Name);
		}


		public string							this[string isoCode]
		{
			get
			{
				string countryName;

				this.map.TryGetValue (isoCode, out countryName);

				return countryName;
			}
		}

		private readonly Dictionary<string, string> map;


		public static readonly IsoCountryNames	Instance = new IsoCountryNames ();
	}
}
