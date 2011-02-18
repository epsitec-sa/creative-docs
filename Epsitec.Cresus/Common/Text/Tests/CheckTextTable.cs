//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			
			int cursor1 = table.NewCursor (null);
			int cursor2 = table.NewCursor (null);
			
			table.InsertText (cursor1, new ulong[] { 65ul, 66ul, 67ul });
			
			int cursor3 = table.NewCursor (null);
			
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor1) == 3);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor1) == 3);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor2) == 3);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor3) == 0);
			
			CursorInfo[] infoX0 = table.FindCursorsBefore (5);
			CursorInfo[] infoX1 = table.FindCursorsBefore (3);
			CursorInfo[] infoX2 = table.FindCursorsBefore (2);
			CursorInfo[] infoX3 = table.FindCursorsBefore (0);
			
			Debug.Assert.IsTrue (infoX0.Length == 1);
			Debug.Assert.IsTrue (infoX1.Length == 1);
			Debug.Assert.IsTrue (infoX2.Length == 1);
			Debug.Assert.IsTrue (infoX3.Length == 0);
			
			Debug.Assert.IsTrue (infoX0[0].Position == 0);
			Debug.Assert.IsTrue (infoX0[0].CursorId == cursor3);
			
			ulong[] text = new ulong[6000];
			
			Debug.Assert.IsTrue (table.ReadText (cursor1, 10, text) == 0);
			Debug.Assert.IsTrue (table.ReadText (cursor2, 10, text) == 0);
			Debug.Assert.IsTrue (table.ReadText (cursor3, 10, text) == 3);
			
			Debug.Assert.IsTrue (text[0] == 65ul);
			Debug.Assert.IsTrue (text[1] == 66ul);
			Debug.Assert.IsTrue (text[2] == 67ul);
			
			table.InsertText (cursor3, new ulong[] { 34ul });
			table.InsertText (cursor2, new ulong[] { 34ul });
			
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor1) == 5);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor2) == 5);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor3) == 1);
			
			Debug.Assert.IsTrue (table.ReadText (cursor1, 10, text) == 0);
			Debug.Assert.IsTrue (table.ReadText (cursor2, 10, text) == 0);
			Debug.Assert.IsTrue (table.ReadText (cursor3, 10, text) == 4);
			
			Debug.Assert.IsTrue (text[0] == 65ul);
			Debug.Assert.IsTrue (text[1] == 66ul);
			Debug.Assert.IsTrue (text[2] == 67ul);
			Debug.Assert.IsTrue (text[3] == 34ul);
			
			Debug.Assert.IsTrue (table.MoveCursor (cursor3, 2) == 2);
			Debug.Assert.IsTrue (table.MoveCursor (cursor2, -10) == -5);
			
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor1) == 5);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor2) == 0);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor3) == 3);
			
			CursorInfo[] info0 = table.FindCursors (0, 100); Debug.Assert.IsTrue (info0.Length == 3); Debug.Assert.IsTrue (info0[1].Position == 3);
			CursorInfo[] info2 = table.FindCursors (2, 100); Debug.Assert.IsTrue (info2.Length == 2); Debug.Assert.IsTrue (info2[1].Position == 5);
			CursorInfo[] info3 = table.FindCursors (3, 100); Debug.Assert.IsTrue (info3.Length == 2); Debug.Assert.IsTrue (info3[1].CursorId == cursor1);
			CursorInfo[] info4 = table.FindCursors (4, 100); Debug.Assert.IsTrue (info4.Length == 1); Debug.Assert.IsTrue (info4[0].Position == 5);
			CursorInfo[] info5 = table.FindCursors (5, 100); Debug.Assert.IsTrue (info5.Length == 1); Debug.Assert.IsTrue (info5[0].CursorId == cursor1);
			CursorInfo[] info6 = table.FindCursors (6, 100); Debug.Assert.IsTrue (info6.Length == 0);
			
			Debug.Assert.IsTrue (table.ReadText (cursor1, 10, text) == 0);
			Debug.Assert.IsTrue (table.ReadText (cursor2, 10, text) == 5);
			
			Debug.Assert.IsTrue (text[0] == 34ul);
			Debug.Assert.IsTrue (text[1] == 65ul);
			Debug.Assert.IsTrue (text[2] == 66ul);
			Debug.Assert.IsTrue (text[3] == 67ul);
			Debug.Assert.IsTrue (text[4] == 34ul);
			
			Debug.Assert.IsTrue (table.ReadText (cursor3, 10, text) == 2);
			
			Debug.Assert.IsTrue (text[0] == 67ul);
			Debug.Assert.IsTrue (text[1] == 34ul);
			
			table.MoveCursor (cursor1, -1);
			table.MoveCursor (cursor2, 1);
			
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor2) == 1);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor3) == 3);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor1) == 4);
			Debug.Assert.IsTrue (table.TextLength == 5);
			
			infoX0 = table.FindNextCursor (cursor2, null);
			infoX1 = table.FindNextCursor (cursor3, null);
			infoX2 = table.FindNextCursor (cursor1, null);
			
			Debug.Assert.IsTrue (infoX0.Length == 1);
			Debug.Assert.IsTrue (infoX1.Length == 1);
			Debug.Assert.IsTrue (infoX2.Length == 0);
			Debug.Assert.IsTrue (infoX0[0].CursorId == cursor3);
			Debug.Assert.IsTrue (infoX1[0].CursorId == cursor1);
			
			infoX0 = table.FindPrevCursor (cursor2, null);
			infoX1 = table.FindPrevCursor (cursor3, null);
			infoX2 = table.FindPrevCursor (cursor1, null);
			
			Debug.Assert.IsTrue (infoX0.Length == 0);
			Debug.Assert.IsTrue (infoX1.Length == 1);
			Debug.Assert.IsTrue (infoX2.Length == 1);
			Debug.Assert.IsTrue (infoX1[0].CursorId == cursor2);
			Debug.Assert.IsTrue (infoX2[0].CursorId == cursor3);
			
			infoX0 = table.FindCursorsBefore (5);
			infoX1 = table.FindCursorsBefore (4);
			infoX2 = table.FindCursorsBefore (3);
			infoX3 = table.FindCursorsBefore (2);
			
			Debug.Assert.IsTrue (infoX0.Length == 1);
			Debug.Assert.IsTrue (infoX1.Length == 1);
			Debug.Assert.IsTrue (infoX2.Length == 1);
			Debug.Assert.IsTrue (infoX3.Length == 1);
			
			Debug.Assert.IsTrue (infoX0[0].Position == 4);
			Debug.Assert.IsTrue (infoX0[0].CursorId == cursor1);
			Debug.Assert.IsTrue (infoX1[0].Position == 3);
			Debug.Assert.IsTrue (infoX1[0].CursorId == cursor3);
			Debug.Assert.IsTrue (infoX2[0].Position == 1);
			Debug.Assert.IsTrue (infoX2[0].CursorId == cursor2);
			Debug.Assert.IsTrue (infoX3[0].Position == 1);
			Debug.Assert.IsTrue (infoX3[0].CursorId == cursor2);
			
			CursorInfo[] cInfos1;
			CursorInfo[] cInfos2;
			CursorInfo[] cInfos3;
