//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Data.Platform
{
	public static class SwissPost
	{
		public static void Initialize()
		{
			var streetRepo = SwissPostStreetRepository.Current;
			var zipRepo    = SwissPostZipRepository.Current;
			var countries  = Iso3166.GetCountries ("FR").ToArray ();
		}
	}
}
