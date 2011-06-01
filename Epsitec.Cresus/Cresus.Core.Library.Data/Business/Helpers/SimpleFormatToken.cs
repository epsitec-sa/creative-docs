//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Helpers
{
	internal class SimpleFormatToken : FormatToken
	{
		public SimpleFormatToken(string format, System.Func<FormatterHelper, string> func)
			: base (format)
		{
			this.func   = func;
		}
		
		
		public override bool Matches(FormatterHelper formatter, string format, int pos)
		{
			//	If we need the format/pos for the formatting part of this task,
			//	we can store it into the formatter's FormatContext object...

			return format.ContainsAtPosition (this.format, pos);
		}

		public override int Format(FormatterHelper formatter, System.Text.StringBuilder buffer)
		{
			buffer.Append (this.func (formatter));
			return this.format.Length;
		}
		
		private readonly System.Func<FormatterHelper, string> func;
	}
}
