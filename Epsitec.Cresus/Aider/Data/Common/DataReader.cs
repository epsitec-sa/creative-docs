//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using ClosedXML.Excel;

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Epsitec.Aider.Data.Common
{


	internal static class DataReader
	{


		public static IEnumerable<Dictionary<T, string>> GetRecords<T>(FileInfo input, Dictionary<T, string> stringMapping, int nbToSkip = 0)
		{
			if (input == null)
			{
				yield break;
			}

			Dictionary<T, int?> indexMapping = null;

			foreach (var line in DataReader.GetLines (input).Skip (nbToSkip))
			{
				if (indexMapping == null)
				{
					indexMapping = DataReader.GetIndexMapping (line, stringMapping);
				}
				else
				{
					var record = DataReader.GetRecord (line, indexMapping);

					var isEmpty = record.All (r => string.IsNullOrWhiteSpace (r.Value));

					if (!isEmpty)
					{
						yield return record;
					}
				}
			}
		}


		private static IEnumerable<IList<string>> GetLines(FileInfo input)
		{
			var lines = new List<List<string>> ();

			if (input != null)
			{
				using (var workbook = new XLWorkbook (input.FullName))
				{
					var worksheet = workbook.Worksheets.First ();

					foreach (var row in worksheet.RowsUsed ())
					{
						var line = new List<string> ();

						foreach (var cell in row.CellsUsed ())
						{
							var columnIndex = cell.Address.ColumnNumber - 1;
							var value = cell.GetValue<string> ();

							line.InsertAtIndex (columnIndex, value);
						}

						lines.Add (line);
					}
				}
			}

			return lines;
		}


		private static Dictionary<T, int?> GetIndexMapping<T>
		(
			IList<string> headers,
			Dictionary<T, string> stringMapping
		)
		{
			var indexMapping = new Dictionary<T, int?> ();

			foreach (var item in stringMapping)
			{
				var key = item.Key;
				var index = headers.IndexOf (item.Value);

				indexMapping[key] = index >= 0
					? index
					: (int?) null;
			}

			return indexMapping;
		}


		private static Dictionary<T, string> GetRecord<T>
		(
			IList<string> line,
			Dictionary<T, int?> indexMapping
		)
		{
			var r = new Dictionary<T, string> ();

			foreach (var mapping in indexMapping)
			{
				var key = mapping.Key;
				var index = mapping.Value;

				var value = index.HasValue && line.Count > index.Value
					? line[index.Value]
					: null;

				if (value != null)
				{
					value = value.Trim ();

					if (string.IsNullOrWhiteSpace (value))
					{
						value = null;
					}
				}

				r[key] = value;
			}

			return r;
		}


	}


}
