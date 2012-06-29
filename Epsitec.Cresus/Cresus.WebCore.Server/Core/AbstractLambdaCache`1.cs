using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System;

using System.Collections.Generic;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core
{
	
	
	internal abstract class AbstractLambdaCache<T> : AbstractLambdaCache, IDisposable
	{


		protected AbstractLambdaCache()
		{
			this.lambdaToElement = new Dictionary<string, T> ();
			this.idToElement = new Dictionary<string, T> ();

			this.rwLock = new ReaderWriterLockWrapper ();
		}


		public T Get(LambdaExpression lambda)
		{
			T element;
			bool done;
			
			var lambdaKey = AbstractLambdaCache.GetLambdaKey (lambda);

			using (this.rwLock.LockRead ())
			{
				done = this.lambdaToElement.TryGetValue (lambdaKey, out element);
			}

			if (!done)
			{
				using (this.rwLock.LockWrite ())
				{
					done = this.lambdaToElement.TryGetValue (lambdaKey, out element);

					if (!done)
					{
						var id = InvariantConverter.ToString (this.lambdaToElement.Count);

						element = this.Create (lambda, id);

						this.lambdaToElement[lambdaKey] = element;
						this.idToElement[id] = element;
					}
				}
			}

			return element;
		}


		public T Get(string id)
		{
			using (this.rwLock.LockRead ())
			{
				T element;

				this.idToElement.TryGetValue (id, out element);

				return element;
			}
		}


		#region IDisposable Members


		public void Dispose()
		{
			this.rwLock.Dispose ();
		}


		#endregion
	


		protected abstract T Create(LambdaExpression lambda, string id);


		private readonly Dictionary<string, T> lambdaToElement;


		private readonly Dictionary<string, T> idToElement;


		private readonly ReaderWriterLockWrapper rwLock;


	}


}
