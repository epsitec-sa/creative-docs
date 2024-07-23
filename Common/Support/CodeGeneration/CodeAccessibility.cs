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


namespace Epsitec.Common.Support.CodeGeneration
{
    /// <summary>
    /// The <c>CodeAccessibility</c> enumeration lists all possible code
    /// accessibility types (such as <c>abstract</c>, <c>virtual</c> or
    /// <c>static</c>).
    /// </summary>
    public enum CodeAccessibility : byte
    {
        /// <summary>
        /// The code item has its default accessibility.
        /// </summary>
        Default,

        /// <summary>
        /// The code item is declared final, which means it cannot be overridden
        /// by any other method or property.
        /// </summary>
        Final,

        /// <summary>
        /// The code item is declared <c>abstract</c>.
        /// </summary>
        Abstract,

        /// <summary>
        /// The code item is declared <c>virtual</c>.
        /// </summary>
        Virtual,

        /// <summary>
        /// The code item is declared overridden (with the <c>override</c> keyword).
        /// </summary>
        Override,

        /// <summary>
        /// The code item is declared <c>static</c>.
        /// </summary>
        Static,

        /// <summary>
        /// The code item is declared constant (with the <c>const</c> keyword).
        /// </summary>
        Constant,

        /// <summary>
        /// The code item is declared <c>sealed</c>. This only applies to classes.
        /// </summary>
        Sealed,
    }
}
