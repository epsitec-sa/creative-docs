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
		public PdfExportProfile(PdfStyle style, Size pageSize, Margins pageMargins, Margins cellMargins,
			ExportFont font, double fontSize, bool automaticColumnWidths,
			string header, string footer, string indent, string watermark)
		{
			this.Style                 = style;
			this.PageSize              = pageSize;
			this.PageMargins           = pageMargins;
			this.CellMargins           = cellMargins;
			this.Font                  = font;
			this.FontSize              = fontSize;
			this.AutomaticColumnWidths = automaticColumnWidths;
			this.Header                = header;
			this.Footer                = footer;
			this.Indent                = indent;
			this.Watermark             = watermark;
		}

		public static PdfExportProfile Default = new PdfExportProfile (PdfStyle.Factory (PdfPredefinedStyle.Default), new Size (297.0, 210.0), new Margins (10.0), new Margins (1.0), ExportFont.Arial, 10.0, false, "Crésus Immobilisations", "Epsitec SA", "•   ", "SPECIMEN");

		public readonly PdfStyle				Style;
		public readonly Size					PageSize;
		public readonly Margins					PageMargins;
		public readonly Margins					CellMargins;
		public readonly ExportFont				Font;
		public readonly double					FontSize;
		public readonly bool					AutomaticColumnWidths;
		public readonly string					Header;
		public readonly string					Footer;
		public readonly string					Indent;
		public readonly string					Watermark;
	}
}