using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Types.Converters.Marshalers;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Widgets.Tiles;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors.DynamicFactories
{
	internal abstract class TextFieldDynamicFactory : DynamicFactory
	{
		public static DynamicFactory Create<T>(LambdaExpression lambda, BusinessContext business, System.Func<T> entityGetter, int width)
		{
			var fieldType    = lambda.ReturnType;
			var sourceType   = lambda.Parameters[0].Type;
			var lambdaMember = (MemberExpression) lambda.Body;

			var sourceParameterExpression = Expression.Parameter (sourceType, "source");
			var valueParameterExpression  = Expression.Parameter (fieldType, "value");

			var expressionBlock =
				Expression.Block (
					Expression.Assign (
						Expression.Property (sourceParameterExpression, lambdaMember.Member.Name),
						valueParameterExpression));

			var getterLambda = lambda;
			var setterLambda = Expression.Lambda (expressionBlock, sourceParameterExpression, valueParameterExpression);

			var getterFunc   = getterLambda.Compile ();
			var setterFunc   = setterLambda.Compile ();

			var factoryType = typeof (Factory<,>).MakeGenericType (sourceType, fieldType);
			var instance    = System.Activator.CreateInstance (factoryType, business, lambda, entityGetter, getterFunc, setterFunc, width);

			return (DynamicFactory) instance;
		}

		class Factory<TSource, TField> : DynamicFactory
		{
			public Factory(BusinessContext business, LambdaExpression lambda, System.Func<TSource> sourceGetter, System.Delegate getter, System.Delegate setter, int width)
			{
				this.business = business;
				this.lambda   = lambda;
				this.sourceGetter = sourceGetter;
				this.getter = getter;
				this.setter = setter;
				this.width  = width;
			}

			private System.Func<TField> CreateGetter()
			{
				return () => (TField) this.getter.DynamicInvoke (this.sourceGetter ());
			}

			private System.Action<TField> CreateSetter()
			{
				return x => this.setter.DynamicInvoke (this.sourceGetter (), x);
			}

			private Marshaler CreateMarshaler()
			{
				return new NonNullableMarshaler<TField> (this.CreateGetter (), this.CreateSetter (), this.lambda);
			}

			public override object CreateUI(EditionTile tile, UIBuilder builder)
			{
				Marshaler marshaler = this.CreateMarshaler ();
				return builder.CreateTextField (tile, width, "Prénom", marshaler);
			}


			private readonly BusinessContext		business;
			private readonly LambdaExpression		lambda;
			private readonly System.Func<TSource>	sourceGetter;
			private readonly System.Delegate		getter;
			private readonly System.Delegate		setter;
			private readonly int					width;
		}
	}
}
