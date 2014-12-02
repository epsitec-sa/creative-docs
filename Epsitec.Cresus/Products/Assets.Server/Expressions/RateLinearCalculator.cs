//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	public class RateLinearCalculator : AbstractCalculator
	{
		public RateLinearCalculator(AmortizationDetails details)
			: base (details)
		{
		}

		public override decimal Evaluate()
		{
			decimal value = this.InitialAmount;

			//----------------------------------------------
			if (PeriodCount == 1 ||
				PeriodRank % PeriodCount != PeriodCount-1)
			{
				var rate = Rate * PeriodicityFactor * ProrataFactor;
				var amortization = BaseAmount * rate;

				value = value - amortization;
				value = Round (value);
				value = Residual (value);
			}
			else
			{
				//	If last Period -> adjust.
				var rate = Rate * ProrataFactor;
				var amortization = BaseAmount * rate;

				value = StartYearAmount - amortization;
				value = Round (value);
				value = Residual (value);
			}
			//----------------------------------------------

			return value;
		}
	}
}
