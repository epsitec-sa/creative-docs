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


using System.Runtime.InteropServices;

namespace Epsitec.Common.Support.Platform.Win32
{
    /// <summary>
    /// The <c>UserAccountApi</c> class provides an interface to the Win32
    /// user account APIs.
    /// </summary>
    internal static class UserAccountApi
    {
        /// <summary>
        /// Gets a value indicating whether the user is an administrator.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the user is an administrator; otherwise, <c>false</c>.
        /// </value>
        public static bool IsUserAnAdministrator
        {
            get
            {
                try
                {
                    return UserAccountApi.IsUserAnAdmin();
                }
                catch
                {
                    return false;
                }
            }
        }

        [DllImport("shell32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool IsUserAnAdmin();
    }
}
