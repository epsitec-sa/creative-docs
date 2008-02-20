//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>ItemViewShape</c> enumeration defines the shapes which can be
	/// used to represent the <see cref="ItemView"/> instances.
	/// </summary>
	public enum ItemViewShape
	{
		/// <summary>
		/// The <c>ItemView</c> should be displayed as a row.
		/// </summary>
		Row,

		/// <summary>
		/// The <c>ItemView</c> should be displayed as a tile.
		/// </summary>
		Tile,

		/// <summary>
		/// The elements of the <c>ItemView</c> are to be displayed in a
		/// tool-tip.
		/// </summary>
		ToolTip,
	}
}
