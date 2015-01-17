//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public static class NodeGetterExtensions
	{
		public static IEnumerable<T> GetNodes<T>(this INodeGetter<T> getter)
			where T : struct
		{
			{
				for (int i=0; i<getter.Count; i++)
				{
					yield return getter[i];
				}
			}
		}


	}
}

