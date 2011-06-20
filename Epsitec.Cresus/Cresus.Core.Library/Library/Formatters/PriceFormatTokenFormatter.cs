//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Resolvers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library.Formatters
{
	public class PriceFormatTokenFormatter : IFormatTokenFormatter
	{
		#region IFormatTokenFormatter Members

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
