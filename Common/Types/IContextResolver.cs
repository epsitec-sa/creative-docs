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
    /// The <c>IContextResolver</c> can be used to map between objects and markup
    /// tags. This interface is implemented by <c>Serialization.Context</c>.
    /// </summary>
    public interface IContextResolver
    {
        /// <summary>
        /// Resolves the object to its corresponding markup.
        /// </summary>
        /// <param name="value">The object to convert to a markup string.</param>
        /// <returns>The markup string.</returns>
        string ResolveToMarkup(object value);

        /// <summary>
        /// Resolves the object from its corresponding markup.
        /// </summary>
        /// <param name="markup">The markup string to convert to an object.</param>
        /// <param name="type">The expected type.</param>
        /// <returns>The object.</returns>
        object ResolveFromMarkup(string markup, System.Type type);

        /// <summary>
        /// Gets the external map used to map referemces to external objects.
        /// </summary>
        /// <value>The external map.</value>
        Serialization.Generic.MapTag<object> ExternalMap { get; }
    }
}
