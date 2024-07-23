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


namespace Epsitec.Common.Types
{
    /// <summary>
    /// The <c>PropertyGroupDescription</c> class describes the grouping of
    /// items using a property name as the grouping criteria.
    /// </summary>
    public class PropertyGroupDescription : GroupDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGroupDescription"/> class.
        /// </summary>
        public PropertyGroupDescription() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGroupDescription"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public PropertyGroupDescription(string propertyName)
        {
            this.propertyName = propertyName;
        }

        /// <summary>
        /// Gets or sets the name of the property that is used to determine
        /// which group an item belongs to.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName
        {
            get { return this.propertyName; }
            set
            {
                if (this.propertyName != value)
                {
                    this.propertyName = value;
                    this.cachedType = null;
                    this.cachedGetter = null;
                }
            }
        }

        /// <summary>
        /// Gets the value used to derive the group(s).
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The value used to derive the group(s) or <c>UnknownValue.Value</c> if
        /// no value can be found.</returns>
        protected override object GetGroupValue(object item)
        {
            //	Based on the property name, return either the property value or
            //	the item itself as the base value used to derive the group name.

            if ((item == null) || (string.IsNullOrEmpty(this.propertyName)))
            {
                return item;
            }

            System.Type type = item.GetType();

            if (this.cachedType != type)
            {
                this.cachedType = type;
                this.cachedGetter = this.CreateGetter(item);
            }

            if (this.cachedGetter == null)
            {
                return UnknownValue.Value;
            }
            else
            {
                return this.cachedGetter(item);
            }
        }

        private Support.PropertyGetter CreateGetter(object item)
        {
            if (item is IStructuredData)
            {
                return StructuredData.CreatePropertyGetter(this.propertyName);
            }
            else
            {
                return Support.DynamicCodeFactory.CreatePropertyGetter(
                    this.cachedType,
                    this.propertyName
                );
            }
        }

        private string propertyName;
        private System.Type cachedType;
        private Support.PropertyGetter cachedGetter;
    }
}
