using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Support.Platform
{
	public static class FileInfo
	{
		public static FolderItem CreateFolderItem(SystemFileId file, FolderDetailsMode mode)
		{
			return Win32.FileInfo.CreateFolderItem (file, mode);
		}
		
		public static FolderItem CreateFolderItem(string path, FolderDetailsMode mode)
		{
			return Win32.FileInfo.CreateFolderItem (path, mode);
		}

		public static IEnumerable<FolderItem> GetFolderItems(FolderItem path, FolderDetailsMode mode)
		{
			return Win32.FileInfo.GetFolderItems (path, mode);
		}
	}
}
