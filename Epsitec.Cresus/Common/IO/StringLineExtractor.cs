//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.IO
{
	public static class StringLineExtractor
	{
		public static IEnumerable<string> GetLines(string buffer)
		{
			int pos = 0;

			while (pos < buffer.Length)
			{
				int end = buffer.IndexOf ('\n', pos);

				if (end < 0)
				{
					end = buffer.Length;
				}

				if (pos < end)
				{
					int trim;
					if (buffer[end-1] == '\r')
					{
						trim = 1;
					}
					else
					{
						trim = 0;
					}

					string line = buffer.Substring (pos, end-pos-trim);

					yield return line;
				}

				pos = end+1;
			}
		}
	}
}
