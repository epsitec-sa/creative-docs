//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Tests
{
	/// <summary>
	/// Vérifie le bon fonctionnement des propriétés.
	/// </summary>
	public sealed class CheckProperties
	{
		public static void RunTests()
		{
			CheckProperties.TestFont ();
			CheckProperties.TestFontSize ();
			CheckProperties.TestMargins ();
			CheckProperties.TestXlines ();
			CheckProperties.TestUserTags ();
			CheckProperties.TestSerialization ();
			CheckProperties.TestGeneratorProperties ();
			CheckProperties.TestTraverseText ();
			CheckProperties.TestGetTextDistance ();
		}

		
		private static void TestFont()
		{
			Properties.FontProperty fontA = new Properties.FontProperty ("Futura", "Roman");
			Properties.FontProperty fontB = new Properties.FontProperty (null, "Heavy");
			Properties.FontProperty fontC = new Properties.FontProperty ("Arial", null);
			
			Properties.FontProperty fontAb = fontA.GetCombination (fontB) as Properties.FontProperty;
			Properties.FontProperty fontAc = fontA.GetCombination (fontC) as Properties.FontProperty;
			
			Debug.Assert.IsTrue (fontAb.FaceName == "Futura");
			Debug.Assert.IsTrue (fontAb.StyleName == "Heavy");
			Debug.Assert.IsTrue (fontAc.FaceName == "Arial");
			Debug.Assert.IsTrue (fontAc.StyleName == "Roman");
			
			Debug.Assert.IsTrue (fontA.ToString () == "Futura/Roman/[null]");
			Debug.Assert.IsTrue (fontB.ToString () == "[null]/Heavy/[null]");
			Debug.Assert.IsTrue (fontC.ToString () == "Arial/[null]/[null]");
		}
		
		private static void TestFontSize()
		{
			Properties.FontSizeProperty fontSizeA = new Properties.FontSizeProperty ( 12.0, Properties.SizeUnits.Points);
			Properties.FontSizeProperty fontSizeB = new Properties.FontSizeProperty ( 50.0/100, Properties.SizeUnits.Percent);
			Properties.FontSizeProperty fontSizeC = new Properties.FontSizeProperty ( -2.0, Properties.SizeUnits.DeltaPoints);
			Properties.FontSizeProperty fontSizeD = new Properties.FontSizeProperty (200.0/100, Properties.SizeUnits.Percent);
			
			Properties.FontSizeProperty fontSizeAb = fontSizeA.GetCombination (fontSizeB) as Properties.FontSizeProperty;
			Properties.FontSizeProperty fontSizeBa = fontSizeB.GetCombination (fontSizeA) as Properties.FontSizeProperty;
			Properties.FontSizeProperty fontSizeAc = fontSizeA.GetCombination (fontSizeC) as Properties.FontSizeProperty;
			Properties.FontSizeProperty fontSizeCb = fontSizeC.GetCombination (fontSizeB) as Properties.FontSizeProperty;
			Properties.FontSizeProperty fontSizeBd = fontSizeB.GetCombination (fontSizeD) as Properties.FontSizeProperty;
			
			Debug.Assert.IsTrue (fontSizeAb.ToString () == "6/pt/[NaN]");
			Debug.Assert.IsTrue (fontSizeBa.ToString () == "12/pt/[NaN]");
			Debug.Assert.IsTrue (fontSizeAc.ToString () == "10/pt/[NaN]");
			Debug.Assert.IsTrue (fontSizeCb.ToString () == "-1/+pt/[NaN]");
			Debug.Assert.IsTrue (fontSizeBd.ToString () == "1/%/[NaN]");
			
			Debug.Expect.Exception (new Debug.Method (Ex1), typeof (System.InvalidOperationException));
		}
		
		private static void TestMargins()
		{
			Properties.MarginsProperty marginsA = new Properties.MarginsProperty ();
			Properties.MarginsProperty marginsB = new Properties.MarginsProperty (15.0, 20.0, 0.0, 0.0, Properties.SizeUnits.Points, 0, 0, 0, 0, 0, Properties.ThreeState.False);
			
			Properties.MarginsProperty marginsC = marginsA.GetCombination (marginsB) as Properties.MarginsProperty;
			Properties.MarginsProperty marginsD = marginsB.GetCombination (marginsA) as Properties.MarginsProperty;
			
			marginsA = new Properties.MarginsProperty (double.NaN, 10, double.NaN, 10, Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 0.0, 0.0, Properties.ThreeState.Undefined);
			
			Properties.MarginsProperty marginsE = marginsA.GetCombination (marginsB) as Properties.MarginsProperty;
			Properties.MarginsProperty marginsF = marginsB.GetCombination (marginsA) as Properties.MarginsProperty;
			
			Debug.Assert.IsTrue (marginsC.LeftMarginFirstLine == 15.0);
			Debug.Assert.IsTrue (marginsC.LeftMarginBody      == 20.0);
			Debug.Assert.IsTrue (marginsC.RightMarginBody     ==  0.0);
			
			Debug.Assert.IsTrue (marginsD.LeftMarginFirstLine == 15.0);
			Debug.Assert.IsTrue (marginsD.LeftMarginBody      == 20.0);
			Debug.Assert.IsTrue (marginsD.RightMarginBody     ==  0.0);
			
			Debug.Assert.IsTrue (marginsE.LeftMarginFirstLine == 15.0);
			Debug.Assert.IsTrue (marginsE.LeftMarginBody      == 20.0);
			Debug.Assert.IsTrue (marginsE.RightMarginBody     ==  0.0);
			
			Debug.Assert.IsTrue (marginsF.LeftMarginFirstLine == 15.0);
			Debug.Assert.IsTrue (marginsF.LeftMarginBody      == 10.0);
			Debug.Assert.IsTrue (marginsF.RightMarginBody     == 10.0);
			
			Debug.Assert.IsTrue (marginsA.ToString () == "[NaN]/10/[NaN]/10/pt/0/0/0/0/0/[?]/-1/[null]");
			Debug.Assert.IsTrue (marginsB.ToString () == "15/20/0/0/pt/0/0/0/0/0/[false]/-1/[null]");
		}
		
		private static void TestXlines()
		{
			TextStory story = new TextStory ();
			
			ulong[] text;
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties.Add (new Properties.UnderlineProperty (double.NaN, Properties.SizeUnits.None, double.NaN, Properties.SizeUnits.None, "underline", "-"));
			properties.Add (new Properties.StrikeoutProperty (double.NaN, Properties.SizeUnits.None, double.NaN, Properties.SizeUnits.None, "underline", "color=red"));
			properties.Add (new Properties.TextBoxProperty (double.NaN, Properties.SizeUnits.None, double.NaN, Properties.SizeUnits.None, "frame", "backcolor=yellow;color=black"));
			properties.Add (new Properties.OverlineProperty (double.NaN, Properties.SizeUnits.None, 2.0, Properties.SizeUnits.Points, "underline", "color=red"));
			properties.Add (new Properties.LinkProperty ("http://www.epsitec.ch"));
			properties.Add (new Properties.LinkProperty ("mailto:epsitec@epsitec.ch"));
			
			story.ConvertToStyledText ("Abc", properties, out text);
			
			Properties.AbstractXlineProperty[] underlines;
			Properties.LinkProperty[]          links;
			
			story.TextContext.GetXlines (text[0], out underlines);
			story.TextContext.GetLinks (text[0], out links);
			
			Debug.Assert.IsTrue (underlines.Length == 4);
			Debug.Assert.IsTrue (links.Length == 2);
			
//			System.Array.Sort (underlines, Properties.AbstractXlineProperty.Comparer);
//			System.Array.Sort (links, Properties.LinkProperty.Comparer);
			
			Debug.Assert.IsTrue (underlines[0].WellKnownType == Properties.WellKnownType.Underline);
			Debug.Assert.IsTrue (underlines[1].WellKnownType == Properties.WellKnownType.Strikeout);
			Debug.Assert.IsTrue (underlines[2].WellKnownType == Properties.WellKnownType.Overline);
			Debug.Assert.IsTrue (underlines[3].WellKnownType == Properties.WellKnownType.TextBox);
			
			Debug.Assert.IsTrue (underlines[0].DrawStyle == "-");
			Debug.Assert.IsTrue (underlines[1].DrawStyle == "color=red");
			Debug.Assert.IsTrue (underlines[2].DrawStyle == "color=red");
			Debug.Assert.IsTrue (underlines[3].DrawStyle == "backcolor=yellow;color=black");
			
			Debug.Assert.IsTrue (underlines[1].ThicknessUnits == Properties.SizeUnits.None);
			Debug.Assert.IsTrue (underlines[2].ThicknessUnits == Properties.SizeUnits.Points);
			
			Debug.Assert.IsTrue (links[0].Link == "http://www.epsitec.ch");
			Debug.Assert.IsTrue (links[1].Link == "mailto:epsitec@epsitec.ch");
		}
		
		private static void TestUserTags()
		{
			TextStory story = new TextStory ();
			
			ulong[] text;
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties.Add (new Properties.UserTagProperty ("x", "foo", 0));
			properties.Add (new Properties.UserTagProperty ("x", "bar", 1));
			properties.Add (new Properties.UserTagProperty ("Comment", "Hello :-)", 2));
			
			story.ConvertToStyledText ("Abc", properties, out text);
			
			Properties.UserTagProperty[] usertags;
			
			story.TextContext.GetUserTags (text[0], out usertags);
			
			Debug.Assert.IsTrue (usertags.Length == 3);
			
			Debug.Assert.IsTrue (usertags[0].TagType == "Comment");
			Debug.Assert.IsTrue (usertags[1].TagType == "x");
			Debug.Assert.IsTrue (usertags[2].TagType == "x");
			
			Debug.Assert.IsTrue (usertags[0].TagData == "Hello :-)");
			Debug.Assert.IsTrue (usertags[1].TagData == "bar");
			Debug.Assert.IsTrue (usertags[2].TagData == "foo");
			
			Debug.Assert.IsTrue (usertags[0].Id == 2);
			Debug.Assert.IsTrue (usertags[1].Id == 1);
			Debug.Assert.IsTrue (usertags[2].Id == 0);
		}
		
		private static void TestSerialization()
		{
			Properties.FontProperty     p1 = new Properties.FontProperty ("Futura", "Roman");
			Properties.FontSizeProperty p2 = new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points);
			Properties.MarginsProperty  p3 = new Properties.MarginsProperty (15.0, 20.0, 0.0, 0.0, Properties.SizeUnits.Points, 0, 0, 0, 0, 0, Properties.ThreeState.False);
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			int end1, end2, end3;
			
			Property.SerializeToText (buffer, p1);	end1 = buffer.Length;
			Property.SerializeToText (buffer, p2);	end2 = buffer.Length;
			Property.SerializeToText (buffer, p3);	end3 = buffer.Length;
			
			Property p;
			
			Properties.FontProperty     p1x;
			Properties.FontSizeProperty p2x;
			Properties.MarginsProperty  p3x;
			
			Property.DeserializeFromText (null, buffer.ToString (),    0, end1,        out p); p1x = p as Properties.FontProperty;
			Property.DeserializeFromText (null, buffer.ToString (), end1, end2 - end1, out p); p2x = p as Properties.FontSizeProperty;
			Property.DeserializeFromText (null, buffer.ToString (), end2, end3 - end2, out p); p3x = p as Properties.MarginsProperty;
			
			Debug.Assert.IsNotNull (p1x);
			Debug.Assert.IsNotNull (p2x);
			Debug.Assert.IsNotNull (p3x);
			
			Debug.Assert.IsTrue (p1.FaceName == p1x.FaceName);
			Debug.Assert.IsTrue (p1.StyleName == p1x.StyleName);
			
			Debug.Assert.IsTrue (p2.Size == p2x.Size);
			Debug.Assert.IsTrue (p2.Units == p2x.Units);
			
			Debug.Assert.IsTrue (p3.EnableHyphenation == p3x.EnableHyphenation);
		}
		
		private static void TestGeneratorProperties()
		{
			TextStory          story  = new TextStory ();
			Cursors.TempCursor cursor = new Cursors.TempCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] text;
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Verdana", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			
			TextStyle style = story.StyleList.NewTextStyle (null, "Normal", TextStyleClass.Paragraph, properties);
			
			story.ConvertToStyledText ("Texte ", style, null, out text);				//	6
			story.InsertText (cursor, text);
			
			properties.Clear ();
			properties.Add (new Properties.GeneratorProperty ("G1", 0, 1));
			story.ConvertToStyledText ("généré", style, properties, out text);			//	12
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText (" automatiquement ", style, null, out text);		//	29
			story.InsertText (cursor, text);
			
			properties.Clear ();
			properties.Add (new Properties.GeneratorProperty ("G1", 0, 2));
			story.ConvertToStyledText ("[1]", style, properties, out text);				//	32
			story.InsertText (cursor, text);
			
			properties.Clear ();
			properties.Add (new Properties.GeneratorProperty ("G1", 0, 3));
			story.ConvertToStyledText ("[2]", style, properties, out text);				//	35
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("...\n", style, null, out text);
			story.InsertText (cursor, text);
			
			text = new ulong[story.TextLength];
			story.SetCursorPosition (cursor, 0);
			story.ReadText (cursor, text.Length, text);
			
			System.Diagnostics.Debug.WriteLine (story.GetDebugStyledText (text));
			
			Cursors.GeneratorCursor[] cursors = Internal.GeneratorEnumerator.CreateCursors (story, "G1");
			
			Debug.Assert.IsTrue (cursors.Length == 3);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursors[0]) == 6);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursors[1]) == 29);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursors[2]) == 32);
			
			properties.Clear ();
			properties.Add (new Properties.GeneratorProperty ("G1", 0, 4));
			story.ConvertToStyledText ("[1]", style, properties, out text);				//	3
			story.InsertText (cursor, text);
			
			properties.Clear ();
			properties.Add (new Properties.GeneratorProperty ("G1", 0, 5));
			story.ConvertToStyledText ("[2]", style, properties, out text);				//	6
			story.InsertText (cursor, text);
			
			cursors = Internal.GeneratorEnumerator.CreateCursors (story, "G1");
			
			Debug.Assert.IsTrue (cursors.Length == 5);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursors[0]) == 0);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursors[1]) == 3);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursors[2]) == 6+6);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursors[3]) == 29+6);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursors[4]) == 32+6);
		}
		
		private static void TestTraverseText()
		{
			TextStory          story  = new TextStory ();
			Cursors.TempCursor cursor = new Cursors.TempCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] text;
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Verdana", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			
			TextStyle style = story.StyleList.NewTextStyle (null, "Normal", TextStyleClass.Paragraph, properties);
			
			story.ConvertToStyledText ("1234567890", style, null, out text);
			story.InsertText (cursor, text);
			
			Traverser traverser = new Traverser ();
			
			TextStory.CodeCallback cbFalse = new TextStory.CodeCallback (traverser.TestFalse);
			TextStory.CodeCallback cbTrue  = new TextStory.CodeCallback (traverser.TestTrue);
			
			story.SetCursorPosition (cursor, 5);
			
			Debug.Assert.IsTrue (-1 == story.TextTable.TraverseText (cursor.CursorId, 5, cbFalse));
			Debug.Assert.IsTrue (0  == story.TextTable.TraverseText (cursor.CursorId, 5, cbTrue));
			Debug.Assert.IsTrue (-1 == story.TextTable.TraverseText (cursor.CursorId, -5, cbFalse));
			Debug.Assert.IsTrue (0  == story.TextTable.TraverseText (cursor.CursorId, -5, cbTrue));
		}
		
		private class Traverser
		{
			public bool TestTrue(ulong code)
			{
				return true;
			}
			
			public bool TestFalse(ulong code)
			{
				return false;
			}
		}
		
		private static void TestGetTextDistance()
		{
			TextStory          story  = new TextStory ();
			Cursors.TempCursor cursor = new Cursors.TempCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] text;
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Verdana", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			
			TextStyle style = story.StyleList.NewTextStyle (null, "Normal", TextStyleClass.Paragraph, properties);
			
			story.ConvertToStyledText ("Abc", style, null, out text);
			story.InsertText (cursor, text);
			
			Properties.GeneratorProperty g1A = new Properties.GeneratorProperty ("G1", 0, 1);
			Properties.GeneratorProperty g1B = new Properties.GeneratorProperty ("G1", 0, 2);
			
			properties.Clear (); properties.Add (g1A);
			
			story.ConvertToStyledText ("Def", style, properties, out text);
			story.InsertText (cursor, text);
			
			properties.Clear (); properties.Add (g1B);
			
			story.ConvertToStyledText ("Ghi", style, properties, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("Klm", style, null, out text);
			story.InsertText (cursor, text);
			
			story.SetCursorPosition (cursor, 0);
			
			Debug.Assert.IsTrue ('A' == (char) Unicode.Bits.GetCode (story.ReadChar (cursor, 0)));
			Debug.Assert.IsTrue ('b' == (char) Unicode.Bits.GetCode (story.ReadChar (cursor, 1)));
			Debug.Assert.IsTrue ('c' == (char) Unicode.Bits.GetCode (story.ReadChar (cursor, 2)));
			
			Debug.Assert.IsFalse (story.TextContext.ContainsProperty (story, cursor, 0, g1A));
			Debug.Assert.IsTrue (story.TextContext.ContainsProperty (story, cursor, 3, g1A));
			
			story.SetCursorPosition (cursor, 3);
			
			Debug.Assert.IsTrue ('A' == (char) Unicode.Bits.GetCode (story.ReadChar (cursor, -3)));
			Debug.Assert.IsTrue ('b' == (char) Unicode.Bits.GetCode (story.ReadChar (cursor, -2)));
			Debug.Assert.IsTrue ('c' == (char) Unicode.Bits.GetCode (story.ReadChar (cursor, -1)));
			
			Debug.Assert.IsFalse (story.TextContext.ContainsProperty (story, cursor, -1, g1A));
			Debug.Assert.IsTrue (story.TextContext.ContainsProperty (story, cursor, 0, g1A));
			Debug.Assert.IsTrue (story.TextContext.ContainsProperty (story, cursor, 3, g1B));
			
			TextContext context = story.TextContext;
			
			Debug.Assert.IsTrue (0 == context.GetTextStartDistance (story, cursor, g1A));
			Debug.Assert.IsTrue (3 == context.GetTextEndDistance (story, cursor, g1A));
			Debug.Assert.IsTrue (-1 == context.GetTextEndDistance (story, cursor, g1B));
			
			story.SetCursorPosition (cursor, 4);
			
			Debug.Assert.IsTrue (1 == context.GetTextStartDistance (story, cursor, g1A));
			Debug.Assert.IsTrue (2 == context.GetTextEndDistance (story, cursor, g1A));
			
			story.SetCursorPosition (cursor, 5);
			
			Debug.Assert.IsTrue (2 == context.GetTextStartDistance (story, cursor, g1A));
			Debug.Assert.IsTrue (1 == context.GetTextEndDistance (story, cursor, g1A));
			
			story.SetCursorPosition (cursor, 6);
			
			Debug.Assert.IsTrue (-1 == context.GetTextStartDistance (story, cursor, g1A));
			Debug.Assert.IsTrue (-1 == context.GetTextEndDistance (story, cursor, g1A));
		}
		
		private static void Ex1()
		{
			Properties.FontSizeProperty a = new Properties.FontSizeProperty (80.0/100, Properties.SizeUnits.Percent);
			Properties.FontSizeProperty b = new Properties.FontSizeProperty (-5.0, Properties.SizeUnits.DeltaPoints);
			
			a.GetCombination (b);
		}
	}
}
