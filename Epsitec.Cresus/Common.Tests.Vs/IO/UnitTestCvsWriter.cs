using Epsitec.Common.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.IO;

using System.Text;

namespace Epsitec.Common.Tests.Vs.IO
{
	[TestClass]
	public class UnitTestCvsWriter
	{
		[TestMethod]
		public void SimpleTest()
		{
			var format = new CsvFormat ()
			{
				ColumnNames = this.GetHeaders ()
			};
			var rows = this.GetRows ();

			var encoding = Encoding.Default;
			var expected = "a;b;c\n1;2;3\n4;5;6\n7;8;9";

			this.Check (format, rows, encoding, expected);
		}

		[TestMethod]
		public void TestFieldSeparator()
		{
			var format = new CsvFormat ()
			{
				ColumnNames = this.GetHeaders (),
				FieldSeparator = '-'
			};
			var rows = this.GetRows ();

			var encoding = Encoding.Default;
			var expected = "a-b-c\n1-2-3\n4-5-6\n7-8-9";

			this.Check (format, rows, encoding, expected);
		}

		[TestMethod]
		public void TestLineSeparator()
		{
			var format = new CsvFormat ()
			{
				ColumnNames = this.GetHeaders (),
				LineSeparator = '-'
			};
			var rows = this.GetRows ();

			var encoding = Encoding.Default;
			var expected = "a;b;c-1;2;3-4;5;6-7;8;9";

			this.Check (format, rows, encoding, expected);
		}

		[TestMethod]
		public void TestEncoding()
		{
			// The € sign is not at the same place in the greek encoding than in the encoding that
			// you should get with the Encoding.Default property. This way we check that the csv
			// writer does not uses it.

			var greekEncoding = Encoding.GetEncoding ("iso-8859-7");
			var format = new CsvFormat ()
			{
				ColumnNames = this.GetHeaders (),
				Encoding = greekEncoding
			};

			var rows = new List<IEnumerable<string>> ()
			{
				new List<string> () { "€", "€", "€", }
			};

			var expected = "a;b;c\n€;€;€";

			this.Check (format, rows, greekEncoding, expected);
		}

		[TestMethod]
		public void TestQuote()
		{
			var format = new CsvFormat ()
			{
				ColumnNames = this.GetHeaders ()
			};

			var rows = new List<IEnumerable<string>> ()
			{
				new List<string> () { " a", "a ", "\"", ";", "\n" },
			};

			var encoding = Encoding.Default;
			var expected = "a;b;c\n"
				+ "\" a\"" + ";"
				+ "\"a \"" + ";"
				+ "\"\"\"\"" + ";"
				+ "\";\"" + ";"
				+ "\"\n\"";

			this.Check (format, rows, encoding, expected);
		}

		private string[] GetHeaders()
		{
			return new string[] { "a", "b", "c" };
		}

		private IEnumerable<IEnumerable<string>> GetRows()
		{
			int nb = 1;

			for (int i = 0; i < 3; i++)
			{
				var list = new List<string> ();

				for (int j = 0; j < 3; j++)
				{
					list.Add (nb.ToString ());

					nb++;
				}

				yield return list;
			}
		}

		private void Check(CsvFormat format, IEnumerable<IEnumerable<string>> rows, Encoding encoding, string expected)
		{
			var actualBytes = this.GetActualBytes (format, rows);
			var expectedBytes = this.GetExpectedBytes (encoding, expected);

			CollectionAssert.AreEqual (expectedBytes, actualBytes);
		}

		private byte[] GetActualBytes(CsvFormat format, IEnumerable<IEnumerable<string>> rows)
		{
			using (var memoryStream = new MemoryStream ())
			{
				CsvWriter.Write (format, rows, memoryStream);

				return memoryStream.ToArray ();
			}
		}

		private byte[] GetExpectedBytes(Encoding encoding, string expected)
		{
			return encoding.GetBytes (expected);
		}
	}
}
