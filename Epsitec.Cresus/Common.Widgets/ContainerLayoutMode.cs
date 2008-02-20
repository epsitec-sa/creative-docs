//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
