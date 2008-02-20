//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>HintListVisibilityMode</c> enumeration specifies how and when the
	/// hint list will be displayed.
	/// </summary>
	public enum HintListVisibilityMode
	{
		/// <summary>
		/// The hint list is always visible.
		/// </summary>
		Visible,

		/// <summary>
		/// The hint list is always invisible.
		/// </summary>
		Invisible,

		/// <summary>
		/// The hint list pops up when it is required and it disappears as soon
		/// as it is no longer needed.
		/// </summary>
		AutoHide
	}
}
