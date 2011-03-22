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

namespace Epsitec.Cresus.Core.Controllers.DataAccessors.DynamicFactories
{
	internal abstract class EntityAutoCompleteTextFieldDynamicFactory : DynamicFactory
	{
		public static DynamicFactory Create<T>(BusinessContext business, LambdaExpression lambda, System.Func<T> entityGetter, string title)
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
			var instance    = System.Activator.CreateInstance (factoryType, business, lambda, entityGetter, getterFunc, setterFunc, title);

			return (DynamicFactory) instance;
		}
		
		class Factory<TSource, TField> : DynamicFactory
			where TField : AbstractEntity, new ()
		{
			public Factory(BusinessContext business, LambdaExpression lambda, System.Func<TSource> sourceGetter, System.Delegate getter, System.Delegate setter, string title)
			{
				this.business = business;
				this.lambda = lambda;
				this.sourceGetter = sourceGetter;
				this.getter = getter;
				this.setter = setter;
				this.title  = title;
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

			private ReferenceController CreateReferenceController()
			{
				return new ReferenceController ("x", this.CreateGenericGetter (), creator: this.CreateNewEntity);
			}

			private NewEntityReference CreateNewEntity(DataContext context)
			{
				return context.CreateEntityAndRegisterAsEmpty<TField> ();
			}

			public override object CreateUI(FrameBox frame, UIBuilder builder)
			{
				var sel = new SelectionController<TField> (this.business)
				{
					ValueGetter = this.CreateGetter (),
					ValueSetter = this.CreateSetter (),
					ReferenceController = this.CreateReferenceController (),
				};

				var tile    = frame as EditionTile;
				var caption = DynamicFactory.GetInputCaption (this.lambda);
				var title   = this.title ?? DynamicFactory.GetInputTitle (caption);
				var widget  = builder.CreateAutoCompleteTextField<TField> (tile, title, sel);

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
		}
	}
}
