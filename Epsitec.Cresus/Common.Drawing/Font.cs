//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Runtime.InteropServices;

namespace Epsitec.Common.Drawing
{
	public class Font : System.IDisposable
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
		
		public string					LocalStyleName
		{
			get { return AntiGrain.Font.Face.GetName (this.handle, (int) NameId.StyleUserLocale); }
		}
		
		public string					OpticalName
		{
			get { return AntiGrain.Font.Face.GetName (this.handle, (int) NameId.Optical); }
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
		
		
		public int GetGlyphIndex(int unicode)
		{
			return AntiGrain.Font.Face.GetGlyphIndex (this.handle, unicode);
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
		
		
		protected virtual void Dispose(bool disposing)
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

		protected static void SetupFonts()
		{
			if (Font.array == null)
			{
				System.Diagnostics.Debug.WriteLine ("SetupFonts called");
				
				int n = AntiGrain.Font.GetFaceCount ();
				
				Font.array = new Font[n];
				Font.hash = new System.Collections.Hashtable ();
				
				int m = 0;
				
				for (int i = 0; i < n; i++)
				{
					Font   font = new Font (AntiGrain.Font.GetFaceByRank (i));
					string name = font.FullName;
					
					if (Font.hash.ContainsKey (name) == false)
					{
						Font.array[m++] = font;
						Font.hash[name] = font;
					}
				}
				
				Font[] array_compact = new Font[m];
				
				int j = 0;
				
				for (int i = 0; i < Font.array.Length; i++)
				{
					if (Font.array[i] != null)
					{
						array_compact[j++] = Font.array[i];
					}
				}
				
				Font.array = array_compact;
				
				System.Diagnostics.Debug.Assert (Font.array.Length == m);
			}
		}
		
		
		public static int				Count
		{
			get
			{
				Font.SetupFonts ();
				return Font.array.Length;
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
		
		
		public static Font GetFont(int rank)
		{
			Font.SetupFonts ();
			
			if ((rank >= 0) &&
				(rank < Font.array.Length))
			{
				return Font.array[rank];
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
			
			Font font = Font.hash[key] as Font;
			
			if (font == null)
			{
				int pos;
				
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
						System.Diagnostics.Debug.Assert (Font.hash.ContainsKey (syn_name) == false);
						
						int n = Font.array.Length;
						Font[] array_copy = new Font[n+1];
						Font.array.CopyTo (array_copy, 0);
						Font.array = array_copy;
						
						Font.array[n]       = syn_font;
						Font.hash[syn_name] = syn_font;
						
						font = syn_font;
					}
				}
			}
			
			return font;
		}
		
		
		
		protected System.IntPtr							handle;
		protected string								synthetic_style;
		protected SyntheticFontMode						synthetic_mode;
		protected System.Drawing.Font					os_font;
		protected System.Collections.Hashtable			os_font_cache;
		
		protected static Font[]							array = null;
		protected static System.Collections.Hashtable	hash;
		protected static int							count;
		protected static Font							default_font;
		protected static bool							initialised = false;
		
		protected enum NameId
		{
			None, Face = 1, Style = 2, StyleUserLocale = 3, Optical = 4
		}
		
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
		
		public enum ClassId
		{
			Space		= 0,
			PlainText	= 1,
			Ligature_2	= 2,
			Ligature_3	= 3,
			
			//	...
			
			Other		= 100
		}
	}
	
	public enum SyntheticFontMode
	{
		None, Oblique
	}
}
