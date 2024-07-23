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
    /// The <c>IIsDisposed</c> interface provides the <c>IsDisposed</c> property in addition of the
    /// regular <see cref="System.IDisposable"/> members.
    /// </summary>
    public interface IIsDisposed : System.IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether this instance was disposed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance was disposed; otherwise, <c>false</c>.
        /// </value>
        bool IsDisposed { get; }
    }
}
