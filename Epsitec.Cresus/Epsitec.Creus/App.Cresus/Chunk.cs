//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus
{
	public sealed class Chunk
	{
		public Chunk(System.Guid guid, string name, byte[] data = null)
		{
			this.guid = guid;
			this.name = name;
			this.data = data ?? Chunk.EmptyData;
		}


		public byte[]							Data
		{
			get
			{
				return this.data;
			}
		}

		public System.Guid						Guid
		{
			get
			{
				return this.guid;
			}
		}

		public string							Name
		{
			get
			{
				return this.name;
			}
		}


		private static readonly byte[]			EmptyData = new byte[0];
		
		private readonly System.Guid			guid;
		private readonly string					name;
		private readonly byte[]					data;
	}
}