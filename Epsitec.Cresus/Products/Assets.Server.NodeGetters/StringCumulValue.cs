//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public class StringCumulValue : AbstractCumulValue
	{
		public StringCumulValue(string value)
			: base ()
		{
			this.value = value;
		}

		public string							Value
		{
			get
			{
				return (string) this.value;
			}
		}

		public override bool					IsExist
		{
			get
			{
				return !string.IsNullOrEmpty (this.value);
			}
		}

		public override AbstractCumulValue Merge(AbstractCumulValue a)
		{
			var aa = a as StringCumulValue;

			if (this.IsExist && aa.IsExist)
			{
				if (this.Value == aa.Value)
				{
					return this;
				}
				else
				{
					//	La fusion de deux chaînes différentes affiche "...".
					return new StringCumulValue ("...");
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

			return new StringCumulValue (null);
		}


		private readonly string					value;
	}
}
