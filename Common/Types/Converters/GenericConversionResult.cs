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
    /// The <c>GenericConversionResult</c> class stores a conversion result. See
    /// also method <see cref="GenericConverter{T}.ConvertFromString"/> and class
    /// <see cref="ConversionResult{T}"/>.
    /// </summary>
    public abstract class GenericConversionResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the result is null.
        /// </summary>
        /// <value><c>true</c> if the result is null; otherwise, <c>false</c>.</value>
        public bool IsNull { get; set; }

        /// <summary>
        /// Gets a value indicating whether the result has a valid, non-null value.
        /// </summary>
        /// <value><c>true</c> if this instance has a valid, non-null value; otherwise, <c>false</c>.</value>
        public bool HasValue
        {
            get { return !this.IsNull && !this.IsInvalid; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the result is invalid.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the result is invalid; otherwise, <c>false</c>.
        /// </value>
        public bool IsInvalid { get; set; }

        /// <summary>
        /// Gets a value indicating whether the result is valid.
        /// </summary>
        /// <value><c>true</c> if the result is valid; otherwise, <c>false</c>.</value>
        public bool IsValid
        {
            get { return !this.IsInvalid; }
        }
    }
}
