//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>FileManager</c> class provides complex file operations which are
	/// handled by the Operating System file manager (on Windows, this is known
	/// as the "Shell").
	/// </summary>
	public static class FileManager
	{
		static FileManager()
		{
			System.Threading.ThreadPool.QueueUserWorkItem (
				delegate (object arg)
				{
					Platform.Win32.FileInfo.InitializeWellKnownFolderItems ();
				});
		}

		/// <summary>
		/// Initializes the file manager.
		/// </summary>
		public static void Initialize()
		{
		}

		/// <summary>
		/// Deletes the file. This sends the file to the recycle bin.
		/// </summary>
		/// <param name="mode">The file operation mode.</param>
		/// <param name="file">The fully qualified file path.</param>
		/// <returns>Returns <c>true</c> on success.</returns>
		public static bool DeleteFile(FileOperationMode mode, string file)
		{
			return FileManager.DeleteFiles (mode, file);
		}

		/// <summary>
		/// Deletes the files. This sends the files to the recycle bin.
		/// </summary>
		/// <param name="mode">The file operation mode.</param>
		/// <param name="files">The fully qualified file paths.</param>
		/// <returns>Returns <c>true</c> on success.</returns>
		public static bool DeleteFiles(FileOperationMode mode, params string[] files)
		{
			IEnumerable<string> enumFiles = files;
			return FileManager.DeleteFiles (mode, enumFiles);
		}

		/// <summary>
		/// Deletes the files. This sends the files to the recycle bin.
		/// </summary>
		/// <param name="mode">The file operation mode.</param>
		/// <param name="files">The fully qualified file paths.</param>
		/// <returns>Returns <c>true</c> on success.</returns>
		public static bool DeleteFiles(FileOperationMode mode, IEnumerable<string> files)
		{
			return Platform.FileOperation.DeleteFiles (mode, files);
		}


		/// <summary>
		/// Moves the file from the source to the destination.
		/// </summary>
		/// <param name="mode">The file operation mode.</param>
		/// <param name="source">The fully qualified source file path.</param>
		/// <param name="destination">The fully qualified destination file path.</param>
		/// <returns>Returns <c>true</c> on success.</returns>
		public static bool MoveFile(FileOperationMode mode, string source, string destination)
		{
			return FileManager.MoveFiles (mode, new string[] { source }, new string[] { destination });
		}

		/// <summary>
		/// Moves the file from the source to the destination.
		/// </summary>
		/// <param name="mode">The file operation mode.</param>
		/// <param name="source">The fully qualified source file paths.</param>
		/// <param name="destination">The fully qualified destination file paths.</param>
		/// <returns>Returns <c>true</c> on success.</returns>
		public static bool MoveFiles(FileOperationMode mode, IEnumerable<string> source, IEnumerable<string> destination)
		{
			return Platform.FileOperation.MoveFiles (mode, source, destination);
		}

		/// <summary>
		/// Moves the file from the source to the destination folder.
		/// </summary>
		/// <param name="mode">The file operation mode.</param>
		/// <param name="source">The fully qualified source file paths.</param>
		/// <param name="destination">The fully qualified destination folder path.</param>
		/// <returns>Returns <c>true</c> on success.</returns>
		public static bool MoveFilesToFolder(FileOperationMode mode, IEnumerable<string> source, string destinationFolder)
		{
			return Platform.FileOperation.MoveFilesToFolder (mode, source, destinationFolder);
		}

		/// <summary>
		/// Copy file from the source to the destination.
		/// </summary>
		/// <param name="mode">The file operation mode.</param>
		/// <param name="source">The fully qualified source file path.</param>
		/// <param name="destination">The fully qualified destination file path.</param>
		/// <returns>Returns <c>true</c> on success.</returns>
		public static bool CopyFile(FileOperationMode mode, string source, string destination)
		{
			return FileManager.CopyFiles (mode, new string[] { source }, new string[] { destination });
		}

		/// <summary>
		/// Copy files from the source to the destination.
		/// </summary>
		/// <param name="mode">The file operation mode.</param>
		/// <param name="source">The fully qualified source file paths.</param>
		/// <param name="destination">The fully qualified destination file paths.</param>
		/// <returns>Returns <c>true</c> on success.</returns>
		public static bool CopyFiles(FileOperationMode mode, IEnumerable<string> source, IEnumerable<string> destination)
		{
			return Platform.FileOperation.CopyFiles (mode, source, destination);
		}

		/// <summary>
		/// Copy files from the source to the destination folder.
		/// </summary>
		/// <param name="mode">The file operation mode.</param>
		/// <param name="source">The fully qualified source file paths.</param>
		/// <param name="destination">The fully qualified destination folder path.</param>
		/// <returns>Returns <c>true</c> on success.</returns>
		public static bool CopyFilesToFolder(FileOperationMode mode, IEnumerable<string> source, string destinationFolder)
		{
			return Platform.FileOperation.CopyFilesToFolder (mode, source, destinationFolder);
		}

		/// <summary>
		/// Rename file from the source to the destination.
		/// </summary>
		/// <param name="mode">The file operation mode.</param>
		/// <param name="source">The fully qualified source file path.</param>
		/// <param name="destination">The fully qualified destination file path.</param>
		/// <returns>Returns <c>true</c> on success.</returns>
		public static bool RenameFile(FileOperationMode mode, string source, string destination)
		{
			if (string.Equals (source, destination, System.StringComparison.CurrentCulture))
			{
				return false;
			}
			else
			{
				return Platform.FileOperation.RenameFiles (mode, new string[] { source }, new string[] { destination });
			}
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
		/// Gets the icon for the specified folder item.
		/// </summary>
		/// <param name="item">The folder item.</param>
		/// <param name="mode">The folder query mode.</param>
		/// <returns>The icon for the folder item or <c>null</c>.</returns>
		public static FolderItemIcon GetFolderItemIcon(FolderItem item, FolderQueryMode mode)
		{
			return Platform.FileInfo.CreateFolderItem (item.Handle, mode).Icon;
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

			return Platform.FileInfo.GetFolderItems (path, mode, null);
		}

		/// <summary>
		/// Enumerates the items found in the specified folder, applying the
		/// specified filter before returing possible folder items.
		/// </summary>
		/// <param name="path">The folder.</param>
		/// <param name="mode">The details retrieval mode.</param>
		/// <returns>An enumeration of folder items.</returns>
		public static IEnumerable<FolderItem> GetFolderItems(FolderItem path, FolderQueryMode mode, System.Predicate<FileFilterInfo> filter)
		{
			if (path.IsEmpty)
			{
				throw new System.ArgumentException ("Empty FolderItem provided");
			}

			return Platform.FileInfo.GetFolderItems (path, mode, filter);
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
				return FileManager.GetFolderItems (Platform.FileInfo.CreateFolderItem (path, mode), mode);
			}
			catch (System.IO.FileNotFoundException)
			{
				throw new System.IO.FileNotFoundException (string.Format ("File {0} does not exist", path), path);
			}
		}

		/// <summary>
		/// Gets the parent folder item.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="mode">The mode.</param>
		/// <returns>The parent folder item or <c>FolderItem.Empty</c>.</returns>
		public static FolderItem GetParentFolderItem(FolderItem path, FolderQueryMode mode)
		{
			return Platform.FileInfo.GetParentFolderItem (path, mode);
		}

		/// <summary>
		/// Adds the file to the recent documents.
		/// </summary>
		/// <param name="path">The file path.</param>
		public static void AddToRecentDocuments(string path)
		{
			Platform.FileOperation.AddToRecentDocuments (path);
		}

		/// <summary>
		/// Adds the file to the recent documents.
		/// </summary>
		/// <param name="path">The file path.</param>
		public static void AddToRecentDocuments(FolderItem path)
		{
			Platform.FileOperation.AddToRecentDocuments (path);
		}

		/// <summary>
		/// Resolves the shortcut.
		/// </summary>
		/// <param name="path">The shortcut path.</param>
		/// <param name="mode">The mode.</param>
		/// <returns>The folder item for the target of the shortcut or <c>FolderItem.Empty</c>.</returns>
		public static FolderItem ResolveShortcut(string path, FolderQueryMode mode)
		{
			return Platform.Shortcut.Resolve (path, mode);
		}

		/// <summary>
		/// Resolves the shortcut.
		/// </summary>
		/// <param name="path">The shortcut path.</param>
		/// <param name="mode">The mode.</param>
		/// <returns>The folder item for the target of the shortcut or <c>FolderItem.Empty</c>.</returns>
		public static FolderItem ResolveShortcut(FolderItem path, FolderQueryMode mode)
		{
			return Platform.Shortcut.Resolve (path.FullPath, mode);
		}
	}
}
