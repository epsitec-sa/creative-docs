//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Types.Converters.Marshalers;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Resolvers;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Bricks.Factories
{
	internal static class EntityAutoCompleteTextFieldDynamicFactory
	{
		public static DynamicFactory Create<T>(BusinessContext business, LambdaExpression lambda, System.Func<T> entityGetter, string title, System.Collections.IEnumerable collection, int? specialController, bool readOnly)
		{
			var getterLambda = lambda;
			var setterLambda = ExpressionAnalyzer.CreateSetter (getterLambda);

			var fieldType    = lambda.ReturnType;
			var sourceType   = lambda.Parameters[0].Type;

			var getterFunc   = getterLambda.Compile ();
			var setterFunc   = setterLambda == null ? null : setterLambda.Compile ();

			var factoryType = typeof (Factory<,>).MakeGenericType (sourceType, fieldType);
			var instance    = System.Activator.CreateInstance (factoryType, business, lambda, entityGetter, getterFunc, setterFunc, title, collection, specialController, readOnly);

			return (DynamicFactory) instance;
		}

		#region Factory Class

		private sealed class Factory<TSource, TField> : DynamicFactory
			where TSource : AbstractEntity
			where TField : AbstractEntity, new ()
		{
			public Factory(BusinessContext business, LambdaExpression lambda, System.Func<TSource> sourceGetter, System.Delegate getter, System.Delegate setter, string title, System.Collections.IEnumerable collection, int? specialController, bool readOnly)
			{
				this.business = business;
				this.lambda = lambda;
				this.sourceGetter = sourceGetter;
				this.getter = getter;
				this.setter = setter;
				this.title  = title;
				this.collection = collection == null ? null : collection.OfType<TField> ();
				this.specialController = specialController;
				this.readOnly = readOnly;
			}

			private System.Func<TField> CreateGetter()
			{
				return () => (TField) this.getter.DynamicInvoke (this.sourceGetter ());
			}

			private System.Action<TField> CreateSetter()
			{
				if (this.setter == null)
				{
					return x => NOOP ();
				}
				else
				{
					return x => BridgeSpy.ExecuteSetter (this.sourceGetter (), this.lambda, x,
						(entity, field) => this.setter.DynamicInvoke (entity, field),
						(entity) => (TField) this.getter.DynamicInvoke (entity));
				}
			}

			private static void NOOP()
			{
				throw new System.InvalidOperationException ("No setter provided");
			}


			private System.Func<AbstractEntity> CreateGenericGetter()
			{
				return () => (AbstractEntity) this.getter.DynamicInvoke (this.sourceGetter ());
			}

			private ReferenceController CreateReferenceController()
			{
				return new ReferenceController ("x", this.CreateGenericGetter (), creator: this.CreateNewEntity);
			}

			private System.Func<IEnumerable<TField>> CreatePossibleItemsGetter()
			{
				if (this.collection == null)
				{
					return null;
				}
				else
				{
					return () => this.collection;
				}
			}

			private NewEntityReference CreateNewEntity(DataContext context)
			{
				return context.CreateEntityAndRegisterAsEmpty<TField> ();
			}

			public override object CreateUI(FrameBox frame, UIBuilder builder)
			{
				if (this.specialController.HasValue)
				{
					var fieldGetter = this.CreateGetter ();
					var entity      = fieldGetter ();
					var mode        = this.specialController.Value;
					var controller  = EntitySpecialControllerResolver.Create (builder.TileContainer, entity, mode);
					var tile        = frame as EditionTile;

					controller.CreateUI (tile.Container, builder, false);
					
					return null;
				}
				else
				{
					var sel = new SelectionController<TField> (this.business)
					{
						ValueGetter = this.CreateGetter (),
						ValueSetter = this.CreateSetter (),
						ReferenceController = this.CreateReferenceController (),
						PossibleItemsGetter = this.CreatePossibleItemsGetter (),
					};

					var caption = DynamicFactory.GetInputCaption (this.lambda);
					var title   = this.title ?? DynamicFactory.GetInputTitle (caption);
					var tile    = frame as EditionTile;
					var widget  = builder.CreateAutoCompleteTextField<TField> (tile, title, this.readOnly, sel);

					if ((caption != null) &&
						(caption.HasDescription))
					{
						ToolTip.SetToolTipCaption (widget, caption);
					}

					return widget;
				}
			}


			private readonly BusinessContext business;
			private readonly LambdaExpression lambda;
			private readonly System.Func<TSource> sourceGetter;
			private readonly System.Delegate getter;
			private readonly System.Delegate setter;
			private readonly string title;
			private readonly IEnumerable<TField> collection;
			private readonly int? specialController;
			private readonly bool readOnly;
		}

		#endregion
	}
}
