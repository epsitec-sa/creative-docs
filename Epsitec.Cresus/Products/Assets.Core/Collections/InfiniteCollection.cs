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
			
			this.enumerable   = enumerable;
			this.cancellation = new CancellationTokenSource ();
		}


		public T this[int index]
		{
			get
			{
				T value;
				
				this.TryGetValue (index, out value);
				
				return value;
			}
			set
			{
				this.SetValue (index, value);
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
				if (this.enumerable != null)
				{
					this.FillCacheItemAsync (item, index)
						.ForgetSafely ();
				}
			}
			
			value = default (T);
			return false;
		}

		public bool SetValue(int index, T value)
		{
			CacheItem item = null;

			lock (this.cache)
			{
				if ((this.cache.TryGetValue (index, out item)) &&
					(item.IsReady) &&
					(item.Equals (value)))
				{
					return false;
				}

				this.cache[index] = new CacheItem (value);
			}

			if (item == null)
			{
				this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Add, index));
				
				return true;
			}
			else
			{
				if (item.IsReady)
				{
					item.Dispose ();
					this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Replace, index));
				}
				else
				{
					item.Dispose ();
					this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Add, index));
				}
				
				return false;
			}
		}

		public bool ClearValue(int index)
		{
			CacheItem item = null;

			lock (this.cache)
			{
				this.cache.TryGetValue (index, out item);
				this.cache.Remove (index);
			}

			if (item == null)
			{
				return false;
			}

			if (item.IsReady)
			{
				this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Remove, -1, null, index, null));
			}
			
			item.Dispose ();

			return true;
		}
		
		public bool Clear()
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

			return changed;
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

				if (item.TrySetValue (value))
				{
					this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Add, index));
				}
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
					//	Did someone already put a value into the collection? If so, we won't
					//	overwrite it with the exception information:

					if (item.State != CacheItemState.Uninitialized)
					{
						//	TODO: notify someone about the exception?

						return;
					}

					CacheItem currentItem;
					
					if ((this.cache.TryGetValue (index, out currentItem)) &&
						(currentItem != item))
					{
						//	TODO: notify someone about the exception?

						return;
					}

					this.cache[index] = new CacheItemWithException (ex);
				}
			}
		}

		private Task<T> RetrieveDataAsync(int index)
		{
			return this.enumerable.FirstOrDefaultAsync (index, this.cancellation.Token);
		}


		#region CacheItemState Enumeration

		enum CacheItemState
		{
			Uninitialized,
			Transitioning,
			Ready,
			Disposed,
		}

		#endregion

		#region CacheItem Class

		private class CacheItem : System.IDisposable
		{
			public CacheItem()
			{
				this.value = default (T);
				this.state = (int) CacheItemState.Uninitialized;
			}

			public CacheItem(T value)
			{
				this.value = value;
				this.state = (int) CacheItemState.Ready;
			}


			public virtual T					Value
			{
				get
				{
					//	Ensure that if the caller checks IsReady and then reads the Value based
					//	on that result, that the CPU won't execute the read of IsReady after the
					//	read of the value:

					Thread.MemoryBarrier ();
					
					return this.value;
				}
			}

			public bool							IsReady
			{
				get
				{
					return this.state == (int) CacheItemState.Ready;
				}
			}

			public CacheItemState				State
			{
				get
				{
					return (CacheItemState) this.state;
				}
			}


			public bool TrySetValue(T value)
			{
				var oldState = this.SetState (CacheItemState.Transitioning, CacheItemState.Uninitialized);

				if (oldState == CacheItemState.Uninitialized)
				{
					//	We successfully started the 'set state' operation. Write the new value and
					//	mark the state as ready.

					this.value = value;

					switch (this.SetState (CacheItemState.Ready, CacheItemState.Transitioning))
					{
						case CacheItemState.Transitioning:
							break;

						case CacheItemState.Disposed:
							//	Dispose was called just after we started setting the value; let
							//	the dispose win:
							
							this.value = default (T);
							break;

						default:
							throw new System.NotSupportedException ("Invalid transition");
					}

					return true;
				}

				return false;
			}

			public void SetValue(T value)
			{
				while (true)
				{
					switch (this.State)
					{
						case CacheItemState.Ready:
						case CacheItemState.Transitioning:
							throw new System.InvalidOperationException ("SetValue called several times on the same cache item");

						case CacheItemState.Disposed:
							throw new System.ObjectDisposedException ("Cache item");

						case CacheItemState.Uninitialized:
							break;

						default:
							throw new System.NotSupportedException ();
					}

					if (this.TrySetValue (value))
					{
						return;
					}
				}
			}

			private CacheItemState SetState(CacheItemState newState)
			{
				return (CacheItemState) Interlocked.Exchange (ref this.state, (int) newState);
			}

			private CacheItemState SetState(CacheItemState newState, CacheItemState oldState)
			{
				return (CacheItemState) Interlocked.CompareExchange (ref this.state, (int) newState, (int) oldState);
			}

			public bool Equals(T value)
			{
				return EqualityComparer<T>.Default.Equals (value, this.Value);
			}
			
			#region IDisposable Members

			public void Dispose()
			{
				this.value = default (T);

				//	No need to check previous state: disposing wins. Always.

				this.SetState (CacheItemState.Disposed);
			}

			#endregion

			private T							value;
			private int							state;
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