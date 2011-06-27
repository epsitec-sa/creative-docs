//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers.DataAccessors.DynamicFactories.Helpers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.DataLayer.Context;

using System.Linq.Expressions;
using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors.DynamicFactories
{
	internal static class ItemPickerDynamicFactory
	{
		public static DynamicFactory Create<T>(BusinessContext business, LambdaExpression lambda, System.Func<T> entityGetter, string title, int? controllerSubType)
		{
			var fieldType    = lambda.ReturnType;
			var sourceType   = lambda.Parameters[0].Type;
			var itemType     = fieldType.GetGenericArguments ()[0];

			var getterLambda = lambda;
			var getterFunc   = getterLambda.Compile ();

			var factoryType = typeof (Factory<,,>).MakeGenericType (sourceType, fieldType, itemType);
			var instance    = System.Activator.CreateInstance (factoryType, business, lambda, entityGetter, getterFunc, title, controllerSubType);

			return (DynamicFactory) instance;
		}

		#region Factory Class

		private sealed class Factory<TSource, TField, TItem> : DynamicFactory
			where TSource : AbstractEntity
			where TItem : AbstractEntity, new ()
			where TField : IList<TItem>
		{
			public Factory(BusinessContext business, LambdaExpression lambda, System.Func<TSource> sourceGetter, System.Delegate getter, string title, int? controllerSubType)
			{
				this.business = business;
				this.lambda = lambda;
				this.sourceGetter = sourceGetter;
				this.getter = getter;
				this.title  = title;
				this.controllerSubType = controllerSubType;
			}

			private System.Func<IList<TItem>> CreateGetter()
			{
				return () => (IList<TItem>) this.getter.DynamicInvoke (this.sourceGetter ());
			}

			public override object CreateUI(FrameBox frame, UIBuilder builder)
			{
				var dummyList  = this.CreateDummyListEntity ();
				var controller = new SelectionController<TItem> (this.business)
				{
					CollectionValueGetter    = this.CreateGetter (),
					ToFormattedTextConverter = x => TextFormatter.FormatText (x.GetCompactSummary ()).IfNullOrEmptyReplaceWith (CollectionTemplate.DefaultEmptyText),
				};

				var tile    = frame as EditionTile;
				var caption = DynamicFactory.GetInputCaption (this.lambda);
				var title   = this.title ?? DynamicFactory.GetInputTitle (caption);
				var name    = DynamicFactory.GetInputName (caption);
				var widget  = builder.CreateEditionDetailedItemPicker (tile, name, dummyList, title, controller, EnumValueCardinality.Any, ViewControllerMode.Summary, this.controllerSubType.GetValueOrDefault (-1));

				if ((caption != null) &&
					(caption.HasDescription))
				{
					ToolTip.SetToolTipCaption (widget, caption);
				}

				return widget;
			}


			private DummyListEntity<TItem> CreateDummyListEntity()
			{
				//	The GUI needs an entity to attach to, in order to edit the list of entities
				//	found in the database; create such a dummy entity, which also has a specific
				//	controller :

				var temp = this.business.Data.CreateDummyEntity<DummyListEntity<TItem>> ();
				var list = this.business.GetAllEntities<TItem> ();

				foreach (var item in list)
				{
					temp.Items.Add (item);
				}
				
				return temp;
			}

			private readonly BusinessContext business;
			private readonly LambdaExpression lambda;
			private readonly System.Func<TSource> sourceGetter;
			private readonly System.Delegate getter;
			private readonly string title;
			private readonly int? controllerSubType;
		}

		#endregion
	}
}