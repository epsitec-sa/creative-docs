//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	public static class FileManager
	{
		/// <summary>
		/// Deletes the file. This sends the file to the recycle bin.
		/// </summary>
		/// <param name="file">The fully qualified file path.</param>
		public static void DeleteFile(string file)
		{
			FileManager.DeleteFiles (file);
		}

		/// <summary>
		/// Deletes the files. This sends the files to the recycle bin.
		/// </summary>
		/// <param name="file">The fully qualified file paths.</param>
		public static void DeleteFiles(params string[] files)
		{
			IEnumerable<string> enumFiles = files;
			FileManager.DeleteFiles (enumFiles);
		}

		/// <summary>
		/// Deletes the files. This sends the files to the recycle bin.
		/// </summary>
		/// <param name="file">The fully qualified file paths.</param>
		public static void DeleteFiles(IEnumerable<string> files)
		{
			Platform.Win32.ShellFileOperation fo = new Epsitec.Common.Support.Platform.Win32.ShellFileOperation ();
			
			fo.Operation = Epsitec.Common.Support.Platform.Win32.ShellFileOperation.FileOperations.FO_DELETE;
			fo.OperationFlags = Epsitec.Common.Support.Platform.Win32.ShellFileOperation.ShellFileOperationFlags.FOF_ALLOWUNDO
			/**/			  | Epsitec.Common.Support.Platform.Win32.ShellFileOperation.ShellFileOperationFlags.FOF_WANTNUKEWARNING;
			fo.SourceFiles = Types.Collection.ToArray (files);
			fo.DoOperation ();
		}

		/// <summary>
		/// Gets the folder item for a special folder (such as the desktop,
		/// for instance).
		/// </summary>
		/// <param name="file">The special folder identifier.</param>
		/// <param name="mode">The details retrieval mode.</param>
		/// <returns>A valid folder item or <c>FolderItem.Empty</c>.</returns>
		public static FolderItem GetFolderItem(FolderId file, FolderQueryMode mode)
		{
			return Platform.FileInfo.CreateFolderItem (file, mode);
		}

		/// <summary>
		/// Gets the folder item for a given path (which must exist).
		/// </summary>
		/// <param name="path">The fully qualified path.</param>
		/// <param name="mode">The details retrieval mode.</param>
		/// <returns>A valid folder item or <c>FolderItem.Empty</c>.</returns>
		public static FolderItem GetFolderItem(string path, FolderQueryMode mode)
		{
			return Platform.FileInfo.CreateFolderItem (path, mode);
		}

		/// <summary>
		/// Enumerates the items found in the specified folder.
		/// </summary>
		/// <param name="path">The folder.</param>
		/// <param name="mode">The details retrieval mode.</param>
		/// <returns>An enumeration of folder items.</returns>
		public static IEnumerable<FolderItem> GetFolderItems(FolderItem path, FolderQueryMode mode)
		{
			return Platform.FileInfo.GetFolderItems (path, mode);
		}

		/// <summary>
		/// Enumerates the items found at the specified path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="mode">The details retrieval mode.</param>
		/// <returns>An enumeration of folder items.</returns>
		public static IEnumerable<FolderItem> GetFolderItems(string path, FolderQueryMode mode)
		{
			return Platform.FileInfo.GetFolderItems (Platform.FileInfo.CreateFolderItem (path, FolderQueryMode.NoIcons), mode);
		}
		
		public static FolderItem GetParentFolderItem(FolderItem path, FolderQueryMode mode)
		{
			return Platform.FileInfo.GetParentFolderItem (path, mode);
		}
	}
}
