//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 15/04/2004

namespace Epsitec.Common.Support.IO
{
	/// <summary>
	/// Summary description for AbstractStream.
	/// </summary>
	public abstract class AbstractStream : System.IO.Stream
	{
		public AbstractStream()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		
		
		public System.IO.Stream					Stream
		{
			get
			{
				return this.stream;
			}
			set
			{
				if (this.stream != value)
				{
					this.stream = value;
				}
			}
		}
		
		
		public override bool					CanRead
		{
			get
			{
				return this.stream.CanRead;
			}
		}
		
		public override bool					CanWrite
		{
			get
			{
				return this.stream.CanWrite;
			}
		}

		public override bool					CanSeek
		{
			get
			{
				return this.stream.CanSeek;
			}
		}

		public override long					Length
		{
			get
			{
				return this.stream.Length;
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
				this.stream.Position = value;
			}
		}


		public override System.IAsyncResult BeginRead(byte[] buffer, int offset, int count, System.AsyncCallback callback, object state)
		{
			return this.stream.BeginRead (buffer, offset, count, callback, state);
		}

		public override System.IAsyncResult BeginWrite(byte[] buffer, int offset, int count, System.AsyncCallback callback, object state)
		{
			return this.stream.BeginWrite (buffer, offset, count, callback, state);
		}
		
		public override int  EndRead(System.IAsyncResult asyncResult)
		{
			return this.stream.EndRead (asyncResult);
		}
		
		public override void EndWrite(System.IAsyncResult asyncResult)
		{
			this.stream.EndWrite (asyncResult);
		}

		
		public override void Close()
		{
			this.stream.Close ();
		}

		public override void Flush()
		{
			this.stream.Flush ();
		}
		
		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.stream.Read (buffer, offset, count);
		}
		
		public override int ReadByte()
		{
			return this.stream.ReadByte ();
		}
		
		public override void Write(byte[] buffer, int offset, int count)
		{
			this.stream.Write (buffer, offset, count);
		}

		public override void WriteByte(byte value)
		{
			this.stream.WriteByte (value);
		}

		public override long Seek(long offset, System.IO.SeekOrigin origin)
		{
			return this.stream.Seek (offset, origin);
		}
		
		public override void SetLength(long value)
		{
			this.stream.SetLength (value);
		}
		
		
		private System.IO.Stream				stream;
	}
}
