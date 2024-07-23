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

namespace Epsitec.Common.UI
{
    /// <summary>
    /// The <c>Verbosity</c> enumeration defines different verbosity modes
    /// used by the <see cref="Placeholder"/> to represent its labels.
    /// </summary>
    [DesignerVisible]
    public enum Verbosity
    {
        /// <summary>
        /// No text.
        /// </summary>
        None,

        /// <summary>
        /// Shortest text or label.
        /// </summary>
        Compact,

        /// <summary>
        /// Default text or label length.
        /// </summary>
        Default,

        /// <summary>
        /// Longest text or label.
        /// </summary>
        Verbose,
    }
}
