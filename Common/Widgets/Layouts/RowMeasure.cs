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


namespace Epsitec.Common.Widgets.Layouts
{
    public sealed class RowMeasure : LayoutMeasure
    {
        public RowMeasure(int passId)
            : base(passId) { }

        public double MinH1
        {
            get { return this.minH1; }
        }

        public double MinH2
        {
            get { return this.minH2; }
        }

        public void UpdateMinH1H2(int passId, double h1, double h2)
        {
            double oldH1 = this.minH1;
            double oldH2 = this.minH2;

            if (this.PassId == passId)
            {
                this.minH1 = System.Math.Max(oldH1, h1);
                this.minH2 = System.Math.Max(oldH2, h2);
            }
            else
            {
                this.minH1 = h1;
                this.minH2 = h2;
            }

            if ((this.minH1 != oldH1) || (this.minH2 != oldH2))
            {
                this.UpdateMin(passId, this.minH1 + this.minH2, forceChange: true);
            }
        }

        private double minH1;
        private double minH2;
    }
}
