//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Runtime.InteropServices;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Win32Api exporte quelques fonctions de l'API Win32 utilisées
	/// par des couches très bas niveau.
	/// </summary>
	internal class Win32Api
	{
		[DllImport ("User32.dll")]	internal extern static int SetWindowLong(System.IntPtr handle, int index, int value);
		[DllImport ("User32.dll")]	internal extern static int GetWindowLong(System.IntPtr handle, int index);
		[DllImport ("User32.dll")]	internal extern static int SetClassLong(System.IntPtr handle, int index, int value);
		[DllImport ("User32.dll")]	internal extern static int GetClassLong(System.IntPtr handle, int index);
		[DllImport ("User32.dll", SetLastError=true)]	internal extern static bool UpdateLayeredWindow(System.IntPtr handle, System.IntPtr dst_dc, ref Win32Api.Point dst, ref Win32Api.Size size, System.IntPtr src_dc, ref Win32Api.Point src, int color, ref Win32Api.BlendFunction blend, int flags);
		[DllImport ("User32.dll")]	internal extern static System.IntPtr GetDC(System.IntPtr handle);
		[DllImport ("User32.dll")]	internal extern static int ReleaseDC(System.IntPtr handle, System.IntPtr dc);
		[DllImport ("User32.dll")]	internal extern static bool PostMessage(System.IntPtr handle, int msg, System.IntPtr w_param, System.IntPtr l_param);
		[DllImport ("User32.dll")]  internal extern static int GetWindowThreadProcessId(System.IntPtr handle, out int pid);		
		[DllImport ("User32.dll")]	internal extern static System.IntPtr GetWindow(System.IntPtr handle, int direction);
		[DllImport ("User32.dll")]	internal extern static bool IsWindowVisible(System.IntPtr handle);
		[DllImport ("User32.dll")]	internal extern static bool BringWindowToTop(System.IntPtr handle);
		[DllImport ("User32.dll")]	internal extern static bool GetIconInfo(System.IntPtr handle, out IconInfo info);
		[DllImport ("User32.dll")]	internal extern static System.IntPtr CreateIconIndirect(ref IconInfo info);
		[DllImport ("User32.dll")]	internal extern static bool DestroyIcon(System.IntPtr handle);
		[DllImport ("User32.dll")]	internal extern static bool ShowWindow(System.IntPtr handle, int cmd_show);
		
		[DllImport ("GDI32.dll")]	internal extern static System.IntPtr CreateCompatibleDC(System.IntPtr dc);
		[DllImport ("GDI32.dll")]	internal extern static System.IntPtr SelectObject(System.IntPtr dc, System.IntPtr handle_object);
		[DllImport ("GDI32.dll")]	internal extern static System.IntPtr DeleteObject(System.IntPtr handle_object);
		[DllImport ("GDI32.dll")]	internal extern static bool DeleteDC(System.IntPtr dc);
		[DllImport ("GDI32.dll")]	internal extern static void SetStretchBltMode(System.IntPtr dc, int mode);
		[DllImport ("GDI32.dll")]	internal extern static void StretchBlt(System.IntPtr dc, int x, int y, int dx, int dy, System.IntPtr src_dc, int src_x, int src_y, int src_dx, int src_dy, int rop);
		
		[StructLayout(LayoutKind.Sequential, Pack=1)] public struct Point
		{
			public int x;
			public int y;
		}
		
		[StructLayout(LayoutKind.Sequential, Pack=1)] public struct Size
		{
			public int cx;
			public int cy;
		}
		
		[StructLayout(LayoutKind.Sequential, Pack=1)] public struct BlendFunction
		{
			public byte BlendOp;
			public byte BlendFlags;
			public byte SourceConstantAlpha;
			public byte AlphaFormat;
		}
		
		[StructLayout(LayoutKind.Sequential, Pack=1)] public struct IconInfo
		{
			public byte	FlagIcon;
			public byte r1,r2,r3;
			public int HotspotX;
			public int HotspotY;
			public System.IntPtr BitmapMask;
			public System.IntPtr BitmapColor;
		}
		
		
		internal static int GetWindowStyle(System.IntPtr handle)
		{
			return Win32Api.GetWindowLong (handle, Win32Const.GWL_STYLE);
		}
		
		internal static int SetWindowStyle(System.IntPtr handle, int style)
		{
			return Win32Api.SetWindowLong (handle, Win32Const.GWL_STYLE, style);
		}
		
		internal static int GetWindowExStyle(System.IntPtr handle)
		{
			return Win32Api.GetWindowLong (handle, Win32Const.GWL_EXSTYLE);
		}
		
		internal static int SetWindowExStyle(System.IntPtr handle, int ex_style)
		{
			return Win32Api.SetWindowLong (handle, Win32Const.GWL_EXSTYLE, ex_style);
		}
		
		internal static bool UpdateLayeredWindow(System.IntPtr handle, System.Drawing.Bitmap bitmap, System.Drawing.Rectangle bounds, double alpha)
		{
			Win32Api.Point src_point = new Win32Api.Point ();
			Win32Api.Point dst_point = new Win32Api.Point ();
			Win32Api.Size  new_size  = new Win32Api.Size ();
			
			Win32Api.BlendFunction blend_function = new Win32Api.BlendFunction ();
			
			//	Get the screen DC, then create a memory based DC compatible with the screen DC.
			
			System.IntPtr screen_dc = Win32Api.GetDC (System.IntPtr.Zero);
			System.IntPtr memory_dc = Win32Api.CreateCompatibleDC (screen_dc);
			
			//	Get access to the bitmap handle contained in the Bitmap object, then select it.
			
			System.IntPtr memory_bitmap = bitmap.GetHbitmap (System.Drawing.Color.FromArgb (0));
			System.IntPtr old_bitmap    = Win32Api.SelectObject (memory_dc, memory_bitmap);
			
			new_size.cx = bounds.Width;
			new_size.cy = bounds.Height;
			dst_point.x = bounds.Left;
			dst_point.y = bounds.Top;
			src_point.x = 0;
			src_point.y = 0;
			
			blend_function.BlendOp				= Win32Const.AC_SRC_OVER;
			blend_function.BlendFlags			= 0;
			blend_function.SourceConstantAlpha	= (byte) System.Math.Min (255, System.Math.Max (0, (int) (alpha * 255.9)));
			blend_function.AlphaFormat			= Win32Const.AC_SRC_ALPHA;
			
			int flags = Win32Const.ULW_ALPHA;
			bool res = false;
			
			res = Win32Api.UpdateLayeredWindow (handle,
				/**/							screen_dc,
				/**/							ref dst_point,
				/**/							ref new_size,
				/**/							memory_dc,
				/**/							ref src_point,
				/**/							0,
				/**/							ref blend_function,
				/**/							flags);
			
			if (res == false)
			{
				System.Diagnostics.Debug.WriteLine ("LastError = " + Marshal.GetLastWin32Error ().ToString ());
			}
			
			Win32Api.SelectObject (memory_dc, old_bitmap);
			Win32Api.ReleaseDC (System.IntPtr.Zero, screen_dc);
			Win32Api.DeleteObject (memory_bitmap);
			Win32Api.DeleteDC (memory_dc);
			
			return res;
		}
		
		public static void GrabScreen(Drawing.Image bitmap, int x, int y)
		{
			System.Drawing.Bitmap   native = bitmap.BitmapImage.NativeBitmap;
			System.Drawing.Graphics gfx = System.Drawing.Graphics.FromImage (native);
			
			System.IntPtr bitmap_dc  = gfx.GetHdc ();
			System.IntPtr desktop_dc = Win32Api.GetDC (System.IntPtr.Zero);
			
			int dx = native.Width;
			int dy = native.Height;
			
			y = System.Windows.Forms.SystemInformation.VirtualScreen.Height - y - dy;
			
			Win32Api.SetStretchBltMode (bitmap_dc, Win32Const.BLT_COLOR_ON_COLOR);
			Win32Api.StretchBlt (bitmap_dc, 0, 0, dx, dy, desktop_dc, x, y, dx, dy, Win32Const.ROP_SRC_COPY);
			
			gfx.ReleaseHdc (bitmap_dc);
			Win32Api.ReleaseDC (System.IntPtr.Zero, desktop_dc);
			gfx.Flush ();
			gfx.Dispose ();
		}
	}
}
