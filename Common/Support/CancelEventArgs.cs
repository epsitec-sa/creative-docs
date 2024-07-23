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


namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>CancelEventArgs</c> class provides data for a cancelable event.
    /// </summary>
    public class CancelEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelEventArgs"/> class.
        /// </summary>
        public CancelEventArgs() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelEventArgs"/> class.
        /// </summary>
        /// <param name="cancel">The default value of the <see cref="Cancel"/> property.</param>
        public CancelEventArgs(bool cancel)
        {
            this.Cancel = cancel;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the action associated with the event
        /// should be canceled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the event should be canceled; otherwise, <c>false</c>.
        /// </value>
        public bool Cancel { get; set; }
    }
}
