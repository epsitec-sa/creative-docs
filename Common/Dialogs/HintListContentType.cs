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


namespace Epsitec.Common.Dialogs
{
    /// <summary>
    /// The <c>HintListContentType</c> enumeration specifies what content type
    /// is displayed by the hint list.
    /// </summary>
    public enum HintListContentType
    {
        /// <summary>
        /// The hint list has no defined content type; no specific header will
        /// be displayed.
        /// </summary>
        Default,

        /// <summary>
        /// The hint list displays catalog data, i.e. it is used as a browser
        /// to access a record in a data base.
        /// </summary>
        Catalog,

        /// <summary>
        /// The hint list displays suggestions, i.e. it is used as a central
        /// auto complete system.
        /// </summary>
        Suggestions
    }
}
