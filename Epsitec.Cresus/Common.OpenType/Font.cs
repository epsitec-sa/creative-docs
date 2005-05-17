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
		
		
		public FontIdentity						FontIdentity
		{
			get
			{
				return this.identity;
			}
		}
		
		
		internal void Initialize(FontIdentity identity)
		{
			this.identity = identity;
			this.Initialize (this.identity.FontData);
		}
		
		internal void Initialize(FontData font_data)
		{
			TableEntry entry;
			
			this.font_data = font_data;
			
			this.ot_GSUB = new Table_GSUB (this.font_data["GSUB"]);
			this.ot_GDEF = new Table_GDEF (this.font_data["GDEF"]);
			this.ot_cmap = new Table_cmap (this.font_data["cmap"]);
			this.ot_maxp = new Table_maxp (this.font_data["maxp"]);
			this.ot_head = new Table_head (this.font_data["head"]);
			this.ot_hhea = new Table_hhea (this.font_data["hhea"]);
			this.ot_hmtx = new Table_hmtx (this.font_data["hmtx"]);
			
			entry = this.font_data["kern"];
			
			if (entry != null)
			{
				Table_kern ot_kern = new Table_kern (entry);
				
				if ((ot_kern.Count > 0) &&
					(ot_kern.GetKerningTable (0).SubtableFormat == 0))
				{
					this.ot_kern_fmt_0 = ot_kern.GetKerningTable (0).Format0Subtable;
				}
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
				glyphs[i] = this.ot_index_mapping.GetGlyphIndex (text[i]);
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
				int code  = 0x001fffff & (int) text[start+i];
				glyphs[i] = this.ot_index_mapping.GetGlyphIndex (code);
			}
			
			this.ApplySubstitutions (ref glyphs, ref gl_map);
			
			return glyphs;
		}
		
		public void GenerateGlyphs(string text, out ushort[] glyphs, byte[] attributes)
		{
			int   length = text.Length;
			int[] gl_map;
			
			glyphs = new ushort[length];
			gl_map = new int[length];
			
			for (int i = 0; i < length; i++)
			{
				glyphs[i] = this.ot_index_mapping.GetGlyphIndex (text[i]);
			}
			
			this.ApplySubstitutions (ref glyphs, ref gl_map);
			
			length = glyphs.Length;
			
			int src = 0;
			int dst = 0;
			
			for (int i = 0; i < length; i++)
			{
				attributes[dst] = attributes[src];
				
				dst += 1;
				src += gl_map[i] + 1;
			}
		}
		
		public void GenerateGlyphs(ulong[] text, int start, int length, out ushort[] glyphs, byte[] attributes)
		{
			int[] gl_map;
			
			glyphs = new ushort[length];
			gl_map = new int[length];
			
			for (int i = 0; i < length; i++)
			{
				int code  = 0x001fffff & (int) text[start+i];
				glyphs[i] = this.ot_index_mapping.GetGlyphIndex (code);
			}
			
			this.ApplySubstitutions (ref glyphs, ref gl_map);
			
			length = glyphs.Length;
			
			int src = 0;
			int dst = 0;
			
			for (int i = 0; i < length; i++)
			{
				attributes[dst] = attributes[src];
				
				dst += 1;
				src += gl_map[i] + 1;
			}
		}
		
		public void GenerateGlyphs(ulong[] text, int start, int length, out ushort[] glyphs, short[] attributes)
		{
			int[] gl_map;
			
			glyphs = new ushort[length];
			gl_map = new int[length];
			
			for (int i = 0; i < length; i++)
			{
				int code  = 0x001fffff & (int) text[start+i];
				glyphs[i] = this.ot_index_mapping.GetGlyphIndex (code);
			}
			
			this.ApplySubstitutions (ref glyphs, ref gl_map);
			
			length = glyphs.Length;
			
			int src = 0;
			int dst = 0;
			
			for (int i = 0; i < length; i++)
			{
				attributes[dst] = attributes[src];
				
				dst += 1;
				src += gl_map[i] + 1;
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
					width += info.GetGlyphWidth (glyphs[i]);
				}
				
				return width;
			}
			
			int num_glyph     = this.ot_maxp.NumGlyphs;
			int num_h_metrics = this.ot_hhea.NumHMetrics;
			
			double scale      = size / this.ot_head.UnitsPerEm;
			int    advance    = 0;
			ushort prev_glyph = 0xffff;
			
			for (int i = 0; i < glyphs.Length; i++)
			{
				ushort glyph = glyphs[i];
				
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
				
				this.ApplyKerningInformation (glyph, ref prev_glyph, ref advance, num_glyph);
			}
			
			return advance * scale;
		}
		
		public double GetPositions(ushort[] glyphs, double size, double ox, double[] x_pos)
		{
			//	TODO: gérer use_system_glyph_size
			
			int num_glyph     = this.ot_maxp.NumGlyphs;
			int num_h_metrics = this.ot_hhea.NumHMetrics;
			
			double scale      = size / this.ot_head.UnitsPerEm;
			int    advance    = 0;
			ushort prev_glyph = 0xffff;
			
			for (int i = 0; i < glyphs.Length; i++)
			{
				ushort glyph = glyphs[i];
				
				x_pos[i] = ox + advance * scale;
				
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
				
				this.ApplyKerningInformation (glyph, ref prev_glyph, ref advance, num_glyph);
			}
			
			return advance * scale;
		}
		
		public double GetPositions(ushort[] glyphs, double size, double ox, double[] x_pos, double[] x_scale)
		{
			//	TODO: gérer use_system_glyph_size
			
			int num_glyph     = this.ot_maxp.NumGlyphs;
			int num_h_metrics = this.ot_hhea.NumHMetrics;
			
			double scale      = size / this.ot_head.UnitsPerEm;
			double advance    = 0;
			ushort prev_glyph = 0xffff;
			
			for (int i = 0; i < glyphs.Length; i++)
			{
				ushort glyph = glyphs[i];
				
				x_pos[i] = ox + advance;
				
				if (glyph < num_glyph)
				{
					if (glyph < num_h_metrics)
					{
						advance += this.ot_hmtx.GetAdvanceWidth (glyph) * x_scale[i] * scale;
					}
					else
					{
						advance += this.ot_hmtx.GetAdvanceWidth (num_h_metrics-1) * x_scale[i] * scale;
					}
				}
				
				int int_advance = 0;
				
				if (this.ApplyKerningInformation (glyph, ref prev_glyph, ref int_advance, num_glyph))
				{
					advance += int_advance * x_scale[i] * scale;
				}
			}
			
			return advance;
		}
		
		public double GetPositions(ushort[] glyphs, double size, double ox, double oy, double[] x_pos, double[] y_pos)
		{
			//	TODO: gérer use_system_glyph_size
			
			int num_glyph     = this.ot_maxp.NumGlyphs;
			int num_h_metrics = this.ot_hhea.NumHMetrics;
			
			double scale      = size / this.ot_head.UnitsPerEm;
			int    advance    = 0;
			ushort prev_glyph = 0xffff;
			
			for (int i = 0; i < glyphs.Length; i++)
			{
				ushort glyph = glyphs[i];
				
				x_pos[i] = ox + advance * scale;
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
				
				this.ApplyKerningInformation (glyph, ref prev_glyph, ref advance, num_glyph);
			}
			
			return advance * scale;
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
			
			if (this.ot_GSUB.ScriptListTable.ContainsScript (script))
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
		}
		
		public void SelectFeatures(params string[] features)
		{
			string collapsed_features = string.Join ("/", features);
			
			if ((this.active_features == collapsed_features) &&
				(this.substitution_lookups != null))
			{
				return;
			}
			
			this.active_features = collapsed_features;
			
			FeatureListTable             feature_list    = this.ot_GSUB.FeatureListTable;
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
			
			if (this.script_required_feature != null)
			{
				active_features.Add (this.script_required_feature);
			}
			
			if (this.script_optional_features == null)
			{
				int n = feature_list.FeatureCount;
				
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
			
			this.GenerateSubstitutionLookups (active_features);
		}
		
		public void SelectFontManager(string manager)
		{
			switch (manager)
			{
				case "OpenType":
					this.use_system_glyph_size = false;
					break;
				
				case "System":
					this.use_system_glyph_size = true;
					break;
			}
		}
		
		
		public string[] GetSupportedScripts()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			ScriptListTable script_list = this.ot_GSUB.ScriptListTable;
			int n = script_list.ScriptCount;
			
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
				FeatureListTable feature_list = this.ot_GSUB.FeatureListTable;
				System.Collections.Hashtable hash = new System.Collections.Hashtable ();
				
				int n = feature_list.FeatureCount;
				
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
		
		
		public double GetAscender(double size)
		{
			double scale = size / this.ot_head.UnitsPerEm;
			
			return this.ot_hhea.MacAscender * scale;
		}
		
		public double GetDescender(double size)
		{
			double scale = size / this.ot_head.UnitsPerEm;
			
			return this.ot_hhea.MacDescender * scale;
		}
		
		
		public ulong GetHyphen()
		{
			return '-';
		}
		
		
		private void MapToGlyphs(string text, out ushort[] glyphs, out int[] gl_map)
		{
			int length = text.Length;
			
			glyphs = new ushort[length];
			gl_map = new int[length];
			
			for (int i = 0; i < length; i++)
			{
				glyphs[i] = this.ot_index_mapping.GetGlyphIndex (text[i]);
			}
			
			this.ApplySubstitutions (ref glyphs, ref gl_map);
		}
		
		private void MapToGlyphs(ulong[] text, int start, int length, out ushort[] glyphs, out int[] gl_map)
		{
			glyphs = new ushort[length];
			gl_map = new int[length];
			
			for (int i = 0; i < length; i++)
			{
				int code  = 0x001fffff & (int) text[start+i];
				glyphs[i] = this.ot_index_mapping.GetGlyphIndex (code);
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
			
			int count = 0;
			int index = 0;
			
			foreach (int lookup in lookup_indexes)
			{
				count += (int) this.ot_GSUB.LookupListTable.GetLookupTable (lookup).SubTableCount;
			}
			
			this.substitution_lookups = new BaseSubstitution[count];
			
			foreach (int lookup in lookup_indexes)
			{
				LookupTable lookup_table = this.ot_GSUB.LookupListTable.GetLookupTable (lookup);
				
				int n = lookup_table.SubTableCount;
				
				for (int i = 0; i < n; i++)
				{
					this.substitution_lookups[index++] = lookup_table.GetSubstitution (i);
				}
			}
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
						this.ApplySubstitutions (this.substitution_lookups[i], input, length, output, out length, ref gl_map);
						
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
		
		private void ApplySubstitutions(BaseSubstitution substitution, ushort[] input_glyphs, int input_length, ushort[] output_glyphs, out int output_length, ref int[] gl_map)
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
		
		private bool ApplyKerningInformation(ushort glyph, ref ushort prev_glyph, ref int advance, int num_glyph)
		{
			int adjust;
			
			if ((this.use_kerning) &&
				(glyph < num_glyph) &&
				(prev_glyph < num_glyph) &&
				(this.ot_kern_fmt_0 != null) &&
				(this.ot_kern_fmt_0.FindKernValue (prev_glyph, glyph, out adjust)))
			{
				advance += adjust;
				prev_glyph = glyph;
				return true;
			}
			else
			{
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
				
				this.ApplyKerningInformation (glyph, ref prev_glyph, ref advance, num_glyph);
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
			
			LigatureCaretListTable caret_list = this.ot_GDEF.LigatureCaretListTable;
			
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
	}
}
