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


using System.Collections.Generic;

namespace Epsitec.Common.Document.PDF
{
    class PdfExportPageRange
    {
        public PdfExportPageRange(int max)
        {
            this.Max = max;
            this.From = 1;
            this.To = max;
        }

        public void Constrain()
        {
            this.From = System.Math.Min(this.From, this.Max);
            this.To = System.Math.Min(this.To, this.Max);
            if (this.From > this.To)
                Misc.Swap(ref this.From, ref this.To);
        }

        public IEnumerable<int> GetPrintablePageList(Modifier modifier)
        {
            List<int> pageList = new List<int>();
            if (this.JustOneMaster)
                pageList.Add(this.From);
            else
                for (int page = this.From; page <= this.To; page++)
                {
                    int rank = modifier.PrintablePageRank(page - 1);
                    if (rank != -1)
                        pageList.Add(rank);
                }
            return pageList;
        }

        public int Max;
        public int From;
        public int To;
        public bool JustOneMaster;
        public int Total
        {
            get { return this.To - this.From + 1; }
        }
    }
}
