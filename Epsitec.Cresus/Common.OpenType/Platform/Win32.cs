//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System;
using System.Runtime.InteropServices;

namespace Epsitec.Common.OpenType.Platform
{
	internal sealed class Win32
	{
		private Win32()
		{
		}
		
		
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
		
		#region Win32 ABC Structure
		[StructLayout(LayoutKind.Sequential)] public struct ABC
		{ 
			public int			A; 
			public int			B; 
			public int			C; 
		}
		#endregion
		
		#region Win32 Rect Structure
		[StructLayout(LayoutKind.Sequential, Pack=1)] public struct Rect
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}
		#endregion
		
		#region Win32 EnumLogFontEx Structure
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)] public class EnumLogFontEx
		{ 
			public const int LF_FACESIZE = 32;
			public const int LF_FULLFACESIZE = 64;
			
			public int				lfHeight;
			public int				lfWidth;
			public int				lfEscapement;
			public int				lfOrientation;
			public int				lfWeight;
			public byte				lfItalic;
			public byte				lfUnderline;
			public byte				lfStrikeOut;
			public byte				lfCharSet;
			public byte				lfOutPrecision;
			public byte				lfClipPrecision;
			public byte				lfQuality;
			public byte				lfPitchAndFamily;
			
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=LF_FACESIZE)]
			public string			lfFaceName; 
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=LF_FULLFACESIZE)]
			public string			elfFullName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=LF_FACESIZE)]
			public string			elfStyle;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=LF_FACESIZE)]
			public string			elfScript;
		}
		#endregion
		
		#region Win32 TextMetric Structure
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)] public class TextMetric
		{ 
			public int				tmHeight			= 0;
			public int				tmAscent			= 0;
			public int				tmDescent			= 0;
			public int				tmInternalLeading	= 0;
			public int				tmExternalLeading	= 0;
			public int				tmAveCharWidth		= 0;
			public int				tmMaxCharWidth		= 0;
			public int				tmWeight			= 0;
			public int				tmOverhang			= 0;
			public int				tmDigitizedAspectX	= 0;
			public int				tmDigitizedAspectY	= 0;
			public char				tmFirstChar			= '\0';
			public char				tmLastChar			= '\0';
			public char				tmDefaultChar		= '\0';
			public char				tmBreakChar			= '\0';
			public byte				tmItalic			= 0;
			public byte				tmUnderlined		= 0;
			public byte				tmStruckOut			= 0;
			public byte				tmPitchAndFamily	= 0;
			public byte				tmCharSet			= 0;
			public byte				tmReserved_1		= 0;
			public byte				tmReserved_2		= 0;
			public byte				tmReserved_3		= 0;
		}
		#endregion
		
		#region Win32 NewTextMetricEx Structure
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)] public class NewTextMetricEx
		{ 
			public int				tmHeight			= 0;
			public int				tmAscent			= 0;
			public int				tmDescent			= 0;
			public int				tmInternalLeading	= 0;
			public int				tmExternalLeading	= 0;
			public int				tmAveCharWidth		= 0;
			public int				tmMaxCharWidth		= 0;
			public int				tmWeight			= 0;
			public int				tmOverhang			= 0;
			public int				tmDigitizedAspectX	= 0;
			public int				tmDigitizedAspectY	= 0;
			public char				tmFirstChar			= '\0';
			public char				tmLastChar			= '\0';
			public char				tmDefaultChar		= '\0';
			public char				tmBreakChar			= '\0';
			public byte				tmItalic			= 0;
			public byte				tmUnderlined		= 0;
			public byte				tmStruckOut			= 0;
			public byte				tmPitchAndFamily	= 0;
			public byte				tmCharSet			= 0;
			public byte				tmReserved_1		= 0;
			public byte				tmReserved_2		= 0;
			public byte				tmReserved_3		= 0;
			public int				ntmFlags			= 0;
			public int				ntmSizeEM			= 0;
			public int				ntmCellHeight		= 0;
			public int				ntmAvgWidth			= 0;
			public uint				fsUsb_0				= 0;
			public uint				fsUsb_1				= 0;
			public uint				fsUsb_2				= 0;
			public uint				fsUsb_3				= 0;
			public uint				fsCsb_0				= 0;
			public uint				fsCsb_1				= 0;
			
			public class Flags
			{
				public const int	Italic				= 0x00000001;
				public const int	Bold				= 0x00000020;
				public const int	Regular				= 0x00000100;
				public const int	PostScript_OpenType	= 0x00020000;
				public const int	TrueType_OpenType	= 0x00040000;
				public const int	MultipleMaster		= 0x00080000;
				public const int	Type1				= 0x00100000;
				public const int	DigitalSignature	= 0x00200000;
			}
			
			public class FontType
			{
				public const int	Raster				= 0x01;
				public const int	Device				= 0x02;
				public const int	TrueType			= 0x04;
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
		[DllImport ("gdi32.dll")] static extern bool GetCharABCWidthsI(IntPtr hdc, int first, int count, [In] ushort[] indices, [Out] ABC [] buffer);
		[DllImport ("gdi32.dll", CharSet=CharSet.Unicode)] static extern bool GetTextMetrics(IntPtr hdc, [Out] TextMetric buffer);
		[DllImport ("gdi32.dll", CharSet=CharSet.Unicode)] static extern bool ExtTextOut(IntPtr hdc, int x, int y, int options, [In, MarshalAs(UnmanagedType.LPStruct)] Rect rect, [In] ushort[] text, int count, [In] int[] dx);
		[DllImport ("gdi32.dll", CharSet=CharSet.Unicode)] static extern bool ExtTextOut(IntPtr hdc, int x, int y, int options, System.IntPtr rect, [In] ushort[] text, int count, [In] int[] dx);
		#endregion
		
		#region ExtTextOutOptions
		[Flags] public enum ExtTextOutOptions
		{
			Grayed		= 0x0001,
			Opaque		= 0x0002,
			Clipped		= 0x0004,
			GlyphIndex	= 0x0010,
			PairDxDy	= 0x2000
		}
		#endregion
		
		#region Win32 Callback Procedures
		delegate int EnumFontFamExProc(EnumLogFontEx log_font_ex, NewTextMetricEx text_metrics, int font_type, IntPtr lParam);
		#endregion
		
		#region TempDC Class
		internal sealed class TempDC : System.IDisposable
		{
			public TempDC()
			{
				this.hdc = Win32.GetDC (System.IntPtr.Zero);
			}
			
			public TempDC(System.IntPtr font) : this ()
			{
				this.sel_font = Win32.SelectObject (this.hdc, font);
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
				if (this.sel_font != System.IntPtr.Zero)
				{
					Win32.SelectObject (this.hdc, this.sel_font);
				}
				
				Win32.ReleaseDC (System.IntPtr.Zero, this.hdc);
			}
			#endregion
			
			private System.IntPtr				hdc;
			private System.IntPtr				sel_font;
		}
		#endregion
		
		#region FontHandle Class
		internal sealed class FontHandle : Platform.IFontHandle
		{
			internal FontHandle(System.IntPtr handle)
			{
				this.handle = handle;
			}
			
			~FontHandle()
			{
				this.Dispose (false);
			}
			
			
			#region IFontHandle Members
			public System.IntPtr				Handle
			{
				get
				{
					return this.handle;
				}
			}
			
			
			public void Dispose()
			{
				this.Dispose (true);
				System.GC.SuppressFinalize (this);
			}
			#endregion
			
			private void Dispose(bool disposing)
			{
				if (this.handle != System.IntPtr.Zero)
				{
					Win32.DeleteObject (this.handle);
					this.handle = System.IntPtr.Zero;
				}
			}
			
			
			private System.IntPtr				handle;
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
				
//				System.Diagnostics.Debug.WriteLine (string.Format ("{0} : {1:X8}/{2}/{3}", name, flags, log_font_ex.lfOutPrecision, log_font_ex.elfScript));
				
				if ((name.Length > 0) &&
					(name[0] != '@') &&
					(log_font_ex.lfOutPrecision == 3) &&
					(log_font_ex.elfScript != "OEM/DOS"))
				{
					//	On conserve toutes les fontes TrueType et OpenType...
					
					if (((NewTextMetricEx.Flags.TrueType_OpenType & flags) != 0) ||
						((NewTextMetricEx.Flags.PostScript_OpenType & flags) != 0) ||
						(font_type == NewTextMetricEx.FontType.TrueType))
					{
						if (this.names.Contains (name) == false)
						{
//							System.Diagnostics.Debug.WriteLine (string.Format ("{0} : {1:X8}/{2}/{3}", name, flags, log_font_ex.lfOutPrecision, log_font_ex.elfScript));
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
		
		public static Platform.IFontHandle GetFontHandle(object system_description, int size)
		{
			LogFont lf = system_description as LogFont;
			
			lf.lfHeight = -size;
			lf.lfWidth  = 0;
			
			System.IntPtr handle = Win32.CreateFontIndirect (lf);
			
			return handle == System.IntPtr.Zero ? null : new FontHandle (handle);
		}
		 
		
		public static bool FillFontWidths(Platform.IFontHandle font, int glyph, int count, int[] widths, int[] lsb, int[] rsb)
		{
			using (TempDC dc = new TempDC (font.Handle))
			{
				ABC[] abc = new ABC[count];
				TextMetric metric = new TextMetric ();
				Win32.GetTextMetrics (dc.Handle, metric);
				
				if (Win32.GetCharABCWidthsI (dc.Handle, glyph, count, null, abc))
				{
					if (widths != null)
					{
						for (int i = 0; i < count; i++)
						{
							widths[i] = abc[i].A + abc[i].B + abc[i].C;
						}
					}
					
					if (lsb != null)
					{
						for (int i = 0; i < count; i++)
						{
							lsb[i] = abc[i].A;
						}
					}
					
					if (rsb != null)
					{
						for (int i = 0; i < count; i++)
						{
							rsb[i] = abc[i].B;
						}
					}
					
					return true;
				}
				
				return false;
			}
		}
		
		public static bool FillFontHeights(Platform.IFontHandle font, out int height, out int ascent, out int descent, out int internal_leading, out int external_leading)
		{
			height  = 0;
			ascent  = 0;
			descent = 0;
			
			internal_leading = 0;
			external_leading = 0;
			
			using (TempDC dc = new TempDC (font.Handle))
			{
				TextMetric metric = new TextMetric ();
				Win32.GetTextMetrics (dc.Handle, metric);
				
				height  = metric.tmHeight;
				ascent  = metric.tmAscent;
				descent = - metric.tmDescent;
				
				internal_leading = metric.tmInternalLeading;
				external_leading = metric.tmExternalLeading;
				
				return true;
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
				using (TempDC dc = new TempDC (font))
				{
					int table  = 0x66637474;		//	"ttcf"
					int length = Win32.GetFontData (dc.Handle, table, 0, null, 0);
					
					//	Si la tentative d'accès à la table TTC n'a pas abouti, on essaie
					//	encore d'accéder normalement à la fonte :
					
					if (length <= 0)
					{
						table  = 0;
						length = Win32.GetFontData (dc.Handle, table, 0, null, 0);
					}
					
					if (length > 0)
					{
						data = new byte[length];
						
						int count = System.Math.Min (100*1024, length);
						
						if (count == length)
						{
							Win32.GetFontData (dc.Handle, table, 0, data, length);
						}
						else
						{
							int    offset = 0;
							byte[] temp   = new byte[count];
							
							for (;;)
							{
								count = System.Math.Min (length, offset + temp.Length) - offset;
								
								if (count == 0)
								{
									break;
								}
								
								Win32.GetFontData (dc.Handle, table, offset, temp, count);
								
								System.Buffer.BlockCopy (temp, 0, data, offset, count);
								
								offset += count;
							}
						}
					}
				}
				
				Win32.DeleteObject (font);
			}
			
			return data;
		}
		
		
		public static byte[] LoadFontDataNameTable(string family, string style)
		{
			return Win32.LoadFontDataNameTable (Win32.GetFontSystemDescription (family, style));
		}
		
		private static byte[] LoadFontDataNameTable(object system_description)
		{
			return Win32.LoadFontDataNameTable (system_description as LogFont);
		}
		
		private static byte[] LoadFontDataNameTable(LogFont lf) 
		{
			byte[]        data = null;
			System.IntPtr font = Win32.CreateFontIndirect (lf);
			
			if (font != System.IntPtr.Zero)
			{
				using (TempDC dc = new TempDC (font))
				{
					if (Win32.IsTrueTypeCollection (dc.Handle))
					{
						//	C'est une collection TrueType. On ne peut pas simplement
						//	demander à GetFontData de retourner la table en question,
						//	car les offsets seraient faux. Il faudrait donc lire la
						//	fonte en entier et extraire ce qui est pertinent !
						
						//	TODO: ...
						
						return null;
					}
					
					int table  = (('n') << 0) | (('a') << 8) | (('m') << 16) | (('e') << 24);
					int length = Win32.GetFontData (dc.Handle, table, 0, null, 0);
					
					if (length > 0)
					{
						data = new byte[length];
						Win32.GetFontData (dc.Handle, table, 0, data, length);
					}
				}
				
				Win32.DeleteObject (font);
			}
			
			return data;
		}
		
		
		private static bool IsTrueTypeCollection(System.IntPtr dc_handle)
		{
			int table  = 0x66637474;		//	"ttcf"
			int length = Win32.GetFontData (dc_handle, table, 0, null, 0);
			
			return (length > 0) ? true : false;
		}
	}
}