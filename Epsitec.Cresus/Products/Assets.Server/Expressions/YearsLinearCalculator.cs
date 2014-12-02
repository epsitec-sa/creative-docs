//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	public class YearsLinearCalculator : AbstractCalculator
	{
		public YearsLinearCalculator(AmortizationDetails details)
			: base (details)
		{
		}

		public override decimal Evaluate()
		{
			decimal value = this.InitialAmount;

			//----------------------------------------------
			var rate = 1.0m;
			decimal n = this.YearCount - this.YearRank;  // remaining years

			if (n > 0)
			{
				rate = 1.0m / n;
			}

			var amortization = this.InitialAmount * rate;
			value = value - amortization;
			value = this.Round (value);
			value = this.Residual (value);
			//----------------------------------------------

			return value;
		}
	}
}
