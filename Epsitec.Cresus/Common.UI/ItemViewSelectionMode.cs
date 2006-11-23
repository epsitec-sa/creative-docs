//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>ItemViewSelection</c> enumeration lists all supported selection
	/// modes for the <see cref="ItemView"/> instances in an <see cref="ItemPanel"/>.
	/// </summary>
	public enum ItemViewSelectionMode : byte
	{
		None,

		ZeroOrOne,
		ExactlyOne,

		Multiple,
	}
}
