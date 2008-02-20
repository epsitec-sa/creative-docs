//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>ItemPanelLayout</c> enumeration lists all supported layout modes
	/// for the items represented by an <see cref="ItemPanel"/>.
	/// </summary>
	public enum ItemPanelLayout : byte
	{
		None,

		/// <summary>
		/// The <c>VerticalList</c> represents the items in a similar way to the
		/// detailed view of Microsoft Windows Explorer.
		/// </summary>
		VerticalList,

		/// <summary>
		/// The <c>RowOfTiles</c> represents each item with a rectangular tile;
		/// the tiles are arranged in rows.
		/// </summary>
		RowsOfTiles,

		/// <summary>
		/// The <c>ColumnOfTiles</c> represents each item with a rectangluar tile;
		/// the tiles are arranges in columns.
		/// </summary>
		ColumnsOfTiles,
	}
}
