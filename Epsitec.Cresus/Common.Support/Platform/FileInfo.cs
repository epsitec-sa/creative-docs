//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.Platform
{
	public static class FileInfo
	{
		public static FolderItem CreateFolderItem(FolderId file, FolderDetailsMode mode)
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
