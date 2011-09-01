//	Copyright © 2003-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Platform
{
	/// <summary>
	/// The <c>Win32Const</c> class exports the Window constants used by the
	/// framework.
	/// </summary>
	internal class Win32Const
	{
		public const int GCL_HICON   = -14;
		public const int GCL_HICONSM = -34;

		public const int WM_ACTIVATE		= 0x0006;
		public const int WM_QUERYENDSESSION	= 0x0011;
		public const int WM_ENDSESSION		= 0x0016;
		public const int WM_SHOWWINDOW		= 0x0018;
		public const int WM_ACTIVATEAPP		= 0x001C;
		public const int WM_MOUSEACTIVATE	= 0x0021;
		public const int WM_GETMINMAXINFO   = 0x0024;
		public const int WM_WINDOWPOSCHANGING = 0x0046;
		public const int WM_WINDOWPOSCHANGED = 0x0047;
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
		public const int WM_SIZING			= 0x0214;
		public const int WM_ENTERSIZEMOVE	= 0x0231;
		public const int WM_EXITSIZEMOVE	= 0x0232;
		public const int WM_MOUSELEAVE		= 0x02A3;
		public const int WM_APPCOMMAND		= 0x0319;
		public const int WM_NCCALCSIZE		= 0x0083;
		public const int WM_CHANGEUISTATE	= 0x0127;

		public const int APPCOMMAND_BROWSER_BACKWARD = 1;
		public const int APPCOMMAND_BROWSER_FORWARD = 2;
		
		public const int WM_APP				= 0x8000;
		public const int WM_APP_DISPOSE		= WM_APP + 1;
		public const int WM_APP_EXEC_CMD	= WM_APP + 2;
		public const int WM_APP_VALIDATION	= WM_APP + 3;
		public const int WM_APP_SYNCMDCACHE	= WM_APP + 4;
		public const int WM_APP_AWAKE		= WM_APP + 5;
		
		public const int VK_SHIFT			= 0x0010;
		public const int VK_CONTROL			= 0x0011;
		public const int VK_MENU			= 0x0012;
		public const int VK_SPACE			= 0x0020;
		
		public const int MK_LBUTTON			= 0x01;
		public const int MK_RBUTTON			= 0x02;
		public const int MK_MBUTTON			= 0x10;
		public const int MK_XBUTTON1		= 0x20;
		public const int MK_XBUTTON2		= 0x40;

		public const int GWL_STYLE			= -16;
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

		public const uint HT_CAPTION		= 2;
		public const uint HT_SYSMENU		= 3;
		public const uint HT_MINBUTTON		= 8;
		public const uint HT_MAXBUTTON		= 9;
		public const uint HT_LEFT			= 10;
		public const uint HT_RIGHT			= 11;
		public const uint HT_TOP			= 12;
		public const uint HT_TOPLEFT		= 13;
		public const uint HT_TOPRIGHT		= 14;
		public const uint HT_BOTTOM			= 15;
		public const uint HT_BOTTOMLEFT		= 16;
		public const uint HT_BOTTOMRIGHT	= 17;
		public const uint HT_CLOSE			= 20;
		
		public const uint SWP_NOSIZE = 0x0001;
		public const uint SWP_NOMOVE = 0x0002;
		public const uint SWP_NOZORDER = 0x0004;
		public const uint SWP_NOREDRAW = 0x0008;
		public const uint SWP_NOACTIVATE = 0x0010;
		public const uint SWP_FRAMECHANGED = 0x0020;  /* The frame changed: send WM_NCCALCSIZE */
		public const uint SWP_SHOWWINDOW = 0x0040;
		public const uint SWP_HIDEWINDOW = 0x0080;
		public const uint SWP_NOCOPYBITS = 0x0100;
		public const uint SWP_NOOWNERZORDER = 0x0200;  /* Don't do owner Z ordering */
		public const uint SWP_NOSENDCHANGING = 0x0400;  /* Don't send WM_WINDOWPOSCHANGING */

		public const int WPF_RESTORETOMAXIMIZED = 0x0002;
		
		public const int WMSZ_LEFT = 1;
		public const int WMSZ_RIGHT = 2;
		public const int WMSZ_TOP = 3;
		public const int WMSZ_TOPLEFT = 4;
		public const int WMSZ_TOPRIGHT = 5;
		public const int WMSZ_BOTTOM = 6;
		public const int WMSZ_BOTTOMLEFT = 7;
		public const int WMSZ_BOTTOMRIGHT = 8;

		public const int ENDSESSION_CLOSEAPP = 1;
		
		public const int ERROR_ALREADY_EXISTS = 183;
	}
}
