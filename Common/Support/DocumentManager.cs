//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types.Collections;

namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>DocumentManager</c> class reads and writes documents to disk
    /// </summary>
    public class DocumentManager : System.IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentManager"/> class.
        /// </summary>
        public DocumentManager()
        {
            lock (DocumentManager.managers)
            {
                managers.Add(this);
            }
        }

        ~DocumentManager()
        {
            this.Dispose(false);
        }

        public void Associate(string extension, GetDocumentInfoCallback callback)
        {
            extension = DocumentManager.GetCleanExtension(extension);

            lock (this.associations)
            {
                this.associations[extension] = callback;
            }
        }

        private static string GetCleanExtension(string extension)
        {
            if (extension == null)
            {
                return null;
            }

            if (extension.StartsWith("."))
            {
                extension = extension.Substring(1);
            }

            extension = extension.ToLowerInvariant();
            return extension;
        }

        /// <summary>
        /// Gets a value indicating whether this document is open or not.
        /// </summary>
        /// <value><c>true</c> if this document is open; otherwise, <c>false</c>.</value>
        public bool IsOpen
        {
            get { return string.IsNullOrEmpty(this.sourcePath) ? false : true; }
        }

        /// <summary>
        /// Opens the specified file and creates a local copy to work on.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        public void Open(string path)
        {
            this.sourcePath = path;
        }

        /// <summary>
        /// Closes this document. This deletes the local copy of the document file
        /// created by the <see cref="Open"/> method.
        /// </summary>
        public void Close()
        {
            this.sourcePath = null;
        }

        /// <summary>
        /// Saves the document to the specified path. The caller must provide a
        /// callback which is responsible for writing data into the output stream.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="callback">The save callback.</param>
        /// <returns><c>true</c> if the save callback returned <c>true</c>; otherwise, <c>false</c>.</returns>
        public bool Save(string path, SaveCallback callback)
        {
            string tempPath = string.Concat(path, ".tmp");

            if (System.IO.File.Exists(tempPath))
            {
                System.IO.File.Delete(tempPath);
            }

            try
            {
                using (
                    System.IO.Stream stream = System.IO.File.Open(
                        tempPath,
                        System.IO.FileMode.CreateNew,
                        System.IO.FileAccess.ReadWrite,
                        System.IO.FileShare.None
                    )
                )
                {
                    if (callback(stream) == false)
                    {
                        return false;
                    }
                }

                //	TODO: re-encrypt output file if the original was encrypted ?

                System.IO.File.Delete(path);
                System.IO.File.Move(tempPath, path);
            }
            finally
            {
                try
                {
                    if (System.IO.File.Exists(tempPath))
                    {
                        System.IO.File.Delete(tempPath);
                    }
                }
                catch { }
            }

            return true;
        }

        /// <summary>
        /// Gets a stream for the open file.
        /// </summary>
        /// <param name="access">The access.</param>
        /// <returns>The stream.</returns>
        public System.IO.Stream GetSourceFileStream(System.IO.FileAccess access)
        {
            switch (access)
            {
                case System.IO.FileAccess.Read:
                    return new Internal.DocumentManagerStream(this, this.sourcePath);

                case System.IO.FileAccess.ReadWrite:
                case System.IO.FileAccess.Write:
                    return new System.IO.FileStream(
                        this.sourcePath,
                        System.IO.FileMode.Open,
                        access,
                        System.IO.FileShare.ReadWrite
                    );
            }

            return null;
        }

        /// <summary>
        /// Gets the path to the local file.
        /// </summary>
        /// <returns>The path to the local file.</returns>
        public string GetSourceFilePath()
        {
            return this.sourcePath;
        }

        /// <summary>
        /// Finds the <c>GetDocumentInfoCallback</c> associated with the specified
        /// file extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>The <c>GetDocumentInfoCallback</c> if the extension is known; otherwise, <c>null</c>.</returns>
        public static GetDocumentInfoCallback FindAssociation(string extension)
        {
            extension = DocumentManager.GetCleanExtension(extension);

            if (string.IsNullOrEmpty(extension))
            {
                return null;
            }

            lock (DocumentManager.managers)
            {
                foreach (DocumentManager manager in DocumentManager.managers)
                {
                    GetDocumentInfoCallback callback = manager.FindLocalAssociation(extension);

                    if (callback != null)
                    {
                        return callback;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the document information for the specified file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>The document information or null.</returns>
        public static IDocumentInfo GetDocumentInfo(string path)
        {
            string extension = System.IO.Path.GetExtension(path);
            GetDocumentInfoCallback callback = DocumentManager.FindAssociation(extension);

            if (callback == null)
            {
                return null;
            }
            else
            {
                return callback(path);
            }
        }

        private GetDocumentInfoCallback FindLocalAssociation(string cleanExtension)
        {
            lock (this.associations)
            {
                GetDocumentInfoCallback callback;
                this.associations.TryGetValue(cleanExtension, out callback);
                return callback;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Close();
                managers.Remove(this);
            }
        }

        public delegate bool SaveCallback(System.IO.Stream stream);
        public delegate IDocumentInfo GetDocumentInfoCallback(string path);

        private static WeakList<DocumentManager> managers = new WeakList<DocumentManager>();

        private string sourcePath;
        private Dictionary<string, GetDocumentInfoCallback> associations =
            new Dictionary<string, GetDocumentInfoCallback>();
    }
}
