//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class PriceRoundingModeEntity : System.IComparable<PriceRoundingModeEntity>
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
			object before = this.AddBeforeModulo == 0 ? null : TextFormatter.FormatText ("+", this.AddBeforeModulo, TextFormatter.FormatCommand ("#price()"), " / ");
			return TextFormatter.FormatText (this.Name, "~,", before, this.Modulo, TextFormatter.FormatCommand ("#price()"));
		}

		public override EntityStatus GetEntityStatus()
		{
			return EntityStatus.Valid;
		}

		#region IComparable<PriceRoundingModeEntity> Members

		int System.IComparable<PriceRoundingModeEntity>.CompareTo(PriceRoundingModeEntity that)
		{
			if (that == null)
			{
				return 1;
			}

			if (this.Modulo < that.Modulo)
			{
				return -1;
			}
			else if (this.Modulo > that.Modulo)
			{
				return 1;
			}

			if (this.AddBeforeModulo < that.AddBeforeModulo)
			{
				return -1;
			}
			else if (this.AddBeforeModulo > that.AddBeforeModulo)
			{
				return 1;
			}
			
			int thisPolicy = (int) this.RoundingPolicy;
			int thatPolicy = (int) that.RoundingPolicy;

			return thisPolicy.CompareTo (thatPolicy);
		}

		#endregion
	}
}
