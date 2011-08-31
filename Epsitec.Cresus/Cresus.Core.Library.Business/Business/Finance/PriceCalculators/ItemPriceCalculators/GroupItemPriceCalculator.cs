//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators.ItemPriceCalculators
{
	public class GroupItemPriceCalculator : AbstractItemPriceCalculator
	{
		public GroupItemPriceCalculator(int groupLevel)
		{
			this.members = new List<AbstractItemPriceCalculator> ();
			this.groupLevel = groupLevel;
		}


		public int								GroupLevel
		{
			get
			{
				return this.groupLevel;
			}
		}

		public decimal							TotalPriceBeforeTax
		{
			get
			{
				return this.totalPriceBeforeTaxDiscountable + this.totalPriceBeforeTaxNotDiscountable;
			}
		}

		public decimal							TotalPriceBeforeTaxDiscountable
		{
			get
			{
				return this.totalPriceBeforeTaxDiscountable;
			}
		}

		public decimal							TotalPriceBeforeTaxNotDiscountable
		{
			get
			{
				return this.totalPriceBeforeTaxNotDiscountable;
			}
		}

		public decimal							MeanDiscountableTaxRate
		{
			get
			{
				if (this.totalPriceBeforeTaxDiscountable == 0)
				{
					return 0;
				}

				return this.totalTaxDiscountable / this.totalPriceBeforeTaxDiscountable;
			}
		}

		public decimal							TotalTax
		{
			get
			{
				return this.totalTaxDiscountable + this.totalTaxNotDiscountable;
			}
		}

		public decimal							TotalTaxDiscountable
		{
			get
			{
				return this.totalTaxDiscountable;
			}
		}

		public decimal							TotalTaxNotDiscountable
		{
			get
			{
				return this.totalTaxNotDiscountable;
			}
		}

		public Tax								TaxDiscountable
		{
			get
			{
				return this.taxDiscountable;
			}
		}

		public Tax								TaxNotDiscountable
		{
			get
			{
				return this.taxNotDiscountable;
			}
		}


		public void IncludeSubGroups(DocumentPriceCalculator calculator, int level)
		{
			calculator.IncludeSubGroups (this, level);
		}

		public void Add(GroupItemPriceCalculator calculator)
		{
			calculator.members.OfType<ArticleItemPriceCalculator> ().ForEach (x => this.Add (x));
			calculator.members.OfType<SubTotalItemPriceCalculator> ().ForEach (x => this.Add (x));
		}

		public void Add(ArticleItemPriceCalculator calculator)
		{
			this.members.Add (calculator);
			this.Accumulate (calculator.Tax, calculator.NotDiscountable);
		}

		public void Add(SubTotalItemPriceCalculator calculator)
		{
			var group = calculator.Group;

			this.members.Add (calculator);

			decimal totalBeforeAdjustment = group.TotalPriceBeforeTax;
			decimal totalAfterAdjustment  = calculator.Item.ResultingPriceBeforeTax.GetValueOrDefault (0M);
			decimal delta = totalAfterAdjustment - totalBeforeAdjustment;
			decimal discountableBefore = group.totalPriceBeforeTaxDiscountable;
			decimal discountableAfter  = discountableBefore + delta;

			decimal ratio = GroupItemPriceCalculator.GetRatio (discountableBefore, discountableAfter);

			if (group.TaxDiscountable != null)
			{
				Tax discountedTax = new Tax (group.TaxDiscountable.RateAmounts.Select (tax => new TaxRateAmount (tax.Amount * ratio, tax.Code, tax.Rate)));

				this.Accumulate (discountedTax, neverApplyDiscount: false);
			}
			if (group.TaxNotDiscountable != null)
			{
				this.Accumulate (group.TaxNotDiscountable, neverApplyDiscount: true);
			}
		}


		public void AdjustFinalPrices(decimal desiredPriceBeforeTax)
		{
			decimal delta  = desiredPriceBeforeTax - this.TotalPriceBeforeTax;
			decimal result = this.TotalPriceBeforeTaxDiscountable;
			decimal final  = result + delta;
			decimal ratio = GroupItemPriceCalculator.GetRatio (result, final);
			this.members.ForEach (x => x.ApplyFinalPriceAdjustment (ratio));
		}

		public override void ApplyFinalPriceAdjustment(decimal adjustment)
		{
			throw new System.NotImplementedException ();
		}


		public void Accumulate(Tax tax, bool neverApplyDiscount)
		{
			if (tax != null)
			{
				if (neverApplyDiscount)
				{
					this.AccumulateNotDiscountable (tax);
				}
				else
				{
					this.AccumulateDiscountable (tax);
				}
			}
		}

		private void AccumulateDiscountable(Tax tax)
		{
			this.totalPriceBeforeTaxDiscountable += tax.TotalAmount;
			this.totalTaxDiscountable += tax.TotalTax;
			this.taxDiscountable = Tax.Combine (tax, this.taxDiscountable);
		}

		private void AccumulateNotDiscountable(Tax tax)
		{
			this.totalPriceBeforeTaxNotDiscountable += tax.TotalAmount;
			this.totalTaxNotDiscountable += tax.TotalTax;
			this.taxNotDiscountable = Tax.Combine (tax, this.taxNotDiscountable);
		}


		public void ComputeDiscountBeforeTax(decimal expectedPriceBeforeTax, out decimal totalBeforeTaxDiscountable, out decimal totalTaxDiscountable, out Tax taxDiscountable)
		{
			decimal total = this.TotalPriceBeforeTax;
			decimal delta = expectedPriceBeforeTax - total;

			totalBeforeTaxDiscountable = this.TotalPriceBeforeTaxDiscountable + delta;
			totalTaxDiscountable       = totalBeforeTaxDiscountable * this.MeanDiscountableTaxRate;

			taxDiscountable = this.ComputeAdjustedTax (this.totalPriceBeforeTaxDiscountable, totalBeforeTaxDiscountable);
		}

		public void ComputeDiscountAfterTax(decimal expectedPriceAfterTax, out decimal totalBeforeTaxDiscountable, out decimal totalTaxDiscountable, out Tax taxDiscountable)
		{
			decimal total = this.TotalPriceBeforeTax + this.TotalTax;
			decimal minus = total - expectedPriceAfterTax;

			minus = minus / (1M + this.MeanDiscountableTaxRate);

			totalBeforeTaxDiscountable = this.TotalPriceBeforeTaxDiscountable - minus;
			totalTaxDiscountable       = totalBeforeTaxDiscountable * this.MeanDiscountableTaxRate;

			taxDiscountable = this.ComputeAdjustedTax (this.totalPriceBeforeTaxDiscountable, totalBeforeTaxDiscountable);
		}

		private static decimal GetRatio(decimal before, decimal after)
		{
			decimal ratio  = 1;

			if (System.Math.Abs (after - before) < 0.01M)
			{
				//	Same totals, within 0.01 monetary unit. We won't need to do any
				//	real adjustments.
			}
			else if (before == 0)
			{
				System.Diagnostics.Debug.WriteLine ("Cannot adjust final price since it is zero");
			}
			else
			{
				ratio = after / before;
			}

			return ratio;
		}

		private Tax ComputeAdjustedTax(decimal before, decimal after)
		{
			if (before == 0)
			{
				//	There is no total price before we apply the discount; we cannot produce a
				//	meaningful Tax record :

				return null;
			}
			else if (this.taxDiscountable == null)
			{
				return null;
			}
			else
			{
				decimal ratio = after / before;
				
				return new Tax (this.taxDiscountable.RateAmounts.Select (tax => new TaxRateAmount (tax.Amount * ratio, tax.Code, tax.Rate)));
			}
		}

		private readonly List<AbstractItemPriceCalculator> members;
		private readonly int					groupLevel;

		private decimal							totalPriceBeforeTaxDiscountable;
		private decimal							totalPriceBeforeTaxNotDiscountable;
		private decimal							totalTaxDiscountable;
		private decimal							totalTaxNotDiscountable;

		private Tax								taxDiscountable;
		private Tax								taxNotDiscountable;
	}
}
