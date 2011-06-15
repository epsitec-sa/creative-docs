//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library.Formatters
{
	/// <summary>
	/// The <c>SimpleFormatToken</c> class handles simple formatting directives, such
	/// as "yy" to produce the short year (11) or "yyyy" to produce the long year (2011)
	/// representation.
	/// </summary>
	public sealed class SimpleFormatToken : FormatToken
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SimpleFormatToken"/> class.
		/// </summary>
		/// <param name="format">The format string.</param>
		/// <param name="func">The formatting function.</param>
		public SimpleFormatToken(string format, System.Func<FormatterHelper, string> func)
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
			return format.ContainsAtPosition (this.format, pos);
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
			buffer.Append (this.func (formatter));
			
			return this.format.Length;
		}
		
		private readonly System.Func<FormatterHelper, string> func;
	}
}
