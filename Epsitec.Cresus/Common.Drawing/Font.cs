//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Runtime.InteropServices;

namespace Epsitec.Common.Drawing
{
	public sealed class Font : System.IDisposable
	{
		#region Private constructors
		Font(System.IntPtr handle)
		{
			System.Diagnostics.Debug.Assert (handle != System.IntPtr.Zero);
			this.handle = handle;
		}
		
		Font(System.IntPtr handle, string synthetic_style, SyntheticFontMode synthetic_mode) : this (handle)
		{
			this.synthetic_style = synthetic_style;
			this.synthetic_mode  = synthetic_mode;
		}

		~Font()
		{
			this.Dispose (false);
		}
		#endregion
		
		#region Low level initialisation
		[System.Runtime.InteropServices.DllImport("Kernel32.dll")] private static extern System.IntPtr LoadLibrary(string fullpath);
		
		static Font()
		{
			//	Pour une raison étrange, la DLL Win32 doit être chargée très, très tôt, sinon
			//	elle pourrait ne pas être trouvée (c'est probablement lié à la copie locale des
			//	assemblies .NET qui est faite lors de l'exécution avec NUnit).
			
			System.IntPtr result = Font.LoadLibrary ("AntiGrain.Win32.dll");
			System.Diagnostics.Debug.Assert (result != System.IntPtr.Zero);
			
			System.Diagnostics.Debug.WriteLine ("AntiGrain.Win32.dll loaded successfully");
			
			Font.Initialise ();
		}
		#endregion
		
		#region IDisposable members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		public System.IntPtr			Handle
		{
			get { return this.handle; }
		}
		
		public string					FaceName
		{
			get { return AntiGrain.Font.Face.GetName (this.handle, (int) NameId.Face); }
		}
		
		public string					StyleName
		{
			get
			{
				if (this.synthetic_style == null)
				{
					return AntiGrain.Font.Face.GetName (this.handle, (int) NameId.Style);
				}
				
				return this.synthetic_style;
			}
		}
		
		public bool						IsStyleBold
		{
			get
			{
				if (this.StyleName.IndexOf ("Bold") > -1)
				{
					return true;
				}
				
				return false;
			}
		}
		
		public bool						IsStyleItalic
		{
			get
			{
				if ((this.StyleName.IndexOf ("Italic") > -1) ||
					(this.StyleName.IndexOf ("Oblique") > -1))
				{
					return true;
				}
                
				return false;
			}
		}
		
		public bool						IsStyleRegular
		{
			get
			{
				switch (this.StyleName)
				{
					case "Roman":
					case "Normal":
					case "Regular":
						return true;
				}
				return false;
			}
		}
		
		public bool						IsSingleFontInFace
		{
			get
			{
				FaceInfo info = this.GetFaceInfo ();
				
				return info.Count == 1;
			}
		}
		
		public string					LocalStyleName
		{
			get { return AntiGrain.Font.Face.GetName (this.handle, (int) NameId.StyleUserLocale); }
		}
		
		public string					OpticalName
		{
			get { return AntiGrain.Font.Face.GetName (this.handle, (int) NameId.Optical); }
		}
		
		public string					UniqueName
		{
			get { return AntiGrain.Font.Face.GetName (this.handle, (int) NameId.Unique); }
		}
		
		public string					FullName
		{
			get
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				string face    = this.FaceName;
				string style   = this.StyleName;
				string optical = this.OpticalName;
				
				buffer.Append (face);
				
				if (style != "")
				{
					buffer.Append (" ");
					buffer.Append (style);
				}
				
				if (optical != "")
				{
					buffer.Append (" ");
					buffer.Append (optical);
				}
				
				return buffer.ToString ();
			}
		}
		
		public double					Ascender
		{
			get { return AntiGrain.Font.Face.GetMetrics (this.handle, 1); }
		}
		
		public double					Descender
		{
			get { return AntiGrain.Font.Face.GetMetrics (this.handle, 2); }
		}
		
		public double					LineHeight
		{
			get { return AntiGrain.Font.Face.GetMetrics (this.handle, 3); }
		}
		
		public bool						IsSynthetic
		{
			get { return this.synthetic_mode != SyntheticFontMode.None; }
		}
		
		public SyntheticFontMode		SyntheticFontMode
		{
			get { return this.synthetic_mode; }
		}
		
