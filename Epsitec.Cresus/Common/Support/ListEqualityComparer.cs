//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support
{
	public static class ListEqualityComparer
	{
		public static IEqualityComparer<IList<T>> GetComparer<T>()
			where T : class
		{
			return Cache<T>.Comparer;
		}


		private static class Cache<T>
			where T : class
		{
			public static readonly IEqualityComparer<IList<T>> Comparer = new ListEqualityComparer<IList<T>, T> ();
		}
	}
}
