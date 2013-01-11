//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Pdf.Array
{
	public class Array
	{
		public PdfExportException GeneratePdf(string path, int lineCount, List<ColumnDefinition> columnDefinitions, Func<int, int, FormattedText> accessor, ExportPdfInfo info, ArraySetup setup)
		{
			this.lineCount         = lineCount;
			this.columnDefinitions = columnDefinitions;
			this.accessor          = accessor;
			this.info              = info;
			this.setup             = setup;

			int pageCount = 1;

			var export = new Export (this.info);
			return export.ExportToFile (path, pageCount, this.Renderer);
		}

		private void Renderer(Port port, int page)
		{
		}


		private int lineCount;
		private List<ColumnDefinition> columnDefinitions;
		private Func<int, int, FormattedText> accessor;
		private ArraySetup setup;
		private ExportPdfInfo info;
	}
}
