//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Formatters
{
	/// <summary>
	/// The <c>PriceFormatTokenFormatter</c> class implements the <c>#price()</c> formatting
	/// command, which outputs the numeric value as a monetary amount.
	/// </summary>
	public class PriceFormatTokenFormatter : IFormatTokenFormatter
	{
		#region IFormatTokenFormatter Members

		/// <summary>
		/// Gets the format token of this formatter.
		/// </summary>
		/// <returns>
		/// The <see cref="FormatToken"/> of this formatter.
		/// </returns>
		public FormatToken GetFormatToken()
		{
			return new ArgumentFormatToken ("#price", this.Format);
		}

		#endregion

		private string Format(FormatterHelper helper, string argument)
		{
			var value = helper.FormattingContext.Data;

			if (value == null)
			{
				return null;
			}
			else
			{
				decimal price;

				if (InvariantConverter.Convert (value, out price))
				{
					return Numeric.MonetaryRange.ConvertToString (price);
				}
				else
				{
					return null;
				}
			}
		}
	}
}
