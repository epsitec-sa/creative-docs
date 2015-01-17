//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using System.Collections.Generic;
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

		public void AddRange(IEnumerable<T> collection)
		{
			foreach (var item in collection)
			{
				this.Add (item);
			}
		}
	}
}
