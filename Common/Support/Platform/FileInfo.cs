//	Copyright © 2006-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.Platform
{
    public static class FileInfo
    {
        // ********************************************************************
        // TODO bl-net8-cross
        // - implement FileInfo (stub)
        // ********************************************************************
        public static FolderItem CreateFolderItem(FolderId file, FolderQueryMode mode)
        {
            /*
            return Win32.FileInfo.CreateFolderItem(file, mode);
            */
            return FolderItem.Empty;
        }

        public static FolderItem CreateFolderItem(string path, FolderQueryMode mode)
        {
            /*
            return Win32.FileInfo.CreateFolderItem(path, mode);
            */
            return FolderItem.Empty;
        }

        internal static FolderItem CreateFolderItem(FolderItemHandle handle, FolderQueryMode mode)
        {
            /*
            return Win32.FileInfo.CreateFolderItem(handle, mode);
            */
            return FolderItem.Empty;
        }

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
            return null;
        }

        public static FolderItem GetParentFolderItem(FolderItem path, FolderQueryMode mode)
        {
            /*
            return Win32.FileInfo.GetParentFolderItem(path, mode);
            */
            return FolderItem.Empty;
        }
    }
}
