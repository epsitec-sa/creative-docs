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


using System.Collections.Generic;

namespace Epsitec.Common.Support.Platform
{
    public static class FileInfo
    {
        public static FolderItem CreateFolderItem(FolderId file, FolderQueryMode mode)
        {
            /*
            return Win32.FileInfo.CreateFolderItem(file, mode);
            */
            throw new System.NotImplementedException();
            return FolderItem.Empty;
        }

        public static FolderItem CreateFolderItem(string path, FolderQueryMode mode)
        {
            return new FolderItem(path, mode);
        }

        //internal static FolderItem CreateFolderItem(FolderItemHandle handle, FolderQueryMode mode)
        //{
        //    /*
        //    return Win32.FileInfo.CreateFolderItem(handle, mode);
        //    */
        //    throw new System.NotImplementedException();
        //    return FolderItem.Empty;
        //}

        /// <summary>
        /// Returns a collection of folders within another folder defined by its path
        /// </summary>
        public static IEnumerable<FolderItem> GetFolderItems(
            FolderItem path,
            FolderQueryMode mode,
            System.Predicate<FileFilterInfo> filter
        )
        {
            /*
            return Win32.FileInfo.GetFolderItems(path, mode, filter);
            */
            throw new System.NotImplementedException();
            return null;
        }

        public static FolderItem GetParentFolderItem(FolderItem path, FolderQueryMode mode)
        {
            /*
            return Win32.FileInfo.GetParentFolderItem(path, mode);
            */
            throw new System.NotImplementedException();
            return FolderItem.Empty;
        }
    }
}
