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


namespace Epsitec.Common.Drawing
{
    /// <summary>
    /// The DashedPath class represents a ... dashed path.
    /// </summary>
    public class DashedPath : Path
    {
        public DashedPath() { }

        public double DashOffset
        {
            get { return this.start; }
            set
            {
                this.start = value;
                this.path.SetDashOffset(this.start);
            }
        }

        public void ResetDash()
        {
            this.path.ResetDash();
        }

        public void AddDash(double dashLength, double gapLength)
        {
            this.path.AddDash(dashLength, gapLength);
        }

        public Path GenerateDashedPath()
        {
            return this.GenerateDashedPath(this.defaultZoom);
        }

        public Path GenerateDashedPath(double approximationZoom)
        {
            if (this.IsEmpty)
            {
                return null;
            }

            AntigrainSharp.Path path = new AntigrainSharp.Path();

            //path.InternalCreateNonEmpty();

            path.AppendDashedPath(this.path, approximationZoom);

            return new Path(path);
        }

        private double start;
    }
}
