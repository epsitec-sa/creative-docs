//	Copyright © 2003-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ScrollableScrollerMode</c> enumeration defines how and when the
	/// <see cref="Scrollable"/> displays the scrollers.
	/// </summary>
	public enum ScrollableScrollerMode
	{
		/// <summary>
		/// Automatic hide/show. The scroller will be visible if and only if it is
		/// needed.
		/// </summary>
		Auto,

		/// <summary>
		/// Hide the scroller, always.
		/// </summary>
		HideAlways,
		
		/// <summary>
		/// Show the scroller, always.
		/// </summary>
		ShowAlways,

		/// <summary>
		/// Show the scroller, always. The scroller will be placed on the opposite
		/// side of where it would be laid out by default.
		/// </summary>
		ShowAlwaysOppositeSide,
	}
}
