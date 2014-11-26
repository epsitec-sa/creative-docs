//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	public class RateLinearCalculator : AbstractCalculator
	{
		public RateLinearCalculator(AmortizedAmount amount)
			: base (amount)
		{
		}

		public override object Evaluate()
		{
			decimal value = this.InitialAmount;

			//----------------------------------------------
			var rate = this.Rate * this.PeriodicityFactor * this.ProrataFactor;
			var amortization = this.BaseAmount * rate;

			value = value - amortization;
			value = this.Round (value);
			value = this.Residual (value);
			value = this.Override (value);
			//----------------------------------------------

			return value;
		}
	}
}
