//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{
	public interface IPriceCalculator : System.IDisposable
	{
		void UpdatePrices();
	}
}
