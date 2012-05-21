//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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

		Frameless				= 0x00001000,

		DefaultDocumentWindow	= CanResize|CanMinimize|CanMaximize|HasCloseButton
	}
}
