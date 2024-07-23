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


namespace Epsitec.Common.Types.Converters
{
    /// <summary>
    /// The <c>IFieldBinderProvider</c> interface tries to find a <see cref="IFieldBinder"/> for
    /// the specified <see cref="INamedType"/>.
    /// </summary>
    public interface IFieldBinderProvider
    {
        /// <summary>
        /// Gets the field binder for the specified named type.
        /// </summary>
        /// <param name="namedType">The named type.</param>
        /// <returns>The field binder if one can be found; otherwise, <c>null</c>.</returns>
        IFieldBinder GetFieldBinder(INamedType namedType);
    }
}
