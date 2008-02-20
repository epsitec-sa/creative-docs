//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>TextFieldStyle</c> enumeration defines how a text field looks like
	/// and how it behaves.
	/// </summary>
	public enum TextFieldStyle
	{
		/// <summary>
		/// Normal text field. This is the standard, default, text line.
		/// </summary>
		Normal,

		/// <summary>
		/// Flat text field, without any frame nor margins.
		/// </summary>
		Flat,

		/// <summary>
		/// Multiline text field.
		/// </summary>
		Multiline,

		/// <summary>
		/// Combo box with a drop-down list.
		/// </summary>
		Combo,

		/// <summary>
		/// Text field with up/down buttons to increment/decrement a value.
		/// </summary>
		UpDown,

		/// <summary>
		/// Simple text field with a thin frame, just like a menu item. This
		/// is mainly used for text painted by <see cref="ScrollList"/>.
		/// </summary>
		Simple,
	}
}
