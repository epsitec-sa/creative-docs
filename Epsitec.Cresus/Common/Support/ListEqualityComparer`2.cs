//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support
{
	public class ListEqualityComparer<T1, T2> : IEqualityComparer<T1>
		where T1 : IList<T2>
		where T2 : class
	{
		#region IEqualityComparer<IList<string>> Members

		public bool Equals(T1 x, T1 y)
		{
			if (x.Count != y.Count)
			{
				return false;
			}

			int n = x.Count;

			for (int i = 0; i < n; i++)
			{
				if (x[i] == y[i])
				{
					continue;
				}

				if (x[i] == null)
				{
					return false;
				}

				if (x[i].Equals (y[i]) == false)
				{
					return false;
				}
			}

			return true;
		}

		public int GetHashCode(T1 obj)
		{
			return obj.Aggregate (0, (n, x) => (x == null ? 0 : x.GetHashCode () * 37) ^ n);
		}

		#endregion
	}
}

