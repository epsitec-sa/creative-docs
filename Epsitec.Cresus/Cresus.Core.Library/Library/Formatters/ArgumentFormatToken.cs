//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library.Formatters
{
	/// <summary>
	/// The <c>ArgumentFormatToken</c> class implements handling of formatting tokens
	/// which take one or more arguments, simply enclosed by a pair of parentheses.
	/// </summary>
	public sealed class ArgumentFormatToken : FormatToken
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ArgumentFormatToken"/> class.
		/// </summary>
		/// <param name="format">The format string.</param>
		/// <param name="func">The formatting function, which takes an additional argument.</param>
		public ArgumentFormatToken(string format, System.Func<FormatterHelper, string, string> func)
			: base (format)
		{
			this.func = func;
		}

		/// <summary>
		/// Checks whether this format token matches the specified format string.
		/// </summary>
		/// <param name="formatter">The formatter.</param>
		/// <param name="format">The format string.</param>
		/// <param name="pos">The position in the format string.</param>
		/// <returns>
		///   <c>true</c> if the format token matches; otherwise, <c>false</c>.
		/// </returns>
		public override bool Matches(FormatterHelper formatter, string format, int pos)
		{
			//	Since we need the format and position for the formatting part of this task,
			//	we store them both into the formatter's FormatContext object...

			if (format.ContainsAtPosition (this.format, pos))
			{
				int start  = format.IndexOf ('(', pos) + 1;
				int length = format.IndexOf (')', start) - start;

				if ((start == pos + this.format.Length + 1) &&
					(length >= 0))
				{
					string arguments = format.Substring (start, length);
					formatter.FormattingContext.Args = arguments;
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Outputs the formatted data, as requested by the format string submitted to
		/// <see cref="Matches"/>.
		/// </summary>
		/// <param name="formatter">The formatter.</param>
		/// <param name="buffer">The buffer where to output the result.</param>
		/// <returns>
		/// The length to skip in the format string.
		/// </returns>
		public override int Format(FormatterHelper formatter, System.Text.StringBuilder buffer)
		{
			string args = formatter.FormattingContext.Args;
			int skipLength = this.format.Length + 1 + args.Length + 1;

			buffer.Append (this.func (formatter, args));

			return skipLength;
		}

		
		private readonly System.Func<FormatterHelper, string, string> func;
	}
}
