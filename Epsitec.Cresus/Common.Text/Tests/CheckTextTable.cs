//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Tests
{
	/// <summary>
	/// Vérifie le bon fonctionnement de la classe TextTable.
	/// </summary>
	public sealed class CheckTextTable
	{
		public static void RunTests()
		{
			Internal.TextTable table = new Internal.TextTable ();
			
			int cursor_1 = table.NewCursor ();
			int cursor_2 = table.NewCursor ();
			
			table.InsertText (cursor_1, new ulong[] { 65ul, 66ul, 67ul });
			
			int cursor_3 = table.NewCursor ();
			
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor_1) == 3);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor_2) == 3);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor_3) == 0);
			
			ulong[] text = new ulong[6000];
			
			Debug.Assert.IsTrue (table.ReadText (cursor_1, 10, ref text) == 0);
			Debug.Assert.IsTrue (table.ReadText (cursor_2, 10, ref text) == 0);
			Debug.Assert.IsTrue (table.ReadText (cursor_3, 10, ref text) == 3);
			
			Debug.Assert.IsTrue (text[0] == 65ul);
			Debug.Assert.IsTrue (text[1] == 66ul);
			Debug.Assert.IsTrue (text[2] == 67ul);
			
			table.InsertText (cursor_3, new ulong[] { 34ul });
			table.InsertText (cursor_2, new ulong[] { 34ul });
			
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor_1) == 5);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor_2) == 5);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor_3) == 1);
			
			Debug.Assert.IsTrue (table.ReadText (cursor_1, 10, ref text) == 0);
			Debug.Assert.IsTrue (table.ReadText (cursor_2, 10, ref text) == 0);
			Debug.Assert.IsTrue (table.ReadText (cursor_3, 10, ref text) == 4);
			
			Debug.Assert.IsTrue (text[0] == 65ul);
			Debug.Assert.IsTrue (text[1] == 66ul);
			Debug.Assert.IsTrue (text[2] == 67ul);
			Debug.Assert.IsTrue (text[3] == 34ul);
			
			Debug.Assert.IsTrue (table.MoveCursor (cursor_3, 2) == 2);
			Debug.Assert.IsTrue (table.MoveCursor (cursor_2, -10) == -5);
			
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor_1) == 5);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor_2) == 0);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor_3) == 3);
			
			Debug.Assert.IsTrue (table.ReadText (cursor_1, 10, ref text) == 0);
			Debug.Assert.IsTrue (table.ReadText (cursor_2, 10, ref text) == 5);
			
			Debug.Assert.IsTrue (text[0] == 34ul);
			Debug.Assert.IsTrue (text[1] == 65ul);
			Debug.Assert.IsTrue (text[2] == 66ul);
			Debug.Assert.IsTrue (text[3] == 67ul);
			Debug.Assert.IsTrue (text[4] == 34ul);
			
			Debug.Assert.IsTrue (table.ReadText (cursor_3, 10, ref text) == 2);
			
			Debug.Assert.IsTrue (text[0] == 67ul);
			Debug.Assert.IsTrue (text[1] == 34ul);
			
			
			
			Internal.TextTable table1 = new Internal.TextTable ();
			Internal.TextTable table2 = new Internal.TextTable ();
			Internal.TextTable table3 = new Internal.TextTable ();
			
			int cursor_id = table1.NewCursor ();
			
			string  sample = "ceci est un bref exemple de texte français permettant de générer des fréquences " +
				/**/         "correctes au niveau de l'utilisation des divers caractères (si l'on fait, bien " +
				/**/		 "sûr abstraction de l'ordre dans lequel les lettres apparaissent, et que l'on " +
				/**/		 "oublie les lettres majuscules).";
			
			int size = 1*1024*1024;
			
			System.Random random = new System.Random (1);
			
			System.Diagnostics.Trace.WriteLine (string.Format ("Generating {0}MB of text / {1} million signs.", 8*size / (1024*1024), size/1000000M));
			
			while (table1.TextLength < size)
			{
				for (int i = 0; i < text.Length; i += 4)
				{
					int p = random.Next (0, sample.Length-4);
					
					text[i+0] = sample[p+0];
					text[i+1] = sample[p+1];
					text[i+2] = sample[p+2];
					text[i+3] = sample[p+3];
				}
			
				table1.InsertText (cursor_id, text);
			}
			
			System.Diagnostics.Trace.WriteLine ("Saving file.");
			using (System.IO.FileStream file = new System.IO.FileStream (@"c:\test.text", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
			{
				table1.WriteRawText (file);
			}
			System.Diagnostics.Trace.WriteLine ("Saving file, done.");
			
			
			System.Diagnostics.Trace.WriteLine ("Loading file.");
			using (System.IO.FileStream file = new System.IO.FileStream (@"c:\test.text", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				table2.ReadRawText (file);
			}
			System.Diagnostics.Trace.WriteLine ("Loading file, done.");
			
			Debug.Assert.IsTrue (table1.TextLength == table2.TextLength);
			
			
			System.Diagnostics.Trace.WriteLine ("Saving compressed file.");
			using (System.IO.FileStream file = new System.IO.FileStream (@"c:\test.compressed", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
			{
				using (System.IO.Stream stream = Common.IO.Compression.CreateDeflateStream (file, 3))
				{
					table1.WriteRawText (stream);
					stream.Flush ();
					file.SetLength (file.Position);
				}
			}
			System.Diagnostics.Trace.WriteLine ("Saving compressed file, done.");
			
			System.Diagnostics.Trace.WriteLine ("Loading compressed file.");
			using (System.IO.FileStream file = new System.IO.FileStream (@"c:\test.compressed", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				using (System.IO.Stream stream = Common.IO.Decompression.CreateStream (file))
				{
					table3.ReadRawText (stream);
				}
			}
			System.Diagnostics.Trace.WriteLine ("Loading compressed file, done.");
			
			
			/*
			 *	Mesures sur bi-Xeon 1.7GHz :
			 *
			 *	Traversée --> 810ns / caractère
			 *	Traversée <-- 750ns / caractère
			 */
			
			Internal.CursorId cursor_3_1 = table3.NewCursor ();
			Internal.CursorId cursor_3_2 = table3.NewCursor ();
			
			System.Diagnostics.Trace.WriteLine ("Starting traversal, 1 char. at a time -->");
			while (table3.MoveCursor (cursor_3_1, 1) > 0)
			{
			}
			System.Diagnostics.Trace.WriteLine ("Done.");
			
			System.Diagnostics.Trace.WriteLine ("Starting traversal, 10'000 char. at a time >>>");
			while (table3.MoveCursor (cursor_3_2, 10000) > 0)
			{
			}
			System.Diagnostics.Trace.WriteLine ("Done.");
			
			Debug.Assert.IsTrue (table3.TextLength == table3.GetCursorPosition (cursor_3_1));
			Debug.Assert.IsTrue (table3.TextLength == table3.GetCursorPosition (cursor_3_2));
			
			System.Diagnostics.Trace.WriteLine ("Starting traversal, 1 char. at a time <--");
			while (table3.MoveCursor (cursor_3_1, -1) < 0)
			{
			}
			System.Diagnostics.Trace.WriteLine ("Done.");
			
			System.Diagnostics.Trace.WriteLine ("Starting traversal, 10'000 char. at a time <<<");
			while (table3.MoveCursor (cursor_3_2, -10000) < 0)
			{
			}
			
			System.Diagnostics.Trace.WriteLine ("Done.");
			
			Debug.Assert.IsTrue (0 == table3.GetCursorPosition (cursor_3_1));
			Debug.Assert.IsTrue (0 == table3.GetCursorPosition (cursor_3_2));

			
			/*
			 *	Mesures sur bi-Xeon 1.7GHz :
			 *
			 *	Traversée avec lecture --> 1450ns / caractère
			 *	Traversée avec lecture <-- 1350ns / caractère
			 */
			
			ulong xxx = table3[cursor_3_1];
			ulong yyy = 0;
			
			System.Diagnostics.Trace.WriteLine ("Starting traversal + read, 1 char. at a time -->");
			while (table3.MoveCursor (cursor_3_1, 1) > 0)
			{
				xxx += table3[cursor_3_1];
			}
			System.Diagnostics.Trace.WriteLine ("Done.");
			
			System.Diagnostics.Trace.WriteLine ("Starting traversal + read, 1 char. at a time <--");
			while (table3.MoveCursor (cursor_3_1, -1) < 0)
			{
				yyy += table3[cursor_3_1];
			}
			System.Diagnostics.Trace.WriteLine ("Done.");
			
			System.Diagnostics.Trace.WriteLine ("xxx="+xxx+"\nyyy="+yyy);
			
			
#if false
			using (System.IO.FileStream input = new System.IO.FileStream (@"c:\source.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				using (System.IO.FileStream file = new System.IO.FileStream (@"c:\deflate-1.bin", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
				{
					using (System.IO.Stream stream = Common.IO.Compression.CreateDeflateStream (file, 1))
					{
						byte[] data = new byte[1000];
						for (;;)
						{
							int read = input.Read (data, 0, 1000);
							if (read == 0) break;
							stream.Write (data, 0, read);
						}
					}
				}
			}
			
			using (System.IO.FileStream input = new System.IO.FileStream (@"c:\source.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				using (System.IO.FileStream file = new System.IO.FileStream (@"c:\deflate-9.bin", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
				{
					using (System.IO.Stream stream = Common.IO.Compression.CreateDeflateStream (file, 9))
					{
						byte[] data = new byte[1000];
						for (;;)
						{
							int read = input.Read (data, 0, 1000);
							if (read == 0) break;
							stream.Write (data, 0, read);
						}
					}
				}
			}
			
			using (System.IO.FileStream input = new System.IO.FileStream (@"c:\source.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				using (System.IO.FileStream file = new System.IO.FileStream (@"c:\bzip2.bin", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
				{
					using (System.IO.Stream stream = Common.IO.Compression.CreateBZip2Stream (file))
					{
						byte[] data = new byte[1000];
						for (;;)
						{
							int read = input.Read (data, 0, 1000);
							if (read == 0) break;
							stream.Write (data, 0, read);
						}
					}
				}
			}
			
			using (System.IO.FileStream input = new System.IO.FileStream (@"c:\source.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				using (System.IO.FileStream file = new System.IO.FileStream (@"c:\gzip.bin", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
				{
					using (System.IO.Stream stream = Common.IO.Compression.CreateGZipStream (file))
					{
						byte[] data = new byte[1000];
						for (;;)
						{
							int read = input.Read (data, 0, 1000);
							if (read == 0) break;
							stream.Write (data, 0, read);
						}
					}
				}
			}
			
			using (System.IO.FileStream input = new System.IO.FileStream (@"c:\source.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				using (System.IO.FileStream file = new System.IO.FileStream (@"c:\zip.bin", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
				{
					using (System.IO.Stream stream = Common.IO.Compression.CreateZipStream (file, "Z"))
					{
						byte[] data = new byte[1000];
						for (;;)
						{
							int read = input.Read (data, 0, 1000);
							if (read == 0) break;
							stream.Write (data, 0, read);
						}
					}
				}
			}
#endif
		}
	}
}
