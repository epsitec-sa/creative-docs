using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAutoCreator
{


	internal sealed class AutoCreatorCache : AbstractLambdaCache<AutoCreator>
	{


		protected override AutoCreator Create(LambdaExpression lambda, int id)
		{
			return new AutoCreator (lambda, id);
		}


	}


}
