namespace Epsitec.Common.Drawing
{
	public class Font : System.IDisposable
	{
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
		
		static Font()
		{
			Font.Initialise ();
		}
		
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		
		
		public System.IntPtr			Handle
		{
			get { return this.handle; }
		}
		
		public string					FaceName
		{
			get { return Agg.Library.AggFontFaceGetName (this.handle, (int) NameID.Face); }
		}
		
		public string					StyleName
		{
			get
			{
				if (this.synthetic_style == null)
				{
					return Agg.Library.AggFontFaceGetName (this.handle, (int) NameID.Style);
				}
				
				return this.synthetic_style;
			}
		}
		
		public string					LocalStyleName
		{
			get { return Agg.Library.AggFontFaceGetName (this.handle, (int) NameID.StyleUserLocale); }
		}
		
		public string					OpticalName
		{
			get { return Agg.Library.AggFontFaceGetName (this.handle, (int) NameID.Optical); }
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
			get { return Agg.Library.AggFontFaceGetMetrics (this.handle, 1); }
		}
		
		public double					Descender
		{
			get { return Agg.Library.AggFontFaceGetMetrics (this.handle, 2); }
		}
		
		public double					LineHeight
		{
			get { return Agg.Library.AggFontFaceGetMetrics (this.handle, 3); }
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
		
		
		public int GetGlyphIndex(int unicode)
		{
			return Agg.Library.AggFontFaceGetGlyphIndex (this.handle, unicode);
		}
		
		public double GetGlyphAdvance(int glyph)
		{
			return Agg.Library.AggFontFaceGetGlyphAdvance (this.handle, glyph);
		}
		
		public double GetCharAdvance(int unicode)
		{
			return Agg.Library.AggFontFaceGetCharAdvance (this.handle, unicode);
		}
		
		public double GetTextAdvance(string text)
		{
			return Agg.Library.AggFontFaceGetTextAdvance (this.handle, text, 0);
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
			Agg.Library.AggFontFaceGetGlyphBounds (this.handle, glyph, out x1, out y1, out x2, out y2);
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
			Agg.Library.AggFontFaceGetCharBounds (this.handle, unicode, out x1, out y1, out x2, out y2);
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
			Agg.Library.AggFontFaceGetTextBounds (this.handle, text, 0, out x1, out y1, out x2, out y2);
			return new Rectangle (x1, y1, x2 - x1, y2 - y1);
		}
		
		
		public double[] GetTextCharEndX(string text)
		{
			int      len     = text.Length;
			double[] x_array = new double[len];
			int      count   = Agg.Library.AggFontFaceGetTextCharEndXArray (this.handle, text, 0, x_array);
			
			System.Diagnostics.Debug.Assert (count == len);
			
			return x_array;
		}
		
		
		public void FillPixelCache(string text, double size, double ox, double oy)
		{
			Agg.Library.AggFontPixelCacheFill (this.handle, text, size, ox, oy);
		}
		
		public void RenderPixelCache(Pixmap pixmap, string text, double size, double ox, double oy)
		{
			Agg.Library.AggFontPixelCacheRender (pixmap.Handle, this.handle, text, size, ox, oy);
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
			Agg.Library.AggFontInitialise ();
		}

		static protected void SetupFonts()
		{
			if (Font.array == null)
			{
				int n = Agg.Library.AggFontGetFaceCount ();
				
				Font.array = new Font[n];
				Font.hash = new System.Collections.Hashtable ();
				
				int m = 0;
				
				for (int i = 0; i < n; i++)
				{
					Font   font = new Font (Agg.Library.AggFontGetFaceByRank (i));
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
				return 11.6;
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
		
		protected static Font[]							array;
		protected static System.Collections.Hashtable	hash;
		protected static int							count;
		protected static Font							default_font;
		
		protected enum NameID
		{
			None, Face = 1, Style = 2, StyleUserLocale = 3, Optical = 4
		}
	}
	
	public enum SyntheticFontMode
	{
		None, Oblique
	}
}
