//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Tests
{
	/// <summary>
	/// La batterie de tests CheckParagraphManager vérifie le bon fonctionnement
	/// des gestionnaires de paragraphes (puces, etc.)
	/// </summary>
	public sealed class CheckParagraphManager
	{
		public static void RunTests()
		{
			CheckParagraphManager.TestNavigation ();
			CheckParagraphManager.TestAttachDetach ();
		}
		
		private static void TestNavigation()
		{
			TextStory     story     = new TextStory ();
			TextNavigator navigator = new TextNavigator (story);
			
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			properties.Clear ();
			properties.Add (new Properties.FontProperty ("Verdana", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties.Add (new Properties.LeadingProperty (14.0, Properties.SizeUnits.Points, Properties.AlignMode.None));
			properties.Add (new Properties.MarginsProperty (5.0, 5.0, 5.0, 5.0, Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 15, 5, Properties.ThreeState.True));
			
			story.TextContext.DefaultParagraphStyle = story.StyleList.NewTextStyle (null, "Normal", TextStyleClass.Paragraph, properties);
			
			navigator.Insert ("Abcdef");
			navigator.MoveTo (3, 1);
			
			ulong[] text1;
			ulong[] text2;
			
			Properties.AutoTextProperty at;
			Properties.AutoTextProperty at1 = new Epsitec.Common.Text.Properties.AutoTextProperty ("NX");
			Properties.AutoTextProperty at2 = new Epsitec.Common.Text.Properties.AutoTextProperty ("NN");
			Properties.AutoTextProperty at3 = new Epsitec.Common.Text.Properties.AutoTextProperty ("NN");
			
			//	Deux propriétés AutoText identiques ne le sont jamais (à cause de
			//	leur identificateur unique) :
			
			Debug.Assert.IsFalse (Property.CompareEqualContents (at2, at3));
			
			story.ConvertToStyledText ("X", story.TextContext.DefaultParagraphStyle, new Property[] { at1 }, out text1);
			story.ConvertToStyledText ("12", story.TextContext.DefaultParagraphStyle, new Property[] { at2 }, out text2);
			
			story.InsertText (navigator.ActiveCursor, text1);
			story.InsertText (navigator.ActiveCursor, text2);
			
			//	Le texte est maintenant "Abc" + "X" + "12" + "def" avec les
			//	fragments "X" -> at1 et "12" -> at2.
			
			Debug.Assert.Equals ("AbcX12def", story.GetDebugText ());
			Debug.Assert.Equals (6, navigator.CursorPosition);
			
			navigator.MoveTo (TextNavigator.Target.CharacterNext, 1);
			Debug.Assert.Equals (7, navigator.CursorPosition);
			
			navigator.MoveTo (TextNavigator.Target.CharacterPrevious, 1);
			Debug.Assert.Equals (6, navigator.CursorPosition);
			
			//	On recule d'un caractère, mais on en saute 2 à cause de 'at2'.
			
			navigator.MoveTo (TextNavigator.Target.CharacterPrevious, 1);
			Debug.Assert.Equals (4, navigator.CursorPosition);
			Debug.Assert.IsTrue (story.TextContext.GetAutoText (story.ReadChar (navigator.ActiveCursor), out at));
			Debug.Assert.IsTrue (Property.CompareEqualContents (at, at2));
			
			//	On recule d'un caractère et on en saute effectivement 1, même
			//	si 'at1' décore "X" (vérifie que le code de navigation est OK).
			
			navigator.MoveTo (TextNavigator.Target.CharacterPrevious, 1);
			Debug.Assert.Equals (3, navigator.CursorPosition);
			Debug.Assert.IsTrue (story.TextContext.GetAutoText (story.ReadChar (navigator.ActiveCursor), out at));
			Debug.Assert.IsTrue (Property.CompareEqualContents (at, at1));
			
			navigator.MoveTo (TextNavigator.Target.CharacterPrevious, 1);
			Debug.Assert.Equals (2, navigator.CursorPosition);
			
			navigator.MoveTo (TextNavigator.Target.CharacterNext, 2);
			Debug.Assert.Equals (4, navigator.CursorPosition);

			//	On se trouve à cheval entre "X" et "12". Les propriétés visibles
			//	par le navigateur ne reflètent jamais AutoText, car une insertion
			//	à ce point insère du texte normal !
			
			foreach (Property property in navigator.AccumulatedTextProperties)
			{
				Debug.Assert.IsFalse (property.WellKnownType == Properties.WellKnownType.AutoText);
			}
		}
		
		private static void TestAttachDetach()
		{
			TextStory story  = new TextStory ();
			ICursor   cursor = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] text;
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			properties.Clear ();
			properties.Add (new Properties.FontProperty ("Verdana", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties.Add (new Properties.LeadingProperty (14.0, Properties.SizeUnits.Points, Properties.AlignMode.None));
			properties.Add (new Properties.MarginsProperty (5.0, 5.0, 5.0, 5.0, Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 15, 5, Properties.ThreeState.True));
			
			TextStyle style1 = story.StyleList.NewTextStyle (null, "Normal", TextStyleClass.Paragraph, properties);
			
			Generator generator = story.TextContext.GeneratorList.NewGenerator ("liste");
			
			generator.Add (Generator.CreateSequence (Generator.SequenceType.Alphabetic, "", ")"));
			
			ParagraphManagers.ItemListManager.Parameters items = new ParagraphManagers.ItemListManager.Parameters ();
			
			TabList tabs = story.TextContext.TabList;
			
			items.Generator = generator;
			items.TabItem   = tabs.NewTab ("T.item", 10.0, Properties.SizeUnits.Points, 0.0, null, TabPositionMode.Absolute);
			items.TabBody   = tabs.NewTab ("T.body", 40.0, Properties.SizeUnits.Points, 0.0, null, TabPositionMode.Absolute);
			
			properties.Clear ();
			properties.Add (new Properties.FontProperty ("Verdana", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties.Add (new Properties.LeadingProperty (14.0, Properties.SizeUnits.Points, Properties.AlignMode.None));
			properties.Add (new Properties.ManagedParagraphProperty ("ItemList", items.Save ()));
			properties.Add (new Properties.MarginsProperty (0, 40.0, double.NaN, double.NaN, Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 15, 5, Properties.ThreeState.Undefined));
			
			TextStyle style2 = story.StyleList.NewTextStyle (null, "Puces", TextStyleClass.Paragraph, properties);
			
			story.ConvertToStyledText ("Xyz\n", style1, null, out text);
			story.InsertText (cursor, text);
			
			Debug.Assert.IsTrue (story.TextLength == 4);

			text = new ulong[story.TextLength];
			story.SetCursorPosition (cursor, 0);
			story.ReadText (cursor, story.TextLength, text);
			
			System.Diagnostics.Debug.WriteLine ("Before SetParagraphStylesAndProperties (style2) :");
			System.Diagnostics.Debug.WriteLine (story.GetDebugStyledText (text));
			
			//	Crée la liste à puces :
			
			ICursor temp = new Cursors.TempCursor ();
			
			story.NewCursor (temp);
			story.SetCursorPosition (temp, 2);
			Internal.Navigator.SetParagraphStyles (story, temp, style2);
			
			Properties.ManagedParagraphProperty[] mpp;
			
			Debug.Assert.IsTrue (story.TextLength == 1+2+1+4);
			Debug.Assert.IsTrue (Internal.Navigator.GetManagedParagraphProperties (story, temp, 0, out mpp));
			Debug.Assert.IsTrue (mpp.Length == 1);
			Debug.Assert.IsTrue (mpp[0].ManagerName == "ItemList");
			
			text = new ulong[story.TextLength];
			story.SetCursorPosition (temp, 0);
			story.ReadText (temp, story.TextLength, text);
			
			System.Diagnostics.Debug.WriteLine ("After SetParagraphStylesAndProperties (style2) :");
			System.Diagnostics.Debug.WriteLine (story.GetDebugStyledText (text));
			
			//	Supprime la liste à puces :
			
			story.SetCursorPosition (temp, 1);
			Internal.Navigator.SetParagraphStyles (story, temp, style1);
			
			Debug.Assert.IsTrue (Internal.Navigator.GetManagedParagraphProperties (story, temp, 0, out mpp));
			Debug.Assert.IsTrue (mpp.Length == 0);
			
			text = new ulong[story.TextLength];
			story.SetCursorPosition (temp, 0);
			story.ReadText (temp, story.TextLength, text);
			
			System.Diagnostics.Debug.WriteLine ("After SetParagraphStylesAndProperties (style1) :");
			System.Diagnostics.Debug.WriteLine (story.GetDebugStyledText (text));
			
			Debug.Assert.IsTrue (story.TextLength == 4);
		}
	}
}
