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


using Epsitec.Common.Support.Extensions;
using System.Collections.Generic;

namespace Epsitec.Common.UI
{
    /// <summary>
    /// The <c>ItemViewFactoryAttribute</c> attribute is used to tag a class as
    /// compatible with the <see cref="ItemPanel"/> and <see cref="IItemViewFactory"/>
    /// pattern. This attribute is applied at the assembly level.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ItemViewFactoryAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemViewFactoryAttribute"/> class.
        /// </summary>
        /// <param name="factoryType">The type of the factory class.</param>
        public ItemViewFactoryAttribute(System.Type factoryType)
        {
            this.factoryType = factoryType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemViewFactoryAttribute"/> class.
        /// </summary>
        /// <param name="factoryType">The type of the factory class.</param>
        /// <param name="itemType">Type of the item represented by the factory class.</param>
        public ItemViewFactoryAttribute(System.Type factoryType, System.Type itemType)
        {
            this.factoryType = factoryType;
            this.itemType = itemType;
        }

        /// <summary>
        /// Gets or sets the type of the class which adheres to the
        /// <see cref="ItemPanel"/> and <see cref="IItemViewFactory"/> pattern.
        /// </summary>
        /// <value>The type.</value>
        public System.Type FactoryType
        {
            get { return this.factoryType; }
            set { this.factoryType = value; }
        }

        /// <summary>
        /// Gets or sets the item type for which the factory works.
        /// </summary>
        /// <value>The item type for which this factory works.</value>
        public System.Type ItemType
        {
            get { return this.itemType; }
            set { this.itemType = value; }
        }

        /// <summary>
        /// Gets the registered types for a specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The first registered type (as a collection).</returns>
        public static IEnumerable<KeyValuePair<System.Type, System.Type>> GetRegisteredTypes(
            System.Reflection.Assembly assembly
        )
        {
            foreach (var attribute in assembly.GetCustomAttributes<ItemViewFactoryAttribute>())
            {
                if (attribute.FactoryType.ContainsInterface<IItemViewFactory>())
                {
                    yield return new KeyValuePair<System.Type, System.Type>(
                        attribute.FactoryType,
                        attribute.ItemType
                    );
                    break;
                }
            }
        }

        private System.Type factoryType;
        private System.Type itemType;
    }
}
