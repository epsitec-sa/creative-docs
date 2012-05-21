//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus
{
	public sealed class ChunkWriter
	{
		public ChunkWriter(System.IO.Stream stream)
		{
			this.stream = stream;
		}

		public void Add(Chunk chunk)
		{
			this.Write (chunk.Data.Length);
			this.Write (new byte[1] { 0 });
			this.Write (chunk.Guid.ToByteArray ());
			this.Write (System.Text.Encoding.UTF8.GetBytes (chunk.Name));
			this.Write (new byte[1] { 0 });
			this.Write (chunk.Data);
		}

		private void Write(int value)
		{
			byte[] num = new byte[4];

			num[0] = (byte) (value >> 24);
			num[1] = (byte) (value >> 16);
			num[2] = (byte) (value >> 8);
			num[3] = (byte) (value);

			this.Write (num);
		}

		private void Write(byte[] buffer)
		{
			this.stream.Write (buffer, 0, buffer.Length);
		}

		private readonly System.IO.Stream stream;
	}
}
