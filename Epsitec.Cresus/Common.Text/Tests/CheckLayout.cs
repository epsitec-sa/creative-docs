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
			CheckLayout.TestLineEngineWithHyphens ();
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
			
			breaks  = null;
			context = new Layout.Context (story.Context, story_text, 0, -100, 1000, 1100, 40, 10);
			status  = context.Fit (layout, ref breaks);
			
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			Debug.Assert.IsTrue (breaks[0].Offset == 17);
			Debug.Assert.IsTrue (breaks[0].Advance > 1092.06);
			Debug.Assert.IsTrue (breaks[0].Advance < 1092.07);
			
			double y1 = context.MaxY;
			double y2 = context.MinY;
			
			context = new Layout.Context (story.Context, story_text, 0, -100, 1000, 1170, 40, 10);
			status  = context.Fit (layout, ref breaks);
			
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			Debug.Assert.IsTrue (breaks[0].Offset == 27);
			Debug.Assert.IsTrue (breaks[0].Advance > 1148.75);
			Debug.Assert.IsTrue (breaks[0].Advance < 1148.76);
			
			double y3 = context.MaxY;
			double y4 = context.MinY;
			
			context = new Layout.Context (story.Context, story_text, 0, -100, 1000, 1300, 40, 10);
			status  = context.Fit (layout, ref breaks);
			
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			Debug.Assert.IsTrue (breaks[0].Offset == 56);
			Debug.Assert.IsTrue (breaks[0].Advance > 1307.10);
			Debug.Assert.IsTrue (breaks[0].Advance < 1307.11);
			
			context = new Layout.Context (story.Context, story_text, 0, -100, 1000, 1400, 40, 10);
			status  = context.Fit (layout, ref breaks);
			
			Debug.Assert.IsTrue (status == Layout.Status.ErrorNeedMoreText);
			
			context = new Layout.Context (story.Context, story_text, 0, -100, 1000, 1020, 10, 10);
			status  = context.Fit (layout, ref breaks);
			
			Debug.Assert.IsTrue (status == Layout.Status.ErrorCannotFit);
			
			story.MoveCursor (cursor, story.TextLength);
			properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.FontSizeUnits.Points));
			
			story.ConvertToStyledText ("\n", properties, out styled_text);
			story.InsertText (cursor, styled_text);
			story.MoveCursor (cursor, - story.TextLength);
			
			story_text = new ulong[story.TextLength];
			story.ReadText (cursor, story_text.Length, story_text);
			
			context = new Layout.Context (story.Context, story_text, 0, -100, 1000, 1400, 10, 10);
			status  = context.Fit (layout, ref breaks);
			
			Debug.Assert.IsTrue (status == Layout.Status.OkFitEnded);
			
			
			context = new Layout.Context (story.Context, story_text, 0, -100, 1000, 1200, 10, 10);
			status  = context.Fit (layout, ref breaks);
			
			Debug.Assert.IsTrue (breaks[0].Offset == 27);
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			
			context.TextOffset = breaks[0].Offset;
			status  = context.Fit (layout, ref breaks);
			
			Debug.Assert.IsTrue (breaks[0].Offset == 56);
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			
			context.TextOffset = breaks[0].Offset;
			status  = context.Fit (layout, ref breaks);
			
			Debug.Assert.IsTrue (breaks[0].Offset == 68);
			Debug.Assert.IsTrue (status == Layout.Status.OkFitEnded);
		}
		
		private static void TestLineEngineWithHyphens()
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
			
			story.ConvertToStyledText ("essayer", properties, out styled_text);
			story.InsertText (cursor, styled_text);
			
			properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.FontSizeUnits.Points));
			
			story.ConvertToStyledText (" l'algo-rithme.\n", properties, out styled_text);
			story.InsertText (cursor, styled_text);
			
			story.MoveCursor (cursor, - story.TextLength);
			
			ulong[] story_text = new ulong[story.TextLength];
			
			story.ReadText (cursor, story_text.Length, story_text);
			
			Layout.LineEngine layout = new Layout.LineEngine ();
			Layout.Context    context;
			
			Layout.BreakCollection breaks = null;
			Layout.Status status;
			
			context = new Layout.Context (story.Context, story_text, 0, 0, 1000, 1300, 150, 10);
			status  = context.Fit (layout, ref breaks);
			
			Debug.Assert.IsTrue (breaks.Count == 3);
			Debug.Assert.IsTrue (breaks[0].Offset == 35);
			Debug.Assert.IsTrue (breaks[1].Offset == 42);
			Debug.Assert.IsTrue (breaks[2].Offset == 50);
			
			
			OpenType.Font font;
			double        font_size;
			
			story.Context.GetFont (story_text[0], out font, out font_size);
			
			Internal.StretchProfile profile = new Internal.StretchProfile ();
			Internal.StretchProfile.Analyse (font, font_size, story_text, 0, 26, profile);
			
			context = new Layout.Context (story.Context, story_text, 0, 0, 1000, 1270, 30, 0);
			status  = context.Fit (layout, ref breaks);
			
			Debug.Assert.IsTrue (breaks.Count == 1);
			Debug.Assert.IsTrue (breaks[0].Offset == 42);
			
			System.Diagnostics.Trace.WriteLine ("Starting layout.");
			for (int i = 0; i < 1000000; i++)
			{
				context = new Layout.Context (story.Context, story_text, 0, 0, 1000, 1270, 30, 0);
				status  = context.Fit (layout, ref breaks);
			}
			System.Diagnostics.Trace.WriteLine ("Done.");
		}
	}
}
