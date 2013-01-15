//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Data.Platform
{
	/// <summary>
	/// The <c>GeoNamesCountryInformation</c> class describes what is returned by the geonames.org web service.
	/// </summary>
	[System.Serializable]
	public sealed class GeoNamesCountryInformation
	{
		public string IsoAlpha2
		{
			get;
			set;
		}
		public string IsoAlpha3
		{
			get;
			set;
		}
		public string IsoNumeric
		{
			get;
			set;
		}
		public string FipsCode
		{
			get;
			set;
		}
		public string Name
		{
			get;
			set;
		}
		public string Capital
		{
			get;
			set;
		}
		public string Continent
		{
			get;
			set;
		}
		public string Languages
		{
			get;
			set;
		}
		public string Currency
		{
			get;
			set;
		}
	}
}
