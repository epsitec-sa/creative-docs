using Epsitec.Common.Pdf.Array;
using Epsitec.Common.Pdf.Engine;

using Epsitec.Common.Types;

using System;

using System.Collections.Generic;

using System.IO;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.Extraction
{


	using Array = Epsitec.Common.Pdf.Array.Array;


	/// <summary>
	/// This class will serialize an array of entities to a pdf file.
	/// </summary>
	internal sealed class PdfArrayFormat : ArrayFormat
	{


		// This class is not finished and thus not used yet. There are two main problems:
		// - If should be more configurable. Like the user should be able to choose stuff like
		//   - The paper size
		//   - The paper orientation
		//   - The font
		//   - The font size
		//   - Basic colors
		//   - Basic line stuff
		//   - Whether to display the page number
		//   - Some headers and footers
		//   - ...
		// - The Common.Pdf classes are way too slow to use in procution. This must be corrected
		//   before this class is used in production.
		// Once these two points are resolved, the javascript client and the server must be
		// modified so that the client can configure its pdf exportation and do it.


		public override string Extension
		{
			get
			{
				return "pdf";
			}
		}


		public override void Write(Stream stream, IList<string> headers, IList<IList<string>> rows)
		{
			var pdfArray = this.GetPdfArray ();
			var rowCount = rows.Count;
			var columnDefinitions = this.GetColumnDefinitions (headers);
			var dataAccessor = this.GetDataAccessor (rows);

			pdfArray.GeneratePdf (stream, rowCount, columnDefinitions, dataAccessor);
		}


		private Array GetPdfArray()
		{
			var exportPdfInfo = this.GetExportPdfInfo ();
			var arraySetup = this.GetArraySetup ();

			return new Array (exportPdfInfo, arraySetup);
		}


		private ExportPdfInfo GetExportPdfInfo()
		{
			return new ExportPdfInfo ()
			{
				// TODO Add some configuration here.
			};
		}


		private ArraySetup GetArraySetup()
		{
			return new ArraySetup ()
			{
				// TODO Add some configuration here.
			};
		}


		private List<ColumnDefinition> GetColumnDefinitions(IEnumerable<string> headers)
		{
			return headers
				.Select (h => new ColumnDefinition (FormattedText.FromSimpleText (h), ColumnType.Automatic))
				.ToList ();
		}


		private Func<int, int, CellContent> GetDataAccessor(IList<IList<string>> rows)
		{
			return (i, j) => new CellContent (FormattedText.FromSimpleText (rows[i][j]));
		}


	}


}
