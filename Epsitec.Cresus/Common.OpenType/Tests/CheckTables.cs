//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType.Tests
{
	/// <summary>
	/// Summary description for CheckTables.
	/// </summary>
	public sealed class CheckTables
	{
		public static void RunTests()
		{
			FontCollection collection = new FontCollection ();
			
			System.Diagnostics.Trace.WriteLine ("Building font collection.");
			collection.Initialize ();
			System.Diagnostics.Trace.WriteLine ("Done.");
			
			foreach (FontIdentity font in collection)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("{0}/{1} ({2}/{3}) weight={5}, italic={6} ---> {4}", font.InvariantFaceName, font.InvariantStyleName, font.LocaleFaceName, font.LocaleStyleName, font.FullName, font.FontWeight, font.FontIsItalic));
			}
			
			Font         arial    = collection.CreateFont ("Arial Unicode MS");
			FontIdentity arial_id = arial.FontIdentity;
			
			int[] sizes = new int[] { 8, 9, 10, 11, 12, 24, 120, 2048 };
			
			foreach (int size in sizes)
			{
				ushort[] glyphs = arial.GenerateGlyphs ("Affiche un petit texte pour vérifier le bon fonctionnement du calcul des largeurs.");
				
				arial.SelectFontManager ("System");
				double system = arial.GetTotalWidth (glyphs, size);
				
				arial.SelectFontManager ("OpenType");
				double perfect = arial.GetTotalWidth (glyphs, size);
				
				System.Diagnostics.Debug.WriteLine (string.Format ("{0}, size {4}, text width is {1}, perfect is {2}, delta is {3:0.00}%", arial_id.FullName, system, perfect, 100*system/perfect-100, size));
			}
			
			CheckTables.TestFeatureTable ();
			CheckTables.TestArial ();
			CheckTables.TestFont ();
		}
		
		
		private static void TestArial()
		{
			string font = "Arial Unicode MS";
			byte[] data = Platform.Win32.LoadFontData (font, "Normal");
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Loaded font {0}: length {1}", font, data.Length));
			
			TableDirectory td = new TableDirectory (data, 0);
			
			for (int i = 0; i < (int) td.NumTables; i++)
			{
				TableEntry entry = td.GetEntry (i);
				System.Diagnostics.Debug.WriteLine (string.Format ("Table {0}, offset={1}, length={2}.", entry.Tag, entry.Offset, entry.Length));
			}
			
			TableEntry name_e = td.FindTable ("name");
			Table_name name_t = new Table_name (data, (int) name_e.Offset);
			
			System.Array name_ids = System.Enum.GetValues (typeof (OpenType.NameId));
			System.Array plat_ids = System.Enum.GetValues (typeof (OpenType.PlatformId));
			
			Table_name.NameEncoding[] encodings = name_t.GetAvailableNameEncodings ();
			
			foreach (Table_name.NameEncoding encoding in encodings)
			{
				string latin_name   = name_t.GetLatinName (encoding.Language, encoding.Name, encoding.Platform);
				string unicode_name = name_t.GetUnicodeName (encoding.Language, encoding.Name, encoding.Platform);
					
				if (latin_name != null)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("{0}/{1} : {2}", encoding.Platform, encoding.Name, latin_name));
				}
				if (unicode_name != null)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("{0}/{1} : {2} -- {3}", encoding.Platform, encoding.Name, unicode_name, encoding.Language));
				}
				
				if (latin_name == unicode_name)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Error: {0}/{1}.", encoding.Platform, encoding.Name));
				}
			}
			
			string s_arial_family = name_t.GetUnicodeName (0x0409, OpenType.NameId.FontFamily, OpenType.PlatformId.Microsoft);
			string s_arial_sub_de = name_t.GetUnicodeName (0x0407, OpenType.NameId.FontSubfamily, OpenType.PlatformId.Microsoft);
			string s_arial_sub_fr = name_t.GetUnicodeName (0x040c, OpenType.NameId.FontSubfamily, OpenType.PlatformId.Microsoft);
			
			Debug.Assert.IsTrue (s_arial_family == "Arial Unicode MS");
			Debug.Assert.IsTrue (s_arial_sub_de == "Standard");
			Debug.Assert.IsTrue (s_arial_sub_fr == "Normal");
			
			Table_maxp maxp_t = new Table_maxp (data, (int) td.FindTable ("maxp").Offset);
			Table_hhea hhea_t = new Table_hhea (data, (int) td.FindTable ("hhea").Offset);
			Table_hmtx hmtx_t = new Table_hmtx (data, (int) td.FindTable ("hmtx").Offset);
			Table_cmap cmap_t = new Table_cmap (data, (int) td.FindTable ("cmap").Offset);
			
			IndexMappingTable cmap_sub_t = cmap_t.FindFormatSubTable ();
			
			int total = 0;
			int width = 0;
			
			System.Diagnostics.Trace.WriteLine ("Measuring text width.");
			
			for (int i = 0; i < 100; i++)
			{
				for (char c = ' '; c < '\u01F6'; c++)
				{
					int c_index   = (int) c;
					
					if ((c_index >= 127) && (c_index < 160))
					{
						continue;
					}
					
					int glyph     = cmap_sub_t.GetGlyphIndex ((int) c);
					
					if (glyph == 0)
					{
//						System.Diagnostics.Debug.WriteLine (string.Format ("Missing glyph for '{0}', unicode {1}", c, c_index.ToString ("X4")));
						continue;
					}
					
					total++;
					
					int num_glyph = maxp_t.NumGlyphs;
					int num_h_metrics = hhea_t.NumHMetrics;
					
					int advance = 0;
					int  lsb     = 0;
					
					if (glyph < num_glyph)
					{
						if (glyph < num_h_metrics)
						{
							advance = hmtx_t.GetAdvanceWidth (glyph);
							lsb     = hmtx_t.GetLeftSideBearing (glyph);
						}
						else
						{
							advance = hmtx_t.GetAdvanceWidth (num_h_metrics-1);
							lsb     = hmtx_t.GetExtraLeftSideBearing (num_h_metrics, glyph - num_h_metrics);
						}
					}
					
					width += (int) advance;
					width += lsb;
					
//					System.Diagnostics.Debug.WriteLine (string.Format ("{3} =\t{0:000}: {1} {2}", glyph, advance, lsb, c));
				}			
			}
			
			System.Diagnostics.Trace.WriteLine ("Done, " + total + " glyphs.");
			System.Diagnostics.Trace.WriteLine ("Width is " + width + " units.");
		}
		
		private static void TestFeatureTable()
		{
			string font = "Palatino Linotype";
			byte[] data = Platform.Win32.LoadFontData (font, "Normal");
			
			TableDirectory td = new TableDirectory (data, 0);
			Table_GSUB gsub_t = new Table_GSUB (td.FindTable ("GSUB"));
			
			ScriptListTable  script_l  = gsub_t.ScriptListTable;
			FeatureListTable feature_l = gsub_t.FeatureListTable;
			
			System.Collections.Hashtable referenced_features = new System.Collections.Hashtable ();
			
			for (int i = 0; i < script_l.ScriptCount; i++)
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				buffer.Append (script_l.GetScriptTag (i));
				buffer.Append (": ");
				
				ScriptTable script_t = script_l.GetScriptTable (i);
				
				buffer.Append ("default=[");
				
				for (int k = 0; k < script_t.DefaultLangSysTable.FeatureCount; k++)
				{
					if (k > 0) buffer.Append (";");
					buffer.Append (script_t.DefaultLangSysTable.GetFeatureIndex (k));
					referenced_features[(int) script_t.DefaultLangSysTable.GetFeatureIndex (k)] = true;
				}
				
				buffer.Append ("]");
				
				for (int j = 0; j < script_t.LangSysCount; j++)
				{
					buffer.Append (", '");
					buffer.Append (script_t.GetLangSysTag (j));
					buffer.Append ("' f=[");
					buffer.Append ((short) script_t.GetLangSysTable (j).RequiredFeatureIndex);
					
					if (script_t.GetLangSysTable (j).RequiredFeatureIndex != 0xffff)
					{
						referenced_features[(int) script_t.GetLangSysTable (j).RequiredFeatureIndex] = true;
					}
					
					for (int k = 0; k < script_t.GetLangSysTable (j).FeatureCount; k++)
					{
						buffer.Append (";");
						buffer.Append (script_t.GetLangSysTable (j).GetFeatureIndex (k));
						referenced_features[(int) script_t.GetLangSysTable (j).GetFeatureIndex (k)] = true;
					}
					
					buffer.Append ("]");
				}
				
				System.Diagnostics.Debug.WriteLine (buffer.ToString ());
			}
			
			System.Diagnostics.Debug.WriteLine (string.Format ("{0} features, {1} referenced.", feature_l.FeatureCount, referenced_features.Count));
			
			
			string[] feature_names = new string[feature_l.FeatureCount];
			int[]   feature_index = new int[feature_l.FeatureCount];
			
			for (int i = 0; i < feature_l.FeatureCount; i++)
			{
				feature_names[i] = feature_l.GetFeatureTag (i) + "-" + i.ToString ("X4");
				feature_index[i] = i;
			}
			
			System.Array.Sort (feature_names, feature_index);
			
			for (int i = 0; i < feature_index.Length; i++)
			{
				string       feature_n = feature_l.GetFeatureTag (feature_index[i]);
				FeatureTable feature_t = feature_l.GetFeatureTable (feature_index[i]);
				
				System.Diagnostics.Debug.WriteLine (string.Format ("{0}-{2:000}: {1} lookups.{3}", feature_n, feature_t.LookupCount, feature_index[i], referenced_features.Contains ((int)i) ? "" : " Not referenced."));
				
			}
			
			int rq_feature_arab_urdu = gsub_t.GetRequiredFeatureIndex ("arab", "URD ");
			int[] features_arab_urdu = gsub_t.GetFeatureIndexes ("arab", "URD ");
			
			int rq_feature_arab_xxxx = gsub_t.GetRequiredFeatureIndex ("arab", "XXXX");
			int[] features_arab_xxxx = gsub_t.GetFeatureIndexes ("arab", "XXXX");
			
			int[] fina_features = gsub_t.GetFeatureIndexes ("fina");
			
