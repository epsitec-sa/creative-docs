//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			CheckTextFitter.TestSimpleTextFrame ();
			CheckTextFitter.TestFit ();
			CheckTextFitter.TestFitTabs1 ();
			CheckTextFitter.TestFitTabs2 ();
			CheckTextFitter.TestFitTabs3 ();
			CheckTextFitter.TestFitTabs4 ();
			CheckTextFitter.TestTabs ();
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
			properties.Add (new Properties.FontColorProperty ("Black"));
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
			
			for (int i = 0; i < 330; i++)
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
			
			System.IO.StreamWriter writer = new System.IO.StreamWriter (@"S:\text.txt", false, System.Text.Encoding.UTF8);
			story.MoveCursor (cursor, -story.TextLength);
			
			int lastPage = 0;
			
			foreach (CursorInfo info in infos)
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				Cursors.FitterCursor fitterCursor = story.TextTable.GetCursorInstance (info.CursorId) as Cursors.FitterCursor;
				
				buffer.AppendFormat ("{0}:", story.GetCursorPosition (fitterCursor));
				
				int pos = 0;
				
				foreach (Cursors.FitterCursor.Element element in fitterCursor.Elements)
				{
					int    length = element.Length;
					string textStr;
					
					text = new ulong[length];
					
					story.ReadText (cursor, length, text);
					story.MoveCursor (cursor, length);
					
					if (lastPage != fitter.FrameList[element.FrameIndex].PageNumber)
					{
						lastPage = fitter.FrameList[element.FrameIndex].PageNumber;
						writer.WriteLine ("_{0}___________________________________________________________________\r\n", lastPage);
					}
					
					TextConverter.ConvertToString (text, out textStr);
					writer.WriteLine (textStr.Replace ("\n", "<\r\n"));
					
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
				Cursors.FitterCursor fitterCursor = story.TextTable.GetCursorInstance (info.CursorId) as Cursors.FitterCursor;
				fitter.RenderParagraph (fitterCursor, renderer);
				renderer.NewParagraph ();
			}
			System.Diagnostics.Trace.WriteLine ("Done.");
			
			foreach (CursorInfo info in infos)
			{
				Cursors.FitterCursor fitterCursor = story.TextTable.GetCursorInstance (info.CursorId) as Cursors.FitterCursor;
				
				if (fitterCursor.Elements.Length == 5)
				{
					System.Diagnostics.Trace.WriteLine ("Fitter: render (2) -- single paragraph x 1000");
					for (int i = 0; i < 1000; i++)
					{
						fitter.RenderParagraph (fitterCursor, renderer);
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
			
			System.Collections.ArrayList properties1 = new System.Collections.ArrayList ();
			System.Collections.ArrayList properties2 = new System.Collections.ArrayList ();
			
			TabList tabs = story.TextContext.TabList;
			
			properties1.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties1.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties1.Add (new Properties.FontColorProperty ("Black"));
			properties1.Add (new Properties.MarginsProperty (0, 0, 0, 0, Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			properties1.Add (tabs.NewTab ("T1", 60, Properties.SizeUnits.Points, 0.0, null, TabPositionMode.Absolute));
			
			properties2.Add (new Properties.FontProperty ("Arial", "Bold"));
			properties2.Add (new Properties.FontSizeProperty (12.5, Properties.SizeUnits.Points));
			properties2.Add (new Properties.FontColorProperty ("Black"));
			properties2.Add (new Properties.MarginsProperty (0, 0, 0, 0, Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			
			story.ConvertToStyledText ("Text:\t", properties1, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("T", properties2, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("1\nXyz blablabla blablabla blablah blah.\tT2 et du texte pour la suite...\n", properties1, out text);
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
				Cursors.FitterCursor fitterCursor = story.TextTable.GetCursorInstance (info.CursorId) as Cursors.FitterCursor;
				
				System.Console.Out.WriteLine ("{0}:", story.GetCursorPosition (fitterCursor));
				
				foreach (Cursors.FitterCursor.Element elem in fitterCursor.Elements)
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
			
			System.Collections.ArrayList properties1 = new System.Collections.ArrayList ();
			System.Collections.ArrayList properties2 = new System.Collections.ArrayList ();
			
			TabList tabs = story.TextContext.TabList;
			
			properties1.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties1.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties1.Add (new Properties.FontColorProperty ("Black"));
			properties1.Add (new Properties.MarginsProperty (0, 0, 0, 0, Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			properties1.Add (tabs.NewTab ("T1", 60, Properties.SizeUnits.Points, 0.5, null, TabPositionMode.Absolute));
			
			properties2.Add (new Properties.FontProperty ("Arial", "Bold"));
			properties2.Add (new Properties.FontSizeProperty (12.5, Properties.SizeUnits.Points));
			properties2.Add (new Properties.FontColorProperty ("Black"));
			properties2.Add (new Properties.MarginsProperty (0, 0, 0, 0, Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			
			story.ConvertToStyledText ("Text:\t", properties1, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("T", properties2, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("1\nXyz blablabla blablabla blablah blah.\tT2, texte centré\n", properties1, out text);
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
				Cursors.FitterCursor fitterCursor = story.TextTable.GetCursorInstance (info.CursorId) as Cursors.FitterCursor;
				
				System.Console.Out.WriteLine ("{0}:", story.GetCursorPosition (fitterCursor));
				
				foreach (Cursors.FitterCursor.Element elem in fitterCursor.Elements)
				{
					System.Console.Out.WriteLine ("    [{0:0.00}:{1:0.00}], width={4:0.00}/{2:0.00}, length={3}", elem.LineStartX, elem.LineBaseY, elem.LineWidth, elem.Length, elem.Profile.TotalWidth);
				}
			}
			
			ZeroRenderer renderer = new ZeroRenderer ();
			
			foreach (CursorInfo info in infos)
			{
				Cursors.FitterCursor fitterCursor = story.TextTable.GetCursorInstance (info.CursorId) as Cursors.FitterCursor;
				fitter.RenderParagraph (fitterCursor, renderer);
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
			
			System.Collections.ArrayList properties1 = new System.Collections.ArrayList ();
			System.Collections.ArrayList properties2 = new System.Collections.ArrayList ();
			
			TabList tabs = story.TextContext.TabList;
			
			properties1.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties1.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties1.Add (new Properties.FontColorProperty ("Black"));
			properties1.Add (new Properties.MarginsProperty (0, 0, 0, 0, Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			properties1.Add (tabs.NewTab ("T1", 60, Properties.SizeUnits.Points, 0.0, null, TabPositionMode.Absolute));
			
			properties2.Add (new Properties.FontProperty ("Arial", "Bold"));
			properties2.Add (new Properties.FontSizeProperty (12.5, Properties.SizeUnits.Points));
			properties2.Add (new Properties.FontColorProperty ("Black"));
			properties2.Add (new Properties.MarginsProperty (0, 0, 0, 0, Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			
			story.ConvertToStyledText ("Text:\t", properties1, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("T", properties2, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("1\nXyz blablabla blablabla blablah blah.\tT2 et du texte pour la suite...\n", properties1, out text);
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
				Cursors.FitterCursor fitterCursor = story.TextTable.GetCursorInstance (info.CursorId) as Cursors.FitterCursor;
				
				System.Console.Out.WriteLine ("{0}:", story.GetCursorPosition (fitterCursor));
				
				foreach (Cursors.FitterCursor.Element elem in fitterCursor.Elements)
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
			
			System.Collections.ArrayList properties1 = new System.Collections.ArrayList ();
			System.Collections.ArrayList properties2 = new System.Collections.ArrayList ();
			
			TabList tabs = story.TextContext.TabList;
			
			properties1.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties1.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties1.Add (new Properties.FontColorProperty ("Black"));
			properties1.Add (new Properties.MarginsProperty (0, 0, 0, 0, Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			properties1.Add (tabs.NewTab ("T1", 60, Properties.SizeUnits.Points, 0.0, null, TabPositionMode.Absolute));
			
			properties2.Add (new Properties.FontProperty ("Arial", "Bold"));
			properties2.Add (new Properties.FontSizeProperty (12.5, Properties.SizeUnits.Points));
			properties2.Add (new Properties.FontColorProperty ("Black"));
			properties2.Add (new Properties.MarginsProperty (60, 60, 0, 0, Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			
			story.ConvertToStyledText ("Text:\t", properties1, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("Tout un paragraphe indenté (comme si le tabulateur se comportait comme un indentateur).\n", properties2, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("Xyz\n", properties1, out text);
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
				Cursors.FitterCursor fitterCursor = story.TextTable.GetCursorInstance (info.CursorId) as Cursors.FitterCursor;
				
				System.Console.Out.WriteLine ("{0}:", story.GetCursorPosition (fitterCursor));
				
				foreach (Cursors.FitterCursor.Element elem in fitterCursor.Elements)
				{
					System.Console.Out.WriteLine ("    [{0:0.00}:{1:0.00}], width={4:0.00}/{2:0.00}, length={3}", elem.LineStartX, elem.LineBaseY, elem.LineWidth, elem.Length, elem.Profile.TotalWidth);
				}
			}
		}
		
		private static void TestTabs()
		{
			TextStory story = new TextStory ();
			TabList   tabs  = story.TextContext.TabList;
			
			string p1 = TabList.PackToAttribute ();
			string p2 = TabList.PackToAttribute ("a", "b;/\\", "c");
			
			string[] u1 = TabList.UnpackFromAttribute (p1);
			string[] u2 = TabList.UnpackFromAttribute (p2);
			
			Debug.Assert.IsTrue (u1.Length == 0);
			Debug.Assert.IsTrue (u2.Length == 3);
			Debug.Assert.IsTrue (u2[0] == "a");
			Debug.Assert.IsTrue (u2[1] == "b;/\\");
			Debug.Assert.IsTrue (u2[2] == "c");
			
			string a1 = TabList.PackToAttribute ("Dummy", "LevelMultiplier:10 mm");
			string a2 = TabList.PackToAttribute ("Dummy", "LevelTable:10 mm;50 pt;2.54 in");
			
			double v1_0 = TabList.GetLevelOffset (1.0, 0, a1);
			double v1_1 = TabList.GetLevelOffset (1.0, 1, a1);
			double v1_2 = TabList.GetLevelOffset (1.0, 2, a1);
			double v1_3 = TabList.GetLevelOffset (1.0, 3, a1);
			
			Debug.Assert.Equals (0,   (int)(v1_0*10));
			Debug.Assert.Equals (283, (int)(v1_1*10));
			Debug.Assert.Equals (566, (int)(v1_2*10));
			Debug.Assert.Equals (850, (int)(v1_3*10));
			
			double v2_0 = TabList.GetLevelOffset (1.0, 0, a2);
			double v2_1 = TabList.GetLevelOffset (1.0, 1, a2);
			double v2_2 = TabList.GetLevelOffset (1.0, 2, a2);
			double v2_3 = TabList.GetLevelOffset (1.0, 3, a2);
			
			Debug.Assert.Equals ( 283, (int)(v2_0*10));
			Debug.Assert.Equals ( 500, (int)(v2_1*10));
			Debug.Assert.Equals (1828, (int)(v2_2*10));
			Debug.Assert.Equals (1828, (int)(v2_3*10));
		}
		
		private static void TestCursorGeometry()
		{
			TextStory story  = new TextStory ();
			ICursor   cursor = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] text;
			
			System.Collections.ArrayList properties1 = new System.Collections.ArrayList ();
			System.Collections.ArrayList properties2 = new System.Collections.ArrayList ();
			System.Collections.ArrayList properties3 = new System.Collections.ArrayList ();
			
			TabList tabs = story.TextContext.TabList;
			
			Properties.FontProperty fontRegular = new Properties.FontProperty ("Arial", "Regular", "liga");
			Properties.FontProperty fontBold    = new Properties.FontProperty ("Arial", "Bold");
			Properties.FontProperty fontItalic  = new Properties.FontProperty ("Arial", "Italic");
			
			properties1.Add (fontRegular);
			properties1.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties1.Add (new Properties.FontColorProperty ("Black"));
			properties1.Add (new Properties.MarginsProperty (0, 0, 0, 0, Properties.SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			
			properties2.Add (fontBold);
			properties2.Add (new Properties.FontSizeProperty (12.5, Properties.SizeUnits.Points));
			properties2.Add (new Properties.FontColorProperty ("Black"));
			properties2.Add (new Properties.MarginsProperty (60, 60, 0, 0, Properties.SizeUnits.Points, 1.0, 0.0, 0.0, 15, 1, Properties.ThreeState.False));
			
			properties3.Add (fontItalic);
			properties3.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties3.Add (new Properties.FontColorProperty ("Black"));
			properties3.Add (new Properties.MarginsProperty (0, 0, 0, 0, Properties.SizeUnits.Points, 0.0, 0.0, 1.0, 15, 1, Properties.ThreeState.False));
			
			story.ConvertToStyledText ("Text:", properties1, out text);
			story.InsertText (cursor, text);
			
			properties1.Add (tabs.NewTab ("T1", 60, Properties.SizeUnits.Points, 0.0, null, TabPositionMode.Absolute));
			story.ConvertToStyledText ("\t", properties1, out text);
			story.InsertText (cursor, text);
			
			story.ConvertToStyledText ("Tout un paragraphe indenté (comme si le tabulateur se comportait comme un indentateur).\n", properties2, out text);
			story.InsertText (cursor, text);
			
			Text.TextStyle defaultStyle = story.StyleList.NewTextStyle (null, "Default", Text.TextStyleClass.Paragraph, properties3);
			
			story.TextContext.DefaultParagraphStyle = defaultStyle;
			
			story.ConvertToStyledText ("fin\n", defaultStyle, out text);
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
			
			ITextFrame cFrame;
			double cx, cy;
			int cLine, cChar;
			
			fitter.GenerateAllMarks ();
			fitter.GetCursorGeometry (cursor, out cFrame, out cx, out cy, out cLine, out cChar);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Cursor at {0:0.00}:{1:0.00}, line {2}, column {3}", cx, cy, cLine, cChar));
			
			Debug.Assert.IsTrue (cFrame == frame);
			Debug.Assert.IsTrue (cLine == 0);
			Debug.Assert.IsTrue (cChar == 0);
			
			story.SetCursorPosition (cursor, 6, 1);
			fitter.GetCursorGeometry (cursor, out cFrame, out cx, out cy, out cLine, out cChar);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Cursor at {0:0.00}:{1:0.00}, line {2}, column {3}", cx, cy, cLine, cChar));
			
			Debug.Assert.IsTrue (cFrame == frame);
			Debug.Assert.IsTrue (cLine == 0);
			Debug.Assert.IsTrue (cChar == 6);
			
			story.SetCursorPosition (cursor, 14, 1);
			fitter.GetCursorGeometry (cursor, out cFrame, out cx, out cy, out cLine, out cChar);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Cursor at {0:0.00}:{1:0.00}, line {2}, column {3}", cx, cy, cLine, cChar));
			
			Debug.Assert.IsTrue (cFrame == frame);
			Debug.Assert.IsTrue (cLine == 0);
			Debug.Assert.IsTrue (cChar == 14);
			
			story.SetCursorPosition (cursor, 15, 1);
			fitter.GetCursorGeometry (cursor, out cFrame, out cx, out cy, out cLine, out cChar);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Cursor at {0:0.00}:{1:0.00}, line {2}, column {3}", cx, cy, cLine, cChar));
			
			Debug.Assert.IsTrue (cFrame == frame);
			Debug.Assert.IsTrue (cLine == 1);
			Debug.Assert.IsTrue (cChar == 1);
			
			story.SetCursorPosition (cursor, 14, -1);
			fitter.GetCursorGeometry (cursor, out cFrame, out cx, out cy, out cLine, out cChar);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Cursor at {0:0.00}:{1:0.00}, line {2}, column {3}", cx, cy, cLine, cChar));
			
			Debug.Assert.IsTrue (cFrame == frame);
			Debug.Assert.IsTrue (cLine == 1);
			Debug.Assert.IsTrue (cChar == 0);
			
			story.SetCursorPosition (cursor, 97, 1);
			fitter.GetCursorGeometry (cursor, out cFrame, out cx, out cy, out cLine, out cChar);
			
			Debug.Assert.IsTrue (cFrame == frame);
			Debug.Assert.IsTrue (cLine == 0);
			Debug.Assert.IsTrue (cChar == 3);
			Debug.Assert.IsTrue (cx == 150.0);
			
			story.SetCursorPosition (cursor, 98, 1);
			fitter.GetCursorGeometry (cursor, out cFrame, out cx, out cy, out cLine, out cChar);
			System.Diagnostics.Debug.WriteLine (string.Format ("Cursor at {0:0.00}:{1:0.00}, line {2}, column {3}", cx, cy, cLine, cChar));
			
			story.SetCursorPosition (cursor, 98, -1);
			fitter.GetCursorGeometry (cursor, out cFrame, out cx, out cy, out cLine, out cChar);
			System.Diagnostics.Debug.WriteLine (string.Format ("Cursor at {0:0.00}:{1:0.00}, line {2}, column {3}", cx, cy, cLine, cChar));
			
			Debug.Assert.IsTrue (cLine == 0);
			Debug.Assert.IsTrue (cChar == 0);
			
			TextNavigator navigator = new TextNavigator (fitter);
			
			double ascender;
			double descender;
			double angle;
			
			
			navigator.MoveTo (TextNavigator.Target.TextEnd, 0);
			navigator.GetCursorGeometry (out cFrame, out cx, out cy, out ascender, out descender, out angle);
			
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
			
			public void RenderTab(Layout.Context layout, string tag, double tabOrigin, double tabStop, ulong tabCode, bool isTabDefined, bool isTabAuto)
			{
			}
			
			public void Render(Layout.Context layout, Epsitec.Common.OpenType.Font font, double size, string color, Layout.TextToGlyphMapping mapping, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy, bool isLastRun)
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
			
			public void Render(Layout.Context layout, IGlyphRenderer glyphRenderer, string color, double x, double y, bool isLastRun)
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
			
			public void RenderTab(Layout.Context layout, string tag, double tabOrigin, double tabStop, ulong tabCode, bool isTabDefined, bool isTabAuto)
			{
			}
			
			public void Render(Layout.Context layout, Epsitec.Common.OpenType.Font font, double size, string color, Layout.TextToGlyphMapping mapping, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy, bool isLastRun)
			{
			}

			public void Render(Layout.Context layout, IGlyphRenderer glyphRenderer, string color, double x, double y, bool isLastRun)
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
