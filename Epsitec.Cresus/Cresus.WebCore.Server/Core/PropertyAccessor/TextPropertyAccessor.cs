using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using System;

using System.Linq.Expressions;

using System.Reflection;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	internal sealed class TextPropertyAccessor : AbstractPropertyAccessor
	{


		// NOTE The way we check the values are valid is ugly but I didn't found any better way. We
		// use reflexion to obtain the converter used by the marshaler to convert the values. Then
		// we use that converter to convert the value and check this value against the property type
		// validator. This implies that the value is being converted twice, which is stupid.


		public TextPropertyAccessor(LambdaExpression lambda, int id)
			: base (lambda, id)
		{
			this.isTextProperty = this.IsTextProperty ();
			this.marshalerFactory = this.GetMarshalerFactory (lambda);
			this.valueChecker = this.GetValueChecker ();
		}


		private bool IsTextProperty()
		{
			return this.Type == typeof (string)
				|| this.Type == typeof (FormattedText)
				|| this.Type == typeof (FormattedText?);
		}


		private AbstractMarshalerFactory GetMarshalerFactory(LambdaExpression lambda)
		{
			Type fieldType = lambda.ReturnType;
			Type factoryType;

			if (fieldType.IsNullable ())
			{
				fieldType = fieldType.GetNullableTypeUnderlyingType ();
				factoryType = typeof (NullableMarshalerFactory<>);
			}
			else
			{
				factoryType = typeof (NonNullableMarshalerFactory<>);
			}

			var factoryGenericType = factoryType.MakeGenericType (fieldType);
			var factory = Activator.CreateInstance (factoryGenericType, lambda, this.Getter, this.Setter);

			return (AbstractMarshalerFactory) factory;
		}


		private Func<string, bool> GetValueChecker()
		{
			var highLevelType = (AbstractType) this.Property.Type;
			var lowLevelValueConverter = TextPropertyAccessor.GetValueConverter (this.Type);

			return value =>
			{
				var conversionResult = lowLevelValueConverter (value);

				return conversionResult.Item1
					&& highLevelType.IsValidValue (conversionResult.Item2);
			};
		}


		private static Func<string, Tuple<bool, object>> GetValueConverter(Type type)
		{
			Type systemType = type;

			if (systemType.IsNullable ())
			{
				systemType = systemType.GetNullableTypeUnderlyingType ();
			}

			var classType = typeof (TextPropertyAccessor);
			var methodName = "GetValueConverterImplementation";
			var bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;

			var method = classType.GetMethod (methodName, bindingFlags);
			var genericMethod = method.MakeGenericMethod (systemType);

			return (Func<string, Tuple<bool, object>>) genericMethod.Invoke (null, new object[0]);
		}


		private static Func<string, Tuple<bool, object>> GetValueConverterImplementation<T>()
		{
			var converter = GenericConverter.GetConverter<T> ();

			return value =>
			{
				var conversionResult = converter.ConvertFromString (value);

				var success = conversionResult.HasValue;		
				object result = success ? conversionResult.Value : default(T);

				return Tuple.Create (success, result);
			};


			throw new NotImplementedException ();
		}	


		public string GetString(AbstractEntity entity)
		{
			return this.marshalerFactory.CreateMarshaler (entity).GetStringValue ();
		}


		public void SetString(AbstractEntity entity, string value)
		{
			var processedValue = this.ProcessStringValue (value);

			this.marshalerFactory.CreateMarshaler (entity).SetStringValue (processedValue);
		}


		public bool CheckString(AbstractEntity entity, string value)
		{
			var processedValue = this.ProcessStringValue (value);

			if (processedValue == null)
			{
				return this.Property.IsNullable;
			}
			else
			{
				return this.valueChecker (processedValue);
			}
		}


		private string ProcessStringValue(string value)
		{
			// NOTE Because of how the marshaler for text values works, we must replace null strings
			// with empty strings if the property is a text property. NonNullableMarshalers consider
			// null values as invalid and resets the entity property if it is given a null value.
			
			return this.isTextProperty
				? value ?? ""
				: value;
		}


		public override void SetValue(AbstractEntity entity, object value)
		{
			this.SetString (entity, (string) value);
		}


		public override bool CheckValue(AbstractEntity entity, object value)
		{
			return (value == null || value is string)
				&& this.CheckString (entity, (string) value);
		}


		private readonly bool isTextProperty;


		private readonly AbstractMarshalerFactory marshalerFactory;


		private readonly Func<string, bool> valueChecker;


	}


}

