//	Copyright © 2003-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ScrollListStyle</c> enum defines the various styles which can
	/// be applied to a <see cref="ScrollList"/>.
	/// </summary>
	public enum ScrollListStyle
	{
		/// <summary>
		/// Scroll list.
		/// </summary>
		Standard,

		/// <summary>
		/// Not really a scroll list per se; used to represent the content of
		/// a <see cref="TextFieldCombo"/>.
		/// </summary>
		Menu,

		/// <summary>
		/// Scroll list with alternating rows: white/gray/white/...
		/// </summary>
		AlternatingRows,

		/// <summary>
		/// Scroll list without frame and without background
		/// </summary>
		FrameLess,
	}
}
