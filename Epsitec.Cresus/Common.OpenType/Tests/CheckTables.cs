//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			//			Platform.Win32.LoadFontDataDrawing ();
			
			CheckTables.TestFeatureTable ();
			CheckTables.TestArial ();
		}
		
		
		private static void TestArial()
		{
			string font = "Arial Unicode MS";
			byte[] data = Platform.Win32.LoadFontData (font);
			
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
					uint c_index   = (uint) c;
					
					if ((c_index >= 127) && (c_index < 160))
					{
						continue;
					}
					
					uint glyph     = cmap_sub_t.GetGlyphIndex ((int) c);
					
					if (glyph == 0)
					{
//						System.Diagnostics.Debug.WriteLine (string.Format ("Missing glyph for '{0}', unicode {1}", c, c_index.ToString ("X4")));
						continue;
					}
					
					total++;
					
					uint num_glyph = maxp_t.NumGlyphs;
					uint num_h_metrics = hhea_t.NumHMetrics;
					
					uint advance = 0;
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
			string font = "Arial Unicode MS";
			byte[] data = Platform.Win32.LoadFontData (font);
			
			TableDirectory td = new TableDirectory (data, 0);
			TableEntry gsub_e = td.FindTable ("GSUB");
			Table_GSUB gsub_t = new Table_GSUB (data, (int) gsub_e.Offset);
			
			ScriptListTable  script_l  = gsub_t.ScriptListTable;
			FeatureListTable feature_l = gsub_t.FeatureListTable;
			
			System.Collections.Hashtable referenced_features = new System.Collections.Hashtable ();
			
			for (uint i = 0; i < script_l.ScriptCount; i++)
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				buffer.Append (script_l.GetScriptTag (i));
				buffer.Append (": ");
				
				ScriptTable script_t = script_l.GetScriptTable (i);
				
				buffer.Append ("default=[");
				
				for (uint k = 0; k < script_t.DefaultLangSysTable.FeatureCount; k++)
				{
					if (k > 0) buffer.Append (";");
					buffer.Append (script_t.DefaultLangSysTable.GetFeatureIndex (k));
					referenced_features[(int) script_t.DefaultLangSysTable.GetFeatureIndex (k)] = true;
				}
				
				buffer.Append ("]");
				
				for (uint j = 0; j < script_t.LangSysCount; j++)
				{
					buffer.Append (", '");
					buffer.Append (script_t.GetLangSysTag (j));
					buffer.Append ("' f=[");
					buffer.Append ((short) script_t.GetLangSysTable (j).RequiredFeatureIndex);
					
					if (script_t.GetLangSysTable (j).RequiredFeatureIndex != 0xffff)
					{
						referenced_features[(int) script_t.GetLangSysTable (j).RequiredFeatureIndex] = true;
					}
					
					for (uint k = 0; k < script_t.GetLangSysTable (j).FeatureCount; k++)
					{
						buffer.Append (";");
						buffer.Append (script_t.GetLangSysTable (j).GetFeatureIndex (k));
						referenced_features[(int) script_t.DefaultLangSysTable.GetFeatureIndex (k)] = true;
					}
					
					buffer.Append ("]");
				}
				
				System.Diagnostics.Debug.WriteLine (buffer.ToString ());
			}
			
			System.Diagnostics.Debug.WriteLine (string.Format ("{0} features, {1} referenced.", feature_l.FeatureCount, referenced_features.Count));
			
			
			string[] feature_names = new string[feature_l.FeatureCount];
			uint[]   feature_index = new uint[feature_l.FeatureCount];
			
			for (uint i = 0; i < feature_l.FeatureCount; i++)
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
			
			uint rq_feature_arab_urdu = gsub_t.GetRequiredFeatureIndex ("arab", "URD ");
			uint[] features_arab_urdu = gsub_t.GetFeatureIndexes ("arab", "URD ");
			
			uint rq_feature_arab_xxxx = gsub_t.GetRequiredFeatureIndex ("arab", "XXXX");
			uint[] features_arab_xxxx = gsub_t.GetFeatureIndexes ("arab", "XXXX");
			
			uint[] fina_features = gsub_t.GetFeatureIndexes ("fina");
			
			Debug.Assert.IsTrue (fina_features.Length == 3);
			
			Debug.Assert.IsTrue (fina_features[0] == 3);
			Debug.Assert.IsTrue (fina_features[1] == 6);
			Debug.Assert.IsTrue (fina_features[2] == 11);
			
			LookupListTable gsub_lookup = gsub_t.LookupListTable;
			uint[] liga_features = gsub_t.GetFeatureIndexes ("liga");
			
			for (int i = 0; i < liga_features.Length; i++)
			{
				LookupTable lookup = gsub_lookup.GetLookupTable (liga_features[i]);
				
				System.Diagnostics.Debug.WriteLine (string.Format ("Lookup: type={0}, flags={1}, sub={2}", lookup.LookupType, lookup.LookupFlags.ToString ("X4"), lookup.SubTableCount));
				
				for (uint s = 0; s < lookup.SubTableCount; s++)
				{
					SubstSubTable subst = lookup.GetSubTable (s);
					
					System.Diagnostics.Debug.WriteLine (string.Format ("Lookup {0}-{1}: format {2}, coverage {3}", i, s, subst.SubstFormat, subst.CoverageOffset));
					
					LigatureSubst liga_subst = new LigatureSubst (subst);
					
					System.Diagnostics.Debug.WriteLine (string.Format ("LigatureSetCount = {0}", liga_subst.LigatureSetCount));
					
					for (uint j = 0; j < liga_subst.LigatureSetCount; j++)
					{
						LigatureSet liga_set = liga_subst.GetLigatureSet (j);
						
						for (uint k = 0; k < liga_set.LigatureCount; k++)
						{
							Ligature ligature = liga_set.GetLigature (k);
							System.Diagnostics.Debug.WriteLine (string.Format ("  {0:00} : {1}, {2} replacements", k, ligature.Glyph, ligature.ComponentCount));
						}
					}
				}
			}
		}
	}
}
