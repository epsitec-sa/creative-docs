//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Concurrent;

namespace Epsitec.Common.Types.Collections.Concurrent
{
	/// <summary>
	/// The <c>BlockingQueue&lt;T&gt;</c> class implements a concurrent queue, wrapped
	/// by a <see cref="BlockingCollection"/>.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection.</typeparam>
	public sealed class BlockingQueue<T> : BlockingCollection<T>
	{
		public BlockingQueue()
			: base (new ConcurrentQueue<T> ())
		{
		}
	}
}
