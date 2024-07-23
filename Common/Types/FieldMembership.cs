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
    /// The <c>FieldMembership</c> enumeration specifies where a field is
    /// originally defined. Do not change the values, as they are used in
    /// the serialization of <see cref="StructuredTypeField"/>.
    /// </summary>
    [DesignerVisible]
    public enum FieldMembership : byte
    {
        /// <summary>
        /// The field is defined locally.
        /// </summary>
        Local = 0,

        /// <summary>
        /// The field is inherited from some parent or base class.
        /// </summary>
        Inherited = 1,

        /// <summary>
        /// The field is redefined locally (e.g. override of an expression
        /// defined by a locally included interface).
        /// </summary>
        LocalOverride = 2,
    }
}
