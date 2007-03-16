//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>CurrentItemTrackingMode</c> enumeration defines how an
	/// <see cref="ItemPanel"/> behaves when the current item of the
	/// <see cref="ICollectionView"/> changes.
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
	}
}