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
			CheckTextFitter.TestSimpleTextFrame ();
			CheckTextFitter.TestFit ();
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
			
			ok = frame.ConstrainLineBox (y, asc, desc, height, out ox, out oy, out width, out y);
			
			Debug.Assert.IsTrue (ok);
			Debug.Assert.IsTrue (oy == -11);
			
			ok = frame.ConstrainLineBox (y, asc, desc, height, out ox, out oy, out width, out y);
			
			Debug.Assert.IsTrue (ok);
			Debug.Assert.IsTrue (oy == -25);
			
			ok = frame.ConstrainLineBox (y, asc, desc, height, out ox, out oy, out width, out y);
						
			Debug.Assert.IsTrue (ok);
			Debug.Assert.IsTrue (oy == -39);
			
			ok = frame.ConstrainLineBox (y, asc, desc, height, out ox, out oy, out width, out y);
			
			Debug.Assert.IsFalse (ok);
			Debug.Assert.IsTrue (oy == -53);
			
			ok = frame.ConstrainLineBox (y, asc, desc, height, out ox, out oy, out width, out y);
			
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
			properties.Add (new Properties.FontSizeProperty (24.0, Properties.FontSizeUnits.Points));
			properties.Add (new Properties.MarginsProperty (200, 0, 0, 0, 1.0, 0.0, 0.0));
			
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
			
			ZeroRenderer renderer = new ZeroRenderer ();
			
			System.Diagnostics.Trace.WriteLine ("Fitter: render.");
			foreach (CursorInfo info in infos)
			{
				Cursors.FitterCursor fitter_cursor = story.TextTable.GetCursorInstance (info.CursorId) as Cursors.FitterCursor;
//				System.Console.Out.WriteLine ("{0}:", story.GetCursorPosition (fitter_cursor));
				fitter.RenderParagraph (fitter_cursor, renderer);
				renderer.NewParagraph ();
			}
			System.Diagnostics.Trace.WriteLine ("Done.");
			
			
			System.Diagnostics.Trace.WriteLine ("Fitter: clear.");
			fitter.ClearAllMarks ();
			System.Diagnostics.Trace.WriteLine ("Done.");
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
			public void Render(Epsitec.Common.OpenType.Font font, double size, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy)
			{
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
			public void Render(Epsitec.Common.OpenType.Font font, double size, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy)
			{
			}
			#endregion
		}
		#endregion
	}
}
