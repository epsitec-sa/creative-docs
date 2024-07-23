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


namespace Epsitec.Common.Support.ResourceAccessors
{
    /// <summary>
    /// The <c>PrefixedCultureMap</c> class is used to represent fields
    /// (from a structured type) and values (from an enumeration) which
    /// have a read only prefix part.
    /// </summary>
    internal class PrefixedCultureMap : CultureMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrefixedCultureMap"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="id">The id associated with the item.</param>
        /// <param name="source">The module source.</param>
        /// <param name="prefix">The prefix (if any) or <c>null</c>.</param>
        public PrefixedCultureMap(
            IResourceAccessor owner,
            Druid id,
            CultureMapSource source,
            string prefix
        )
            : base(owner, id, source)
        {
            this.prefix = prefix;
        }

        /// <summary>
        /// Gets or sets the prefix for this item.
        /// </summary>
        /// <value>The prefix.</value>
        public override string Prefix
        {
            get { return this.prefix; }
            set { this.prefix = value; }
        }

        /// <summary>
        /// Returns a string that represents the current item. This is the result
        /// of the concatenation of the prefix and of the name.
        /// </summary>
        /// <returns>
        /// A string that represents the current item.
        /// </returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.prefix))
            {
                return this.Name ?? "";
            }
            else
            {
                return string.Concat(this.prefix, ".", this.Name ?? "");
            }
        }

        private string prefix;
    }
}
