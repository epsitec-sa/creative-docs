//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Tests
{
	/// <summary>
	/// Vérifie le bon fonctionnement de la classe CheckTextFitter.
	/// </summary>
	public sealed class CheckTextFitter
	{
		public static void RunTests()
		{
//			CheckTextFitter.TestSimpleTextFrame ();
//			CheckTextFitter.TestFit ();
//			CheckTextFitter.TestFitTabs1 ();
//			CheckTextFitter.TestFitTabs2 ();
//			CheckTextFitter.TestFitTabs3 ();
//			CheckTextFitter.TestFitTabs4 ();
			CheckTextFitter.TestCursorGeometry ();
		}
		
		
		private static void TestSimpleTextFrame()
		{
			SimpleTextFrame frame = new SimpleTextFrame ();
			
			frame.X      = 100;
			frame.Y      = 200;
			frame.Height = 50;
			frame.Width  = 120;
			
			frame.PageNumber = 0;
			
			double y = 0;
			
			double asc    = 10;
			double desc   = -2;
			double height = 14;
			
			double ox, oy, width;
			bool ok;
			
			ok = frame.ConstrainLineBox (y, asc, desc, height, height, false, out ox, out oy, out width, out y);
			
			Debug.Assert.IsTrue (ok);
			Debug.Assert.IsTrue (oy == -11);
			
			ok = frame.ConstrainLineBox (y, asc, desc, height, height, false, out ox, out oy, out width, out y);
			
			Debug.Assert.IsTrue (ok);
			Debug.Assert.IsTrue (oy == -25);
			
			ok = frame.ConstrainLineBox (y, asc, desc, height, height, false, out ox, out oy, out width, out y);
						
			Debug.Assert.IsTrue (ok);
			Debug.Assert.IsTrue (oy == -39);
			
			ok = frame.ConstrainLineBox (y, asc, desc, height, height, false, out ox, out oy, out width, out y);
			
			Debug.Assert.IsFalse (ok);
			Debug.Assert.IsTrue (oy == -53);
			
			ok = frame.ConstrainLineBox (y, asc, desc, height, height, false, out ox, out oy, out width, out y);
			
			Debug.Assert.IsFalse (ok);
			Debug.Assert.IsTrue (oy == -53);
		}
		
		private static void TestFit()
		{
			TextStory story  = new TextStory ();
			ICursor   cursor = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] text;
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties.Add (new Properties.FontSizeProperty (24.0, Properties.SizeUnits.Points));
			properties.Add (new Properties.ColorProperty (Drawing.Color.FromName ("Black")));
			properties.Add (new Properties.MarginsProperty (200, 0, 0, 0, Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			
			string[] words = "Bonjour, ceci est un texte d'exemple permettant de vérifier le bon fonctionnement des divers algorithmes de découpe. Le nombre de mots moyen s'élève à environ 40 mots par paragraphe, ce qui correspond à des paragraphes de taille réduite.\n".Split (' ');
			System.Random random = new System.Random (0);
			
			int count = 0;
			int para  = 0;
			
			System.Diagnostics.Trace.WriteLine ("Generating random text.");
			
			System.Text.StringBuilder paragraph = new System.Text.StringBuilder ();
			
			while (story.TextLength < 1000000)
			{
				int    index  = random.Next (words.Length);
				string source = words[index];
				
				paragraph.Append (source);
				
				if (source.EndsWith (".\n"))
				{
					story.ConvertToStyledText (paragraph.ToString (), properties, out text);
					story.InsertText (cursor, text);
					
					paragraph.Length = 0;
					
					para++;
				}
				else
				{
					paragraph.Append (" ");
				}
				
				count++;
			}
			
			if (paragraph.Length > 0)
			{
				story.ConvertToStyledText (paragraph.ToString (), properties, out text);
				story.InsertText (cursor, text);
			}
			
			System.Diagnostics.Trace.WriteLine (string.Format ("Generated {0} words, {1} paragraphs, total: {2} characters, {3:0.00} words/paragraph.", count, para, story.TextLength, 1.0*count/para));
			
			TextFitter fitter = new TextFitter (story);
			
			for (int i = 0; i < 320; i++)
			{
				SimpleTextFrame frame = new SimpleTextFrame (1000, 1400);
				frame.PageNumber = i;
				fitter.FrameList.InsertAt (i, frame);
			}
			
			System.Diagnostics.Trace.WriteLine ("Fitter: generate.");
			fitter.GenerateAllMarks ();
			System.Diagnostics.Trace.WriteLine ("Done.");
			System.Diagnostics.Trace.WriteLine (string.Format ("Fitter produced {0} paragraphs.", fitter.CursorCount));
			
			CursorInfo[] infos = story.TextTable.FindCursors (0, story.TextLength, Cursors.FitterCursor.Filter);
			count = 0;
			
			System.IO.StreamWriter writer = new System.IO.StreamWriter (@"c:\text.txt", false, System.Text.Encoding.UTF8);
			story.MoveCursor (cursor, -story.TextLength);
			
			int last_page = 0;
			
			foreach (CursorInfo info in infos)
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				Cursors.FitterCursor fitter_cursor = story.TextTable.GetCursorInstance (info.CursorId) as Cursors.FitterCursor;
				
				buffer.AppendFormat ("{0}:", story.GetCursorPosition (fitter_cursor));
				
				int pos = 0;
				
				foreach (Cursors.FitterCursor.Element element in fitter_cursor.Elements)
				{
					int    length = element.Length;
					string text_str;
					
					text = new ulong[length];
					
					story.ReadText (cursor, length, text);
					story.MoveCursor (cursor, length);
					
					if (last_page != fitter.FrameList[element.FrameIndex].PageNumber)
					{
						last_page = fitter.FrameList[element.FrameIndex].PageNumber;
						writer.WriteLine ("_{0}___________________________________________________________________\r\n", last_page);
					}
					
					TextConverter.ConvertToString (text, out text_str);
					writer.WriteLine (text_str.Replace ("\n", "<\r\n"));
					
					buffer.AppendFormat (" {0}", element.Length);
					
					count += 1;
					pos   += length;
				}
				
				System.Console.Out.WriteLine (buffer.ToString ());
			}
			
			writer.Close ();
			
			System.Diagnostics.Trace.WriteLine (string.Format ("Fitter produced {0} paragraphs and {1} lines.", fitter.CursorCount, count));
			
			ZeroRenderer renderer = new ZeroRenderer ();
			
			System.Diagnostics.Trace.WriteLine ("Fitter: render (1) -- full document x 1");
			foreach (CursorInfo info in infos)
			{
				Cursors.FitterCursor fitter_cursor = story.TextTable.GetCursorInstance (info.CursorId) as Cursors.FitterCursor;
				fitter.RenderParagraph (fitter_cursor, renderer);
				renderer.NewParagraph ();
			}
			System.Diagnostics.Trace.WriteLine ("Done.");
			
			foreach (CursorInfo info in infos)
			{
				Cursors.FitterCursor fitter_cursor = story.TextTable.GetCursorInstance (info.CursorId) as Cursors.FitterCursor;
				
				if (fitter_cursor.Elements.Length == 5)
				{
					System.Diagnostics.Trace.WriteLine ("Fitter: render (2) -- single paragraph x 1000");
					for (int i = 0; i < 1000; i++)
					{
						fitter.RenderParagraph (fitter_cursor, renderer);
					}
					System.Diagnostics.Trace.WriteLine ("Done.");
					break;
				}
			}
			
			System.Diagnostics.Trace.WriteLine ("Fitter: render (3) -- single frame x 1000");
			for (int i = 0; i < 1000; i++)
			{
				fitter.RenderTextFrame (fitter.FrameList[0], renderer);
			}
			System.Diagnostics.Trace.WriteLine ("Done.");
			
			System.Diagnostics.Trace.WriteLine ("Fitter: clear.");
			fitter.ClearAllMarks ();
			System.Diagnostics.Trace.WriteLine ("Done.");
		}
		
		private static void TestFitTabs1()
		{
			TextStory story  = new TextStory ();
			ICursor   cursor = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] text;
			int     length;
			
			System.Collections.ArrayList properties_1 = new System.Collections.ArrayList ();
			System.Collections.ArrayList properties_2 = new System.Collections.ArrayList ();
			
			TabList tabs = story.TextContext.TabList;
			
			properties_1.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties_1.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties_1.Add (new Properties.ColorProperty (Drawing.Color.FromName ("Black")));
			properties_1.Add (new Properties.MarginsProperty (0, 0, 0, 0, Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			properties_1.Add (tabs.NewTab ("T1", 60, Properties.SizeUnits.Points, 0.0, null));
			
			properties_2.Add (new Properties.FontProperty ("Arial", "Bold"));
			properties_2.Add (new Properties.FontSizeProperty (12.5, Properties.SizeUnits.Points));
			properties_2.Add (new Properties.ColorProperty (Drawing.Color.FromName ("Black")));
			properties_2.Add (new Properties.MarginsProperty (0, 0, 0, 0, Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			
			story.ConvertToStyledText ("Text:\t", properties_1, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("T", properties_2, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("1\nXyz blablabla blablabla blablah blah.\tT2 et du texte pour la suite...\n", properties_1, out text);
			story.InsertText (cursor, text);
			
			/*
			 *	Text:---->T1
			 * 
			 *	Xyz blablabla blablabla 
			 *	blablah blah.-------------
			 *	--------->T2 et du texte
			 *	pour la suite...
			 * 
			 *	[vide]
			 */
			
			story.MoveCursor (cursor, - story.TextLength);
			
			length = story.TextLength;
			text   = new ulong[length];
			
			story.ReadText (cursor, length, text);
			
			
			TextFitter      fitter = new TextFitter (story);
			SimpleTextFrame frame  = new SimpleTextFrame (150, 600);
			frame.PageNumber = 0;
			fitter.FrameList.InsertAt (0, frame);
			
			fitter.GenerateAllMarks ();
			
			CursorInfo[] infos = story.TextTable.FindCursors (0, story.TextLength, Cursors.FitterCursor.Filter);
			
			foreach (CursorInfo info in infos)
			{
				Cursors.FitterCursor fitter_cursor = story.TextTable.GetCursorInstance (info.CursorId) as Cursors.FitterCursor;
				
				System.Console.Out.WriteLine ("{0}:", story.GetCursorPosition (fitter_cursor));
				
				foreach (Cursors.FitterCursor.Element elem in fitter_cursor.Elements)
				{
					System.Console.Out.WriteLine ("    [{0:0.00}:{1:0.00}], width={4:0.00}/{2:0.00}, length={3}", elem.LineStartX, elem.LineBaseY, elem.LineWidth, elem.Length, elem.Profile.TotalWidth);
				}
			}
		}
		
		private static void TestFitTabs2()
		{
			TextStory story  = new TextStory ();
			ICursor   cursor = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] text;
			int     length;
			
			System.Collections.ArrayList properties_1 = new System.Collections.ArrayList ();
			System.Collections.ArrayList properties_2 = new System.Collections.ArrayList ();
			
			TabList tabs = story.TextContext.TabList;
			
			properties_1.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties_1.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties_1.Add (new Properties.ColorProperty (Drawing.Color.FromName ("Black")));
			properties_1.Add (new Properties.MarginsProperty (0, 0, 0, 0, Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			properties_1.Add (tabs.NewTab ("T1", 60, Properties.SizeUnits.Points, 0.5, null));
			
			properties_2.Add (new Properties.FontProperty ("Arial", "Bold"));
			properties_2.Add (new Properties.FontSizeProperty (12.5, Properties.SizeUnits.Points));
			properties_2.Add (new Properties.ColorProperty (Drawing.Color.FromName ("Black")));
			properties_2.Add (new Properties.MarginsProperty (0, 0, 0, 0, Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			
			story.ConvertToStyledText ("Text:\t", properties_1, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("T", properties_2, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("1\nXyz blablabla blablabla blablah blah.\tT2, texte centré\n", properties_1, out text);
			story.InsertText (cursor, text);
			
			/*
			 *	Text:--->T1<-
			 * 
			 *	Xyz blablabla blablabla 
			 *	blablah blah.-------------
			 *	--->T2, texte centré<---
			 * 
			 *	[vide]
			 */
			
			story.MoveCursor (cursor, - story.TextLength);
			
			length = story.TextLength;
			text   = new ulong[length];
			
			story.ReadText (cursor, length, text);
			
			
			TextFitter      fitter = new TextFitter (story);
			SimpleTextFrame frame  = new SimpleTextFrame (150, 600);
			frame.PageNumber = 0;
			fitter.FrameList.InsertAt (0, frame);
			
			fitter.GenerateAllMarks ();
			
			CursorInfo[] infos = story.TextTable.FindCursors (0, story.TextLength, Cursors.FitterCursor.Filter);
			
			foreach (CursorInfo info in infos)
			{
				Cursors.FitterCursor fitter_cursor = story.TextTable.GetCursorInstance (info.CursorId) as Cursors.FitterCursor;
				
				System.Console.Out.WriteLine ("{0}:", story.GetCursorPosition (fitter_cursor));
				
				foreach (Cursors.FitterCursor.Element elem in fitter_cursor.Elements)
				{
					System.Console.Out.WriteLine ("    [{0:0.00}:{1:0.00}], width={4:0.00}/{2:0.00}, length={3}", elem.LineStartX, elem.LineBaseY, elem.LineWidth, elem.Length, elem.Profile.TotalWidth);
				}
			}
			
			ZeroRenderer renderer = new ZeroRenderer ();
			
			foreach (CursorInfo info in infos)
			{
				Cursors.FitterCursor fitter_cursor = story.TextTable.GetCursorInstance (info.CursorId) as Cursors.FitterCursor;
				fitter.RenderParagraph (fitter_cursor, renderer);
				renderer.NewParagraph ();
			}
		}
		
		private static void TestFitTabs3()
		{
			TextStory story  = new TextStory ();
			ICursor   cursor = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] text;
			int     length;
			
			System.Collections.ArrayList properties_1 = new System.Collections.ArrayList ();
			System.Collections.ArrayList properties_2 = new System.Collections.ArrayList ();
			
			TabList tabs = story.TextContext.TabList;
			
			properties_1.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties_1.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties_1.Add (new Properties.ColorProperty (Drawing.Color.FromName ("Black")));
			properties_1.Add (new Properties.MarginsProperty (0, 0, 0, 0, Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			properties_1.Add (tabs.NewTab ("T1", 60, Properties.SizeUnits.Points, 0.0, null));
			
			properties_2.Add (new Properties.FontProperty ("Arial", "Bold"));
			properties_2.Add (new Properties.FontSizeProperty (12.5, Properties.SizeUnits.Points));
			properties_2.Add (new Properties.ColorProperty (Drawing.Color.FromName ("Black")));
			properties_2.Add (new Properties.MarginsProperty (0, 0, 0, 0, Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			
			story.ConvertToStyledText ("Text:\t", properties_1, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("T", properties_2, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("1\nXyz blablabla blablabla blablah blah.\tT2 et du texte pour la suite...\n", properties_1, out text);
			story.InsertText (cursor, text);
			
			/*
			 *	Text:->T1
			 * 
			 *	Xyz blablabla blablabla 
			 *	blablah blah.->T2 et du
			 *	texte pour la suite...
			 * 
			 *	[vide]
			 */
			
			story.MoveCursor (cursor, - story.TextLength);
			
			length = story.TextLength;
			text   = new ulong[length];
			
			story.ReadText (cursor, length, text);
			
			
			TextFitter      fitter = new TextFitter (story);
			SimpleTextFrame frame  = new SimpleTextFrame (150, 600);
			frame.PageNumber = 0;
			fitter.FrameList.InsertAt (0, frame);
			
			fitter.GenerateAllMarks ();
			
			CursorInfo[] infos = story.TextTable.FindCursors (0, story.TextLength, Cursors.FitterCursor.Filter);
			
			foreach (CursorInfo info in infos)
			{
				Cursors.FitterCursor fitter_cursor = story.TextTable.GetCursorInstance (info.CursorId) as Cursors.FitterCursor;
				
				System.Console.Out.WriteLine ("{0}:", story.GetCursorPosition (fitter_cursor));
				
				foreach (Cursors.FitterCursor.Element elem in fitter_cursor.Elements)
				{
					System.Console.Out.WriteLine ("    [{0:0.00}:{1:0.00}], width={4:0.00}/{2:0.00}, length={3}", elem.LineStartX, elem.LineBaseY, elem.LineWidth, elem.Length, elem.Profile.TotalWidth);
				}
			}
		}
		
		private static void TestFitTabs4()
		{
			TextStory story  = new TextStory ();
			ICursor   cursor = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] text;
			int     length;
			
			System.Collections.ArrayList properties_1 = new System.Collections.ArrayList ();
			System.Collections.ArrayList properties_2 = new System.Collections.ArrayList ();
			
			TabList tabs = story.TextContext.TabList;
			
			properties_1.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties_1.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties_1.Add (new Properties.ColorProperty (Drawing.Color.FromName ("Black")));
			properties_1.Add (new Properties.MarginsProperty (0, 0, 0, 0, Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			properties_1.Add (tabs.NewTab ("T1", 60, Properties.SizeUnits.Points, 0.0, null));
			
			properties_2.Add (new Properties.FontProperty ("Arial", "Bold"));
			properties_2.Add (new Properties.FontSizeProperty (12.5, Properties.SizeUnits.Points));
			properties_2.Add (new Properties.ColorProperty (Drawing.Color.FromName ("Black")));
			properties_2.Add (new Properties.MarginsProperty (60, 60, 0, 0, Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			
			story.ConvertToStyledText ("Text:\t", properties_1, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("Tout un paragraphe indenté (comme si le tabulateur se comportait comme un indentateur).\n", properties_2, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("Xyz\n", properties_1, out text);
			story.InsertText (cursor, text);
			
			/*
			 *	Text:---->Tout un 
			 *			  paragraphe 
			 *			  indenté 
			 *			  (comme si le 
			 *			  tabulateur se 
			 *			  comportait 
			 *			  comme un 
			 *			  indentateur).
			 * 
			 *  Xyz
			 *
			 *	[vide]
			 */
			
			story.MoveCursor (cursor, - story.TextLength);
			
			length = story.TextLength;
			text   = new ulong[length];
			
			story.ReadText (cursor, length, text);
			
			
			TextFitter      fitter = new TextFitter (story);
			SimpleTextFrame frame  = new SimpleTextFrame (150, 600);
			frame.PageNumber = 0;
			fitter.FrameList.InsertAt (0, frame);
			
			fitter.GenerateAllMarks ();
			
			CursorInfo[] infos = story.TextTable.FindCursors (0, story.TextLength, Cursors.FitterCursor.Filter);
			
			foreach (CursorInfo info in infos)
			{
				Cursors.FitterCursor fitter_cursor = story.TextTable.GetCursorInstance (info.CursorId) as Cursors.FitterCursor;
				
				System.Console.Out.WriteLine ("{0}:", story.GetCursorPosition (fitter_cursor));
				
				foreach (Cursors.FitterCursor.Element elem in fitter_cursor.Elements)
				{
					System.Console.Out.WriteLine ("    [{0:0.00}:{1:0.00}], width={4:0.00}/{2:0.00}, length={3}", elem.LineStartX, elem.LineBaseY, elem.LineWidth, elem.Length, elem.Profile.TotalWidth);
				}
			}
		}
		
		private static void TestCursorGeometry()
		{
			TextStory story  = new TextStory ();
			ICursor   cursor = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] text;
			
			System.Collections.ArrayList properties_1 = new System.Collections.ArrayList ();
			System.Collections.ArrayList properties_2 = new System.Collections.ArrayList ();
			System.Collections.ArrayList properties_3 = new System.Collections.ArrayList ();
			
			TabList tabs = story.TextContext.TabList;
			
			Properties.FontProperty font_regular = new Properties.FontProperty ("Arial", "Regular", "liga");
			Properties.FontProperty font_bold    = new Properties.FontProperty ("Arial", "Bold");
			Properties.FontProperty font_italic  = new Properties.FontProperty ("Arial", "Italic");
			
			properties_1.Add (font_regular);
			properties_1.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties_1.Add (new Properties.ColorProperty (Drawing.Color.FromName ("Black")));
			properties_1.Add (new Properties.MarginsProperty (0, 0, 0, 0, Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			properties_1.Add (tabs.NewTab ("T1", 60, Properties.SizeUnits.Points, 0.0, null));
			
			properties_2.Add (font_bold);
			properties_2.Add (new Properties.FontSizeProperty (12.5, Properties.SizeUnits.Points));
			properties_2.Add (new Properties.ColorProperty (Drawing.Color.FromName ("Black")));
			properties_2.Add (new Properties.MarginsProperty (60, 60, 0, 0, Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			
			properties_3.Add (font_italic);
			properties_3.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties_3.Add (new Properties.ColorProperty (Drawing.Color.FromName ("Black")));
			properties_3.Add (new Properties.MarginsProperty (0, 0, 0, 0, Properties.SizeUnits.Points, 0.0, 0.0, 1.0, 15, 1, Properties.ThreeState.False));
			
			story.ConvertToStyledText ("Text:\t", properties_1, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("Tout un paragraphe indenté (comme si le tabulateur se comportait comme un indentateur).\n", properties_2, out text);
			story.InsertText (cursor, text);
			
			Text.TextStyle default_style = story.StyleList.NewTextStyle ("Default", Text.TextStyleClass.Paragraph, properties_3);
			
			story.TextContext.DefaultStyle = default_style;
			
			story.ConvertToStyledText ("fin\n", default_style, out text);
			story.InsertText (cursor, text);
			
			/*
			 *	Text:---->Tout un		|
			 *			  paragraphe	|
			 *			  indenté		|
			 *			  (comme si le	|
			 *			  tabulateur se |
			 *			  comportait	|
			 *			  comme un		|
			 *			  indentateur). |
			 *							|
			 *                       fin|
			 *							|
			 *	                   [vide]
			 */
			
			story.SetCursorPosition (cursor, 0);
			
			TextFitter      fitter = new TextFitter (story);
			SimpleTextFrame frame  = new SimpleTextFrame (150, 600);
			frame.PageNumber = 0;
			fitter.FrameList.InsertAt (0, frame);
			
			ITextFrame c_frame;
			double cx, cy;
			int c_line, c_char;
			
			fitter.GenerateAllMarks ();
			fitter.GetCursorGeometry (cursor, out c_frame, out cx, out cy, out c_line, out c_char);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Cursor at {0:0.00}:{1:0.00}, line {2}, column {3}", cx, cy, c_line, c_char));
			
			Debug.Assert.IsTrue (c_frame == frame);
			Debug.Assert.IsTrue (c_line == 0);
			Debug.Assert.IsTrue (c_char == 0);
			
			story.SetCursorPosition (cursor, 6, 1);
			fitter.GetCursorGeometry (cursor, out c_frame, out cx, out cy, out c_line, out c_char);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Cursor at {0:0.00}:{1:0.00}, line {2}, column {3}", cx, cy, c_line, c_char));
			
			Debug.Assert.IsTrue (c_frame == frame);
			Debug.Assert.IsTrue (c_line == 0);
			Debug.Assert.IsTrue (c_char == 6);
			
			story.SetCursorPosition (cursor, 14, 1);
			fitter.GetCursorGeometry (cursor, out c_frame, out cx, out cy, out c_line, out c_char);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Cursor at {0:0.00}:{1:0.00}, line {2}, column {3}", cx, cy, c_line, c_char));
			
			Debug.Assert.IsTrue (c_frame == frame);
			Debug.Assert.IsTrue (c_line == 0);
			Debug.Assert.IsTrue (c_char == 14);
			
			story.SetCursorPosition (cursor, 15, 1);
			fitter.GetCursorGeometry (cursor, out c_frame, out cx, out cy, out c_line, out c_char);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Cursor at {0:0.00}:{1:0.00}, line {2}, column {3}", cx, cy, c_line, c_char));
			
			Debug.Assert.IsTrue (c_frame == frame);
			Debug.Assert.IsTrue (c_line == 1);
			Debug.Assert.IsTrue (c_char == 1);
			
			story.SetCursorPosition (cursor, 14, -1);
			fitter.GetCursorGeometry (cursor, out c_frame, out cx, out cy, out c_line, out c_char);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Cursor at {0:0.00}:{1:0.00}, line {2}, column {3}", cx, cy, c_line, c_char));
			
			Debug.Assert.IsTrue (c_frame == frame);
			Debug.Assert.IsTrue (c_line == 1);
			Debug.Assert.IsTrue (c_char == 0);
			
			story.SetCursorPosition (cursor, 97, 1);
			fitter.GetCursorGeometry (cursor, out c_frame, out cx, out cy, out c_line, out c_char);
			
			Debug.Assert.IsTrue (c_frame == frame);
			Debug.Assert.IsTrue (c_line == 0);
			Debug.Assert.IsTrue (c_char == 3);
			Debug.Assert.IsTrue (cx == 150.0);
			
			story.SetCursorPosition (cursor, 98, 1);
			fitter.GetCursorGeometry (cursor, out c_frame, out cx, out cy, out c_line, out c_char);
			System.Diagnostics.Debug.WriteLine (string.Format ("Cursor at {0:0.00}:{1:0.00}, line {2}, column {3}", cx, cy, c_line, c_char));
			
			story.SetCursorPosition (cursor, 98, -1);
			fitter.GetCursorGeometry (cursor, out c_frame, out cx, out cy, out c_line, out c_char);
			System.Diagnostics.Debug.WriteLine (string.Format ("Cursor at {0:0.00}:{1:0.00}, line {2}, column {3}", cx, cy, c_line, c_char));
			
			Debug.Assert.IsTrue (c_line == 0);
			Debug.Assert.IsTrue (c_char == 0);
			
			TextNavigator navigator = new TextNavigator (story, fitter);
			
			double ascender;
			double descender;
			double angle;
			
			
			navigator.MoveTo (TextNavigator.Target.TextEnd, 0);
			navigator.GetCursorGeometry (out c_frame, out cx, out cy, out ascender, out descender, out angle);
			
			Debug.Assert.IsInBounds (ascender,  10.863, 10.864);
			Debug.Assert.IsInBounds (descender, -2.543, -2.542);
			Debug.Assert.IsInBounds (angle,      1.361,  1.362);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Cursor at {0:0.00}:{1:0.00}, ascender={2}, descender={3}, angle={4}", cx, cy, ascender, descender, angle));
		}
		
		
		#region Renderer Classes
		private class Renderer : Text.ITextRenderer
		{
			public Renderer()
			{
				this.buffer = new System.Text.StringBuilder ();
				this.count  = 0;
			}
			
			
			public void NewParagraph()
			{
				System.Console.Out.WriteLine (this.buffer.ToString ());
				System.Console.Out.WriteLine ();
				
				this.buffer.Length = 0;
				this.count = 0;
			}
			
			
			#region ITextRenderer Members
			public bool IsFrameAreaVisible(ITextFrame frame, double x, double y, double width, double height)
			{
				return true;
			}
			
			public void RenderStartParagraph(Layout.Context context)
			{
			}
			
			public void RenderStartLine(Layout.Context context)
			{
			}
			
			public void Render(Layout.Context layout, Epsitec.Common.OpenType.Font font, double size, Drawing.Color color, Layout.TextToGlyphMapping mapping, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy, bool is_last_run)
			{
				ITextFrame frame = layout.Frame;
				
				for (int i = 0; i < glyphs.Length; i++)
				{
					if (this.count > 0)
					{
						buffer.Append (" ");
						if ((this.count % 4) == 0)
						{
							buffer.Append ("\n");
						}
					}
					
					buffer.AppendFormat ("{0:000}", glyphs[i]);
					buffer.Append ("-");
					buffer.AppendFormat ("{0:000.00}", x[i]);
					buffer.Append (":");
					buffer.AppendFormat ("{0:00.00}", sx[i]);
					
					this.count++;
					
					if (this.count == 16)
					{
						System.Console.Out.WriteLine (buffer.ToString ());
						
						this.count         = 0;
						this.buffer.Length = 0;
					}
				}
			}
			
			public void Render(Layout.Context layout, IGlyphRenderer glyph_renderer, Drawing.Color color, double x, double y, bool is_last_run)
			{
			}
			
			public void RenderEndLine(Layout.Context context)
			{
			}
			
			public void RenderEndParagraph(Layout.Context context)
			{
			}
			#endregion

			private System.Text.StringBuilder	buffer;
			private int							count;
		}

		private class ZeroRenderer : Text.ITextRenderer
		{
			public ZeroRenderer()
			{
			}
			
			
			public void NewParagraph()
			{
			}
			
			
			#region ITextRenderer Members
			public bool IsFrameAreaVisible(ITextFrame frame, double x, double y, double width, double height)
			{
				return true;
			}
			
			public void RenderStartParagraph(Layout.Context context)
			{
			}
			
			public void RenderStartLine(Layout.Context context)
			{
			}
			
			public void Render(Layout.Context layout, Epsitec.Common.OpenType.Font font, double size, Drawing.Color color, Layout.TextToGlyphMapping mapping, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy, bool is_last_run)
			{
			}
			
			public void Render(Layout.Context layout, IGlyphRenderer glyph_renderer, Drawing.Color color, double x, double y, bool is_last_run)
			{
			}
			
			public void RenderEndLine(Layout.Context context)
			{
			}
			
			public void RenderEndParagraph(Layout.Context context)
			{
			}
			#endregion
		}
		#endregion
	}
}
