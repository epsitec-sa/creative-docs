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

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// The <c>DragEventArgs</c> class describes a drag and drop event produced
    /// by the <c>Widgets</c> framework (contrast with <see cref="WindowDragEventArgs"/>
    /// which is produced by the <c>WinForms</c> framework).
    /// </summary>
    public class DragEventArgs : Support.EventArgs
    {
        public DragEventArgs(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public Point FromPoint
        {
            get { return this.p1; }
        }

        public Point ToPoint
        {
            get { return this.p2; }
        }

        public Point Offset
        {
            get { return this.p2 - this.p1; }
        }

        private readonly Point p1;
        private readonly Point p2;
    }
}
