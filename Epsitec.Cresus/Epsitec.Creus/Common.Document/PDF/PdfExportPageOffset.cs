//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.PDF
{
	class PdfExportPageOffset
	{
		public PdfExportPageOffset(Settings.ExportPDFInfo info)
		{
			//	Objet décrivant le format de la page.
			Point pageOffsetEven = Point.Zero;
			Point pageOffsetOdd = Point.Zero;
			pageOffsetEven.X = info.BleedMargin + info.BleedEvenMargins.Left;
			pageOffsetEven.Y = info.BleedMargin + info.BleedEvenMargins.Bottom;
			pageOffsetOdd.X = info.BleedMargin + info.BleedOddMargins.Left;
			pageOffsetOdd.Y = info.BleedMargin + info.BleedOddMargins.Bottom;
			if (info.PrintCropMarks)
			{
				// traits de coupe ?
				pageOffsetEven.X = System.Math.Max (pageOffsetEven.X, info.CropMarksLengthX + info.CropMarksOffsetX);
				pageOffsetEven.Y = System.Math.Max (pageOffsetEven.Y, info.CropMarksLengthY + info.CropMarksOffsetY);
				pageOffsetOdd.X = System.Math.Max (pageOffsetOdd.X, info.CropMarksLengthX + info.CropMarksOffsetX);
				pageOffsetOdd.Y = System.Math.Max (pageOffsetOdd.Y, info.CropMarksLengthY + info.CropMarksOffsetY);
			}
			this.offsetOdd = pageOffsetOdd;
			this.offsetEven = pageOffsetEven;
		}
		public Point GetPageOffset(int rank)
		{
			return (rank % 2 == 1) ? this.offsetEven : this.offsetOdd;
		}
		private Point offsetOdd;
		private Point offsetEven;
	}
}
