//	Copyright � 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	/// <summary>
	/// The <c>RoundingPolicyExtensions</c> class implements methods which operate on
	/// the <see cref="RoundingPolicy"/> enumeration.
	/// </summary>
	public static class RoundingPolicyExtensions
	{
		public static bool Compatible(this RoundingPolicy valueA, RoundingPolicy valueB)
		{
			return (valueA & valueB) == valueB;
		}

		public static IEnumerable<RoundingPolicy> GetFlags(this RoundingPolicy value)
		{
			return RoundingPolicyExtensions.Flags.Where (x => value.HasFlag (x));
		}

		private static RoundingPolicy[] Flags = new RoundingPolicy[]
		{
			RoundingPolicy.OnUnitPrice,
			RoundingPolicy.OnLinePrice,
			RoundingPolicy.OnTotalRounding,
			RoundingPolicy.OnTotalPrice,
			RoundingPolicy.OnTotalVat,
			RoundingPolicy.OnEndTotal,
		};
	}
}
