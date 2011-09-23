//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus
{
	public sealed class ChunkReader
	{
		public ChunkReader(System.IO.Stream stream)
		{
			this.stream = stream;
			this.buffer = new byte[64*1024];
		}

		
		public IEnumerable<Chunk> GetChunks()
		{
			this.stream.Position = 0;
			this.available       = 0;
			this.offset          = 0;

			while (true)
			{
				int?  length     = this.GetNextInt ();
				byte? headerType = this.GetNextByte ();

				if (length == null)
				{
					yield break;
				}

				if ((headerType == null) ||
					(headerType.Value != 0))
				{
					throw new System.FormatException ("Invalid header type found");
				}

				var guid = this.GetNextGuid ();
				var name = this.GetNextString ();
				var data = new byte[length.Value];

				this.GetNextBytes (data);

				yield return new Chunk (guid, name, data);
			}
		}

		
		private System.Guid GetNextGuid()
		{
			byte[] data = new byte[16];
			this.GetNextBytes (data);
			return new System.Guid (data);
		}

		private string GetNextString()
		{
			List<byte> bytes = new List<byte> ();

			while (true)
			{
				byte? c = this.GetNextByte ();

				if (c == null)
				{
					throw new System.FormatException ("Unterminated string");
				}
				if (c.Value == 0)
				{
					break;
				}

				bytes.Add (c.Value);
			}

			return System.Text.Encoding.UTF8.GetString (bytes.ToArray ());
		}

		private byte? GetNextByte()
		{
			if (this.available == 0)
			{
				int read = this.stream.Read (this.buffer, 0, this.buffer.Length);

				if (read < 1)
				{
					return null;
				}

				this.available = read;
				this.offset    = 0;
			}

			byte data = this.buffer[this.offset];

			this.offset++;
			this.available--;

			return data;
		}

		private int? GetNextInt()
		{
			byte? hh = this.GetNextByte ();
			byte? mh = this.GetNextByte ();
			byte? ml = this.GetNextByte ();
			byte? ll = this.GetNextByte ();

			if ((hh == null) || (mh == null) || (ml == null) || (ll == null))
			{
				return null;
			}

			return (hh.Value << 24) + (mh.Value << 16) + (ml.Value << 8) + (ll.Value);
		}

		private void GetNextBytes(byte[] data)
		{
			int dataOffset = 0;

			if (this.available > 0)
			{
				int copy = System.Math.Min (data.Length, this.available);

				System.Array.Copy (this.buffer, this.offset, data, 0, copy);

				this.offset    += copy;
				this.available -= copy;

				if (copy == data.Length)
				{
					return;
				}

				dataOffset = copy;
			}

			int read = this.stream.Read (data, dataOffset, data.Length - dataOffset);

			if (read != data.Length - dataOffset)
			{
				throw new System.FormatException ("Unexpected end of stream while reading binary data");
			}
		}

		
		private readonly System.IO.Stream		stream;
		private readonly byte[]					buffer;

		private int								available;
		private int								offset;
	}
}