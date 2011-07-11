//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators.ItemPriceCalculators
{
	/// <summary>
	/// The <c>AbstractItemPriceCalculator</c> class is the base class for all
	/// specific price calculators.
	/// </summary>
	public abstract class AbstractItemPriceCalculator
	{
		/// <summary>
		/// Applies the final price adjustment. This is a relative amount, expressed
		/// in the active currency. <c>0</c> means no adjustment.
		/// </summary>
		/// <param name="adjustment">The adjustment.</param>
		public abstract void ApplyFinalPriceAdjustment(decimal adjustment);
	}
}
