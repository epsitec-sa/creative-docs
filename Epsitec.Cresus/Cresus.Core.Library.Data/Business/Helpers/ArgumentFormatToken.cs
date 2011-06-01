//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Helpers
{
	internal class ArgumentFormatToken : FormatToken
	{
		public ArgumentFormatToken(string format, System.Func<FormatterHelper, string, string> func)
			: base (format)
		{
			this.func = func;
		}

		public override bool Matches(FormatterHelper formatter, string format, int pos)
		{
			//	Since we need the format and pos for the formatting part of this task,
			//	we store it into the formatter's FormatContext object...

			if (format.ContainsAtPosition (this.format, pos))
			{
				int start  = format.IndexOf ('(', pos) + 1;
				int length = format.IndexOf (')', start) - start;

				if ((start == pos + this.format.Length + 1) &&
					(length >= 0))
				{
					formatter.FormatContext.Args = format.Substring (start, length);
					return true;
				}
			}
			
			return false;
		}

		public override int Format(FormatterHelper formatter, System.Text.StringBuilder buffer)
		{
			string args = formatter.FormatContext.Args;

			buffer.Append (this.func (formatter, args));
			
			return this.format.Length + 1 + args.Length + 1;
		}

		private readonly System.Func<FormatterHelper, string, string> func;
	}
}
