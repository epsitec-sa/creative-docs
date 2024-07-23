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
    /// The <c>Shortcut</c> class wraps the shortcut creation and resolution
    /// functionality.
    /// </summary>
    internal static class Shortcut
    {
        /// <summary>
        /// Resolves the shortcut defined by the specified path and returns its
        /// target <see cref="FolderItem"/>.
        /// </summary>
        /// <param name="path">The path to the shortcut file.</param>
        /// <param name="mode">The query mode.</param>
        /// <returns>The <c>FolderItem</c> if the shortcut could be resolved; otherwise, <c>null</c>.</returns>
        public static FolderItem Resolve(string path, FolderQueryMode mode)
        {
            /*
            using (Win32.ShellShortcut shortcut = new Win32.ShellShortcut(path))
            {
                FolderItemHandle handle = shortcut.TargetPidl;

                return handle == null
                    ? FolderItem.Empty
                    : FileInfo.CreateFolderItem(handle, mode);
            }
            */
            throw new System.NotImplementedException();
        }
    }
}
