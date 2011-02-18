//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.Platform
{
	internal static class FileOperation
	{
		public static bool DeleteFiles(FileOperationMode mode, IEnumerable<string> files)
		{
			Platform.Win32.ShellFileOperation fo = new Win32.ShellFileOperation ();

			fo.Operation     = Win32.ShellFileOperation.FileOperations.FO_DELETE;
			fo.OperationMode = mode;
			fo.SourceFiles   = Types.Collection.ToArray (files);
			fo.DestFiles     = null;

			return fo.DoOperation ();
		}

		public static bool CopyFiles(FileOperationMode mode, IEnumerable<string> sourceFiles, IEnumerable<string> destinationFiles)
		{
			Platform.Win32.ShellFileOperation fo = new Win32.ShellFileOperation ();

			fo.Operation       = Win32.ShellFileOperation.FileOperations.FO_COPY;
			fo.OperationMode   = mode;
			fo.OperationFlags |= Win32.ShellFileOperation.ShellFileOperationFlags.FOF_MULTIDESTFILES;
			fo.SourceFiles     = Types.Collection.ToArray (sourceFiles);
			fo.DestFiles       = Types.Collection.ToArray (destinationFiles);

			return fo.DoOperation ();
		}

		public static bool CopyFilesToFolder(FileOperationMode mode, IEnumerable<string> sourceFiles, string destinationFolder)
		{
			Platform.Win32.ShellFileOperation fo = new Win32.ShellFileOperation ();

			fo.Operation       = Win32.ShellFileOperation.FileOperations.FO_COPY;
			fo.OperationMode   = mode;
			fo.OperationFlags &= ~Win32.ShellFileOperation.ShellFileOperationFlags.FOF_MULTIDESTFILES;
			fo.SourceFiles     = Types.Collection.ToArray (sourceFiles);
			fo.DestFiles       = new string[] { destinationFolder };

			return fo.DoOperation ();
		}

		public static bool MoveFiles(FileOperationMode mode, IEnumerable<string> sourceFiles, IEnumerable<string> destinationFiles)
		{
			Platform.Win32.ShellFileOperation fo = new Win32.ShellFileOperation ();

			fo.Operation       = Win32.ShellFileOperation.FileOperations.FO_MOVE;
			fo.OperationMode   = mode;
			fo.OperationFlags |= Win32.ShellFileOperation.ShellFileOperationFlags.FOF_MULTIDESTFILES;
			fo.SourceFiles     = Types.Collection.ToArray (sourceFiles);
			fo.DestFiles       = Types.Collection.ToArray (destinationFiles);

			return fo.DoOperation ();
		}

		public static bool MoveFilesToFolder(FileOperationMode mode, IEnumerable<string> sourceFiles, string destinationFolder)
		{
			Platform.Win32.ShellFileOperation fo = new Win32.ShellFileOperation ();

			fo.Operation       = Win32.ShellFileOperation.FileOperations.FO_MOVE;
			fo.OperationMode   = mode;
			fo.OperationFlags &= ~Win32.ShellFileOperation.ShellFileOperationFlags.FOF_MULTIDESTFILES;
			fo.SourceFiles     = Types.Collection.ToArray (sourceFiles);
			fo.DestFiles       = new string[] { destinationFolder };

			return fo.DoOperation ();
		}
		
		public static bool RenameFiles(FileOperationMode mode, IEnumerable<string> sourceFiles, IEnumerable<string> destinationFiles)
		{
			Platform.Win32.ShellFileOperation fo = new Win32.ShellFileOperation ();

			fo.Operation       = Win32.ShellFileOperation.FileOperations.FO_RENAME;
			fo.OperationMode   = mode;
			fo.OperationFlags |= Win32.ShellFileOperation.ShellFileOperationFlags.FOF_MULTIDESTFILES;
			fo.SourceFiles     = Types.Collection.ToArray (sourceFiles);
			fo.DestFiles       = Types.Collection.ToArray (destinationFiles);

			return fo.DoOperation ();
		}

		public static void AddToRecentDocuments(FolderItem item)
		{
			Win32.PidlHandle handle = item.Handle as Win32.PidlHandle;
			Win32.ShellApi.SHAddToRecentDocs (Win32.ShellApi.SHARD_PIDL, handle.Pidl);
		}

		public static void AddToRecentDocuments(string path)
		{
			Win32.ShellApi.SHAddToRecentDocs (Win32.ShellApi.SHARD_PATHW, path);
		}
	}
}
