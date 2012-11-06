using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core
{
	
	
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
			var part1 = itemIn1.ToString ();
			var part2 = itemIn1.ReturnType.FullName;
			var part3 = itemIn1.Parameters[0].Type.FullName;

			return part1 + part2 + part3;
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
