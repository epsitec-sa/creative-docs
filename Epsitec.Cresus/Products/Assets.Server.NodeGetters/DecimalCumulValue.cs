//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public class DecimalCumulValue : AbstractCumulValue
	{
		public DecimalCumulValue(decimal? value)
			: base ()
		{
			this.value = value;
		}

		public decimal?							Value
		{
			get
			{
				return (decimal?) this.value;
			}
		}

		public override bool					IsExist
		{
			get
			{
				return this.value.HasValue;
			}
		}

		public override AbstractCumulValue Merge(AbstractCumulValue a)
		{
			var aa = a as DecimalCumulValue;

			if (this.IsExist && aa.IsExist)
			{
				return new DecimalCumulValue (this.Value + aa.Value);
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


		private readonly decimal?				value;
	}
}
