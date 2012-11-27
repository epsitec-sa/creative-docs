using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using System;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


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

			if (this.fieldBinder != null)
			{
				value = this.ConvertToUI (value);
			}

			return value;
		}
		

		public override void SetValue(AbstractEntity entity, object value)
		{
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


		private readonly IFieldBinder fieldBinder;


	}


}
