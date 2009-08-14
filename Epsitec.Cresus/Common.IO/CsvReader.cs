//	Copyright © 2008-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.IO
{
	/// <summary>
	/// The <c>CsvReader</c> class provides utility functions to load CSV data
	/// into tables.
	/// </summary>
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
				headers = ReaderHelper.SplitRow (text, ref pos, format);
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
				string[] data = ReaderHelper.SplitRow (text, ref pos, format);

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
	}
}
