//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	public static class DiscountPolicyExtensions
	{
		public static bool Compatible(this DiscountPolicy valueA, DiscountPolicy valueB)
		{
			if (valueA == valueB)
			{
				return true;
			}
			
			if ((valueA == DiscountPolicy.None) ||
				(valueB == DiscountPolicy.None))
			{
				return false;
			}
			
			if ((valueA == DiscountPolicy.All) ||
				(valueB == DiscountPolicy.All))
			{
				return true;
			}

			return false;
		}
	}
}
