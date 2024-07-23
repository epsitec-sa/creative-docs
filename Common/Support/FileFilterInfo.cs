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
    public struct FileFilterInfo
    {
        public FileFilterInfo(string path, System.IO.FileAttributes attributes)
        {
            this.path = path;
            this.extension = string.IsNullOrEmpty(path)
                ? null
                : System.IO.Path.GetExtension(path).ToLowerInvariant();
            this.attributes = attributes;

            if ((path.Length == 3) && (path[1] == ':'))
            {
                this.attributes |= System.IO.FileAttributes.Directory;
            }
        }

        public string Path
        {
            get { return this.path; }
        }

        public string LowerCaseExtension
        {
            get { return this.extension; }
        }

        public System.IO.FileAttributes Attributes
        {
            get { return this.attributes; }
        }

        public bool IsEmpty
        {
            get { return this.path == null; }
        }

        public static readonly FileFilterInfo Empty = new FileFilterInfo();

        private string path;
        private string extension;
        private System.IO.FileAttributes attributes;
    }
}
