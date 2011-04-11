//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// The <c>HintEditorMode</c> enumeration defines how the hints will be showed in an
	/// editor such as <see cref="AutoCompleteTextField"/>.
	/// </summary>
	public enum HintEditorMode
	{
		/// <summary>
		/// The suggested values are replaced in line.
		/// </summary>
		InLine,

		/// <summary>
		/// A menu with the suggestions will be displayed.
		/// </summary>
		DisplayMenu,
		
		/// <summary>
		/// A menu with the suggestions will be displayed, but only for small lists. If there
		/// are too many items, the list won't be displayed.
		/// </summary>
		DisplayMenuForSmallList,
	}
}
