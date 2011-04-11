//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Types.Converters.Marshalers;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epsitec.Cresus.Core.Library;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Widgets;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors.DynamicFactories
{
	internal abstract class EnumAutoCompleteTextFieldDynamicFactory : DynamicFactory
	{
		public static DynamicFactory Create<T>(BusinessContext business, LambdaExpression lambda, System.Func<T> entityGetter, string title, int width)
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
			var instance    = System.Activator.CreateInstance (factoryType, business, lambda, entityGetter, getterFunc, setterFunc, title, width);

			return (DynamicFactory) instance;
		}
		
		class Factory<TSource, TField> : DynamicFactory
			where TField : struct
		{
			public Factory(BusinessContext business, LambdaExpression lambda, System.Func<TSource> sourceGetter, System.Delegate getter, System.Delegate setter, string title, int width)
			{
				this.business = business;
				this.lambda = lambda;
				this.sourceGetter = sourceGetter;
				this.getter = getter;
				this.setter = setter;
				this.title  = title;
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

			private System.Func<AbstractEntity> CreateGenericGetter()
			{
				return () => (AbstractEntity) this.getter.DynamicInvoke (this.sourceGetter ());
			}

			private Marshaler CreateMarshaler()
			{
				return new NonNullableMarshaler<TField> (this.CreateGetter (), this.CreateSetter (), this.lambda);
			}

			public override object CreateUI(FrameBox frame, UIBuilder builder)
			{
				IEnumerable<EnumKeyValues<TField>> possibleItems = EnumKeyValues.FromEnum<TField> ();
				ValueToFormattedTextConverter<EnumKeyValues<TField>> getUserText = x => TextFormatter.FormatText (x);

				var tile    = frame as EditionTile;
				var marshaler = this.CreateMarshaler ();
				var caption = DynamicFactory.GetInputCaption (this.lambda);
				var title   = this.title ?? DynamicFactory.GetInputTitle (caption);
				var widget  = builder.CreateAutoCompleteTextField<TField> (tile, this.width, title, marshaler, possibleItems, getUserText);

				if ((caption != null) &&
					(caption.HasDescription))
				{
					ToolTip.SetToolTipCaption (widget, caption);
				}

				return widget;
			}


			private readonly BusinessContext business;
			private readonly LambdaExpression lambda;
			private readonly System.Func<TSource> sourceGetter;
			private readonly System.Delegate getter;
			private readonly System.Delegate setter;
			private readonly string title;
			private readonly int width;
		}
	}
}
