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
    /// The <c>DialogDataMode</c> enumeration defines the modes which can be
    /// used with the data represented by <see cref="DialogData"/>.
    /// </summary>
    public enum DialogDataMode
    {
        /// <summary>
        /// The isolated mode does not reflect data changes until the dialog
        /// is validated.
        /// </summary>
        Isolated,

        /// <summary>
        /// The real-time mode updates the data on the fly, as it is being
        /// edited in the dialog.
        /// </summary>
        RealTime,

        /// <summary>
        /// The transparent mode does not track changes and binds the dialog
        /// directly to the provided data. Use this if the user interface must
        /// reflect external changes in the provided data.
        /// </summary>
        Transparent,

        /// <summary>
        /// The search mode considers all data typed in by the user as defining
        /// a search template; the original data will never be altered.
        /// </summary>
        Search,
    }
}
