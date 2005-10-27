//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// La classe Font fait le lien entre une fonte réelle (description sous
	/// la forme OpenType) et les besoins concrets des applications désirant
	/// manipuler des glyphes.
	/// </summary>
	public sealed class Font
	{
		public Font()
		{
		}
		
		public Font(FontIdentity identity)
		{
			this.Initialize (identity);
		}
		
		
		public FontManagerType					FontManagerType
		{
			get
			{
				return this.use_system_glyph_size ? FontManagerType.System : FontManagerType.OpenType;
			}
		}
		
		public FontIdentity						FontIdentity
		{
			get
			{
				return this.identity;
			}
		}
		
		
		public double							SpaceWidth
		{
			get
			{
				ushort glyph = this.GetGlyphIndex (' ');
				return glyph == 0xffff ? 0.25 : this.GetGlyphWidth (glyph, 1.0);
			}
		}
		
		public double							FigureWidth
		{
			get
			{
				ushort glyph = this.GetGlyphIndex ('0');
				return glyph == 0xffff ? 0.5 : this.GetGlyphWidth (glyph, 1.0);
			}
		}
		
		public double							PeriodWidth
		{
			get
			{
				ushort glyph = this.GetGlyphIndex ('.');
				return glyph == 0xffff ? 0.5 : this.GetGlyphWidth (glyph, 1.0);
			}
		}
		
		public double							EmWidth
		{
			get
			{
				ushort glyph = this.GetGlyphIndex (0x2014);
				return glyph == 0xffff ? 1.0 : this.GetGlyphWidth (glyph, 1.0);
			}
		}
		
		public double							EnWidth
		{
			get
			{
				ushort glyph = this.GetGlyphIndex (0x2013);
				return glyph == 0xffff ? 0.5 : this.GetGlyphWidth (glyph, 1.0);
			}
		}
		
		
		public ushort							SpaceGlyph
		{
			get
			{
				if (this.space_glyph == 0)
				{
					this.space_glyph = this.ot_index_mapping.GetGlyphIndex (' ');
				}
				
				return this.space_glyph;
			}
		}
		
		internal void Initialize(FontIdentity identity)
		{
			this.identity = identity;
			this.Initialize (this.identity.FontData);
		}
		
		internal void Initialize(FontData font_data)
		{
			this.font_data = font_data;
			
			this.ot_GSUB = Table_GSUB.Create (this.font_data["GSUB"]);
			this.ot_GDEF = Table_GDEF.Create (this.font_data["GDEF"]);
			this.ot_cmap = new Table_cmap (this.font_data["cmap"]);
			this.ot_maxp = new Table_maxp (this.font_data["maxp"]);
			this.ot_head = new Table_head (this.font_data["head"]);
			this.ot_hhea = new Table_hhea (this.font_data["hhea"]);
			this.ot_hmtx = new Table_hmtx (this.font_data["hmtx"]);
			
			Table_kern ot_kern = Table_kern.Create (this.font_data["kern"]);
			
			if ((ot_kern != null) &&
				(ot_kern.Count > 0) &&
				(ot_kern.GetKerningTable (0).SubtableFormat == 0))
			{
				this.ot_kern_fmt_0 = ot_kern.GetKerningTable (0).Format0Subtable;
			}
			
			this.ot_index_mapping = this.ot_cmap.FindFormatSubTable ();
		}
		
		
		public ushort[] GenerateGlyphs(string text)
		{
			int      length = text.Length;
			ushort[] glyphs = new ushort[length];
			int[]    gl_map = null;
			
			for (int i = 0; i < length; i++)
			{
				glyphs[i] = this.GetGlyphIndex (text[i]);
			}
			
			this.ApplySubstitutions (ref glyphs, ref gl_map);
			
			return glyphs;
		}
		
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
		
		
		public double GetSpaceGlyphWidth(ushort glyph)
		{
			if (glyph >= 0xff00)
			{
				switch (glyph)
				{
					case 0xff00: return this.EmWidth;
					case 0xff01: return this.EmWidth / 2;
					case 0xff02: return this.EmWidth / 3;
					case 0xff03: return this.EmWidth / 4;
					case 0xff04: return this.EmWidth * 4 / 18;
					case 0xff05: return this.EmWidth / 5;
					case 0xff06: return this.EmWidth / 6;
					case 0xff07: return this.EmWidth / 16;
					case 0xff08: return 0;
					case 0xff09: return this.PeriodWidth;
					case 0xff0a: return this.FigureWidth;
					case 0xff0b: return this.SpaceWidth / 2;
				}
			}
			
			return 0;
		}
		
		public double GetGlyphWidth(ushort glyph, double size)
		{
			if (this.use_system_glyph_size)
			{
				if (glyph >= 0xff00)
				{
					return (int) (this.GetSpaceGlyphWidth (glyph) * size + 0.5);
				}
				else
				{
					FontIdentity.SizeInfo info = this.identity.GetSizeInfo ((int)(size + 0.5));
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
						advance = this.ot_hmtx.GetAdvanceWidth (glyph);
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
		
		public double GetTotalWidth(ushort[] glyphs, double size)
		{
			if (this.use_system_glyph_size)
			{
				FontIdentity.SizeInfo info = this.identity.GetSizeInfo ((int)(size + 0.5));
				
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
				
				if (this.ApplyKerningInformation (glyph, ref prev_glyph, num_glyph, out delta))
				{
					advance += delta;
				}
				
				if (glyph < num_glyph)
				{
					if (glyph < num_h_metrics)
					{
						advance += this.ot_hmtx.GetAdvanceWidth (glyph);
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
		
		public double GetPositions(ushort[] glyphs, double size, double ox, double[] x_pos)
		{
			if (this.use_system_glyph_size)
			{
				FontIdentity.SizeInfo info = this.identity.GetSizeInfo ((int)(size + 0.5));
				
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
				
				if (this.ApplyKerningInformation (glyph, ref prev_glyph, num_glyph, out delta))
				{
					advance  += delta;
				}
				
				x_pos[i] = ox + advance * size / per_em;
				
				if (glyph < num_glyph)
				{
					if (glyph < num_h_metrics)
					{
						advance += this.ot_hmtx.GetAdvanceWidth (glyph);
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
		
		public double GetPositions(ushort[] glyphs, double size, double ox, double[] x_pos, double[] x_scale, double[] x_adjust, double[] x_glue)
		{
			if (this.use_system_glyph_size)
			{
				FontIdentity.SizeInfo info = this.identity.GetSizeInfo ((int)(size + 0.5));
				
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
				
				if (this.ApplyKerningInformation (glyph, ref prev_glyph, num_glyph, out delta))
				{
					advance += delta * x_scale[i] * size / per_em;
				}
				
				x_pos[i] = ox + advance;
				
				if (glyph < num_glyph)
				{
					if (glyph < num_h_metrics)
					{
						advance += this.ot_hmtx.GetAdvanceWidth (glyph) * x_scale[i] * size / per_em;
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
		
		public double GetPositions(ushort[] glyphs, double size, double ox, double oy, double[] x_pos, double[] y_pos)
		{
			if (this.use_system_glyph_size)
			{
				FontIdentity.SizeInfo info = this.identity.GetSizeInfo ((int)(size + 0.5));
				
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
				
				if (this.ApplyKerningInformation (glyph, ref prev_glyph, num_glyph, out delta))
				{
					advance  += delta;
				}
				
				x_pos[i] = ox + advance * size / per_em;
				y_pos[i] = oy;
				
				if (glyph < num_glyph)
				{
					if (glyph < num_h_metrics)
					{
						advance += this.ot_hmtx.GetAdvanceWidth (glyph);
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
		
		public bool HitTest(string text, double size, double x, double y, out int pos, out double subpos)
		{
			ushort[] glyphs;
			int[]    gl_map;
			
			this.MapToGlyphs (text, out glyphs, out gl_map);
			
			return this.HitTest (glyphs, gl_map, size, x, y, out pos, out subpos);
		}
		
		public bool HitTest(ulong[] text, int start, int length, double size, double x, double y, out int pos, out double subpos)
		{
			ushort[] glyphs;
			int[]    gl_map;
			
			this.MapToGlyphs (text, start, length, out glyphs, out gl_map);
			
			return this.HitTest (glyphs, gl_map, size, x, y, out pos, out subpos);
		}
		
		
		public void SelectScript(string script)
		{
			this.SelectScript (script, "");
		}
		
		public void SelectScript(string script, string language)
		{
			if ((this.active_script == script) &&
				(this.active_language == language))
			{
				return;
			}
			
			this.active_script   = script;
			this.active_language = language;
			
			if ((this.ot_GSUB != null) &&
				(this.ot_GSUB.ScriptListTable.ContainsScript (script)))
			{
				int   required_feature  = this.ot_GSUB.GetRequiredFeatureIndex (script, language);
				int[] optional_features = this.ot_GSUB.GetFeatureIndexes (script, language);
				
				if (required_feature == 0xffff)
				{
					this.script_required_feature = null;
				}
				else
				{
					this.script_required_feature = this.ot_GSUB.FeatureListTable.GetTaggedFeatureTable (required_feature);
				}
				
				this.script_optional_features = new TaggedFeatureTable[optional_features.Length];
				
				for (int i = 0; i < optional_features.Length; i++)
				{
					this.script_optional_features[i] = this.ot_GSUB.FeatureListTable.GetTaggedFeatureTable (optional_features[i]);
				}
			}
			else
			{
				this.script_required_feature  = null;
				this.script_optional_features = null;
			}
			
			this.substitution_lookups = null;
			this.alternate_lookups    = null;
		}
		
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
			
			if ((this.active_features == collapsed_features) &&
				((this.substitution_lookups != null) || (this.ot_GSUB == null)))
			{
				return;
			}
			
			this.active_features = collapsed_features;
			
			FeatureListTable             feature_list    = this.ot_GSUB == null ? null : this.ot_GSUB.FeatureListTable;
			System.Collections.ArrayList active_features = new System.Collections.ArrayList ();
			System.Collections.Hashtable active_names    = new System.Collections.Hashtable ();
			
			for (int i = 0; i < features.Length; i++)
			{
				active_names[features[i]] = null;
			}
			
			if (active_names.Contains ("liga"))
			{
				this.map_default_ligatures = true;
			}
			else
			{
				this.map_default_ligatures = false;
			}
			
			if (active_names.Contains ("kern"))
			{
				this.use_kerning = true;
			}
			else
			{
				this.use_kerning = false;
			}
			
			if (active_names.Contains ("Mgr=System"))
			{
				this.SelectFontManager (FontManagerType.System);
			}
			else if (active_names.Contains ("Mgr=OpenType"))
			{
				this.SelectFontManager (FontManagerType.OpenType);
			}
			
			if (this.script_required_feature != null)
			{
				active_features.Add (this.script_required_feature);
			}
			
			if (this.script_optional_features == null)
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
				int n = this.script_optional_features.Length;
				
				for (int i = 0; i < n; i++)
				{
					if (active_names.Contains (this.script_optional_features[i].Tag))
					{
						active_features.Add (this.script_optional_features[i]);
					}
				}
			}
			
			if (this.ot_GSUB != null)
			{
				this.GenerateSubstitutionLookups (active_features);
			}
		}
		
		public void SelectFontManager(FontManagerType manager)
		{
			switch (manager)
			{
				case FontManagerType.OpenType:
					this.use_system_glyph_size = false;
					break;
				
				case FontManagerType.System:
					this.use_system_glyph_size = true;
					break;
			}
		}
		
		
		public void PushActiveFeatures()
		{
			if (this.saved_features_stack == null)
			{
				this.saved_features_stack = new System.Collections.Stack ();
			}
			
			this.saved_features_stack.Push (this.active_features);
		}
		
		public void DisableActiveFeatures(params string[] features)
		{
			string active  = this.active_features;
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
		
		public void PopActiveFeatures()
		{
			if (this.saved_features_stack != null)
			{
				string features = this.saved_features_stack.Pop () as string;
				this.SelectFeatures (features.Split ('/'));
			}
		}
		
		
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
		
		public string[] GetSupportedFeatures()
		{
			if (this.script_optional_features == null)
			{
				FeatureListTable feature_list = this.ot_GSUB == null ? null : this.ot_GSUB.FeatureListTable;
				System.Collections.Hashtable hash = new System.Collections.Hashtable ();
				
				int n = feature_list == null ? 0 : feature_list.FeatureCount;
				
				for (int i = 0; i < n; i++)
				{
					hash[feature_list.GetFeatureTag (i)] = null;
				}
				
				string[] feature_names = new string[hash.Count];
				hash.Keys.CopyTo (feature_names, 0);
				
				return feature_names;
			}
			else
			{
				int n = this.script_optional_features.Length;
				
				string[] feature_names = new string[n];
				
				for (int i = 0; i < n; i++)
				{
					feature_names[i] = this.script_optional_features[i].Tag;
				}
				
				return feature_names;
			}
		}
		
		
		public LookupTable[] GetLookupTables(params string[] features)
		{
			System.Collections.ArrayList list = new	 System.Collections.ArrayList ();
			
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
			
			LookupTable[] tables = new LookupTable[list.Count];
			list.CopyTo (tables, 0);
			
			return tables;
		}
		
		
		public bool GetAlternates(ushort glyph, out ushort[] alternates)
		{
			//	Trouve les variantes équivalentes d'un glyph donné. Il y a par
			//	exemple plusieurs variantes possibles pour un "&" dans une fonte
			//	et c'est ainsi que l'on peut avoir la liste des autres glyphes.
			
			System.Diagnostics.Debug.Assert (this.ot_GSUB != null);
			
			System.Collections.ArrayList list;
			
			if (this.alternate_lookups == null)
			{
				list = new System.Collections.ArrayList ();
				
				foreach (string feature in this.active_features.Split ('/'))
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
				
				this.alternate_lookups = (BaseSubstitution[]) list.ToArray (typeof (BaseSubstitution));
			}
			
			list = null;
			
			foreach (BaseSubstitution subst in this.alternate_lookups)
			{
				if (subst.Coverage.FindIndex (glyph) >= 0)
				{
					if (list == null)
					{
						list = new System.Collections.ArrayList ();
					}
					
					AlternateSubstitution alternate = subst as AlternateSubstitution;
					SingleSubstitution    single    = subst as SingleSubstitution;
					
					if (alternate != null)
					{
						ushort[] subset = alternate.GetAlternates (glyph);
						
						System.Diagnostics.Debug.Assert (subset != null);
						System.Diagnostics.Debug.Assert (subset.Length > 0);
						
						list.AddRange (subset);
					}
					if (single != null)
					{
						list.Add (single.FindSubstitution (glyph));
					}
				}
			}
			
			if (list == null)
			{
				alternates = null;
				return false;
			}
			else
			{
				alternates = new ushort[list.Count];
				list.CopyTo (alternates, 0);
				return true;
			}
		}
		
		
		public System.IntPtr GetFontHandle(double size)
		{
			if (this.use_system_glyph_size)
			{
				FontIdentity.SizeInfo info = this.identity.GetSizeInfo ((int)(size + 0.5));
				return info.Handle.Handle;
			}
			
			return System.IntPtr.Zero;
		}
		
		
		public double GetAscender(double size)
		{
			if (this.use_system_glyph_size)
			{
				FontIdentity.SizeInfo info = this.identity.GetSizeInfo ((int)(size + 0.5));
				return info.Ascender;
			}
			else
			{
				double scale = size / this.ot_head.UnitsPerEm;
				
				return this.ot_hhea.MacAscender * scale;
			}
		}
		
		public double GetDescender(double size)
		{
			if (this.use_system_glyph_size)
			{
				FontIdentity.SizeInfo info = this.identity.GetSizeInfo ((int)(size + 0.5));
				return info.Descender;
			}
			else
			{
				double scale = size / this.ot_head.UnitsPerEm;
				
				return this.ot_hhea.MacDescender * scale;
			}
		}
		
		public double GetCaretAngle()
		{
			double dx = this.ot_hhea.CaretSlopeRun;
			double dy = this.ot_hhea.CaretSlopeRise;
			
			return System.Math.Atan2 (dy, dx);
		}
		
		
		public ulong GetHyphen()
		{
			return '-';
		}
		
		
		public ushort GetGlyphIndex(char code)
		{
			return this.GetGlyphIndex ((int) code);
		}
		
		public ushort GetGlyphIndex(int code)
		{
			if ((code == 0) ||
				(this.ot_index_mapping == null))
			{
				return 0xffff;
			}
			
			ushort glyph = this.ot_index_mapping.GetGlyphIndex (code);
			
			if (glyph == 0x0000)
			{
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
						glyph = this.ot_index_mapping.GetGlyphIndex ((int) this.GetHyphen ());
						break;
				}
			}
			
			return glyph;
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
			System.Diagnostics.Debug.Assert (this.ot_GSUB != null);
			
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
			
			this.substitution_lookups = new BaseSubstitution[list.Count];
			list.CopyTo (this.substitution_lookups);
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
				
				if ((this.substitution_lookups != null) &&
					(this.substitution_lookups.Length > 0))
				{
					for (int i = 0; i < this.substitution_lookups.Length; i++)
					{
						Font.ApplySubstitutions (this.substitution_lookups[i], input, length, output, out length, ref gl_map);
						
						input   = output;
						output  = temp[toggle & 1];
						toggle += 1;
					}
				}
				
				if (this.map_default_ligatures)
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
			if (this.ot_index_mapping == null)
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
			
			ushort glyph_f = this.ot_index_mapping.GetGlyphIndex ('f');
			ushort glyph_i = this.ot_index_mapping.GetGlyphIndex ('i');
			ushort glyph_l = this.ot_index_mapping.GetGlyphIndex ('l');
			ushort lig_ff  = this.ot_index_mapping.GetGlyphIndex (0xfb00);
			ushort lig_fi  = this.ot_index_mapping.GetGlyphIndex (0xfb01);
			ushort lig_fl  = this.ot_index_mapping.GetGlyphIndex (0xfb02);
			ushort lig_ffi = this.ot_index_mapping.GetGlyphIndex (0xfb03);
			ushort lig_ffl = this.ot_index_mapping.GetGlyphIndex (0xfb04);
			
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
			if ((this.use_kerning) &&
				(glyph < num_glyph) &&
				(prev_glyph < num_glyph) &&
				(this.ot_kern_fmt_0 != null) &&
				(this.ot_kern_fmt_0.FindKernValue (prev_glyph, glyph, out delta)))
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
				
				if (this.ApplyKerningInformation (glyph, ref prev_glyph, num_glyph, out delta))
				{
					advance += delta;
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
						advance += this.ot_hmtx.GetAdvanceWidth (glyph);
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
					advance = this.ot_hmtx.GetAdvanceWidth (glyph);
				}
				else
				{
					advance = this.ot_hmtx.GetAdvanceWidth (num_h_metrics-1);
				}
			}
			
			x = advance * pos * scale / simple_glyph_count;
			y = 0.0;
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
		private FontData						font_data;
		
		private Table_GSUB						ot_GSUB;
		private Table_GDEF						ot_GDEF;
		private Table_cmap						ot_cmap;
		private Table_maxp						ot_maxp;
		private Table_head						ot_head;
		private Table_hhea						ot_hhea;
		private Table_hmtx						ot_hmtx;
		private KerningTableFormat0				ot_kern_fmt_0;
		private IndexMappingTable				ot_index_mapping;
		
		private string							active_script;
		private string							active_language;
		private string							active_features;
		
		private bool							map_default_ligatures;
		private bool							use_kerning;
		private bool							use_system_glyph_size;
		
		private TaggedFeatureTable				script_required_feature;
		private TaggedFeatureTable[]			script_optional_features;
		private BaseSubstitution[]				substitution_lookups;
		private BaseSubstitution[]				alternate_lookups;
		
		private System.Collections.Stack		saved_features_stack;
		private ushort							space_glyph;
	}
}
