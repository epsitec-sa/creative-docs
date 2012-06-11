//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>SlimFieldDisplayMode</c> enumeration defines how a <see cref="SlimField"/>
	/// displays its contents.
	/// </summary>
	public enum SlimFieldDisplayMode
	{
		/// <summary>
		/// Display the label. This is used when the field is empty and its meaning cannot
		/// be inferred by the context.
		/// </summary>
		Label,

		/// <summary>
		/// Display the text value. A prefix and a suffix might be included with the text value
		/// to make the data more meaningful.
		/// </summary>
		Text,

		/// <summary>
		/// Display the menu. This is used to let the user select one of the values.
		/// </summary>
		Menu,

		/// <summary>
		/// Not a display mode: measure the width of the text value.
		/// </summary>
		MeasureTextOnly,

		/// <summary>
		/// Not a display mode: measure the width of the text prefix.
		/// </summary>
		MeasureTextPrefix,

		/// <summary>
		/// Not a display mode: measure the width of the text suffix.
		/// </summary>
		MeasureTextSuffix,
	}
}
