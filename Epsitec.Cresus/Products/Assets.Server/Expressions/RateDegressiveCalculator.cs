//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	public class RateDegressiveCalculator : AbstractCalculator
	{
		public RateDegressiveCalculator(AmortizationDetails details)
			: base (details)
		{
		}

		public override decimal Evaluate()
		{
			decimal value = this.InitialAmount;

			//----------------------------------------------
			var rate = this.Rate * this.PeriodicityFactor * this.ProrataFactor;
			var amortization = this.InitialAmount * rate;

			value = value - amortization;
			value = this.Round (value);
			value = this.Residual (value);
			//----------------------------------------------

			return value;
		}
	}
}
