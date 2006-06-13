using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Comparers
{
	/// <summary>
	///	Compare deux proxies d'après leurs rangs.
	/// </summary>
	public class ProxyRank : IComparer<IProxy>
	{
		public int Compare(IProxy a, IProxy b)
		{
			if (a == b)
			{
				return 0;
			}

			if (a == null)
			{
				return -1;
			}
			
			if (b == null)
			{
				return 1;
			}
			
			if (a.Rank < b.Rank)
			{
				return -1;
			}
			
			if (a.Rank > b.Rank)
			{
				return 1;
			}

			return 0;
		}
	}
}
