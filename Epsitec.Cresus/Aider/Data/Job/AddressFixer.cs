using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Data.Platform;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

namespace Epsitec.Aider.Data.Job
{
	class AddressFixer
	{
		public static bool CheckAmbigousStreet(SwissPostStreetInformation street)
		{
			return rep.Streets.Where(s => s.NormalizedStreetName == street.NormalizedStreetName).Where(z => z.ZipCode == street.ZipCode).Count() > 1;
		}

		public static ISet<SwissPostStreetInformation> GetSet()
		{
			return rep.Streets
				.Distinct(new LambdaComparer<SwissPostStreetInformation>((s1,s2) => s1.NormalizedStreetName == s2.NormalizedStreetName && s1.ZipCode == s2.ZipCode && s1.ZipCodeAddOn == s2.ZipCodeAddOn, s => s.NormalizedStreetName.GetHashCode()))
				.GroupBy (s => Tuple.Create (s.NormalizedStreetName, s.ZipCode))
				.Select (g => g.ToList ())
				.Where (g => g.Count > 1)
				.SelectMany (g => g)
				.ToSet ();
		}
		private static SwissPostStreetRepository rep = SwissPostStreetRepository.Current;
	} 
}