		public Transform				SyntheticTransform
		{
			get
			{
				switch (this.synthetic_mode)
				{
					case SyntheticFontMode.Oblique:
						return new Transform (1, System.Math.Sin (Font.DefaultObliqueAngle * System.Math.PI / 180.0), 0, 1, 0, 0);
					default:
						return new Transform ();
				}
			}
		}
		
		public double					CaretSlope
		{
			get
			{
				if (this.synthetic_mode == SyntheticFontMode.Oblique)
				{
					return 90 - Font.DefaultObliqueAngle;
				}
				
				return AntiGrain.Font.Face.GetCaretSlope (this.handle);
			}
		}
		
		
		public Font.FaceInfo GetFaceInfo()
		{
			Font.SetupFonts ();
			return this.face_info;
		}
		
		
		public int GetGlyphIndex(int unicode)
		{
			try
			{
				return AntiGrain.Font.Face.GetGlyphIndex (this.handle, unicode);
			}
			catch (System.NullReferenceException ex)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Exception in GetGlyphIndex for font {0}:", this.FullName));
				System.Diagnostics.Debug.WriteLine (ex.Message);
				throw;
			}
		}
		
		public double GetGlyphAdvance(int glyph)
		{
			return AntiGrain.Font.Face.GetGlyphAdvance (this.handle, glyph);
		}
		
		public double GetCharAdvance(int unicode)
		{
			return AntiGrain.Font.Face.GetCharAdvance (this.handle, unicode);
		}
		
		public double GetTextAdvance(string text)
		{
			return AntiGrain.Font.Face.GetTextAdvance (this.handle, text, 0);
		}
		
		public Rectangle GetGlyphBounds(int glyph)
		{
			if (this.synthetic_mode == SyntheticFontMode.Oblique)
			{
				Path path = new Path ();
				path.Append (this, glyph, this.SyntheticTransform);
				
				return path.ComputeBounds ();
			}
			
			double x1, y1, x2, y2;
			AntiGrain.Font.Face.GetGlyphBounds (this.handle, glyph, out x1, out y1, out x2, out y2);
			return new Rectangle (x1, y1, x2 - x1, y2 - y1);
		}
		
		public Rectangle GetCharBounds(int unicode)
		{
			if (this.synthetic_mode == SyntheticFontMode.Oblique)
			{
				Path path = new Path ();
				path.Append (this, this.GetGlyphIndex (unicode), this.SyntheticTransform);
				
				return path.ComputeBounds ();
			}
			
			double x1, y1, x2, y2;
			AntiGrain.Font.Face.GetCharBounds (this.handle, unicode, out x1, out y1, out x2, out y2);
			return new Rectangle (x1, y1, x2 - x1, y2 - y1);
		}
		
		public Rectangle GetTextBounds(string text)
		{
			if (this.synthetic_mode == SyntheticFontMode.Oblique)
			{
				Path path = new Path ();
				Transform transform = this.SyntheticTransform;
				
				foreach (char unicode in text)
				{
					int glyph = this.GetGlyphIndex (unicode);
					path.Append (this, glyph, transform);
					transform.TX = transform.TX + this.GetGlyphAdvance (glyph);
				}
				
				return path.ComputeBounds ();
			}
			
			double x1, y1, x2, y2;
			AntiGrain.Font.Face.GetTextBounds (this.handle, text, 0, out x1, out y1, out x2, out y2);
			
			return new Rectangle (x1, y1, x2 - x1, y2 - y1);
		}
		
		
		public void GetTextCharEndX(string text, out double[] x_array)
		{
			int n = text.Length;
			
			x_array = new double[n];
			
			int count = AntiGrain.Font.Face.GetTextCharEndXArray (this.handle, text, 0, x_array);
			
			System.Diagnostics.Debug.Assert (count == n);
		}
		
		public void GetTextCharEndX(string text, Font.ClassInfo[] infos, out double[] x_array)
		{
			int n = text.Length;
			
			x_array = new double[n];
			
			int count = AntiGrain.Font.Face.GetTextCharEndXArray (this.handle, text, 0, x_array);
			
			System.Diagnostics.Debug.Assert (count == n);
			
			double x1 = 0;
			double x2 = 0;
			
			double scale_space = 1.0;
			double scale_plain = 1.0;
			
			for (int i = 0; i < infos.Length; i++)
			{
				switch (infos[i].ClassId)
				{
					case ClassId.PlainText:
						scale_plain = infos[i].Scale;
						break;
					
					case ClassId.Space:
						scale_space = infos[i].Scale;
						break;
				}
			}
			
			//	Transforme les [x] absolus en [dx], multiplie par l'échelle à utiliser pour la classe
			//	de caractère concernée, puis retransforme en [x] absolus :
			
			for (int i = 0; i < n; i++)
			{
				double dx = x_array[i] - x1;
				
				switch (text[i])
				{
					case ' ':
					case (char) 160:
						dx *= scale_space;
						break;
					default:
						dx *= scale_plain;
						break;
				}
				
				x1 = x_array[i];
				x2 = x2 + dx;
				
				x_array[i] = x2;
			}
		}
		
		public void GetTextClassInfos(string text, out ClassInfo[] infos, out double width, out double elasticity)
		{
			int n = text.Length;
			
			int    n_text  = 0;
			int    n_space = 0;
			double w_text  = 0;
			double w_space = 0;
			
			width      = 0;
			elasticity = 0;
			
			for (int i = 0; i < n; i++)
			{
				int unicode = text[i];
				
				switch (unicode)
				{
					case ' ':
					case (char) 160:
						n_space += 1;
						w_space += this.GetCharAdvance (unicode);
						break;
					
					default:
						n_text += 1;
						w_text += this.GetCharAdvance (unicode);
						break;
				}
			}
			
			int m = ((n_text > 0 && w_text > 0) ? 1 : 0) + ((n_space > 0 && w_space > 0) ? 1 : 0);
			infos = new ClassInfo[m];
			
			m = 0;
			
			if ((n_text > 0) && (w_text > 0))
			{
				double e = 0.00 * n_text;
				
				infos[m++]  = new ClassInfo (ClassId.PlainText, n_text, w_text, e);
				width      += w_text;
				elasticity += e;
			}
			
			if ((n_space > 0) && (w_space > 0))
			{
				double e = 1.00 * n_space;
				
				infos[m++]  = new ClassInfo (ClassId.Space, n_space, w_space, e);
				width      += w_space;
				elasticity += e;
			}
		}
		
		public void GetGlyphs(string text, out int[] glyphs, out byte[] glyph_n)
		{
			int n = text.Length;
			
			glyphs  = new int[n];
			glyph_n = new byte[n];
			
			for (int i = 0; i < n; i++)
			{
				glyphs[i]  = this.GetGlyphIndex (text[i]);
				glyph_n[i] = 1;
			}
		}
		
		public void GetGlyphsEndX(string text, out double[] glyph_x, out int[] glyphs, out byte[] glyph_n)
		{
			int n = text.Length;
			
			this.GetTextCharEndX (text, out glyph_x);
			
			glyphs  = new int[n];
			glyph_n = new byte[n];
			
			for (int i = 0; i < n; i++)
			{
				glyphs[i]  = this.GetGlyphIndex (text[i]);
				glyph_n[i] = 1;
			}
		}
		
		
		#region Decompiled from System.Drawing
		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto), ComVisible(false)]
		public class LOGFONT
		{
			public LOGFONT()
			{
			}
			
			public override string ToString()
			{
				object[] objArray1 = new object[28];
				objArray1[0] = "lfHeight=";
				objArray1[1] = this.lfHeight;
				objArray1[2] = ", lfWidth=";
				objArray1[3] = this.lfWidth;
				objArray1[4] = ", lfEscapement=";
				objArray1[5] = this.lfEscapement;
				objArray1[6] = ", lfOrientation=";
				objArray1[7] = this.lfOrientation;
				objArray1[8] = ", lfWeight=";
				objArray1[9] = this.lfWeight;
				objArray1[10] = ", lfItalic=";
				objArray1[11] = this.lfItalic;
				objArray1[12] = ", lfUnderline=";
				objArray1[13] = this.lfUnderline;
				objArray1[14] = ", lfStrikeOut=";
				objArray1[15] = this.lfStrikeOut;
				objArray1[16] = ", lfCharSet=";
				objArray1[17] = this.lfCharSet;
				objArray1[18] = ", lfOutPrecision=";
				objArray1[19] = this.lfOutPrecision;
				objArray1[20] = ", lfClipPrecision=";
				objArray1[21] = this.lfClipPrecision;
				objArray1[22] = ", lfQuality=";
				objArray1[23] = this.lfQuality;
				objArray1[24] = ", lfPitchAndFamily=";
				objArray1[25] = this.lfPitchAndFamily;
				objArray1[26] = ", lfFaceName=";
				objArray1[27] = this.lfFaceName;
				return string.Concat(objArray1);
			}

			public byte lfCharSet;
			public byte lfClipPrecision;
			public int lfEscapement;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)]
			public string lfFaceName;
			public int lfHeight;
			public byte lfItalic;
			public int lfOrientation;
			public byte lfOutPrecision;
			public byte lfPitchAndFamily;
			public byte lfQuality;
			public byte lfStrikeOut;
			public byte lfUnderline;
			public int lfWeight;
			public int lfWidth;
 		}
		
		public struct HandleRef
		{
			// Methods
			public HandleRef(object wrapper, System.IntPtr handle)
			{
				this.m_wrapper = wrapper;
				this.m_handle = handle;
			}
			public static explicit operator System.IntPtr(HandleRef value)
			{
				return value.m_handle;
			}

			// Properties
			public System.IntPtr Handle { get { return this.m_handle; } }
			public object Wrapper { get { return this.m_wrapper; } }

			// Fields
			internal System.IntPtr m_handle;
			internal object m_wrapper;
 		}
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
		public static extern int GetObject(System.IntPtr hObject, int nSize, [In, Out] LOGFONT lf);
		[DllImport("user32.dll", EntryPoint="GetDC", CharSet=CharSet.Auto, ExactSpelling=true)]
		private static extern System.IntPtr IntGetDC(System.IntPtr hWnd);
		[DllImport("user32.dll", EntryPoint="ReleaseDC", CharSet=CharSet.Auto, ExactSpelling=true)]
		private static extern int IntReleaseDC(System.IntPtr hWnd, System.IntPtr hDC);
		

		public static int GetObject(System.IntPtr hObject, ref LOGFONT lp)
		{
			return Font.GetObject(hObject, Marshal.SizeOf(typeof(LOGFONT)), lp);
 		}
		public static System.Drawing.Font FromHfont(System.IntPtr hfont)
		{
			LOGFONT logfont1 = new LOGFONT();
			Font.GetObject(hfont, ref logfont1);
			System.IntPtr ptr1 = Font.IntGetDC(System.IntPtr.Zero);
			try
			{
				return Font.FromLogFont(logfont1, ptr1);
			}
			finally
			{
				Font.IntReleaseDC(System.IntPtr.Zero, ptr1);
 			}
 		}
		[DllImport("gdiplus.dll", CharSet=CharSet.Ansi, ExactSpelling=true)]
		internal static extern int GdipCreateFontFromLogfontA(System.IntPtr hdc, [In, Out, MarshalAs(UnmanagedType.AsAny)] object lf, out System.IntPtr font);
		[DllImport("gdiplus.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
		internal static extern int GdipCreateFontFromLogfontW(System.IntPtr hdc, [In, Out, MarshalAs(UnmanagedType.AsAny)] object lf, out System.IntPtr font);
		
		public static System.Drawing.Font FromLogFont(object lf, System.IntPtr hdc)
		{
			int num1;
			bool flag1;
			System.IntPtr ptr1 = System.IntPtr.Zero;
			if (Marshal.SystemDefaultCharSize == 1)
			{
				num1 = Font.GdipCreateFontFromLogfontA(hdc, lf, out ptr1);
 			}
			else
			{
				num1 = Font.GdipCreateFontFromLogfontW(hdc, lf, out ptr1);
 			}
			
			if (num1 == 16)
			{
				throw new System.ArgumentException("GDI+ not a TrueType font, no name");
 			}
			if (num1 != 0)
			{
				throw new System.Exception("StatusException" + num1);
 			}
			if (ptr1 == System.IntPtr.Zero)
			{
				throw new System.ArgumentException(string.Concat("GDI+ does not handle non True-type fonts: ", lf.ToString()));
 			}
			if (Marshal.SystemDefaultCharSize == 1)
			{
				flag1 = (Marshal.ReadByte(lf, 28) == 64);
 			}
			else
			{
				flag1 = (Marshal.ReadInt16(lf, 28) == 64);
 			}
			
			// return new System.Drawing.Font(ptr1, Marshal.ReadByte(lf, 23), flag1);
 
			return null;
		}
		#endregion
		
		public System.Drawing.Font GetOsFont(double scale)
		{
			if (this.os_font == null)
			{
				if (this.IsSynthetic)
				{
					return null;
				}
				
				System.IntPtr hfont = AntiGrain.Font.Face.GetOsHandle (this.handle);
				
				if (hfont != System.IntPtr.Zero)
				{
					Font.FromHfont (hfont);
					this.os_font = System.Drawing.Font.FromHfont (hfont);
				}
				
				if (this.os_font_cache == null)
				{
					this.os_font_cache = new System.Collections.Hashtable ();
				}
			}
			
			if (this.os_font != null)
			{
				if (this.os_font_cache.Contains (scale))
				{
					return this.os_font_cache[scale] as System.Drawing.Font;
				}
				
				double              size = this.os_font.SizeInPoints / this.os_font.Size;
				System.Drawing.Font font = new System.Drawing.Font (this.os_font.FontFamily, (float) (scale * size), this.os_font.Style);
				
				this.os_font_cache[scale] = font;
				
				return font;
			}
			
			return null;
		}
		
		
		public void FillPixelCache(string text, double size, double ox, double oy)
		{
			AntiGrain.Font.PixelCache.Fill (this.handle, text, size, ox, oy);
		}
		
		public double PaintPixelCache(Pixmap pixmap, string text, double size, double ox, double oy, Color color)
		{
			return AntiGrain.Font.PixelCache.Paint (pixmap.Handle, this.handle, text, size, ox, oy, color.R, color.G, color.B, color.A);
		}
		
		
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
			
			if (this.handle != System.IntPtr.Zero)
			{
				this.handle = System.IntPtr.Zero;
			}
		}
		
		
		public static void Initialise()
		{
			if (Font.initialised == false)
			{
				Font.initialised = true;
				
				System.Diagnostics.Debug.WriteLine ("Calling AntiGrain.Interface.");
				AntiGrain.Interface.Initialise ();
				System.Diagnostics.Debug.WriteLine ("AntiGrain.Interface initialised.");
				AntiGrain.Font.Initialise ();
				System.Diagnostics.Debug.WriteLine ("AntiGrain.Font initialised.");
			}
		}

		private static void SetupFonts()
		{
			if (Font.font_array == null)
			{
				System.Diagnostics.Debug.WriteLine ("SetupFonts called");
				
				int n = AntiGrain.Font.GetFaceCount ();
				
				Font.font_array = new Font[n];
				Font.font_hash  = new System.Collections.Hashtable ();
				
				int m = 0;
				
				for (int i = 0; i < n; i++)
				{
					Font   font = new Font (AntiGrain.Font.GetFaceByRank (i));
					string name = font.FullName;
					
					if (Font.font_hash.ContainsKey (name) == false)
					{
						Font.font_array[m++] = font;
						Font.font_hash[name] = font;
					}
				}
				
				Font[] array_compact = new Font[m];
				
				int j = 0;
				
				for (int i = 0; i < Font.font_array.Length; i++)
				{
					if (Font.font_array[i] != null)
					{
						array_compact[j++] = Font.font_array[i];
					}
				}
				
				Font.font_array = array_compact;
				
				System.Diagnostics.Debug.Assert (Font.font_array.Length == m);
			}
			
			if (Font.face_array == null)
			{
				System.Collections.Hashtable hash = new System.Collections.Hashtable ();
				
				//	Construit la table des familles des diverses fontes.
				
				int n = Font.font_array.Length;
				
				for (int i = 0; i < n; i++)
				{
					Font   font = Font.font_array[i];
					string face = font.FaceName;
					
					System.Collections.ArrayList list = hash[face] as System.Collections.ArrayList;
					
					if (list == null)
					{
						list = new System.Collections.ArrayList ();
						hash[face] = list;
					}
					
					list.Add (font);
				}
				
				n = hash.Count;
				
				//	Alloue la table des familles; pour chaque famille, stocke les diverses
				//	fontes natives trouvées.
				
				Font.face_array = new Font.FaceInfo[n];
				Font.face_hash  = new System.Collections.Hashtable ();
				
				string[] faces = new string[n];
				
				hash.Keys.CopyTo (faces, 0);
				System.Array.Sort (faces);
				
				for (int i = 0; i < n; i++)
				{
					string face = faces[i];
					System.Collections.ArrayList list = hash[face] as System.Collections.ArrayList;
					
					Font[] fonts = new Font[list.Count];
					list.CopyTo (fonts, 0);
					
					Font.face_array[i]   = new Font.FaceInfo (face, fonts);
					Font.face_hash[face] = Font.face_array[i];
				}
			}
		}
		
		
		public static int				Count
		{
			get
			{
				Font.SetupFonts ();
				return Font.font_array.Length;
			}
		}
		
		public static Font				DefaultFont
		{
			get
			{
				if (Font.default_font == null)
				{
					Font.default_font = Font.GetFont ("Tahoma", "Regular");
				}
				
				return Font.default_font;
			}
		}
		
		public static double			DefaultFontSize
		{
			get
			{
				return 10.8;
			}
		}
		
		public static double			DefaultObliqueAngle
		{
			get { return 20.0; }
		}
		
		public static Font.FaceInfo[]	Faces
		{
			get
			{
				Font.SetupFonts ();
				Font.FaceInfo[] faces = new FaceInfo[Font.face_array.Length];
				Font.face_array.CopyTo (faces, 0);
				
				return faces;
			}
		}
		
		public static Font GetFont(int rank)
		{
			Font.SetupFonts ();
			
			if ((rank >= 0) &&
				(rank < Font.font_array.Length))
			{
				return Font.font_array[rank];
			}
			
			return null;
		}
		
		public static Font GetFont(string face, string style)
		{
			return Font.GetFont (face, style, "");
		}
		
		public static Font GetFont(string face, string style, string optical)
		{
			Font.SetupFonts ();
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append (face);
			
			if (style != "")
			{
				buffer.Append (" ");
				buffer.Append (style);
			}
			
			if (optical != "")
			{
				buffer.Append (" ");
				buffer.Append (optical);
			}
			
			string key = buffer.ToString ();
			
			Font font = Font.font_hash[key] as Font;
			
			if (font == null)
			{
				int pos;
				
				pos = style.IndexOf ("Regular");
				
				if (pos >= 0)
				{
					font = Font.GetFont (face, style.Replace ("Regular", "Normal"), optical);
					
					if (font == null)
					{
						font = Font.GetFont (face, style.Replace ("Regular", "Roman"), optical);
					}
					
					return font;
				}
				
				pos = style.IndexOf ("Italic");
				
				if (pos >= 0)
				{
					return Font.GetFont (face, style.Replace ("Italic", "Oblique"), optical);
				}
				
				pos = style.IndexOf ("Oblique");
				
				if (pos >= 0)
				{
					//	Le style oblique n'existe pas pour cette fonte. Tentons de le synthétiser
					//	à partir de la version droite la plus approchante.
					
					string clean_style;
					
					clean_style = style.Replace ("Oblique", "");
					clean_style = clean_style.Trim ();
					
					if (clean_style == "")
					{
						clean_style = "Regular";
					}
					
					font = Font.GetFont (face, clean_style, optical);
					
					if (font != null)
					{
						//	La fonte de base (droite) existe. C'est une bonne nouvelle. On va créer
						//	une fonte synthétique oblique...
						
						Font   syn_font = new Font (font.Handle, style, SyntheticFontMode.Oblique);
						string syn_name = syn_font.FullName;
						
						System.Diagnostics.Debug.Assert (syn_font.StyleName == style);
						System.Diagnostics.Debug.Assert (syn_font.IsSynthetic);
						System.Diagnostics.Debug.Assert (Font.font_hash.ContainsKey (syn_name) == false);
						
						int n = Font.font_array.Length;
						Font[] array_copy = new Font[n+1];
						Font.font_array.CopyTo (array_copy, 0);
						Font.font_array = array_copy;
						
						Font.font_array[n]       = syn_font;
						Font.font_hash[syn_name] = syn_font;
						
						font = syn_font;
					}
				}
			}
			
			return font;
		}
		
		public static Font.FaceInfo GetFaceInfo(string face)
		{
			return Font.face_hash[face] as FaceInfo;
		}
		
		public static Font GetFontFallback(string face)
		{
			FaceInfo info = Font.face_hash[face] as FaceInfo;
			
			if (info != null)
			{
				Font[] fonts = info.GetFonts ();
				
				for (int i = 0; i < fonts.Length; i++)
				{
					Font font = fonts[i];
					
					if ((font.IsStyleBold == false) &&
						(font.IsStyleItalic == false))
					{
						return font;
					}
				}
				
				return fonts[0];
			}
			
			return null;
		}
		
		
		#region Class ClassInfo
		public class ClassInfo
		{
			public ClassInfo(ClassId id, int count, double width, double elasticity)
			{
				this.class_id   = id;
				this.count      = count;
				this.width      = width;
				this.elasticity = elasticity;
				this.scale      = 1.0;
			}
			
			
			public ClassId						ClassId
			{
				get
				{
					return this.class_id;
				}
			}
			
			public int							Count
			{
				get
				{
					return this.count;
				}
			}
			
			public double						Width
			{
				get
				{
					return this.width;
				}
			}
			
			public double						Elasticity
			{
				get
				{
					return this.elasticity;
				}
			}
			
			public double						Scale
			{
				get
				{
					return this.scale;
				}
				set
				{
					this.scale = value;
				}
			}
			
			
			protected ClassId					class_id;
			protected int						count;				//	nombre de glyphes dans cette classe
			protected double					width;				//	largeur cumulée
			protected double					elasticity;			//	élasticité des glyphes
			protected double					scale;				//	facteur d'échelle [x]
		}
		#endregion
		
		#region Enum ClassId
		public enum ClassId
		{
			Space		= 0,
			PlainText	= 1,
			Ligature_2	= 2,
			Ligature_3	= 3,
			
			//	...
			
			Other		= 100
		}
		#endregion
		
		#region Class FaceInfo
		public class FaceInfo
		{
			public FaceInfo(string name, Font[] fonts)
			{
				this.name  = name;
				this.fonts = fonts;
				
				for (int i = 0; i < this.fonts.Length; i++)
				{
					System.Diagnostics.Debug.Assert (this.fonts[i].FaceName == this.name);
					System.Diagnostics.Debug.Assert (this.fonts[i].face_info == null);
					
					this.fonts[i].face_info = this;
				}
			}
			
			
			public string						Name
			{
				get
				{
					return this.name;
				}
			}
			
			public string[]						StyleNames
			{
				get
				{
					System.Collections.Hashtable hash = new System.Collections.Hashtable ();
					
					for (int i = 0; i < this.fonts.Length; i++)
					{
						string name = this.fonts[i].StyleName;
						
						if (name == "")
						{
							name = "Regular";
						}
						
						hash[name] = this;
					}
					
					string[] names = new string[hash.Count];
					hash.Keys.CopyTo (names, 0);
					
					return names;
				}
			}
			
			public int							Count
			{
				get
				{
					return this.fonts.Length;
				}
			}
			
			public bool							IsLatin
			{
				get
				{
					return this.fonts[0].GetGlyphIndex ('e') > 0;
				}
			}
			
			
			public Font GetFont(bool bold, bool italic, double size)
			{
				string style_1 = null;
				string style_2 = null;
				string style_3 = null;
				
				if (bold)
				{
					if (italic)
					{
						style_1 = "Bold Italic";
						style_2 = "Bold Oblique";
					}
					else
					{
						style_1 = "Bold";
					}
				}
				else
				{
					if (italic)
					{
						style_1 = "Italic";
						style_2 = "Oblique";
					}
					else
					{
						style_1 = "Regular";
						style_2 = "Normal";
						style_3 = "Roman";
					}
				}
				
				foreach (Font font in this.fonts)
				{
					if ((font.StyleName == style_1) ||
						(font.StyleName == style_2) ||
						(font.StyleName == style_3))
					{
						return font;
					}
				}
				
				//	Le style spécifié n'existe pas en tant que tel. Demandons encore à Font de
				//	voir si ce n'est pas possible d'obtenir une fonte synthétique :
				
				Font synthetic = Font.GetFont (this.name, style_1);
				
				return synthetic;
			}
			
			public Font[] GetFonts()
			{
				Font[] copy = new Font[this.fonts.Length];
				this.fonts.CopyTo (copy, 0);
				return copy;
			}
			
			
			private string						name;
			private Font[]						fonts;
		}
		#endregion
		
		
		System.IntPtr							handle;
		string									synthetic_style;
		SyntheticFontMode						synthetic_mode;
		System.Drawing.Font						os_font;
		System.Collections.Hashtable			os_font_cache;
		FaceInfo								face_info;
		
		static Font[]							font_array = null;
		static FaceInfo[]						face_array = null;
		static System.Collections.Hashtable		font_hash  = null;
		static System.Collections.Hashtable		face_hash  = null;
		static Font								default_font;
		static bool								initialised = false;
		
		enum NameId
		{
			None, Face = 1, Style = 2, StyleUserLocale = 3, Optical = 4, Unique = 5
		}
	}
	
	public enum SyntheticFontMode
	{
		None, Oblique
	}
}
