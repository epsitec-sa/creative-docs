//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.Export
{
	/// <summary>
	/// Paramètres pour PdfExport.
	/// </summary>
	public class PdfExportProfile : AbstractExportProfile
	{
		public PdfExportProfile(decimal pageWidth, decimal pageHeight, decimal leftMargin, decimal rightMargin, decimal topMargin, decimal bottomMargin, decimal fontSize, decimal cellMargins, bool evenOddGrey)
		{
			this.PageWidth    = pageWidth;
			this.PageHeight   = pageHeight;
			this.LeftMargin   = leftMargin;
			this.RightMargin  = rightMargin;
			this.TopMargin    = topMargin;
			this.BottomMargin = bottomMargin;
			this.FontSize     = fontSize;
			this.CellMargins  = cellMargins;
			this.EvenOddGrey  = evenOddGrey;
		}

		public static PdfExportProfile Default = new PdfExportProfile (297.0m, 210.0m, 10.0m, 10.0m, 10.0m, 10.0m, 10.0m, 1.0m, false);

		public readonly decimal					PageWidth;
		public readonly decimal					PageHeight;
		public readonly decimal					LeftMargin;
		public readonly decimal					RightMargin;
		public readonly decimal					TopMargin;
		public readonly decimal					BottomMargin;
		public readonly decimal					FontSize;
		public readonly decimal					CellMargins;
		public readonly bool					EvenOddGrey;
	}
}