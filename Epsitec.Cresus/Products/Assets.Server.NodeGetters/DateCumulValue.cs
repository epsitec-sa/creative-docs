//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public class DateCumulValue : AbstractCumulValue
	{
		public DateCumulValue(System.DateTime? value)
			: base ()
		{
			this.value = value;
		}

		public System.DateTime?					Value
		{
			get
			{
				return this.value;
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
			var aa = a as DateCumulValue;

			if (this.IsExist && aa.IsExist)
			{
				if (this.Value == aa.Value)
				{
					return this;
				}
			}
			else if (this.IsExist)
			{
				return this;
			}
			else if (aa.IsExist)
			{
				return aa;
			}

			return new DateCumulValue (null);
		}


		private readonly System.DateTime?		value;
	}
}
