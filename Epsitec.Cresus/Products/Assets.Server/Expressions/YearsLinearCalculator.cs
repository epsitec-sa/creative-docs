//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	public class YearsLinearCalculator : AbstractCalculator
	{
		public YearsLinearCalculator(AmortizedAmount amount)
			: base (amount)
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
			value = this.Override (value);
			//----------------------------------------------

			return value;
		}
	}
}
