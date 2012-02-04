//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Compta.Widgets
{
	/// <summary>
	/// The <c>ValueToFormattedTextConverter</c> delegate is used to convert a value to
	/// a human readable representation.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The description of the specified value.</returns>
	public delegate FormattedText ValueToFormattedTextConverter(object value);

	/// <summary>
	/// The <c>ValueToFormattedTextConverter&lt;T&gt;</c> delegate is used to convert a value to
	/// a human readable representation.
	/// </summary>
	/// <typeparam name="T">The type of the expected value.</typeparam>
	/// <param name="value">The value.</param>
	/// <returns>
	/// The description of the specified value.
	/// </returns>
	public delegate FormattedText ValueToFormattedTextConverter<T>(T value);
}
