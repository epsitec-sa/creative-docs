//	Copyright © 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ContainerLayoutMode</c> enumeration defines in what direction
	/// widgets get laid out.
	/// </summary>
	[DesignerVisible]
	public enum ContainerLayoutMode : byte
	{
		/// <summary>
		/// No layout direction preference.
		/// </summary>
		[Hidden]
		None=0,
		
		/// <summary>
		/// Horizontal direction (side by side).
		/// </summary>
		HorizontalFlow=1,
		
		/// <summary>
		/// Vertical direction (one above the other).
		/// </summary>
		VerticalFlow=2,
	}
}
