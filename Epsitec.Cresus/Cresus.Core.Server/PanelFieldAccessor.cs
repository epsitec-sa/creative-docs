//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Types.Converters.Marshalers;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Server
{
	public class PanelFieldAccessor
	{
		public PanelFieldAccessor(LambdaExpression lambda, int id)
		{
			var getterLambda = lambda;
			var setterLambda = ExpressionAnalyzer.CreateSetter (getterLambda);

			var lambdaMember = (MemberExpression) lambda.Body;
			var propertyInfo = lambdaMember.Member as System.Reflection.PropertyInfo;
			var typeField    = EntityInfo.GetStructuredTypeField (propertyInfo);
			var fieldType    = lambda.ReturnType;
			var sourceType   = lambda.Parameters[0].Type;

			bool nullable    = fieldType.IsNullable ();

			this.getterFunc   = getterLambda.Compile ();
			this.setterFunc   = setterLambda.Compile ();

			if (nullable)
			{
				fieldType = fieldType.GetNullableTypeUnderlyingType ();
			}

			var factoryType = (nullable ? typeof (NullableFactory<,>) : typeof (NonNullableFactory<,>)).MakeGenericType (sourceType, fieldType);

			this.id = id;
			this.lambda   = lambda;
			this.marshalerFactory = System.Activator.CreateInstance (factoryType, this) as DynamicFactory;
		}


		public int Id
		{
			get
			{
				return this.id;
			}
		}


		public static string GetLambdaFootprint(LambdaExpression lambda)
		{
			return string.Concat (lambda.ToString (), "/",
								  lambda.ReturnType.FullName, "/",
								  lambda.Parameters[0].Type.FullName);
		}

		abstract class DynamicFactory
		{
			public DynamicFactory(PanelFieldAccessor accessor)
			{
				this.accessor = accessor;
			}

			public abstract Marshaler CreateMarshaler(AbstractEntity entity);

			protected readonly PanelFieldAccessor accessor;
		}

		sealed class NullableFactory<TSource, TField> : DynamicFactory
			where TField : struct
			where TSource : AbstractEntity
		{
			public NullableFactory(PanelFieldAccessor accessor)
				: base (accessor)
			{
			}

			private System.Func<TField?> CreateGetter(TSource source)
			{
				return () => (TField?) this.accessor.getterFunc.DynamicInvoke (source);
			}

			private System.Action<TField?> CreateSetter(TSource source)
			{
				return x => this.accessor.setterFunc.DynamicInvoke (source, x);
			}

			public override Marshaler CreateMarshaler(AbstractEntity entity)
			{
				TSource source = entity as TSource;
				return new NullableMarshaler<TField> (this.CreateGetter (source), this.CreateSetter (source), this.accessor.lambda);
			}
		}

		sealed class NonNullableFactory<TSource, TField> : DynamicFactory
			where TSource : AbstractEntity
		{
			public NonNullableFactory(PanelFieldAccessor accessor)
				: base (accessor)
			{
			}

			private System.Func<TField> CreateGetter(TSource source)
			{
				return () => (TField) this.accessor.getterFunc.DynamicInvoke (source);
			}

			private System.Action<TField> CreateSetter(TSource source)
			{
				return x => this.accessor.setterFunc.DynamicInvoke (source, x);
			}

			public override Marshaler CreateMarshaler(AbstractEntity entity)
			{
				TSource source = entity as TSource;
				return new NonNullableMarshaler<TField> (this.CreateGetter (source), this.CreateSetter (source), this.accessor.lambda);
			}
		}

		private readonly int					id;
		private readonly LambdaExpression		lambda;
		private readonly System.Delegate		getterFunc;
		private readonly System.Delegate		setterFunc;
		private readonly DynamicFactory			marshalerFactory;
	}
}
