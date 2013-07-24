using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	/// <summary>
	/// This class is used as a factory for the property accessors, and holds a mapping between the
	/// lambdas used to create them, the property accessors, and their ids.
	/// </summary>
	internal sealed class PropertyAccessorCache : AbstractLambdaCache<AbstractPropertyAccessor>
	{

		protected override AbstractPropertyAccessor Create(LambdaExpression lambda, string id)
		{
			return AbstractPropertyAccessor.Create (lambda, id);
		}


	}


}
