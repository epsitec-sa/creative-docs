//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			Properties.FontProperty font_a = new Properties.FontProperty ("Futura", "Roman");
			Properties.FontProperty font_b = new Properties.FontProperty (null, "Heavy");
			Properties.FontProperty font_c = new Properties.FontProperty ("Arial", null);
			
			Properties.FontProperty font_ab = font_a.GetCombination (font_b) as Properties.FontProperty;
			Properties.FontProperty font_ac = font_a.GetCombination (font_c) as Properties.FontProperty;
			
			Debug.Assert.IsTrue (font_ab.FaceName == "Futura");
			Debug.Assert.IsTrue (font_ab.StyleName == "Heavy");
			Debug.Assert.IsTrue (font_ac.FaceName == "Arial");
			Debug.Assert.IsTrue (font_ac.StyleName == "Roman");
			
			Debug.Assert.IsTrue (font_a.ToString () == "Futura/Roman/[null]");
			Debug.Assert.IsTrue (font_b.ToString () == "[null]/Heavy/[null]");
			Debug.Assert.IsTrue (font_c.ToString () == "Arial/[null]/[null]");
		}
		
		private static void TestFontSize()
		{
			Properties.FontSizeProperty font_size_a = new Properties.FontSizeProperty ( 12.0, Properties.SizeUnits.Points);
			Properties.FontSizeProperty font_size_b = new Properties.FontSizeProperty ( 50.0/100, Properties.SizeUnits.Percent);
			Properties.FontSizeProperty font_size_c = new Properties.FontSizeProperty ( -2.0, Properties.SizeUnits.DeltaPoints);
			Properties.FontSizeProperty font_size_d = new Properties.FontSizeProperty (200.0/100, Properties.SizeUnits.Percent);
			
			Properties.FontSizeProperty font_size_ab = font_size_a.GetCombination (font_size_b) as Properties.FontSizeProperty;
			Properties.FontSizeProperty font_size_ba = font_size_b.GetCombination (font_size_a) as Properties.FontSizeProperty;
			Properties.FontSizeProperty font_size_ac = font_size_a.GetCombination (font_size_c) as Properties.FontSizeProperty;
			Properties.FontSizeProperty font_size_cb = font_size_c.GetCombination (font_size_b) as Properties.FontSizeProperty;
			Properties.FontSizeProperty font_size_bd = font_size_b.GetCombination (font_size_d) as Properties.FontSizeProperty;
			
			Debug.Assert.IsTrue (font_size_ab.ToString () == "6/pt/[NaN]");
			Debug.Assert.IsTrue (font_size_ba.ToString () == "12/pt/[NaN]");
			Debug.Assert.IsTrue (font_size_ac.ToString () == "10/pt/[NaN]");
			Debug.Assert.IsTrue (font_size_cb.ToString () == "-1/+pt/[NaN]");
			Debug.Assert.IsTrue (font_size_bd.ToString () == "1/%/[NaN]");
			
			Debug.Expect.Exception (new Debug.Method (Ex1), typeof (System.InvalidOperationException));
		}
		
		private static void TestMargins()
		{
			Properties.MarginsProperty margins_a = new Properties.MarginsProperty ();
			Properties.MarginsProperty margins_b = new Properties.MarginsProperty (15.0, 20.0, 0.0, 0.0, Properties.SizeUnits.Points, 0, 0, 0, 0, 0, Properties.ThreeState.False);
			
			Properties.MarginsProperty margins_c = margins_a.GetCombination (margins_b) as Properties.MarginsProperty;
			Properties.MarginsProperty margins_d = margins_b.GetCombination (margins_a) as Properties.MarginsProperty;
			
			margins_a = new Properties.MarginsProperty (double.NaN, 10, double.NaN, 10, Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 0.0, 0.0, Properties.ThreeState.Undefined);
			
			Properties.MarginsProperty margins_e = margins_a.GetCombination (margins_b) as Properties.MarginsProperty;
			Properties.MarginsProperty margins_f = margins_b.GetCombination (margins_a) as Properties.MarginsProperty;
			
			Debug.Assert.IsTrue (margins_c.LeftMarginFirstLine == 15.0);
			Debug.Assert.IsTrue (margins_c.LeftMarginBody      == 20.0);
			Debug.Assert.IsTrue (margins_c.RightMarginBody     ==  0.0);
			
			Debug.Assert.IsTrue (margins_d.LeftMarginFirstLine == 15.0);
			Debug.Assert.IsTrue (margins_d.LeftMarginBody      == 20.0);
			Debug.Assert.IsTrue (margins_d.RightMarginBody     ==  0.0);
			
			Debug.Assert.IsTrue (margins_e.LeftMarginFirstLine == 15.0);
			Debug.Assert.IsTrue (margins_e.LeftMarginBody      == 20.0);
			Debug.Assert.IsTrue (margins_e.RightMarginBody     ==  0.0);
			
			Debug.Assert.IsTrue (margins_f.LeftMarginFirstLine == 15.0);
			Debug.Assert.IsTrue (margins_f.LeftMarginBody      == 10.0);
			Debug.Assert.IsTrue (margins_f.RightMarginBody     == 10.0);
			
			Debug.Assert.IsTrue (margins_a.ToString () == "[NaN]/10/[NaN]/10/pt/0/0/0/0/0/[?]/-1");
			Debug.Assert.IsTrue (margins_b.ToString () == "15/20/0/0/pt/0/0/0/0/0/[false]/-1");
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
			
			TextStyle style = story.StyleList.NewTextStyle ("Normal", TextStyleClass.Paragraph, properties);
			
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
			
			TextStyle style = story.StyleList.NewTextStyle ("Normal", TextStyleClass.Paragraph, properties);
			
			story.ConvertToStyledText ("1234567890", style, null, out text);
			story.InsertText (cursor, text);
			
			Traverser traverser = new Traverser ();
			
			TextStory.CodeCallback cb_false = new TextStory.CodeCallback (traverser.TestFalse);
			TextStory.CodeCallback cb_true  = new TextStory.CodeCallback (traverser.TestTrue);
			
			story.SetCursorPosition (cursor, 5);
			
			Debug.Assert.IsTrue (-1 == story.TextTable.TraverseText (cursor.CursorId, 5, cb_false));
			Debug.Assert.IsTrue (0  == story.TextTable.TraverseText (cursor.CursorId, 5, cb_true));
			Debug.Assert.IsTrue (-1 == story.TextTable.TraverseText (cursor.CursorId, -5, cb_false));
			Debug.Assert.IsTrue (0  == story.TextTable.TraverseText (cursor.CursorId, -5, cb_true));
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
			
			TextStyle style = story.StyleList.NewTextStyle ("Normal", TextStyleClass.Paragraph, properties);
			
			story.ConvertToStyledText ("Abc", style, null, out text);
			story.InsertText (cursor, text);
			
			Properties.GeneratorProperty g1_a = new Properties.GeneratorProperty ("G1", 0, 1);
			Properties.GeneratorProperty g1_b = new Properties.GeneratorProperty ("G1", 0, 2);
			
			properties.Clear (); properties.Add (g1_a);
			
			story.ConvertToStyledText ("Def", style, properties, out text);
			story.InsertText (cursor, text);
			
			properties.Clear (); properties.Add (g1_b);
			
			story.ConvertToStyledText ("Ghi", style, properties, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("Klm", style, null, out text);
			story.InsertText (cursor, text);
			
			story.SetCursorPosition (cursor, 0);
			
			Debug.Assert.IsTrue ('A' == (char) Unicode.Bits.GetCode (story.ReadChar (cursor, 0)));
			Debug.Assert.IsTrue ('b' == (char) Unicode.Bits.GetCode (story.ReadChar (cursor, 1)));
			Debug.Assert.IsTrue ('c' == (char) Unicode.Bits.GetCode (story.ReadChar (cursor, 2)));
			
			Debug.Assert.IsFalse (story.TextContext.ContainsProperty (story, cursor, 0, g1_a));
			Debug.Assert.IsTrue (story.TextContext.ContainsProperty (story, cursor, 3, g1_a));
			
			story.SetCursorPosition (cursor, 3);
			
			Debug.Assert.IsTrue ('A' == (char) Unicode.Bits.GetCode (story.ReadChar (cursor, -3)));
			Debug.Assert.IsTrue ('b' == (char) Unicode.Bits.GetCode (story.ReadChar (cursor, -2)));
			Debug.Assert.IsTrue ('c' == (char) Unicode.Bits.GetCode (story.ReadChar (cursor, -1)));
			
			Debug.Assert.IsFalse (story.TextContext.ContainsProperty (story, cursor, -1, g1_a));
			Debug.Assert.IsTrue (story.TextContext.ContainsProperty (story, cursor, 0, g1_a));
			Debug.Assert.IsTrue (story.TextContext.ContainsProperty (story, cursor, 3, g1_b));
			
			TextContext context = story.TextContext;
			
			Debug.Assert.IsTrue (0 == context.GetTextStartDistance (story, cursor, g1_a));
			Debug.Assert.IsTrue (3 == context.GetTextEndDistance (story, cursor, g1_a));
			Debug.Assert.IsTrue (-1 == context.GetTextEndDistance (story, cursor, g1_b));
			
			story.SetCursorPosition (cursor, 4);
			
			Debug.Assert.IsTrue (1 == context.GetTextStartDistance (story, cursor, g1_a));
			Debug.Assert.IsTrue (2 == context.GetTextEndDistance (story, cursor, g1_a));
			
			story.SetCursorPosition (cursor, 5);
			
			Debug.Assert.IsTrue (2 == context.GetTextStartDistance (story, cursor, g1_a));
			Debug.Assert.IsTrue (1 == context.GetTextEndDistance (story, cursor, g1_a));
			
			story.SetCursorPosition (cursor, 6);
			
			Debug.Assert.IsTrue (-1 == context.GetTextStartDistance (story, cursor, g1_a));
			Debug.Assert.IsTrue (-1 == context.GetTextEndDistance (story, cursor, g1_a));
		}
		
		private static void Ex1()
		{
			Properties.FontSizeProperty a = new Properties.FontSizeProperty (80.0/100, Properties.SizeUnits.Percent);
			Properties.FontSizeProperty b = new Properties.FontSizeProperty (-5.0, Properties.SizeUnits.DeltaPoints);
			
			a.GetCombination (b);
		}
	}
}
