//	Copyright © 2005-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>TextFieldDisplayMode</c> enumeration defines how text from
	/// text fields should be displayed.
	/// </summary>
	public enum TextFieldDisplayMode
	{
		/// <summary>
		/// Displays the text in its default representation.
		/// </summary>
		Default,
		
		/// <summary>
		/// Displays the text with a blue background to show that it was typed
		/// in by the user and that the value overrides a default value.
		/// </summary>
		OverriddenValue,
		
		/// <summary>
		/// Displays an italic text with an orange background to show that this
		/// is an inherited value (e.g. defined in a style).
		/// </summary>
		InheritedValue,
	}
}
