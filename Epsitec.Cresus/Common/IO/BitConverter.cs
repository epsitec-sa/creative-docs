//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.IO
{
	public static class BitConverter
	{
		public static IEnumerable<byte> ToBytes(long[] values)
		{
			for (int i = 0; i  < values.Length; i++)
			{
				long value = values[i];

				yield return (byte) (value >> 56);
				yield return (byte) (value >> 48);
				yield return (byte) (value >> 40);
				yield return (byte) (value >> 32);
				yield return (byte) (value >> 24);
				yield return (byte) (value >> 16);
				yield return (byte) (value >>  8);
				yield return (byte) (value >>  0);
			}
		}
	}
}
