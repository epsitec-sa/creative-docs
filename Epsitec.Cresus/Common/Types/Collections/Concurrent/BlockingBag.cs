using System.Collections.Concurrent;

namespace Epsitec.Common.Types.Collections.Concurrent
{
	/// <summary>
	/// The <c>BlockingBag&lt;T&gt;</c> class implements a concurrent bag, wrapped
	/// by a <see cref="BlockingCollection"/>.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection.</typeparam>
	public sealed class BlockingBag<T> : BlockingCollection<T>
	{
		public BlockingBag()
			: base (new ConcurrentBag<T> ())
		{
		}
	}
}
