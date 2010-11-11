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



		public IList<AbstractPriceCalculator> Members
		{
			get
			{
				return this.members.AsReadOnly ();
			}
		}

		public decimal TotalPriceBeforeTax
		{
			get
			{
				return this.totalPriceBeforeTaxDiscountable + this.totalPriceBeforeTaxNotDiscountable;
			}
		}

		public decimal TotalPriceBeforeTaxDiscountable
		{
			get
			{
				return this.totalPriceBeforeTaxDiscountable;
			}
		}

		public decimal TotalPriceBeforeTaxNotDiscountable
		{
			get
			{
				return this.totalPriceBeforeTaxNotDiscountable;
			}
		}

		public decimal TotalTax
		{
			get
			{
				return this.totalTaxDiscountable + this.totalTaxNotDiscountable;
			}
		}

		public decimal TotalTaxDiscountable
		{
			get
			{
				return this.totalTaxDiscountable;
			}
		}

		public decimal TotalTaxNotDiscountable
		{
			get
			{
				return this.totalTaxNotDiscountable;
			}
		}



		public void Add(AbstractPriceCalculator calculator)
		{
			this.members.Add (calculator);
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


		private readonly List<AbstractPriceCalculator> members;

		private decimal totalPriceBeforeTaxDiscountable;
		private decimal totalPriceBeforeTaxNotDiscountable;
		private decimal totalTaxDiscountable;
		private decimal totalTaxNotDiscountable;
	}
}
