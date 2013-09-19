//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Threading;
using System.Threading.Tasks;

namespace Epsitec.Cresus.Assets.Core.Collections
{
	public static class AsyncEnumerableExtensions
	{
		public static async Task<T> FirstOrDefaultAsync<T>(this IAsyncEnumerable<T> enumerable, int index, CancellationToken token)
		{
			var enumerator = enumerable.GetAsyncEnumerator (index, index < 0 ? -1 : 1, token);

			if (await enumerator.MoveNext ())
			{
				return enumerator.Current;
			}
			else
			{
				return default (T);
			}
		}

		
		public static IAsyncEnumerator<T> GetAsyncEnumerator<T>(this IAsyncEnumerable<T> enumerable, int index, CancellationToken token)
		{
			return enumerable.GetAsyncEnumerator (index, int.MaxValue, token);
		}

		public static IAsyncEnumerator<T> GetReverseAsyncEnumerator<T>(this IAsyncEnumerable<T> enumerable, int index, CancellationToken token)
		{
			return enumerable.GetReverseAsyncEnumerator (index, int.MaxValue, token);
		}

		public static IAsyncEnumerator<T> GetReverseAsyncEnumerator<T>(this IAsyncEnumerable<T> enumerable, int index, int count, CancellationToken token)
		{
			return enumerable.GetAsyncEnumerator (index, -count, token);
		}
	}
}

