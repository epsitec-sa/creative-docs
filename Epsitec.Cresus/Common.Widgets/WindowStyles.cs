//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Summary description for WindowStyles.
	/// </summary>
	
	[System.Flags]
	
	public enum WindowStyles
	{
		None					= 0x00000000,
		
		CanResize				= 0x00000001,
		CanMinimize				= 0x00000002,
		CanMaximize				= 0x00000004,
		
		HasCloseButton			= 0x00000100,
		HasHelpButton			= 0x00000200,
	}
}
