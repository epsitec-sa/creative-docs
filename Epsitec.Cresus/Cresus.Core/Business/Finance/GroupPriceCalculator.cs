//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		public GroupPriceCalculator()
		{
			this.members = new List<AbstractPriceCalculator> ();
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



		public void Add(ArticlePriceCalculator calculator)
		{
			var item  = calculator.ArticleItem;
			var value = item.ResultingLinePriceBeforeTax.Value;
			var tax   = PriceCalculator.Sum (item.ResultingLineTax1, item.ResultingLineTax2);

			this.members.Add (calculator);
			this.Accumulate (value, tax, item.NeverApplyDiscount);
		}

		public void Add(SubTotalPriceCalculator calculator)
		{
			var group = calculator.Group;

			this.members.Add (calculator);
			
			this.Accumulate (group.TotalPriceBeforeTaxDiscountable, group.TotalTaxDiscountable, neverApplyDiscount:false);
			this.Accumulate (group.totalPriceBeforeTaxNotDiscountable, group.TotalTaxNotDiscountable, neverApplyDiscount: true);
		}


		public void Accumulate(decimal valueBeforeTax, decimal tax, bool neverApplyDiscount)
		{
			if (neverApplyDiscount)
			{
				this.AccumulateNotDiscountable (valueBeforeTax, tax);
			}
			else
			{
				this.AccumulateDiscountable (valueBeforeTax, tax);
			}
		}

		public void AccumulateDiscountable(decimal valueBeforeTax, decimal tax)
		{
			this.totalPriceBeforeTaxDiscountable += valueBeforeTax;
			this.totalTaxDiscountable += tax;
		}

		public void AccumulateNotDiscountable(decimal valueBeforeTax, decimal tax)
		{
			this.totalPriceBeforeTaxNotDiscountable += valueBeforeTax;
			this.totalTaxNotDiscountable += tax;
		}


		public void ComputeDiscountBeforeTax(decimal expectedPriceBeforeTax, out decimal totalBeforeTaxDiscountable, out decimal totalTaxDiscountable)
		{
			decimal total = this.TotalPriceBeforeTax;
			decimal minus = total - expectedPriceBeforeTax;

			totalBeforeTaxDiscountable = this.TotalPriceBeforeTaxDiscountable - minus;
			totalTaxDiscountable       = totalBeforeTaxDiscountable * this.MeanDiscountableTaxRate;
		}

		public void ComputeDiscountAfterTax(decimal expectedPriceAfterTax, out decimal totalBeforeTaxDiscountable, out decimal totalTaxDiscountable)
		{
			decimal total = this.TotalPriceBeforeTax + this.TotalTax;
			decimal minus = total - expectedPriceAfterTax;

			minus = minus / (1M + this.MeanDiscountableTaxRate);

			totalBeforeTaxDiscountable = this.TotalPriceBeforeTaxDiscountable - minus;
			totalTaxDiscountable       = totalBeforeTaxDiscountable * this.MeanDiscountableTaxRate;
		}


		private readonly List<AbstractPriceCalculator> members;

		private decimal							totalPriceBeforeTaxDiscountable;
		private decimal							totalPriceBeforeTaxNotDiscountable;
		private decimal							totalTaxDiscountable;
		private decimal							totalTaxNotDiscountable;
	}
}
