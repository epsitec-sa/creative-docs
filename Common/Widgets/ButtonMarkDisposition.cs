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


namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// The <c>ButtonMarkDisposition</c> enumeration defines if and where a mark
    /// should be painted relative to the button box. The mark is usually a small
    /// triangle pointing to the outside of the box.
    /// </summary>
    public enum ButtonMarkDisposition
    {
        /// <summary>
        /// No mark.
        /// </summary>
        None,

        /// <summary>
        /// Mark on the left hand side of the button.
        /// </summary>
        Left,

        /// <summary>
        /// Mark on the right hand side of the button.
        /// </summary>
        Right,

        /// <summary>
        /// Mark above the button.
        /// </summary>
        Above,

        /// <summary>
        /// Mark below the button.
        /// </summary>
        Below,
    }
}
