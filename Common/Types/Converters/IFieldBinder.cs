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
    /// The <c>IFieldBinder</c> interface provides conversion and validation
    /// methods used by the UI binding code, when reading/writing data stored in
    /// entity fields.
    /// </summary>
    public interface IFieldBinder
    {
        /// <summary>
        /// Converts the string from its raw representation to the UI representation.
        /// </summary>
        /// <param name="value">The raw value, as returned by the marshaler.</param>
        /// <returns>The value which should be used for edition.</returns>
        FormattedText ConvertToUI(FormattedText value);

        /// <summary>
        /// Converts the string from its UI representation to the raw representation.
        /// </summary>
        /// <param name="value">The UI value.</param>
        /// <returns>The value which should be handed back to the marshaler.</returns>
        FormattedText ConvertFromUI(FormattedText value);

        /// <summary>
        /// Validates the data from the UI. The data format is expected to be OK; this
        /// method can be used to apply additional validation rules.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The validation result.</returns>
        IValidationResult ValidateFromUI(FormattedText value);

        /// <summary>
        /// Attaches the field binder to the specified marshaler. This can be used to
        /// configure the marshaler's converter in order to filter and preprocess the
        /// data at the marshaler level.
        /// </summary>
        /// <param name="marshaler">The marshaler.</param>
        void Attach(Marshaler marshaler);
    }
}
