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
    /// The <c>IFieldPropertyStore</c> interface gives access to field
    /// properties.
    /// </summary>
    public interface IFieldPropertyStore
    {
        /// <summary>
        /// Gets the value associated with the property of the specified field.
        /// </summary>
        /// <param name="fieldId">The field id.</param>
        /// <param name="property">The property.</param>
        /// <returns>The value or <c>UndefinedValue.Value</c> if it is not
        /// defined.</returns>
        object GetValue(string fieldId, DependencyProperty property);

        /// <summary>
        /// Sets the value associated with the property of the specified field.
        /// </summary>
        /// <param name="fieldId">The field id.</param>
        /// <param name="property">The property.</param>
        /// <param name="value">The value to set.</param>
        void SetValue(string fieldId, DependencyProperty property, object value);

        /// <summary>
        /// Determines whether the specified field contains a value for the
        /// specified property.
        /// </summary>
        /// <param name="fieldId">The field id.</param>
        /// <param name="property">The property.</param>
        /// <returns>
        /// 	<c>true</c> if the specified field contains a value for the
        /// 	specified property; otherwise, <c>false</c>.
        /// </returns>
        bool ContainsValue(string fieldId, DependencyProperty property);
    }
}
