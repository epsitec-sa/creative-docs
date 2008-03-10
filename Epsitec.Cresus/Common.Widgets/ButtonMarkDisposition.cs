//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ButtonMarkDisposition</c> enumeration defines if and where a mark
	/// should be painted relative to the button box. The mark is usually a small
	/// triangle pointing to the outside of the box.
	/// </summary>
	public enum ButtonMarkDisposition
	{
		/// <summary>
		/// No mark.
		/// </summary>
		None,

		/// <summary>
		/// Mark on the left hand side of the button.
		/// </summary>
		Left,

		/// <summary>
		/// Mark on the right hand side of the button.
		/// </summary>
		Right,
		
		/// <summary>
		/// Mark above the button.
		/// </summary>
		Above,
		
		/// <summary>
		/// Mark below the button.
		/// </summary>
		Below,
	}
}
