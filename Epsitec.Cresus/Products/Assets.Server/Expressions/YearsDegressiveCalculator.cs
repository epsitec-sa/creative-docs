//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	public class YearsDegressiveCalculator : AbstractCalculator
	{
		public YearsDegressiveCalculator(AmortizationDetails details)
			: base (details)
		{
		}

		public override decimal Evaluate()
		{
			decimal value = this.InitialAmount;

			//----------------------------------------------
			var rate = 1.0m;
			decimal n = this.YearCount - this.YearRank;  // remaining years

			if (n > 0 && this.ResidualAmount != 0 && this.InitialAmount != 0)
			{
				var x = this.ResidualAmount / this.InitialAmount;
				var y = 1.0m / n;
				rate = 1.0m - (decimal) System.Math.Pow ((double) x, (double) y);
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
