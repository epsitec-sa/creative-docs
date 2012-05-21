//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Tests
{
	/// <summary>
	/// La batterie de tests CheckNavigator vérifie le bon fonctionnement des
	/// navigateurs.
	/// </summary>
	public sealed class CheckNavigator
	{
		public static void RunTests()
		{
			CheckNavigator.TestParagraph ();
		}
		
		private static void TestParagraph()
		{
			TextStory story  = new TextStory ();
			ICursor   cursor = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] text;
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Verdana", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties.Add (new Properties.LeadingProperty (14.0, Properties.SizeUnits.Points, Properties.AlignMode.None));
			
			TextStyle style1 = story.StyleList.NewTextStyle (null, "Normal", TextStyleClass.Paragraph, properties);
			
			properties.Clear ();
			properties.Add (new Properties.FontProperty ("Arial", "Bold"));
			properties.Add (new Properties.FontSizeProperty (15.0, Properties.SizeUnits.Points));
			properties.Add (new Properties.LeadingProperty (0, Properties.SizeUnits.Points, Properties.AlignMode.First));
			properties.Add (new Properties.FontColorProperty ("Blue"));
			
			TextStyle style2 = story.StyleList.NewTextStyle (null, "Titre", TextStyleClass.Paragraph, properties);
			
			properties.Clear ();
			properties.Add (new Properties.UnderlineProperty ());
			
			story.ConvertToStyledText ("Abc ", style1, null, out text);					//	4
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("underline", style1, properties, out text);		//	13
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText (" xyz...\n", style1, null, out text);			//	21
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("Underline.\n", style1, properties, out text);	//	32
			story.InsertText (cursor, text);
			
			properties.Clear ();
			properties.Add (new Properties.LeadingProperty (0.0, Properties.SizeUnits.Points, Properties.AlignMode.Undefined));
			properties.Add (new Properties.MarginsProperty (10.0, 10.0, Properties.SizeUnits.Millimeters));
			
			story.ConvertToStyledText ("Leading.\n", style1, properties, out text);		//	41
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("Titre.\n", style2, null, out text);				//	48
			story.InsertText (cursor, text);
			
			Debug.Assert.IsTrue (story.TextLength == 48);
			
			story.SetCursorPosition (cursor, 0);
			Debug.Assert.IsTrue (Internal.Navigator.GetParagraphStartOffset (story, cursor) == 0);
			Debug.Assert.IsTrue (Internal.Navigator.GetParagraphEndLength (story, cursor) == 21);
			
			story.SetCursorPosition (cursor, 10);
			Debug.Assert.IsTrue (Internal.Navigator.GetParagraphStartOffset (story, cursor) == -10);
			Debug.Assert.IsTrue (Internal.Navigator.GetParagraphEndLength (story, cursor) == 11);
			
			story.SetCursorPosition (cursor, 30);
			Debug.Assert.IsTrue (Internal.Navigator.GetParagraphStartOffset (story, cursor) == -9);
			Debug.Assert.IsTrue (Internal.Navigator.GetParagraphEndLength (story, cursor) == 2);
			
			story.SetCursorPosition (cursor, 48);
			Debug.Assert.IsTrue (Internal.Navigator.GetParagraphStartOffset (story, cursor) == 0);
			Debug.Assert.IsTrue (Internal.Navigator.GetParagraphEndLength (story, cursor) == 0);
			
			text = new ulong[story.TextLength];
			story.SetCursorPosition (cursor, 0);
			story.ReadText (cursor, story.TextLength, text);
			
			System.Diagnostics.Debug.WriteLine (story.GetDebugStyledText (text));
			
			Debug.Assert.IsTrue  (Internal.Navigator.IsParagraphStart (story, cursor, 0));
			Debug.Assert.IsTrue  (Internal.Navigator.IsParagraphStart (story, cursor, 21));
			Debug.Assert.IsTrue  (Internal.Navigator.IsParagraphStart (story, cursor, 41));
			Debug.Assert.IsFalse (Internal.Navigator.IsParagraphStart (story, cursor, 3));
			
			Debug.Assert.IsFalse (Internal.Navigator.IsParagraphEnd (story, cursor, 0));
			Debug.Assert.IsFalse (Internal.Navigator.IsParagraphEnd (story, cursor, 4));
			Debug.Assert.IsTrue  (Internal.Navigator.IsParagraphEnd (story, cursor, 20));
			
			TextStyle[] styles;
			Property[] props;
			
			Debug.Assert.IsTrue (Internal.Navigator.GetParagraphStyles (story, cursor, 0, out styles));
			Debug.Assert.IsTrue (styles.Length == 1);
			Debug.Assert.IsTrue (styles[0].Name == "Normal");
			
			Debug.Assert.IsTrue (Internal.Navigator.GetParagraphStyles (story, cursor, 45, out styles));
			Debug.Assert.IsTrue (styles.Length == 1);
			Debug.Assert.IsTrue (styles[0].Name == "Titre");
			
			Debug.Assert.IsTrue (Internal.Navigator.GetParagraphStyles (story, cursor, 0, out styles));
			Debug.Assert.IsTrue (styles.Length == 1);
			Debug.Assert.IsTrue (styles[0].Name == "Normal");
			
			Debug.Assert.IsTrue (Internal.Navigator.GetParagraphProperties (story, cursor, 0, out props));
			Debug.Assert.IsTrue (props.Length == 0);
			
			Debug.Assert.IsTrue (Internal.Navigator.GetParagraphProperties (story, cursor, 40, out props));
			Debug.Assert.IsTrue (props.Length == 2);
			Debug.Assert.IsTrue (props[0].WellKnownType == Properties.WellKnownType.Leading);
			Debug.Assert.IsTrue (props[1].WellKnownType == Properties.WellKnownType.Margins);
			
			Debug.Assert.IsTrue (Internal.Navigator.GetParagraphProperties (story, cursor, 5, out props));
			Debug.Assert.IsTrue (props.Length == 0);
			
			Debug.Assert.IsTrue (Internal.Navigator.GetParagraphStartOffset (story, cursor) == 0);
			
			story.SetCursorPosition (cursor, 10);
			Debug.Assert.IsTrue (Internal.Navigator.GetParagraphStartOffset (story, cursor) == -10);
			
			story.SetCursorPosition (cursor, 21);
			Debug.Assert.IsTrue (Internal.Navigator.GetParagraphStartOffset (story, cursor) == 0);
			
			story.SetCursorPosition (cursor, 40);
			Debug.Assert.IsTrue (Internal.Navigator.GetParagraphStartOffset (story, cursor) == -8);
			
			story.SetCursorPosition (cursor, 26);
			Internal.Navigator.StartParagraphIfNeeded (story, cursor);
			
			Debug.Assert.IsTrue (story.TextLength == 49);
			Debug.Assert.IsTrue (story.GetCursorPosition (cursor) == 27);
			
			text = new ulong[story.TextLength];
			story.SetCursorPosition (cursor, 0);
			story.ReadText (cursor, story.TextLength, text);
			
			System.Diagnostics.Debug.WriteLine (story.GetDebugStyledText (text));
			
			story.SetCursorPosition (cursor, 27);
			Internal.Navigator.StartParagraphIfNeeded (story, cursor);
			
			Debug.Assert.IsTrue (story.TextLength == 49);
			
			text = new ulong[story.TextLength];
			story.SetCursorPosition (cursor, 0);
			story.ReadText (cursor, story.TextLength, text);
			
			System.Diagnostics.Debug.WriteLine (story.GetDebugStyledText (text));
			
			properties.Clear ();
			properties.Add (new Properties.LeadingProperty (0.0, Properties.SizeUnits.Points, Properties.AlignMode.Undefined));
			properties.Add (new Properties.MarginsProperty (10.0, 10.0, Properties.SizeUnits.Millimeters));
			
			ICursor temp = new Cursors.TempCursor ();
			
			story.NewCursor (temp);
			story.SetCursorPosition (temp, 2);
			Internal.Navigator.SetParagraphStyles (story, temp, style2);
			Internal.Navigator.SetParagraphStyles (story, temp, style1, style2);
			Internal.Navigator.SetParagraphStyles (story, temp, style2);
			
			text = new ulong[story.TextLength];
			story.SetCursorPosition (cursor, 0);
			story.ReadText (cursor, story.TextLength, text);
			
			System.Diagnostics.Debug.WriteLine (story.GetDebugStyledText (text));
		}
	}
}
