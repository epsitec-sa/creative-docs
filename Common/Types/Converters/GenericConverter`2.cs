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
    /// The <c>GenericConverter</c> class is the base class for specific type converters.
    /// Converters must derive from this class, not from <see cref="GenericConverter{T}"/>
    /// and must implement a default constructor, without any parameter.
    /// </summary>
    /// <typeparam name="T">The type on which the converter operates.</typeparam>
    /// <typeparam name="TSelf">The type of the converter itself.</typeparam>
    public abstract class GenericConverter<T, TSelf> : GenericConverter<T>
        where TSelf : GenericConverter<T, TSelf>, new()
    {
        protected GenericConverter(System.Globalization.CultureInfo culture)
            : base(culture) { }
    }
}
