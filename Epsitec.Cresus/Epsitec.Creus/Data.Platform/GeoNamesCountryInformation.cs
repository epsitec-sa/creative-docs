//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Data.Platform
{
	/// <summary>
	/// The <c>GeoNamesCountryInformation</c> class describes what is returned by the geonames.org web service.
	/// </summary>
	public sealed class GeoNamesCountryInformation
	{
		public string							IsoAlpha2;
		public string							IsoAlpha3;
		public string							IsoNumeric;
		public string							FipsCode;
		public string							Name;
		public string							Capital;
		public string							Continent;
		public string							Languages;
		public string							Currency;
	}
}
