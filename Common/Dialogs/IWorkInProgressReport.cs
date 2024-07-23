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
    /// The <c>IWorkInProgressReport</c> interface is used by a worker thread
    /// to interact with a <see cref="WorkInProgressDialog"/> dialog.
    /// </summary>
    public interface IWorkInProgressReport
    {
        /// <summary>
        /// Defines the operation which is currently being executed.
        /// </summary>
        /// <param name="formattedText">The formatted text.</param>
        void DefineOperation(string formattedText);

        /// <summary>
        /// Defines the progress of the operation, both using a value between
        /// 0 and 1 and a text message.
        /// </summary>
        /// <param name="value">The value between 0 and 1.</param>
        /// <param name="formattedText">The formatted text.</param>
        void DefineProgress(double value, string formattedText);

        /// <summary>
        /// Gets a value indicating whether the operation should be canceled.
        /// This will be true if the user pressed the cancel button.
        /// </summary>
        /// <value><c>true</c> if canceled; otherwise, <c>false</c>.</value>
        bool Canceled { get; }
    }
}
