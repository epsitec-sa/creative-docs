//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ContainerLayoutMode</c> enumeration defines in what direction
	/// widgets get laid out.
	/// </summary>
	public enum ContainerLayoutMode
	{
		/// <summary>
		/// No layout direction preference.
		/// </summary>
		None=0,							//	pas de préférence					
		
		/// <summary>
		/// Horizontal direction (side by side).
		/// </summary>
		HorizontalFlow=1,				//	remplit horizontalement
		
		/// <summary>
		/// Vertical direction (one above the other).
		/// </summary>
		VerticalFlow=2					//	remplit verticalement
	}
}
