using Epsitec.Common.IO;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Data;

using System.IO;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{

	
	internal static class RecordHelper
	{


		public static string GetString(ReadOnlyCollection<string> record, int index)
		{
			var data = record[index];

			if (string.IsNullOrWhiteSpace (data))
			{
				data = null;
			}

			return data;
		}


		public static IEnumerable<ReadOnlyCollection<string>> GetRecords(FileInfo inputFile, int columnCount)
		{
			// NOTE Here we use dummy column names with integers as we don't really use them but we
			// need to provide something to the CsvReader, otherwise it will crash as there are no
			// names defined in the CSV files.

			var csvFormat = new CsvFormat ()
			{
				ColumnNames = Enumerable.Range (0, columnCount).Select (i => i.ToString ()).ToArray (),
				Encoding = System.Text.UTF8Encoding.Default,
				FieldSeparator = ";",
				MultilineSeparator = "\n",
			};

			using (var dataTable = CsvReader.ReadCsv (inputFile.FullName, csvFormat))
			{
				foreach (DataRow dataRow in dataTable.Rows)
				{
					var recordList = new List<string> ();

					foreach (var value in dataRow.ItemArray)
					{
						string stringValue;

						if (value == System.DBNull.Value)
						{
							stringValue = null;
						}
						else
						{
							stringValue = (string) value;
						}

						recordList.Add (stringValue);
					}

					yield return recordList.AsReadOnly ();
				}
			}
		}


	}


}

