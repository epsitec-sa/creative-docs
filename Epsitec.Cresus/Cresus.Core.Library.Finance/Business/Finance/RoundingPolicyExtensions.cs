//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	public static class RoundingPolicyExtensions
	{
		public static bool BeforeTax(this RoundingPolicy value)
		{
			if (value == RoundingPolicy.None)
			{
				return false;
			}
			if (value == RoundingPolicy.All)
			{
				return true;
			}

			switch (value)
			{
				case RoundingPolicy.OnLinePriceBeforeTax:
				case RoundingPolicy.OnUnitPriceBeforeTax:
					return true;

				case RoundingPolicy.OnLinePriceAfterTax:
				case RoundingPolicy.OnUnitPriceAfterTax:
					return false;

				default:
					throw new System.NotSupportedException (string.Format ("{0} is not supported", value.GetQualifiedName ()));
			}
		}

		public static bool AfterTax(this RoundingPolicy value)
		{
			if (value == RoundingPolicy.None)
			{
				return false;
			}

			return value.BeforeTax () == false;
		}

		public static bool Compatible(this RoundingPolicy valueA, RoundingPolicy valueB)
		{
			if (valueA == valueB)
			{
				return true;
			}

			if ((valueA == RoundingPolicy.None) ||
					(valueB == RoundingPolicy.None))
			{
				return false;
			}

			if ((valueA == RoundingPolicy.All) ||
					(valueB == RoundingPolicy.All))
			{
				return true;
			}

			return false;
		}
	}
}
