//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	/// <summary>
	/// The <c>TaxRateAmount</c> structure stores the tax rate and the tax
	/// amount; it is used by the <see cref="TaxCaluclator"/>.
	/// </summary>
	public struct TaxRateAmount
	{
		public TaxRateAmount(decimal rate, decimal amount)
		{
			this.rate   = rate;
			this.amount = amount;
		}

		
		public decimal							Rate
		{
			get
			{
				return this.rate;
			}
		}

		public decimal							Amount
		{
			get
			{
				return this.amount;
			}
		}

		public decimal							Tax
		{
			get
			{
				return this.Rate * this.Amount;
			}
		}

		
		private readonly decimal rate;
		private readonly decimal amount;
	}
}
