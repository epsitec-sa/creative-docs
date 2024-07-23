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
    /// The <c>DataSourceType</c> enumeration describes the binding type used
    /// by <see cref="T:BindingExpression"/> to attach to the data source.
    /// </summary>
    public enum DataSourceType : byte
    {
        /// <summary>
        /// The data source is not defined.
        /// </summary>
        None,

        /// <summary>
        /// The data source is defined as a <c>DependencyProperty</c> on a <c>DependencyObject</c>.
        /// </summary>
        PropertyObject,

        /// <summary>
        /// The data source is defined as a field on a <c>IStructuredData</c> object.
        /// </summary>
        StructuredData,

        /// <summary>
        /// The data source is defined as the binding source itself.
        /// </summary>
        SourceItself,

        /// <summary>
        /// The data source is defined as a resource provided by <c>IResourceProvider</c>.
        /// </summary>
        Resource
    }
}
