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
    /// The <c>IEnumValue</c> interface describes values defined by enumerations.
    /// </summary>
    public interface IEnumValue : ICaption, IName
    {
        /// <summary>
        /// Gets the <see cref="System.Enum"/> value of the enumeration value.
        /// </summary>
        /// <value>The enumeration value.</value>
        System.Enum Value { get; }

        /// <summary>
        /// Gets the rank of the enumeration value. See<see cref="T:RankAttribute"/>
        /// attribute.
        /// </summary>
        /// <value>The rank of the enumeration value.</value>
        int Rank { get; }

        /// <summary>
        /// Gets a value indicating whether this enumeration value is hidden. See
        /// <see cref="T:HiddenAttribute"/> attribute.
        /// </summary>
        /// <value><c>true</c> if this enumeration value is hidden; otherwise,
        /// <c>false</c>.</value>
        bool IsHidden { get; }
    }
}
