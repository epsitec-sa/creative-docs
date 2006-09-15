//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'énumération MessageType définit les divers types de messages qui
	/// peuvent survenir dans une application.
	/// </summary>
	public enum MessageType
	{
		None,
		
		MouseEnter,
		MouseLeave,
		MouseMove,
		MouseHover,
		MouseDown,
		MouseUp,
		MouseWheel,
		
		KeyDown,
		KeyUp,
		KeyPress,

		ApplicationCommand
	}
}
