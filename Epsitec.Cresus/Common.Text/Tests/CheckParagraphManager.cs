//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			CheckParagraphManager.TestAttachDetach ();
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
			
			TextStyle style1 = story.StyleList.NewTextStyle ("Normal", TextStyleClass.Paragraph, properties);
			
			Generator generator = story.TextContext.GeneratorList.NewGenerator ("liste");
			
			generator.Add (Generator.CreateSequence (Generator.SequenceType.Alphabetic, "", ")"));
			
			ParagraphManagers.ItemListManager.Parameters items = new ParagraphManagers.ItemListManager.Parameters ();
			
			items.Generator = generator;
			items.TabItem   = new Properties.TabProperty (10.0, 0.0, null);
			items.TabBody   = new Properties.TabProperty (40.0, 0.0, null);
			
			properties.Clear ();
			properties.Add (new Properties.FontProperty ("Verdana", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties.Add (new Properties.LeadingProperty (14.0, Properties.SizeUnits.Points, Properties.AlignMode.None));
			properties.Add (new Properties.ManagedParagraphProperty ("ItemList", items.Save ()));
			properties.Add (new Properties.MarginsProperty (0, 40.0, double.NaN, double.NaN, Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 15, 5, Properties.ThreeState.Undefined));
			
			TextStyle style2 = story.StyleList.NewTextStyle ("Puces", TextStyleClass.Paragraph, properties);
			
			story.ConvertToStyledText ("Xyz\n", style1, null, out text);
			story.InsertText (cursor, text);
			
			Debug.Assert.IsTrue (story.TextLength == 4);

			story.SetCursorPosition (cursor, 0);
			Internal.Navigator.SetParagraphStylesAndProperties (story, cursor, new TextStyle[] { style2 }, null);
			
			text = new ulong[story.TextLength];
			story.SetCursorPosition (cursor, 0);
			story.ReadText (cursor, story.TextLength, text);
			
			System.Diagnostics.Debug.WriteLine (story.GetDebugStyledText (text));
			
			
			Properties.ManagedParagraphProperty mp;
			Property[] flattened;
			
			Internal.Navigator.GetFlattenedProperties (story, cursor, 0, out flattened);
			mp = Properties.ManagedParagraphProperty.Find (flattened, "ItemList");
			
			story.TextContext.ParagraphManagerList["ItemList"].AttachToParagraph (story, cursor, mp);
			Debug.Assert.IsTrue (story.TextLength == 1+2+1+4);
			
			text = new ulong[story.TextLength];
			story.SetCursorPosition (cursor, 0);
			story.ReadText (cursor, story.TextLength, text);
			
			System.Diagnostics.Debug.WriteLine (story.GetDebugStyledText (text));
			
			//	Supprime la liste à puces :
			
			story.SetCursorPosition (cursor, 0);
			
			Internal.Navigator.GetFlattenedProperties (story, cursor, 0, out flattened);
			mp = Properties.ManagedParagraphProperty.Find (flattened, "ItemList");
			
			Internal.Navigator.SetParagraphStylesAndProperties (story, cursor, new TextStyle[] { style1 }, null);
			story.TextContext.ParagraphManagerList["ItemList"].DetachFromParagraph (story, cursor, mp);
			
			text = new ulong[story.TextLength];
			story.SetCursorPosition (cursor, 0);
			story.ReadText (cursor, story.TextLength, text);
			
			System.Diagnostics.Debug.WriteLine (story.GetDebugStyledText (text));
			
			Debug.Assert.IsTrue (story.TextLength == 4);
		}
	}
}
