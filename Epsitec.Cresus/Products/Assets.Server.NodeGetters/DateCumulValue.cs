//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public class DateCumulValue : AbstractCumulValue
	{
		public DateCumulValue(System.DateTime? value)
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
			var aa = a as DateCumulValue;

			if (this.IsExist && aa.IsExist)
			{
				if (this.Value == aa.Value)
				{
					return new DateCumulValue (this.TypedValue);
				}
			}
			else if (this.IsExist)
			{
				return new DateCumulValue (this.TypedValue);
			}
			else if (aa.IsExist)
			{
				return new DateCumulValue (aa.TypedValue);
			}

			return new DateCumulValue (null);
		}


		private System.DateTime? TypedValue
		{
			get
			{
				return (System.DateTime?) this.value;
			}
		}
	}
}
