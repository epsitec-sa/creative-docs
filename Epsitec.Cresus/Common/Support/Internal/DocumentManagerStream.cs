//	Copyright © 2007-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.Internal
{
	/// <summary>
	/// The <c>DocumentManagerStream</c> implements a stream which can be used
	/// to seamlessly read a document while it is still being copied from its
	/// source location to the temporary folder.
	/// </summary>
	internal sealed class DocumentManagerStream : System.IO.Stream
	{
		public DocumentManagerStream(DocumentManager manager, string path)
		{
			this.manager = manager;
			this.stream = new System.IO.FileStream (path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
		}

		
		public override bool					CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool					CanSeek
		{
			get
			{
				return true;
			}
		}

		public override bool					CanWrite
		{
			get
			{
				return false;
			}
		}

		public override long					Length
		{
			get
			{
				return this.manager.SourceLength;
			}
		}

		public override long					Position
		{
			get
			{
				return this.stream.Position;
			}
			set
			{
				this.Seek (value, System.IO.SeekOrigin.Begin);
			}
		}


		public override void Close()
		{
			this.stream.Close ();
			base.Close ();
		}
		
		public override void Flush()
		{
		}

		public override long Seek(long offset, System.IO.SeekOrigin origin)
		{
			long pos = -1;

			switch (origin)
			{
				case System.IO.SeekOrigin.Begin:
					pos = offset;
					break;

				case System.IO.SeekOrigin.Current:
					pos = this.Position + offset;
					break;

				case System.IO.SeekOrigin.End:
					pos = this.Length - offset;
					break;

				default:
					throw new System.InvalidOperationException ();
			}

			this.manager.WaitForLocalCopyLength ((int) pos, -1);

			return this.stream.Seek (pos, System.IO.SeekOrigin.Begin);
		}

		public override void SetLength(long value)
		{
			throw new System.NotImplementedException ("The method or operation is not implemented.");
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.manager.WaitForLocalCopyLength (this.Position + count, -1);

			return this.stream.Read (buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new System.NotImplementedException ("The method or operation is not implemented.");
		}

		private readonly System.IO.Stream		stream;
		private readonly DocumentManager		manager;
	}
}
