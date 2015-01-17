//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.IO
{
	public sealed class ByteStream : System.IO.Stream
	{
		public ByteStream(long length, IEnumerable<byte> source)
		{
			this.length = length;
			this.source = source;
			
			this.enumerator = this.source.GetEnumerator ();
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
				return false;
			}
		}
		public override bool					CanTimeout
		{
			get
			{
				return false;
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
				return this.length;
			}
		}
		public override long					Position
		{
			get
			{
				return this.pos;
			}
			set
			{
				throw new System.NotImplementedException ();
			}
		}
		
		public override void Flush()
		{
		}

		public override long Seek(long offset, System.IO.SeekOrigin origin)
		{
			throw new System.NotImplementedException ();
		}

		public override void SetLength(long value)
		{
			throw new System.NotImplementedException ();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new System.NotImplementedException ();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int read = 0;

			while ((count > 0) && (this.enumerator.MoveNext ()))
			{
				buffer[offset++] = this.enumerator.Current;
				this.pos++;
				count--;
				read++;
			}

			return read;
		}

		public override void Close()
		{
			base.Close ();
			this.enumerator.Dispose ();
		}

		private readonly long					length;
		private long							pos;
		private readonly IEnumerable<byte>		source;
		private readonly IEnumerator<byte>		enumerator;
	}
}
