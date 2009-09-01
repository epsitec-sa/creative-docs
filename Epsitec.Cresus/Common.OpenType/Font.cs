//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// The <c>Font</c> class maps the low-level OpenType font description to
	/// the real needs of an application which has to manipulate glyphs.
	/// </summary>
	public sealed class Font
	{
		internal Font()
		{
		}
		
		internal Font(FontIdentity identity)
		{
			this.Initialize (identity);
		}

		public FontManagerType					FontManagerType
		{
			get
			{
				return this.useSystemGlyphSize ? FontManagerType.System : FontManagerType.OpenType;
			}
		}
		
		public FontIdentity						FontIdentity
		{
			get
			{
				return this.identity;
			}
		}

		public FontData							FontData
		{
			get
			{
				return this.fontData;
			}
		}
		
		
		public double							SpaceWidth
		{
			get
			{
				ushort glyph = this.SpaceGlyph;
				return (glyph == 0xffff) ? 0.25 : this.GetGlyphWidth (glyph, 1.0);
			}
		}
		
		public double							FigureWidth
		{
			get
			{
				ushort glyph = this.GetGlyphIndex ('0');
				return (glyph == 0xffff) ? 0.5 : this.GetGlyphWidth (glyph, 1.0);
			}
		}
		
		public double							EmWidth
		{
			get
			{
				ushort glyph = this.GetGlyphIndex (0x2014);
				return (glyph == 0xffff) ? 1.0 : this.GetGlyphWidth (glyph, 1.0);
			}
		}
		
		public double							EnWidth
		{
			get
			{
				ushort glyph = this.GetGlyphIndex (0x2013);
				return (glyph == 0xffff) ? 0.5 : this.GetGlyphWidth (glyph, 1.0);
			}
		}
		
		public char								SpaceChar
		{
			get
			{
				return ' ';
			}
		}
		
		public char								HyphenChar
		{
			get
			{
				return '-';
			}
		}
		
		public char								EllipsisChar
		{
			get
			{
				return (char) 0x2026;
			}
		}
		
		public char								PeriodChar
		{
			get
			{
				return '.';
			}
		}
		
		public ushort							SpaceGlyph
		{
			get
			{
				if (this.spaceGlyph == 0)
				{
					this.spaceGlyph = this.otIndexMapping.GetGlyphIndex (this.SpaceChar);
				}
				
				return this.spaceGlyph;
			}
		}
		
		public ushort							HyphenGlyph
		{
			get
			{
				if (this.hyphenGlyph == 0)
				{
					this.hyphenGlyph = this.otIndexMapping.GetGlyphIndex (this.HyphenChar);
				}

				return this.hyphenGlyph;
			}
		}

		public ushort							EllipsisGlyph
		{
			get
			{
				if (this.ellipsisGlyph == 0)
				{
					this.ellipsisGlyph = this.otIndexMapping.GetGlyphIndex (this.EllipsisChar);
				}

				return this.ellipsisGlyph;
			}
		}

		public ushort							PeriodGlyph
		{
			get
			{
				if (this.periodGlyph == 0)
				{
					this.periodGlyph = this.otIndexMapping.GetGlyphIndex (this.PeriodChar);
				}

				return this.periodGlyph;
			}
		}

		public double							HyphenWidth
		{
			get
			{
				double perEm  = this.otHead.UnitsPerEm;
				return this.GetAdvance (this.HyphenGlyph) / perEm;
			}
		}

		public double							EllipsisWidth
		{
			get
			{
				double perEm  = this.otHead.UnitsPerEm;
				return this.GetAdvance (this.EllipsisGlyph) / perEm;
			}
		}

		public double							PeriodWidth
		{
			get
			{
				double perEm  = this.otHead.UnitsPerEm;
				ushort glyph   = this.PeriodGlyph;
				
				return glyph == 0xffff ? 0.5 : (this.GetAdvance (this.PeriodGlyph) / perEm);
			}
		}

		public FontType							FontType
		{
			get
			{
				return this.fontType;
			}
		}
		
		
		internal void Initialize(FontIdentity identity)
		{
			this.identity = identity;
			
			this.Initialize (this.identity.FontData);
		}
		
		internal void Initialize(FontData fontData)
		{
			this.fontData = fontData;
			this.fontType = FontType.Unsupported;
			
			this.otGSUB = TableGSUB.Create (this.fontData["GSUB"]);
			this.otGDEF = TableGDEF.Create (this.fontData["GDEF"]);
			this.otCmap = new TableCmap (this.fontData["cmap"]);
			this.otMaxp = new TableMaxp (this.fontData["maxp"]);
			this.otHead = new TableHead (this.fontData["head"]);
			this.otHhea = new TableHhea (this.fontData["hhea"]);
			this.otHmtx = new Tablehmtx (this.fontData["hmtx"]);

			if ((this.fontData["glyf"] != null) &&
				(this.fontData["loca"] != null))
			{
				this.fontType = FontType.TrueType;
			}
			else
			{
				this.fontType = FontType.PostScript;
			}
			
			if (this.FontType == FontType.TrueType)
			{
				this.otGlyf = new TableGlyf (this.fontData["glyf"]);

				switch (this.otHead.IndexToLocFormat)
				{
					case 0:
						this.otLocaShort = new TableLocaShort (this.fontData["loca"]);
						break;
					case 1:
						this.otLocaLong = new TableLocaLong (this.fontData["loca"]);
						break;
				}
			}
			
			//	TODO: handle GPOS for advanced glyph positioning, required by some advanced OpenType fonts.
			//	See http://partners.adobe.com/public/developer/opentype/index_table_formats2.html
			
			TableKern otKern = TableKern.Create (this.fontData["kern"]);
			
			if ((otKern != null) &&
				(otKern.Count > 0) &&
				(otKern.GetKerningTable (0).SubtableFormat == 0))
			{
				this.otKernFormat0 = otKern.GetKerningTable (0).Format0Subtable;
			}
			
			this.otIndexMapping = this.otCmap.FindFormatSubTable ();

			this.glyphF = this.otIndexMapping.GetGlyphIndex ('f');
			this.glyphI = this.otIndexMapping.GetGlyphIndex ('i');
			this.glyphL = this.otIndexMapping.GetGlyphIndex ('l');
			
			this.glyphLigFf  = this.otIndexMapping.GetGlyphIndex (0xfb00);
			this.glyphLigFi  = this.otIndexMapping.GetGlyphIndex (0xfb01);
			this.glyphLigFl  = this.otIndexMapping.GetGlyphIndex (0xfb02);
			this.glyphLigFfi = this.otIndexMapping.GetGlyphIndex (0xfb03);
			this.glyphLigFfl = this.otIndexMapping.GetGlyphIndex (0xfb04);
			
			this.glyphCache   = new ushort[Font.GlyphCacheSize];
			this.advanceCache = new ushort[Font.AdvanceCacheSize];
		}


		/// <summary>
		/// Compares the typography of two fonts and return true if both have the
		/// same glyphs (they use the same font type).
		/// </summary>
		/// <param name="a">The first font.</param>
		/// <param name="b">The second font.</param>
		/// <returns><c>true</c> if both fonts use the same font type; otherwise, <c>false</c></returns>
		public static bool HaveEqualTypography(Font a, Font b)
		{
			if (a == b)
			{
				return true;
			}
			
			if ((a == null) ||
				(b == null))
			{
				return false;
			}

			return a.FontIdentity.UniqueFontId == b.FontIdentity.UniqueFontId;
		}

		public int GetTypographyHashCode()
		{
			return this.FontIdentity.UniqueFontId.GetHashCode ();
		}

		/// <summary>
		/// Generates the glyphs for the specified text.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The glyphs represented by 16-bit unsigned values.</returns>
		public ushort[] GenerateGlyphs(string text)
		{
			int[] glMap = null;
			return this.GenerateGlyphs (text, ref glMap);
		}

		/// <summary>
		/// Generates the glyphs for the specified text. Fills the glyph length
		/// map, if provided, with a character count per glyph, for each output
		/// glyph.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="glMap">The glyph length map array or <c>null</c>.</param>
		/// <returns>
		/// The glyphs represented by 16-bit unsigned values.
		/// </returns>
		public ushort[] GenerateGlyphs(string text, ref int[] glMap)
		{
			int length = text.Length;
			ushort[] glyphs = new ushort[length];
			
			for (int i = 0; i < length; i++)
			{
				glyphs[i] = this.GetGlyphIndex (text[i]);
			}
			
			this.ApplySubstitutions (ref glyphs, ref glMap);
			
			return glyphs;
		}

		/// <summary>
		/// Generates the glyphs for the specified text.
		/// </summary>
		/// <param name="text">The text encoded as 32-bit Unicode characters.</param>
		/// <param name="start">The start offset in the character array.</param>
		/// <param name="length">The length of the text.</param>
		/// <returns>The glyphs represented by 16-bit unsigned values.</returns>
		public ushort[] GenerateGlyphs(ulong[] text, int start, int length)
		{
			ushort[] glyphs = new ushort[length];
			int[]    glMap = null;
			
			for (int i = 0; i < length; i++)
			{
				ulong bits = text[start+i];
				int   code = Font.UnicodeMask & (int) bits;
				
				glyphs[i] = (bits & Font.TransparentGlyphFlag) == 0 ? this.GetGlyphIndex (code) : (ushort) code;
			}
			
			this.ApplySubstitutions (ref glyphs, ref glMap);
			
			return glyphs;
		}

		/// <summary>
		/// Generates the glyphs and adjusts the attributes for the specified text.
		/// The caller can provide an array with one attribute for each input
		/// character; <c>GenerateGlyphs</c> will adjust the contents of the attribute
		/// array so that there is exactly one attribute for each output glyph. If
		/// several characters map to a single glyph, the attribute of the first
		/// character will be preserved and the others will be dropped.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="glyphs">The output glyphs array.</param>
		/// <param name="attributes">The attributes array which must be adjusted or
		/// <c>null</c> if there are no attributes.</param>
		public void GenerateGlyphs(string text, out ushort[] glyphs, ref byte[] attributes)
		{
			int   length = text.Length;
			int[] glMap;
			int   count;
			
			glyphs = new ushort[length];
			glMap = new int[length];
			
			for (int i = 0; i < length; i++)
			{
				glyphs[i] = this.GetGlyphIndex (text[i]);
			}
			
			this.ApplySubstitutions (ref glyphs, ref glMap);
			
			if (attributes != null)
			{
				length = glyphs.Length;
				count  = attributes.Length;
				
				int src = 0;
				int dst = 0;
				
				//	TODO: handle cases where we produce more glyphs than there are characters in the text.
				
				for (int i = 0; i < length; i++)
				{
					attributes[dst] = attributes[src];
					
					dst += 1;
					src += glMap[i] + 1;
				}
				while (src < count)
				{
					attributes[dst++] = attributes[src++];
				}
			}
		}

		/// <summary>
		/// Generates the glyphs.
		/// The caller can provide an array with one attribute for each input
		/// character; <c>GenerateGlyphs</c> will adjust the contents of the attribute
		/// array so that there is exactly one attribute for each output glyph. If
		/// several characters map to a single glyph, the attribute of the first
		/// character will be preserved and the others will be dropped.
		/// </summary>
		/// <param name="text">The text encoded as 32-bit Unicode characters.</param>
		/// <param name="start">The start offset in the character array.</param>
		/// <param name="length">The length of the text.</param>
		/// <param name="glyphs">The output glyphs array.</param>
		/// <param name="attributes">The attributes array which must be adjusted or
		/// <c>null</c> if there are no attributes.</param>
		public void GenerateGlyphs(ulong[] text, int start, int length, out ushort[] glyphs, ref byte[] attributes)
		{
			int[] glMap;
			int   count;
			
			glyphs = new ushort[length];
			glMap = new int[length];
			
			for (int i = 0; i < length; i++)
			{
				ulong bits = text[start+i];
				int   code = Font.UnicodeMask & (int) bits;
				
				glyphs[i] = (bits & Font.TransparentGlyphFlag) == 0 ? this.GetGlyphIndex (code) : (ushort) code;
			}
			
			this.ApplySubstitutions (ref glyphs, ref glMap);
			
			if (attributes != null)
			{
				length = glyphs.Length;
				count  = attributes.Length;
				
				int src = 0;
				int dst = 0;
				
				//	TODO: gérer le cas où il y a plus de glyphes en sortie qu'il n'y a de
				//	place dans la table des attributs.
				
				for (int i = 0; i < length; i++)
				{
					attributes[dst] = attributes[src];
					
					dst += 1;
					src += glMap[i] + 1;
				}
				while (src < count)
				{
					attributes[dst++] = attributes[src++];
				}
			}
		}

		/// <summary>
		/// Generates the glyphs.
		/// The caller can provide an array with one attribute for each input
		/// character; <c>GenerateGlyphs</c> will adjust the contents of the attribute
		/// array so that there is exactly one attribute for each output glyph. If
		/// several characters map to a single glyph, the attribute of the first
		/// character will be preserved and the others will be dropped.
		/// </summary>
		/// <param name="text">The text encoded as 32-bit Unicode characters.</param>
		/// <param name="start">The start offset in the character array.</param>
		/// <param name="length">The length of the text.</param>
		/// <param name="glyphs">The output glyphs array.</param>
		/// <param name="attributes">The attributes array which must be adjusted or
		/// <c>null</c> if there are no attributes.</param>
		public void GenerateGlyphs(ulong[] text, int start, int length, out ushort[] glyphs, ref short[] attributes)
		{
			int[] glMap;
			int   count;
			
			glyphs = new ushort[length];
			glMap = new int[length];
			
			for (int i = 0; i < length; i++)
			{
				ulong bits = text[start+i];
				int   code = Font.UnicodeMask & (int) bits;
				
				glyphs[i] = (bits & Font.TransparentGlyphFlag) == 0 ? this.GetGlyphIndex (code) : (ushort) code;
			}
			
			this.ApplySubstitutions (ref glyphs, ref glMap);
			
			if (attributes != null)
			{
				length = glyphs.Length;
				count  = attributes.Length;
				
				int src = 0;
				int dst = 0;
				
				//	TODO: gérer le cas où il y a plus de glyphes en sortie qu'il n'y a de
				//	place dans la table des attributs.
				
				for (int i = 0; i < length; i++)
				{
					attributes[dst] = attributes[src];
					
					dst += 1;
					src += glMap[i] + 1;
				}
				while (src < count)
				{
					attributes[dst++] = attributes[src++];
				}
			}
		}

		/// <summary>
		/// Gets the width of the specified space glyph.
		/// </summary>
		/// <param name="glyph">The space glyph.</param>
		/// <param name="size">The point size of the font.</param>
		/// <returns>
		/// The width of the specified space glyph.
		/// </returns>
		public double GetGlyphWidth(ushort glyph, double size)
		{
			if (this.useSystemGlyphSize)
			{
				if (glyph >= 0xff00)
				{
					return (int) (this.GetSpaceGlyphWidth (glyph) * size + 0.5);
				}
				else
				{
					FontSizeInfo info = this.identity.GetSizeInfo ((int)(size + 0.5));
					return info.GetGlyphWidth (glyph);
				}
			}
			else
			{
				int numGlyph     = this.otMaxp.NumGlyphs;
				int numHMetrics = this.otHhea.NumHMetrics;
				
				double advance = 0;
				double perEm  = this.otHead.UnitsPerEm;
				
				if (glyph < numGlyph)
				{
					if (glyph < numHMetrics)
					{
						advance = this.GetAdvance (glyph);
					}
					else
					{
						advance = this.otHmtx.GetAdvanceWidth (numHMetrics-1);
					}
				}
				else if (glyph >= 0xff00)
				{
					advance += this.GetSpaceGlyphWidth (glyph) * perEm;
				}
				
				return advance * size / perEm;
			}
		}

		/// <summary>
		/// Gets the glyph bounds.
		/// </summary>
		/// <param name="glyph">The glyph.</param>
		/// <param name="size">The font point size.</param>
		/// <param name="xMin">X minimum.</param>
		/// <param name="xMax">X maximum.</param>
		/// <param name="yMin">Y minimum.</param>
		/// <param name="yMax">Y maximum.</param>
		public void GetGlyphBounds(ushort glyph, double size, out double xMin, out double xMax, out double yMin, out double yMax)
		{
			xMin = 0;
			xMax = 0;
			yMin = 0;
			yMax = 0;
			
			if (this.useSystemGlyphSize)
			{
				//	TODO: handle system font bounds information
			}
			else
			{
				int numGlyph = this.otMaxp.NumGlyphs;

				double perEm  = this.otHead.UnitsPerEm;
				double scale   = size / perEm;

				if (glyph < numGlyph)
				{
					int offset = -1;
					
					if (this.otLocaShort != null)
					{
						offset = this.otLocaShort.GetOffset (glyph);
					}
					else if (this.otLocaLong != null)
					{
						offset = this.otLocaLong.GetOffset (glyph);
					}
					else
					{
						//	TODO: handle fonts which don't have a 'loca' table. See below...
					}

					if (offset >= 0)
					{
						if (this.otGlyf != null)
						{
							TableGlyf otGlyf = new TableGlyf (this.otGlyf.BaseData, this.otGlyf.BaseOffset + offset);

							xMin = otGlyf.XMin * scale;
							xMax = otGlyf.XMax * scale;
							yMin = otGlyf.YMin * scale;
							yMax = otGlyf.YMax * scale;
						}
						else
						{
							//	TODO: handle fonts which don't have a 'glyf' table (such as "Warnock Pro")
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets the total width for the specified glyphs.
		/// </summary>
		/// <param name="glyphs">The glyphs.</param>
		/// <param name="size">The font point size.</param>
		/// <returns>The total width of the glyphs.</returns>
		public double GetTotalWidth(ushort[] glyphs, double size)
		{
			if (this.useSystemGlyphSize)
			{
				FontSizeInfo info = this.identity.GetSizeInfo ((int)(size + 0.5));
				
				int width = 0;
				
				for (int i = 0; i < glyphs.Length; i++)
				{
					if (glyphs[i] >= 0xff00)
					{
						width += (int) (this.GetSpaceGlyphWidth (glyphs[i]) * size + 0.5);
					}
					else
					{
						width += info.GetGlyphWidth (glyphs[i]);
					}
				}
				
				return width;
			}
			
			int numGlyph     = this.otMaxp.NumGlyphs;
			int numHMetrics = this.otHhea.NumHMetrics;
			
			int    advance    = 0;
			ushort prevGlyph = 0xffff;
			double perEm     = this.otHead.UnitsPerEm;
			
			for (int i = 0; i < glyphs.Length; i++)
			{
				ushort glyph = glyphs[i];
				int    delta;
				
				if (this.useKerning)
				{
					if (this.ApplyKerningInformation (glyph, ref prevGlyph, numGlyph, out delta))
					{
						advance += delta;
					}
				}
				
				if (glyph < numGlyph)
				{
					if (glyph < numHMetrics)
					{
						advance += this.GetAdvance (glyph);
					}
					else
					{
						advance += this.otHmtx.GetAdvanceWidth (numHMetrics-1);
					}
				}
				else if (glyph >= 0xff00)
				{
					advance += (int) (this.GetSpaceGlyphWidth (glyph) * perEm);
				}
			}
			
			return advance * size / perEm;
		}

		/// <summary>
		/// Gets the individual glyph positions.
		/// </summary>
		/// <param name="glyphs">The glyphs.</param>
		/// <param name="size">The font point size.</param>
		/// <param name="ox">The x origin for the first glyph.</param>
		/// <param name="xPos">The array of positions after every glyph; the array must
		/// be allocated by the caller.</param>
		/// <returns>The total width of the glyphs.</returns>
		public double GetPositions(ushort[] glyphs, double size, double ox, double[] xPos)
		{
			if (this.useSystemGlyphSize)
			{
				FontSizeInfo info = this.identity.GetSizeInfo ((int)(size + 0.5));
				
				int width = 0;
				
				for (int i = 0; i < glyphs.Length; i++)
				{
					xPos[i] = ox + width;
					
					if (glyphs[i] >= 0xff00)
					{
						width += (int) (this.GetSpaceGlyphWidth (glyphs[i]) * size + 0.5);
					}
					else
					{
						width += info.GetGlyphWidth (glyphs[i]);
					}
				}
				
				return width;
			}
			
			int numGlyph     = this.otMaxp.NumGlyphs;
			int numHMetrics = this.otHhea.NumHMetrics;
			
			int    advance    = 0;
			ushort prevGlyph = 0xffff;
			double perEm     = this.otHead.UnitsPerEm;
			
			for (int i = 0; i < glyphs.Length; i++)
			{
				ushort glyph = glyphs[i];
				int    delta;
				
				if (this.useKerning)
				{
					if (this.ApplyKerningInformation (glyph, ref prevGlyph, numGlyph, out delta))
					{
						advance  += delta;
					}
				}
				
				xPos[i] = ox + advance * size / perEm;
				
				if (glyph < numGlyph)
				{
					if (glyph < numHMetrics)
					{
						advance += this.GetAdvance (glyph);
					}
					else
					{
						advance += this.otHmtx.GetAdvanceWidth (numHMetrics-1);
					}
				}
				else if (glyph >= 0xff00)
				{
					advance += (int) (this.GetSpaceGlyphWidth (glyph) * perEm);
				}
			}
			
			return advance * size / perEm;
		}

		/// <summary>
		/// Gets the individual glyph positions. The scale is used to modify the glyph
		/// width (font glyph width + specified width adjustment); the glue is added
		/// independently of the scale.
		/// </summary>
		/// <param name="glyphs">The glyphs.</param>
		/// <param name="size">The font point size.</param>
		/// <param name="ox">The x origin for the first glyph.</param>
		/// <param name="xPos">The array of positions after every glyph which must
		/// be allocated by the caller.</param>
		/// <param name="xScale">The array of horizontal scales applied for every individual glyph or <c>null</c> if every glyph has a scale of <c>1</c>.</param>
		/// <param name="xAdjust">The array of horizontal width adjustement for every individual glyph or <c>null</c> if every glyph has an adjustement of <c>0</c>.</param>
		/// <param name="xGlue">The array of horizontal glue for every individual glyph or <c>null</c> if every glyph has a glue of <c>0</c>.</param>
		/// <returns>The total width of the glyphs.</returns>
		public double GetPositions(ushort[] glyphs, double size, double ox, double[] xPos, double[] xScale, double[] xAdjust, double[] xGlue)
		{
			if (this.useSystemGlyphSize)
			{
				FontSizeInfo info = this.identity.GetSizeInfo ((int)(size + 0.5));
				
				int width = 0;
				
				for (int i = 0; i < glyphs.Length; i++)
				{
					xPos[i] = ox + width;
					
					double adjust = (xAdjust == null) ? 0 : xAdjust[i];
					
					int glyphWidth;
					
					if (glyphs[i] >= 0xff00)
					{
						glyphWidth = (int) (this.GetSpaceGlyphWidth (glyphs[i]) * size + 0.5);
					}
					else
					{
						glyphWidth = info.GetGlyphWidth (glyphs[i]);
					}
					
					if (xGlue == null)
					{
						width += (int)((glyphWidth + adjust) * xScale[i] + 0.5);
					}
					else
					{
						width += (int)((glyphWidth + adjust) * xScale[i] + xGlue[i] + 0.5);
					}
				}
				
				return width;
			}
			
			int numGlyph     = this.otMaxp.NumGlyphs;
			int numHMetrics = this.otHhea.NumHMetrics;
			
			double perEm     = this.otHead.UnitsPerEm;
			double advance    = 0;
			ushort prevGlyph = 0xffff;
			
			for (int i = 0; i < glyphs.Length; i++)
			{
				ushort glyph = glyphs[i];
				int    delta;
				
				if (this.useKerning)
				{
					if (this.ApplyKerningInformation (glyph, ref prevGlyph, numGlyph, out delta))
					{
						advance += delta * xScale[i] * size / perEm;
					}
				}
				
				xPos[i] = ox + advance;
				
				if (glyph < numGlyph)
				{
					if (glyph < numHMetrics)
					{
						advance += this.GetAdvance (glyph) * xScale[i] * size / perEm;
					}
					else
					{
						advance += this.otHmtx.GetAdvanceWidth (numHMetrics-1) * xScale[i] * size / perEm;
					}
				}
				else if (glyph >= 0xff00)
				{
					advance += this.GetSpaceGlyphWidth (glyph) * xScale[i] * size;
				}
				
				if (xAdjust != null)
				{
					advance += xAdjust[i] * xScale[i];
				}
				
				if (xGlue != null)
				{
					advance += xGlue[i];
				}
			}
			
			return advance;
		}

		/// <summary>
		/// Gets the individual glyph positions.
		/// </summary>
		/// <param name="glyphs">The glyphs.</param>
		/// <param name="size">The font point size.</param>
		/// <param name="ox">The x origin for the first glyph.</param>
		/// <param name="oy">The y origin for the first glyph.</param>
		/// <param name="xPos">The array of horizontal positions after every glyph; the array must
		/// be allocated by the caller.</param>
		/// <param name="yPos">The array of baseline vertical positions for every glyph; the array must
		/// be allocated by the caller.</param>
		/// <returns>The total width of the glyphs.</returns>
		public double GetPositions(ushort[] glyphs, double size, double ox, double oy, double[] xPos, double[] yPos)
		{
			if (this.useSystemGlyphSize)
			{
				FontSizeInfo info = this.identity.GetSizeInfo ((int)(size + 0.5));
				
				int width = 0;
				
				for (int i = 0; i < glyphs.Length; i++)
				{
					xPos[i] = ox + width;
					yPos[i] = oy;
					
					if (glyphs[i] >= 0xff00)
					{
						width += (int) (this.GetSpaceGlyphWidth (glyphs[i]) * size + 0.5);
					}
					else
					{
						width += info.GetGlyphWidth (glyphs[i]);
					}
				}
				
				return width;
			}
			
			int numGlyph     = this.otMaxp.NumGlyphs;
			int numHMetrics = this.otHhea.NumHMetrics;
			
			double perEm     = this.otHead.UnitsPerEm;
			int    advance    = 0;
			ushort prevGlyph = 0xffff;
			
			for (int i = 0; i < glyphs.Length; i++)
			{
				ushort glyph = glyphs[i];
				int    delta;
				
				if (this.useKerning)
				{
					if (this.ApplyKerningInformation (glyph, ref prevGlyph, numGlyph, out delta))
					{
						advance  += delta;
					}
				}
				
				xPos[i] = ox + advance * size / perEm;
				yPos[i] = oy;
				
				if (glyph < numGlyph)
				{
					if (glyph < numHMetrics)
					{
						advance += this.GetAdvance (glyph);
					}
					else
					{
						advance += this.otHmtx.GetAdvanceWidth (numHMetrics-1);
					}
				} 
				else if (glyph >= 0xff00)
				{
					advance += (int) (this.GetSpaceGlyphWidth (glyph) * perEm);
				}
			}
			
			return advance * size / perEm;
		}


		/// <summary>
		/// Maps a text position into coordinates.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="size">The font point size.</param>
		/// <param name="pos">The position in the text expressed as an offet from the start of the string.</param>
		/// <param name="x">The X coordinate.</param>
		/// <param name="y">The Y coordinate.</param>
		/// <returns><c>true</c> if the position exists; otherwise, <c>false</c>.</returns>
		public bool HitTest(string text, double size, int pos, out double x, out double y)
		{
			if ((pos > text.Length) ||
				(pos < 0))
			{
				x = 0.0;
				y = 0.0;
				
				return false;
			}
			
			ushort[] glyphs;
			int[]    glMap;
			
			this.MapToGlyphs (text, out glyphs, out glMap);
			
			return this.HitTest (glyphs, glMap, size, pos, out x, out y);
		}

		/// <summary>
		/// Maps a text position into coordinates.
		/// </summary>
		/// <param name="text">The text encoded as 32-bit Unicode characters.</param>
		/// <param name="start">The start offset in the character array.</param>
		/// <param name="length">The length of the text.</param>
		/// <param name="size">The font point size.</param>
		/// <param name="pos">The position in the text expressed as an offet from the start of the string.</param>
		/// <param name="x">The X coordinate.</param>
		/// <param name="y">The Y coordinate.</param>
		/// <returns><c>true</c> if the position exists; otherwise, <c>false</c>.</returns>
		public bool HitTest(ulong[] text, int start, int length, double size, int pos, out double x, out double y)
		{
			if ((pos > length) ||
				(pos < 0))
			{
				x = 0.0;
				y = 0.0;
				
				return false;
			}
			
			ushort[] glyphs;
			int[]    glMap;
			
			this.MapToGlyphs (text, start, length, out glyphs, out glMap);
			
			return this.HitTest (glyphs, glMap, size, pos, out x, out y);
		}

		/// <summary>
		/// Maps coordinates to a text position.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="size">The font point size.</param>
		/// <param name="x">The X coordinate.</param>
		/// <param name="y">The Y coordinate.</param>
		/// <param name="pos">The position for the specified coordinates.</param>
		/// <param name="subpos">The sub-position relative to the position (0 means exact hit, 0.1 means
		/// that the coordinates are 10% farther than the position, etc.)</param>
		/// <returns><c>true</c> if the position exists; otherwise, <c>false</c>.</returns>
		public bool HitTest(string text, double size, double x, double y, out int pos, out double subpos)
		{
			ushort[] glyphs;
			int[]    glMap;
			
			this.MapToGlyphs (text, out glyphs, out glMap);
			
			return this.HitTest (glyphs, glMap, size, x, y, out pos, out subpos);
		}

		/// <summary>
		/// Maps coordinates to a text position.
		/// </summary>
		/// <param name="text">The text encoded as 32-bit Unicode characters.</param>
		/// <param name="start">The start offset in the character array.</param>
		/// <param name="length">The length of the text.</param>
		/// <param name="size">The font point size.</param>
		/// <param name="x">The X coordinate.</param>
		/// <param name="y">The Y coordinate.</param>
		/// <param name="pos">The position for the specified coordinates.</param>
		/// <param name="subpos">The sub-position relative to the position (0 means exact hit, 0.1 means
		/// that the coordinates are 10% farther than the position, etc.)</param>
		/// <returns><c>true</c> if the position exists; otherwise, <c>false</c>.</returns>
		public bool HitTest(ulong[] text, int start, int length, double size, double x, double y, out int pos, out double subpos)
		{
			ushort[] glyphs;
			int[]    glMap;
			
			this.MapToGlyphs (text, start, length, out glyphs, out glMap);
			
			return this.HitTest (glyphs, glMap, size, x, y, out pos, out subpos);
		}


		/// <summary>
		/// Selects a script for the default language.
		/// </summary>
		/// <param name="script">The OpenType script name.</param>
		public void SelectScript(string script)
		{
			this.SelectScript (script, "");
		}

		/// <summary>
		/// Selects the script for the specified language.
		/// </summary>
		/// <param name="script">The OpenType script name.</param>
		/// <param name="language">The OpenType script language.</param>
		public void SelectScript(string script, string language)
		{
			if ((this.activeScript == script) &&
				(this.activeLanguage == language))
			{
				return;
			}
			
			this.activeScript   = script;
			this.activeLanguage = language;
			
			if ((this.otGSUB != null) &&
				(this.otGSUB.ScriptListTable.ContainsScript (script)))
			{
				int   requiredFeature  = this.otGSUB.GetRequiredFeatureIndex (script, language);
				int[] optionalFeatures = this.otGSUB.GetFeatureIndexes (script, language);
				
				if (requiredFeature == 0xffff)
				{
					this.scriptRequiredFeature = null;
				}
				else
				{
					this.scriptRequiredFeature = this.otGSUB.FeatureListTable.GetTaggedFeatureTable (requiredFeature);
				}
				
				this.scriptOptionalFeatures = new TaggedFeatureTable[optionalFeatures.Length];
				
				for (int i = 0; i < optionalFeatures.Length; i++)
				{
					this.scriptOptionalFeatures[i] = this.otGSUB.FeatureListTable.GetTaggedFeatureTable (optionalFeatures[i]);
				}
			}
			else
			{
				this.scriptRequiredFeature  = null;
				this.scriptOptionalFeatures = null;
			}
			
			this.substitutionLookups = null;
			this.alternateLookups    = null;
		}

		/// <summary>
		/// Selects the specified OpenType font features.
		/// </summary>
		/// <param name="features">The OpenType font features (4 character strings such as
		/// <c>"kern"</c>, <c>"liga"</c>, etc. or <c>"Mgr=System"</c> and <c>"Mgr=OpenType"</c>
		/// to select one of the font managers).</param>
		public void SelectFeatures(params string[] features)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			for (int i = 0; i < features.Length; i++)
			{
				if ((features[i] != null) &&
					(features[i].Length > 0))
				{
					if (buffer.Length > 0)
					{
						buffer.Append ("/");
					}
					
					buffer.Append (features[i]);
				}
			}
			
			string collapsedFeatures = buffer.ToString ();
			
			if ((this.activeFeatures == collapsedFeatures) &&
				((this.substitutionLookups != null) || (this.otGSUB == null)))
			{
				return;
			}
			
			this.activeFeatures = collapsedFeatures;
			
			FeatureListTable             featureList    = this.otGSUB == null ? null : this.otGSUB.FeatureListTable;
			System.Collections.ArrayList activeFeatures = new System.Collections.ArrayList ();
			System.Collections.Hashtable activeNames    = new System.Collections.Hashtable ();
			
			for (int i = 0; i < features.Length; i++)
			{
				activeNames[features[i]] = null;
			}
			
			if (activeNames.Contains ("liga"))
			{
				this.mapDefaultLigatures = true;
			}
			else
			{
				this.mapDefaultLigatures = false;
			}
			
			if (activeNames.Contains ("kern"))
			{
				this.useKerning = true;
			}
			else
			{
				this.useKerning = false;
			}
			
			if (activeNames.Contains ("Mgr=System"))
			{
				this.SelectFontManager (FontManagerType.System);
			}
			else if (activeNames.Contains ("Mgr=OpenType"))
			{
				this.SelectFontManager (FontManagerType.OpenType);
			}
			
			if (this.scriptRequiredFeature != null)
			{
				activeFeatures.Add (this.scriptRequiredFeature);
			}
			
			if (this.scriptOptionalFeatures == null)
			{
				int n = featureList == null ? 0 : featureList.FeatureCount;
				
				for (int i = 0; i < n; i++)
				{
					if (activeNames.Contains (featureList.GetFeatureTag (i)))
					{
						activeFeatures.Add (featureList.GetFeatureTable (i));
						activeNames.Remove (featureList.GetFeatureTag (i));
					}
				}
			}
			else
			{
				int n = this.scriptOptionalFeatures.Length;
				
				for (int i = 0; i < n; i++)
				{
					if (activeNames.Contains (this.scriptOptionalFeatures[i].Tag))
					{
						activeFeatures.Add (this.scriptOptionalFeatures[i]);
					}
				}
			}
			
			if (this.otGSUB != null)
			{
				this.GenerateSubstitutionLookups (activeFeatures);
			}
		}

		/// <summary>
		/// Selects the font manager.
		/// </summary>
		/// <param name="manager">The font manager.</param>
		public void SelectFontManager(FontManagerType manager)
		{
			switch (manager)
			{
				case FontManagerType.OpenType:
					this.useSystemGlyphSize = false;
					break;
				
				case FontManagerType.System:
					this.useSystemGlyphSize = true;
					break;
			}
		}


		/// <summary>
		/// Saves the active features on an internal stack.
		/// </summary>
		public void PushActiveFeatures()
		{
			if (this.savedFeaturesStack == null)
			{
				this.savedFeaturesStack = new Stack<string> ();
			}
			
			this.savedFeaturesStack.Push (this.activeFeatures);
		}

		/// <summary>
		/// Disables some of the active features.
		/// </summary>
		/// <param name="features">The features which should be disabled.</param>
		public void DisableActiveFeatures(params string[] features)
		{
			string active  = this.activeFeatures;
			int    changes = 0;
			
			foreach (string feature in features)
			{
				if (active.IndexOf (feature) != -1)
				{
					active = active.Replace (feature, "");
					changes++;
				}
			}
			
			if (changes > 0)
			{
				this.SelectFeatures (active.Split ('/'));
			}
		}

		/// <summary>
		/// Restore the active features by popping them from an internal stack.
		/// </summary>
		public void PopActiveFeatures()
		{
			if (this.savedFeaturesStack != null)
			{
				string features = this.savedFeaturesStack.Pop ();
				this.SelectFeatures (features.Split ('/'));
			}
		}


		/// <summary>
		/// Gets the scripts supported by this font.
		/// </summary>
		/// <returns>An array of the scripts supported by this font.</returns>
		public string[] GetSupportedScripts()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			ScriptListTable scriptList = this.otGSUB == null ? null : this.otGSUB.ScriptListTable;
			int n = scriptList == null ? 0 : scriptList.ScriptCount;
			
			for (int i = 0; i < n; i++)
			{
				string      scriptTag   = scriptList.GetScriptTag (i);
				ScriptTable scriptTable = scriptList.GetScriptTable (i);
				
				if (i > 0)
				{
					buffer.Append ("|");
				}
				
				buffer.Append (scriptTag);
				
				int m = scriptTable.LangSysCount;
				
				for (int j = 0; j < m; j++)
				{
					buffer.Append ("|");
					buffer.Append (scriptTag);
					buffer.Append (":");
					buffer.Append (scriptTable.GetLangSysTag (j));
				}
			}
			
			return buffer.ToString ().Split ('|');
		}

		/// <summary>
		/// Gets the features supported by this font.
		/// </summary>
		/// <returns>An array of the features supported by this font.</returns>
		public string[] GetSupportedFeatures()
		{
			List<string> dict = new List<string> ();
			
			if (this.scriptOptionalFeatures == null)
			{
				FeatureListTable featureList = this.otGSUB == null ? null : this.otGSUB.FeatureListTable;
				
				int n = featureList == null ? 0 : featureList.FeatureCount;
				
				for (int i = 0; i < n; i++)
				{
					string tag = featureList.GetFeatureTag (i);
					
					if (dict.Contains (tag) == false)
					{
						dict.Add (tag);
					}
				}
			}
			else
			{
				int n = this.scriptOptionalFeatures.Length;
				
				for (int i = 0; i < n; i++)
				{
					string tag = this.scriptOptionalFeatures[i].Tag;
					
					if (dict.Contains (tag) == false)
					{
						dict.Add (tag);
					}
				}
			}
			
			//	Ajoute des features "synthétiques" comme le crénage et les ligatures
			//	que nous savons émuler au besoin :
			
			if (otKernFormat0 != null)
			{
				if (dict.Contains ("kern") == false)
				{
					dict.Add ("kern");
				}
			}
			
			if (dict.Contains ("liga") == false)
			{
				dict.Add ("liga");
			}
			
			dict.Sort ();
			return dict.ToArray ();
		}


		/// <summary>
		/// Checks if this OpenType font supports a given feature.
		/// </summary>
		/// <param name="feature">The feature to check.</param>
		/// <returns><c>true</c> if the feature is supported; otherwise, <c>false</c>.</returns>
		public bool SupportsFeature(string feature)
		{
			string[] features = this.GetSupportedFeatures ();
			
			for (int i = 0; i < features.Length; i++)
			{
				if (features[i] == feature)
				{
					return true;
				}
			}
			
			return false;
		}


		/// <summary>
		/// Gets the lookup tables for the specified features.
		/// </summary>
		/// <param name="features">The features.</param>
		/// <returns></returns>
		public LookupTable[] GetLookupTables(params string[] features)
		{
			List<LookupTable> list = new List<LookupTable> ();
			
			if (this.otGSUB != null)
			{
				foreach (string feature in features)
				{
					int[] indexes = this.otGSUB.GetFeatureIndexes (feature);
					foreach (int index in indexes)
					{
						FeatureTable fTable = this.otGSUB.FeatureListTable.GetFeatureTable (index);
						int          count   = fTable.LookupCount;
						
						for (int i = 0; i < count; i++)
						{
							list.Add (this.otGSUB.LookupListTable.GetLookupTable (fTable.GetLookupIndex (i)));
						}
					}
				}
			}
			
			return list.ToArray ();
		}


		/// <summary>
		/// Gets the alternates for a given glyph (one glyph can have different
		/// representations which are encoded as <i>alternates</i>).
		/// </summary>
		/// <param name="glyph">The glyph.</param>
		/// <param name="alternates">The array of alternates.</param>
		/// <returns><c>true</c> if alternates exist; otherwise, <c>false</c>.</returns>
		public bool GetAlternates(ushort glyph, out ushort[] alternates)
		{
			if (this.alternateLookups == null)
			{
				List<BaseSubstitution> list = new List<BaseSubstitution> ();
				
				foreach (string feature in this.activeFeatures.Split ('/'))
				{
					LookupTable[] tables = this.GetLookupTables (feature);
					
					foreach (LookupTable table in tables)
					{
						if (table.LookupType == Common.OpenType.LookupType.Alternate)
						{
							for (int i = 0; i < table.SubTableCount; i++)
							{
								list.Add (new AlternateSubstitution (table.GetSubTable (i)));
							}
						}
						
						if (table.LookupType == Common.OpenType.LookupType.Single)
						{
							for (int i = 0; i < table.SubTableCount; i++)
							{
								list.Add (new SingleSubstitution (table.GetSubTable (i)));
							}
						}
					}
				}

				this.alternateLookups = list.ToArray ();
			}
			
			if (this.alternateLookups != null)
			{
				List<ushort> list = null;
				
				foreach (BaseSubstitution subst in this.alternateLookups)
				{
					if (subst.Coverage.FindIndex (glyph) >= 0)
					{
						if (list == null)
						{
							list = new List<ushort> ();
						}
						
						AlternateSubstitution alternate = subst as AlternateSubstitution;
						SingleSubstitution    single    = subst as SingleSubstitution;
						
						if (alternate != null)
						{
							ushort[] subset = alternate.GetAlternates (glyph);
							
							System.Diagnostics.Debug.Assert (subset != null);
							System.Diagnostics.Debug.Assert (subset.Length > 0);
							
							for (int i = 0; i < subset.Length; i++)
							{
								ushort replace = subset[i];
								
								if (list.Contains (replace) == false)
								{
									list.Add (replace);
								}
							}
						}
						if (single != null)
						{
							ushort replace = single.FindSubstitution (glyph);
							
							if (list.Contains (replace) == false)
							{
								list.Add (replace);
							}
						}
					}
				}

				if (list != null)
				{
					alternates = list.ToArray ();
					return true;
				}
			}
			
			alternates = null;
			return false;
		}


		/// <summary>
		/// Gets the operating system font handle; this works only if the system
		/// font manager is currently active.
		/// </summary>
		/// <param name="size">The font point size.</param>
		/// <returns>The handle to the font or <c>System.IntPtr.Zero</c>.</returns>
		public System.IntPtr GetFontHandle(double size)
		{
			if (this.useSystemGlyphSize)
			{
				FontSizeInfo info = this.identity.GetSizeInfo ((int)(size + 0.5));
				return info.Handle == null ? System.IntPtr.Zero : info.Handle.Handle;
			}
			
			return System.IntPtr.Zero;
		}


		/// <summary>
		/// Gets the operating system font handle for the "em" size, which is the
		/// natural size used when designing the font.
		/// </summary>
		/// <returns>The handle to the font or <c>System.IntPtr.Zero</c>.</returns>
		public System.IntPtr GetFontHandleAtEmSize()
		{
			FontSizeInfo info = this.identity.GetSizeInfo (this.otHead.UnitsPerEm);
            return info.Handle == null ? System.IntPtr.Zero : info.Handle.Handle;
		}


		/// <summary>
		/// Gets the font ascender.
		/// </summary>
		/// <param name="size">The font point size.</param>
		/// <returns>The font ascender.</returns>
		public double GetAscender(double size)
		{
			if (this.useSystemGlyphSize)
			{
				FontSizeInfo info = this.identity.GetSizeInfo ((int)(size + 0.5));
				return info.Ascender;
			}
			else
			{
				double scale = size / this.otHead.UnitsPerEm;
				
				return this.otHhea.MacAscender * scale;
			}
		}

		/// <summary>
		/// Gets the font descender.
		/// </summary>
		/// <param name="size">The font point size.</param>
		/// <returns>The font descender.</returns>
		public double GetDescender(double size)
		{
			if (this.useSystemGlyphSize)
			{
				FontSizeInfo info = this.identity.GetSizeInfo ((int)(size + 0.5));
				return info.Descender;
			}
			else
			{
				double scale = size / this.otHead.UnitsPerEm;
				
				return this.otHhea.MacDescender * scale;
			}
		}

		/// <summary>
		/// Gets the font line gap.
		/// </summary>
		/// <param name="size">The font point size.</param>
		/// <returns>The font line gap.</returns>
		public double GetLineGap(double size)
		{
			double scale = size / this.otHead.UnitsPerEm;
			return this.otHhea.MacLineGap * scale;
		}

		/// <summary>
		/// Gets the font caret angle in radians.
		/// </summary>
		/// <returns>The font caret angle in radians.</returns>
		public double GetCaretAngleRad()
		{
			double dx = this.otHhea.CaretSlopeRun;
			double dy = this.otHhea.CaretSlopeRise;
			
			return System.Math.Atan2 (dy, dx);
		}

		/// <summary>
		/// Gets the font caret angle in degrees.
		/// </summary>
		/// <returns>The font caret angle in degrees.</returns>
		public double GetCaretAngleDeg()
		{
			return this.GetCaretAngleRad () * 180.0 / System.Math.PI;
		}


		public double GetXHeight(double size)
		{
			double scale = size / this.otHead.UnitsPerEm;
			TableOS2 os2 = this.GetTableOS2 ();
			double xHeight = scale * os2.XHeight;

			if (xHeight == 0)
			{
				double xMin, xMax, yMin, yMax;
				ushort glyph = this.GetGlyphIndex ('o');

				this.GetGlyphBounds (glyph, size, out xMin, out xMax, out yMin, out yMax);

				xHeight = yMax + yMin;
			}

			return xHeight;
		}

		public double GetCapHeight(double size)
		{
			double scale = size / this.otHead.UnitsPerEm;
			TableOS2 os2 = this.GetTableOS2 ();
			double capHeight = scale * os2.CapHeight;

			if (capHeight == 0)
			{
				double xMin, xMax, yMin, yMax;
				ushort glyph = this.GetGlyphIndex ('O');

				this.GetGlyphBounds (glyph, size, out xMin, out xMax, out yMin, out yMax);

				capHeight = yMax + yMin;
			}

			return capHeight;
		}

		private TableOS2 GetTableOS2()
		{
			return new TableOS2 (this.fontData["OS/2"]);
		}


		/// <summary>
		/// Gets the font maximum box.
		/// </summary>
		/// <param name="size">The font point size.</param>
		/// <param name="xMin">The minimum X value.</param>
		/// <param name="xMax">The maximum X value.</param>
		/// <param name="yMin">The minimum Y value.</param>
		/// <param name="yMax">The maximum Y value.</param>
		public void GetMaxBox(double size, out double xMin, out double xMax, out double yMin, out double yMax)
		{
			double scale = size / this.otHead.UnitsPerEm;
			
			xMin = this.otHead.XMin * scale;
			xMax = this.otHead.XMax * scale;
			yMin = this.otHead.YMin * scale;
			yMax = this.otHead.YMax * scale;
		}

		/// <summary>
		/// Gets the maximum box for a glyph string.
		/// </summary>
		/// <param name="glyphs">The glyphs.</param>
		/// <param name="size">The font point size.</param>
		/// <param name="x">The X coordinate for each glyph.</param>
		/// <param name="y">The Y coordinate for each glyph.</param>
		/// <param name="sx">The X scale for each glyph or <c>null</c>.</param>
		/// <param name="sy">The Y scale for each glyph or <c>null</c>.</param>
		/// <param name="xMin">The minimum X value.</param>
		/// <param name="xMax">The maximum X value.</param>
		/// <param name="yMin">The minimum Y value.</param>
		/// <param name="yMax">The maximum Y value.</param>
		public void GetMaxBox(ushort[] glyphs, double size, double[] x, double[] y, double[] sx, double[] sy, out double xMin, out double xMax, out double yMin, out double yMax)
		{
			System.Diagnostics.Debug.Assert (glyphs.Length > 0);
			
			double scale = size / this.otHead.UnitsPerEm;
			
			double otXmin = this.otHead.XMin * scale;
			double otXmax = this.otHead.XMax * scale;
			double otYmin = this.otHead.YMin * scale;
			double otYmax = this.otHead.YMax * scale;
			
			xMin = x[0] + otXmin * (sx == null ? 1 : sx[0]);
			xMax = x[0] + otXmax * (sx == null ? 1 : sx[0]);
			yMin = y[0] + otYmin * (sy == null ? 1 : sy[0]);
			yMax = y[0] + otYmax * (sy == null ? 1 : sy[0]);
			
			for (int i = 1; i < glyphs.Length; i++)
			{
				double xxMin = x[i] + otXmin * (sx == null ? 1 : sx[i]);
				double xxMax = x[i] + otXmax * (sx == null ? 1 : sx[i]);
				double yyMin = y[i] + otYmin * (sy == null ? 1 : sy[i]);
				double yyMax = y[i] + otYmax * (sy == null ? 1 : sy[i]);
				
				if (xxMin < xMin) xMin = xxMin;
				if (xxMax > xMax) xMax = xxMax;
				if (yyMin < yMin) yMin = yyMin;
				if (yyMax > yMax) yMax = yyMax;
			}
		}


		/// <summary>
		/// Gets the glyph index for the specified character code.
		/// </summary>
		/// <param name="code">The character code.</param>
		/// <returns>The glyph index.</returns>
		public ushort GetGlyphIndex(char code)
		{
			return this.GetGlyphIndex ((int) code);
		}

		/// <summary>
		/// Gets the glyph index for the specified character code.
		/// </summary>
		/// <param name="code">The character code.</param>
		/// <returns>The glyph index or <c>0xffff</c> the mapping failed.</returns>
		public ushort GetGlyphIndex(int code)
		{
			ushort glyph;
			
			if (code < Font.GlyphCacheSize)
			{
				glyph = this.glyphCache[code];
				
				if (glyph != 0)
				{
					return glyph;
				}

				if ((this.glyphCacheOthers != null) &&
					(this.glyphCacheOthers.TryGetValue (code, out glyph)))
				{
					return glyph;
				}
			}
			
			if ((code == 0) ||
				(this.otIndexMapping == null))
			{
				return 0xffff;
			}
			
			if (code <= 0x0003)
			{
				//	Start of text and end of text have no graphic representation.
				//	Avoid walking through the font, looking for glyphs :
				
				return 0x0000;
			}
			
			glyph = this.otIndexMapping.GetGlyphIndex (code);
			
			if (glyph == 0x0000)
			{
				//	The font does not contain a glyph definition for this character.
				//	We still handle some characters such as the variants of the space
				//	character (which we graphically map to a special glyph) and of
				//	the dash/hyphen.
				
				switch (code)
				{
					case 0x2000: glyph = 0xff01; break;			//	1/2 em
					case 0x2001: glyph = 0xff00; break;			//	1 em
					case 0x2002: glyph = 0xff01; break;			//	1/2 em
					case 0x2003: glyph = 0xff00; break;			//	1 em
					case 0x2004: glyph = 0xff02; break;			//	1/3 em
					case 0x2005: glyph = 0xff03; break;			//	1/4 em
					case 0x2006: glyph = 0xff06; break;			//	1/6 em
					case 0x2007: glyph = 0xff0a; break;			//	'0' (digit)
					case 0x2008: glyph = 0xff09; break;			//	'.' (narrow punctuation)
					case 0x2009: glyph = 0xff05; break;			//	1/5 em
					case 0x200A: glyph = 0xff07; break;			//	1/16 em
					case 0x200B: glyph = 0xff08; break;			//	zero width
					case 0x200C: glyph = 0xff08; break;			//	zero width
					case 0x200D: glyph = 0xff08; break;			//	zero width
					
					case 0x202F: glyph = 0xff0b; break;			//	narrow space
					case 0x205F: glyph = 0xff04; break;			//	4/18 em
					case 0x2060: glyph = 0xff08; break;			//	zero width
					
					case 0x00A0:
						glyph = this.SpaceGlyph;
						break;
					
					case 0x2010:		//	Hyphen
					case 0x2011:		//	Non Breaking Hyphen
					case 0x00AD:		//	Soft Hyphen
					case 0x1806:		//	Mongolian Todo Hyphen
						glyph = this.HyphenGlyph;
						break;
				}
			}

			if (code < Font.GlyphCacheSize)
			{
				this.glyphCache[code] = glyph;
			}
			else
			{
				if (this.glyphCacheOthers == null)
				{
					this.glyphCacheOthers = new Dictionary<int, ushort> ();
				}

				this.glyphCacheOthers[code] = glyph;
			}
			
			return glyph;
		}

		public static bool IsStretchableSpaceCharacter(int code)
		{
			switch (code)
			{
				case 0x200B:			//	zero width (expandable)
				case 0x202F:			//	narrow no-break space
				case 0x0020:			//	space
				case 0x00A0:			//	no-break space
					return true;
			}

			return false;
		}

		public static bool IsSpaceCharacter(int code)
		{
			switch (code)
			{
				case 0x2000:			//	1/2 em
				case 0x2001:			//	1 em
				case 0x2002:			//	1/2 em
				case 0x2003:			//	1 em
				case 0x2004:			//	1/3 em
				case 0x2005:			//	1/4 em
				case 0x2006:			//	1/6 em
				case 0x2007:			//	'0' (digit)
				case 0x2008:			//	'.' (narrow punctuation)
				case 0x2009:			//	1/5 em
				case 0x200A:			//	1/16 em
				case 0x200B:			//	zero width (expandable)
				case 0x200C:			//	zero width
				case 0x200D:			//	zero width

				case 0x202F:			//	narrow no-break space
				case 0x205F:			//	4/18 em
				case 0x2060:			//	zero width

				case 0x0020:			//	space
				case 0x00A0:			//	no-break space
					return true;
			}

			return false;
		}


		internal double GetSpaceGlyphWidth(ushort glyph)
		{
			if (glyph >= 0xff00)
			{
				switch (glyph)
				{
					case 0xff00:
						return this.EmWidth;
					case 0xff01:
						return this.EmWidth / 2;
					case 0xff02:
						return this.EmWidth / 3;
					case 0xff03:
						return this.EmWidth / 4;
					case 0xff04:
						return this.EmWidth * 4 / 18;
					case 0xff05:
						return this.EmWidth / 5;
					case 0xff06:
						return this.EmWidth / 6;
					case 0xff07:
						return this.EmWidth / 16;
					case 0xff08:
						return 0;
					case 0xff09:
						return this.PeriodWidth;
					case 0xff0a:
						return this.FigureWidth;
					case 0xff0b:
						return this.SpaceWidth / 2;
				}
			}

			return 0;
		}
		
		
		private void MapToGlyphs(string text, out ushort[] glyphs, out int[] glMap)
		{
			int length = text.Length;
			
			glyphs = new ushort[length];
			glMap = new int[length];
			
			for (int i = 0; i < length; i++)
			{
				glyphs[i] = this.GetGlyphIndex (text[i]);
			}
			
			this.ApplySubstitutions (ref glyphs, ref glMap);
		}
		
		private void MapToGlyphs(ulong[] text, int start, int length, out ushort[] glyphs, out int[] glMap)
		{
			glyphs = new ushort[length];
			glMap = new int[length];
			
			for (int i = 0; i < length; i++)
			{
				ulong bits = text[start+i];
				int   code = Font.UnicodeMask & (int) bits;
				
				glyphs[i] = (bits & Font.TransparentGlyphFlag) == 0 ? this.GetGlyphIndex (code) : (ushort) code;
			}
			
			this.ApplySubstitutions (ref glyphs, ref glMap);
		}
		
		
		private void GenerateSubstitutionLookups(System.Collections.ICollection featureTables)
		{
			System.Collections.ArrayList lookupIndexes = new System.Collections.ArrayList ();
			
			foreach (FeatureTable featureTable in featureTables)
			{
				int n = featureTable.LookupCount;
				
				for (int i = 0; i < n; i++)
				{
					int lookup = featureTable.GetLookupIndex (i);
					
					if (lookupIndexes.Contains (lookup) == false)
					{
						lookupIndexes.Add (lookup);
					}
				}
			}
			
			lookupIndexes.Sort ();
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			if (this.otGSUB != null)
			{
				foreach (int lookup in lookupIndexes)
				{
					LookupTable lookupTable = this.otGSUB.LookupListTable.GetLookupTable (lookup);
					
					int n = lookupTable.SubTableCount;
					
					for (int i = 0; i < n; i++)
					{
						BaseSubstitution subst = lookupTable.GetSubstitution (i);
						
						if (subst != null)
						{
							list.Add (subst);
						}
					}
				}
			}
			
			this.substitutionLookups = new BaseSubstitution[list.Count];
			list.CopyTo (this.substitutionLookups);
		}
		
		
		private void ApplySubstitutions(ref ushort[] glyphs, ref int[] glMap)
		{
			if ((this.substitutionLookups == null) ||
				(this.substitutionLookups.Length == 0))
			{
				if (this.mapDefaultLigatures == false)
				{
					return;
				}
			}

			int count = glyphs.Length;
			
			//	Exécute les substitutions de glyphes en fonction des 'features'
			//	sélectionnées :
			
			int maxSize = count + 16;
			
		try_again:
			
			if (Font.tempGlyphMaxSize < maxSize)
			{
				Font.tempGlyphs = new ushort[2][];
				Font.tempGlyphs[0] = new ushort[maxSize];
				Font.tempGlyphs[1] = new ushort[maxSize];
				Font.tempGlyphMaxSize = maxSize;
			}

			ushort[][] temp = Font.tempGlyphs;
			
			try
			{
				ushort[] input  = glyphs;
				ushort[] output = temp[0];
				
				int length = count;
				int toggle = 1;
				
				if ((this.substitutionLookups != null) &&
					(this.substitutionLookups.Length > 0))
				{
					for (int i = 0; i < this.substitutionLookups.Length; i++)
					{
						Font.ApplySubstitutions (this.substitutionLookups[i], input, length, output, out length, ref glMap);
						
						input   = output;
						output  = temp[toggle & 1];
						toggle += 1;
					}
				}
				
				if (this.mapDefaultLigatures)
				{
					this.ApplyManualLigatureSubstitutions (input, length, output, out length, ref glMap);
				}
				else
				{
					output = input;
				}

				if (glyphs.Length != length)
				{
					glyphs = new ushort[length];
				}
				
				for (int i = 0; i < length; i++)
				{
					glyphs[i] = output[i];
				}
			}
			catch (System.IndexOutOfRangeException)
			{
				maxSize += maxSize / 8;
				goto try_again;
			}
		}
		
		private void ApplyManualLigatureSubstitutions(ushort[] inputGlyphs, int inputLength, ushort[] outputGlyphs, out int outputLength, ref int[] glMap)
		{
			if (this.otIndexMapping == null)
			{
				for (int i = 0; i < inputLength; i++)
				{
					outputGlyphs[i] = inputGlyphs[i];
				}
				
				outputLength = inputLength;
				
				return;
			}
			
			int inputOffset  = 0;
			int outputOffset = 0;
			
			while (inputOffset < inputLength)
			{
				int length = inputLength - inputOffset;
				
				if (length > 2)
				{
					if ((this.glyphLigFfi > 0) &&
						(inputGlyphs[inputOffset+0] == this.glyphF) &&
						(inputGlyphs[inputOffset+1] == this.glyphF) &&
						(inputGlyphs[inputOffset+2] == this.glyphI))
					{
						outputGlyphs[outputOffset] = this.glyphLigFfi;
						
						if (glMap != null)
						{
							glMap[outputOffset] = glMap[inputOffset] + 2;
						}
						
						inputOffset  += 3;
						outputOffset += 1;
						
						continue;
					}

					if ((this.glyphLigFfl > 0) &&
						(inputGlyphs[inputOffset+0] == this.glyphF) &&
						(inputGlyphs[inputOffset+1] == this.glyphF) &&
						(inputGlyphs[inputOffset+2] == this.glyphL))
					{
						outputGlyphs[outputOffset] = this.glyphLigFfl;
						
						if (glMap != null)
						{
							glMap[outputOffset] = glMap[inputOffset] + 2;
						}
						
						inputOffset  += 3;
						outputOffset += 1;
						
						continue;
					}
				}
				
				if (length > 1)
				{
					if ((this.glyphLigFf > 0) &&
						(inputGlyphs[inputOffset+0] == this.glyphF) &&
						(inputGlyphs[inputOffset+1] == this.glyphF))
					{
						outputGlyphs[outputOffset] = this.glyphLigFf;
						
						if (glMap != null)
						{
							glMap[outputOffset] = glMap[inputOffset] + 1;
						}
						
						inputOffset  += 2;
						outputOffset += 1;
						
						continue;
					}

					if ((this.glyphLigFi > 0) &&
						(inputGlyphs[inputOffset+0] == this.glyphF) &&
						(inputGlyphs[inputOffset+1] == this.glyphI))
					{
						outputGlyphs[outputOffset] = this.glyphLigFi;
						
						if (glMap != null)
						{
							glMap[outputOffset] = glMap[inputOffset] + 1;
						}
						
						inputOffset  += 2;
						outputOffset += 1;
						
						continue;
					}

					if ((this.glyphLigFl > 0) &&
						(inputGlyphs[inputOffset+0] == this.glyphF) &&
						(inputGlyphs[inputOffset+1] == this.glyphL))
					{
						outputGlyphs[outputOffset] = this.glyphLigFl;
						
						if (glMap != null)
						{
							glMap[outputOffset] = glMap[inputOffset] + 1;
						}
						
						inputOffset  += 2;
						outputOffset += 1;
						
						continue;
					}
				}
				
				outputGlyphs[outputOffset] = inputGlyphs[inputOffset];
				
				if (glMap != null)
				{
					glMap[outputOffset] = glMap[inputOffset];
				}
						
				inputOffset  += 1;
				outputOffset += 1;
			}
			
			outputLength = outputOffset;
		}
		
		private static void ApplySubstitutions(BaseSubstitution substitution, ushort[] inputGlyphs, int inputLength, ushort[] outputGlyphs, out int outputLength, ref int[] glMap)
		{
			int inputOffset  = 0;
			int outputOffset = 0;
			
			if (glMap == null)
			{
				while (inputOffset < inputLength)
				{
					if (substitution.ProcessSubstitution (inputGlyphs, ref inputOffset, inputLength, outputGlyphs, ref outputOffset))
					{
						//	Substitution avec succès.
					}
					else
					{
						outputGlyphs[outputOffset] = inputGlyphs[inputOffset];
						
						inputOffset++;
						outputOffset++;
					}
				}
			}
			else
			{
				while (inputOffset < inputLength)
				{
					int inputOrigin  = inputOffset;
					int outputOrigin = outputOffset;
					
					if (substitution.ProcessSubstitution (inputGlyphs, ref inputOffset, inputLength, outputGlyphs, ref outputOffset))
					{
						//	Substitution avec succès.
						
						int inputAdvance  = inputOffset  - inputOrigin;
						int outputAdvance = outputOffset - outputOrigin;
						
						Debug.Assert.IsTrue (inputAdvance > 0);
						Debug.Assert.IsTrue (outputAdvance == 1);
						
						glMap[outputOrigin++] = glMap[inputOrigin++] + inputAdvance - outputAdvance;
						
						//	Attention, quand les substitutions pourront générer
						//	plusieurs glyphes en sortie, ce code devra être
						//	adapté.
					}
					else
					{
						outputGlyphs[outputOffset] = inputGlyphs[inputOffset];
						glMap[outputOffset]        = glMap[inputOffset];
						
						inputOffset++;
						outputOffset++;
					}
				}
			}
			
			outputLength = outputOffset;
		}
		
		private bool ApplyKerningInformation(ushort glyph, ref ushort prevGlyph, int numGlyph, out int delta)
		{
			if ((this.useKerning) &&
				(glyph < numGlyph) &&
				(prevGlyph < numGlyph) &&
				(this.otKernFormat0 != null) &&
				(this.otKernFormat0.FindKernValue (prevGlyph, glyph, out delta)))
			{
				prevGlyph = glyph;
				return true;
			}
			else
			{
				delta      = 0;
				prevGlyph = glyph;
				return false;
			}
		}
		
		
		private bool HitTest(ushort[] glyphs, int[] glMap, double size, int pos, out double x, out double y)
		{
			//	TODO: gérer useSystemGlyphSize
			
			double scale = size / this.otHead.UnitsPerEm;
			
			int numGlyph     = this.otMaxp.NumGlyphs;
			int numHMetrics = this.otHhea.NumHMetrics;
			
			int    advance    = 0;
			int    distance   = 0;
			ushort prevGlyph = 0xffff;
			
			for (int i = 0; i < glyphs.Length; i++)
			{
				ushort glyph = glyphs[i];
				int    delta;
				
				if (this.useKerning)
				{
					if (this.ApplyKerningInformation (glyph, ref prevGlyph, numGlyph, out delta))
					{
						advance += delta;
					}
				}
				
				if ((distance <= pos) &&
					(distance+glMap[i] >= pos))
				{
					//	Trouve la position [x] exacte si c'est dans une ligature.
					
					if (glMap[i] > 0)
					{
						this.GetLigatureCaretPosition (glyph, glMap[i]+1, size, pos - distance, out x, out y);
						
						x += advance * scale;
						y += 0.0;
					}
					else
					{
						x = advance * scale;
						y = 0.0;
					}
					
					return true;
				}
				
				if (glyph < numGlyph)
				{
					if (glyph < numHMetrics)
					{
						advance += this.GetAdvance (glyph);
					}
					else
					{
						advance += this.otHmtx.GetAdvanceWidth (numHMetrics-1);
					}
				}
				
				distance += 1;
				distance += glMap[i];
			}
			
			x = advance * scale;
			y = 0.0;
			
			return true;
		}
		
		private bool HitTest(ushort[] glyphs, int[] glMap, double size, double x, double y, out int pos, out double subpos)
		{
			//	TODO: gérer useSystemGlyphSize
			
			if (x <= 0)
			{
				pos    = 0;
				subpos = 0;
				
				return false;
			}
			
			ushort[] oneGlyph  = new ushort[1];
			int[]    oneGlMap = new int[1];
			
			int count = 0;
			
			for (int i = 0; i < glyphs.Length; i++)
			{
				oneGlyph[0]  = glyphs[i];
				oneGlMap[0] = glMap[i];
				
				double x1, x2;
				double y1, y2;
				
				x2 = 0;
				
				for (int onePos = 0; onePos < glMap[i]+1; onePos++)
				{
					this.HitTest (oneGlyph, oneGlMap, size, onePos + 0, out x1, out y1);
					this.HitTest (oneGlyph, oneGlMap, size, onePos + 1, out x2, out y2);
					
					if ((x >= x1) &&
						(x <= x2))
					{
						double width = x2 - x1;

						pos    = count;
						subpos = (width > 0.0) ? (x - x1) / width : 0.0;
						
						//	A partir de 50%, considère que l'on est dans le glyphe
						//	suivant :
						
						if (subpos > 0.5)
						{
							pos    += 1;
							subpos -= 1.0;
						}
						
						return true;
					}
					
					count++;
				}
				
				x -= x2;
			}
			
			pos    = count;
			subpos = 0;
			
			return false;
		}
		
		
		private void GetLigatureCaretPosition(ushort glyph, int simpleGlyphCount, double size, int pos, out double x, out double y)
		{
			if (pos < 1)
			{
				x = 0;
				y = 0;
				
				return;
			}
			
			double scale = size / this.otHead.UnitsPerEm;
			
			LigatureCaretListTable caretList = this.otGDEF == null ? null : this.otGDEF.LigatureCaretListTable;
			
			if (caretList != null)
			{
				int index = caretList.Coverage.FindIndex (glyph);
				
				if (index >= 0)
				{
					LigatureGlyphTable ligTable = caretList.GetLigatureGlyphTable (index);
					
					int caretCount = ligTable.CaretCount;
					int caretIndex = pos - 1;
					
					if (caretIndex < caretCount)
					{
						switch (ligTable.GetCaretValueFormat (caretIndex))
						{
							case 1:
								x = ligTable.GetCaretCoordinateFmt1 (caretIndex) * scale;
								y = 0;
								return;
							
							case 2:
								//	TODO: gérer...
								break;
							
							case 3:
								x = ligTable.GetCaretCoordinateFmt3 (caretIndex) * scale;
								y = 0;
								
								//	TODO: gérer la "device table"
								
								return;
						}
					}
				}
			}
			
			int numGlyph     = this.otMaxp.NumGlyphs;
			int numHMetrics = this.otHhea.NumHMetrics;
			int advance       = 0;
			
			if (glyph < numGlyph)
			{
				if (glyph < numHMetrics)
				{
					advance = this.GetAdvance (glyph);
				}
				else
				{
					advance = this.otHmtx.GetAdvanceWidth (numHMetrics-1);
				}
			}
			
			x = advance * pos * scale / simpleGlyphCount;
			y = 0.0;
		}
		
		private int GetAdvance(int glyph)
		{
			int advance;
			
			if (glyph < Font.AdvanceCacheSize)
			{
				advance = this.advanceCache[glyph];
				
				if (advance != 0)
				{
					return advance;
				}
			}
			
			advance = this.otHmtx.GetAdvanceWidth (glyph);
			
			if (glyph < Font.AdvanceCacheSize)
			{
				System.Diagnostics.Debug.Assert (advance >= 0);
				
				this.advanceCache[glyph] = (ushort) advance;
			}
			
			return advance;
		}
		
		private static int[] GetCoverageIndexes(Coverage coverage)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			for (int i = 0; i < 0xffff; i++)
			{
				int index = coverage.FindIndex (i);
				
				if (index != -1)
				{
					list.Add (i);
				}
			}
			
			return (int[]) list.ToArray (typeof (int));
		}
		
		
		private const int						UnicodeMask				= 0x001fffff;
		private const int						TransparentGlyphFlag	= 0x00800000;
		
		private FontIdentity					identity;
		private FontData						fontData;
		
		private TableGSUB						otGSUB;
		private TableGDEF						otGDEF;
		private TableCmap						otCmap;
		private TableMaxp						otMaxp;
		private TableHead						otHead;
		private TableHhea						otHhea;
		private Tablehmtx						otHmtx;
		private TableLocaShort				otLocaShort;
		private TableLocaLong					otLocaLong;
		private TableGlyf						otGlyf;
		private KerningTableFormat0				otKernFormat0;
		private IndexMappingTable				otIndexMapping;
		private FontType						fontType;
		
		private ushort glyphF;
		private ushort glyphI;
		private ushort glyphL;
		private ushort glyphLigFf;
		private ushort glyphLigFi;
		private ushort glyphLigFl;
		private ushort glyphLigFfi;
		private ushort glyphLigFfl;
		
		private string							activeScript;
		private string							activeLanguage;
		private string							activeFeatures = "";
		
		private bool							mapDefaultLigatures;
		private bool							useKerning;
		private bool							useSystemGlyphSize;
		
		private TaggedFeatureTable				scriptRequiredFeature;
		private TaggedFeatureTable[]			scriptOptionalFeatures;
		private BaseSubstitution[]				substitutionLookups;
		private BaseSubstitution[]				alternateLookups;
		
		private Stack<string>					savedFeaturesStack;
		private ushort							spaceGlyph;
		private ushort							hyphenGlyph;
		private ushort							ellipsisGlyph;
		private ushort							periodGlyph;
		private ushort[]						glyphCache;
		private Dictionary<int, ushort>			glyphCacheOthers;
		private ushort[]						advanceCache;
		
		private const int						GlyphCacheSize = 256;
		private const int						AdvanceCacheSize = 256;

		[System.ThreadStatic]
		private static ushort[][]				tempGlyphs;
		
		[System.ThreadStatic]
		private static int						tempGlyphMaxSize;
	}
}
