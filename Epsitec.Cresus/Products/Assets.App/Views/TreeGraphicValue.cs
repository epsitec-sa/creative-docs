//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.Helpers;

namespace Epsitec.Cresus.Assets.App.Views
{
	public struct TreeGraphicValue
	{
		public TreeGraphicValue(string text, decimal? amount)
		{
			this.Text   = text;
			this.Amount = amount;
		}


		public bool IsEmpty
		{
			get
			{
				return this.Text == null
					&& !this.Amount.HasValue;
			}
		}

		public bool IsAmount
		{
			get
			{
				return this.Amount.HasValue;
			}
		}


		public static TreeGraphicValue Empty = new TreeGraphicValue (null, null);


		public static TreeGraphicValue CreateText(string text)
		{
			return new TreeGraphicValue (text, null);
		}

		public static TreeGraphicValue CreateAmount(decimal? amount)
		{
			var text = TypeConverters.AmountToString (amount);
			return new TreeGraphicValue (text, amount);
		}


		public readonly string					Text;
		public readonly decimal?				Amount;
	}
}
