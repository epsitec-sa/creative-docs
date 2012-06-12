//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>ITextFormatterConverter</c> interface is implemented by the pretty pringing
	/// classes, which provide a textual representation for some simple type; this
	/// can be used to convert <c>enum</c> values to their descriptions.
	/// </summary>
	public interface ITextFormatterConverter
	{
		/// <summary>
		/// Gets the collection of all types which can be converted by this
		/// pretty printer.
		/// </summary>
		/// <returns>The collection of all convertible types.</returns>
		IEnumerable<System.Type> GetConvertibleTypes();

		/// <summary>
		/// Converts the value to <see cref="FormattedText"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="culture">The culture.</param>
		/// <param name="detailLevel">The detail level.</param>
		/// <returns>The formatted text if the value is not null; otherwise, <c>FormattedText.Empty</c>.</returns>
		FormattedText ToFormattedText(object value, System.Globalization.CultureInfo culture, TextFormatterDetailLevel detailLevel);
	}
}
