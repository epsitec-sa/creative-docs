using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	internal sealed class PropertyAccessorCache : AbstractLambdaCache<AbstractPropertyAccessor>
	{

		protected override AbstractPropertyAccessor Create(LambdaExpression lambda, string id)
		{
			return AbstractPropertyAccessor.Create (lambda, id);
		}


	}


}
