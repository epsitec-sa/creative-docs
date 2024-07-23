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


using System.Collections.Generic;

namespace Epsitec.Common.Types
{
    /// <summary>
    /// The <c>ITextFormatterConverter</c> interface is implemented by the pretty printing
    /// classes, which provide a textual representation for some simple type; this is for
    /// instance used to convert <c>enum</c> values to their descriptions.
    /// </summary>
    public interface ITextFormatterConverter
    {
        /// <summary>
        /// Gets the collection of all types which can be converted by this
        /// pretty printer.
        /// </summary>
        /// <returns>The collection of all convertible types.</returns>
        IEnumerable<System.Type> GetConvertibleTypes();

        /// <summary>
        /// Converts the value to <see cref="FormattedText"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="detailLevel">The detail level.</param>
        /// <returns>The formatted text if the value is not null; otherwise, <c>FormattedText.Empty</c>.</returns>
        FormattedText ToFormattedText(
            object value,
            System.Globalization.CultureInfo culture,
            TextFormatterDetailLevel detailLevel
        );
    }
}
