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
    /// The <c>FieldSource</c> enumeration specifies how a field is
    /// defined (it is either a value or an expression).
    /// See <see cref="StructuredTypeField"/>.
    /// </summary>
    [DesignerVisible]
    public enum FieldSource : byte
    {
        /// <summary>
        /// The field holds a value.
        /// </summary>
        Value = 0,

        /// <summary>
        /// The field is the result of the evaluation of an expression.
        /// </summary>
        Expression = 1,
    }
}
