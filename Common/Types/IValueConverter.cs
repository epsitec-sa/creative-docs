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
    /// The <c>IValueConverter</c> class  is used by the binding mechanisms to
    /// convert between a data source and a target, and vice versa.
    /// </summary>
    public interface IValueConverter
    {
        /// <summary>
        /// Converts the specified value to the expected type. This method is
        /// used when the information flows from the source to the target.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="expectedType">The expected type for the output value.</param>
        /// <param name="parameter">An optional parameter.</param>
        /// <param name="culture">The culture to use for the conversion.</param>
        /// <returns>The converted value, or <see cref="InvalidValue.Value"/>
        /// if the value cannot be converted to the expected type.</returns>
        object Convert(
            object value,
            System.Type expectedType,
            object parameter,
            System.Globalization.CultureInfo culture
        );

        /// <summary>
        /// Converts the specified value back to the expected type. This method
        /// is used when the information flows from the target to the source.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="expectedType">The expected type for the output value.</param>
        /// <param name="parameter">An optional parameter.</param>
        /// <param name="culture">The culture to use for the conversion.</param>
        /// <returns>The converted value, or <see cref="InvalidValue.Value"/>
        /// if the value cannot be converted to the expected type.</returns>
        object ConvertBack(
            object value,
            System.Type expectedType,
            object parameter,
            System.Globalization.CultureInfo culture
        );
    }
}
