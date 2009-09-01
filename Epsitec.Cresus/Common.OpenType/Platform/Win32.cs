//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			public byte				tmReserved1			= 0;
			public byte				tmReserved2			= 0;
			public byte				tmReserved3			= 0;
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
			public byte				tmReserved1			= 0;
			public byte				tmReserved2			= 0;
			public byte				tmReserved3			= 0;
			public int				ntmFlags			= 0;
			public int				ntmSizeEM			= 0;
			public int				ntmCellHeight		= 0;
			public int				ntmAvgWidth			= 0;
			public uint				fsUsb0				= 0;
			public uint				fsUsb1				= 0;
			public uint				fsUsb2				= 0;
			public uint				fsUsb3				= 0;
			public uint				fsCsb0				= 0;
			public uint				fsCsb1				= 0;
			
			public class Flags
			{
				public const int	Italic				= 0x00000001;
				public const int	Bold				= 0x00000020;
				public const int	Regular				= 0x00000100;
				public const int	PostScriptOpenType	= 0x00020000;
				public const int	TrueTypeOpenType	= 0x00040000;
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
		[DllImport ("gdi32.dll", CharSet=CharSet.Unicode)] static extern IntPtr CreateFontIndirect([In, MarshalAs(UnmanagedType.LPStruct)] LogFont logFont);
		[DllImport ("gdi32.dll", CharSet=CharSet.Unicode)] static extern IntPtr CreateFontIndirect([In, MarshalAs(UnmanagedType.LPStruct)] EnumLogFontEx logFont);
		[DllImport ("gdi32.dll")] static extern bool DeleteObject(IntPtr handle);
		[DllImport ("gdi32.dll")] static extern IntPtr SelectObject(IntPtr hdc, IntPtr handle);
		[DllImport ("gdi32.dll", SetLastError=true)] static extern int GetFontData(IntPtr hdc, int dwTable, int dwOffset, [Out] byte [] lpvBuffer, int cbData);
		[DllImport ("gdi32.dll", CharSet=CharSet.Unicode)] static extern int EnumFontFamiliesEx(IntPtr hdc, [In] LogFont logFont, EnumFontFamExProc proc, IntPtr lParam, uint dwFlags);
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
		delegate int EnumFontFamExProc(EnumLogFontEx logFontEx, NewTextMetricEx textMetrics, int fontType, IntPtr lParam);
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
				this.selFont = Win32.SelectObject (this.hdc, font);
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
				if (this.selFont != System.IntPtr.Zero)
				{
					Win32.SelectObject (this.hdc, this.selFont);
				}
				
				Win32.ReleaseDC (System.IntPtr.Zero, this.hdc);
			}
			#endregion
			
			private System.IntPtr				hdc;
			private System.IntPtr				selFont;
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
			
			public FontEnumerator(string styleName)
			{
				this.styleName = styleName;
			}


			public string						FullHash
			{
				get
				{
					return this.fullHash;
				}
				set
				{
					this.fullHash = value;
				}
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
					return this.logFont;
				}
			}
			
			public int ProcessFamily(EnumLogFontEx logFontEx, NewTextMetricEx textMetrics, int fontType, IntPtr lParam)
			{
				string name  = logFontEx.lfFaceName;
				int    flags = textMetrics.ntmFlags;
				
//				System.Diagnostics.Debug.WriteLine (string.Format ("{0} : {1:X8}/{2}/{3}", name, flags, logFontEx.lfOutPrecision, logFontEx.elfScript));
				
				if ((name.Length > 0) &&
					(name[0] != '@') &&
					(logFontEx.lfOutPrecision == 3) &&
					(logFontEx.elfScript != "OEM/DOS"))
				{
					//	On conserve toutes les fontes TrueType et OpenType...
					
					if (((NewTextMetricEx.Flags.TrueTypeOpenType & flags) != 0) ||
						((NewTextMetricEx.Flags.PostScriptOpenType & flags) != 0) ||
						(fontType == NewTextMetricEx.FontType.TrueType))
					{
						if (this.names.Contains (name) == false)
						{
//							System.Diagnostics.Debug.WriteLine (string.Format ("{0} : {1:X8}/{2}/{3}", name, flags, logFontEx.lfOutPrecision, logFontEx.elfScript));
							this.names[name] = name;
						}
					}
				}
				
				return 1;
			}
			
			public int ProcessStyle(EnumLogFontEx logFontEx, NewTextMetricEx textMetrics, int fontType, IntPtr lParam)
			{
				string name  = logFontEx.elfFullName;
				string style = logFontEx.elfStyle;
				
				if ((name.Length > 0) &&
					(name[0] != '@') &&
					(logFontEx.lfOutPrecision == 3) &&
					(logFontEx.elfScript != "OEM/DOS"))
				{
					if (this.names.Contains (style) == false)
					{
						this.names[style] = name;
					}
				}
				
				return 1;
			}
			
			public int ProcessFindLogFont(EnumLogFontEx logFontEx, NewTextMetricEx textMetrics, int fontType, IntPtr lParam)
			{
				bool ok = false;

				if (this.fullHash != null)
				{
					ok = (this.fullHash == FontName.GetFullHash (logFontEx.lfFaceName));
				}
				else if (logFontEx.elfStyle == styleName)
				{
					ok = true;
				}

				if (ok)
				{
					this.logFont = new LogFont ();
					
					this.logFont.lfHeight			= logFontEx.lfHeight;
					this.logFont.lfWidth			= logFontEx.lfWidth;
					this.logFont.lfEscapement		= logFontEx.lfEscapement;
					this.logFont.lfOrientation		= logFontEx.lfOutPrecision;
					this.logFont.lfWeight			= logFontEx.lfWeight;
					this.logFont.lfItalic			= logFontEx.lfItalic;
					this.logFont.lfUnderline		= logFontEx.lfUnderline;
					this.logFont.lfStrikeOut		= logFontEx.lfStrikeOut;
					this.logFont.lfCharSet			= logFontEx.lfCharSet;
					this.logFont.lfOutPrecision		= logFontEx.lfOutPrecision;
					this.logFont.lfClipPrecision	= logFontEx.lfClipPrecision;
					this.logFont.lfQuality			= logFontEx.lfQuality;
					this.logFont.lfPitchAndFamily	= logFontEx.lfPitchAndFamily;
					this.logFont.lfFaceName			= logFontEx.lfFaceName;
					
					return 0;
				}
				
				return 1;
			}
			
			
			System.Collections.Hashtable		names;
			string								styleName;
			string								fullHash;
			LogFont								logFont;
		}
		#endregion
		
		
		public static string[] GetFontFamilies()
		{
			Win32.LogFont logFont = new Win32.LogFont ();
			
			logFont.lfCharSet = 1;
			
			using (TempDC dc = new TempDC ())
			{
				FontEnumerator enumerator = new FontEnumerator ();
				
				Win32.EnumFontFamExProc proc = new EnumFontFamExProc (enumerator.ProcessFamily);
				Win32.EnumFontFamiliesEx (dc.Handle, logFont, proc, System.IntPtr.Zero, 0);
				
				return enumerator.Names;
			}
		}
		
		public static string[] GetFontStyles(string family)
		{
			Win32.LogFont logFont = new Win32.LogFont ();
			
			logFont.lfCharSet  = 1;
			logFont.lfFaceName = family;
			
			using (TempDC dc = new TempDC ())
			{
				FontEnumerator enumerator = new FontEnumerator ();
				
				Win32.EnumFontFamExProc proc = new EnumFontFamExProc (enumerator.ProcessStyle);
				Win32.EnumFontFamiliesEx (dc.Handle, logFont, proc, System.IntPtr.Zero, 0);
				
				return enumerator.Names;
			}
		}
		
		
		public static object GetFontSystemDescription(string family, string style)
		{
			Win32.LogFont logFont = new Win32.LogFont ();
			
			logFont.lfCharSet  = 1;
			logFont.lfFaceName = family;
			
			using (TempDC dc = new TempDC ())
			{
				FontEnumerator enumerator;

				if (family == null)
				{
					enumerator = new FontEnumerator ();
					enumerator.FullHash = style;
				}
				else
				{
					enumerator = new FontEnumerator (style);
				}

				Win32.EnumFontFamExProc proc = new EnumFontFamExProc (enumerator.ProcessFindLogFont);
				Win32.EnumFontFamiliesEx (dc.Handle, logFont, proc, System.IntPtr.Zero, 0);
				
				return enumerator.LogFont;
			}
		}
		
		public static Platform.IFontHandle GetFontHandle(object systemDescription, int size)
		{
			LogFont lf = systemDescription as LogFont;

            if (lf == null)
            {
                return null;
            }

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
		
		public static bool FillFontHeights(Platform.IFontHandle font, out int height, out int ascent, out int descent, out int internalLeading, out int externalLeading)
		{
			height  = 0;
			ascent  = 0;
			descent = 0;
			
			internalLeading = 0;
			externalLeading = 0;

            if (font == null)
            {
                return false;
            }

			using (TempDC dc = new TempDC (font.Handle))
			{
				TextMetric metric = new TextMetric ();
				Win32.GetTextMetrics (dc.Handle, metric);
				
				height  = metric.tmHeight;
				ascent  = metric.tmAscent;
				descent = - metric.tmDescent;
				
				internalLeading = metric.tmInternalLeading;
				externalLeading = metric.tmExternalLeading;
				
				return true;
			}
		}
		
		
		public static int GetFontWeight(object systemDescription)
		{
			LogFont lf = systemDescription as LogFont;
			
			return lf == null ? 0 : lf.lfWeight;
		}
		
		public static int GetFontItalic(object systemDescription)
		{
			LogFont lf = systemDescription as LogFont;
			
			return lf == null ? 0 : (int) lf.lfItalic;
		}
		
		
		public static byte[] LoadFontData(string family, string style)
		{
			return Win32.LoadFontData (Win32.GetFontSystemDescription (family, style));
		}
		
		public static byte[] LoadFontData(object systemDescription)
		{
			return Win32.LoadFontData (systemDescription as LogFont);
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
		
		private static byte[] LoadFontDataNameTable(object systemDescription)
		{
			return Win32.LoadFontDataNameTable (systemDescription as LogFont);
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
		
		
		private static bool IsTrueTypeCollection(System.IntPtr dcHandle)
		{
			int table  = 0x66637474;		//	"ttcf"
			int length = Win32.GetFontData (dcHandle, table, 0, null, 0);
			
			return (length > 0) ? true : false;
		}
	}
}