/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using Epsitec.Common.Text;
using Epsitec.Common.Text.Internal;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Text
{
    /// <summary>
    /// Vérifie le bon fonctionnement de la classe TextTable.
    /// </summary>
    [TestFixture]
    public sealed class CheckTextTable
    {
        [Test]
        public static void CheckTextTableBehavior()
        {
            TextTable table = new TextTable();

            int cursor1 = table.NewCursor(null);
            int cursor2 = table.NewCursor(null);

            table.InsertText(cursor1, new ulong[] { 65ul, 66ul, 67ul });

            int cursor3 = table.NewCursor(null);

            Assert.IsTrue(table.GetCursorPosition(cursor1) == 3);
            Assert.IsTrue(table.GetCursorPosition(cursor1) == 3);
            Assert.IsTrue(table.GetCursorPosition(cursor2) == 3);
            Assert.IsTrue(table.GetCursorPosition(cursor3) == 0);

            CursorInfo[] infoX0 = table.FindCursorsBefore(5);
            CursorInfo[] infoX1 = table.FindCursorsBefore(3);
            CursorInfo[] infoX2 = table.FindCursorsBefore(2);
            CursorInfo[] infoX3 = table.FindCursorsBefore(0);

            Assert.IsTrue(infoX0.Length == 1);
            Assert.IsTrue(infoX1.Length == 1);
            Assert.IsTrue(infoX2.Length == 1);
            Assert.IsTrue(infoX3.Length == 0);

            Assert.IsTrue(infoX0[0].Position == 0);
            Assert.IsTrue(infoX0[0].CursorId == cursor3);

            ulong[] text = new ulong[6000];

            Assert.IsTrue(table.ReadText(cursor1, 10, text) == 0);
            Assert.IsTrue(table.ReadText(cursor2, 10, text) == 0);
            Assert.IsTrue(table.ReadText(cursor3, 10, text) == 3);

            Assert.IsTrue(text[0] == 65ul);
            Assert.IsTrue(text[1] == 66ul);
            Assert.IsTrue(text[2] == 67ul);

            table.InsertText(cursor3, new ulong[] { 34ul });
            table.InsertText(cursor2, new ulong[] { 34ul });

            Assert.IsTrue(table.GetCursorPosition(cursor1) == 5);
            Assert.IsTrue(table.GetCursorPosition(cursor2) == 5);
            Assert.IsTrue(table.GetCursorPosition(cursor3) == 1);

            Assert.IsTrue(table.ReadText(cursor1, 10, text) == 0);
            Assert.IsTrue(table.ReadText(cursor2, 10, text) == 0);
            Assert.IsTrue(table.ReadText(cursor3, 10, text) == 4);

            Assert.IsTrue(text[0] == 65ul);
            Assert.IsTrue(text[1] == 66ul);
            Assert.IsTrue(text[2] == 67ul);
            Assert.IsTrue(text[3] == 34ul);

            Assert.IsTrue(table.MoveCursor(cursor3, 2) == 2);
            Assert.IsTrue(table.MoveCursor(cursor2, -10) == -5);

            Assert.IsTrue(table.GetCursorPosition(cursor1) == 5);
            Assert.IsTrue(table.GetCursorPosition(cursor2) == 0);
            Assert.IsTrue(table.GetCursorPosition(cursor3) == 3);

            CursorInfo[] info0 = table.FindCursors(0, 100);
            Assert.IsTrue(info0.Length == 3);
            Assert.IsTrue(info0[1].Position == 3);
            CursorInfo[] info2 = table.FindCursors(2, 100);
            Assert.IsTrue(info2.Length == 2);
            Assert.IsTrue(info2[1].Position == 5);
            CursorInfo[] info3 = table.FindCursors(3, 100);
            Assert.IsTrue(info3.Length == 2);
            Assert.IsTrue(info3[1].CursorId == cursor1);
            CursorInfo[] info4 = table.FindCursors(4, 100);
            Assert.IsTrue(info4.Length == 1);
            Assert.IsTrue(info4[0].Position == 5);
            CursorInfo[] info5 = table.FindCursors(5, 100);
            Assert.IsTrue(info5.Length == 1);
            Assert.IsTrue(info5[0].CursorId == cursor1);
            CursorInfo[] info6 = table.FindCursors(6, 100);
            Assert.IsTrue(info6.Length == 0);

            Assert.IsTrue(table.ReadText(cursor1, 10, text) == 0);
            Assert.IsTrue(table.ReadText(cursor2, 10, text) == 5);

            Assert.IsTrue(text[0] == 34ul);
            Assert.IsTrue(text[1] == 65ul);
            Assert.IsTrue(text[2] == 66ul);
            Assert.IsTrue(text[3] == 67ul);
            Assert.IsTrue(text[4] == 34ul);

            Assert.IsTrue(table.ReadText(cursor3, 10, text) == 2);

            Assert.IsTrue(text[0] == 67ul);
            Assert.IsTrue(text[1] == 34ul);

            table.MoveCursor(cursor1, -1);
            table.MoveCursor(cursor2, 1);

            Assert.IsTrue(table.GetCursorPosition(cursor2) == 1);
            Assert.IsTrue(table.GetCursorPosition(cursor3) == 3);
            Assert.IsTrue(table.GetCursorPosition(cursor1) == 4);
            Assert.IsTrue(table.TextLength == 5);

            infoX0 = table.FindNextCursor(cursor2, null);
            infoX1 = table.FindNextCursor(cursor3, null);
            infoX2 = table.FindNextCursor(cursor1, null);

            Assert.IsTrue(infoX0.Length == 1);
            Assert.IsTrue(infoX1.Length == 1);
            Assert.IsTrue(infoX2.Length == 0);
            Assert.IsTrue(infoX0[0].CursorId == cursor3);
            Assert.IsTrue(infoX1[0].CursorId == cursor1);

            infoX0 = table.FindPrevCursor(cursor2, null);
            infoX1 = table.FindPrevCursor(cursor3, null);
            infoX2 = table.FindPrevCursor(cursor1, null);

            Assert.IsTrue(infoX0.Length == 0);
            Assert.IsTrue(infoX1.Length == 1);
            Assert.IsTrue(infoX2.Length == 1);
            Assert.IsTrue(infoX1[0].CursorId == cursor2);
            Assert.IsTrue(infoX2[0].CursorId == cursor3);

            infoX0 = table.FindCursorsBefore(5);
            infoX1 = table.FindCursorsBefore(4);
            infoX2 = table.FindCursorsBefore(3);
            infoX3 = table.FindCursorsBefore(2);

            Assert.IsTrue(infoX0.Length == 1);
            Assert.IsTrue(infoX1.Length == 1);
            Assert.IsTrue(infoX2.Length == 1);
            Assert.IsTrue(infoX3.Length == 1);

            Assert.IsTrue(infoX0[0].Position == 4);
            Assert.IsTrue(infoX0[0].CursorId == cursor1);
            Assert.IsTrue(infoX1[0].Position == 3);
            Assert.IsTrue(infoX1[0].CursorId == cursor3);
            Assert.IsTrue(infoX2[0].Position == 1);
            Assert.IsTrue(infoX2[0].CursorId == cursor2);
            Assert.IsTrue(infoX3[0].Position == 1);
            Assert.IsTrue(infoX3[0].CursorId == cursor2);

            CursorInfo[] cInfos1;
            CursorInfo[] cInfos2;
            CursorInfo[] cInfos3;
            //			CursorInfo[] cInfos4;

            table.DeleteText(cursor3, 2, out cInfos1);

            Assert.IsTrue(table.GetCursorPosition(cursor2) == 1);
            Assert.IsTrue(table.GetCursorPosition(cursor3) == 3);
            Assert.IsTrue(table.GetCursorPosition(cursor1) == 3);
            Assert.IsTrue(table.TextLength == 3);
            Assert.IsNull(cInfos1);
            //			Assert.IsNotNull (cInfos1);
            //			Assert.IsTrue (cInfos1.Length == 1);
            //			Assert.IsTrue (cInfos1[0].CursorId == cursor1);

            table.DeleteText(cursor2, 1, out cInfos2);

            Assert.IsTrue(table.GetCursorPosition(cursor2) == 1);
            Assert.IsTrue(table.GetCursorPosition(cursor3) == 2);
            Assert.IsTrue(table.GetCursorPosition(cursor1) == 2);
            Assert.IsTrue(table.TextLength == 2);
            Assert.IsNull(cInfos2);

            infoX0 = table.FindPrevCursor(cursor2, null);
            infoX1 = table.FindPrevCursor(cursor3, null);
            infoX2 = table.FindPrevCursor(cursor1, null);

            Assert.IsTrue(infoX0.Length == 0);
            Assert.IsTrue(infoX1.Length == 1);
            Assert.IsTrue(infoX2.Length == 1);
            Assert.IsTrue(infoX1[0].CursorId == cursor2);
            Assert.IsTrue(infoX2[0].CursorId == cursor3);

            table.MoveCursor(cursor1, -1);
            Assert.IsTrue(table.GetCursorPosition(cursor1) == 1);
            Assert.IsTrue(table.GetCursorPosition(cursor2) == 1);
            Assert.IsTrue(table.GetCursorPosition(cursor3) == 2);

            infoX0 = table.FindCursorsBefore(2);

            Assert.IsTrue(infoX0.Length == 2);

            Assert.IsTrue(infoX0[0].Position == 1);
            Assert.IsTrue(infoX0[1].Position == 1);
            Assert.IsTrue(infoX0[0].CursorId == cursor1);
            Assert.IsTrue(infoX0[1].CursorId == cursor2);

            table.DeleteText(cursor2, 1, out cInfos3);

            Assert.IsTrue(table.GetCursorPosition(cursor2) == 1);
            Assert.IsTrue(table.GetCursorPosition(cursor3) == 1);
            Assert.IsTrue(table.GetCursorPosition(cursor1) == 1);
            Assert.IsTrue(table.TextLength == 1);
            Assert.IsNull(cInfos3);

            //			table.DeleteTextAtPosition (0, 1, out cInfos4);
            //
            //			Assert.IsTrue (table.GetCursorPosition (cursor2) == 0);
            //			Assert.IsTrue (table.GetCursorPosition (cursor3) == 0);
            //			Assert.IsTrue (table.GetCursorPosition (cursor1) == 0);
            //			Assert.IsTrue (table.TextLength == 0);
            //			Assert.IsNull (cInfos4);

            TextTable table1 = new TextTable();
            TextTable table2 = new TextTable();
            TextTable table3 = new TextTable();

            int cursorId = table1.NewCursor(null);

            string sample =
                "ceci est un bref exemple de texte français permettant de générer des fréquences "
                +
                /**/"correctes au niveau de l'utilisation des divers caractères (si l'on fait, bien "
                +
                /**/"sûr abstraction de l'ordre dans lequel les lettres apparaissent, et que l'on "
                +
                /**/"oublie les lettres majuscules).";

            int size = 1 * 1024 * 1024;

            System.Random random = new System.Random(1);

            System.Diagnostics.Trace.WriteLine(
                string.Format(
                    "Generating {0}MB of text / {1} million signs.",
                    8 * size / (1024 * 1024),
                    size / 1000000M
                )
            );

            while (table1.TextLength < size)
            {
                for (int i = 0; i < text.Length; i += 4)
                {
                    int p = random.Next(0, sample.Length - 4);

                    text[i + 0] = sample[p + 0];
                    text[i + 1] = sample[p + 1];
                    text[i + 2] = sample[p + 2];
                    text[i + 3] = sample[p + 3];
                }

                table1.InsertText(cursorId, text);
            }

            string testFile = System.IO.Path.GetTempFileName();
            string testFileCompressed = System.IO.Path.GetTempFileName();
            System.Diagnostics.Trace.WriteLine("Saving file.");
            using (
                System.IO.FileStream file = new System.IO.FileStream(
                    testFile,
                    System.IO.FileMode.OpenOrCreate,
                    System.IO.FileAccess.Write
                )
            )
            {
                table1.WriteRawText(file);
            }
            System.Diagnostics.Trace.WriteLine("Saving file, done.");

            System.Diagnostics.Trace.WriteLine("Loading file.");
            using (
                System.IO.FileStream file = new System.IO.FileStream(
                    testFile,
                    System.IO.FileMode.Open,
                    System.IO.FileAccess.Read
                )
            )
            {
                table2.ReadRawText(file);
            }
            System.Diagnostics.Trace.WriteLine("Loading file, done.");

            Assert.IsTrue(table1.TextLength == table2.TextLength);

            System.Diagnostics.Trace.WriteLine("Saving compressed file.");
            using (
                System.IO.FileStream file = new System.IO.FileStream(
                    testFileCompressed,
                    System.IO.FileMode.OpenOrCreate,
                    System.IO.FileAccess.Write
                )
            )
            {
                using (System.IO.Stream stream = Common.IO.Compression.CreateDeflateStream(file, 3))
                {
                    table1.WriteRawText(stream);
                    stream.Flush();
                    file.SetLength(file.Position);
                }
            }
            System.Diagnostics.Trace.WriteLine("Saving compressed file, done.");

            System.Diagnostics.Trace.WriteLine("Loading compressed file.");
            using (
                System.IO.FileStream file = new System.IO.FileStream(
                    testFileCompressed,
                    System.IO.FileMode.Open,
                    System.IO.FileAccess.Read
                )
            )
            {
                using (System.IO.Stream stream = Common.IO.Decompression.CreateStream(file))
                {
                    table3.ReadRawText(stream);
                }
            }
            System.Diagnostics.Trace.WriteLine("Loading compressed file, done.");

            /*
             *	Mesures sur bi-Xeon 1.7GHz :
             *
             *	Traversée --> 810ns / caractère
             *	Traversée <-- 750ns / caractère
             */

            CursorId cursor3_1 = table3.NewCursor(null);
            CursorId cursor3_2 = table3.NewCursor(null);

            System.Diagnostics.Trace.WriteLine("Starting traversal, 1 char. at a time -->");
            while (table3.MoveCursor(cursor3_1, 1) > 0) { }
            System.Diagnostics.Trace.WriteLine("Done.");

            System.Diagnostics.Trace.WriteLine("Starting traversal, 10'000 char. at a time >>>");
            while (table3.MoveCursor(cursor3_2, 10000) > 0) { }
            System.Diagnostics.Trace.WriteLine("Done.");

            Assert.IsTrue(table3.TextLength == table3.GetCursorPosition(cursor3_1));
            Assert.IsTrue(table3.TextLength == table3.GetCursorPosition(cursor3_2));

            System.Diagnostics.Trace.WriteLine("Starting traversal, 1 char. at a time <--");
            while (table3.MoveCursor(cursor3_1, -1) < 0) { }
            System.Diagnostics.Trace.WriteLine("Done.");

            System.Diagnostics.Trace.WriteLine("Starting traversal, 10'000 char. at a time <<<");
            while (table3.MoveCursor(cursor3_2, -10000) < 0) { }

            System.Diagnostics.Trace.WriteLine("Done.");

            Assert.IsTrue(0 == table3.GetCursorPosition(cursor3_1));
            Assert.IsTrue(0 == table3.GetCursorPosition(cursor3_2));

            /*
             *	Mesures sur bi-Xeon 1.7GHz :
             *
             *	Traversée avec lecture --> 1450ns / caractère
             *	Traversée avec lecture <-- 1350ns / caractère
             */

            ulong xxx = table3[cursor3_1];
            ulong yyy = 0;

            System.Diagnostics.Trace.WriteLine("Starting traversal + read, 1 char. at a time -->");
            while (table3.MoveCursor(cursor3_1, 1) > 0)
            {
                xxx += table3[cursor3_1];
            }
            System.Diagnostics.Trace.WriteLine("Done.");

            System.Diagnostics.Trace.WriteLine("Starting traversal + read, 1 char. at a time <--");
            while (table3.MoveCursor(cursor3_1, -1) < 0)
            {
                yyy += table3[cursor3_1];
            }
            System.Diagnostics.Trace.WriteLine("Done.");

            System.Diagnostics.Trace.WriteLine("xxx=" + xxx + "\nyyy=" + yyy);

            /*
             *  Vérifie que les curseurs peuvent aussi être retrouvés dans un
             *	texte utilisant plusieurs morceaux.
             */

            table3.MoveCursor(cursor3_1, 8000);
            table3.MoveCursor(cursor3_2, 25000);

            CursorInfo[] info3A = table3.FindCursors(10, 100);
            CursorInfo[] info3B = table3.FindCursors(10, 10000);
            CursorInfo[] info3C = table3.FindCursors(10000, 30000);
            CursorInfo[] info3D = table3.FindCursors(1000, 30000);

            Assert.IsTrue(info3A.Length == 0);
            Assert.IsTrue(info3B.Length == 1);
            Assert.IsTrue(info3B[0].Position == 8000);
            Assert.IsTrue(info3C.Length == 1);
            Assert.IsTrue(info3C[0].Position == 25000);
            Assert.IsTrue(info3D.Length == 2);
            Assert.IsTrue(info3D[0].Position == 8000);
            Assert.IsTrue(info3D[1].Position == 25000);
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
