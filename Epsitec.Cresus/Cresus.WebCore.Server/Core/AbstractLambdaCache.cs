using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core
{


	internal abstract class AbstractLambdaCache
	{


		protected static string GetLambdaKey(LambdaExpression lambda)
		{
			var part1 = lambda.ToString ();
			var part2 = lambda.ReturnType.FullName;
			var part3 = lambda.Parameters[0].Type.FullName;

			return part1 + part2 + part3;
		}


	}


}
