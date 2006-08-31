//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Drawing
{
	public sealed class Font : System.IDisposable
	{
		#region Private constructors
		Font(OpenType.FontIdentity fontIdentity)
		{
			this.open_type_font = Font.font_collection.CreateFont (fontIdentity);
		}
		
		Font(Font baseFont, string synthetic_style, SyntheticFontMode synthetic_mode)
		{
			this.open_type_font  = baseFont.open_type_font;
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

			Agg.Library.Initialize ();

			Font.SetupFonts ();
		}
		#endregion

		public static void Initialize()
		{
		}
		
		#region IDisposable members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		public System.IntPtr					Handle
		{
			get
			{
				if (this.handle == System.IntPtr.Zero)
				{
					byte[] data = this.OpenTypeFont.FontData.Data;
					int    size = data.Length;

					System.IntPtr osHandle = this.OpenTypeFont.FontIdentity.IsDynamicFont ? System.IntPtr.Zero : this.OpenTypeFont.GetFontHandleAtEmSize ();
					
					this.handle = AntiGrain.Font.CreateFaceHandle (data, size, osHandle);
				}
				
				return this.handle;
			}
		}
		
		public string							FaceName
		{
			get
			{
				return this.OpenTypeFont.FontIdentity.InvariantFaceName;
			}
		}

		public FontFaceInfo						FaceInfo
		{
			get
			{
				return this.face_info;
			}
		}
		
		public string							StyleName
		{
			get
			{
				return this.synthetic_style ?? this.OpenTypeFont.FontIdentity.InvariantStyleName;
			}
		}
		
		public bool								IsStyleBold
		{
			get
			{
				return (this.StyleName.IndexOf ("Bold") > -1);
			}
		}
		
		public bool								IsStyleItalic
		{
			get
			{
				return (this.StyleName.IndexOf ("Italic") > -1)
					|| (this.StyleName.IndexOf ("Oblique") > -1);
			}
		}
		
		public bool								IsStyleRegular
		{
			get
			{
				switch (this.StyleName)
				{
					case "Roman":
					case "Normal":
					case "Regular":
						return true;
					
					default:
						return false;
				}
			}
		}
		
		public string							LocaleStyleName
		{
			get
			{
				return this.OpenTypeFont.FontIdentity.LocaleStyleName;
			}
		}
		
		public string							OpticalName
		{
			get
			{
				return "";
				//	TODO: handle optical name
//				return this.OpenTypeFont.FontIdentity.InvariantOpticalName;
			}
		}
		
		public string							UniqueName
		{
			get
			{
				return this.OpenTypeFont.FontIdentity.UniqueFontId;
			}
		}
		
		public string							FullName
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
		
		public double							Ascender
		{
			get
			{
				return this.OpenTypeFont.GetAscender (1.0);
			}
		}
		
		public double							Descender
		{
			get
			{
				return this.OpenTypeFont.GetDescender (1.0);
			}
		}
		
		public double							LineHeight
		{
			get
			{
				double ascender  = this.OpenTypeFont.GetAscender (1.0);
				double descender = this.OpenTypeFont.GetDescender (1.0);
				double lineGap   = this.OpenTypeFont.GetLineGap (1.0);

				return ascender-descender+lineGap;
			}
		}
		
		public bool								IsSynthetic
		{
			get
			{
				return this.synthetic_mode != SyntheticFontMode.None;
			}
		}
		
		public SyntheticFontMode				SyntheticFontMode
		{
			get
			{
				return this.synthetic_mode;
			}
		}
		
		public Transform						SyntheticTransform
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
		
		public double							CaretSlope
		{
			get
			{
				if (this.synthetic_mode == SyntheticFontMode.Oblique)
				{
					return 90 - Font.DefaultObliqueAngle;
				}

				return this.OpenTypeFont.GetCaretAngleDeg ();
			}
		}
		
		public OpenType.Font					OpenTypeFont
		{
			get
			{
				return this.open_type_font;
			}
		}
		
		
		public ushort GetGlyphIndex(int unicode)
		{
			return this.OpenTypeFont.GetGlyphIndex (unicode);
		}
		
		public double GetGlyphAdvance(ushort glyph)
		{
			return this.OpenTypeFont.GetGlyphWidth (glyph, 1.0);
		}
		
		public double GetCharAdvance(int unicode)
		{
			return this.GetGlyphAdvance (this.GetGlyphIndex (unicode));
		}
		
		public double GetTextAdvance(string text)
		{
			ushort[] glyphs = this.OpenTypeFont.GenerateGlyphs (text);
			return this.OpenTypeFont.GetTotalWidth (glyphs, 1.0);
		}
		
		public Rectangle GetGlyphBounds(ushort glyph, double size)
		{
			Drawing.Rectangle rect = this.GetGlyphBounds (glyph);
			
			rect.Scale (size);
			
			return rect;
		}
		
		public Rectangle GetGlyphBounds(ushort glyph)
		{
			if (this.synthetic_mode == SyntheticFontMode.Oblique)
			{
				using (Path path = new Path ())
				{
					path.Append (this, glyph, this.SyntheticTransform);
					return path.ComputeBounds ();
				}
			}
			
			double x1, y1, x2, y2;

			this.OpenTypeFont.GetGlyphBounds (glyph, 1.0, out x1, out x2, out y1, out y2);
			
			return new Rectangle (x1, y1, x2 - x1, y2 - y1);
		}
		
		public Rectangle GetCharBounds(int unicode)
		{
			return this.GetGlyphBounds (this.GetGlyphIndex (unicode));
		}
		
		public Rectangle GetTextBounds(string text)
		{
			if (this.synthetic_mode == SyntheticFontMode.Oblique)
			{
				Path path = new Path ();
				Transform transform = this.SyntheticTransform;
				
				foreach (char unicode in text)
				{
					ushort glyph = this.GetGlyphIndex (unicode);
					path.Append (this, glyph, transform);
					transform.TX = transform.TX + this.GetGlyphAdvance (glyph);
				}
				
				return path.ComputeBounds ();
			}

			ushort[] glyphs = this.OpenTypeFont.GenerateGlyphs (text);
			double[] temp_x = new double[glyphs.Length];

			if (glyphs.Length == 0)
			{
				return Rectangle.Empty;
			}
			
			this.OpenTypeFont.GetPositions (glyphs, 1.0, 0.0, temp_x);

			double x1, y1, x2, y2;
			double xmin, ymin, xmax, ymax;

			this.OpenTypeFont.GetGlyphBounds (glyphs[0], 1.0, out x1, out x2, out y1, out y2);
			
			xmin = x1;
			ymin = y1;
			xmax = x2;
			ymax = y2;

			for (int i = 1; i < glyphs.Length; i++)
			{
				this.OpenTypeFont.GetGlyphBounds (glyphs[i], 1.0, out x1, out x2, out y1, out y2);
				
				x1 += temp_x[i];
				x2 += temp_x[i];

				xmin = System.Math.Min (xmin, x1);
				xmax = System.Math.Max (xmax, x2);
				ymin = System.Math.Min (ymin, y1);
				ymax = System.Math.Max (ymax, y2);
			}
			
			return new Rectangle (xmin, ymin, xmax - xmin, ymax - ymin);
		}
		
		
		public void GetTextCharEndX(string text, out double[] xPos)
		{
			int   n        = text.Length;
			int[] glyphMap = new int[n];
			
			xPos = new double[n];

			ushort[] glyphs = this.OpenTypeFont.GenerateGlyphs (text, ref glyphMap);
			double[] temp_x = new double[glyphs.Length+1];
			
			temp_x[glyphs.Length] = this.OpenTypeFont.GetPositions (glyphs, 1.0, 0.0, temp_x);

			int index = 0;
			
			for (int i = 0; i < glyphs.Length; i++)
			{
				int mapped = glyphMap[i]+1;

				if (mapped > 1)
				{
					double dx = temp_x[i+1] - temp_x[i];
					
					for (int j = 0; j < mapped; j++)
					{
						xPos[index] = temp_x[i] + dx * (j+1) / mapped;
						index++;
					}
				}
				else
				{
					xPos[index] = temp_x[i+1];
					index++;
				}
			}
		}
		
		public void GetTextCharEndX(string text, FontClassInfo[] infos, out double[] x_array)
		{
			this.GetTextCharEndX (text, out x_array);
			
			double scale_space = 1.0;
			double scale_plain = 1.0;
			
			for (int i = 0; i < infos.Length; i++)
			{
				switch (infos[i].GlyphClass)
				{
					case GlyphClass.PlainText:
						scale_plain = infos[i].Scale;
						break;
					
					case GlyphClass.Space:
						scale_space = infos[i].Scale;
						break;
				}
			}
			
			//	Transform absolute [x] into relative [dx], scale using the glyph class and then
			//	transform back [dx] to [x] :

			double x1 = 0;
			double x2 = 0;

			for (int i = 0; i < x_array.Length; i++)
			{
				double dx = x_array[i] - x1;
				
				dx *= (OpenType.Font.IsStretchableSpaceCharacter (text[i])) ? scale_space : scale_plain;
				
				x1 = x_array[i];
				x2 = x2 + dx;
				
				x_array[i] = x2;
			}
		}
		
		public void GetTextClassInfos(string text, out FontClassInfo[] infos, out double width, out double elasticity)
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

				if (OpenType.Font.IsStretchableSpaceCharacter (unicode))
				{
					n_space += 1;
					w_space += this.GetCharAdvance (unicode);
				}
				else
				{
					n_text += 1;
					w_text += this.GetCharAdvance (unicode);
				}
			}
			
			int m = ((n_text > 0 && w_text > 0) ? 1 : 0) + ((n_space > 0 && w_space > 0) ? 1 : 0);
			infos = new FontClassInfo[m];
			
			m = 0;
			
			if ((n_text > 0) && (w_text > 0))
			{
				double e = 0.00 * n_text;
				
				infos[m++]  = new FontClassInfo (GlyphClass.PlainText, n_text, w_text, e);
				width      += w_text;
				elasticity += e;
			}
			
			if ((n_space > 0) && (w_space > 0))
			{
				double e = 1.00 * n_space;
				
				infos[m++]  = new FontClassInfo (GlyphClass.Space, n_space, w_space, e);
				width      += w_space;
				elasticity += e;
			}
		}

		public void GetGlyphsEndX(string text, out double[] xPos, out ushort[] glyphs, out byte[] glyphCharCount)
		{
			int[] glyphMap = new int[text.Length];
			
			glyphs         = this.OpenTypeFont.GenerateGlyphs (text, ref glyphMap);
			xPos           = new double[glyphs.Length];
			glyphCharCount = new byte[glyphs.Length];
			
			this.OpenTypeFont.GetPositions (glyphs, 1.0, 0.0, xPos);
			
			for (int i = 0; i < glyphs.Length; i++)
			{
				glyphCharCount[i] = (byte) (glyphMap[i] + 1);
			}
		}
		
		
		
		
		public void FillPixelCache(string text, double size, double ox, double oy)
		{
			if (string.IsNullOrEmpty (text))
			{
				return;
			}
			
			ushort[] glyphs = this.OpenTypeFont.GenerateGlyphs (text);
			AntiGrain.Font.PixelCache.Fill (this.Handle, glyphs, size, ox, oy);
		}
		
		public double PaintPixelCache(Pixmap pixmap, string text, double size, double ox, double oy, Color color)
		{
			if (string.IsNullOrEmpty (text))
			{
				return 0.0;
			}

			ushort[] glyphs = this.OpenTypeFont.GenerateGlyphs (text);
			return AntiGrain.Font.PixelCache.Paint (pixmap.Handle, this.Handle, glyphs, size, ox, oy, color.R, color.G, color.B, color.A);
		}


		public static void RegisterDynamicFont(byte[] data)
		{
			OpenType.FontIdentity fid = Font.font_collection.RegisterDynamicFont (data);
			
			if (fid != null)
			{
				Font font = new Font (fid);
				string name = font.FullName;
				string face = font.FaceName;

				System.Diagnostics.Debug.Assert (Font.font_hash.ContainsKey (name) == false);
				
				Font.font_array.Add (font);
				Font.font_hash[name] = font;
				
				FontFaceInfo info;

				if (Font.face_hash.TryGetValue (face, out info) == false)
				{
					info = new FontFaceInfo (face);
					Font.face_hash[face] = info;
					Font.face_array.Add (info);
				}

				info.Add (font);
			}
		}
		
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
			
			if (this.handle != System.IntPtr.Zero)
			{
				AntiGrain.Font.DisposeFaceHandle (this.handle);
				this.handle = System.IntPtr.Zero;
			}
		}
		
		
		private static void SetupFonts()
		{
			bool save = false;
			
			if (Font.font_array == null)
			{
				System.Diagnostics.Debug.WriteLine ("SetupFonts called");

				Font.font_collection = OpenType.FontCollection.Default;
				Font.font_collection.LoadFromCache ();
				
				save = Font.font_collection.Initialize ();

				Font.font_array = new List<Font> ();
				Font.font_hash  = new Dictionary<string, Font> ();
				
				foreach (OpenType.FontIdentity fontIdentity in Font.font_collection)
				{
					Font font = new Font (fontIdentity);

					if (font.OpenTypeFont != null)
					{
						string name = font.FullName;

						System.Diagnostics.Debug.Assert (Font.font_hash.ContainsKey (name) == false);

						Font.font_array.Add (font);
						Font.font_hash[name] = font;
					}
					else
					{
						System.Diagnostics.Debug.WriteLine (string.Format ("{0} has no data", fontIdentity.FullName));
					}
				}
				
				System.Diagnostics.Debug.WriteLine ("SetupFonts done");
			}
			
			if (Font.face_array == null)
			{
				Font.face_array = new List<FontFaceInfo> ();
				Font.face_hash = new Dictionary<string, FontFaceInfo> ();
				
				foreach (Font font in Font.font_array)
				{
					string face = font.FaceName;

					FontFaceInfo info;

					if (Font.face_hash.TryGetValue (face, out info) == false)
					{
						info = new FontFaceInfo (face);
						Font.face_hash[face] = info;
						Font.face_array.Add (info);
					}

					info.Add (font);
				}
			}

			if (save)
			{
				Font.font_collection.SaveToCache ();
			}
		}
		
		
		public static int						Count
		{
			get
			{
				return Font.font_array.Count;
			}
		}
		
		public static Font						DefaultFont
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
		
		public static double					DefaultFontSize
		{
			get
			{
				return 10.8;
			}
		}
		
		public static double					DefaultObliqueAngle
		{
			get { return 20.0; }
		}
		
		public static FontFaceInfo[]			Faces
		{
			get
			{
				return Font.face_array.ToArray ();
			}
		}
		
		
		public static Font GetFont(int rank)
		{
			if ((rank >= 0) &&
				(rank < Font.font_array.Count))
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
			
			Font font;
			
			if (Font.font_hash.TryGetValue (key, out font) == false)
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
					if (font == null)
					{
						font = Font.GetFont (face, style.Replace ("Regular", ""), optical);
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
						
						Font   syn_font = new Font (font, style, SyntheticFontMode.Oblique);
						string syn_name = syn_font.FullName;
						
						System.Diagnostics.Debug.Assert (syn_font.StyleName == style);
						System.Diagnostics.Debug.Assert (syn_font.IsSynthetic);
						System.Diagnostics.Debug.Assert (Font.font_hash.ContainsKey (syn_name) == false);

						Font.font_array.Add (syn_font);
						Font.font_hash[syn_name] = syn_font;
						
						font = syn_font;
					}
				}
			}
			
			return font;
		}
		
		public static Font GetFont(OpenType.Font font)
		{
			return Font.GetFont (font.FontIdentity);
		}
		
		public static Font GetFont(OpenType.FontIdentity id)
		{
			return Font.GetFont (id.InvariantFaceName, id.InvariantStyleName);
		}
		
		public static FontFaceInfo GetFaceInfo(string face)
		{
			return Font.face_hash[face];
		}
		
		public static Font GetFontFallback(string face)
		{
			FontFaceInfo info = Font.face_hash[face];
			
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
		
		
		internal void DefineFaceInfo(FontFaceInfo info)
		{
			this.face_info = info;
		}
		
		
		
		System.IntPtr							handle;
		string									synthetic_style;
		SyntheticFontMode						synthetic_mode;
#if false
		System.Drawing.Font						os_font;
		System.Collections.Hashtable			os_font_cache;
#endif
		FontFaceInfo							face_info;
		OpenType.Font							open_type_font;
		
		static OpenType.FontCollection			font_collection;
		static List<Font>						font_array;
		static List<FontFaceInfo>				face_array;
		static Dictionary<string, Font>			font_hash;
		static Dictionary<string, FontFaceInfo>	face_hash;
		static Font								default_font;
		
		enum NameId
		{
			None,
			Face			= 1,
			Style			= 2,
			StyleUserLocale	= 3,
			Optical			= 4,
			Unique			= 5
		}
	}
	
	public enum SyntheticFontMode
	{
		None,
		Oblique
	}
}
