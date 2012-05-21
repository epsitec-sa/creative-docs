//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ButtonShowCondition</c> enumeration defines when buttons will
	/// be shown in a <see cref="TextFieldEx"/> widget.
	/// </summary>
	public enum ButtonShowCondition
	{
		/// <summary>
		/// Always show the accept/reject buttons.
		/// </summary>
		Always,

		/// <summary>
		/// Show the accept/reject buttons when the widget has the focus.
		/// </summary>
		WhenFocused,
		
		/// <summary>
		/// Show the accept/reject buttons when the widget has the keyboard
		/// focus.
		/// </summary>
		WhenKeyboardFocused,

		/// <summary>
		/// Show the accept/reject buttons when the widget contains modified
		/// data.
		/// </summary>
		WhenModified,

		/// <summary>
		/// Never show the accept/reject buttons.
		/// </summary>
		Never
	}
}
