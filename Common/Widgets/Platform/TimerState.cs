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


namespace Epsitec.Common.Widgets.Platform
{
    /// <summary>
    /// The <c>TimerState</c> enumeration lists all possible timer states.
    /// </summary>
    public enum TimerState
    {
        ///// <summary>
        ///// The timer is in an invalid state.
        ///// </summary>
        //Invalid,

        ///// <summary>
        ///// The timer has been disposed.
        ///// </summary>
        //Disposed,

        /// <summary>
        /// The timer does not run. Calling <see cref="Timer.Start"/> will start
        /// a new delay.
        /// </summary>
        Stopped,

        /// <summary>
        /// The timer is running.
        /// </summary>
        Running,

        /// <summary>
        /// The timer does not run. Calling <see cref="Timer.Start"/> will resume
        /// the previous delay.
        /// </summary>
        Suspended,

        ///// <summary>
        ///// The timer does not run. The delay has elapsed. Calling
        ///// <see cref="Timer.Start"/> will start a new delay.
        ///// </summary>
        //Elapsed
    }
}
