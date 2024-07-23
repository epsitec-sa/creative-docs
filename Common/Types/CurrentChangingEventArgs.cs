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
    /// The <c>CurrentChangingEventArgs</c> class provides information for the
    /// <c>CurrentChanging</c> event.
    /// </summary>
    public class CurrentChangingEventArgs : Support.CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentChangingEventArgs"/> class,
        /// defaulting to a cancelable event.
        /// </summary>
        public CurrentChangingEventArgs()
            : this(true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentChangingEventArgs"/> class.
        /// </summary>
        /// <param name="isCancelable">if set to <c>true</c>, the event is cancelable.</param>
        public CurrentChangingEventArgs(bool isCancelable)
        {
            this.isCancelable = isCancelable;
        }

        /// <summary>
        /// Gets a value indicating whether the event is cancelable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the event is cancelable; otherwise, <c>false</c>.
        /// </value>
        public bool IsCancelable
        {
            get { return this.isCancelable; }
        }

        private bool isCancelable;
    }
}
