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


using Epsitec.Common.Widgets;

namespace Epsitec.Common.Dialogs
{
    /// <summary>
    /// The <c>IDialog</c> interface must be implemented by every dialog.
    /// </summary>
    public interface IDialog
    {
        /// <summary>
        /// Opens the dialog (creates the window and makes it visible). If the
        /// dialog is modal, this method won't return until the user closes
        /// the dialog.
        /// </summary>
        void OpenDialog(Window owner = null);

        /// <summary>
        /// Gets or sets the owner window for this dialog.
        /// </summary>
        /// <value>The owner window.</value>
        Window OwnerWindow { get; set; }

        /// <summary>
        /// Gets the dialog result.
        /// </summary>
        /// <value>The dialog result.</value>
        DialogResult Result { get; }
    }
}
