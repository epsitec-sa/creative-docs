using System;
using System.Runtime.InteropServices;

namespace Epsitec.Common.OpenType.Platform
{
	public class Win32
	{
		#region Win32 LogFont Structure
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
		#endregion
		
		#region Win32 EnumLogFontEx Structure
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
		#endregion
		
		#region Win32 NewTextMetricEx Structure
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)] public class NewTextMetricEx
		{ 
			public int			tmHeight; 
			public int			tmAscent; 
			public int			tmDescent;
			public int			tmInternalLeading;
			public int			tmExternalLeading;
			public int			tmAveCharWidth;
			public int			tmMaxCharWidth;
			public int			tmWeight;
			public int			tmOverhang;
			public int			tmDigitizedAspectX;
			public int			tmDigitizedAspectY;
			public char			tmFirstChar;
			public char			tmLastChar;
			public char			tmDefaultChar;
			public char			tmBreakChar;
			public byte			tmItalic;
			public byte			tmUnderlined;
			public byte			tmStruckOut;
			public byte			tmPitchAndFamily;
			public byte			tmCharSet;
			public byte			tmReserved_1;
			public byte			tmReserved_2;
			public byte			tmReserved_3;
			public int			ntmFlags;
			public int			ntmSizeEM;
			public int			ntmCellHeight;
			public int			ntmAvgWidth;
			public uint			fsUsb_0;
			public uint			fsUsb_1;
			public uint			fsUsb_2;
			public uint			fsUsb_3;
			public uint			fsCsb_0;
			public uint			fsCsb_1;
			
			public class Flags
			{
				public const int	Italic				= 0x00000001;
				public const int	Bold				= 0x00000020;
				public const int	Regular				= 0x00000100;
				public const int	PostScript			= 0x00020000;
				public const int	TrueType			= 0x00040000;
				public const int	MultipleMaster		= 0x00080000;
				public const int	Type1				= 0x00100000;
				public const int	DigitalSignature	= 0x00200000;
			}
		}
		#endregion
		
		#region Win32 DLL Imports
		[DllImport ("gdi32.dll", CharSet=CharSet.Unicode)] static extern IntPtr CreateFontIndirect([In, MarshalAs(UnmanagedType.LPStruct)] LogFont log_font);
		[DllImport ("gdi32.dll", CharSet=CharSet.Unicode)] static extern IntPtr CreateFontIndirect([In, MarshalAs(UnmanagedType.LPStruct)] EnumLogFontEx log_font);
		[DllImport ("gdi32.dll")] static extern bool DeleteObject(IntPtr handle);
		[DllImport ("gdi32.dll")] static extern IntPtr SelectObject(IntPtr hdc, IntPtr handle);
		[DllImport ("gdi32.dll", SetLastError=true)] static extern int GetFontData(IntPtr hdc, int dwTable, int dwOffset, [Out] byte [] lpvBuffer, int cbData);
		[DllImport ("gdi32.dll", CharSet=CharSet.Unicode)] static extern int EnumFontFamiliesEx(IntPtr hdc, [In] LogFont log_font, EnumFontFamExProc proc, IntPtr lParam, uint dwFlags);
		[DllImport ("user32.dll")] static extern IntPtr GetDC(IntPtr hwnd);
		[DllImport ("user32.dll")] static extern void ReleaseDC(IntPtr hwnd, IntPtr hdc);
		#endregion
		
		#region Win32 Callback Procedures
		delegate int EnumFontFamExProc(EnumLogFontEx log_font_ex, NewTextMetricEx text_metrics, int font_type, IntPtr lParam);
		#endregion
		
		#region TempDC Class
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
		#endregion
		
		#region FontEnumerator Class
		private class FontEnumerator
		{
			public FontEnumerator()
			{
				this.names = new System.Collections.Hashtable ();
			}
			
			public FontEnumerator(string style_name)
			{
				this.style_name = style_name;
			}
			
			
			public string[]						Names
			{
				get
				{
					string[] names = new string[this.names.Count];
					this.names.Keys.CopyTo (names, 0);
					System.Array.Sort (names);
					return names;
				}
			}
			
			public Win32.LogFont				LogFont
			{
				get
				{
					return this.log_font;
				}
			}
			
			public int ProcessFamily(EnumLogFontEx log_font_ex, NewTextMetricEx text_metrics, int font_type, IntPtr lParam)
			{
				string name  = log_font_ex.lfFaceName;
				int    flags = text_metrics.ntmFlags;
				
				if ((name.Length > 0) &&
					(name[0] != '@') &&
					(log_font_ex.lfOutPrecision == 3) &&
					(log_font_ex.elfScript != "OEM/DOS"))
				{
					//	Pour le moment, ne conserve que les fontes OpenType purement
					//	TrueType (on ne sait pas que faire des fontes PostScript).
					
					if (((NewTextMetricEx.Flags.TrueType & flags) != 0)/* ||
						((NewTextMetricEx.Flags.PostScript & flags) != 0)*/)
					{
						if (this.names.Contains (name) == false)
						{
							this.names[name] = name;
						}
					}
				}
				
				return 1;
			}
			
			public int ProcessStyle(EnumLogFontEx log_font_ex, NewTextMetricEx text_metrics, int font_type, IntPtr lParam)
			{
				string name  = log_font_ex.elfFullName;
				string style = log_font_ex.elfStyle;
				
				if ((name.Length > 0) &&
					(name[0] != '@') &&
					(log_font_ex.lfOutPrecision == 3) &&
					(log_font_ex.elfScript != "OEM/DOS"))
				{
					if (this.names.Contains (style) == false)
					{
						this.names[style] = name;
					}
				}
				
				return 1;
			}
			
			public int ProcessFindLogFont(EnumLogFontEx log_font_ex, NewTextMetricEx text_metrics, int font_type, IntPtr lParam)
			{
				if (log_font_ex.elfStyle == style_name)
				{
					this.log_font = new LogFont ();
					
					this.log_font.lfHeight			= log_font_ex.lfHeight;
					this.log_font.lfWidth			= log_font_ex.lfWidth;
					this.log_font.lfEscapement		= log_font_ex.lfEscapement;
					this.log_font.lfOrientation		= log_font_ex.lfOutPrecision;
					this.log_font.lfWeight			= log_font_ex.lfWeight;
					this.log_font.lfItalic			= log_font_ex.lfItalic;
					this.log_font.lfUnderline		= log_font_ex.lfUnderline;
					this.log_font.lfStrikeOut		= log_font_ex.lfStrikeOut;
					this.log_font.lfCharSet			= log_font_ex.lfCharSet;
					this.log_font.lfOutPrecision	= log_font_ex.lfOutPrecision;
					this.log_font.lfClipPrecision	= log_font_ex.lfClipPrecision;
					this.log_font.lfQuality			= log_font_ex.lfQuality;
					this.log_font.lfPitchAndFamily	= log_font_ex.lfPitchAndFamily;
					this.log_font.lfFaceName		= log_font_ex.lfFaceName;
					
					return 0;
				}
				
				return 1;
			}
			
			
			System.Collections.Hashtable		names;
			string								style_name;
			LogFont								log_font;
		}
		#endregion
		
		public static string[] GetFontFamilies()
		{
			Win32.LogFont log_font = new Win32.LogFont ();
			
			log_font.lfCharSet = 1;
			
			using (TempDC dc = new TempDC ())
			{
				FontEnumerator enumerator = new FontEnumerator ();
				
				Win32.EnumFontFamExProc proc = new EnumFontFamExProc (enumerator.ProcessFamily);
				Win32.EnumFontFamiliesEx (dc.Handle, log_font, proc, System.IntPtr.Zero, 0);
				
				return enumerator.Names;
			}
		}
		
		public static string[] GetFontStyles(string family)
		{
			Win32.LogFont log_font = new Win32.LogFont ();
			
			log_font.lfCharSet  = 1;
			log_font.lfFaceName = family;
			
			using (TempDC dc = new TempDC ())
			{
				FontEnumerator enumerator = new FontEnumerator ();
				
				Win32.EnumFontFamExProc proc = new EnumFontFamExProc (enumerator.ProcessStyle);
				Win32.EnumFontFamiliesEx (dc.Handle, log_font, proc, System.IntPtr.Zero, 0);
				
				return enumerator.Names;
			}
		}
		
		
		public static object GetFontSystemDescription(string family, string style)
		{
			Win32.LogFont log_font = new Win32.LogFont ();
			
			log_font.lfCharSet  = 1;
			log_font.lfFaceName = family;
			
			using (TempDC dc = new TempDC ())
			{
				FontEnumerator enumerator = new FontEnumerator (style);
				
				Win32.EnumFontFamExProc proc = new EnumFontFamExProc (enumerator.ProcessFindLogFont);
				Win32.EnumFontFamiliesEx (dc.Handle, log_font, proc, System.IntPtr.Zero, 0);
				
				return enumerator.LogFont;
			}
		}
		
		 
		public static int GetFontWeight(object system_description)
		{
			LogFont lf = system_description as LogFont;
			
			return lf == null ? 0 : lf.lfWeight;
		}
		
		public static int GetFontItalic(object system_description)
		{
			LogFont lf = system_description as LogFont;
			
			return lf == null ? 0 : (int) lf.lfItalic;
		}
		
		public static byte[] LoadFontData(string family, string style)
		{
			return Win32.LoadFontData (Win32.GetFontSystemDescription (family, style));
		}
		
		public static byte[] LoadFontData(object system_description)
		{
			return Win32.LoadFontData (system_description as LogFont);
		}
		
		public static byte[] LoadFontData(LogFont lf) 
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
		
		
		public static byte[] LoadFontDataNameTable(string family, string style)
		{
			return Win32.LoadFontDataNameTable (Win32.GetFontSystemDescription (family, style));
		}
		
		public static byte[] LoadFontDataNameTable(object system_description)
		{
			return Win32.LoadFontDataNameTable (system_description as LogFont);
		}
		
		public static byte[] LoadFontDataNameTable(LogFont lf) 
		{
			byte[]        data = null;
			System.IntPtr font = Win32.CreateFontIndirect (lf);
			
			if (font != System.IntPtr.Zero)
			{
				using (TempDC dc = new TempDC ())
				{
					System.IntPtr old_font = Win32.SelectObject (dc.Handle, font);
					
					int table  = (('n') << 0) | (('a') << 8) | (('m') << 16) | (('e') << 24);
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