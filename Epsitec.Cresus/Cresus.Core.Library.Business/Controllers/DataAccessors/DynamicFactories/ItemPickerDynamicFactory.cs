//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.DataLayer.Context;

using System.Linq.Expressions;
using Epsitec.Cresus.Core.Library;
using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors.DynamicFactories
{
	internal abstract class ItemPickerDynamicFactory : DynamicFactory
	{
		public static DynamicFactory Create<T>(BusinessContext business, LambdaExpression lambda, System.Type itemType, System.Func<T> entityGetter, string title)
		{
			var fieldType    = lambda.ReturnType;
			var sourceType   = lambda.Parameters[0].Type;

			var getterLambda = lambda;
			var getterFunc   = getterLambda.Compile ();

			var factoryType = typeof (Factory<,,>).MakeGenericType (sourceType, fieldType, itemType);
			var instance    = System.Activator.CreateInstance (factoryType, business, lambda, entityGetter, getterFunc, title);

			return (DynamicFactory) instance;
		}

		class Factory<TSource, TField, TItem> : DynamicFactory
			where TSource : AbstractEntity
			where TItem : AbstractEntity, new ()
			where TField : IList<TItem>
		{
			public Factory(BusinessContext business, LambdaExpression lambda, System.Func<TSource> sourceGetter, System.Delegate getter, string title)
			{
				this.business = business;
				this.lambda = lambda;
				this.sourceGetter = sourceGetter;
				this.getter = getter;
				this.title  = title;
			}

			private System.Func<IList<TItem>> CreateGetter()
			{
				return () => (IList<TItem>) this.getter.DynamicInvoke (this.sourceGetter ());
			}

			public override object CreateUI(EditionTile tile, UIBuilder builder)
			{
				var controller = new SelectionController<TItem> (this.business)
				{
					CollectionValueGetter    = this.CreateGetter (),
					ToFormattedTextConverter = x => TextFormatter.FormatText (x.GetCompactSummary ()).IfNullOrEmptyReplaceWith (CollectionTemplate.DefaultEmptyText),
				};

				var caption = DynamicFactory.GetInputCaption (this.lambda);
				var title   = this.title ?? DynamicFactory.GetInputTitle (caption);
				var widget  = builder.CreateEditionDetailedItemPicker (tile, "Pictures", this.sourceGetter (), title, controller, EnumValueCardinality.Any, ViewControllerMode.Summary, 6);
				
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
			private readonly string title;
		}
	}
}
