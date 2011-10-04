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
		public TaxRateAmount(decimal amount, VatRateType code, decimal rate)
		{
			this.amount   = amount;
			this.codeRate = new VatCodeRate (code, rate);
		}


		/// <summary>
		/// Gets the VAT rate.
		/// </summary>
		/// <value>The VAT rate.</value>
		public decimal							Rate
		{
			get
			{
				return this.codeRate.Rate;
			}
		}

		/// <summary>
		/// Gets the VAT rate type.
		/// </summary>
		/// <value>The VAT rate type.</value>
		public VatRateType						VatRateType
		{
			get
			{
				return this.codeRate.Code;
			}
		}

		/// <summary>
		/// Gets the VAT code and rate as a <see cref="VatCodeRate"/> instance.
		/// </summary>
		/// <value>The VAT code and rate.</value>
		public VatCodeRate						CodeRate
		{
			get
			{
				return this.codeRate;
			}
		}

		/// <summary>
		/// Gets the amount before tax.
		/// </summary>
		/// <value>The amount before tax.</value>
		public decimal							Amount
		{
			get
			{
				return this.amount;
			}
		}

		/// <summary>
		/// Gets the tax for the amount (this is computed by multiplying the
		/// <see cref="Amount"/> with the <see cref="Rate"/>).
		/// </summary>
		/// <value>The tax for the amount.</value>
		public decimal							Tax
		{
			get
			{
				return this.Rate * this.Amount;
			}
		}

		
		private readonly decimal				amount;
		private readonly VatCodeRate			codeRate;
	}
}