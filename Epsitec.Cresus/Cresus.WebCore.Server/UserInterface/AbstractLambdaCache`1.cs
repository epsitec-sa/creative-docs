using Epsitec.Common.Types;

using System.Collections.Generic;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface
{
	
	
	internal abstract class AbstractLambdaCache<T> : AbstractLambdaCache
	{

		protected AbstractLambdaCache()
		{
			this.lambdaToElement = new Dictionary<string, T> ();
			this.idToElement = new Dictionary<string, T> ();
		}


		public T Get(LambdaExpression lambda)
		{
			T element;

			var lambdaKey = AbstractLambdaCache.GetLambdaKey (lambda);

			var exists = this.lambdaToElement.TryGetValue (lambdaKey, out element);

			if (!exists)
			{
				var id = InvariantConverter.ToString (this.lambdaToElement.Count);

				element = this.Create (lambda, id);

				this.lambdaToElement[lambdaKey] = element;
				this.idToElement[id] = element;
			}

			return element;
		}


		public T Get(string id)
		{
			T element;

			this.idToElement.TryGetValue (id, out element);

			return element;
		}


		protected abstract T Create(LambdaExpression lambda, string id);


		private readonly Dictionary<string, T> lambdaToElement;


		private readonly Dictionary<string, T> idToElement;


	}


}
