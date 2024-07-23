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


using Epsitec.Common.Types;

namespace Epsitec.Common.Support.ResourceAccessors
{
    /// <summary>
    /// The <c>DataCreationMode</c> enumeration defines the possible modes
    /// used by the <c>FillDataFromCaption</c> method, when creating a
    /// <see cref="StructuredData"/> instance.
    /// </summary>
    public enum DataCreationMode
    {
        /// <summary>
        /// Creates a public data record, with attached event handlers.
        /// </summary>
        Public,

        /// <summary>
        /// Creates a lightweight data record, which should just be used
        /// to store temporary data. No event handlers get created for it
        /// and the data may be simply disposed of without any side effects.
        /// </summary>
        Temporary
    }
}
