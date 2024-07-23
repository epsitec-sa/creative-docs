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


namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>FileOperationMode</c> class is used to customize the file operations
    /// supported by the <see cref="FileManager"/> class.
    /// </summary>
    public sealed class FileOperationMode
    {
        public FileOperationMode() { }

        public FileOperationMode(Platform.IFileOperationWindow ownerWindow)
            : this()
        {
            this.ownerWindow = ownerWindow;
        }

        public Platform.IFileOperationWindow OwnerWindow
        {
            get { return this.ownerWindow; }
            set { this.ownerWindow = value; }
        }

        public bool Silent
        {
            get { return this.silent; }
            set { this.silent = value; }
        }

        public bool AutoRenameOnCollision
        {
            get { return this.autoRenameOnCollision; }
            set { this.autoRenameOnCollision = value; }
        }

        public bool AutoConfirmation
        {
            get { return this.autoConfirmation; }
            set { this.autoConfirmation = value; }
        }

        public bool AutoCreateDirectory
        {
            get { return this.autoCreateDirectory; }
            set { this.autoCreateDirectory = value; }
        }

        private bool silent;
        private bool autoRenameOnCollision;
        private bool autoConfirmation;
        private bool autoCreateDirectory;
        private Platform.IFileOperationWindow ownerWindow;
    }
}
