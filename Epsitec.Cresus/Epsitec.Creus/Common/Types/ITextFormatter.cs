//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>ITextFormatter</c> interface can be used to prepare a human readable
	/// representation of the instance.
	/// </summary>
	public interface ITextFormatter
	{
		FormattedText ToFormattedText(System.Globalization.CultureInfo culture, TextFormatterDetailLevel detailLevel);
	}
}
