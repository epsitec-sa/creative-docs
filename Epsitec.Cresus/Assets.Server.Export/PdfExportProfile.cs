//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.Server.Export
{
	/// <summary>
	/// Paramètres pour PdfExport.
	/// </summary>
	public class PdfExportProfile : AbstractExportProfile
	{
		public PdfExportProfile(Size pageSize, Margins pageMargins, Margins cellMargins,
			double fontSize, bool automaticColumnWidths, bool evenOddGrey,
			string header, string footer, string indent, string watermark)
		{
			this.PageSize              = pageSize;
			this.PageMargins           = pageMargins;
			this.CellMargins           = cellMargins;
			this.FontSize              = fontSize;
			this.AutomaticColumnWidths = automaticColumnWidths;
			this.EvenOddGrey           = evenOddGrey;
			this.Header                = header;
			this.Footer                = footer;
			this.Indent                = indent;
			this.Watermark             = watermark;
		}

		public static PdfExportProfile Default = new PdfExportProfile (new Size (297.0, 210.0), new Margins (10.0), new Margins (1.0), 10.0, false, false, "Crésus Immobilisations", "Epsitec SA", ".   ", "SPECIMEN");

		public readonly Size					PageSize;
		public readonly Margins					PageMargins;
		public readonly Margins					CellMargins;
		public readonly double					FontSize;
		public readonly bool					AutomaticColumnWidths;
		public readonly bool					EvenOddGrey;
		public readonly string					Header;
		public readonly string					Footer;
		public readonly string					Indent;
		public readonly string					Watermark;
	}
}