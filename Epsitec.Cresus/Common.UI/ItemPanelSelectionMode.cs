//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>ItemPanelSelectionMode</c> enumeration lists all supported selection
	/// modes for the items represented by an <see cref="ItemPanel"/>.
	/// </summary>
	public enum ItemPanelSelectionMode : byte
	{
		None,

		ZeroOrOne,
		ExactlyOne,

		Multiple,
	}
}
