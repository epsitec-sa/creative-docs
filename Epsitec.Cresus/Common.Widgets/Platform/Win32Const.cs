namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Win32Const exporte des constantes liées à Windows.
	/// </summary>
	internal class Win32Const
	{
		public const int WM_ACTIVATE		= 0x0006;
		public const int WM_ACTIVATEAPP		= 0x001C;
		public const int WM_MOUSEACTIVATE	= 0x0021;
		public const int WM_NCACTIVATE		= 0x0086;
		public const int WM_NCLBUTTONDOWN	= 0x00A1;
		public const int WM_NCRBUTTONDOWN	= 0x00A4;
		public const int WM_NCMBUTTONDOWN	= 0x00A7;
		public const int WM_NCXBUTTONDOWN	= 0x00AB;
		public const int WM_KEYDOWN			= 0x0100;
		public const int WM_SYSKEYDOWN		= 0x0104;
		public const int WM_KEYUP			= 0x0101;
		public const int WM_SYSKEYUP		= 0x0105;
		public const int WM_CHAR			= 0x0102;
		public const int WM_SYSCHAR			= 0x0106;
		public const int WM_DEADCHAR		= 0x0103;
		public const int WM_SYSDEADCHAR		= 0x0107;
		public const int WM_MOUSEMOVE		= 0x0200;
		public const int WM_LBUTTONDOWN		= 0x0201;
		public const int WM_LBUTTONUP		= 0x0202;
		public const int WM_LBUTTONDBLCLK	= 0x0203;
		public const int WM_RBUTTONDOWN		= 0x0204;
		public const int WM_RBUTTONUP		= 0x0205;
		public const int WM_RBUTTONDBLCLK	= 0x0206;
		public const int WM_MBUTTONDOWN		= 0x0207;
		public const int WM_MBUTTONUP		= 0x0208;
		public const int WM_MBUTTONDBLCLK	= 0x0209;
		public const int WM_MOUSEWHEEL		= 0x020A;
		public const int WM_XBUTTONDOWN		= 0x020B;
		public const int WM_XBUTTONUP		= 0x020C;
		public const int WM_XBUTTONDBLCLK	= 0x020D;
		public const int WM_MOUSELEAVE		= 0x02A3;
		public const int WM_NCCALCSIZE		= 0x0083;
		public const int WM_CHANGEUISTATE	= 0x0127;
		
		public const int WM_APP				= 0x8000;
		public const int WM_APP_DISPOSE		= WM_APP + 1;
		public const int WM_APP_EXEC_CMD	= WM_APP + 2;
		public const int WM_APP_VALIDATION	= WM_APP + 3;
		
		public const int VK_SHIFT			= 0x0010;
		public const int VK_CONTROL			= 0x0011;
		public const int VK_MENU			= 0x0012;
		public const int VK_SPACE			= 0x0020;
		
		public const int MK_LBUTTON			= 0x01;
		public const int MK_RBUTTON			= 0x02;
		public const int MK_MBUTTON			= 0x10;
		public const int MK_XBUTTON1		= 0x20;
		public const int MK_XBUTTON2		= 0x40;

		public const int GWL_EXSTYLE		= -20;
		
		public const int GCL_STYLE			= -26;
		
		public const int CS_VREDRAW			= 0x1;		
		public const int CS_HREDRAW			= 0x2;		
		
		public const int WS_EX_LAYERED		= 0x080000;
		
		public const int ULW_COLORKEY		= 0x00000001;
		public const int ULW_ALPHA			= 0x00000002;
		public const int ULW_OPAQUE			= 0x00000004;
		
		public const byte AC_SRC_OVER		= 0x00;
		public const byte AC_SRC_ALPHA		= 0x01;
		
		public const int MA_ACTIVATE		= 1;
		public const int MA_ACTIVATEANDEAT	= 2;
		public const int MA_NOACTIVATE		= 3;
		public const int MA_NOACTIVATEANDEAT= 4;
		
		public const int GW_HWNDFIRST		= 0;
		public const int GW_HWNDLAST		= 1;
		public const int GW_HWNDNEXT		= 2;
		public const int GW_HWNDPREV		= 3;
		
		public const int SW_HIDE             = 0;
		public const int SW_SHOWNORMAL       = 1;
		public const int SW_NORMAL           = 1;
		public const int SW_SHOWMINIMIZED    = 2;
		public const int SW_SHOWMAXIMIZED    = 3;
		public const int SW_MAXIMIZE         = 3;
		public const int SW_SHOWNOACTIVATE   = 4;
		public const int SW_SHOW             = 5;
		public const int SW_MINIMIZE         = 6;
		public const int SW_SHOWMINNOACTIVE  = 7;
		public const int SW_SHOWNA           = 8;
		public const int SW_RESTORE          = 9;
		public const int SW_SHOWDEFAULT      = 10;
		public const int SW_FORCEMINIMIZE    = 11;
		
		public const int ROP_SRC_COPY		= 0xCC0020;
		public const int BLT_COLOR_ON_COLOR	= 0x03;
	}
}
