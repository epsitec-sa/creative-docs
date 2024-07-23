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
    /// The <c>IValidationResult</c> interface is used to retrieve a validation result.
    /// </summary>
    public interface IValidationResult
    {
        /// <summary>
        /// Gets a value indicating whether the associated widget contains valid data,
        /// ie <see cref="State"/> is set to <c>ValidationState.Ok</c>.
        /// </summary>
        /// <value><c>true</c> if the associated widget contains valid data; otherwise, <c>false</c>.</value>
        bool IsValid { get; }

        /// <summary>
        /// Gets the validation state (basically OK or not OK).
        /// </summary>
        /// <value>The validation state.</value>
        ValidationState State { get; }

        /// <summary>
        /// Gets the error message explaining what is wrong with the data.
        /// </summary>
        /// <value>The error message.</value>
        FormattedText ErrorMessage { get; }
    }
}
