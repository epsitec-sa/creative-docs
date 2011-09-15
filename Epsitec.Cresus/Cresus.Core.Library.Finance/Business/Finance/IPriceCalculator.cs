//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{
	/// <summary>
	/// The <c>IPriceCalculator</c> interface is implemented by the real price calculators,
	/// which can be provided by external assemblies.
	/// </summary>
	public interface IPriceCalculator : System.IDisposable
	{
		/// <summary>
		/// Updates the prices of the attached business document. This will most likely
		/// change the resulting prices and final prices of all lines in the invoice.
		/// This will also create lines for the taxes (VAT) and the grand total, if
		/// they are not yet present in the document.
		/// </summary>
		void UpdatePrices();

		/// <summary>
		/// Rounds the specified value according to the attached business document
		/// rounding rules.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="policy">The rounding policy.</param>
		/// <returns>The rounded value.</returns>
		decimal Round(decimal value, RoundingPolicy policy);

		/// <summary>
		/// Gets the billing mode in use in the attached business document.
		/// </summary>
		/// <returns>The billing mode.</returns>
		BillingMode GetBillingMode();
	}
}
