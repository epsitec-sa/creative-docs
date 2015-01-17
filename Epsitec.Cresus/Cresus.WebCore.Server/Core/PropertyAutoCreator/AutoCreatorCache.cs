using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAutoCreator
{


	/// <summary>
	/// This class is used as a factory for the auto creators, and holds a mapping between the
	/// lambdas used to create them, the auto creators, and their ids.
	/// </summary>
	internal sealed class AutoCreatorCache : AbstractLambdaCache<AutoCreator>
	{


		protected override AutoCreator Create(LambdaExpression lambda, string id)
		{
			return new AutoCreator (lambda, id);
		}


	}


}
