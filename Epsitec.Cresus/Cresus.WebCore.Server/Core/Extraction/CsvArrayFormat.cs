using Epsitec.Common.IO;

using System.Collections.Generic;

using System.IO;

using System.Linq;

using System.Text;


namespace Epsitec.Cresus.WebCore.Server.Core.Extraction
{


	/// <summary>
	/// This ArrayFormat will serializes an array of entities to a CSV file.
	/// </summary>
	internal sealed class CsvArrayFormat : ArrayFormat
	{


		public override string Extension
		{
			get
			{
				return "csv";
			}
		}


		public override void Write(Stream stream, IList<string> headers, IList<IList<string>> rows)
		{
			var format = new CsvFormat ()
			{
				FieldSeparator = ';',
				LineSeparator = '\n',
				QuoteChar = '"',
				Encoding = Encoding.UTF8,
				ColumnNames = headers.ToArray ()
			};

			CsvWriter.Write (format, rows, stream);
		}


	}


}
