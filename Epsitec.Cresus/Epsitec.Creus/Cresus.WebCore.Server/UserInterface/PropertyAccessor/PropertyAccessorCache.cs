using Epsitec.Common.Types;

using System.Collections.Generic;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.PropertyAccessor
{


	internal sealed class PropertyAccessorCache
	{


		public PropertyAccessorCache()
		{
			this.lambdaToPropertyAccessor = new Dictionary<string, AbstractPropertyAccessor> ();
			this.idToPropertyAccessor = new Dictionary<string, AbstractPropertyAccessor> ();
		}


		public AbstractPropertyAccessor Get(LambdaExpression lambda)
		{
			AbstractPropertyAccessor propertyAccessor;

			var lambdaKey = PropertyAccessorCache.GetLambdaKey (lambda);

			var exists = this.lambdaToPropertyAccessor.TryGetValue (lambdaKey, out propertyAccessor);

			if (!exists)
			{
				var id = InvariantConverter.ToString (this.lambdaToPropertyAccessor.Count);

				propertyAccessor = AbstractPropertyAccessor.Create (lambda, id);

				this.lambdaToPropertyAccessor[lambdaKey] = propertyAccessor;
				this.idToPropertyAccessor[id] = propertyAccessor;
			}

			return propertyAccessor;
		}


		public AbstractPropertyAccessor Get(string id)
		{
			AbstractPropertyAccessor propertyAccessor;

			this.idToPropertyAccessor.TryGetValue (id, out propertyAccessor);

			return propertyAccessor;
		}


		private static string GetLambdaKey(LambdaExpression lambda)
		{
			var part1 = lambda.ToString ();
			var part2 = lambda.ReturnType.FullName;
			var part3 = lambda.Parameters[0].Type.FullName;

			return part1 + part2 + part3;
		}


		private readonly Dictionary<string, AbstractPropertyAccessor> lambdaToPropertyAccessor;


		private readonly Dictionary<string, AbstractPropertyAccessor> idToPropertyAccessor;


	}


}
