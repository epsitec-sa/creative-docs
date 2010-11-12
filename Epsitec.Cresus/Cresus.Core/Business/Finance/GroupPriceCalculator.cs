//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	public class GroupPriceCalculator : AbstractPriceCalculator
	{
		public GroupPriceCalculator(int groupLevel)
		{
			this.members = new List<AbstractPriceCalculator> ();
			this.groupLevel = groupLevel;
		}


		public int								GroupLevel
		{
			get
			{
				return this.groupLevel;
			}
		}

		public IList<AbstractPriceCalculator>	Members
		{
			get
			{
				return this.members.AsReadOnly ();
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


		public void Add(GroupPriceCalculator calculator)
		{
			calculator.Members.OfType<ArticlePriceCalculator> ().ForEach (x => this.Add (x));
			calculator.Members.OfType<SubTotalPriceCalculator> ().ForEach (x => this.Add (x));
		}

		public void Add(ArticlePriceCalculator calculator)
		{
			var item  = calculator.ArticleItem;
			var tax   = calculator.Tax;

			this.members.Add (calculator);
			this.Accumulate (tax, item.NeverApplyDiscount);
		}

		public void Add(SubTotalPriceCalculator calculator)
		{
			var group = calculator.Group;

			this.members.Add (calculator);
			
			this.Accumulate (group.TaxDiscountable, neverApplyDiscount: false);
			this.Accumulate (group.TaxNotDiscountable, neverApplyDiscount: true);
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

		public void AccumulateDiscountable(Tax tax)
		{
			this.totalPriceBeforeTaxDiscountable += tax.TotalAmount;
			this.totalTaxDiscountable += tax.TotalTax;
			this.taxDiscountable = Tax.Combine (tax, this.taxDiscountable);
		}

		public void AccumulateNotDiscountable(Tax tax)
		{
			this.totalPriceBeforeTaxNotDiscountable += tax.TotalAmount;
			this.totalTaxNotDiscountable += tax.TotalTax;
			this.taxNotDiscountable = Tax.Combine (tax, this.taxNotDiscountable);
		}


		public void ComputeDiscountBeforeTax(decimal expectedPriceBeforeTax, out decimal totalBeforeTaxDiscountable, out decimal totalTaxDiscountable, out Tax taxDiscountable)
		{
			decimal total = this.TotalPriceBeforeTax;
			decimal minus = total - expectedPriceBeforeTax;

			totalBeforeTaxDiscountable = this.TotalPriceBeforeTaxDiscountable - minus;
			totalTaxDiscountable       = totalBeforeTaxDiscountable * this.MeanDiscountableTaxRate;

			taxDiscountable = this.ComputeAdjustedTax (totalBeforeTaxDiscountable);
		}

		public void ComputeDiscountAfterTax(decimal expectedPriceAfterTax, out decimal totalBeforeTaxDiscountable, out decimal totalTaxDiscountable, out Tax taxDiscountable)
		{
			decimal total = this.TotalPriceBeforeTax + this.TotalTax;
			decimal minus = total - expectedPriceAfterTax;

			minus = minus / (1M + this.MeanDiscountableTaxRate);

			totalBeforeTaxDiscountable = this.TotalPriceBeforeTaxDiscountable - minus;
			totalTaxDiscountable       = totalBeforeTaxDiscountable * this.MeanDiscountableTaxRate;

			taxDiscountable = this.ComputeAdjustedTax (totalBeforeTaxDiscountable);
		}

		private Tax ComputeAdjustedTax(decimal newTotal)
		{
			decimal oldTotal = this.TotalPriceBeforeTax;

			if (oldTotal == 0)
			{
				//	There is no total price before we apply the discount; we cannot produce a
				//	meaningful Tax record :

				return null;
			}
			else
			{
				decimal ratio = newTotal / oldTotal;
				
				return new Tax (this.taxDiscountable.RateAmounts.Select (tax => new TaxRateAmount (tax.Amount * ratio, tax.Code, tax.Rate)));
			}
		}

		private readonly List<AbstractPriceCalculator> members;
		private readonly int					groupLevel;

		private decimal							totalPriceBeforeTaxDiscountable;
		private decimal							totalPriceBeforeTaxNotDiscountable;
		private decimal							totalTaxDiscountable;
		private decimal							totalTaxNotDiscountable;

		private Tax								taxDiscountable;
        private Tax								taxNotDiscountable;
	}
}
