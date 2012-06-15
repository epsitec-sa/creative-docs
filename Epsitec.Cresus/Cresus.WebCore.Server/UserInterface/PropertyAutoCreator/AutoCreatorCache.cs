using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.PropertyAutoCreator
{


	internal sealed class AutoCreatorCache : AbstractLambdaCache<AutoCreator>
	{


		protected override AutoCreator Create(LambdaExpression lambda, string id)
		{
			return new AutoCreator (lambda, id);
		}


	}


}
