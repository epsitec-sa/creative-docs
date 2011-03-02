using NUnit.Framework;
using Epsitec.Common.OpenType;

namespace Epsitec.Common.Tests.Drawing
{
	[TestFixture]
	public class OpenTypeTest
	{
		[Test] public void CheckFeatures()
		{
			//	Ce test requiert les fontes suivantes sur le système:
			//	- Zapf Dingbats BT
			//	- Arial Unicode MS
			
			FontCollection collection = new FontCollection ();
			
			collection.Initialize ();
			
			Font         zapf    = collection.CreateFont ("Zapf Dingbats BT");
			FontIdentity zapf_id = zapf.FontIdentity;
			
			TableCmap zapf_cmap = zapf_id.InternalGetTableCmap ();
			IndexMappingTable zapf_imp = zapf_cmap.FindFormatSubTable (3, 0, 4);
			
			Assert.AreEqual (3, zapf_imp.GetGlyphIndex (0xF020));
			
			Assert.AreEqual (".notdef", zapf_id.GetGlyphName (0));
			Assert.AreEqual (".null", zapf_id.GetGlyphName (1));
			Assert.AreEqual ("nonmarkingreturn", zapf_id.GetGlyphName (2));
			Assert.AreEqual ("space", zapf_id.GetGlyphName (3));
			Assert.AreEqual ("a1", zapf_id.GetGlyphName (4));
			Assert.AreEqual ("a2", zapf_id.GetGlyphName (5));
			
			Font         wing1    = collection.CreateFont ("Wingdings");
			FontIdentity wing1_id = wing1.FontIdentity;
			
			TableCmap wing1_cmap = wing1_id.InternalGetTableCmap ();
			IndexMappingTable wing1_imp = wing1_cmap.FindFormatSubTable (3, 0, 4);
			
			Assert.AreEqual (3, wing1_imp.GetGlyphIndex (0xF020));
			
			Assert.AreEqual (".notdef", wing1_id.GetGlyphName (0));
			Assert.AreEqual ("space", wing1_id.GetGlyphName (3));
			Assert.AreEqual ("pencil", wing1_id.GetGlyphName (4));
			Assert.AreEqual ("scissors", wing1_id.GetGlyphName (5));
			Assert.AreEqual ("windowslogo", wing1_id.GetGlyphName (225));
			
			Common.OpenType.Tests.CheckTables.RunTests ();
		}

		[Test]
		public void ListAllFontNames()
		{
			FontCollection collection = new FontCollection ();

			collection.Initialize ();

			System.Console.WriteLine ("Available fonts & names");
			System.Console.WriteLine ("-----------------------");

			foreach (FontIdentity id in collection)
			{
				OpenType.TableName name = id.OpenTypeTableName;
				OpenType.TableName.NameEncoding[] encodings = name.GetAvailableNameEncodings ();
				
				System.Console.WriteLine (
					"{0,-32}Style={1} Weight={2} Count={3}\n" +
					"                                Invariant={4} / {5}\n" +
					"                                Invariant Preferred={6} / {7}\n" +
					"                                Invariant Hash={8}\n" +
					"                                Locale={9} / {10}\n" +
					"                                Locale Preferred={11} / {12}\n" +
					"                                UniqueFontId={13}, entries={14}",
					id.FullName, id.FontStyle, id.FontWeight, id.FontStyleCount,
					id.InvariantSimpleFaceName, id.InvariantSimpleStyleName,
					id.InvariantPreferredFaceName, id.InvariantPreferredStyleName,
					id.InvariantStyleHash,
					id.LocaleSimpleFaceName, id.LocaleSimpleStyleName,
					id.LocalePreferredFaceName, id.LocalePreferredStyleName,
					id.UniqueFontId, encodings.Length);

				string[] lines = new string[encodings.Length];

				for (int i = 0; i < encodings.Length; i++)
				{
					string unicode = encodings[i].Platform == PlatformId.Microsoft ? name.GetUnicodeName (encodings[i].Language, encodings[i].Name, encodings[i].Platform) : null;
					string latin   = encodings[i].Platform == PlatformId.Macintosh ? name.GetLatinName (encodings[i].Language, encodings[i].Name, encodings[i].Platform) : null;

					lines[i] = string.Format ("{0,-4} {1,-24} {2,-10} : {3}", encodings[i].Language, encodings[i].Name, encodings[i].Platform, (unicode ?? latin ?? "").Split ('\n')[0]);
				}
				
				System.IO.File.WriteAllLines (id.FullName + ".txt", lines);
			}
			
			System.Console.Out.Flush ();
		}

		[Test]
		public void ListAllChameleonFontNames()
		{
			FontCollection collection = new FontCollection ();

			collection.Initialize ();

			System.Console.WriteLine ("Chameleon fonts (different names for Microsoft or Macintosh)");
			System.Console.WriteLine ("------------------------------------------------------------");

			foreach (FontIdentity id in collection)
			{
				if (string.IsNullOrEmpty (id.MacintoshFaceName))
				{
					continue;
				}

				if ((id.InvariantSimpleFaceName != id.MacintoshFaceName) ||
					(id.InvariantSimpleStyleName != id.MacintoshStyleName))
				{
					System.Console.WriteLine ("{0} + {1} <--> {2} + {3}", id.InvariantSimpleFaceName, id.InvariantSimpleStyleName, id.MacintoshFaceName, id.MacintoshStyleName);
				}
			}

			System.Console.Out.Flush ();
		}

