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
			//			Platform.Win32.LoadFontDataDrawing ();
			
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
			
			IndexMappingTable cmap_sub_t = cmap_t.FindFormatSubtable ();
			
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
	}
}
