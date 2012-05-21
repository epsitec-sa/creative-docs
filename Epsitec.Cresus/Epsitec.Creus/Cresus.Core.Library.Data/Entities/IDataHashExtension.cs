//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public static class IDataHashExtension
	{
		public static void SetHashes(this IDataHash dataHash, byte[] data)
		{
			dataHash.WeakHash   = Checksum.ComputeAdler32 (data, 32*1024);
			dataHash.StrongHash = Checksum.ComputeMd5Hash (data);
		}

		public static int? GetWeakHash(byte[] data)
		{
			return Checksum.ComputeAdler32 (data, 32*1024);
		}

		public static string GetStrongHash(byte[] data)
		{
			return Checksum.ComputeMd5Hash (data);
		}
	}
}