//			CursorInfo[] cInfos4;
			
			table.DeleteText (cursor3, 2, out cInfos1);
			
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor2) == 1);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor3) == 3);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor1) == 3);
			Debug.Assert.IsTrue (table.TextLength == 3);
			Debug.Assert.IsNull (cInfos1);
//			Debug.Assert.IsNotNull (cInfos1);
//			Debug.Assert.IsTrue (cInfos1.Length == 1);
//			Debug.Assert.IsTrue (cInfos1[0].CursorId == cursor1);
			
			table.DeleteText (cursor2, 1, out cInfos2);
			
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor2) == 1);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor3) == 2);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor1) == 2);
			Debug.Assert.IsTrue (table.TextLength == 2);
			Debug.Assert.IsNull (cInfos2);
			
			infoX0 = table.FindPrevCursor (cursor2, null);
			infoX1 = table.FindPrevCursor (cursor3, null);
			infoX2 = table.FindPrevCursor (cursor1, null);
			
			Debug.Assert.IsTrue (infoX0.Length == 0);
			Debug.Assert.IsTrue (infoX1.Length == 1);
			Debug.Assert.IsTrue (infoX2.Length == 1);
			Debug.Assert.IsTrue (infoX1[0].CursorId == cursor2);
			Debug.Assert.IsTrue (infoX2[0].CursorId == cursor3);
			
			
			table.MoveCursor (cursor1, -1);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor1) == 1);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor2) == 1);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor3) == 2);
			
			infoX0 = table.FindCursorsBefore (2);
			
			Debug.Assert.IsTrue (infoX0.Length == 2);
			
			Debug.Assert.IsTrue (infoX0[0].Position == 1);
			Debug.Assert.IsTrue (infoX0[1].Position == 1);
			Debug.Assert.IsTrue (infoX0[0].CursorId == cursor1);
			Debug.Assert.IsTrue (infoX0[1].CursorId == cursor2);
			
			table.DeleteText (cursor2, 1, out cInfos3);
			
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor2) == 1);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor3) == 1);
			Debug.Assert.IsTrue (table.GetCursorPosition (cursor1) == 1);
			Debug.Assert.IsTrue (table.TextLength == 1);
			Debug.Assert.IsNull (cInfos3);
			
