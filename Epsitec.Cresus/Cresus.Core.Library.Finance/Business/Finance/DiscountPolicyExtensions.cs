//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	public static class DiscountPolicyExtensions
	{
		public static bool BeforeTax(this DiscountPolicy value)
		{
			switch (value)
			{
				case DiscountPolicy.OnLinePriceBeforeTax:
				case DiscountPolicy.OnUnitPriceBeforeTax:
					return true;

				case DiscountPolicy.OnLinePriceAfterTax:
				case DiscountPolicy.OnUnitPriceAfterTax:
					return false;

				default:
					throw new System.NotSupportedException (string.Format ("{0} is not supported", value.GetQualifiedName ()));
			}
		}
		
		public static bool AfterTax(this DiscountPolicy value)
		{
			return value.BeforeTax () == false;
		}
	}
}
