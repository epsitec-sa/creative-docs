//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Epsitec.Cresus.Assets.Core.Collections
{
	public class InfiniteCollection<T> : INotifyCollectionChanged
	{
		public InfiniteCollection(IAsyncValueProvider<T> provider)
		{
			this.cache = new Dictionary<int, CacheItem> ();
			this.provider = provider;
			this.cancellation = new CancellationTokenSource ();
		}


		public T this[int index]
		{
			get
			{
				T value;

				if (this.TryGetValue (index, out value))
				{
					return value;
				}
				else
				{
					return default (T);
				}
			}
		}


		public bool TryGetValue(int index, out T value)
		{
			CacheItem item = null;

			lock (this.cache)
			{
				if (this.cache.TryGetValue (index, out item))
				{
					if (item.IsReady)
					{
						value = item.Value;
						return true;
					}
				}
				else
				{
					item = new CacheItem ();
					this.cache[index] = item;
					this.FetchData (item, this.FetchDataAsync (index)).ForgetSafely ();
				}
			}

			value = default (T);
			return false;
		}

		public void Clear()
		{
			this.cancellation.Cancel ();
			this.cancellation = new CancellationTokenSource ();

			lock (this.cache)
			{
				this.cache.Clear ();
			}
		}


		private async Task FetchData(CacheItem item, Task<T> valueTask)
		{
			var value = await valueTask;

			item.SetValue (value);
		}

		private async Task<T> FetchDataAsync(int index)
		{
			var enumerator = this.provider.GetValuesAsync (index, 1, this.cancellation.Token);

			if (await enumerator.MoveNext ())
			{
				return enumerator.Current;
			}
			else
			{
				return default (T);
			}
		}


		protected void OnCollectionChanged(CollectionChangedEventArgs e)
		{
			this.CollectionChanged.Raise (this, e);
		}


		#region INotifyCollectionChanged Members

		public event EventHandler<CollectionChangedEventArgs> CollectionChanged;

		#endregion

		private class CacheItem
		{
			public CacheItem()
			{
				this.value = default (T);
				this.ready = 0;
			}


			public T Value
			{
				get
				{
					//	Ensure that if the caller checks IsReady and then reads the Value based
					//	on that result, that the CPU won't execute the read of IsReady after the
					//	read of the value:

					System.Threading.Thread.MemoryBarrier ();
					
					return this.value;
				}
			}

			public bool IsReady
			{
				get
				{
					return this.ready > 0;
				}
			}


			public void SetValue(T value)
			{
				this.value = value;

				if (System.Threading.Interlocked.Increment (ref this.ready) > 1)
				{
					throw new System.InvalidOperationException ("SetValue called several times on the same cache item");
				}
			}

			private T							value;
			private int							ready;
		}


		private readonly Dictionary<int, CacheItem> cache;
		private readonly IAsyncValueProvider<T>	provider;
		private CancellationTokenSource cancellation;
	}
}