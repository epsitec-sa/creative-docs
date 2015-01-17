//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{
	/// <summary>
	/// This class is the property accessor class used to read, write and check values for entity
	/// fields that are values, like int, decimal, etc.
	/// This kind of property accessor takes into account an optional field binder that might be
	/// associated to the property by Cresus.Designer. If there is one, it is used to convert the
	/// values back and forth from the entity property representation to the UI representation when
	/// required.
	/// </summary>
	internal sealed class ValuePropertyAccessor : AbstractPropertyAccessor
	{
		public ValuePropertyAccessor(LambdaExpression lambda, FieldType fieldType, string id)
			: base (lambda, fieldType, id)
		{
			this.fieldBinder = FieldBinderFactory.Create (this.Property.Type);
		}

		public override object GetValue(AbstractEntity entity)
		{
			var value = base.GetValue (entity);

			// Before giving the value to the javascript client,, we must conver it to its UI
			// representation by using the field binder.
			if (this.fieldBinder != null)
			{
				value = this.ConvertToUI (value);
			}

			return value;
		}

		public override void SetValue(AbstractEntity entity, object value)
		{
			// Before setting the value to the property, we must convert its UI reprensetation to
			// the one that will be used to store the value.
			if (this.fieldBinder != null)
			{
				value = this.ConvertFromUI (value);
			}

			base.SetValue (entity, value);
		}

		public override IValidationResult CheckValue(object value)
		{
			if (this.fieldBinder != null)
			{
				var validationResult = this.CheckValueFromUI (value);

				if (!validationResult.IsValid)
				{
					return validationResult;
				}

				//	Before checking if the constraints for the high level type of the
				//	associated property are met, convert the entered data so that it
				//	has the same format as what it will have when setting the value.

				value = this.ConvertFromUI (value);
			}

			var valid = this.CheckValueInternal (value);

			return ValidationResult.Create (valid);
		}

		private bool CheckValueInternal(object value)
		{
			if (value == null)
			{
				return this.Property.IsNullable;
			}

			var highLevelType = (AbstractType) this.Property.Type;

			return highLevelType.IsValidValue (value);
		}

		private object ConvertToUI(object value)
		{
			if (value is FormattedText)
			{
				return this.fieldBinder.ConvertToUI ((FormattedText) value);
			}
			else if (value is FormattedText?)
			{
				var textValue = ((FormattedText?) value) ?? FormattedText.Empty;

				return (FormattedText?) this.fieldBinder.ConvertToUI (textValue);
			}
			else if (value is string)
			{
				return this.fieldBinder.ConvertToUI ((string) value).ToString ();
			}
			else
			{
				return value;
			}
		}

		private object ConvertFromUI(object value)
		{
			if (value is FormattedText)
			{
				return this.fieldBinder.ConvertFromUI ((FormattedText) value);
			}
			else if (value is FormattedText?)
			{
				var textValue = ((FormattedText?) value) ?? FormattedText.Empty;

				return (FormattedText?) this.fieldBinder.ConvertFromUI (textValue);
			}
			else if (value is string)
			{
				return this.fieldBinder.ConvertFromUI ((string) value).ToString ();
			}
			else
			{
				return value;
			}
		}

		private IValidationResult CheckValueFromUI(object value)
		{
			if (value is FormattedText)
			{
				return this.fieldBinder.ValidateFromUI ((FormattedText) value);
			}
			else if (value is FormattedText?)
			{
				var textValue = ((FormattedText?) value) ?? FormattedText.Empty;

				return this.fieldBinder.ValidateFromUI (textValue);
			}
			else if (value is string)
			{
				return this.fieldBinder.ValidateFromUI ((string) value);
			}
			else
			{
				return ValidationResult.Create (true);
			}
		}

		private readonly IFieldBinder			fieldBinder;
	}
}
