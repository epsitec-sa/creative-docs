//	Copyright © 2009-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	public static class StringBuilderExtensions
	{
		public static void AppendJoin(this System.Text.StringBuilder buffer, string separator, IEnumerable<string> collection)
		{
			bool first = true;

			foreach (var item in collection)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					buffer.Append (separator);
				}

				buffer.Append (item);
			}
		}

		public static bool EndsWith(this System.Text.StringBuilder buffer, string value)
		{
			if (buffer == null)
			{
				return false;
			}
			if (string.IsNullOrEmpty (value))
            {
				return true;
            }

			int n = buffer.Length;
			
			if (value.Length > n)
            {
				return false;
            }

			for (int i = value.Length; i > 0; )
			{
				char c1 = value[--i];
				char c2 = buffer[--n];

				if (c1 != c2)
                {
					return false;
                }
			}

			return true;
		}
	}
}