//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Resolvers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library.Formatters
{
	public class StringFormatTokenFormatter : IFormatTokenFormatter
	{
		#region IFormatTokenFormatter Members

		public FormatToken GetFormatToken()
		{
			return new ArgumentFormatToken ("#string", this.Format);
		}

		#endregion

		private string Format(FormatterHelper helper, string argument)
		{
			var value = helper.FormattingContext.Data;

			return string.Format (argument, value);
		}
	}
}
