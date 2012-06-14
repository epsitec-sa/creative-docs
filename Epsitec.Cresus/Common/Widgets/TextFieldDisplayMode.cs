//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

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

		/// <summary>
		/// Displays a hint instead of the text typed in by the user. The real
		/// text must be part of the hint and will be made somehow more visible.
		/// </summary>
		ActiveHint,

		/// <summary>
		/// Displays a hint instead of the text typed in by the user. Visually,
		/// there will be no difference between the standard text and the hint.
		/// </summary>
		PassiveHint,

		Transparent,
	}
}
