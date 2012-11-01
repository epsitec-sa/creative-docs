using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Core
{


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
