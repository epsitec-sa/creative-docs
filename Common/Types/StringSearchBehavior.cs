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
    /// The <c>StringSearchBehavior</c> enumeration defines how a string value
    /// should be matched against a reference string.
    /// </summary>
    [DesignerVisible]
    public enum StringSearchBehavior
    {
        /// <summary>
        /// Searches for exact matches.
        /// </summary>
        ExactMatch,

        /// <summary>
        /// Searches for matches using a simple wildcard pattern with "*" and
        /// "?" behaving in the standard way.
        /// </summary>
        WildcardMatch,

        /// <summary>
        /// Searches for matches where both value and reference strings start
        /// with the same text.
        /// </summary>
        MatchStart,

        /// <summary>
        /// Searches for matches where both value and reference strings end
        /// with the same text.
        /// </summary>
        MatchEnd,

        /// <summary>
        /// Searches for matches where the string value is contained anywhere
        /// in the reference string.
        /// </summary>
        MatchAnywhere,
    }
}
