using System;
using System.Runtime.InteropServices;

namespace Epsitec.Common.OpenType.Platform
{
	public class Win32
	{
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)] public class LogFont
		{ 
			public const int LF_FACESIZE = 32;
			public int			lfHeight; 
			public int			lfWidth; 
			public int			lfEscapement; 
			public int			lfOrientation; 
			public int			lfWeight; 
			public byte			lfItalic; 
			public byte			lfUnderline; 
			public byte			lfStrikeOut; 
			public byte			lfCharSet; 
			public byte			lfOutPrecision; 
			public byte			lfClipPrecision; 
			public byte			lfQuality; 
			public byte			lfPitchAndFamily;
			
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=LF_FACESIZE)]
			public string		lfFaceName; 
		}
		
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)] public class EnumLogFontEx
		{ 
			public const int LF_FACESIZE = 32;
			public const int LF_FULLFACESIZE = 64;
			public int			lfHeight; 
			public int			lfWidth; 
			public int			lfEscapement; 
			public int			lfOrientation; 
			public int			lfWeight; 
			public byte			lfItalic; 
			public byte			lfUnderline; 
			public byte			lfStrikeOut; 
			public byte			lfCharSet; 
			public byte			lfOutPrecision; 
			public byte			lfClipPrecision; 
			public byte			lfQuality; 
			public byte			lfPitchAndFamily;
			
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=LF_FACESIZE)]
			public string		lfFaceName; 
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=LF_FULLFACESIZE)]
			public string		elfFullName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=LF_FACESIZE)]
			public string		elfStyle; 
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=LF_FACESIZE)]
			public string		elfScript; 
		}
		
		[DllImport ("gdi32.dll", CharSet=CharSet.Unicode)] static extern IntPtr CreateFontIndirect([In, MarshalAs(UnmanagedType.LPStruct)] LogFont log_font);
		[DllImport ("gdi32.dll", CharSet=CharSet.Unicode)] static extern IntPtr CreateFontIndirect([In, MarshalAs(UnmanagedType.LPStruct)] EnumLogFontEx log_font);
		[DllImport ("gdi32.dll")] static extern bool DeleteObject(IntPtr handle);
		[DllImport ("gdi32.dll")] static extern IntPtr SelectObject(IntPtr hdc, IntPtr handle);
		[DllImport ("gdi32.dll", SetLastError=true)] static extern int GetFontData(IntPtr hdc, int dwTable, int dwOffset, [Out] byte [] lpvBuffer, int cbData);
		[DllImport ("user32.dll")] static extern IntPtr GetDC(IntPtr hwnd);
		[DllImport ("user32.dll")] static extern void ReleaseDC(IntPtr hwnd, IntPtr hdc);
		
		delegate int EnumFontFamExProc(EnumLogFontEx lpelfe, System.IntPtr lpntme, int font_type, IntPtr lParam);
		
		[DllImport ("gdi32.dll", CharSet=CharSet.Unicode)] static extern int EnumFontFamiliesEx(IntPtr hdc, [In] LogFont log_font, EnumFontFamExProc proc, IntPtr lParam, uint dwFlags);
		
		class TempDC : System.IDisposable
		{
			public TempDC()
			{
				this.hdc = Win32.GetDC (System.IntPtr.Zero);
			}
			
			
			public System.IntPtr				Handle
			{
				get
				{
					return this.hdc;
				}
			}
			
			
			#region IDisposable Members
			public void Dispose()
			{
				Win32.ReleaseDC (System.IntPtr.Zero, this.hdc);
			}
			#endregion
			
			private System.IntPtr				hdc;
		}
		
		public static int EnumFontFamilies(EnumLogFontEx log_font_ex, System.IntPtr lpntme, int font_type, IntPtr lParam)
		{
			System.Diagnostics.Debug.WriteLine ("Enumerating: " + log_font_ex.elfFullName);
			Win32.LoadFontData (log_font_ex);
			return 1;
		}
 		
		public static byte[] LoadFontDataDrawing()
		{
			System.Drawing.Text.InstalledFontCollection fonts = new System.Drawing.Text.InstalledFontCollection ();
			System.Drawing.Font font_sample = new System.Drawing.Font (new System.Drawing.FontFamily ("Arial"), 10);
			
			Win32.LogFont log_font = new Win32.LogFont ();
			
			//font_sample.ToLogFont (log_font);
			
			log_font.lfCharSet = 1;
//			log_font.lfFaceName = "Arial";
			
			using (TempDC dc = new TempDC ())
			{
				Win32.EnumFontFamExProc proc = new EnumFontFamExProc (Win32.EnumFontFamilies);
				Win32.EnumFontFamiliesEx (dc.Handle, log_font, proc, System.IntPtr.Zero, 0);
			}
			
			return null;
		}
		
		public static byte[] LoadFontData(EnumLogFontEx lf) 
		{
			byte[]        data = null;
			System.IntPtr font = Win32.CreateFontIndirect (lf);
			
			if (font != System.IntPtr.Zero)
			{
				using (TempDC dc = new TempDC ())
				{
					System.IntPtr old_font = Win32.SelectObject (dc.Handle, font);
					
					int table  = 0;
					int length = Win32.GetFontData (dc.Handle, table, 0, null, 0);
					
					if (length > 0)
					{
						data = new byte[length];
						Win32.GetFontData (dc.Handle, table, 0, data, length);
					}
					
					Win32.SelectObject (dc.Handle, old_font);
				}
				
				Win32.DeleteObject (font);
			}
			
			return data;
		}
		
		public static byte[] LoadFontData(string face_name) 
		{
			LogFont lf = new LogFont ();
			
			lf.lfCharSet  = 1;
			lf.lfHeight   = -13;
			lf.lfWeight   = 400;
			lf.lfFaceName = face_name;
			
			byte[]        data = null;
			System.IntPtr font = Win32.CreateFontIndirect (lf);
			
			if (font != System.IntPtr.Zero)
			{
				using (TempDC dc = new TempDC ())
				{
					System.IntPtr old_font = Win32.SelectObject (dc.Handle, font);
					
					int table  = 0;
					int length = Win32.GetFontData (dc.Handle, table, 0, null, 0);
					
					if (length > 0)
					{
						data = new byte[length];
						Win32.GetFontData (dc.Handle, table, 0, data, length);
					}
					
					Win32.SelectObject (dc.Handle, old_font);
				}
				
				Win32.DeleteObject (font);
			}
			
			return data;
		}
	}
}