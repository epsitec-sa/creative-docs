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
    /// The <c>TaskletRunMode</c> defines how <see cref="TaskletJob"/> jobs have to
    /// be executed within a <see cref="Tasklet"/> batch (before/within/after).
    /// </summary>
    public enum TaskletRunMode
    {
        /// <summary>
        /// Synchronous execution -- reserved for internal use only.
        /// </summary>
        Sync,

        /// <summary>
        /// Asynchronous execution; the job is part of the main body of the
        /// batch.
        /// </summary>
        Async,

        /// <summary>
        /// Synchronous execution before the main body of the batch starts.
        /// </summary>
        Before,

        /// <summary>
        /// Synchronous execution before the main body of the batch starts;
        /// and then also asynchronous execution after the main body of the
        /// batch ends.
        /// </summary>
        BeforeAndAfter,

        /// <summary>
        /// Asynchronous execution after the main body of the batch ends.
        /// </summary>
        After,
    }
}
