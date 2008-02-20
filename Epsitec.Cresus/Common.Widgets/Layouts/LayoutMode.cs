//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Layouts
{
	/// <summary>
	/// The <c>LayoutMode</c> enumeration identifies well known layout modes
	/// which can be easily mapped to layout engines.
	/// </summary>
	public enum LayoutMode
	{
		/// <summary>
		/// No automatic layout, or unknown layout engine.
		/// </summary>
		None,
		
		/// <summary>
		/// Docked layout.
		/// </summary>
		Docked,
		
		/// <summary>
		/// Anchored layout.
		/// </summary>
		Anchored,
		
		/// <summary>
		/// Stacked layout.
		/// </summary>
		Stacked,
		
		/// <summary>
		/// Grid layout.
		/// </summary>
		Grid,
	}
}
