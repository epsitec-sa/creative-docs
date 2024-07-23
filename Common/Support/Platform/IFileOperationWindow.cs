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


namespace Epsitec.Common.Support.Platform
{
    /// <summary>
    /// The <c>IFileOperationWindow</c> interface is used to get access to the
    /// platform window handle, given a window. This interface is required as
    /// we cannot add a reference to <c>Epsitec.Common.Widgets</c>, yet we must
    /// be able to get data from it.
    /// </summary>
    public interface IFileOperationWindow
    {
        /// <summary>
        /// Gets the platform window handle.
        /// </summary>
        /// <returns>The platform window handle</returns>
        System.IntPtr GetPlatformHandle();
    }
}
