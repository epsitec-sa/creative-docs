//	Copyright © 2008-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.IO
{
	/// <summary>
	/// The <c>CsvReader</c> class provides utility functions to load CSV data
	/// into tables.
	/// </summary>
	/// <remarks>
	/// Warning: this class strictly follows the CSV "standard" defined here
	/// http://www.creativyst.com/Doc/Articles/CSV/CSV01.htm and does not follow exactly the options
	/// defined in the given instance of CsvFormat. In particular:
	/// - Trailing spaces in fields are not skipped
	/// - Some leading whitespaces in lines are skipped but should not
	/// - The lines are not split according to CsvFormat.LineSeparator but according to '\r' or '\n'
	/// - Chars corresponding to CsvFormat.LineSeparator in fields are replaced with '\n'
	/// I choose not to correct these bugs, because some code might actually depend on them.
	/// </remarks>
	public static class CsvReader
	{
		public static System.Data.DataTable ReadCsv(string path, CsvFormat format)
		{
			using (System.IO.FileStream stream = new System.IO.FileStream (path, System.IO.FileMode.Open))
			{
				return CsvReader.ReadCsv (stream, format);
			}
		}

		public static System.Data.DataTable ReadCsv(System.IO.Stream stream, CsvFormat format)
		{
			int    pos  = 0;
			string text = ReaderHelper.ReadText (stream, format.Encoding);

			//	First line in CSV contains the explicit field names; this is a
			//	requirement for us, or else we would not know how to name the
			//	columns :

			string[] headers;

			if ((format.ColumnNames == null) ||
				(format.ColumnNames.Length == 0))
			{
				headers = CsvReader.SplitRow (text, ref pos, format);
			}
			else
			{
				headers = format.ColumnNames;
			}

			System.Data.DataTable table = new System.Data.DataTable ();

			foreach (string header in headers)
			{
				table.Columns.Add (header);
			}

			//	Read the rows, line by line.

			while (true)
			{
				string[] data = CsvReader.SplitRow (text, ref pos, format);

				if ((data == null) ||
					(data.Length == 0))
				{
					break;
				}

				object[] fields = new object[headers.Length];
				data.CopyTo (fields, 0);

				table.Rows.Add (fields);
			}

			return table;
		}

		/// <summary>
		/// Splits the text row into cells. This takes into account the field separator
		/// character; no other preprocessing is done here.
		/// </summary>
		/// <param name="source">The source text.</param>
		/// <param name="pos">The position in the source text.</param>
		/// <param name="format">The CSV format.</param>
		/// <returns>The array of cells or an empty array.</returns>
		private static string[] SplitRow(string source, ref int pos, CsvFormat format)
		{
			List<string> items = new List<string> ();

			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			bool quote = false;
			bool first = true;

			char fieldSep = format.FieldSeparator;

			while (pos < source.Length)
			{
				char c = source[pos];

				if ((c == ' ') || (c == '\n') || (c == '\r'))
				{
					pos++;
				}
				else
				{
					break;
				}
			}

			char lineBreak = format.LineSeparator;

			if (pos >= source.Length)
			{
				return items.ToArray ();
			}

			while (pos < source.Length)
			{
				char c = source[pos++];

				if (c == format.QuoteChar)
				{
					quote = !quote;

					if (first || !quote)
					{
						first = false;
						continue;
					}
				}
				else if ((c == ' ') && first)
				{
					continue;
				}
				else if (!quote)
				{
					//	Not inside quoted text; handle separators and line endings as
					//	meaningful markers, not just data :

					if (c == fieldSep)
					{
						items.Add (buffer.ToString ());
						buffer.Length = 0;
						first = true;
						continue;
					}
					if ((c == '\r') ||
						(c == '\n'))
					{
						items.Add (buffer.ToString ());
						break;
					}
				}
				else
				{
					//	Character to be treated as data.
				}

				if (c == lineBreak)
				{
					c = '\n';
				}

				buffer.Append (c);
				first = false;
			}

			return items.ToArray ();
		}
	}
}
