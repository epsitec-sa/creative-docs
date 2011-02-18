//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>CurrentItemTrackingMode</c> enumeration defines how an
	/// <see cref="ItemPanel"/> behaves when the current item of the
	/// <see cref="Epsitec.Common.Types.ICollectionView"/> changes.
	/// </summary>
	public enum CurrentItemTrackingMode
	{
		/// <summary>
		/// Don't track the current item.
		/// </summary>
		None,

		/// <summary>
		/// Automatically synchronize the focus with the current item.
		/// </summary>
		AutoFocus,

		/// <summary>
		/// Automatically select the current item.
		/// </summary>
		AutoSelect,

		/// <summary>
		/// Automatically select the current item; when no current item is defined,
		/// automatically deselect all items.
		/// </summary>
		AutoSelectAndDeselect,
	}
}