//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class PriceRoundingModeEntity
	{
		public decimal Round(decimal value)
		{
			decimal sign;

			if (value < 0)
			{
				value = -value;
				sign  = -1M;
			}
			else
			{
				sign = 1M;
			}
			
			value = value + this.AddBeforeModulo;
			value = System.Math.Floor (value / this.Modulo) * this.Modulo;

			return sign * value;
		}

		public override FormattedText GetSummary()
		{
			return this.GetCompactSummary ();
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name, "~, ", "(x", "+", this.AddBeforeModulo, ")", "modulo", this.Modulo);
		}

		public override EntityStatus GetEntityStatus()
		{
			return EntityStatus.Valid;
		}
	}
}
