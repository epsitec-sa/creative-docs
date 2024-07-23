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
    /// The <c>CodeVisibility</c> enumeration lists all possible code access
    /// visibilities defined by the C# language.
    /// </summary>
    public enum CodeVisibility : byte
    {
        /// <summary>
        /// The code has no specific access visibility.
        /// </summary>
        None,

        /// <summary>
        /// The code item is declared <c>public</c>.
        /// </summary>
        Public,

        /// <summary>
        /// The code item is declared <c>internal</c>.
        /// </summary>
        Internal,

        /// <summary>
        /// The code item is declared <c>protected</c>.
        /// </summary>
        Protected,

        /// <summary>
        /// The code item is declared <c>private</c>.
        /// </summary>
        Private
    }
}
