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
    /// The <c>IStructuredType</c> interface makes the metadata about a structured
    /// tree accessible.
    /// </summary>
    public interface IStructuredType
    {
        /// <summary>
        /// Gets a collection of field identifiers.
        /// </summary>
        /// <returns>A collection of field identifiers.</returns>
        IEnumerable<string> GetFieldIds();

        /// <summary>
        /// Gets the field descriptor for the specified field identifier.
        /// </summary>
        /// <param name="fieldId">The field identifier.</param>
        /// <returns>The matching field descriptor; otherwise, <c>null</c>.</returns>
        StructuredTypeField GetField(string fieldId);

        /// <summary>
        /// Gets the structured type class for this instance. The default is
        /// simply <c>StructuredTypeClass.None</c>.
        /// </summary>
        /// <returns>The structured type class to which this instance belongs.</returns>
        StructuredTypeClass GetClass();
    }
}
