/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


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
                pageOffsetEven.X = System.Math.Max(
                    pageOffsetEven.X,
                    info.CropMarksLengthX + info.CropMarksOffsetX
                );
                pageOffsetEven.Y = System.Math.Max(
                    pageOffsetEven.Y,
                    info.CropMarksLengthY + info.CropMarksOffsetY
                );
                pageOffsetOdd.X = System.Math.Max(
                    pageOffsetOdd.X,
                    info.CropMarksLengthX + info.CropMarksOffsetX
                );
                pageOffsetOdd.Y = System.Math.Max(
                    pageOffsetOdd.Y,
                    info.CropMarksLengthY + info.CropMarksOffsetY
                );
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
