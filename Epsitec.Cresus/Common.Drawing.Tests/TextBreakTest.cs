using NUnit.Framework;

namespace Epsitec.Common.Drawing
{
	[TestFixture] public class TextBreakTest
	{
		[Test] public void CheckBreak()
		{
			TextBreak tb = new TextBreak ();
			
			string[] text = new string[3];
			
			text[0] = "Bonjour, ";
			text[1] = "comment";
			text[2] = " va-t-il ? C'est   juste une question.";
			
			tb.SetText (string.Concat (text), TextBreakMode.Hyphenate | TextBreakMode.Ellipsis);
			
			Font[] fonts = new Font[2];
			
			fonts[0] = Font.GetFont ("Tahoma", "Regular");
			fonts[1] = Font.GetFont ("Tahoma", "Bold");
			
			int[]    run_lengths = new int[3];
			int[]    run_fonts   = new int[3];
			double[] run_sizes   = new double[3];
			
			run_lengths[0] = text[0].Length;	run_fonts[0] = 0;	run_sizes[0] = 12.0;
			run_lengths[1] = text[1].Length;	run_fonts[1] = 1;	run_sizes[1] = 12.0;
			run_lengths[2] = text[2].Length;	run_fonts[2] = 0;	run_sizes[2] = 12.0;
			
			tb.SetFonts (fonts);
			tb.SetRuns (run_lengths, run_fonts, run_sizes);
			
			double space = 300;
			double width;
			int    n_char;
			string chunk;
			
			double last_width  = -1;
			int    last_n_char = -1;
			
			while (space > 1)
			{
				space--;
				
				tb.Rewind ();
				tb.GetNextBreak (space, out chunk, out width, out n_char);
				
				if ((n_char == last_n_char) &&
					(width  == last_width))
				{
					continue;
				}
				
				double width_2;
				int    n_char_2;
				string chunk_2;
				
				tb.GetNextBreak (1000, out chunk_2, out width_2, out n_char_2);
				
				System.Console.Out.WriteLine ("{0}, used {1} => {2} chars,\t'{3}'//'{4}'", space, (int) width, n_char, chunk, chunk_2);
				
				last_width  = width;
				last_n_char = n_char;
			}
		}
	}
}
