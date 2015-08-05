//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public class DecimalCumulValue : AbstractCumulValue
	{
		public DecimalCumulValue(decimal? value)
			: base (value)
		{
		}

		public override bool IsExist
		{
			get
			{
				return this.TypedValue.HasValue;
			}
		}

		public override AbstractCumulValue Merge(AbstractCumulValue a)
		{
			var aa = a as DecimalCumulValue;

			if (this.IsExist && aa.IsExist)
			{
				return new DecimalCumulValue (this.TypedValue + aa.TypedValue);
			}
			else if (this.IsExist)
			{
				return this;
			}
			else if (aa.IsExist)
			{
				return aa;
			}
			else
			{
				return new DecimalCumulValue (null);
			}
		}


		private decimal? TypedValue
		{
			get
			{
				return (decimal?) this.value;
			}
		}
	}
}
