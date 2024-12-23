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
    internal static class FileOperation
    {
        public static bool DeleteFiles(FileOperationMode mode, IEnumerable<string> files)
        {
            /*
            Platform.Win32.ShellFileOperation fo = new Win32.ShellFileOperation();

            fo.Operation = Win32.ShellFileOperation.FileOperations.FO_DELETE;
            fo.OperationMode = mode;
            fo.SourceFiles = Types.Collection.ToArray(files);
            fo.DestFiles = null;

            return fo.DoOperation();
            */
            throw new System.NotImplementedException();
        }

        public static bool CopyFiles(
            FileOperationMode mode,
            IEnumerable<string> sourceFiles,
            IEnumerable<string> destinationFiles
        )
        {
            /*
            Platform.Win32.ShellFileOperation fo = new Win32.ShellFileOperation();

            fo.Operation = Win32.ShellFileOperation.FileOperations.FO_COPY;
            fo.OperationMode = mode;
            fo.OperationFlags |= Win32
                .ShellFileOperation
                .ShellFileOperationFlags
                .FOF_MULTIDESTFILES;
            fo.SourceFiles = Types.Collection.ToArray(sourceFiles);
            fo.DestFiles = Types.Collection.ToArray(destinationFiles);

            return fo.DoOperation();
            */
            throw new System.NotImplementedException();
        }

        public static bool CopyFilesToFolder(
            FileOperationMode mode,
            IEnumerable<string> sourceFiles,
            string destinationFolder
        )
        {
            /*
            Platform.Win32.ShellFileOperation fo = new Win32.ShellFileOperation();

            fo.Operation = Win32.ShellFileOperation.FileOperations.FO_COPY;
            fo.OperationMode = mode;
            fo.OperationFlags &= ~Win32
                .ShellFileOperation
                .ShellFileOperationFlags
                .FOF_MULTIDESTFILES;
            fo.SourceFiles = Types.Collection.ToArray(sourceFiles);
            fo.DestFiles = new string[] { destinationFolder };

            return fo.DoOperation();
            */
            throw new System.NotImplementedException();
        }

        public static bool MoveFiles(
            FileOperationMode mode,
            IEnumerable<string> sourceFiles,
            IEnumerable<string> destinationFiles
        )
        {
            /*
            Platform.Win32.ShellFileOperation fo = new Win32.ShellFileOperation();

            fo.Operation = Win32.ShellFileOperation.FileOperations.FO_MOVE;
            fo.OperationMode = mode;
            fo.OperationFlags |= Win32
                .ShellFileOperation
                .ShellFileOperationFlags
                .FOF_MULTIDESTFILES;
            fo.SourceFiles = Types.Collection.ToArray(sourceFiles);
            fo.DestFiles = Types.Collection.ToArray(destinationFiles);

            return fo.DoOperation();
            */
            throw new System.NotImplementedException();
        }

        public static bool MoveFilesToFolder(
            FileOperationMode mode,
            IEnumerable<string> sourceFiles,
            string destinationFolder
        )
        {
            /*
            Platform.Win32.ShellFileOperation fo = new Win32.ShellFileOperation();

            fo.Operation = Win32.ShellFileOperation.FileOperations.FO_MOVE;
            fo.OperationMode = mode;
            fo.OperationFlags &= ~Win32
                .ShellFileOperation
                .ShellFileOperationFlags
                .FOF_MULTIDESTFILES;
            fo.SourceFiles = Types.Collection.ToArray(sourceFiles);
            fo.DestFiles = new string[] { destinationFolder };

            return fo.DoOperation();
            */
            throw new System.NotImplementedException();
        }

        public static bool RenameFiles(
            FileOperationMode mode,
            IEnumerable<string> sourceFiles,
            IEnumerable<string> destinationFiles
        )
        {
            /*
            Platform.Win32.ShellFileOperation fo = new Win32.ShellFileOperation();

            fo.Operation = Win32.ShellFileOperation.FileOperations.FO_RENAME;
            fo.OperationMode = mode;
            fo.OperationFlags |= Win32
                .ShellFileOperation
                .ShellFileOperationFlags
                .FOF_MULTIDESTFILES;
            fo.SourceFiles = Types.Collection.ToArray(sourceFiles);
            fo.DestFiles = Types.Collection.ToArray(destinationFiles);

            return fo.DoOperation();
            */
            throw new System.NotImplementedException();
        }

        public static void AddToRecentDocuments(FolderItem item)
        {
            /*
            Win32.PidlHandle handle = item.Handle as Win32.PidlHandle;
            Win32.ShellApi.SHAddToRecentDocs(Win32.ShellApi.SHARD_PIDL, handle.Pidl);
            */
            throw new System.NotImplementedException();
        }

        public static void AddToRecentDocuments(string path)
        {
            /*
            Win32.ShellApi.SHAddToRecentDocs(Win32.ShellApi.SHARD_PATHW, path);
            */
            throw new System.NotImplementedException();
        }
    }
}
