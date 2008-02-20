//	Copyright � 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Tests
{
	/// <summary>
	/// V�rifie le bon fonctionnement des classes Layout.XyzEngine.
	/// </summary>
	public class CheckLayout
	{
		public static void RunTests()
		{
			CheckLayout.TestLineEngine ();
			CheckLayout.TestLineEngineWithFrame ();
			CheckLayout.TestLineEngineWithHyphens ();
			CheckLayout.TestLineEngineWithSpaces ();
			CheckLayout.TestTextToGlyphMapping ();
		}
		
		
		private static void TestLineEngineWithFrame()
		{
			TextStory story  = new TextStory ();
			ICursor   cursor = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] styled_text;
			System.Collections.ArrayList properties;
			
			properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties.Add (new Properties.MarginsProperty (40, 0, 0, 0, Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 0.0, 0.0, Properties.ThreeState.False));
			
			story.ConvertToStyledText ("Affiche juste quelques mots pour ", properties, out styled_text);	// 33
			story.InsertText (cursor, styled_text);
			
			properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Bold"));
			properties.Add (new Properties.FontSizeProperty (16.0, Properties.SizeUnits.Points));
			properties.Add (new Properties.MarginsProperty (40, 0, 0, 0, Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 0.0, 0.0, Properties.ThreeState.False));
			
			story.ConvertToStyledText ("voir", properties, out styled_text);								// 37
			story.InsertText (cursor, styled_text);
			
			properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties.Add (new Properties.MarginsProperty (40, 0, 0, 0, Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 0.0, 0.0, Properties.ThreeState.False));
			
			story.ConvertToStyledText (" comment l'algorithme de "
				/**/				 + "d�coupe des lignes se d�brouille "
				/**/				 + "lorsqu'il faut couler du texte dans " /*131*/
				/**/				 + "plusieurs cadres.\n", properties, out styled_text);
			
			story.InsertText (cursor, styled_text);
			
			story.MoveCursor (cursor, - story.TextLength);
			
			ulong[] story_text = new ulong[story.TextLength];
			
			story.ReadText (cursor, story_text.Length, story_text);
			
			TextFitter fitter = new TextFitter (story);
			FrameList  frames = new FrameList (fitter);
			
			SimpleTextFrame frame_0 = new SimpleTextFrame (200, 20);
			SimpleTextFrame frame_1 = new SimpleTextFrame (200, 1000);
			
			frames.InsertAt (0, frame_0);
			frames.InsertAt (1, frame_1);
			
			Layout.LineEngine layout  = new Layout.LineEngine ();
			Layout.Context    context = new Layout.Context (story.TextContext, story_text, 0, frames);
			
			Layout.BreakCollection breaks;
			Layout.Status status;
			
			breaks  = null;
			
			context.SelectFrame (0, 0);
			
			status  = context.Fit (ref breaks, 0);
			context.DefineTextOffset (breaks[0].Offset);
			
			System.Diagnostics.Debug.WriteLine ("CheckLayout: " + context.LineHeight.ToString ());
			
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			Debug.Assert.IsTrue (breaks[0].Offset == 28);
			Debug.Assert.IsTrue (context.FrameIndex == 0);
			Debug.Assert.IsTrue (context.LineHeight > 16.0874);
			Debug.Assert.IsTrue (context.LineHeight < 16.0876);
			
			status  = context.Fit (ref breaks, 1);
			context.DefineTextOffset (breaks[0].Offset);
			
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			Debug.Assert.IsTrue (breaks[0].Offset == 62);
			Debug.Assert.IsTrue (context.FrameIndex == 1);
			Debug.Assert.IsTrue (context.LineHeight > 21.4499);
			Debug.Assert.IsTrue (context.LineHeight < 21.4501);
			
			status  = context.Fit (ref breaks, 2);
			context.DefineTextOffset (breaks[0].Offset);
			
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			Debug.Assert.IsTrue (breaks[0].Offset == 95);
			Debug.Assert.IsTrue (context.FrameIndex == 1);
			Debug.Assert.IsTrue (context.LineHeight > 16.0874);
			Debug.Assert.IsTrue (context.LineHeight < 16.0876);
			
			status  = context.Fit (ref breaks, 3);
			context.DefineTextOffset (breaks[0].Offset);
			
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			Debug.Assert.IsTrue (breaks[0].Offset == 131);
			Debug.Assert.IsTrue (context.FrameIndex == 1);
			Debug.Assert.IsTrue (context.LineHeight > 16.0874);
			Debug.Assert.IsTrue (context.LineHeight < 16.0876);
			
			status  = context.Fit (ref breaks, 4);
			context.DefineTextOffset (breaks[0].Offset);
			
			Debug.Assert.IsTrue (status == Layout.Status.OkFitEnded);
			Debug.Assert.IsTrue (breaks[0].Offset == 149);
			Debug.Assert.IsTrue (context.FrameIndex == 1);
			Debug.Assert.IsTrue (context.LineHeight > 16.0874);
			Debug.Assert.IsTrue (context.LineHeight < 16.0876);
			
			status  = context.Fit (ref breaks, 4);
			
			Debug.Assert.IsTrue (status == Layout.Status.ErrorNeedMoreText);
			
			
			//	Elargit le cadre 0 pour permettre de placer le mot en Arial 16
			//	autant en largeur qu'en hauteur :
			
			frame_0.Width = 300;
			frame_0.Height = 22;
			
			context.DefineTextOffset (0);
			context.SelectFrame (0, 0);
			
			status  = context.Fit (ref breaks, 0);
			context.DefineTextOffset (breaks[0].Offset);
			
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			Debug.Assert.IsTrue (breaks[0].Offset == 46);
			Debug.Assert.IsTrue (context.FrameIndex == 0);
			
			//	Diminue la hauteur du cadre 0 pour que la ligne en Arial 16 ne
			//	tienne plus dans le cadre :
			
			frame_0.Width = 300;
			frame_0.Height = 18;
			
			context.DefineTextOffset (0);
			context.SelectFrame (0, 0);
			
			status  = context.Fit (ref breaks, 0);
			context.DefineTextOffset (breaks[0].Offset);
			
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			Debug.Assert.IsTrue (breaks[0].Offset == 28);
			Debug.Assert.IsTrue (context.FrameIndex == 1);
			Debug.Assert.IsTrue (context.LineHeight > 21.4499);
			Debug.Assert.IsTrue (context.LineHeight < 21.4501);
			
			status  = context.Fit (ref breaks, 0);
			context.DefineTextOffset (breaks[0].Offset);
			
			Debug.Assert.IsTrue (context.LineHeight > 21.4499);
			Debug.Assert.IsTrue (context.LineHeight < 21.4501);
			
			status  = context.Fit (ref breaks, 0);
			context.DefineTextOffset (breaks[0].Offset);
			
			Debug.Assert.IsTrue (context.LineHeight > 16.0874);
			Debug.Assert.IsTrue (context.LineHeight < 16.0876);
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
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			
			story.ConvertToStyledText ("Affiche quelques mots pour ", properties, out styled_text);
			story.InsertText (cursor, styled_text);
			
			properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Bold"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			
			story.ConvertToStyledText ("voir", properties, out styled_text);
			story.InsertText (cursor, styled_text);
			
			properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			
			story.ConvertToStyledText ("\u00A0"+"comment l'algorithme se d�brouille.", properties, out styled_text);
			story.InsertText (cursor, styled_text);
			
			story.MoveCursor (cursor, - story.TextLength);
			
			ulong[] story_text = new ulong[story.TextLength];
			
			story.ReadText (cursor, story_text.Length, story_text);
			
			Layout.LineEngine layout = new Layout.LineEngine ();
			Layout.Context    context;
			
			Layout.BreakCollection breaks;
			Layout.Status status;
			
			breaks  = null;
			context = new Layout.Context (story.TextContext, story_text, 0, -100, 14.0, 120, 0, 20, 40, 10);
			status  = context.Fit (ref breaks, 0);
			
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			Debug.Assert.IsTrue (breaks[0].Offset == 17);
			Debug.Assert.IsTrue (breaks[0].Advance + breaks[0].Profile.WidthEndSpace > 92.06);
			Debug.Assert.IsTrue (breaks[0].Advance + breaks[0].Profile.WidthEndSpace < 92.07);
			
			double y1 = context.MaxY;
			double y2 = context.MinY;
			
			context = new Layout.Context (story.TextContext, story_text, 0, -100, 14.0, 200, 0, 30, 40, 10);
			status  = context.Fit (ref breaks, 0);
			
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			Debug.Assert.IsTrue (breaks[0].Offset == 27);
			Debug.Assert.IsTrue (breaks[0].Advance + breaks[0].Profile.WidthEndSpace > 148.75);
			Debug.Assert.IsTrue (breaks[0].Advance + breaks[0].Profile.WidthEndSpace < 148.76);
			
			double y3 = context.MaxY;
			double y4 = context.MinY;
			
			context = new Layout.Context (story.TextContext, story_text, 0, -100, 14.0, 300, 0, 0, 40, 10);
			status  = context.Fit (ref breaks, 0);
			
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			Debug.Assert.IsTrue (breaks.Count == 2);
			Debug.Assert.IsTrue (breaks[0].Offset == 53);
			Debug.Assert.IsTrue (breaks[0].Advance + breaks[0].Profile.WidthEndSpace > 291.09);
			Debug.Assert.IsTrue (breaks[0].Advance + breaks[0].Profile.WidthEndSpace < 291.10);
			Debug.Assert.IsTrue (breaks[1].Offset == 56);
			Debug.Assert.IsTrue (breaks[1].Advance + breaks[1].Profile.WidthEndSpace > 307.10);
			Debug.Assert.IsTrue (breaks[1].Advance + breaks[1].Profile.WidthEndSpace < 307.11);
			
			context = new Layout.Context (story.TextContext, story_text, 0, -100, 14.0, 400, 0, 0, 40, 10);
			status  = context.Fit (ref breaks, 0);
			
			Debug.Assert.IsTrue (status == Layout.Status.ErrorNeedMoreText);
			
			context = new Layout.Context (story.TextContext, story_text, 0, -100, 14.0, 20, 0, 0, 10, 10);
			status  = context.Fit (ref breaks, 0);
			
			Debug.Assert.IsTrue (status == Layout.Status.ErrorCannotFit);
			
			context = new Layout.Context (story.TextContext, story_text, 0, -100, 14.0, 20, 0, 0, 10, 10);
			story.TextContext.IsDegradedLayoutEnabled = true;
			status  = context.Fit (ref breaks, 0);
			
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			
			story.MoveCursor (cursor, story.TextLength);
			properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			
			story.ConvertToStyledText ("\n", properties, out styled_text);
			story.InsertText (cursor, styled_text);
			story.MoveCursor (cursor, - story.TextLength);
			
			story_text = new ulong[story.TextLength];
			story.ReadText (cursor, story_text.Length, story_text);
			
			context = new Layout.Context (story.TextContext, story_text, 0, -100, 14.0, 400, 0, 0, 10, 10);
			status  = context.Fit (ref breaks, 0);
			
			Debug.Assert.IsTrue (status == Layout.Status.OkFitEnded);
			
			
			context = new Layout.Context (story.TextContext, story_text, 0, -100, 14.0, 200, 0, 0, 10, 10);
			status  = context.Fit (ref breaks, 0);
			
			Debug.Assert.IsTrue (breaks[0].Offset == 27);
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			
			context.DefineTextOffset (breaks[0].Offset);
			status  = context.Fit (ref breaks, 0);
			
			Debug.Assert.IsTrue (breaks[0].Offset == 56);
			Debug.Assert.IsTrue (status == Layout.Status.Ok);
			
			context.DefineTextOffset (breaks[0].Offset);
			status  = context.Fit (ref breaks, 0);
			
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
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			
			story.ConvertToStyledText ("Affiche quelques mots pour ", properties, out styled_text);
			story.InsertText (cursor, styled_text);
			
			properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Bold"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			
			story.ConvertToStyledText ("essayer", properties, out styled_text);
			story.InsertText (cursor, styled_text);
			
			properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			
			story.ConvertToStyledText (" l'algo-rithme.\n", properties, out styled_text);
			story.InsertText (cursor, styled_text);
			
			story.MoveCursor (cursor, - story.TextLength);
			
			ulong[] story_text = new ulong[story.TextLength];
			
			story.ReadText (cursor, story_text.Length, story_text);
			
			Layout.LineEngine layout = new Layout.LineEngine ();
			Layout.Context    context;
			
			Layout.BreakCollection breaks = null;
			Layout.Status status;
			
			context = new Layout.Context (story.TextContext, story_text, 0, 0, 14.0, 300, 0, 0, 150, 10);
			status  = context.Fit (ref breaks, 0);
			
			Debug.Assert.IsTrue (breaks.Count == 3);
			Debug.Assert.IsTrue (breaks[0].Offset == 35);
			Debug.Assert.IsTrue (breaks[1].Offset == 42);
			Debug.Assert.IsTrue (breaks[2].Offset == 50);
			Debug.Assert.IsTrue (System.Math.Round (breaks[0].SpacePenalty) == 70);
			Debug.Assert.IsTrue (System.Math.Round (breaks[1].SpacePenalty) == 38);
			Debug.Assert.IsTrue (System.Math.Round (breaks[2].SpacePenalty) == 0);
			
			
			OpenType.Font font;
			double        font_size;
			double        font_scale;
			
			story.TextContext.GetFontAndSize (story_text[0], out font, out font_size, out font_scale);
			
			Layout.StretchProfile profile = new Layout.StretchProfile ();
			profile.Add (null, font, font_size, story_text, 0, 26, 0);
			
			context = new Layout.Context (story.TextContext, story_text, 0, 0, 14.0, 300, 20, 20, 30, 0);
			status  = context.Fit (ref breaks, 0);
			
			Debug.Assert.IsTrue (breaks.Count == 1);
			Debug.Assert.IsTrue (breaks[0].Offset == 42);
			
//			System.Diagnostics.Trace.WriteLine ("Starting layout.");
//			for (int i = 0; i < 1000000; i++)
//			{
//				context = new Layout.TextContext (story.TextContext, story_text, 0, 0, 14.0, 300, 15, 15, 30, 0);
//				status  = context.Fit (ref breaks, 0);
//			}
//			System.Diagnostics.Trace.WriteLine ("Done.");
		}
		
		private static void TestLineEngineWithSpaces()
		{
			TextStory story  = new TextStory ();
			ICursor   cursor = new Cursors.SimpleCursor ();
			
			ulong[] story_text;
			
			story.NewCursor (cursor);
			
			ulong[] styled_text;
			System.Collections.ArrayList properties;
			
			properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			
			story.ConvertToStyledText ("Essai tout simple. XXXXXXXXXX", properties, out styled_text);
			story.InsertText (cursor, styled_text);
			
			story.MoveCursor (cursor, - story.TextLength);
			
			story_text = new ulong[story.TextLength];
			
			story.ReadText (cursor, story_text.Length, story_text);
			
			Layout.LineEngine layout = new Layout.LineEngine ();
			Layout.Context    context;
			
			Layout.BreakCollection breaks = null;
			Layout.Status status;
			
			context = new Layout.Context (story.TextContext, story_text, 0, 0, 14.0, 105, 0, 0, 0, 0);
			status  = context.Fit (ref breaks, 0);
			
			Debug.Assert.IsTrue (breaks.Count == 1);
			Debug.Assert.IsTrue (breaks[0].Offset == 19);
			Debug.Assert.IsTrue (breaks[0].Advance > 94.03);
			Debug.Assert.IsTrue (breaks[0].Advance < 94.04);
			
			story.MoveCursor (cursor, story.TextLength - 10);
			story.ConvertToStyledText ("    ", properties, out styled_text);
			story.InsertText (cursor, styled_text);
			
			story.MoveCursor (cursor, - (story.TextLength - 10));
			
			story_text = new ulong[story.TextLength];
			
			story.ReadText (cursor, story_text.Length, story_text);
			
			context = new Layout.Context (story.TextContext, story_text, 0, 0, 14.0, 105, 0, 0, 0, 0);
			status  = context.Fit (ref breaks, 0);
			
			Debug.Assert.IsTrue (breaks.Count == 1);
			Debug.Assert.IsTrue (breaks[0].Offset == 23);
			Debug.Assert.IsTrue (breaks[0].Advance > 94.03);
			Debug.Assert.IsTrue (breaks[0].Advance < 94.04);
		}
		
		private static void TestTextToGlyphMapping()
		{
			ulong[]  text   = new ulong[] { 'A', 'f', 'f', 'i', 'c', 'h', 'e' };
			ushort[] glyphs = new ushort[] { 1, 100, 60, 65, 62 };
			short[]  map    = new short[] { 0, 1, 4, 5, 6, 7 };
			
			Layout.TextToGlyphMapping mapping = new Layout.TextToGlyphMapping (text, 0, text.Length, glyphs, map);
			
			int[]    c;
			ushort[] g;
			
			mapping.GetNextMapping (out c, out g);
			
			Debug.Assert.IsTrue (c.Length == 1);
			Debug.Assert.IsTrue (c[0] == 'A');
			Debug.Assert.IsTrue (g.Length == 1);
			Debug.Assert.IsTrue (g[0] == 1);
			
			mapping.GetNextMapping (out c, out g);
			
			Debug.Assert.IsTrue (c.Length == 3);
			Debug.Assert.IsTrue (c[0] == 'f');
			Debug.Assert.IsTrue (c[1] == 'f');
			Debug.Assert.IsTrue (c[2] == 'i');
			Debug.Assert.IsTrue (g.Length == 1);
			Debug.Assert.IsTrue (g[0] == 100);
			
			mapping.GetNextMapping (out c, out g);
			
			Debug.Assert.IsTrue (c.Length == 1);
			Debug.Assert.IsTrue (c[0] == 'c');
			Debug.Assert.IsTrue (g.Length == 1);
			Debug.Assert.IsTrue (g[0] == 60);
			
			mapping.GetNextMapping (out c, out g);
			
			Debug.Assert.IsTrue (c.Length == 1);
			Debug.Assert.IsTrue (c[0] == 'h');
			Debug.Assert.IsTrue (g.Length == 1);
			Debug.Assert.IsTrue (g[0] == 65);
			
			mapping.GetNextMapping (out c, out g);
			
			Debug.Assert.IsTrue (c.Length == 1);
			Debug.Assert.IsTrue (c[0] == 'e');
			Debug.Assert.IsTrue (g.Length == 1);
			Debug.Assert.IsTrue (g[0] == 62);
			
			mapping.GetNextMapping (out c, out g);
			
			Debug.Assert.IsNull (c);
			Debug.Assert.IsNull (g);
			
			text   = new ulong[] { 'a', '�', 'x' };
			glyphs = new ushort[] { 1, 5, 100, 20 };
			map    = new short[] { 0, 1, 1, 2, 3 };
			
			mapping = new Layout.TextToGlyphMapping (text, 0, text.Length, glyphs, map);
			
			mapping.GetNextMapping (out c, out g);
			
			Debug.Assert.IsTrue (c.Length == 1);
			Debug.Assert.IsTrue (c[0] == 'a');
			Debug.Assert.IsTrue (g.Length == 1);
			Debug.Assert.IsTrue (g[0] == 1);
			
			mapping.GetNextMapping (out c, out g);
			
			Debug.Assert.IsTrue (c.Length == 1);
			Debug.Assert.IsTrue (c[0] == '�');
			Debug.Assert.IsTrue (g.Length == 2);
			Debug.Assert.IsTrue (g[0] == 5);
			Debug.Assert.IsTrue (g[1] == 100);
			
			mapping.GetNextMapping (out c, out g);
			
			Debug.Assert.IsTrue (c.Length == 1);
			Debug.Assert.IsTrue (c[0] == 'x');
			Debug.Assert.IsTrue (g.Length == 1);
			Debug.Assert.IsTrue (g[0] == 20);
			
			mapping.GetNextMapping (out c, out g);
			
			Debug.Assert.IsNull (c);
			Debug.Assert.IsNull (g);
		}
	}
}
