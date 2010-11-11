//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	public abstract class AbstractPriceCalculator
	{
	}

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

	public class SubTotalPriceCalculator : AbstractPriceCalculator
	{
		public SubTotalPriceCalculator(BusinessDocumentEntity document, SubTotalDocumentItemEntity totalItem)
		{
			this.document  = document;
			this.totalItem = totalItem;
			this.calculators = new List<AbstractPriceCalculator> ();
		}

		
		public void ComputePrice(GroupPriceCalculator groupPriceCalculator)
		{
			this.AssignCalculators (groupPriceCalculator.Members);
		}
		
		private void AssignCalculators(IEnumerable<AbstractPriceCalculator> calculators)
		{
			this.calculators.AddRange (calculators.Where (x => x != this));
		}

		
		private readonly BusinessDocumentEntity		document;
		private readonly SubTotalDocumentItemEntity	totalItem;
		private readonly List<AbstractPriceCalculator> calculators;
	}
}
