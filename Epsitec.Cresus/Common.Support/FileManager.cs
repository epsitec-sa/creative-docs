using System;
using System.Collections.Generic;
using System.Text;

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

		public static FolderItem CreateFolderItem(SystemFileId file, FolderDetailsMode mode)
		{
			return Platform.FileInfo.CreateFolderItem (file, mode);
		}

		public static FolderItem CreateFolderItem(string path, FolderDetailsMode mode)
		{
			return Platform.FileInfo.CreateFolderItem (path, mode);
		}

		public static IEnumerable<FolderItem> GetFolderItems(FolderItem path, FolderDetailsMode mode)
		{
			return Platform.FileInfo.GetFolderItems (path, mode);
		}

		public static IEnumerable<FolderItem> GetFolderItems(string path, FolderDetailsMode mode)
		{
			return Platform.FileInfo.GetFolderItems (Platform.FileInfo.CreateFolderItem (path, FolderDetailsMode.NoIcons), mode);
		}
	}
}
