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


[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.Types.DataObject))]

namespace Epsitec.Common.Types
{
    /// <summary>
    /// The <c>DataObject</c> is only used as a support for the <c>DataContext</c>
    /// property.
    /// </summary>
    public abstract class DataObject : DependencyObject
    {
        /// <summary>
        /// Gets the data context (an instance of <see cref="Binding"/>) associated with an object.
        /// </summary>
        /// <param name="o">The object to query.</param>
        /// <returns>The data context.</returns>
        public static Binding GetDataContext(DependencyObject o)
        {
            return o.GetValue(DataObject.DataContextProperty) as Binding;
        }

        /// <summary>
        /// Sets the data context (an instance of <see cref="Binding"/>) associated with an object.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <param name="value">The data context.</param>
        public static void SetDataContext(DependencyObject o, Binding value)
        {
            o.SetValue(DataObject.DataContextProperty, value);
        }

        /// <summary>
        /// Clears the data context associated with an object.
        /// </summary>
        /// <param name="o">The object to clear.</param>
        public static void ClearDataContext(DependencyObject o)
        {
            o.ClearValue(DataObject.DataContextProperty);
        }

        /// <summary>
        /// Gets the collection view for a collection object in the context of
        /// a dependency object.
        /// </summary>
        /// <param name="contextHost">An object with an associated data context.</param>
        /// <param name="collection">The collection object.</param>
        /// <returns>A collection view or <c>null</c> if the collection object is not
        /// compatible with <c>ICollectionView</c>.</returns>
        public static ICollectionView GetCollectionView(
            DependencyObject contextHost,
            object collection
        )
        {
            if (contextHost == null)
            {
                throw new System.ArgumentNullException("contextHost");
            }
            if (collection == null)
            {
                throw new System.ArgumentNullException("collection");
            }

            Binding binding = DataObject.GetDataContext(contextHost);

            if (binding == null)
            {
                return null;
            }
            else
            {
                return Internal.CollectionViewResolver.Default.GetCollectionView(
                    binding,
                    collection
                );
            }
        }

        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.RegisterAttached(
                "DataContext",
                typeof(Binding),
                typeof(DataObject),
                new DependencyPropertyMetadataWithInheritance()
            );
    }
}
