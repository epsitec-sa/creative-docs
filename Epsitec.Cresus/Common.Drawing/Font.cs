namespace Epsitec.Common.Drawing
{
	public class Font : System.IDisposable
	{
		Font(System.IntPtr handle)
		{
			System.Diagnostics.Debug.Assert (handle != System.IntPtr.Zero);
			this.handle = handle;
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
			get { return Agg.Library.AggFontFaceGetName (this.handle, (int) NameID.Style); }
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
			double x1, y1, x2, y2;
			Agg.Library.AggFontFaceGetGlyphBounds (this.handle, glyph, out x1, out y1, out x2, out y2);
			return new Rectangle (x1, y1, x2 - x1, y2 - y1);
		}
		
		public Rectangle GetCharBounds(int unicode)
		{
			double x1, y1, x2, y2;
			Agg.Library.AggFontFaceGetCharBounds (this.handle, unicode, out x1, out y1, out x2, out y2);
			return new Rectangle (x1, y1, x2 - x1, y2 - y1);
		}
		
		public Rectangle GetTextBounds(string text)
		{
			double x1, y1, x2, y2;
			Agg.Library.AggFontFaceGetTextBounds (this.handle, text, 0, out x1, out y1, out x2, out y2);
			return new Rectangle (x1, y1, x2 - x1, y2 - y1);
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
				
				int j = 0;
				
				for (int i = 0; i < n; i++)
				{
					Font   font = new Font (Agg.Library.AggFontGetFaceByRank (i));
					string name = font.FullName;
					
					if (Font.hash.ContainsKey (name) == false)
					{
						Font.array[j++] = font;
						Font.hash[name] = font;
					}
					
					System.Diagnostics.Debug.Assert (font != null);
				}
				
				Font.count = j;
			}
		}
		
		
		public static int				Count
		{
			get
			{
				Font.SetupFonts ();
				return Font.count;
			}
		}
		
		public static Font GetFont(int rank)
		{
			Font.SetupFonts ();
			
			if ((rank >= 0) &&
				(rank < Font.count))
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
			
			return Font.hash[key] as Font;
		}
		
		
		
		protected System.IntPtr							handle;
		protected static Font[]							array;
		protected static System.Collections.Hashtable	hash;
		protected static int							count;
		
		protected enum NameID
		{
			None, Face = 1, Style = 2, StyleUserLocale = 3, Optical = 4
		}
	}
}
