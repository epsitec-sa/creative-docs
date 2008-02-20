//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>DocumentManager</c> class copies documents from their source
	/// location to a local temporary storage on <c>Open</c> and copies them
	/// back to the source location on <c>Save</c>.
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
				managers.Add (this);
			}
		}

		~DocumentManager()
		{
			this.Dispose (false);
		}

		public void Associate(string extension, GetDocumentInfoCallback callback)
		{
			extension = DocumentManager.GetCleanExtension (extension);
			
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

			if (extension.StartsWith ("."))
			{
				extension = extension.Substring (1);
			}

			extension = extension.ToLowerInvariant ();
			return extension;
		}

		/// <summary>
		/// Gets a value indicating whether this document is open or not.
		/// </summary>
		/// <value><c>true</c> if this document is open; otherwise, <c>false</c>.</value>
		public bool IsOpen
		{
			get
			{
				return string.IsNullOrEmpty (this.sourcePath) ? false : true;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the local copy of the document is ready.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the local copy of the document is ready; otherwise, <c>false</c>.
		/// </value>
		public bool IsLocalCopyReady
		{
			get
			{
				return this.localCopyReady;
			}
		}

		/// <summary>
		/// Gets the length of the local copy. This value can change while the
		/// file gets copied.
		/// </summary>
		/// <value>The length of the local copy.</value>
		public long LocalCopyLength
		{
			get
			{
				return this.localCopyWritten;
			}
		}

		/// <summary>
		/// Gets the length of the source file.
		/// </summary>
		/// <value>The length of the source.</value>
		public long SourceLength
		{
			get
			{
				return this.sourceLength;
			}
		}


		/// <summary>
		/// Opens the specified file and creates a local copy to work on.
		/// </summary>
		/// <param name="path">The path to the file.</param>
		public void Open(string path)
		{
			this.sourcePath = path;
			this.sourceStream = new System.IO.FileStream (path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
			this.sourceLength = this.sourceStream.Length;
			this.localCopyPath = System.IO.Path.GetTempFileName ();
			this.localCopyStream = new System.IO.FileStream (this.localCopyPath, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite, System.IO.FileShare.Read);
			this.copyThread = new System.Threading.Thread (this.CopyThread);
			this.copyThread.Start ();
		}

		/// <summary>
		/// Closes this document. This deletes the local copy of the document file
		/// created by the <see cref="Open"/> method.
		/// </summary>
		public void Close()
		{
			if (this.copyThread != null)
			{
				this.copyThreadAbortRequest = true;
				this.copyThread.Join ();
				this.copyThread = null;
			}

			this.DeleteLocalCopy ();

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
			if (this.IsOpen)
			{
				this.WaitForLocalCopyReady (-1);
			}

			string tempPath = string.Concat (path, ".tmp");

			if (System.IO.File.Exists (tempPath))
			{
				System.IO.File.Delete (tempPath);
			}
			
			try
			{
				using (System.IO.Stream stream = System.IO.File.Open (tempPath, System.IO.FileMode.CreateNew, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None))
				{
					if (callback (stream) == false)
					{
						return false;
					}
				}

				//	TODO: re-encrypt output file if the original was encrypted ?

				System.IO.File.Delete (path);
				System.IO.File.Move (tempPath, path);
				
			}
			finally
			{
				try
				{
					if (System.IO.File.Exists (tempPath))
					{
						System.IO.File.Delete (tempPath);
					}
				}
				catch
				{
				}
			}

			return true;
		}

		/// <summary>
		/// Gets a stream for the local copy of the open file. If read access
		/// is requested, then the call returns immediately, even if the copy
		/// is not finished yet. The stream itself will ensure that the reader
		/// does not see that the file is still being written to.
		/// </summary>
		/// <param name="access">The access.</param>
		/// <returns>The stream.</returns>
		public System.IO.Stream GetLocalFileStream(System.IO.FileAccess access)
		{
			switch (access)
			{
				case System.IO.FileAccess.Read:
					return new Internal.DocumentManagerStream (this, this.localCopyPath);

				case System.IO.FileAccess.ReadWrite:
				case System.IO.FileAccess.Write:
					this.WaitForLocalCopyReady (-1);
					return new System.IO.FileStream (this.localCopyPath, System.IO.FileMode.Open, access, System.IO.FileShare.ReadWrite);
			}

			return null;
		}

		/// <summary>
		/// Gets the path to the local file.
		/// </summary>
		/// <returns>The path to the local file.</returns>
		public string GetLocalFilePath()
		{
			return this.localCopyPath;
		}

		/// <summary>
		/// Waits for the local copy to become ready.
		/// </summary>
		/// <param name="timeout">The timeout.</param>
		/// <returns><c>true</c> if the local copy is ready; <c>false</c> otherwise.</returns>
		public bool WaitForLocalCopyReady(int timeout)
		{
			return this.localCopyWait.Wait (
				delegate ()
				{
					return this.localCopyReady;
				},
				timeout);
		}

		/// <summary>
		/// Waits for the local copy to reach the specified length.
		/// </summary>
		/// <param name="minimumLength">The minimum length.</param>
		/// <param name="timeout">The timeout.</param>
		/// <returns><c>true</c> if the local copy has reached the expected length; <c>false</c> otherwise.</returns>
		public bool WaitForLocalCopyLength(long minimumLength, int timeout)
		{
			this.localCopyWait.Wait (
				delegate ()
				{
					return this.localCopyReady || (this.localCopyWritten >= minimumLength);
				},
				timeout);

			return (this.localCopyWritten >= minimumLength);
		}


		/// <summary>
		/// Finds the <c>GetDocumentInfoCallback</c> associated with the specified
		/// file extension.
		/// </summary>
		/// <param name="extension">The extension.</param>
		/// <returns>The <c>GetDocumentInfoCallback</c> if the extension is known; otherwise, <c>null</c>.</returns>
		public static GetDocumentInfoCallback FindAssociation(string extension)
		{
			extension = DocumentManager.GetCleanExtension (extension);

			if (string.IsNullOrEmpty (extension))
			{
				return null;
			}

			lock (DocumentManager.managers)
			{
				foreach (DocumentManager manager in DocumentManager.managers)
				{
					GetDocumentInfoCallback callback = manager.FindLocalAssociation (extension);
					
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
			string extension = System.IO.Path.GetExtension (path);
			GetDocumentInfoCallback callback = DocumentManager.FindAssociation (extension);
			
			if (callback == null)
			{
				return null;
			}
			else
			{
				return callback (path);
			}
		}

		private GetDocumentInfoCallback FindLocalAssociation(string cleanExtension)
		{
			lock (this.associations)
			{
				GetDocumentInfoCallback callback;
				this.associations.TryGetValue (cleanExtension, out callback);
				return callback;
			}
		}


		private void CopyThread()
		{
			System.Diagnostics.Debug.WriteLine ("Copying from '" + this.sourcePath + "' to '" + this.localCopyPath + "'");
			byte[] buffer = new byte[64*1024];

			while (true)
			{
				int count = this.sourceStream.Read (buffer, 0, buffer.Length);

				if ((count == 0) ||
					(this.copyThreadAbortRequest))
				{
					break;
				}

				this.localCopyStream.Write (buffer, 0, count);
				this.localCopyStream.Flush ();
				
				System.Threading.Interlocked.Add (ref this.localCopyWritten, count);

				this.localCopyWait.Signal ();
			}

			lock (this.exclusion)
			{
				this.CloseSourceStream ();
				this.CloseLocalCopyStream ();
			}

			this.localCopyReady = true;
			this.localCopyWait.Signal ();

			System.Diagnostics.Debug.WriteLine ("Copy done : " + this.localCopyWritten.ToString ());
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		#endregion

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Close ();
				
				lock (DocumentManager.managers)
				{
					managers.Remove (this);
				}
			}

			lock (this.exclusion)
			{
				this.CloseSourceStream ();
				this.CloseLocalCopyStream ();
				this.DeleteLocalCopy ();
			}
		}

		private void CloseSourceStream()
		{
			if (this.sourceStream != null)
			{
				this.sourceStream.Close ();
				this.sourceStream = null;
			}
		}

		private void CloseLocalCopyStream()
		{
			if (this.localCopyStream != null)
			{
				this.localCopyStream.Close ();
				this.localCopyStream = null;
			}
		}

		private void DeleteLocalCopy()
		{
			if (this.localCopyPath != null)
			{
				try
				{
					System.IO.File.Delete (this.localCopyPath);
					this.localCopyPath = null;
				}
				catch (System.IO.IOException)
				{
				}
			}
		}

		public delegate bool SaveCallback(System.IO.Stream stream);
		public delegate IDocumentInfo GetDocumentInfoCallback(string path);

		private static WeakList<DocumentManager> managers = new WeakList<DocumentManager> ();

		private readonly object exclusion = new object ();
		private string sourcePath;
		private string localCopyPath;
		private System.Threading.Thread copyThread;
		private bool copyThreadAbortRequest;
		private System.IO.FileStream sourceStream;
		private System.IO.FileStream localCopyStream;
		private long sourceLength;
		private int localCopyWritten;
		private bool localCopyReady;
		private WaitCondition localCopyWait = new WaitCondition ();
		private Dictionary<string, GetDocumentInfoCallback> associations = new Dictionary<string, GetDocumentInfoCallback> ();
	}
}
