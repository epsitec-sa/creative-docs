using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Core
{


	/// <summary>
	/// This is the base class of all caches classes used in WebCore to create objects and keep a
	/// mapping between them and some kind of ids.
	/// </summary>
	/// <remarks>
	/// The general idea, is that you can create an object of type TOut1, by giving this class an
	/// object of type TIn1. Later on, you can access to this instance of TOut1 by using the same
	/// instance of TIn1 that you provided to create it. There is an optional conversion step
	/// between TIn1 and TKey1, if TIn1 cannot be used directly as a key of a dictionnary.
	/// In addition, we keep another mapping between TIn2 and TOut2, which are both created after
	/// the creation of the instance of TOut1. They are used to offer some kind of reverse mapping.
	/// If you have troubles understanding this class, as it is quite tricky, maybe a look in its
	/// subclasses will shed some light on it and how it can be used.
	/// 
	/// The methods used to create or access items are threadsafe, but not the constructor or the
	/// Dispose() method.
	/// </remarks>
	internal abstract class ItemCache<TIn1, TKey1, TOut1, TIn2, TOut2> : IDisposable
	{


		public ItemCache()
		{
			this.mapping1 = new Dictionary<TKey1, TOut1> ();
			this.mapping2 = new Dictionary<TIn2, TOut2> ();
			this.rwLock = new ReaderWriterLockWrapper ();
		}


		protected TOut1 Get1(TIn1 itemIn1)
		{
			TOut1 itemOut1;
			bool done;

			TKey1 key1 = this.GetKey1 (itemIn1);

			using (this.rwLock.LockRead ())
			{
				done = this.mapping1.TryGetValue (key1, out itemOut1);
			}

			if (!done)
			{
				using (this.rwLock.LockWrite ())
				{
					done = this.mapping1.TryGetValue (key1, out itemOut1);

					if (!done)
					{
						itemOut1 = this.GetItemOut1 (itemIn1);

						var itemIn2 = this.GetItemIn2 (itemIn1, itemOut1);
						var itemOut2 = this.GetItemOut2 (itemIn1, itemOut1, itemIn2);

						this.mapping1[key1] = itemOut1;
						this.mapping2[itemIn2] = itemOut2;
					}
				}
			}

			return itemOut1;
		}


		protected TOut2 Get2(TIn2 itemIn2)
		{
			using (this.rwLock.LockRead ())
			{
				TOut2 itemOut2;

				this.mapping2.TryGetValue (itemIn2, out itemOut2);

				return itemOut2;
			}
		}


		protected string GetCurrentId()
		{
			return "id" + InvariantConverter.ToString (this.mapping1.Count);
		}


		protected abstract TKey1 GetKey1(TIn1 itemIn1);


		protected abstract TOut1 GetItemOut1(TIn1 itemIn1);


		protected abstract TIn2 GetItemIn2(TIn1 itemIn1, TOut1 itemOut1);


		protected abstract TOut2 GetItemOut2(TIn1 itemIn1, TOut1 itemOut1, TIn2 itemIn2);


		#region IDisposable Members


		public void Dispose()
		{
			this.rwLock.Dispose ();
		}


		#endregion


		private readonly Dictionary<TKey1, TOut1> mapping1;


		private readonly Dictionary<TIn2, TOut2> mapping2;


		private readonly ReaderWriterLockWrapper rwLock;


	}


}
