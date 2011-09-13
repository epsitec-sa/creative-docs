//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	public static class PriceCalculator
	{
		public static void UpdatePrices(IPriceCalculator calculator)
		{
			if (PriceCalculator.activeCalculator != null)
			{
				//	Calls to UpdatePrices while the document price calculator is working
				//	should never happen, since the business rules applied when updating
				//	an invoice cannot fire recursively. However, if this is invoked manually
				//	in order to test the code, it can nevertheless happen.
				
				return;
			}

			try
			{
				PriceCalculator.activeCalculator = calculator;
				PriceCalculator.activeCalculator.UpdatePrices ();
			}
			finally
			{
				PriceCalculator.activeCalculator = null;
			}
		}



		/// <summary>
		/// Rounds the amount to the nearest cent.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="cent">The value of a cent (0.01).</param>
		/// <returns>The rounded value.</returns>
		public static decimal RoundToCents(decimal value, decimal cent = 0.01M)
		{
			if (value < 0)
			{
				return -PriceCalculator.RoundToCents (-value, cent);
			}

			System.Diagnostics.Debug.Assert (cent > 0M);

			decimal halfCent = cent / 2;
			decimal rest     = value % cent;

			if (rest < halfCent)
			{
				return value - rest;
			}
			else
			{
				return value + cent - rest;
			}
		}

		public static decimal ClipPriceValue(decimal value, CurrencyCode currency = CurrencyCode.None)
		{
			//	Currently, keep only cents for any price value. Maybe we should consider
			//	using 3 decimal places for some currencies ?

			return PriceCalculator.RoundToCents (value);
		}

		public static decimal? ClipPriceValue(decimal? value, CurrencyCode currency = CurrencyCode.None)
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

		public static decimal ClipPercentValue(decimal value)
		{
			//	Percent up to 4 digits after the decimal point (this represents a millionth
			//	of a part and should be enough for any purposes).

			return 0.01M * 0.0001M * System.Math.Round (value * 100 * 10000);
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

		public static decimal Sum(params decimal?[] values)
		{
			decimal sum = 0;

			foreach (var value in values)
			{
				if (value.HasValue)
				{
					sum += value.Value;
				}
			}

			return sum;
		}
		
		
		[System.ThreadStatic]
		private static IPriceCalculator	activeCalculator;
	}
}
