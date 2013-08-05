using Epsitec.Common.Support;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core
{


	/// <summary>
	/// This class is used to instantiates types relating to lambda expressions, and keep a mapping
	/// between these lambda expression and these items, and another mapping between the ids of
	/// the items and the items. We have thus the two mappings
	/// - lambda => items
	/// - id => items
	/// This allows reference of these objects to be given to the javascript client as small ids,
	/// that can then be resolved as these objects later on when the client gives back an id.
	/// </summary>
	internal abstract class AbstractLambdaCache<T> : ItemCache<LambdaExpression, string, T, string, T>
	{


		public T Get(LambdaExpression lambda)
		{
			return this.Get1 (lambda);
		}


		public T Get(string id)
		{
			return this.Get2 (id);
		}


		protected abstract T Create(LambdaExpression lambda, string id);


		protected override string GetKey1(LambdaExpression itemIn1)
		{
			var part1 = ExpressionNormalizer.Normalize (itemIn1.Body).ToString ();
			var part2 = itemIn1.Parameters[0].Type.TypeHandle.Value.ToInt64 ().ToString ();

			return part1 + part2;
		}


		protected override T GetItemOut1(LambdaExpression itemIn1)
		{
			return this.Create (itemIn1, this.GetCurrentId ());
		}


		protected override string GetItemIn2(LambdaExpression itemIn1, T itemOut1)
		{
			return this.GetCurrentId ();
		}


		protected override T GetItemOut2(LambdaExpression itemIn1, T itemOut1, string itemIn2)
		{
			return itemOut1;
		}


	}


}
