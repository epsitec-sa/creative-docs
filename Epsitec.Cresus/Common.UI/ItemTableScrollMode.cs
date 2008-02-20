//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>ItemTableScrollMode</c> enumeration defines how an <see
	/// cref="ItemTable"/> scrolls its contents.
	/// </summary>
	public enum ItemTableScrollMode
	{
		/// <summary>
		/// Don't display a scroller and don't scroll.
		/// </summary>
		None,
		
		/// <summary>
		/// Scroll in a linear way; the scroller defines the position in the
		/// surface displayed by the <see cref="ItemTable"/>.
		/// </summary>
		Linear,
		
		/// <summary>
		/// Scroll item by item; the scroller defines the first visible item
		/// in the <see cref="ItemTable"/>.
		/// </summary>
		ItemBased
	}
}