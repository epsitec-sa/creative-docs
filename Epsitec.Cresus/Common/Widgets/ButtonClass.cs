//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ButtonClass</c> enumeration specifies how a command button
	/// should be rendered, based on its class.
	/// </summary>
	[Types.DesignerVisible]
	public enum ButtonClass
	{
		/// <summary>
		/// No associated button class.
		/// </summary>
		[Types.Hidden]
		None,

		/// <summary>
		/// Plain text button for a dialog, used to implement "OK" and
		/// "Cancel" buttons, for instance.
		/// </summary>
		DialogButton,

		/// <summary>
		/// Rich text button for a dialog, used to implement standard
		/// buttons, including an optional icon.
		/// </summary>
		RichDialogButton,

		/// <summary>
		/// Flat button for use in the ribbon, in tool palettes, etc.
		/// The content (icon and/or text) is managed by the button itself.
		/// </summary>
		FlatButton,
	}
}