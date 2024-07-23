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


namespace Epsitec.Common.Document
{
    /// <summary>
    /// The <c>ImageFileReference</c> class is used to reference a data file, which may
    /// be automatically deleted when it is no longer needed.
    /// </summary>
    public class ImageFileReference : System.IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFileReference"/> class,
        /// pointing to an existing file.
        /// </summary>
        /// <param name="path">The path.</param>
        public ImageFileReference(string path)
        {
            this.path = path;
        }

        ~ImageFileReference()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageFileReference"/> class,
        /// pointing to a temporary file, created on the fly based on the provided data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="entryName">Name of the entry.</param>
        public ImageFileReference(byte[] data, string entryName)
        {
            string ext = string.IsNullOrEmpty(entryName)
                ? ""
                : System.IO.Path.GetExtension(entryName);
            string dir = System.IO.Path.GetTempPath();
            string name = "pdf-export-" + System.Guid.NewGuid().ToString("D");

            this.path = System.IO.Path.Combine(dir, name + ext);
            this.deleteOnDispose = true;

            //	Save the data to a temporary file, which will be deleted when this
            //	reference is disposed.

            System.IO.File.WriteAllBytes(this.path, data);
        }

        /// <summary>
        /// Gets the path to the file.
        /// </summary>
        public string Path
        {
            get { return this.path; }
        }

        public void Dispose()
        {
            System.GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (
                (this.deleteOnDispose)
                && (!this.disposeExecuted)
                && (System.IO.File.Exists(this.path))
            )
            {
                this.disposeExecuted = true;
                System.IO.File.Delete(this.path);
            }
        }

        private readonly string path;
        private readonly bool deleteOnDispose;
        private bool disposeExecuted;
    }
}
