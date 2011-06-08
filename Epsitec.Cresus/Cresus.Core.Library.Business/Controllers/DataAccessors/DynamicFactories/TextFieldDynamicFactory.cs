//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Types.Converters.Marshalers;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Linq.Expressions;
using System.Collections.Generic;
using Epsitec.Cresus.Core.Library;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors.DynamicFactories
{
	/// <summary>
	/// The <c>TextFieldDynamicFactory</c> creates a <see cref="DynamicFactory"/> for
	/// a string or value based field.
	/// </summary>
	internal static class TextFieldDynamicFactory
	{
		public static DynamicFactory Create<T>(BusinessContext business, LambdaExpression lambda, System.Func<T> entityGetter, string title, int width)
		{
			var lambdaMember = (MemberExpression) lambda.Body;
			var propertyInfo = lambdaMember.Member as System.Reflection.PropertyInfo;
			var typeField    = EntityInfo.GetStructuredTypeField (propertyInfo);

			var fieldType    = lambda.ReturnType;
			var sourceType   = lambda.Parameters[0].Type;
			
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

			bool nullable    = false;

			if ((fieldType.IsGenericType) &&
				(fieldType.GetGenericTypeDefinition () == typeof (System.Nullable<>)))
			{
				nullable  = true;
				fieldType = fieldType.GetGenericArguments ()[0];
			}


			if ((typeField != null) &&
				(fieldType == typeof (string)) &&
				(typeField.TypeId == Druid.Parse ("[8VAF1]")))	//	Data.String.EntityId
			{
				var list        = new List<Druid> (EntityClassFactory.GetAllEntityIds ());
				var factoryType = typeof (EntityIdFactory<>).MakeGenericType (sourceType);
				var instance    = System.Activator.CreateInstance (factoryType, business, lambda, entityGetter, getterFunc, setterFunc, title, width, list);

				return (DynamicFactory) instance;
			}
			else
			{
				var factoryType = (nullable ? typeof (NullableTextFieldFactory<,>) : typeof (TextFieldFactory<,>)).MakeGenericType (sourceType, fieldType);
				var instance    = System.Activator.CreateInstance (factoryType, business, lambda, entityGetter, getterFunc, setterFunc, title, width);

				return (DynamicFactory) instance;
			}
		}

		#region TextFieldFactory Class

		private sealed class TextFieldFactory<TSource, TField> : DynamicFactory
		{
			public TextFieldFactory(BusinessContext business, LambdaExpression lambda, System.Func<TSource> sourceGetter, System.Delegate getter, System.Delegate setter, string title, int width)
			{
				this.business = business;
				this.lambda   = lambda;
				this.sourceGetter = sourceGetter;
				this.getter   = getter;
				this.setter   = setter;
				this.title    = title;
				this.width    = width;
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

			public override object CreateUI(FrameBox frame, UIBuilder builder)
			{
				var tile      = frame as EditionTile;
				var marshaler = this.CreateMarshaler ();
				var caption   = DynamicFactory.GetInputCaption (this.lambda);
				var title     = this.title ?? DynamicFactory.GetInputTitle (caption);

				TextFieldEx widget;

				if (tile != null)
				{
					widget = builder.CreateTextField (tile, width, title, marshaler);
				}
				else
				{
					widget = builder.CreateTextField (frame, DockStyle.Stacked, width, marshaler);
				}

				if ((caption != null) &&
					(caption.HasDescription))
				{
					ToolTip.SetToolTipCaption (widget, caption);
				}

				return widget;
			}


			private readonly BusinessContext		business;
			private readonly LambdaExpression		lambda;
			private readonly System.Func<TSource>	sourceGetter;
			private readonly System.Delegate		getter;
			private readonly System.Delegate		setter;
			private readonly string					title;
			private readonly int					width;
		}

		#endregion

		#region NullableTextFieldFactory Class

		private sealed class NullableTextFieldFactory<TSource, TField> : DynamicFactory
			where TField : struct
		{
			public NullableTextFieldFactory(BusinessContext business, LambdaExpression lambda, System.Func<TSource> sourceGetter, System.Delegate getter, System.Delegate setter, string title, int width)
			{
				this.business = business;
				this.lambda   = lambda;
				this.sourceGetter = sourceGetter;
				this.getter   = getter;
				this.setter   = setter;
				this.title    = title;
				this.width    = width;
			}

			private System.Func<TField?> CreateGetter()
			{
				return () => (TField?) this.getter.DynamicInvoke (this.sourceGetter ());
			}

			private System.Action<TField?> CreateSetter()
			{
				return x => this.setter.DynamicInvoke (this.sourceGetter (), x);
			}

			private Marshaler CreateMarshaler()
			{
				return new NullableMarshaler<TField> (this.CreateGetter (), this.CreateSetter (), this.lambda);
			}

			public override object CreateUI(FrameBox frame, UIBuilder builder)
			{
				var tile      = frame as EditionTile;
				var marshaler = this.CreateMarshaler ();
				var caption   = DynamicFactory.GetInputCaption (this.lambda);
				var title     = this.title ?? DynamicFactory.GetInputTitle (caption);

				TextFieldEx widget;

				if (tile != null)
				{
					widget = builder.CreateTextField (tile, width, title, marshaler);
				}
				else
				{
					widget = builder.CreateTextField (frame, DockStyle.Stacked, width, marshaler);
				}

				if ((caption != null) &&
					(caption.HasDescription))
				{
					ToolTip.SetToolTipCaption (widget, caption);
				}

				return widget;
			}


			private readonly BusinessContext		business;
			private readonly LambdaExpression		lambda;
			private readonly System.Func<TSource>	sourceGetter;
			private readonly System.Delegate		getter;
			private readonly System.Delegate		setter;
			private readonly string					title;
			private readonly int					width;
		}

		#endregion

		#region EntityIdFactory Class

		/// <summary>
		/// The <c>EntityIdFactory</c> class glues together a getter/setter with the UI.
		/// The field is a string and gets mapped to a DRUID through the magic of the
		/// <see cref="EnumValueController&lt;Druid&gt;"/> and the marshalers.
		/// </summary>
		/// <typeparam name="TSource">The type of the entity on which the getter/setter operate.</typeparam>
		private sealed class EntityIdFactory<TSource> : DynamicFactory
		{
			public EntityIdFactory(BusinessContext business, LambdaExpression lambda, System.Func<TSource> sourceGetter, System.Delegate getter, System.Delegate setter, string title, int width, IEnumerable<Druid> entityIds)
			{
				this.business = business;
				this.lambda = lambda;
				this.sourceGetter = sourceGetter;
				this.getter = getter;
				this.setter = setter;
				this.title  = title;
				this.width  = width;
				this.entityIds = new List<Druid> (entityIds);
			}

			private System.Func<string> CreateGetter()
			{
				return () => (string) this.getter.DynamicInvoke (this.sourceGetter ());
			}

			private System.Action<string> CreateSetter()
			{
				return x => this.setter.DynamicInvoke (this.sourceGetter (), x);
			}

			private Marshaler CreateMarshaler()
			{
				return new NonNullableMarshaler<string> (this.CreateGetter (), this.CreateSetter (), this.lambda);
			}

			public override object CreateUI(FrameBox frame, UIBuilder builder)
			{
				IEnumerable<EnumKeyValues<Druid>> possibleItems = EnumKeyValues.FromEntityIds (this.entityIds);

				var tile    = frame as EditionTile;
				var marshaler = this.CreateMarshaler ();
				var caption = DynamicFactory.GetInputCaption (this.lambda);
				var title   = this.title ?? DynamicFactory.GetInputTitle (caption);
				var widget  = builder.CreateAutoCompleteTextField<Druid> (tile, this.width, title, marshaler, possibleItems);

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
			private readonly List<Druid> entityIds;
		}

		#endregion
	}
}