//			Debug.Assert.IsTrue (fina_features.Length == 3);
//			Debug.Assert.IsTrue (fina_features[0] == 3);
//			Debug.Assert.IsTrue (fina_features[1] == 6);
//			Debug.Assert.IsTrue (fina_features[2] == 11);
			
			LookupListTable gsub_lookup = gsub_t.LookupListTable;
			
			int[] liga_features = gsub_t.GetFeatureIndexes ("liga");
			int[] rlig_features = gsub_t.GetFeatureIndexes ("rlig");
			int[] dlig_features = gsub_t.GetFeatureIndexes ("dlig");
			
			System.Collections.ArrayList ligature_list = new System.Collections.ArrayList ();
			
			ligature_list.AddRange (liga_features);
			ligature_list.AddRange (rlig_features);
			ligature_list.AddRange (dlig_features);
			
			for (int i = 0; i < ligature_list.Count; i++)
			{
				int         feature_i    = (int)ligature_list[i];
				FeatureTable feature      = gsub_t.FeatureListTable.GetFeatureTable (feature_i);
				int         lookup_count = feature.LookupCount;
				
				for (int feature_lookup_i = 0; feature_lookup_i < lookup_count; feature_lookup_i++)
				{
					LookupTable lookup = gsub_lookup.GetLookupTable (feature.GetLookupIndex (feature_lookup_i));
					
					System.Diagnostics.Debug.WriteLine (string.Format ("Lookup: type={0}, flags={1}, sub.count={2}", lookup.LookupType, lookup.LookupFlags.ToString ("X4"), lookup.SubTableCount));
					
					if (lookup.LookupType == 1)
					{
						for (int s = 0; s < lookup.SubTableCount; s++)
						{
							SubstSubTable base_subst = lookup.GetSubTable (s);
							SingleSubstitution single_subst = new SingleSubstitution (base_subst);
							
							int[] covered = CheckTables.GetCoverageIndexes (single_subst.Coverage);
							
							System.Diagnostics.Debug.WriteLine (string.Format ("  Subtable: format {0}, coverage format {1}, covered {2}", single_subst.SubstFormat, single_subst.Coverage.CoverageFormat, covered.Length));
							
							for (int c = 0; c < covered.Length; c++)
							{
								System.Diagnostics.Debug.WriteLine (string.Format ("Replace {0:0000} --> {1:0000}", covered[c], single_subst.FindSubstitution (covered[c])));
							}
						}
					}
					else if (lookup.LookupType == 4)
					{
						for (int s = 0; s < lookup.SubTableCount; s++)
						{
							SubstSubTable base_subst = lookup.GetSubTable (s);
							LigatureSubstitution liga_subst = new LigatureSubstitution (base_subst);
							
							int[] covered = CheckTables.GetCoverageIndexes (liga_subst.Coverage);
							
							System.Diagnostics.Debug.WriteLine (string.Format ("  Subtable: format {0}, coverage format {1}, covered {2}", liga_subst.SubstFormat, liga_subst.Coverage.CoverageFormat, covered.Length));
							System.Diagnostics.Debug.WriteLine (string.Format ("            # of ligature sets: {0}", liga_subst.LigatureSetCount));
							
							Debug.Assert.IsTrue (liga_subst.LigatureSetCount == covered.Length);
							
							for (int j = 0; j < liga_subst.LigatureSetCount; j++)
							{
								LigatureSet liga_set = liga_subst.GetLigatureSet (j);
								
								for (int k = 0; k < liga_set.LigatureInfoCount; k++)
								{
									System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
									
									LigatureInfo ligature = liga_set.GetLigatureInfo (k);
									int result_glyph = ligature.Glyph;
									
									buffer.Append ("  o ");
									buffer.Append (covered[j]);
									
									for (int l = 1; l < ligature.ComponentCount; l++)
									{
										buffer.Append ("+");
										buffer.Append (ligature.GetComponent (l-1));
									}
									
									buffer.Append (" -> ");
									buffer.Append (ligature.Glyph);
									
									System.Diagnostics.Debug.WriteLine (buffer.ToString ());
								}
							}
							
							ushort[] glyphs = new ushort[] { 143, 143, 139, 143, 143, 12, 143, 147, 157, 157, 25, 0, 0, 0 };
							ushort[] result = new ushort[20];
							
							System.Diagnostics.Trace.WriteLine ("Starting ligature substitution:");
							for (int zz = 0; zz < 1000; zz++)
							{
								int i_offset = 0;
								int o_offset = 0;
								int length   = glyphs.Length;
								
								liga_subst.ProcessSubstitution (glyphs, ref i_offset, length, result, ref o_offset);
								liga_subst.ProcessSubstitution (glyphs, ref i_offset, length, result, ref o_offset);
								liga_subst.ProcessSubstitution (glyphs, ref i_offset, length, result, ref o_offset); i_offset++; o_offset++;
								liga_subst.ProcessSubstitution (glyphs, ref i_offset, length, result, ref o_offset);
								liga_subst.ProcessSubstitution (glyphs, ref i_offset, length, result, ref o_offset);
							}
							System.Diagnostics.Trace.WriteLine ("Done.");
						}
					}
					else if (lookup.LookupType == 6)
					{
						for (int s = 0; s < lookup.SubTableCount; s++)
						{
							SubstSubTable base_subst = lookup.GetSubTable (s);
							ChainingContextSubstitution cctx_subst = new ChainingContextSubstitution (base_subst);
							
							int[] covered = CheckTables.GetCoverageIndexes (cctx_subst.Coverage);
							
							System.Diagnostics.Debug.WriteLine (string.Format ("  Subtable: format {0}, coverage format {1}, covered {2}", cctx_subst.SubstFormat, cctx_subst.Coverage.CoverageFormat, covered.Length));
						}
					}
				}
			}
		}
		
		private static void TestFont()
		{
			string font_name_1 = "Palatino Linotype";
			string font_name_2 = "Times New Roman";
			
			Font font_1 = new Font ();
			Font font_2 = new Font ();
			
			font_1.Initialize (new FontData (Platform.Win32.LoadFontData (font_name_1, "Normal")));
			font_2.Initialize (new FontData (Platform.Win32.LoadFontData (font_name_2, "Normal")));
			
			System.Diagnostics.Debug.WriteLine (string.Join ("; ", font_1.GetSupportedScripts ()));
			System.Diagnostics.Debug.WriteLine (string.Join ("; ", font_2.GetSupportedScripts ()));
			System.Diagnostics.Debug.WriteLine (string.Join ("; ", font_1.GetSupportedFeatures ()));
			System.Diagnostics.Debug.WriteLine (string.Join ("; ", font_2.GetSupportedFeatures ()));
			
			font_1.SelectFeatures ("dlig", "liga");
			
			font_1.SelectScript ("latn", "ROM ");
			font_2.SelectScript ("arab");
			
			font_1.SelectFeatures ("dlig", "liga");
			
			System.Diagnostics.Debug.WriteLine (string.Join ("; ", font_1.GetSupportedFeatures ()));
			System.Diagnostics.Debug.WriteLine (string.Join ("; ", font_2.GetSupportedFeatures ()));
			
			font_2.SelectScript ("latn");
			font_2.SelectFeatures ("liga");
			
			string text = "Quelle fin affreuse, affligeant !";
			
			ushort[] glyphs_1a = font_1.GenerateGlyphs (text);
			ushort[] glyphs_2a = font_2.GenerateGlyphs (text);
			
			ushort[] glyphs_1aa;
			ushort[] glyphs_2aa;
			byte[] attrib_1aa = new byte[text.Length];
			byte[] attrib_2aa = new byte[text.Length];
			
			for (int i = 0; i < text.Length; i++)
			{
				attrib_1aa[i] = (byte) (i+1);
				attrib_2aa[i] = (byte) (i+1);
			}
			
			font_1.GenerateGlyphs(text, out glyphs_1aa, attrib_1aa);
			font_2.GenerateGlyphs(text, out glyphs_2aa, attrib_2aa);
			
			Debug.Assert.IsTrue (attrib_1aa[0] == 1);
			Debug.Assert.IsTrue (attrib_1aa[1] == 3);
			Debug.Assert.IsTrue (attrib_1aa[18] == 22);
			Debug.Assert.IsTrue (attrib_1aa[19] == 23);
			Debug.Assert.IsTrue (attrib_1aa[20] == 26);
			
			Debug.Assert.IsTrue (attrib_2aa[0] == 1);
			Debug.Assert.IsTrue (attrib_2aa[1] == 2);
			Debug.Assert.IsTrue (attrib_2aa[22] == 24);
			Debug.Assert.IsTrue (attrib_2aa[23] == 26);
			
			double w1a = font_1.GetTotalWidth (glyphs_1a, 10.0);
			double w2a = font_2.GetTotalWidth (glyphs_2a, 10.0);
			
			double[] x_pos_1c = new double[glyphs_1a.Length];
			double[] x_pos_2c = new double[glyphs_2a.Length];
			
			double w1c = font_1.GetPositions (glyphs_1a, 10.0, 0.0, x_pos_1c);
			double w2c = font_1.GetPositions (glyphs_1a, 10.0, 0.0, x_pos_2c);
			
			double x, y;
			
			font_1.HitTest (text, 10.0, 24, out x, out y);
			font_2.HitTest (text, 10.0, 24, out x, out y);
			
			font_1.HitTest (text, 10.0, 0, out x, out y);
			font_1.HitTest (text, 10.0, 1, out x, out y);
			font_1.HitTest (text, 10.0, 2, out x, out y);
			font_1.HitTest (text, 10.0, text.Length, out x, out y);
			
			int    pos;
			double subpos;
			
			font_1.HitTest (text, 10.0,  0.0, 0.0, out pos, out subpos);
			font_1.HitTest (text, 10.0,  2.0, 0.0, out pos, out subpos);
			font_1.HitTest (text, 10.0,  4.0, 0.0, out pos, out subpos);
			font_1.HitTest (text, 10.0,  6.0, 0.0, out pos, out subpos);
			font_1.HitTest (text, 10.0,  8.0, 0.0, out pos, out subpos);
			font_1.HitTest (text, 10.0, 10.0, 0.0, out pos, out subpos);
			font_1.HitTest (text, 10.0, 12.0, 0.0, out pos, out subpos);
			font_1.HitTest (text, 10.0, 14.0, 0.0, out pos, out subpos);
			
			font_1.HitTest (text, 10.0, x, y, out pos, out subpos);
			
			Debug.Assert.IsTrue (pos == text.Length);
			
			font_1.SelectFeatures ();
			font_2.SelectFeatures ();
			
			ushort[] glyphs_1b = font_1.GenerateGlyphs (text);
			ushort[] glyphs_2b = font_2.GenerateGlyphs (text);
			
			double w1b = font_1.GetTotalWidth (glyphs_1b, 10.0);
			double w2b = font_2.GetTotalWidth (glyphs_2b, 10.0);
			
			double asc1 = font_1.GetAscender (10.0);
			double dsc1 = font_1.GetDescender (10.0);
			
			Debug.Assert.IsTrue (asc1 > 0);
			Debug.Assert.IsTrue (dsc1 < 0);
			
			double asc2 = font_2.GetAscender (10.0);
			double dsc2 = font_2.GetDescender (10.0);
			
			Debug.Assert.IsTrue (asc2 > 0);
			Debug.Assert.IsTrue (dsc2 < 0);
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
	}
}
