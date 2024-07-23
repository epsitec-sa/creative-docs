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
using Epsitec.Common.Types.Collections;
using Epsitec.Common.Widgets.Layouts;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
    public sealed class ItemListColumnCollection : ObservableList<ItemListColumn>
    {
        public ItemListColumnCollection() { }

        public IEnumerable<ItemListColumn> BySortOrder
        {
            get { return this.list.OrderBy(x => x.SortIndex); }
        }

        public ItemListColumn GetColumn(int index)
        {
            return this.FirstOrDefault(x => x.Index == index);
        }

        public ItemListColumn DetectColumn(Point pos)
        {
            return this.FirstOrDefault(x => x.Contains(pos));
        }

        public void SpecifySort(ItemListColumn column, ItemSortOrder sortOrder)
        {
            if (column.CanSort == false)
            {
                return;
            }

            System.Diagnostics.Debug.Assert(this.Contains(column));

            column.SortOrder = sortOrder;

            var sorted = this
                .list.Where(x => x.CanSort && x != column)
                .OrderBy(x => x.SortIndex)
                .ThenBy(x => x.Index);
            var unsorted = this.list.Where(x => !x.CanSort).OrderBy(x => x.Index);

            int index = 0;

            if (sortOrder == ItemSortOrder.None)
            {
                //	The column should no longer participate in the sort. Place it at its
                //	natural position in the list, with respect to all other unsorted columns.
            }
            else
            {
                //	The newly sorted column will be the main sort column; assign it the index
                //	zero. Then assign the other columns 1, 2, 3, ...

                column.SortIndex = index++;
            }

            foreach (var x in sorted.Concat(unsorted).ToArray())
            {
                x.SortIndex = index++;
            }
        }

        public double Layout(double availableSpace)
        {
            return ColumnLayoutList.Fit(
                this.OrderBy(x => x.Index).Select(x => x.Layout),
                availableSpace
            );
        }
    }
}
