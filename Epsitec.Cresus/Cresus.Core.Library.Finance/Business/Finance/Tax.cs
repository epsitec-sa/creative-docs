//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Collections;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	public class Tax
	{
		public Tax(params TaxRateAmount[] rateAmounts)
			: this((IEnumerable<TaxRateAmount>) rateAmounts)
		{
		}

		public Tax(IEnumerable<TaxRateAmount> rateAmounts)
		{
			this.rateAmounts = new ReadOnlyList<TaxRateAmount> (rateAmounts);

			this.totalAmount = this.rateAmounts.Sum (x => x.Amount);
			this.totalTax    = this.rateAmounts.Sum (x => x.Tax);
		}

		private Tax(IEnumerable<TaxRateAmount> rateAmounts, decimal totalAmount, decimal totalTax)
		{
			this.rateAmounts = new ReadOnlyList<TaxRateAmount> (rateAmounts);
			this.totalAmount = totalAmount;
			this.totalTax    = totalTax;
		}


		public IList<TaxRateAmount>			RateAmounts
		{
			get
			{
				return this.rateAmounts;
			}
		}

		public decimal						TotalAmount
		{
			get
			{
				return this.totalAmount;
			}
		}

		public decimal						TotalTax
		{
			get
			{
				return this.totalTax;
			}
		}

		public decimal						TotalTaxRate
		{
			get
			{
				if ((this.totalTax == 0) ||
					(this.totalAmount == 0))
				{
					return 0;
				}
				else
				{
					return this.totalTax / this.totalAmount;
				}
			}
		}


		public decimal ComputeAmountBeforeTax(decimal amountAfterTax)
		{
			if (this.TotalTaxRate == 0)
			{
				return amountAfterTax;
			}
			else
			{
				decimal directRate  = 1.0M + this.TotalTaxRate;
				decimal inverseRate = 1.0M / directRate;

				decimal after  = amountAfterTax * inverseRate;
				decimal cent   = 0.01M;
				decimal adjust = cent - (after % cent);

				return after + adjust;
			}
		}

		public decimal? GetTax(int index)
		{
			if ((index < 0) ||
				(index >= this.rateAmounts.Count))
			{
				return null;
			}

			return this.rateAmounts[index].Tax;
		}

		public decimal? GetTaxRate(int index)
		{
			if ((index < 0) ||
				(index >= this.rateAmounts.Count))
			{
				return null;
			}

			return this.rateAmounts[index].Rate;
		}


		public static Tax Combine(Tax a, Tax b)
		{
			if (a == null)
			{
				return b;
			}
			if (b == null)
			{
				return a;
			}

			var rateAmounts = from rateAmount in a.rateAmounts.Concat (b.rateAmounts)
							  group rateAmount by rateAmount.CodeRate into g
							  select new TaxRateAmount (g.Sum (x => x.Amount), g.Key.Code, g.Key.Rate);

			return new Tax (rateAmounts, a.totalAmount + b.totalAmount, a.totalTax + b.totalTax);
		}
		
		
		private readonly IList<TaxRateAmount> rateAmounts;
		private readonly decimal			totalAmount;
		private readonly decimal			totalTax;
	}
}
