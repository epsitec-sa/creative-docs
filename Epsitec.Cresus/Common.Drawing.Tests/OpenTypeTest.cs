using NUnit.Framework;

namespace Epsitec.Common.OpenType
{
	[TestFixture]
	public class OpenTypeTest
	{
		[Test] public void CheckFeatures()
		{
			FontCollection collection = new FontCollection ();
			
			collection.Initialize ();
			
			Font         zapf    = collection.CreateFont ("Zapf Dingbats BT");
			FontIdentity zapf_id = zapf.FontIdentity;
			
			Table_cmap zapf_cmap = zapf_id.GetTable_cmap ();
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
			
			Table_cmap wing1_cmap = wing1_id.GetTable_cmap ();
			IndexMappingTable wing1_imp = wing1_cmap.FindFormatSubTable (3, 0, 4);
			
			Assert.AreEqual (3, wing1_imp.GetGlyphIndex (0xF020));
			
			Assert.AreEqual (".notdef", wing1_id.GetGlyphName (0));
			Assert.AreEqual ("space", wing1_id.GetGlyphName (3));
			Assert.AreEqual ("pencil", wing1_id.GetGlyphName (4));
			Assert.AreEqual ("scissors", wing1_id.GetGlyphName (5));
			
//			Common.OpenType.Tests.CheckTables.RunTests ();
		}
	}
}

