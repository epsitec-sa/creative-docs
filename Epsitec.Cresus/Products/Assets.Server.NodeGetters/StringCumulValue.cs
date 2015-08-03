﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public class StringCumulValue : AbstractCumulValue
	{
		public StringCumulValue(string value)
			: base (value)
		{
		}

		public override bool IsExist
		{
			get
			{
				return !string.IsNullOrEmpty (this.TypedValue);
			}
		}

		public override AbstractCumulValue Merge(AbstractCumulValue a)
		{
			var aa = a as StringCumulValue;

			if (this.IsExist && aa.IsExist)
			{
				if (this.Value == aa.Value)
				{
					return new StringCumulValue (this.TypedValue);
				}
			}
			else if (this.IsExist)
			{
				return new StringCumulValue (this.TypedValue);
			}
			else if (aa.IsExist)
			{
				return new StringCumulValue (aa.TypedValue);
			}

			return new StringCumulValue (null);
		}


		private string TypedValue
		{
			get
			{
				return (string) this.value;
			}
		}
	}
}
