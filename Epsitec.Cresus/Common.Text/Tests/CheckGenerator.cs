//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Tests
{
	/// <summary>
	/// La batterie de tests CheckGenerator vérifie le bon fonctionnement des
	/// générateurs.
	/// </summary>
	public sealed class CheckGenerator
	{
		public static void RunTests()
		{
			CheckGenerator.TestGenerator ();
			CheckGenerator.TestTextStory ();
		}
		
		private static void TestGenerator()
		{
			Generator generator = new Generator ("Test");
			
			Generator.Sequence s_num = Generator.CreateSequence (Generator.SequenceType.Numeric);
			Generator.Sequence s_alf = Generator.CreateSequence (Generator.SequenceType.Alphabetic);
			Generator.Sequence s_ALF = Generator.CreateSequence (Generator.SequenceType.Alphabetic);
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			s_num.Suffix = ".";
			s_alf.Suffix = ")";
			s_ALF.Prefix = "<";
			s_ALF.Casing = Generator.Casing.Upper;
			s_ALF.Suffix = ">";
			
			buffer.Append (s_num.Prefix == null ? "" : s_num.Prefix);
			buffer.Append (s_num.GenerateText (1, System.Globalization.CultureInfo.CurrentCulture));
			buffer.Append (s_num.Suffix == null ? "" : s_num.Suffix);
			buffer.Append (s_num.Prefix == null ? "" : s_num.Prefix);
			buffer.Append (s_num.GenerateText (10, System.Globalization.CultureInfo.CurrentCulture));
			buffer.Append (s_num.Suffix == null ? "" : s_num.Suffix);
			buffer.Append (s_alf.Prefix == null ? "" : s_alf.Prefix);
			buffer.Append (s_alf.GenerateText (3, System.Globalization.CultureInfo.CurrentCulture));
			buffer.Append (s_alf.Suffix == null ? "" : s_alf.Suffix);
			buffer.Append (s_ALF.Prefix == null ? "" : s_ALF.Prefix);
			buffer.Append (s_ALF.GenerateText (1, System.Globalization.CultureInfo.CurrentCulture));
			buffer.Append (s_ALF.Suffix == null ? "" : s_ALF.Suffix);
			
			Debug.Assert.IsTrue (buffer.ToString () == "1.10.c)<A>");
			
			generator.Add (s_num);
			generator.Add (s_num);
			generator.Add (s_alf);
			generator.Add (s_num);
			
			int[] ranks = new int[] { 1, 10, 3, 2, 3, 4 };
			
			Debug.Assert.IsTrue ("1.10.c)2.3.4." == generator.GenerateTextString (ranks, System.Globalization.CultureInfo.CurrentCulture));
			
			Generator.Series series;
			generator.StartVector = null;
			
			series = generator.NewSeries (System.Globalization.CultureInfo.CurrentCulture);
			
			Debug.Assert.IsTrue ("1." == series.GetNextTextString (0));
			Debug.Assert.IsTrue ("2." == series.GetNextTextString (0));
			Debug.Assert.IsTrue ("2.1." == series.GetNextTextString (1));
			Debug.Assert.IsTrue ("2.1.a)1." == series.GetNextTextString (3));
			Debug.Assert.IsTrue ("2.1.b)" == series.GetNextTextString (2));
			Debug.Assert.IsTrue ("2.1.c)" == series.GetNextTextString (2));
			Debug.Assert.IsTrue ("3." == series.GetNextTextString (0));
			Debug.Assert.IsTrue ("3.1.a)1.1.1." == series.GetNextTextString (5));
		}
		
		private static void TestTextStory()
		{
			TextStory story  = new TextStory ();
			ICursor   cursor = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] text;
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Verdana", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			
			TextStyle style = story.StyleList.NewTextStyle (null, "Normal", TextStyleClass.Paragraph, properties);
			
			Properties.GeneratorProperty g1_a = new Properties.GeneratorProperty ("G1", 0, 101);
			Properties.GeneratorProperty g1_b = new Properties.GeneratorProperty ("G1", 0, 102);
			Properties.GeneratorProperty g1_c = new Properties.GeneratorProperty ("G1", 1, 103);
			Properties.GeneratorProperty g1_d = new Properties.GeneratorProperty ("G1", 2, 104);
			Properties.GeneratorProperty g1_e = new Properties.GeneratorProperty ("G1", 1, 105);

			properties.Clear ();
			properties.Add (g1_a);
			properties.Add (new Properties.AutoTextProperty ("g1a"));
			
			story.ConvertToStyledText ("X", style, properties, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("Chapitre premier\n", style, null, out text);
			story.InsertText (cursor, text);

			properties.Clear ();
			properties.Add (g1_b);
			properties.Add (new Properties.AutoTextProperty ("g1b"));
			
			story.ConvertToStyledText ("X", style, properties, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("Chapitre second\n", style, null, out text);
			story.InsertText (cursor, text);

			properties.Clear ();
			properties.Add (g1_c);
			properties.Add (new Properties.AutoTextProperty ("g1c"));
			
			story.ConvertToStyledText ("X", style, properties, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("Introduction\n", style, null, out text);
			story.InsertText (cursor, text);

			properties.Clear ();
			properties.Add (g1_d);
			properties.Add (new Properties.AutoTextProperty ("g1d"));
			
			story.ConvertToStyledText ("X", style, properties, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("Plan\n", style, null, out text);
			story.InsertText (cursor, text);

			properties.Clear ();
			properties.Add (g1_e);
			properties.Add (new Properties.AutoTextProperty ("g1e"));
			
			story.ConvertToStyledText ("X", style, properties, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("Blabla...\n", style, null, out text);
			story.InsertText (cursor, text);
			
			text = new ulong[story.TextLength];
			story.SetCursorPosition (cursor, 0);
			story.ReadText (cursor, story.TextLength, text);
			
			System.Diagnostics.Debug.WriteLine (story.GetDebugStyledText (text));
			
			Generator generator = story.TextContext.GeneratorList.NewGenerator ("G1");
			
			generator.Add (Generator.CreateSequence (Generator.SequenceType.Numeric, "", "."));
			generator.Add (Generator.CreateSequence (Generator.SequenceType.Numeric, "", "."));
			generator.Add (Generator.CreateSequence (Generator.SequenceType.Alphabetic));
			
			int count1 = generator.UpdateAllFields (story, null, System.Globalization.CultureInfo.CurrentCulture);
			int count2 = generator.UpdateAllFields (story, null, System.Globalization.CultureInfo.CurrentCulture);
			
			generator.StartVector = new int[] { 8, 12 };
			
			int count3 = generator.UpdateAllFields (story, null, System.Globalization.CultureInfo.CurrentCulture);
			
			Debug.Assert.IsTrue (count1 == 5);
			Debug.Assert.IsTrue (count2 == 0);
			Debug.Assert.IsTrue (count3 == 5);
			
			text = new ulong[story.TextLength];
			story.SetCursorPosition (cursor, 0);
			story.ReadText (cursor, story.TextLength, text);
			
			System.Diagnostics.Debug.WriteLine (story.GetDebugStyledText (text));
		}
	}
}
