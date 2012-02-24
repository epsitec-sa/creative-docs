using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types.Converters;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface
{


	internal abstract class AbstractMarshallerFactory
	{


		protected AbstractMarshallerFactory(LambdaExpression lambda)
		{
			this.lambda = lambda;
		}


		protected LambdaExpression Lambda
		{
			get
			{
				return this.lambda;
			}
		}


		public abstract Marshaler CreateMarshaler(AbstractEntity entity);


		private readonly LambdaExpression lambda;


	}


}