//			table.DeleteTextAtPosition (0, 1, out cInfos4);
//			
//			Debug.Assert.IsTrue (table.GetCursorPosition (cursor2) == 0);
//			Debug.Assert.IsTrue (table.GetCursorPosition (cursor3) == 0);
//			Debug.Assert.IsTrue (table.GetCursorPosition (cursor1) == 0);
//			Debug.Assert.IsTrue (table.TextLength == 0);
//			Debug.Assert.IsNull (cInfos4);
			
			Internal.TextTable table1 = new Internal.TextTable ();
			Internal.TextTable table2 = new Internal.TextTable ();
			Internal.TextTable table3 = new Internal.TextTable ();
			
			int cursorId = table1.NewCursor (null);
			
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
			
				table1.InsertText (cursorId, text);
			}
			
			System.Diagnostics.Trace.WriteLine ("Saving file.");
			using (System.IO.FileStream file = new System.IO.FileStream (@"S:\test.text", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
			{
				table1.WriteRawText (file);
			}
			System.Diagnostics.Trace.WriteLine ("Saving file, done.");
			
			
			System.Diagnostics.Trace.WriteLine ("Loading file.");
			using (System.IO.FileStream file = new System.IO.FileStream (@"S:\test.text", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				table2.ReadRawText (file);
			}
			System.Diagnostics.Trace.WriteLine ("Loading file, done.");
			
			Debug.Assert.IsTrue (table1.TextLength == table2.TextLength);
			
			
			System.Diagnostics.Trace.WriteLine ("Saving compressed file.");
			using (System.IO.FileStream file = new System.IO.FileStream (@"S:\test.compressed", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
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
			using (System.IO.FileStream file = new System.IO.FileStream (@"S:\test.compressed", System.IO.FileMode.Open, System.IO.FileAccess.Read))
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
			
			Internal.CursorId cursor3_1 = table3.NewCursor (null);
			Internal.CursorId cursor3_2 = table3.NewCursor (null);
			
			System.Diagnostics.Trace.WriteLine ("Starting traversal, 1 char. at a time -->");
			while (table3.MoveCursor (cursor3_1, 1) > 0)
			{
			}
			System.Diagnostics.Trace.WriteLine ("Done.");
			
			System.Diagnostics.Trace.WriteLine ("Starting traversal, 10'000 char. at a time >>>");
			while (table3.MoveCursor (cursor3_2, 10000) > 0)
			{
			}
			System.Diagnostics.Trace.WriteLine ("Done.");
			
			Debug.Assert.IsTrue (table3.TextLength == table3.GetCursorPosition (cursor3_1));
			Debug.Assert.IsTrue (table3.TextLength == table3.GetCursorPosition (cursor3_2));
			
			System.Diagnostics.Trace.WriteLine ("Starting traversal, 1 char. at a time <--");
			while (table3.MoveCursor (cursor3_1, -1) < 0)
			{
			}
			System.Diagnostics.Trace.WriteLine ("Done.");
			
			System.Diagnostics.Trace.WriteLine ("Starting traversal, 10'000 char. at a time <<<");
			while (table3.MoveCursor (cursor3_2, -10000) < 0)
			{
			}
			
			System.Diagnostics.Trace.WriteLine ("Done.");
			
			Debug.Assert.IsTrue (0 == table3.GetCursorPosition (cursor3_1));
			Debug.Assert.IsTrue (0 == table3.GetCursorPosition (cursor3_2));

			
			/*
			 *	Mesures sur bi-Xeon 1.7GHz :
			 *
			 *	Traversée avec lecture --> 1450ns / caractère
			 *	Traversée avec lecture <-- 1350ns / caractère
			 */
			
			ulong xxx = table3[cursor3_1];
			ulong yyy = 0;
			
			System.Diagnostics.Trace.WriteLine ("Starting traversal + read, 1 char. at a time -->");
			while (table3.MoveCursor (cursor3_1, 1) > 0)
			{
				xxx += table3[cursor3_1];
			}
			System.Diagnostics.Trace.WriteLine ("Done.");
			
			System.Diagnostics.Trace.WriteLine ("Starting traversal + read, 1 char. at a time <--");
			while (table3.MoveCursor (cursor3_1, -1) < 0)
			{
				yyy += table3[cursor3_1];
			}
			System.Diagnostics.Trace.WriteLine ("Done.");
			
			System.Diagnostics.Trace.WriteLine ("xxx="+xxx+"\nyyy="+yyy);
			
			/*
			 *  Vérifie que les curseurs peuvent aussi être retrouvés dans un
			 *	texte utilisant plusieurs morceaux.
			 */
			
			table3.MoveCursor (cursor3_1, 8000);
			table3.MoveCursor (cursor3_2, 25000);
			
			CursorInfo[] info3A = table3.FindCursors (10, 100);
			CursorInfo[] info3B = table3.FindCursors (10, 10000);
			CursorInfo[] info3C = table3.FindCursors (10000, 30000);
			CursorInfo[] info3D = table3.FindCursors (1000, 30000);
			
			Debug.Assert.IsTrue (info3A.Length == 0);
			Debug.Assert.IsTrue (info3B.Length == 1); Debug.Assert.IsTrue (info3B[0].Position == 8000);
			Debug.Assert.IsTrue (info3C.Length == 1); Debug.Assert.IsTrue (info3C[0].Position == 25000);
			Debug.Assert.IsTrue (info3D.Length == 2); Debug.Assert.IsTrue (info3D[0].Position == 8000);  Debug.Assert.IsTrue (info3D[1].Position == 25000);
#if false
			using (System.IO.FileStream input = new System.IO.FileStream (@"S:\source.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				using (System.IO.FileStream file = new System.IO.FileStream (@"S:\deflate-1.bin", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
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
			
			using (System.IO.FileStream input = new System.IO.FileStream (@"S:\source.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				using (System.IO.FileStream file = new System.IO.FileStream (@"S:\deflate-9.bin", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
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
			
			using (System.IO.FileStream input = new System.IO.FileStream (@"S:\source.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				using (System.IO.FileStream file = new System.IO.FileStream (@"S:\bzip2.bin", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
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
			
			using (System.IO.FileStream input = new System.IO.FileStream (@"S:\source.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				using (System.IO.FileStream file = new System.IO.FileStream (@"S:\gzip.bin", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
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
			
			using (System.IO.FileStream input = new System.IO.FileStream (@"S:\source.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				using (System.IO.FileStream file = new System.IO.FileStream (@"S:\zip.bin", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
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
