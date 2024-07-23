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


namespace Epsitec.Common.Widgets.Helpers
{
    /// <summary>
    /// La classe GroupComparer permet de comparer/trier des widgets selon leur
    /// groupe, puis selon leur index.
    /// </summary>
    public class GroupComparer : System.Collections.IComparer
    {
        public GroupComparer(int dir)
        {
            System.Diagnostics.Debug.Assert((dir == -1) || (dir == 1));

            this.dir = dir;
        }

        public int Compare(object x, object y)
        {
            Widget bx = x as Widget;
            Widget by = y as Widget;

            if (bx == by)
                return 0;

            if (bx == null)
                return -this.dir;
            if (by == null)
                return this.dir;

            int compare = string.Compare(bx.Group, by.Group) * this.dir;

            return (compare == 0) ? (bx.Index - by.Index) * this.dir : compare;
        }

        protected int dir;
    }
}
