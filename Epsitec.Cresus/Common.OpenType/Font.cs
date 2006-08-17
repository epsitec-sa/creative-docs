//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		
		public double							PeriodWidth
		{
			get
			{
				ushort glyph = this.GetGlyphIndex ('.');
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
		
		public ushort							SpaceGlyph
		{
			get
			{
				if (this.spaceGlyph == 0)
				{
					this.spaceGlyph = this.ot_indexMapping.GetGlyphIndex (this.SpaceChar);
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
					this.hyphenGlyph = this.ot_indexMapping.GetGlyphIndex (this.HyphenChar);
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
					this.ellipsisGlyph = this.ot_indexMapping.GetGlyphIndex (this.EllipsisChar);
				}

				return this.ellipsisGlyph;
			}
		}
		
		public double							HyphenWidth
		{
			get
			{
				double per_em  = this.ot_head.UnitsPerEm;
				return this.GetAdvance (this.HyphenGlyph) / per_em;
			}
		}

		public double							EllipsisWidth
		{
			get
			{
				double per_em  = this.ot_head.UnitsPerEm;
				return this.GetAdvance (this.EllipsisGlyph) / per_em;
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
			
			this.ot_GSUB = Table_GSUB.Create (this.fontData["GSUB"]);
			this.ot_GDEF = Table_GDEF.Create (this.fontData["GDEF"]);
			this.ot_cmap = new Table_cmap (this.fontData["cmap"]);
			this.ot_maxp = new Table_maxp (this.fontData["maxp"]);
			this.ot_head = new Table_head (this.fontData["head"]);
			this.ot_hhea = new Table_hhea (this.fontData["hhea"]);
			this.ot_hmtx = new Table_hmtx (this.fontData["hmtx"]);
			this.ot_glyf = this.fontData["glyf"] == null ? null : new Table_glyf (this.fontData["glyf"]);

			if (this.fontData["loca"] != null)
			{
				switch (this.ot_head.IndexToLocFormat)
				{
					case 0:
						this.ot_loca_short = new Table_loca_Short (this.fontData["loca"]);
						break;
					case 1:
						this.ot_loca_long = new Table_loca_Long (this.fontData["loca"]);
						break;
				}
			}
			
			//	TODO: handle GPOS for advanced glyph positioning, required by some advanced OpenType fonts.
			//	See http://partners.adobe.com/public/developer/opentype/index_table_formats2.html
			
			Table_kern ot_kern = Table_kern.Create (this.fontData["kern"]);
			
			if ((ot_kern != null) &&
				(ot_kern.Count > 0) &&
				(ot_kern.GetKerningTable (0).SubtableFormat == 0))
			{
				this.ot_kernFormat0 = ot_kern.GetKerningTable (0).Format0Subtable;
			}
			
			this.ot_indexMapping = this.ot_cmap.FindFormatSubTable ();
			
			this.glyphCache   = new ushort[Font.GlyphCacheSize];
			this.advanceCache = new ushort[Font.AdvanceCacheSize];
		}


		/// <summary>
		/// Generates the glyphs for the specified text.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns>The glyphs represented by 16-bit unsigned values.</returns>
		public ushort[] GenerateGlyphs(string text)
		{
			int[] gl_map = null;
			return this.GenerateGlyphs (text, ref gl_map);
		}

		/// <summary>
		/// Generates the glyphs for the specified text. Fills the glyph length
		/// map, if provided, with a character count per glyph, for each output
		/// glyph.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="gl_map">The glyph length map array or <c>null</c>.</param>
		/// <returns>
		/// The glyphs represented by 16-bit unsigned values.
		/// </returns>
		public ushort[] GenerateGlyphs(string text, ref int[] gl_map)
		{
			int length = text.Length;
			ushort[] glyphs = new ushort[length];
			
			for (int i = 0; i < length; i++)
			{
				glyphs[i] = this.GetGlyphIndex (text[i]);
			}
			
			this.ApplySubstitutions (ref glyphs, ref gl_map);
			
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
			int[]    gl_map = null;
			
			for (int i = 0; i < length; i++)
			{
				ulong bits = text[start+i];
				int   code = Font.UnicodeMask & (int) bits;
				
				glyphs[i] = (bits & Font.TransparentGlyphFlag) == 0 ? this.GetGlyphIndex (code) : (ushort) code;
			}
			
			this.ApplySubstitutions (ref glyphs, ref gl_map);
			
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
			int[] gl_map;
			int   count;
			
			glyphs = new ushort[length];
			gl_map = new int[length];
			
			for (int i = 0; i < length; i++)
			{
				glyphs[i] = this.GetGlyphIndex (text[i]);
			}
			
			this.ApplySubstitutions (ref glyphs, ref gl_map);
			
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
					src += gl_map[i] + 1;
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
			int[] gl_map;
			int   count;
			
			glyphs = new ushort[length];
			gl_map = new int[length];
			
			for (int i = 0; i < length; i++)
			{
				ulong bits = text[start+i];
				int   code = Font.UnicodeMask & (int) bits;
				
				glyphs[i] = (bits & Font.TransparentGlyphFlag) == 0 ? this.GetGlyphIndex (code) : (ushort) code;
			}
			
			this.ApplySubstitutions (ref glyphs, ref gl_map);
			
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
					src += gl_map[i] + 1;
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
			int[] gl_map;
			int   count;
			
			glyphs = new ushort[length];
			gl_map = new int[length];
			
			for (int i = 0; i < length; i++)
			{
				ulong bits = text[start+i];
				int   code = Font.UnicodeMask & (int) bits;
				
				glyphs[i] = (bits & Font.TransparentGlyphFlag) == 0 ? this.GetGlyphIndex (code) : (ushort) code;
			}
			
			this.ApplySubstitutions (ref glyphs, ref gl_map);
			
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
					src += gl_map[i] + 1;
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
				int num_glyph     = this.ot_maxp.NumGlyphs;
				int num_h_metrics = this.ot_hhea.NumHMetrics;
				
				double advance = 0;
				double per_em  = this.ot_head.UnitsPerEm;
				
				if (glyph < num_glyph)
				{
					if (glyph < num_h_metrics)
					{
						advance = this.GetAdvance (glyph);
					}
					else
					{
						advance = this.ot_hmtx.GetAdvanceWidth (num_h_metrics-1);
					}
				}
				else if (glyph >= 0xff00)
				{
					advance += this.GetSpaceGlyphWidth (glyph) * per_em;
				}
				
				return advance * size / per_em;
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
				int num_glyph = this.ot_maxp.NumGlyphs;

				double advance = 0;
				double per_em  = this.ot_head.UnitsPerEm;
				double scale   = size / per_em;

				if (glyph < num_glyph)
				{
					int offset = -1;
					
					if (this.ot_loca_short != null)
					{
						offset = this.ot_loca_short.GetOffset (glyph);
					}
					else if (this.ot_loca_long != null)
					{
						offset = this.ot_loca_long.GetOffset (glyph);
					}
					else
					{
						//	TODO: handle fonts which don't have a 'loca' table. See below...
					}

					if (offset >= 0)
					{
						if (this.ot_glyf != null)
						{
							Table_glyf ot_glyf = new Table_glyf (this.ot_glyf.BaseData, this.ot_glyf.BaseOffset + offset);

							xMin = ot_glyf.XMin * scale;
							xMax = ot_glyf.XMax * scale;
							yMin = ot_glyf.YMin * scale;
							yMax = ot_glyf.YMax * scale;
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
			
			int num_glyph     = this.ot_maxp.NumGlyphs;
			int num_h_metrics = this.ot_hhea.NumHMetrics;
			
			int    advance    = 0;
			ushort prev_glyph = 0xffff;
			double per_em     = this.ot_head.UnitsPerEm;
			
			for (int i = 0; i < glyphs.Length; i++)
			{
				ushort glyph = glyphs[i];
				int    delta;
				
				if (this.useKerning)
				{
					if (this.ApplyKerningInformation (glyph, ref prev_glyph, num_glyph, out delta))
					{
						advance += delta;
					}
				}
				
				if (glyph < num_glyph)
				{
					if (glyph < num_h_metrics)
					{
						advance += this.GetAdvance (glyph);
					}
					else
					{
						advance += this.ot_hmtx.GetAdvanceWidth (num_h_metrics-1);
					}
				}
				else if (glyph >= 0xff00)
				{
					advance += (int) (this.GetSpaceGlyphWidth (glyph) * per_em);
				}
			}
			
			return advance * size / per_em;
		}

		/// <summary>
		/// Gets the individual glyph positions.
		/// </summary>
		/// <param name="glyphs">The glyphs.</param>
		/// <param name="size">The font point size.</param>
		/// <param name="ox">The x origin for the first glyph.</param>
		/// <param name="x_pos">The array of positions after every glyph; the array must
		/// be allocated by the caller.</param>
		/// <returns>The total width of the glyphs.</returns>
		public double GetPositions(ushort[] glyphs, double size, double ox, double[] x_pos)
		{
			if (this.useSystemGlyphSize)
			{
				FontSizeInfo info = this.identity.GetSizeInfo ((int)(size + 0.5));
				
				int width = 0;
				
				for (int i = 0; i < glyphs.Length; i++)
				{
					x_pos[i] = ox + width;
					
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
			
			int num_glyph     = this.ot_maxp.NumGlyphs;
			int num_h_metrics = this.ot_hhea.NumHMetrics;
			
			int    advance    = 0;
			ushort prev_glyph = 0xffff;
			double per_em     = this.ot_head.UnitsPerEm;
			
			for (int i = 0; i < glyphs.Length; i++)
			{
				ushort glyph = glyphs[i];
				int    delta;
				
				if (this.useKerning)
				{
					if (this.ApplyKerningInformation (glyph, ref prev_glyph, num_glyph, out delta))
					{
						advance  += delta;
					}
				}
				
				x_pos[i] = ox + advance * size / per_em;
				
				if (glyph < num_glyph)
				{
					if (glyph < num_h_metrics)
					{
						advance += this.GetAdvance (glyph);
					}
					else
					{
						advance += this.ot_hmtx.GetAdvanceWidth (num_h_metrics-1);
					}
				}
				else if (glyph >= 0xff00)
				{
					advance += (int) (this.GetSpaceGlyphWidth (glyph) * per_em);
				}
			}
			
			return advance * size / per_em;
		}

		/// <summary>
		/// Gets the individual glyph positions. The scale is used to modify the glyph
		/// width (font glyph width + specified width adjustment); the glue is added
		/// independently of the scale.
		/// </summary>
		/// <param name="glyphs">The glyphs.</param>
		/// <param name="size">The font point size.</param>
		/// <param name="ox">The x origin for the first glyph.</param>
		/// <param name="x_pos">The array of positions after every glyph which must
		/// be allocated by the caller.</param>
		/// <param name="x_scale">The array of horizontal scales applied for every individual glyph or <c>null</c> if every glyph has a scale of <c>1</c>.</param>
		/// <param name="x_adjust">The array of horizontal width adjustement for every individual glyph or <c>null</c> if every glyph has an adjustement of <c>0</c>.</param>
		/// <param name="x_glue">The array of horizontal glue for every individual glyph or <c>null</c> if every glyph has a glue of <c>0</c>.</param>
		/// <returns>The total width of the glyphs.</returns>
		public double GetPositions(ushort[] glyphs, double size, double ox, double[] x_pos, double[] x_scale, double[] x_adjust, double[] x_glue)
		{
			if (this.useSystemGlyphSize)
			{
				FontSizeInfo info = this.identity.GetSizeInfo ((int)(size + 0.5));
				
				int width = 0;
				
				for (int i = 0; i < glyphs.Length; i++)
				{
					x_pos[i] = ox + width;
					
					double adjust = (x_adjust == null) ? 0 : x_adjust[i];
					
					int glyph_width;
					
					if (glyphs[i] >= 0xff00)
					{
						glyph_width = (int) (this.GetSpaceGlyphWidth (glyphs[i]) * size + 0.5);
					}
					else
					{
						glyph_width = info.GetGlyphWidth (glyphs[i]);
					}
					
					if (x_glue == null)
					{
						width += (int)((glyph_width + adjust) * x_scale[i] + 0.5);
					}
					else
					{
						width += (int)((glyph_width + adjust) * x_scale[i] + x_glue[i] + 0.5);
					}
				}
				
				return width;
			}
			
			int num_glyph     = this.ot_maxp.NumGlyphs;
			int num_h_metrics = this.ot_hhea.NumHMetrics;
			
			double per_em     = this.ot_head.UnitsPerEm;
			double advance    = 0;
			ushort prev_glyph = 0xffff;
			
			for (int i = 0; i < glyphs.Length; i++)
			{
				ushort glyph = glyphs[i];
				int    delta;
				
				if (this.useKerning)
				{
					if (this.ApplyKerningInformation (glyph, ref prev_glyph, num_glyph, out delta))
					{
						advance += delta * x_scale[i] * size / per_em;
					}
				}
				
				x_pos[i] = ox + advance;
				
				if (glyph < num_glyph)
				{
					if (glyph < num_h_metrics)
					{
						advance += this.GetAdvance (glyph) * x_scale[i] * size / per_em;
					}
					else
					{
						advance += this.ot_hmtx.GetAdvanceWidth (num_h_metrics-1) * x_scale[i] * size / per_em;
					}
				}
				else if (glyph >= 0xff00)
				{
					advance += this.GetSpaceGlyphWidth (glyph) * x_scale[i] * size;
				}
				
				if (x_adjust != null)
				{
					advance += x_adjust[i] * x_scale[i];
				}
				
				if (x_glue != null)
				{
					advance += x_glue[i];
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
		/// <param name="x_pos">The array of horizontal positions after every glyph; the array must
		/// be allocated by the caller.</param>
		/// <param name="y_pos">The array of baseline vertical positions for every glyph; the array must
		/// be allocated by the caller.</param>
		/// <returns>The total width of the glyphs.</returns>
		public double GetPositions(ushort[] glyphs, double size, double ox, double oy, double[] x_pos, double[] y_pos)
		{
			if (this.useSystemGlyphSize)
			{
				FontSizeInfo info = this.identity.GetSizeInfo ((int)(size + 0.5));
				
				int width = 0;
				
				for (int i = 0; i < glyphs.Length; i++)
				{
					x_pos[i] = ox + width;
					y_pos[i] = oy;
					
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
			
			int num_glyph     = this.ot_maxp.NumGlyphs;
			int num_h_metrics = this.ot_hhea.NumHMetrics;
			
			double per_em     = this.ot_head.UnitsPerEm;
			int    advance    = 0;
			ushort prev_glyph = 0xffff;
			
			for (int i = 0; i < glyphs.Length; i++)
			{
				ushort glyph = glyphs[i];
				int    delta;
				
				if (this.useKerning)
				{
					if (this.ApplyKerningInformation (glyph, ref prev_glyph, num_glyph, out delta))
					{
						advance  += delta;
					}
				}
				
				x_pos[i] = ox + advance * size / per_em;
				y_pos[i] = oy;
				
				if (glyph < num_glyph)
				{
					if (glyph < num_h_metrics)
					{
						advance += this.GetAdvance (glyph);
					}
					else
					{
						advance += this.ot_hmtx.GetAdvanceWidth (num_h_metrics-1);
					}
				} 
				else if (glyph >= 0xff00)
				{
					advance += (int) (this.GetSpaceGlyphWidth (glyph) * per_em);
				}
			}
			
			return advance * size / per_em;
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
			int[]    gl_map;
			
			this.MapToGlyphs (text, out glyphs, out gl_map);
			
			return this.HitTest (glyphs, gl_map, size, pos, out x, out y);
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
			int[]    gl_map;
			
			this.MapToGlyphs (text, start, length, out glyphs, out gl_map);
			
			return this.HitTest (glyphs, gl_map, size, pos, out x, out y);
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
			int[]    gl_map;
			
			this.MapToGlyphs (text, out glyphs, out gl_map);
			
			return this.HitTest (glyphs, gl_map, size, x, y, out pos, out subpos);
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
			int[]    gl_map;
			
			this.MapToGlyphs (text, start, length, out glyphs, out gl_map);
			
			return this.HitTest (glyphs, gl_map, size, x, y, out pos, out subpos);
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
			
			if ((this.ot_GSUB != null) &&
				(this.ot_GSUB.ScriptListTable.ContainsScript (script)))
			{
				int   required_feature  = this.ot_GSUB.GetRequiredFeatureIndex (script, language);
				int[] optional_features = this.ot_GSUB.GetFeatureIndexes (script, language);
				
				if (required_feature == 0xffff)
				{
					this.scriptRequiredFeature = null;
				}
				else
				{
					this.scriptRequiredFeature = this.ot_GSUB.FeatureListTable.GetTaggedFeatureTable (required_feature);
				}
				
				this.scriptOptionalFeatures = new TaggedFeatureTable[optional_features.Length];
				
				for (int i = 0; i < optional_features.Length; i++)
				{
					this.scriptOptionalFeatures[i] = this.ot_GSUB.FeatureListTable.GetTaggedFeatureTable (optional_features[i]);
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
			
			string collapsed_features = buffer.ToString ();
			
			if ((this.activeFeatures == collapsed_features) &&
				((this.substitutionLookups != null) || (this.ot_GSUB == null)))
			{
				return;
			}
			
			this.activeFeatures = collapsed_features;
			
			FeatureListTable             feature_list    = this.ot_GSUB == null ? null : this.ot_GSUB.FeatureListTable;
			System.Collections.ArrayList active_features = new System.Collections.ArrayList ();
			System.Collections.Hashtable active_names    = new System.Collections.Hashtable ();
			
			for (int i = 0; i < features.Length; i++)
			{
				active_names[features[i]] = null;
			}
			
			if (active_names.Contains ("liga"))
			{
				this.mapDefaultLigatures = true;
			}
			else
			{
				this.mapDefaultLigatures = false;
			}
			
			if (active_names.Contains ("kern"))
			{
				this.useKerning = true;
			}
			else
			{
				this.useKerning = false;
			}
			
			if (active_names.Contains ("Mgr=System"))
			{
				this.SelectFontManager (FontManagerType.System);
			}
			else if (active_names.Contains ("Mgr=OpenType"))
			{
				this.SelectFontManager (FontManagerType.OpenType);
			}
			
			if (this.scriptRequiredFeature != null)
			{
				active_features.Add (this.scriptRequiredFeature);
			}
			
			if (this.scriptOptionalFeatures == null)
			{
				int n = feature_list == null ? 0 : feature_list.FeatureCount;
				
				for (int i = 0; i < n; i++)
				{
					if (active_names.Contains (feature_list.GetFeatureTag (i)))
					{
						active_features.Add (feature_list.GetFeatureTable (i));
						active_names.Remove (feature_list.GetFeatureTag (i));
					}
				}
			}
			else
			{
				int n = this.scriptOptionalFeatures.Length;
				
				for (int i = 0; i < n; i++)
				{
					if (active_names.Contains (this.scriptOptionalFeatures[i].Tag))
					{
						active_features.Add (this.scriptOptionalFeatures[i]);
					}
				}
			}
			
			if (this.ot_GSUB != null)
			{
				this.GenerateSubstitutionLookups (active_features);
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
			
			ScriptListTable script_list = this.ot_GSUB == null ? null : this.ot_GSUB.ScriptListTable;
			int n = script_list == null ? 0 : script_list.ScriptCount;
			
			for (int i = 0; i < n; i++)
			{
				string      script_tag   = script_list.GetScriptTag (i);
				ScriptTable script_table = script_list.GetScriptTable (i);
				
				if (i > 0)
				{
					buffer.Append ("|");
				}
				
				buffer.Append (script_tag);
				
				int m = script_table.LangSysCount;
				
				for (int j = 0; j < m; j++)
				{
					buffer.Append ("|");
					buffer.Append (script_tag);
					buffer.Append (":");
					buffer.Append (script_table.GetLangSysTag (j));
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
				FeatureListTable feature_list = this.ot_GSUB == null ? null : this.ot_GSUB.FeatureListTable;
				
				int n = feature_list == null ? 0 : feature_list.FeatureCount;
				
				for (int i = 0; i < n; i++)
				{
					string tag = feature_list.GetFeatureTag (i);
					
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
			
			if (ot_kernFormat0 != null)
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
			
			if (this.ot_GSUB != null)
			{
				foreach (string feature in features)
				{
					int[] indexes = this.ot_GSUB.GetFeatureIndexes (feature);
					foreach (int index in indexes)
					{
						FeatureTable f_table = this.ot_GSUB.FeatureListTable.GetFeatureTable (index);
						int          count   = f_table.LookupCount;
						
						for (int i = 0; i < count; i++)
						{
							list.Add (this.ot_GSUB.LookupListTable.GetLookupTable (f_table.GetLookupIndex (i)));
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
				return info.Handle.Handle;
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
			FontSizeInfo info = this.identity.GetSizeInfo (this.ot_head.UnitsPerEm);
			return info.Handle.Handle;
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
				double scale = size / this.ot_head.UnitsPerEm;
				
				return this.ot_hhea.MacAscender * scale;
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
				double scale = size / this.ot_head.UnitsPerEm;
				
				return this.ot_hhea.MacDescender * scale;
			}
		}

		/// <summary>
		/// Gets the font line gap.
		/// </summary>
		/// <param name="size">The font point size.</param>
		/// <returns>The font line gap.</returns>
		public double GetLineGap(double size)
		{
			double scale = size / this.ot_head.UnitsPerEm;
			return this.ot_hhea.MacLineGap * scale;
		}

		/// <summary>
		/// Gets the font caret angle in radians.
		/// </summary>
		/// <returns>The font caret angle in radians.</returns>
		public double GetCaretAngleRad()
		{
			double dx = this.ot_hhea.CaretSlopeRun;
			double dy = this.ot_hhea.CaretSlopeRise;
			
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
			double scale = size / this.ot_head.UnitsPerEm;
			
			xMin = this.ot_head.XMin * scale;
			xMax = this.ot_head.XMax * scale;
			yMin = this.ot_head.YMin * scale;
			yMax = this.ot_head.YMax * scale;
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
			
			double scale = size / this.ot_head.UnitsPerEm;
			
			double ot_xmin = this.ot_head.XMin * scale;
			double ot_xmax = this.ot_head.XMax * scale;
			double ot_ymin = this.ot_head.YMin * scale;
			double ot_ymax = this.ot_head.YMax * scale;
			
			xMin = x[0] + ot_xmin * (sx == null ? 1 : sx[0]);
			xMax = x[0] + ot_xmax * (sx == null ? 1 : sx[0]);
			yMin = y[0] + ot_ymin * (sy == null ? 1 : sy[0]);
			yMax = y[0] + ot_ymax * (sy == null ? 1 : sy[0]);
			
			for (int i = 1; i < glyphs.Length; i++)
			{
				double xx_min = x[i] + ot_xmin * (sx == null ? 1 : sx[i]);
				double xx_max = x[i] + ot_xmax * (sx == null ? 1 : sx[i]);
				double yy_min = y[i] + ot_ymin * (sy == null ? 1 : sy[i]);
				double yy_max = y[i] + ot_ymax * (sy == null ? 1 : sy[i]);
				
				if (xx_min < xMin) xMin = xx_min;
				if (xx_max > xMax) xMax = xx_max;
				if (yy_min < yMin) yMin = yy_min;
				if (yy_max > yMax) yMax = yy_max;
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
			}
			
			if ((code == 0) ||
				(this.ot_indexMapping == null))
			{
				return 0xffff;
			}
			
			glyph = this.ot_indexMapping.GetGlyphIndex (code);
			
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
		
		
		private void MapToGlyphs(string text, out ushort[] glyphs, out int[] gl_map)
		{
			int length = text.Length;
			
			glyphs = new ushort[length];
			gl_map = new int[length];
			
			for (int i = 0; i < length; i++)
			{
				glyphs[i] = this.GetGlyphIndex (text[i]);
			}
			
			this.ApplySubstitutions (ref glyphs, ref gl_map);
		}
		
		private void MapToGlyphs(ulong[] text, int start, int length, out ushort[] glyphs, out int[] gl_map)
		{
			glyphs = new ushort[length];
			gl_map = new int[length];
			
			for (int i = 0; i < length; i++)
			{
				ulong bits = text[start+i];
				int   code = Font.UnicodeMask & (int) bits;
				
				glyphs[i] = (bits & Font.TransparentGlyphFlag) == 0 ? this.GetGlyphIndex (code) : (ushort) code;
			}
			
			this.ApplySubstitutions (ref glyphs, ref gl_map);
		}
		
		
		private void GenerateSubstitutionLookups(System.Collections.ICollection feature_tables)
		{
			System.Collections.ArrayList lookup_indexes = new System.Collections.ArrayList ();
			
			foreach (FeatureTable feature_table in feature_tables)
			{
				int n = feature_table.LookupCount;
				
				for (int i = 0; i < n; i++)
				{
					int lookup = feature_table.GetLookupIndex (i);
					
					if (lookup_indexes.Contains (lookup) == false)
					{
						lookup_indexes.Add (lookup);
					}
				}
			}
			
			lookup_indexes.Sort ();
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			if (this.ot_GSUB != null)
			{
				foreach (int lookup in lookup_indexes)
				{
					LookupTable lookup_table = this.ot_GSUB.LookupListTable.GetLookupTable (lookup);
					
					int n = lookup_table.SubTableCount;
					
					for (int i = 0; i < n; i++)
					{
						BaseSubstitution subst = lookup_table.GetSubstitution (i);
						
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
		
		
		private void ApplySubstitutions(ref ushort[] glyphs, ref int[] gl_map)
		{
			int count = glyphs.Length;
			
			//	Exécute les substitutions de glyphes en fonction des 'features'
			//	sélectionnées :
			
			int        max_size = count + 16;
			ushort[][] temp     = new ushort[2][];
			
		try_again:
			
			temp[0] = new ushort[max_size];
			temp[1] = new ushort[max_size];
			
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
						Font.ApplySubstitutions (this.substitutionLookups[i], input, length, output, out length, ref gl_map);
						
						input   = output;
						output  = temp[toggle & 1];
						toggle += 1;
					}
				}
				
				if (this.mapDefaultLigatures)
				{
					this.ApplyManualLigatureSubstitutions (input, length, output, out length, ref gl_map);
				}
				else
				{
					output = input;
				}
				
				glyphs = new ushort[length];
				
				for (int i = 0; i < length; i++)
				{
					glyphs[i] = output[i];
				}
			}
			catch (System.IndexOutOfRangeException)
			{
				max_size += max_size / 8;
				goto try_again;
			}
		}
		
		private void ApplyManualLigatureSubstitutions(ushort[] input_glyphs, int input_length, ushort[] output_glyphs, out int output_length, ref int[] gl_map)
		{
			if (this.ot_indexMapping == null)
			{
				for (int i = 0; i < input_length; i++)
				{
					output_glyphs[i] = input_glyphs[i];
				}
				
				output_length = input_length;
				
				return;
			}
			
			int input_offset  = 0;
			int output_offset = 0;
			
			ushort glyph_f = this.ot_indexMapping.GetGlyphIndex ('f');
			ushort glyph_i = this.ot_indexMapping.GetGlyphIndex ('i');
			ushort glyph_l = this.ot_indexMapping.GetGlyphIndex ('l');
			ushort lig_ff  = this.ot_indexMapping.GetGlyphIndex (0xfb00);
			ushort lig_fi  = this.ot_indexMapping.GetGlyphIndex (0xfb01);
			ushort lig_fl  = this.ot_indexMapping.GetGlyphIndex (0xfb02);
			ushort lig_ffi = this.ot_indexMapping.GetGlyphIndex (0xfb03);
			ushort lig_ffl = this.ot_indexMapping.GetGlyphIndex (0xfb04);
			
			while (input_offset < input_length)
			{
				int length = input_length - input_offset;
				
				if (length > 2)
				{
					if ((lig_ffi > 0) &&
						(input_glyphs[input_offset+0] == glyph_f) &&
						(input_glyphs[input_offset+1] == glyph_f) &&
						(input_glyphs[input_offset+2] == glyph_i))
					{
						output_glyphs[output_offset] = lig_ffi;
						
						if (gl_map != null)
						{
							gl_map[output_offset] = gl_map[input_offset] + 2;
						}
						
						input_offset  += 3;
						output_offset += 1;
						
						continue;
					}
					
					if ((lig_ffl > 0) &&
						(input_glyphs[input_offset+0] == glyph_f) &&
						(input_glyphs[input_offset+1] == glyph_f) &&
						(input_glyphs[input_offset+2] == glyph_l))
					{
						output_glyphs[output_offset] = lig_ffl;
						
						if (gl_map != null)
						{
							gl_map[output_offset] = gl_map[input_offset] + 2;
						}
						
						input_offset  += 3;
						output_offset += 1;
						
						continue;
					}
				}
				
				if (length > 1)
				{
					if ((lig_ff > 0) &&
						(input_glyphs[input_offset+0] == glyph_f) &&
						(input_glyphs[input_offset+1] == glyph_f))
					{
						output_glyphs[output_offset] = lig_ff;
						
						if (gl_map != null)
						{
							gl_map[output_offset] = gl_map[input_offset] + 1;
						}
						
						input_offset  += 2;
						output_offset += 1;
						
						continue;
					}
					
					if ((lig_fi > 0) &&
						(input_glyphs[input_offset+0] == glyph_f) &&
						(input_glyphs[input_offset+1] == glyph_i))
					{
						output_glyphs[output_offset] = lig_fi;
						
						if (gl_map != null)
						{
							gl_map[output_offset] = gl_map[input_offset] + 1;
						}
						
						input_offset  += 2;
						output_offset += 1;
						
						continue;
					}
					
					if ((lig_fl > 0) &&
						(input_glyphs[input_offset+0] == glyph_f) &&
						(input_glyphs[input_offset+1] == glyph_l))
					{
						output_glyphs[output_offset] = lig_fl;
						
						if (gl_map != null)
						{
							gl_map[output_offset] = gl_map[input_offset] + 1;
						}
						
						input_offset  += 2;
						output_offset += 1;
						
						continue;
					}
				}
				
				output_glyphs[output_offset] = input_glyphs[input_offset];
				
				if (gl_map != null)
				{
					gl_map[output_offset] = gl_map[input_offset];
				}
						
				input_offset  += 1;
				output_offset += 1;
			}
			
			output_length = output_offset;
		}
		
		private static void ApplySubstitutions(BaseSubstitution substitution, ushort[] input_glyphs, int input_length, ushort[] output_glyphs, out int output_length, ref int[] gl_map)
		{
			int input_offset  = 0;
			int output_offset = 0;
			
			if (gl_map == null)
			{
				while (input_offset < input_length)
				{
					if (substitution.ProcessSubstitution (input_glyphs, ref input_offset, input_length, output_glyphs, ref output_offset))
					{
						//	Substitution avec succès.
					}
					else
					{
						output_glyphs[output_offset] = input_glyphs[input_offset];
						
						input_offset++;
						output_offset++;
					}
				}
			}
			else
			{
				while (input_offset < input_length)
				{
					int input_origin  = input_offset;
					int output_origin = output_offset;
					
					if (substitution.ProcessSubstitution (input_glyphs, ref input_offset, input_length, output_glyphs, ref output_offset))
					{
						//	Substitution avec succès.
						
						int input_advance  = input_offset  - input_origin;
						int output_advance = output_offset - output_origin;
						
						Debug.Assert.IsTrue (input_advance > 0);
						Debug.Assert.IsTrue (output_advance == 1);
						
						gl_map[output_origin++] = gl_map[input_origin++] + input_advance - output_advance;
						
						//	Attention, quand les substitutions pourront générer
						//	plusieurs glyphes en sortie, ce code devra être
						//	adapté.
					}
					else
					{
						output_glyphs[output_offset] = input_glyphs[input_offset];
						gl_map[output_offset]        = gl_map[input_offset];
						
						input_offset++;
						output_offset++;
					}
				}
			}
			
			output_length = output_offset;
		}
		
		private bool ApplyKerningInformation(ushort glyph, ref ushort prev_glyph, int num_glyph, out int delta)
		{
			if ((this.useKerning) &&
				(glyph < num_glyph) &&
				(prev_glyph < num_glyph) &&
				(this.ot_kernFormat0 != null) &&
				(this.ot_kernFormat0.FindKernValue (prev_glyph, glyph, out delta)))
			{
				prev_glyph = glyph;
				return true;
			}
			else
			{
				delta      = 0;
				prev_glyph = glyph;
				return false;
			}
		}
		
		
		private bool HitTest(ushort[] glyphs, int[] gl_map, double size, int pos, out double x, out double y)
		{
			//	TODO: gérer use_system_glyph_size
			
			double scale = size / this.ot_head.UnitsPerEm;
			
			int num_glyph     = this.ot_maxp.NumGlyphs;
			int num_h_metrics = this.ot_hhea.NumHMetrics;
			
			int    advance    = 0;
			int    distance   = 0;
			ushort prev_glyph = 0xffff;
			
			for (int i = 0; i < glyphs.Length; i++)
			{
				ushort glyph = glyphs[i];
				int    delta;
				
				if (this.useKerning)
				{
					if (this.ApplyKerningInformation (glyph, ref prev_glyph, num_glyph, out delta))
					{
						advance += delta;
					}
				}
				
				if ((distance <= pos) &&
					(distance+gl_map[i] >= pos))
				{
					//	Trouve la position [x] exacte si c'est dans une ligature.
					
					if (gl_map[i] > 0)
					{
						this.GetLigatureCaretPosition (glyph, gl_map[i]+1, size, pos - distance, out x, out y);
						
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
				
				if (glyph < num_glyph)
				{
					if (glyph < num_h_metrics)
					{
						advance += this.GetAdvance (glyph);
					}
					else
					{
						advance += this.ot_hmtx.GetAdvanceWidth (num_h_metrics-1);
					}
				}
				
				distance += 1;
				distance += gl_map[i];
			}
			
			x = advance * scale;
			y = 0.0;
			
			return true;
		}
		
		private bool HitTest(ushort[] glyphs, int[] gl_map, double size, double x, double y, out int pos, out double subpos)
		{
			//	TODO: gérer use_system_glyph_size
			
			if (x <= 0)
			{
				pos    = 0;
				subpos = 0;
				
				return false;
			}
			
			ushort[] one_glyph  = new ushort[1];
			int[]    one_gl_map = new int[1];
			
			int count = 0;
			
			for (int i = 0; i < glyphs.Length; i++)
			{
				one_glyph[0]  = glyphs[i];
				one_gl_map[0] = gl_map[i];
				
				double x1, x2;
				double y1, y2;
				
				x2 = 0;
				
				for (int one_pos = 0; one_pos < gl_map[i]+1; one_pos++)
				{
					this.HitTest (one_glyph, one_gl_map, size, one_pos + 0, out x1, out y1);
					this.HitTest (one_glyph, one_gl_map, size, one_pos + 1, out x2, out y2);
					
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
		
		
		private void GetLigatureCaretPosition(ushort glyph, int simple_glyph_count, double size, int pos, out double x, out double y)
		{
			if (pos < 1)
			{
				x = 0;
				y = 0;
				
				return;
			}
			
			double scale = size / this.ot_head.UnitsPerEm;
			
			LigatureCaretListTable caret_list = this.ot_GDEF == null ? null : this.ot_GDEF.LigatureCaretListTable;
			
			if (caret_list != null)
			{
				int index = caret_list.Coverage.FindIndex (glyph);
				
				if (index >= 0)
				{
					LigatureGlyphTable lig_table = caret_list.GetLigatureGlyphTable (index);
					
					int caret_count = lig_table.CaretCount;
					int caret_index = pos - 1;
					
					if (caret_index < caret_count)
					{
						switch (lig_table.GetCaretValueFormat (caret_index))
						{
							case 1:
								x = lig_table.GetCaretCoordinateFmt1 (caret_index) * scale;
								y = 0;
								return;
							
							case 2:
								//	TODO: gérer...
								break;
							
							case 3:
								x = lig_table.GetCaretCoordinateFmt3 (caret_index) * scale;
								y = 0;
								
								//	TODO: gérer la "device table"
								
								return;
						}
					}
				}
			}
			
			int num_glyph     = this.ot_maxp.NumGlyphs;
			int num_h_metrics = this.ot_hhea.NumHMetrics;
			int advance       = 0;
			
			if (glyph < num_glyph)
			{
				if (glyph < num_h_metrics)
				{
					advance = this.GetAdvance (glyph);
				}
				else
				{
					advance = this.ot_hmtx.GetAdvanceWidth (num_h_metrics-1);
				}
			}
			
			x = advance * pos * scale / simple_glyph_count;
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
			
			advance = this.ot_hmtx.GetAdvanceWidth (glyph);
			
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
		
		private Table_GSUB						ot_GSUB;
		private Table_GDEF						ot_GDEF;
		private Table_cmap						ot_cmap;
		private Table_maxp						ot_maxp;
		private Table_head						ot_head;
		private Table_hhea						ot_hhea;
		private Table_hmtx						ot_hmtx;
		private Table_loca_Short				ot_loca_short;
		private Table_loca_Long					ot_loca_long;
		private Table_glyf						ot_glyf;
		private KerningTableFormat0				ot_kernFormat0;
		private IndexMappingTable				ot_indexMapping;
		
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
		private ushort[]						glyphCache;
		private ushort[]						advanceCache;
		
		private const int						GlyphCacheSize = 256;
		private const int						AdvanceCacheSize = 256;
	}
}
