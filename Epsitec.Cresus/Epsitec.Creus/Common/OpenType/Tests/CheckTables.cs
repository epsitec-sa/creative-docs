//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
//				string pFamily = font.GetName (OpenType.NameId.PreferredFamily);
//				string pSubF  = font.GetName (OpenType.NameId.PreferredSubfamily);
//				
//				if ((pFamily != null) ||
//					(pSubF!= null))
//				{
//					System.Diagnostics.Debug.WriteLine ("La fonte " + font.FullName + " a des infos préférées.");
//				}
				
				string fontFace  = font.InvariantFaceName;
				string fontStyle = font.InvariantStyleName;
				
				System.Diagnostics.Debug.WriteLine (string.Format ("{0}/{1} ({2}/{3})\n    weight={5}, italic={6}, kerning={7}\n    {4}", fontFace, fontStyle, font.LocaleFaceName, font.LocaleStyleName, font.FullName, font.FontWeight, font.FontStyle, CheckTables.TestKerningInformation (font)));
			}
			
			Font         arial    = collection.CreateFont ("Arial Unicode MS");
			FontIdentity arialId = arial.FontIdentity;
			
			int[] sizes = new int[] { 8, 9, 10, 11, 12, 24, 120, 2048 };
			
			foreach (int size in sizes)
			{
				ushort[] glyphs = arial.GenerateGlyphs ("Affiche un petit texte pour vérifier le bon fonctionnement du calcul des largeurs.");
				
				arial.SelectFontManager (FontManagerType.System);
				double system = arial.GetTotalWidth (glyphs, size);
				
				arial.SelectFontManager (FontManagerType.OpenType);
				double perfect = arial.GetTotalWidth (glyphs, size);
				
				System.Diagnostics.Debug.WriteLine (string.Format ("{0}, size {4}, text width is {1}, perfect is {2}, delta is {3:0.00}%", arialId.FullName, system, perfect, 100*system/perfect-100, size));
			}
			
			CheckTables.TestFeatureTable ();
			CheckTables.TestArial ();
			CheckTables.TestFont ();
		}
		
		private static bool TestKerningInformation(FontIdentity font)
		{
			string fontFace  = font.InvariantFaceName;
			string fontStyle = font.InvariantStyleName;
			
			FontData fontData = font.FontData;
			
			if (fontData == null)
			{
				return false;
			}
			
			TableEntry entry   = fontData["kern"];
			
			if (entry == null)
			{
				return false;
			}
			
			TableKern kerning = new TableKern (entry);
			
			for (int i = 0; i < kerning.Count; i++)
			{
				KerningTable        kTable   = kerning.GetKerningTable (i);
				KerningTableFormat0 kTable0 = kTable.Format0Subtable;
				
				if (kTable0 == null)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("    {0}: unsupported format {1}.", i, kTable.SubtableFormat));
				}
				else
				{
					int pCount = kTable0.PairCount;
					int kValue;
					
					for (int j = 0; j < pCount; j++)
					{
						int gLeft  = kTable0.GetLeftGlyph (j);
						int gRight = kTable0.GetRightGlyph (j);
						
						Debug.Assert.IsTrue (kTable0.FindKernValue (gLeft, gRight, out kValue));
						Debug.Assert.IsTrue (kTable0.GetKernValue (j) == kValue);
					}
					
					Debug.Assert.IsFalse (kTable0.FindKernValue (0, 0, out kValue));
					Debug.Assert.IsFalse (kTable0.FindKernValue (0xfff, 0xfff, out kValue));
				}
			}
			
			return true;
		}
		
		private static void TestArial()
		{
			string font = "Arial Unicode MS";
			byte[] data = Platform.Neutral.LoadFontData (font, "Normal");
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Loaded font {0}: length {1}", font, data.Length));
			
			TableDirectory td = new TableDirectory (data, 0);
			
			for (int i = 0; i < (int) td.NumTables; i++)
			{
				TableEntry entry = td.GetEntry (i);
				System.Diagnostics.Debug.WriteLine (string.Format ("Table {0}, offset={1}, length={2}.", entry.Tag, entry.Offset, entry.Length));
			}
			
			TableEntry nameE = td.FindTable ("name");
			TableName nameT = new TableName (data, (int) nameE.Offset);
			
			System.Array nameIds = System.Enum.GetValues (typeof (OpenType.NameId));
			System.Array platIds = System.Enum.GetValues (typeof (OpenType.PlatformId));
			
			TableName.NameEncoding[] encodings = nameT.GetAvailableNameEncodings ();
			
			foreach (TableName.NameEncoding encoding in encodings)
			{
				string latinName   = nameT.GetLatinName (encoding.Language, encoding.Name, encoding.Platform);
				string unicodeName = nameT.GetUnicodeName (encoding.Language, encoding.Name, encoding.Platform);
					
				if (latinName != null)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("{0}/{1} : {2}", encoding.Platform, encoding.Name, latinName));
				}
				if (unicodeName != null)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("{0}/{1} : {2} -- {3}", encoding.Platform, encoding.Name, unicodeName, encoding.Language));
				}
				
				if (latinName == unicodeName)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Error: {0}/{1}.", encoding.Platform, encoding.Name));
				}
			}
			
			string sArialFamily = nameT.GetUnicodeName (0x0409, OpenType.NameId.FontFamily, OpenType.PlatformId.Microsoft);
			string sArialSubDe = nameT.GetUnicodeName (0x0407, OpenType.NameId.FontSubfamily, OpenType.PlatformId.Microsoft);
			string sArialSubFr = nameT.GetUnicodeName (0x040c, OpenType.NameId.FontSubfamily, OpenType.PlatformId.Microsoft);
			
			Debug.Assert.IsTrue (sArialFamily == "Arial Unicode MS");
			Debug.Assert.IsTrue (sArialSubDe == "Standard");
			Debug.Assert.IsTrue (sArialSubFr == "Normal");
			
			TableMaxp maxpT = new TableMaxp (data, (int) td.FindTable ("maxp").Offset);
			TableHhea hheaT = new TableHhea (data, (int) td.FindTable ("hhea").Offset);
			Tablehmtx hmtxT = new Tablehmtx (data, (int) td.FindTable ("hmtx").Offset);
			TableCmap cmapT = new TableCmap (data, (int) td.FindTable ("cmap").Offset);
			
			IndexMappingTable cmapSubT = cmapT.FindFormatSubTable ();
			
			int total = 0;
			int width = 0;
			
			System.Diagnostics.Trace.WriteLine ("Measuring text width.");
			
			for (int i = 0; i < 100; i++)
			{
				for (char c = ' '; c < '\u01F6'; c++)
				{
					int cIndex   = (int) c;
					
					if ((cIndex >= 127) && (cIndex < 160))
					{
						continue;
					}
					
					int glyph     = cmapSubT.GetGlyphIndex ((int) c);
					
					if (glyph == 0)
					{
//						System.Diagnostics.Debug.WriteLine (string.Format ("Missing glyph for '{0}', unicode {1}", c, cIndex.ToString ("X4")));
						continue;
					}
					
					total++;
					
					int numGlyph = maxpT.NumGlyphs;
					int numHMetrics = hheaT.NumHMetrics;
					
					int advance = 0;
					int  lsb     = 0;
					
					if (glyph < numGlyph)
					{
						if (glyph < numHMetrics)
						{
							advance = hmtxT.GetAdvanceWidth (glyph);
							lsb     = hmtxT.GetLeftSideBearing (glyph);
						}
						else
						{
							advance = hmtxT.GetAdvanceWidth (numHMetrics-1);
							lsb     = hmtxT.GetExtraLeftSideBearing (numHMetrics, glyph - numHMetrics);
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
			byte[] data = Platform.Neutral.LoadFontData (font, "Normal");
			
			TableDirectory td = new TableDirectory (data, 0);
			TableGSUB gsubT = new TableGSUB (td.FindTable ("GSUB"));
			
			ScriptListTable  scriptL  = gsubT.ScriptListTable;
			FeatureListTable featureL = gsubT.FeatureListTable;
			
			System.Collections.Hashtable referencedFeatures = new System.Collections.Hashtable ();
			
			for (int i = 0; i < scriptL.ScriptCount; i++)
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				buffer.Append (scriptL.GetScriptTag (i));
				buffer.Append (": ");
				
				ScriptTable scriptT = scriptL.GetScriptTable (i);
				
				buffer.Append ("default=[");
				
				for (int k = 0; k < scriptT.DefaultLangSysTable.FeatureCount; k++)
				{
					if (k > 0) buffer.Append (";");
					buffer.Append (scriptT.DefaultLangSysTable.GetFeatureIndex (k));
					referencedFeatures[(int) scriptT.DefaultLangSysTable.GetFeatureIndex (k)] = true;
				}
				
				buffer.Append ("]");
				
				for (int j = 0; j < scriptT.LangSysCount; j++)
				{
					buffer.Append (", '");
					buffer.Append (scriptT.GetLangSysTag (j));
					buffer.Append ("' f=[");
					buffer.Append ((short) scriptT.GetLangSysTable (j).RequiredFeatureIndex);
					
					if (scriptT.GetLangSysTable (j).RequiredFeatureIndex != 0xffff)
					{
						referencedFeatures[(int) scriptT.GetLangSysTable (j).RequiredFeatureIndex] = true;
					}
					
					for (int k = 0; k < scriptT.GetLangSysTable (j).FeatureCount; k++)
					{
						buffer.Append (";");
						buffer.Append (scriptT.GetLangSysTable (j).GetFeatureIndex (k));
						referencedFeatures[(int) scriptT.GetLangSysTable (j).GetFeatureIndex (k)] = true;
					}
					
					buffer.Append ("]");
				}
				
				System.Diagnostics.Debug.WriteLine (buffer.ToString ());
			}
			
			System.Diagnostics.Debug.WriteLine (string.Format ("{0} features, {1} referenced.", featureL.FeatureCount, referencedFeatures.Count));
			
			
			string[] featureNames = new string[featureL.FeatureCount];
			int[]   featureIndex = new int[featureL.FeatureCount];
			
			for (int i = 0; i < featureL.FeatureCount; i++)
			{
				featureNames[i] = featureL.GetFeatureTag (i) + "-" + i.ToString ("X4");
				featureIndex[i] = i;
			}
			
			System.Array.Sort (featureNames, featureIndex);
			
			for (int i = 0; i < featureIndex.Length; i++)
			{
				string       featureN = featureL.GetFeatureTag (featureIndex[i]);
				FeatureTable featureT = featureL.GetFeatureTable (featureIndex[i]);
				
				System.Diagnostics.Debug.WriteLine (string.Format ("{0}-{2:000}: {1} lookups.{3}", featureN, featureT.LookupCount, featureIndex[i], referencedFeatures.Contains ((int)i) ? "" : " Not referenced."));
				
			}
			
			int rqFeatureArabUrdu = gsubT.GetRequiredFeatureIndex ("arab", "URD ");
			int[] featuresArabUrdu = gsubT.GetFeatureIndexes ("arab", "URD ");
			
			int rqFeatureArabXxxx = gsubT.GetRequiredFeatureIndex ("arab", "XXXX");
			int[] featuresArabXxxx = gsubT.GetFeatureIndexes ("arab", "XXXX");
			
			int[] finaFeatures = gsubT.GetFeatureIndexes ("fina");
			
//			Debug.Assert.IsTrue (finaFeatures.Length == 3);
//			Debug.Assert.IsTrue (finaFeatures[0] == 3);
//			Debug.Assert.IsTrue (finaFeatures[1] == 6);
//			Debug.Assert.IsTrue (finaFeatures[2] == 11);
			
			LookupListTable gsubLookup = gsubT.LookupListTable;
			
			int[] ligaFeatures = gsubT.GetFeatureIndexes ("liga");
			int[] rligFeatures = gsubT.GetFeatureIndexes ("rlig");
			int[] dligFeatures = gsubT.GetFeatureIndexes ("dlig");
			
			System.Collections.ArrayList ligatureList = new System.Collections.ArrayList ();
			
			ligatureList.AddRange (ligaFeatures);
			ligatureList.AddRange (rligFeatures);
			ligatureList.AddRange (dligFeatures);
			ligatureList.AddRange (gsubT.GetFeatureIndexes ("alts"));
			ligatureList.AddRange (gsubT.GetFeatureIndexes ("hlig"));
			ligatureList.AddRange (gsubT.GetFeatureIndexes ("hist"));
			ligatureList.AddRange (gsubT.GetFeatureIndexes ("smcp"));
			ligatureList.AddRange (gsubT.GetFeatureIndexes ("frac"));
			
			for (int i = 0; i < ligatureList.Count; i++)
			{
				int          featureI		= (int)ligatureList[i];
				FeatureTable feature		= gsubT.FeatureListTable.GetFeatureTable (featureI);
				int          lookupCount	= feature.LookupCount;
				
				for (int featureLookupI = 0; featureLookupI < lookupCount; featureLookupI++)
				{
					LookupTable lookup = gsubLookup.GetLookupTable (feature.GetLookupIndex (featureLookupI));
					
					System.Diagnostics.Debug.WriteLine (string.Format ("Lookup: type={0}, flags={1}, sub.count={2}", lookup.LookupType, lookup.LookupFlags.ToString ("X4"), lookup.SubTableCount));
					
					if (lookup.LookupType == LookupType.Single)
					{
						for (int s = 0; s < lookup.SubTableCount; s++)
						{
							SubstSubTable baseSubst = lookup.GetSubTable (s);
							SingleSubstitution singleSubst = new SingleSubstitution (baseSubst);
							
							int[] covered = CheckTables.GetCoverageIndexes (singleSubst.Coverage);
							
							System.Diagnostics.Debug.WriteLine (string.Format ("  Subtable: format {0}, coverage format {1}, covered {2} -- Single", singleSubst.SubstFormat, singleSubst.Coverage.CoverageFormat, covered.Length));
							
							for (int c = 0; c < covered.Length; c++)
							{
								System.Diagnostics.Debug.WriteLine (string.Format ("Replace {0:0000} --> {1:0000}", covered[c], singleSubst.FindSubstitution (covered[c])));
							}
						}
					}
					else if (lookup.LookupType == LookupType.Ligature)
					{
						for (int s = 0; s < lookup.SubTableCount; s++)
						{
							SubstSubTable baseSubst = lookup.GetSubTable (s);
							LigatureSubstitution ligaSubst = new LigatureSubstitution (baseSubst);
							
							int[] covered = CheckTables.GetCoverageIndexes (ligaSubst.Coverage);
							
							System.Diagnostics.Debug.WriteLine (string.Format ("  Subtable: format {0}, coverage format {1}, covered {2} -- Ligature", ligaSubst.SubstFormat, ligaSubst.Coverage.CoverageFormat, covered.Length));
							System.Diagnostics.Debug.WriteLine (string.Format ("            # of ligature sets: {0}", ligaSubst.LigatureSetCount));
							
							Debug.Assert.IsTrue (ligaSubst.LigatureSetCount == covered.Length);
							
							for (int j = 0; j < ligaSubst.LigatureSetCount; j++)
							{
								LigatureSet ligaSet = ligaSubst.GetLigatureSet (j);
								
								for (int k = 0; k < ligaSet.LigatureInfoCount; k++)
								{
									System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
									
									LigatureInfo ligature = ligaSet.GetLigatureInfo (k);
									int resultGlyph = ligature.Glyph;
									
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
								int iOffset  = 0;
								int oOffset	 = 0;
								int length   = glyphs.Length;
								
								ligaSubst.ProcessSubstitution (glyphs, ref iOffset, length, result, ref oOffset);
								ligaSubst.ProcessSubstitution (glyphs, ref iOffset, length, result, ref oOffset);
								ligaSubst.ProcessSubstitution (glyphs, ref iOffset, length, result, ref oOffset); iOffset++; oOffset++;
								ligaSubst.ProcessSubstitution (glyphs, ref iOffset, length, result, ref oOffset);
								ligaSubst.ProcessSubstitution (glyphs, ref iOffset, length, result, ref oOffset);
							}
							System.Diagnostics.Trace.WriteLine ("Done.");
						}
					}
					else if (lookup.LookupType == LookupType.ChainingContext)
					{
						for (int s = 0; s < lookup.SubTableCount; s++)
						{
							SubstSubTable baseSubst = lookup.GetSubTable (s);
							ChainingContextSubstitution cctxSubst = new ChainingContextSubstitution (baseSubst);
							
							int[] covered = CheckTables.GetCoverageIndexes (cctxSubst.Coverage);
							
							System.Diagnostics.Debug.WriteLine (string.Format ("  Subtable: format {0}, coverage format {1}, covered {2} -- ChainingContext", cctxSubst.SubstFormat, cctxSubst.Coverage.CoverageFormat, covered.Length));
						}
					}
					else if (lookup.LookupType == LookupType.Alternate)
					{
						for (int s = 0; s < lookup.SubTableCount; s++)
						{
							SubstSubTable baseSubst = lookup.GetSubTable (s);
							AlternateSubstitution altSubst = new AlternateSubstitution (baseSubst);
							
							int[] covered = CheckTables.GetCoverageIndexes (altSubst.Coverage);
							
							System.Diagnostics.Debug.WriteLine (string.Format ("  Subtable: format {0}, coverage format {1}, covered {2} -- Alternate", altSubst.SubstFormat, altSubst.Coverage.CoverageFormat, covered.Length));
							
							for (int c = 0; c < covered.Length; c++)
							{
								System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
								buffer.AppendFormat ("    Alternates for {0:0000} :", covered[c]);
								
								ushort[] alts = altSubst.GetAlternates ((ushort) covered[c]);
								
								foreach (ushort alt in alts)
								{
									buffer.AppendFormat (" {0:0000}", alt);
								}
								
								System.Diagnostics.Debug.WriteLine (buffer.ToString ());
							}
						}
					}
				}
			}
		}
		
		private static void TestFont()
		{
			string fontName1 = "Palatino Linotype";
			string fontName2 = "Times New Roman";
			
			Font font1 = new Font ();
			Font font2 = new Font ();
			
			font1.Initialize (new FontData (Platform.Neutral.LoadFontData (fontName1, "Normal"), -1));
			font2.Initialize (new FontData (Platform.Neutral.LoadFontData (fontName2, "Normal"), -1));
			
			System.Diagnostics.Debug.WriteLine ("Palatino Linotype scripts:  " + string.Join ("; ", font1.GetSupportedScripts ()));
			System.Diagnostics.Debug.WriteLine ("Times New Roman scripts:    " + string.Join ("; ", font2.GetSupportedScripts ()));
			System.Diagnostics.Debug.WriteLine ("Palatino Linotype features: " + string.Join ("; ", font1.GetSupportedFeatures ()));
			System.Diagnostics.Debug.WriteLine ("Times New Roman features:   " + string.Join ("; ", font2.GetSupportedFeatures ()));
			
			font1.SelectFeatures ("dlig", "liga");
			
			font1.SelectScript ("latn", "ROM ");
			font2.SelectScript ("arab");
			
			font1.SelectFeatures ("dlig", "liga");
			
			System.Diagnostics.Debug.WriteLine ("Palatino Linotype / 'latn'+'ROM ' features: " + string.Join ("; ", font1.GetSupportedFeatures ()));
			System.Diagnostics.Debug.WriteLine ("Times New Roman / 'arab' features:          " + string.Join ("; ", font2.GetSupportedFeatures ()));
			
			font2.SelectScript ("latn");
			font2.SelectFeatures ("liga");
			
			string text = "Quelle fin affreuse, affligeant !";
			
			ushort[] glyphs1a = font1.GenerateGlyphs (text);
			ushort[] glyphs2a = font2.GenerateGlyphs (text);
			
			ushort[] glyphs1aa;
			ushort[] glyphs2aa;
			byte[] attrib1aa = new byte[text.Length];
			byte[] attrib2aa = new byte[text.Length];
			
			for (int i = 0; i < text.Length; i++)
			{
				attrib1aa[i] = (byte) (i+1);
				attrib2aa[i] = (byte) (i+1);
			}
			
			font1.GenerateGlyphs(text, out glyphs1aa, ref attrib1aa);
			font2.GenerateGlyphs(text, out glyphs2aa, ref attrib2aa);
			
			Debug.Assert.IsTrue (attrib1aa[0] == 1);
			Debug.Assert.IsTrue (attrib1aa[1] == 3);
			Debug.Assert.IsTrue (attrib1aa[18] == 22);
			Debug.Assert.IsTrue (attrib1aa[19] == 23);
			Debug.Assert.IsTrue (attrib1aa[20] == 26);
			
			Debug.Assert.IsTrue (attrib2aa[0] == 1);
			Debug.Assert.IsTrue (attrib2aa[1] == 2);
			Debug.Assert.IsTrue (attrib2aa[22] == 24);
			Debug.Assert.IsTrue (attrib2aa[23] == 26);
			
			double w1a = font1.GetTotalWidth (glyphs1a, 10.0);
			double w2a = font2.GetTotalWidth (glyphs2a, 10.0);
			
			double[] xPos1c = new double[glyphs1a.Length];
			double[] xPos2c = new double[glyphs2a.Length];
			
			double w1c = font1.GetPositions (glyphs1a, 10.0, 0.0, xPos1c);
			double w2c = font1.GetPositions (glyphs1a, 10.0, 0.0, xPos2c);
			
			double x, y;
			
			font1.HitTest (text, 10.0, 24, out x, out y);
			font2.HitTest (text, 10.0, 24, out x, out y);
			
			font1.HitTest (text, 10.0, 0, out x, out y);
			font1.HitTest (text, 10.0, 1, out x, out y);
			font1.HitTest (text, 10.0, 2, out x, out y);
			font1.HitTest (text, 10.0, text.Length, out x, out y);
			
			int    pos;
			double subpos;
			
			font1.HitTest (text, 10.0,  0.0, 0.0, out pos, out subpos);
			font1.HitTest (text, 10.0,  2.0, 0.0, out pos, out subpos);
			font1.HitTest (text, 10.0,  4.0, 0.0, out pos, out subpos);
			font1.HitTest (text, 10.0,  6.0, 0.0, out pos, out subpos);
			font1.HitTest (text, 10.0,  8.0, 0.0, out pos, out subpos);
			font1.HitTest (text, 10.0, 10.0, 0.0, out pos, out subpos);
			font1.HitTest (text, 10.0, 12.0, 0.0, out pos, out subpos);
			font1.HitTest (text, 10.0, 14.0, 0.0, out pos, out subpos);
			
			font1.HitTest (text, 10.0, x, y, out pos, out subpos);
			
			Debug.Assert.IsTrue (pos == text.Length);
			
			font1.SelectFeatures ();
			font2.SelectFeatures ();
			
			ushort[] glyphs1b = font1.GenerateGlyphs (text);
			ushort[] glyphs2b = font2.GenerateGlyphs (text);
			
			double w1b = font1.GetTotalWidth (glyphs1b, 10.0);
			double w2b = font2.GetTotalWidth (glyphs2b, 10.0);
			
			double asc1 = font1.GetAscender (10.0);
			double dsc1 = font1.GetDescender (10.0);
			
			Debug.Assert.IsTrue (asc1 > 0);
			Debug.Assert.IsTrue (dsc1 < 0);
			
			double asc2 = font2.GetAscender (10.0);
			double dsc2 = font2.GetDescender (10.0);
			
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
