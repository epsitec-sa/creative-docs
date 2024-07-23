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


namespace Epsitec.Common.Types.Exceptions
{
    /// <summary>
    /// An instance of the <c>ReadOnlyException</c> should be thrown whenever an instance of
    /// <see cref="IReadOnly"/> in read only state is written to.
    /// </summary>
    public sealed class ReadOnlyException : System.InvalidOperationException
    {
        /// <summary>
        /// Builds a new instance of <c>ReadOnlyException</c>.
        /// </summary>
        /// <param name="obj">The <see cref="IReadOnly"/> object that has been written to.</param>
        /// <param name="message">The message of the exception.</param>
        public ReadOnlyException(IReadOnly obj = null, string message = null)
            : base(message ?? "")
        {
            this.readOnlyObject = obj;
        }

        /// <summary>
        /// The <see cref="IReadOnly"/> object that has been written to.
        /// </summary>
        private readonly IReadOnly readOnlyObject;
    }
}
