using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using System;

using System.Linq.Expressions;

using System.Reflection;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	internal abstract class AbstractStringPropertyAccessor : AbstractPropertyAccessor
	{


		// NOTE The way we check the values are valid is ugly but I didn't found any better way. We
		// use reflexion to obtain the converter used by the marshaler to convert the values. Then
		// we use that converter to convert the value and check this value against the property type
		// validator. This implies that the value is being converted twice, which is stupid.


		public AbstractStringPropertyAccessor(LambdaExpression lambda, int id)
			: base (lambda, id)
		{
			this.marshalerFactory = this.GetMarshalerFactory (lambda);
			this.valueConverter = this.GetValueConverter ();
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


		private Func<string, Tuple<bool, object>> GetValueConverter()
		{
			Type systemType = this.Type;

			if (systemType.IsNullable ())
			{
				systemType = systemType.GetNullableTypeUnderlyingType ();
			}

			var classType = typeof (AbstractStringPropertyAccessor);
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

				var success = conversionResult.IsValid;

				object result;

				if (conversionResult.HasValue)
				{
					result = conversionResult.Value;
				}
				else if (conversionResult.IsNull)
				{
					result = null;
				}
				else
				{
					result = default (T);
				}

				return Tuple.Create (success, result);
			};
		}


		private string GetString(AbstractEntity entity)
		{
			return this.marshalerFactory.CreateMarshaler (entity).GetStringValue ();
		}


		private void SetString(AbstractEntity entity, string value)
		{
			this.marshalerFactory.CreateMarshaler (entity).SetStringValue (value);
		}


		private bool CheckString(string value)
		{
			if (value == null)
			{
				return this.Property.IsNullable;
			}
			else
			{
				var highLevelType = (AbstractType) this.Property.Type;
				var conversionResult = this.valueConverter (value);

				return conversionResult.Item1
					&& highLevelType.IsValidValue (conversionResult.Item2);
			}
		}


		private object ConvertString(string value)
		{
			var conversionResult = this.valueConverter (value);

			if (!conversionResult.Item1)
			{
				throw new ArgumentException ();
			}

			return conversionResult.Item2;
		}


		public override void SetValue(AbstractEntity entity, object value)
		{
			var text = this.ConvertValueToText (value);

			this.SetString (entity, text);
		}


		public override bool CheckValue(AbstractEntity entity, object value)
		{
			var text = this.ConvertValueToText (value);

			return this.CheckString (text);
		}


		public object GetValue(AbstractEntity entity)
		{
			var text = this.GetString (entity);

			return this.ConvertTextToValue (text);
		}


		public object ConvertValue(object value)
		{
			var text = this.ConvertValueToText (value);

			return this.ConvertString (text);
		}


		protected abstract string ConvertValueToText(object value);


		protected abstract object ConvertTextToValue(string text);


		private readonly AbstractMarshalerFactory marshalerFactory;


		private readonly Func<string, Tuple<bool, object>> valueConverter;


	}


}
