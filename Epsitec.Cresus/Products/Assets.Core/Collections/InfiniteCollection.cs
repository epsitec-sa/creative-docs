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
		public InfiniteCollection(IAsyncEnumerable<T> enumerable)
		{
			this.cache = new Dictionary<int, CacheItem> ();
			this.enumerable = enumerable;
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
					else
					{
						item = null;
					}
				}
				else
				{
					item = new CacheItem ();
					this.cache[index] = item;
				}
			}

			if (item != null)
			{
				this.FillCacheItemAsync (item, index)
					.ForgetSafely ();
			}
			
			value = default (T);
			return false;
		}

		public void Clear()
		{
			this.cancellation.Cancel ();
			this.cancellation = new CancellationTokenSource ();

			bool changed = false;

			lock (this.cache)
			{
				if (this.cache.Count > 0)
				{
					this.cache.Clear ();
					changed = true;
				}
			}

			if (changed)
			{
				this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Reset));
			}
		}


		protected void OnCollectionChanged(CollectionChangedEventArgs e)
		{
			this.CollectionChanged.Raise (this, e);
		}


		private async Task FillCacheItemAsync(CacheItem item, int index)
		{
			try
			{
				var value = await this.RetrieveDataAsync (index);
				
				item.SetValue (value);

				this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Add, index));
			}
			catch (System.AggregateException ex)
			{
				ex.Handle (e => e is TaskCanceledException);
			}
			catch (TaskCanceledException)
			{
				throw;
			}
			catch (System.Exception ex)
			{
				lock (this.cache)
				{
					this.cache[index] = new CacheItemWithException (ex);
				}
			}
		}

		private Task<T> RetrieveDataAsync(int index)
		{
			return this.enumerable.FirstOrDefaultAsync (index, this.cancellation.Token);
		}


		#region CacheItem Class

		private class CacheItem
		{
			public CacheItem()
			{
				this.value = default (T);
				this.ready = 0;
			}

			public CacheItem(T value)
			{
				this.value = value;
				this.ready = 1;
			}


			public virtual T Value
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

		#endregion

		#region CacheItemWithException Class

		private class CacheItemWithException : CacheItem
		{
			public CacheItemWithException(System.Exception ex)
				: base (default (T))
			{
				this.exception = ex;
			}


			public override T					Value
			{
				get
				{
					throw new System.Exception ("Value is not available", this.exception);
				}
			}

			private readonly System.Exception	exception;
		}

		#endregion

		#region INotifyCollectionChanged Members

		public event EventHandler<CollectionChangedEventArgs> CollectionChanged;

		#endregion

		private readonly Dictionary<int, CacheItem> cache;
		private readonly IAsyncEnumerable<T>	enumerable;
		private CancellationTokenSource			cancellation;
	}
}