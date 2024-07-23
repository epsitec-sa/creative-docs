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
    /// The <c>IStringType</c> interface describes a text type.
    /// </summary>
    public interface IStringType : INamedType
    {
        /// <summary>
        /// Gets the minimum length for the text.
        /// </summary>
        /// <value>The minimum length.</value>
        int MinimumLength { get; }

        /// <summary>
        /// Gets the maximum length for the text.
        /// </summary>
        /// <value>The maximum length.</value>
        int MaximumLength { get; }

        /// <summary>
        /// Gets a value indicating whether the strings use fixed length storage
        /// (this information is required by the database engine, for instance).
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the strings use fixed length storage; otherwise, <c>false</c>.
        /// </value>
        bool UseFixedLengthStorage { get; }

        /// <summary>
        /// Gets a value indicating whether the strings use multilingual storage.
        /// storage.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the strings use multilingual storage; otherwise, <c>false</c>.
        /// </value>
        bool UseMultilingualStorage { get; }

        /// <summary>
        /// Gets a value indicating whether the strings store formatted text.
        /// </summary>
        /// <value><c>true</c> if the strings store formatted text; otherwise, <c>false</c>.</value>
        bool UseFormattedText { get; }

        /// <summary>
        /// Gets the default search behavior for this string type.
        /// </summary>
        /// <value>The default search behavior.</value>
        StringSearchBehavior DefaultSearchBehavior { get; }

        /// <summary>
        /// Gets the default comparison behavior for this string type.
        /// </summary>
        /// <value>The default comparison behavior.</value>
        StringComparisonBehavior DefaultComparisonBehavior { get; }
    }
}
