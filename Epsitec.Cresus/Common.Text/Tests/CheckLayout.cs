//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Tests
{
	/// <summary>
	/// Vérifie le bon fonctionnement des classes Layout.XyzEngine.
	/// </summary>
	public class CheckLayout
	{
		public static void RunTests()
		{
			CheckLayout.TestLineEngine ();
		}
		
		private static void TestLineEngine()
		{
			TextStory story  = new TextStory ();
			ICursor   cursor = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] styled_text;
			System.Collections.ArrayList properties;
			
			properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.FontSizeUnits.Points));
			
			story.ConvertToStyledText ("Affiche quelques mots pour ", properties, out styled_text);
			story.InsertText (cursor, styled_text);
			
			properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Bold"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.FontSizeUnits.Points));
			
			story.ConvertToStyledText ("voir", properties, out styled_text);
			story.InsertText (cursor, styled_text);
			
			properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.FontSizeUnits.Points));
			
			story.ConvertToStyledText ("\u00A0"+"comment l'algorithme se débrouille.", properties, out styled_text);
			story.InsertText (cursor, styled_text);
			
			story.MoveCursor (cursor, - story.TextLength);
			
			ulong[] story_text = new ulong[story.TextLength];
			
			story.ReadText (cursor, story_text.Length, story_text);
			
			Layout.LineEngine layout = new Layout.LineEngine ();
			Layout.Context    context;
			
			Layout.BreakCollection breaks;
			Layout.Status status;
			
			context = new Layout.Context (story.Context, story_text, 0, 0, 1000, 1100, 40, 10);
			status  = layout.Fit (context, out breaks);
			
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			Debug.Assert.IsTrue (breaks[0].Offset == 17);
			Debug.Assert.IsTrue (breaks[0].Advance > 1092.06);
			Debug.Assert.IsTrue (breaks[0].Advance < 1092.07);
			
			context = new Layout.Context (story.Context, story_text, 0, 0, 1000, 1170, 40, 10);
			status  = layout.Fit (context, out breaks);
			
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			Debug.Assert.IsTrue (breaks[0].Offset == 27);
			Debug.Assert.IsTrue (breaks[0].Advance > 1148.75);
			Debug.Assert.IsTrue (breaks[0].Advance < 1148.76);
			
			context = new Layout.Context (story.Context, story_text, 0, 0, 1000, 1300, 40, 10);
			status  = layout.Fit (context, out breaks);
			
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			Debug.Assert.IsTrue (breaks[0].Offset == 56);
			Debug.Assert.IsTrue (breaks[0].Advance > 1307.10);
			Debug.Assert.IsTrue (breaks[0].Advance < 1307.11);
			
			context = new Layout.Context (story.Context, story_text, 0, 0, 1000, 1400, 40, 10);
			status  = layout.Fit (context, out breaks);
			
			Debug.Assert.IsTrue (status == Layout.Status.ErrorNeedMoreText);
			
			context = new Layout.Context (story.Context, story_text, 0, 0, 1000, 1020, 10, 10);
			status  = layout.Fit (context, out breaks);
			
			Debug.Assert.IsTrue (status == Layout.Status.ErrorCannotFit);
		}
	}
}
