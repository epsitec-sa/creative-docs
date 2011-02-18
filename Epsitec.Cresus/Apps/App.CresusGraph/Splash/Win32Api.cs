//	Copyright © 2009-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Epsitec.Cresus.Graph.Splash
{
	internal class Win32Api
	{
		public static class Win32Const
		{
			public const int ULW_ALPHA			= 0x00000002;
			
			public const byte AC_SRC_OVER		= 0x00;
			public const byte AC_SRC_ALPHA		= 0x01;
			
			public const int WS_EX_LAYERED		= 0x080000;
			
			public const int GWL_EXSTYLE		= -20;
		}

		[StructLayout (LayoutKind.Sequential, Pack=1)]
		internal struct Point
		{
			public int X;
			public int Y;
		}

		[StructLayout (LayoutKind.Sequential, Pack=1)]
		internal struct Size
		{
			public int Width;
			public int Height;
		}

		[StructLayout (LayoutKind.Sequential, Pack=1)]
		internal struct BlendFunction
		{
			public byte BlendOp;
			public byte BlendFlags;
			public byte SourceConstantAlpha;
			public byte AlphaFormat;
		}

		public const int ERROR_ALREADY_EXISTS = 183;

		[DllImport ("User32.dll")]
		internal extern static int SetWindowLong(System.IntPtr handle, int index, int value);
		[DllImport ("User32.dll")]
		internal extern static int GetWindowLong(System.IntPtr handle, int index);
		
		[DllImport ("GDI32.dll")]
		internal extern static System.IntPtr CreateCompatibleDC(System.IntPtr dc);
		
		[DllImport ("User32.dll")]
		internal extern static System.IntPtr GetDC(System.IntPtr handle);
		
		[DllImport ("User32.dll")]
		internal extern static int ReleaseDC(System.IntPtr handle, System.IntPtr dc);

		[DllImport ("GDI32.dll")]
		internal extern static System.IntPtr SelectObject(System.IntPtr dc, System.IntPtr handleObject);
		
		[DllImport ("GDI32.dll")]
		internal extern static System.IntPtr DeleteObject(System.IntPtr handleObject);
		
		[DllImport ("GDI32.dll")]
		internal extern static bool DeleteDC(System.IntPtr dc);

		internal static int GetWindowExStyle(System.IntPtr handle)
		{
			return Win32Api.GetWindowLong (handle, Win32Const.GWL_EXSTYLE);
		}

		internal static int SetWindowExStyle(System.IntPtr handle, int exStyle)
		{
			return Win32Api.SetWindowLong (handle, Win32Const.GWL_EXSTYLE, exStyle);
		}

		[DllImport ("User32.dll", SetLastError=true)]
		internal extern static bool UpdateLayeredWindow(System.IntPtr handle, System.IntPtr dstDc, ref Win32Api.Point dst, ref Win32Api.Size size, System.IntPtr srcDc, ref Win32Api.Point src, int color, ref Win32Api.BlendFunction blend, int flags);
		
		internal static bool UpdateLayeredWindow(System.IntPtr handle, System.Drawing.Bitmap bitmap, int x, int y, double alpha)
		{
			Win32Api.Point srcPoint = new Win32Api.Point ();
			Win32Api.Point dstPoint = new Win32Api.Point ();
			Win32Api.Size  newSize  = new Win32Api.Size ();

			Win32Api.BlendFunction blendFunction = new Win32Api.BlendFunction ();

			//	Get the screen DC, then create a memory based DC compatible with the screen DC.

			System.IntPtr screenDc = Win32Api.GetDC (System.IntPtr.Zero);
			System.IntPtr memoryDc = Win32Api.CreateCompatibleDC (screenDc);

			//	Get access to the bitmap handle contained in the Bitmap object, then select it.

			System.IntPtr memoryBitmap = bitmap.GetHbitmap (System.Drawing.Color.FromArgb (0));
			System.IntPtr oldBitmap    = Win32Api.SelectObject (memoryDc, memoryBitmap);

			newSize.Width  = bitmap.Width;
			newSize.Height = bitmap.Height;
			dstPoint.X = x;
			dstPoint.Y = y;
			srcPoint.X = 0;
			srcPoint.Y = 0;

			blendFunction.BlendOp				= Win32Const.AC_SRC_OVER;
			blendFunction.BlendFlags			= 0;
			blendFunction.SourceConstantAlpha	= (byte) System.Math.Min (255, System.Math.Max (0, (int) (alpha * 255.9)));
			blendFunction.AlphaFormat			= Win32Const.AC_SRC_ALPHA;

			int flags = Win32Const.ULW_ALPHA;
			bool res = false;

			res = Win32Api.UpdateLayeredWindow (handle,
				/**/							screenDc,
				/**/							ref dstPoint,
				/**/							ref newSize,
				/**/							memoryDc,
				/**/							ref srcPoint,
				/**/							0,
				/**/							ref blendFunction,
				/**/							flags);

			if (res == false)
			{
				System.Diagnostics.Debug.WriteLine ("LastError = " + Marshal.GetLastWin32Error ().ToString ());
			}

			Win32Api.SelectObject (memoryDc, oldBitmap);
			Win32Api.ReleaseDC (System.IntPtr.Zero, screenDc);
			Win32Api.DeleteObject (memoryBitmap);
			Win32Api.DeleteDC (memoryDc);

			return res;
		}
	}
}
