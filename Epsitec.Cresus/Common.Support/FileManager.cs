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
		public static bool DeleteFile(FileOperationMode mode, string file)
		{
			return FileManager.DeleteFiles (mode, file);
		}

		/// <summary>
		/// Deletes the files. This sends the files to the recycle bin.
		/// </summary>
		/// <param name="file">The fully qualified file paths.</param>
		public static bool DeleteFiles(FileOperationMode mode, params string[] files)
		{
			IEnumerable<string> enumFiles = files;
			return FileManager.DeleteFiles (mode, enumFiles);
		}

		/// <summary>
		/// Deletes the files. This sends the files to the recycle bin.
		/// </summary>
		/// <param name="file">The fully qualified file paths.</param>
		public static bool DeleteFiles(FileOperationMode mode, IEnumerable<string> files)
		{
			return Platform.FileOperation.DeleteFiles (mode, files);
		}


		public static bool MoveFile(FileOperationMode mode, string source, string destination)
		{
			return FileManager.MoveFiles (mode, new string[] { source }, new string[] { destination });
		}

		public static bool MoveFiles(FileOperationMode mode, IEnumerable<string> source, IEnumerable<string> destination)
		{
			return Platform.FileOperation.MoveFiles (mode, source, destination);
		}

		public static bool MoveFilesToFolder(FileOperationMode mode, IEnumerable<string> source, string destinationFolder)
		{
			return Platform.FileOperation.MoveFilesToFolder (mode, source, destinationFolder);
		}

		public static bool CopyFile(FileOperationMode mode, string source, string destination)
		{
			return FileManager.CopyFiles (mode, new string[] { source }, new string[] { destination });
		}

		public static bool CopyFiles(FileOperationMode mode, IEnumerable<string> source, IEnumerable<string> destination)
		{
			return Platform.FileOperation.CopyFiles (mode, source, destination);
		}

		public static bool CopyFilesToFolder(FileOperationMode mode, IEnumerable<string> source, string destinationFolder)
		{
			return Platform.FileOperation.CopyFilesToFolder (mode, source, destinationFolder);
		}

		public static bool RenameFile(FileOperationMode mode, string source, string destination)
		{
			return Platform.FileOperation.RenameFiles (mode, new string[] { source }, new string[] { destination });
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
			if (path.IsEmpty)
			{
				throw new System.ArgumentException ("Empty FolderItem provided");
			}

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
			try
			{
				return Platform.FileInfo.GetFolderItems (Platform.FileInfo.CreateFolderItem (path, mode), mode);
			}
			catch (System.IO.FileNotFoundException)
			{
				throw new System.IO.FileNotFoundException (string.Format ("File {0} does not exist", path), path);
			}
		}
		
		public static FolderItem GetParentFolderItem(FolderItem path, FolderQueryMode mode)
		{
			return Platform.FileInfo.GetParentFolderItem (path, mode);
		}

		public static void AddToRecentDocuments(string path)
		{
			Platform.FileOperation.AddToRecentDocuments (path);
		}

		public static void AddToRecentDocuments(FolderItem path)
		{
			Platform.FileOperation.AddToRecentDocuments (path);
		}
	}
}
