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
			CheckTextFitter.TestFit ();
		}

		
		private static void TestFit()
		{
			TextStory story  = new TextStory ();
			ICursor   cursor = new Cursors.SimpleCursor ();
			
			story.NewCursor (cursor);
			
			ulong[] text;
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties.Add (new Properties.FontSizeProperty (24.0, Properties.FontSizeUnits.Points));
			
			string[] words = "Bonjour, ceci est un texte d'exemple permettant de vérifier le bon fonctionnement des divers algorithmes de découpe. Le nombre de mots moyen s'élève à environ 40 mots par paragraphe, ce qui correspond à des paragraphes de taille réduite.\n".Split (' ');
			System.Random random = new System.Random (0);
			
			int count = 0;
			int para  = 0;
			
			System.Diagnostics.Trace.WriteLine ("Generating random text.");
			
			while (story.TextLength < 1000000)
			{
				int    index  = random.Next (words.Length);
				string source = words[index];
				
				if (source.EndsWith (".\n"))
				{
					para++;
				}
				else
				{
					source = source + " ";
				}
				
				story.ConvertToStyledText (source, properties, out text);
				story.InsertText (cursor, text);
				
				count++;
			}
			
			System.Diagnostics.Trace.WriteLine (string.Format ("Generated {0} words, {1} paragraphs, total: {2} characters, {3:0.00} words/paragraph.", count, para, story.TextLength, 1.0*count/para));
			
			TextFitter fitter = new TextFitter (story);
			
			System.Diagnostics.Trace.WriteLine ("Fitter: generate.");
			fitter.GenerateAllMarks ();
			System.Diagnostics.Trace.WriteLine ("Done.");
			System.Diagnostics.Trace.WriteLine (string.Format ("Fitter produced {0} paragraphs.", fitter.CursorCount));
			
			CursorInfo[] infos = story.TextTable.FindCursors (0, story.TextLength, Cursors.FitterCursor.Filter);
			count = 0;
			
			System.IO.StreamWriter writer = new System.IO.StreamWriter (@"c:\text.txt", false, System.Text.Encoding.UTF8);
			story.MoveCursor (cursor, -story.TextLength);
			
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
			
			System.Diagnostics.Trace.WriteLine ("Fitter: clear.");
			fitter.ClearAllMarks ();
			System.Diagnostics.Trace.WriteLine ("Done.");
		}
	}
}
