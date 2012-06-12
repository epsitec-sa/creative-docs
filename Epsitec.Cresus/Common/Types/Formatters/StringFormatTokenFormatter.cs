//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Formatters
{
	/// <summary>
	/// The <c>StringFormatTokenFormatter</c> class implements the <c>#string(format)</c>
	/// formatting command, which has the same syntax as the <see cref="string.Format"/>
	/// format string.
	/// </summary>
	public class StringFormatTokenFormatter : IFormatTokenFormatter
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
			return new ArgumentFormatToken ("#string", this.Format);
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
				return string.Format (argument, value);
			}
		}
	}
}
