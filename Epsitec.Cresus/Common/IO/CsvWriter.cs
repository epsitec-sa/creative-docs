using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;

using System.IO;

using System.Linq;
using System;


namespace Epsitec.Common.IO
{
	/// <summary>
	/// Helper class that is used to write data as CSV.
	/// </summary>
	/// <remarks>
	/// The definition of the CSV format used here is defined here:
	/// http://www.creativyst.com/Doc/Articles/CSV/CSV01.htm
	/// 
	/// Here is a summary of the rules:
	/// - Lines are separated by the given instance CsvFormat.LineSeparator char
	/// - Fields are separated by the given instance CsvFormat.FieldSeparator char
	/// - The quote char is defined by the instance CsvFormat.QuoteChar char
	/// - A field must be quoted if
	///   - it starts with space
	///   - it ends with space
	///   - it contains the line separator char
	///   - it contains the field separator char
	///   - it contains the quote char
	/// - An quoted field is enclosed within quote chars and each occurence of the quote char
	///   within it is escaped with the quote char (the field [a"b] becomes ["a""b"])
	/// 
	/// The CsvFormat must contain the header definitions.
	/// </remarks>
	public static class CsvWriter
	{
		public static void Write(CsvFormat format, IEnumerable<IEnumerable<string>> rows, string outputFile)
		{
			using (var outputStream = File.OpenWrite (outputFile))
			{
				CsvWriter.Write (format, rows, outputStream);
			}
		}

		public static void Write(CsvFormat format, IEnumerable<IEnumerable<string>> rows, Stream outputStream)
		{
			var lines = CsvWriter.GetLines (format, rows);

			CsvWriter.WriteLines (format, lines, outputStream);
		}

		private static IEnumerable<IEnumerable<string>> GetLines(CsvFormat format, IEnumerable<IEnumerable<string>> rows)
		{
			var headers = format.ColumnNames.ToEnumerable<IEnumerable<string>> ();
			var lines = headers.Concat (rows);

			return lines.Select (l => CsvWriter.GetFields (format, l));
		}

		private static IEnumerable<string> GetFields(CsvFormat format, IEnumerable<string> cells)
		{
			return cells.Select (f => CsvWriter.GetField (format, f));
		}

		private static string GetField(CsvFormat format, string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return "";
			}

			if (CsvWriter.MustQuoteField (format, value))
			{
				return CsvWriter.QuoteField (format, value);
			}

			return value;
		}

		private static bool MustQuoteField(CsvFormat format, string field)
		{
			// If the string contains any special char, it must be quoted.
			var specialChars = new char[]
			{
				format.QuoteChar,
				format.FieldSeparator,
				format.LineSeparator
			};

			var index = field.IndexOfAny (specialChars);

			if (index != -1)
			{
				return true;
			}

			// If the string starts or ends with space, it must be quoted. 
			if (field[0] == ' ' || field[field.Length - 1] == ' ')
			{
				return true;
			}

			// If none of the above condition holds, the string don't require to be quoted.
			return false;
		}

		private static string QuoteField(CsvFormat format, string field)
		{
			var oneQuote = "" + format.QuoteChar;
			var twoQuotes = "" + format.QuoteChar + format.QuoteChar;

			var escapedField = field.Replace (oneQuote, twoQuotes);

			return "" + format.QuoteChar + escapedField + format.QuoteChar;
		}

		private static void WriteLines(CsvFormat format, IEnumerable<IEnumerable<string>> lines, Stream stream)
		{
			// We don't dispose the stream writer because we don't want to dispose the underlying
			// stream here. Maybe the caller has used a memory stream and intends to read back the
			// result.
			var streamWriter = new StreamWriter (stream, format.Encoding);

			var firstLine = true;

			foreach (var line in lines)
			{
				if (!firstLine)
				{
					streamWriter.Write (format.LineSeparator);
				}
				else
				{
					firstLine = false;
				}

				var firstField = true;

				foreach (var field in line)
				{
					if (!firstField)
					{
						streamWriter.Write (format.FieldSeparator);
					}
					else
					{
						firstField = false;
					}

					streamWriter.Write (field);
				}
			}

			streamWriter.Flush ();
		}
	}
}
