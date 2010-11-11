//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	public class PriceCalculator
	{
		public static decimal ClipPriceValue(decimal value, CurrencyCode currency)
		{
			//	Currently, keep only cents for any price value. Maybe we should consider
			//	using 3 decimal places after the EUR...

			return 0.01M * System.Math.Round (value * 100);
		}
		
		public static decimal? ClipPriceValue(decimal? value, CurrencyCode currency)
		{
			if (value.HasValue)
			{
				return PriceCalculator.ClipPriceValue (value.Value, currency);
			}
			else
			{
				return null;
			}
		}
		
		public static decimal ClipTaxRateValue(decimal value)
		{
			//	The tax rate will be expressed as 8.000000 % (six digits after the decimal point)
			//	in order to limit the rounding errors when the VAT rate is the result of some
			//	composition of two or more other rates.

			return 0.01M * 0.000001M * System.Math.Round (value * 100 * 1000000);
		}

		public static decimal? ClipTaxRateValue(decimal? value)
		{
			if (value.HasValue)
			{
				return PriceCalculator.ClipTaxRateValue (value.Value);
			}
			else
			{
				return null;
			}
		}
	}
}