		[Test]
		public void ListAllFullFontNames()
		{
			FontCollection collection = new FontCollection ();

			collection.Initialize ();

			System.Console.WriteLine ("FullFontName compared with Mac FontFamily+FontSubfamily");
			System.Console.WriteLine ("-------------------------------------------------------");

			foreach (FontIdentity id in collection)
			{
				if (string.IsNullOrEmpty (id.MacintoshFaceName))
				{
					continue;
				}

				System.Console.WriteLine ("{0} --> {1} + {2} ==> {3} + {4}", id.FullName, id.MacintoshFaceName, id.MacintoshStyleName, id.InvariantFaceName, id.InvariantStyleName);
			}

			System.Console.Out.Flush ();
		}
		
		
		
		[Test] public void ListAllFeatures()
		{
			FontCollection collection = new FontCollection ();
			
			collection.Initialize ();
			
			System.Console.WriteLine ("Available fonts & features");
			System.Console.WriteLine ("--------------------------");
			
			System.Collections.Hashtable hash = new System.Collections.Hashtable ();
			
			foreach (FontIdentity id in collection)
			{
				Font font = collection.CreateFont (id);
				string[] features = font.GetSupportedFeatures ();
				
				System.Array.Sort (features);
				
				foreach (string feature in features)
				{
					hash[feature] = null;
					LookupTable[] tables = font.GetLookupTables (feature);
					
					System.Console.Write ("{0} [{1}] =>", font.FontIdentity.FullName, feature);
					
					foreach (LookupTable table in tables)
					{
						System.Console.Write (" {0}", table.LookupType);
					}
					
					System.Console.WriteLine ();
				}
			}
			
			System.Console.WriteLine ();
			System.Console.WriteLine ("List of all features, sorted");
			System.Console.WriteLine ("----------------------------");
			
			string[] full = new string[hash.Count];
			hash.Keys.CopyTo (full, 0);
			System.Array.Sort (full);
			
			System.Console.WriteLine (string.Join ("\n", full));

			System.Console.WriteLine ();
			System.Console.Out.Flush ();
		}
		
		[Test] public void ListAlternateFeatures()
		{
			FontCollection collection = new FontCollection ();
			
			collection.Initialize ();
			
			System.Console.WriteLine ("Font with alternate features");
			System.Console.WriteLine ("----------------------------");
			
			foreach (FontIdentity id in collection)
			{
				Font font = collection.CreateFont (id);
				string[] features = font.GetSupportedFeatures ();
				
				System.Array.Sort (features);
				
				foreach (string feature in features)
				{
					LookupTable[] tables = font.GetLookupTables (feature);
					
					foreach (LookupTable table in tables)
					{
						if (table.LookupType == LookupType.Alternate)
						{
							System.Console.WriteLine ("{0} [{1}] has {2} alternate(s)", font.FontIdentity.FullName, feature, tables.Length);
							break;
						}
					}
				}
			}
			
			System.Console.WriteLine ();
			System.Console.Out.Flush ();
		}
		
		
		[Test] public void RefreshCache()
		{
			FontCollection collection = new FontCollection ();
			
			System.Diagnostics.Debug.WriteLine ("OpenType: RefreshCache");
			bool changed = collection.RefreshCache (new FontIdentityCallback (this.FontCallback));
			System.Diagnostics.Debug.WriteLine ("done, changed=" + changed);
		}
		
		private void FontCallback(FontIdentity fid)
		{
			System.Console.WriteLine (fid.FullName);
		}
		
		[Test] public void SaveToCache()
		{
			FontCollection collection = new FontCollection ();
			
			System.Diagnostics.Debug.WriteLine ("OpenType: Initialize");
			collection.Initialize ();
			System.Diagnostics.Debug.WriteLine ("done");
			
			System.Diagnostics.Debug.WriteLine ("OpenType: SaveToCache");
			collection.SaveToCache ();
			System.Diagnostics.Debug.WriteLine ("done");
		}
		
		[Test] public void LoadFromCache()
		{
			FontCollection collection = new FontCollection ();
			
			System.Diagnostics.Debug.WriteLine ("OpenType: LoadFromCache");
			collection.LoadFromCache ();
			System.Diagnostics.Debug.WriteLine ("done");
			
			int count_1 = 0;
			int count_2 = 0;
			
			foreach (FontIdentity fid in collection)
			{
				if (fid != null)
				{
					count_1++;
				}
			}
			
			System.Diagnostics.Debug.WriteLine ("OpenType: Initialize");
			collection.Initialize ();
			System.Diagnostics.Debug.WriteLine ("done");
			
			foreach (FontIdentity fid in collection)
			{
				if (fid != null)
				{
					count_2++;
				}
			}
			
			System.Console.WriteLine ("In cache: {0}, total: {1}", count_1, count_2);
		}
	}
}

