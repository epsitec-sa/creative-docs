﻿//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Types.Converters.Marshalers;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Bricks.DynamicFactories
{
	internal static class EnumAutoCompleteTextFieldDynamicFactory
	{
		public static DynamicFactory Create<T>(BusinessContext business, LambdaExpression lambda, System.Func<T> entityGetter, string title, int width, bool readOnly)
		{
			var getterLambda = lambda;
			var setterLambda = ExpressionAnalyzer.CreateSetter (getterLambda);

			var fieldType    = lambda.ReturnType;
			var sourceType   = lambda.Parameters[0].Type;
			var lambdaMember = (MemberExpression) lambda.Body;

			bool nullable    = fieldType.IsNullable ();

			var getterFunc   = getterLambda.Compile ();
			var setterFunc   = setterLambda.Compile ();

			if (nullable)
			{
				fieldType = fieldType.GetNullableTypeUnderlyingType ();
			}

			var factoryType =  typeof (Factory<,>).MakeGenericType (sourceType, fieldType);
			var instance    = System.Activator.CreateInstance (factoryType, business, lambda, entityGetter, getterFunc, setterFunc, title, width, readOnly, nullable);

			return (DynamicFactory) instance;
		}

		#region Factory Class

		private sealed class Factory<TSource, TField> : DynamicFactory
			where TSource : AbstractEntity
			where TField : struct
		{
			public Factory(BusinessContext business, LambdaExpression lambda, System.Func<TSource> sourceGetter, System.Delegate getter, System.Delegate setter, string title, int width, bool readOnly, bool nullable)
			{
				this.business = business;
				this.lambda = lambda;
				this.sourceGetter = sourceGetter;
				this.getter = getter;
				this.setter = setter;
				this.title = title;
				this.width = width;
				this.readOnly = readOnly;
				this.nullable = nullable;
			}

			private System.Func<TField> CreateGetter()
			{
				return () => (TField) this.getter.DynamicInvoke (this.sourceGetter ());
			}

			private System.Action<TField> CreateSetter()
			{
				return x => BridgeSpy.ExecuteSetter (this.sourceGetter (), this.lambda, x,
					(entity, field) => this.setter.DynamicInvoke (entity, field),
					(entity) => (TField) this.getter.DynamicInvoke (entity));
			}

			private System.Func<TField?> CreateNullableGetter()
			{
				return () => (TField?) this.getter.DynamicInvoke (this.sourceGetter ());
			}

			private System.Action<TField?> CreateNullableSetter()
			{
				return x => BridgeSpy.ExecuteSetter (this.sourceGetter (), this.lambda, x,
					(entity, field) => this.setter.DynamicInvoke (entity, field),
					(entity) => (TField?) this.getter.DynamicInvoke (entity));
			}

			private Marshaler CreateMarshaler()
			{
				if (this.nullable)
				{
					return new NonNullableMarshaler<TField> (this.CreateGetter (), this.CreateSetter (), this.lambda);
				}
				else
				{
					return new NullableMarshaler<TField> (this.CreateNullableGetter (), this.CreateNullableSetter (), this.lambda);
				}
			}

			public override object CreateUI(FrameBox frame, UIBuilder builder)
			{
				IEnumerable<EnumKeyValues<TField>> possibleItems = EnumKeyValues.FromEnum<TField> ();

				var tile    = frame as EditionTile;
				var marshaler = this.CreateMarshaler ();
				var caption = DynamicFactory.GetInputCaption (this.lambda);
				var title   = this.title ?? DynamicFactory.GetInputTitle (caption);
				var widget  = builder.CreateAutoCompleteTextField<TField> (tile, this.width, this.readOnly, title, marshaler, possibleItems);
				
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
			private readonly bool readOnly;
			private readonly bool nullable;
		}

		#endregion
	}
}
