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
    /// The <c>HintListVisibilityMode</c> enumeration specifies how and when the
    /// hint list will be displayed.
    /// </summary>
    public enum HintListVisibilityMode
    {
        /// <summary>
        /// The hint list is always visible.
        /// </summary>
        Visible,

        /// <summary>
        /// The hint list is always invisible.
        /// </summary>
        Invisible,

        /// <summary>
        /// The hint list pops up when it is required and it disappears as soon
        /// as it is no longer needed.
        /// </summary>
        AutoHide
    }
}
