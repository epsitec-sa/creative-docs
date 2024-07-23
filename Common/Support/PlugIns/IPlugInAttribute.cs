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


namespace Epsitec.Common.Support.PlugIns
{
    /// <summary>
    /// The <c>IPlugInAttribute</c> generic interface must be implemented by the
    /// attribute classes which interact with the <see cref="PlugInFactory"/>.
    /// </summary>
    /// <typeparam name="TId">The type of the id.</typeparam>
    public interface IPlugInAttribute<TId>
    {
        /// <summary>
        /// Gets the id of the class described by this attribute.
        /// </summary>
        /// <value>The id.</value>
        TId Id { get; }

        /// <summary>
        /// Gets the type of the class described by this attribute.
        /// </summary>
        /// <value>The type.</value>
        System.Type Type { get; }
    }
}
