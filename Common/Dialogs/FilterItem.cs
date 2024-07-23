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


using System.Linq;
using Epsitec.Common.Support.Extensions;

namespace Epsitec.Common.Dialogs
{
    /// <summary>
    /// The <c>FilterItem</c> class represents a filter for the file dialogs.
    /// </summary>
    public sealed class FilterItem
    {
        public FilterItem(string name, string caption, string filter)
        {
            ExceptionThrower.ThrowIfNullOrEmpty(name, "name");
            ExceptionThrower.ThrowIfNullOrEmpty(caption, "caption");
            ExceptionThrower.ThrowIfNullOrEmpty(filter, "filter");

            string[] items = filter
                .Split(' ', '|', ':', '/')
                .Where(x => x.Length > 0)
                .Select(x => FilterItem.SanitizeFilterItem(x))
                .ToArray();

            this.name = name;
            this.caption = caption;
            this.filter = string.Join(",", items);
        }

        public string Name
        {
            get { return this.name; }
        }

        public string Caption
        {
            get { return this.caption; }
        }

        public string Filter
        {
            get { return this.filter; }
        }

        public string GetFileDialogFilter()
        {
            return this.filter;
        }

        /// <summary>
        /// Ensures that the filter item starts always with <c>"*."</c> so that it is
        /// compatible with the file dialogs.
        /// </summary>
        /// <param name="item">The filter item.</param>
        /// <returns>The sanitized filter item.</returns>
        private static string SanitizeFilterItem(string item)
        {
            if (item.StartsWith("*."))
            {
                return item.Substring(2);
            }
            else if (item.StartsWith("."))
            {
                return item.Substring(1);
            }
            else
            {
                return item;
            }
        }

        private readonly string name;
        private readonly string caption;
        private readonly string filter;
    }
}
