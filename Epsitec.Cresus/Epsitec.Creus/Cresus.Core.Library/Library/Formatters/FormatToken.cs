//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library.Formatters
{
	/// <summary>
	/// The <c>FormatToken</c> class is the base class for <see cref="SimpleFormatToken"/> and
	/// <see cref="ArgumentFormatToken"/>, which implement the formatting details.
	/// </summary>
	public abstract class FormatToken
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FormatToken"/> class for
		/// the specified format string.
		/// </summary>
		/// <param name="format">The format.</param>
		protected FormatToken(string format)
		{
			this.format = format;
		}

		/// <summary>
		/// Gets all known <see cref="FormatToken"/> instances. This is thread-safe.
		/// </summary>
		public static IEnumerable<FormatToken> Items
		{
			get
			{
				return FormatToken.items;
			}
		}

		/// <summary>
		/// Checks whether this format token matches the specified format string.
		/// </summary>
		/// <param name="formatter">The formatter.</param>
		/// <param name="format">The format string.</param>
		/// <param name="pos">The position in the format string.</param>
		/// <returns><c>true</c> if the format token matches; otherwise, <c>false</c>.</returns>
		public abstract bool Matches(FormatterHelper formatter, string format, int pos);

		/// <summary>
		/// Outputs the formatted data, as requested by the format string submitted to
		/// <see cref="Matches"/>.
		/// </summary>
		/// <param name="formatter">The formatter.</param>
		/// <param name="buffer">The buffer where to output the result.</param>
		/// <returns>The length to skip in the format string.</returns>
		public abstract int Format(FormatterHelper formatter, System.Text.StringBuilder buffer);
		
		
		
		static FormatToken()
		{
			//	Tokens will be sorted from longest to shortest ("yyyy", then "yy") so that
			//	they can be evaluated one after the other in order to find the proper match:

			FormatToken.items = new List<FormatToken> (FormatterHelper.GetTokens ().OrderByDescending (x => x.format.Length));
		}
		
		
		private readonly static List<FormatToken> items;
		
		protected readonly string				format;
	}
}