//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Types.Converters.Marshalers;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Linq.Expressions;
using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Bricks.Factories
{
	/// <summary>
	/// The <c>TextFieldDynamicFactory</c> creates a <see cref="DynamicFactory"/> for
	/// a string or value based field.
	/// </summary>
	internal static class TextFieldDynamicFactory
	{
		public static DynamicFactory Create<T>(BusinessContext business, LambdaExpression lambda, System.Func<T> entityGetter, string title, int width, int height, bool readOnly, System.Collections.IEnumerable collection)
		{
			var getterLambda = lambda;
			var setterLambda = ExpressionAnalyzer.CreateSetter (getterLambda);

			var lambdaMember = (MemberExpression) lambda.Body;
			var propertyInfo = lambdaMember.Member as System.Reflection.PropertyInfo;
			var typeField    = EntityInfo.GetStructuredTypeField (propertyInfo);
			var fieldType    = lambda.ReturnType;
			var sourceType   = lambda.Parameters[0].Type;
			
			bool nullable    = fieldType.IsNullable ();

			var getterFunc   = getterLambda.Compile ();
			var setterFunc   = setterLambda.Compile ();

			if (nullable)
			{
				fieldType = fieldType.GetNullableTypeUnderlyingType ();
			}

			//	TODO: improve the special case handling here -- probably should make something
			//	truly dynamic with plug-ins.

			CreateWidget callback = null;

			if ((typeField != null) &&
				(height == 0))
			{
				if (fieldType == typeof (string))
				{
					if (typeField.TypeId == Druid.Parse ("[8VAF1]"))		//	Data.String.EntityId
					{
						var list  = new List<Druid> (collection as IEnumerable<Druid> ?? EntityInfo.GetAllTypeIds ());
						var items = EnumKeyValues.FromEntityIds (list);

						callback = (frame, builder, caption, marshaler) => builder.CreateAutoCompleteTextField<Druid> (frame as EditionTile, width, readOnly, caption, marshaler, items);
					}
					else if (typeField.TypeId == Druid.Parse ("[CVAK]"))	//	Finance.BookAccount
					{
						callback = (frame, builder, caption, marshaler) => builder.CreateAccountEditor (frame as EditionTile, caption, readOnly, marshaler);
					}
				}

				if ((fieldType == typeof (string)) ||
					(fieldType == typeof (FormattedText)))
				{
					if (StringType.IsMultilineText (typeField.Type))
					{
						height = 60;
					}
				}

				if ((fieldType == typeof (bool)) ||
					(fieldType == typeof (bool?)))
				{
					callback = (frame, builder, caption, marshaler) => builder.CreateCheckButton (frame as EditionTile, width, readOnly, caption, marshaler);
				}
			}

			if (callback == null)
			{
				//	Default text field creation callback:

				callback = (frame, builder, caption, marshaler) => TextFieldDynamicFactory.CreateTextField (frame, builder, caption, marshaler, width, height, readOnly);
			}

			var factoryType = (nullable ? typeof (NullableTextFieldFactory<,>) : typeof (TextFieldFactory<,>)).MakeGenericType (sourceType, fieldType);
			var instance    = System.Activator.CreateInstance (factoryType, business, lambda, entityGetter, getterFunc, setterFunc, title, callback) as DynamicFactory;

			return instance;
		}

		private static Widget CreateTextField(FrameBox frame, UIBuilder builder, string title, Marshaler marshaler, int width, int height, bool readOnly)
		{
			var tile = frame as EditionTile;
			
			if (tile != null)
			{
				if (height > 0)
				{
					return builder.CreateTextFieldMulti (tile, height, readOnly, title, marshaler);
				}
				else
				{
					return builder.CreateTextField (tile, width, readOnly, title, marshaler);
				}
			}
			else
			{
				if (height > 0)
				{
					return builder.CreateTextFieldMulti (frame, DockStyle.Stacked, height, readOnly, marshaler);
				}
				else
				{
					return builder.CreateTextField (frame, DockStyle.Stacked, width, readOnly, marshaler);
				}
			}
		}

		delegate Widget CreateWidget(FrameBox frame, UIBuilder builder, string caption, Marshaler marshaler);
		
		
		#region TextFieldFactory Class

		private sealed class TextFieldFactory<TSource, TField> : DynamicFactory
			where TSource : AbstractEntity
		{
			public TextFieldFactory(BusinessContext business, LambdaExpression lambda, System.Func<TSource> sourceGetter, System.Delegate getter, System.Delegate setter, string title, CreateWidget createWidgetCallback)
			{
				this.business = business;
				this.lambda   = lambda;
				this.sourceGetter = sourceGetter;
				this.getter   = getter;
				this.setter   = setter;
				this.title    = title;
				this.createWidgetCallback = createWidgetCallback;
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

			private Marshaler CreateMarshaler()
			{
				return new NonNullableMarshaler<TField> (this.CreateGetter (), this.CreateSetter (), this.lambda);
			}

			public override object CreateUI(FrameBox frame, UIBuilder builder)
			{
				var marshaler = this.CreateMarshaler ();
				var caption   = DynamicFactory.GetInputCaption (this.lambda);
				var title     = this.title ?? DynamicFactory.GetInputTitle (caption);

				Widget widget = this.createWidgetCallback (frame, builder, title, marshaler);

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
			private readonly CreateWidget			createWidgetCallback;
		}

		#endregion

		#region NullableTextFieldFactory Class

		private sealed class NullableTextFieldFactory<TSource, TField> : DynamicFactory
			where TSource : AbstractEntity
			where TField : struct
		{
			public NullableTextFieldFactory(BusinessContext business, LambdaExpression lambda, System.Func<TSource> sourceGetter, System.Delegate getter, System.Delegate setter, string title, CreateWidget createWidgetCallback)
			{
				this.business = business;
				this.lambda   = lambda;
				this.sourceGetter = sourceGetter;
				this.getter   = getter;
				this.setter   = setter;
				this.title    = title;
				this.createWidgetCallback = createWidgetCallback;
			}

			private System.Func<TField?> CreateGetter()
			{
				return () => (TField?) this.getter.DynamicInvoke (this.sourceGetter ());
			}

			private System.Action<TField?> CreateSetter()
			{
				return x => BridgeSpy.ExecuteSetter (this.sourceGetter (), this.lambda, x,
					(entity, field) => this.setter.DynamicInvoke (entity, field),
					(entity) => (TField?) this.getter.DynamicInvoke (entity));
			}

			private Marshaler CreateMarshaler()
			{
				return new NullableMarshaler<TField> (this.CreateGetter (), this.CreateSetter (), this.lambda);
			}

			public override object CreateUI(FrameBox frame, UIBuilder builder)
			{
				var marshaler = this.CreateMarshaler ();
				var caption   = DynamicFactory.GetInputCaption (this.lambda);
				var title     = this.title ?? DynamicFactory.GetInputTitle (caption);

				Widget widget = this.createWidgetCallback (frame, builder, title, marshaler);

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
			private readonly CreateWidget			createWidgetCallback;
		}

		#endregion
	}
}
