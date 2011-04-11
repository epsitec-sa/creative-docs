//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// The <c>ValueToDescriptionConverter</c> delegate is used to convert a value to
	/// a human readable representation.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The description of the specified value.</returns>
	public delegate FormattedText ValueToDescriptionConverter(object value);
}
