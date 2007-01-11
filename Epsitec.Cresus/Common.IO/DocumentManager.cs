//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.IO
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
		}

		~DocumentManager()
		{
			this.Dispose (false);
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
			this.sourceStream = new System.IO.FileStream (path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
			this.sourceLength = this.sourceStream.Length;
			this.localCopyPath = System.IO.Path.GetTempFileName ();
			this.localCopyStream = new System.IO.FileStream (this.localCopyPath, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite, System.IO.FileShare.Read);
			this.copyThread = new System.Threading.Thread (this.CopyThread);
			this.copyThread.Start ();
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




		private void CopyThread()
		{
			byte[] buffer = new byte[1*1024];

			while (true)
			{
				int count = this.sourceStream.Read (buffer, 0, buffer.Length);

				if (count == 0)
				{
					break;
				}

				this.localCopyStream.Write (buffer, 0, count);
				
				System.Threading.Interlocked.Add (ref this.localCopyWritten, count);
				System.Threading.Thread.Sleep (1);

				this.localCopyWait.Signal ();
			}

			this.sourceStream.Close ();
			this.localCopyStream.Close ();

			this.localCopyReady = true;
			this.localCopyWait.Signal ();
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
			this.CloseSourceStream ();
			this.CloseLocalCopyStream ();
			this.DeleteLocalCopy ();
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

		private object exclusion = new object ();
		private string localCopyPath;
		private System.Threading.Thread copyThread;
		private System.IO.FileStream sourceStream;
		private System.IO.FileStream localCopyStream;
		private long sourceLength;
		private int localCopyWritten;
		private bool localCopyReady;
		private WaitCondition localCopyWait = new WaitCondition ();
	}
}
