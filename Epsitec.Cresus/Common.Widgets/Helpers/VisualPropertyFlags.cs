//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// VisualPropertyFlags.
	/// </summary>
	
	[System.Flags]
	
	public enum VisualPropertyFlags
	{
		None					= 0,
		
		AffectsLayout			= 0x0001,
		AffectsParentLayout		= 0x0002,
	}
}
