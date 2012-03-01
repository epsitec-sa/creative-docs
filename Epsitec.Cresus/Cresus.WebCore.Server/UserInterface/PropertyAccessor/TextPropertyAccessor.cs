using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using System;

using System.Linq.Expressions;
using Epsitec.Common.Types;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.PropertyAccessor
{


	internal sealed class TextPropertyAccessor : AbstractPropertyAccessor
	{


		public TextPropertyAccessor(LambdaExpression lambda, string id)
			: base (lambda, id)
		{
			this.marshalerFactory = this.GetMarshallerFactory (lambda);
		}


		private AbstractMarshallerFactory GetMarshallerFactory(LambdaExpression lambda)
		{
			Type fieldType = lambda.ReturnType;
			Type factoryType;

			if (fieldType.IsNullable ())
			{
				fieldType = fieldType.GetNullableTypeUnderlyingType ();
				factoryType = typeof (NullableMarshallerFactory<>);
			}
			else
			{
				factoryType = typeof (NonNullableMarshallerFactory<>);
			}

			var factoryGenericType = factoryType.MakeGenericType (fieldType);
			var factory = Activator.CreateInstance (factoryGenericType, lambda, this.Getter, this.Setter);

			return (AbstractMarshallerFactory) factory;
		}


		public string GetString(AbstractEntity entity)
		{
			return this.marshalerFactory.CreateMarshaler (entity).GetStringValue ();
		}


		public void SetString(AbstractEntity entity, string value)
		{
			this.marshalerFactory.CreateMarshaler (entity).SetStringValue (value);
		}


		public override void SetValue(AbstractEntity entity, object value)
		{
			this.SetString (entity, (string) value);
		}


		public override bool CheckValue(AbstractEntity entity, object value)
		{
			// TODO This only checks that the value is convertible from the given text to something
			// meaningful in the system type underlying the property. If the high level type of the
			// property has additional constraints (such as a range for numeric values, or size
			// constraints on text values), they are not taken in account here. This is a bug that
			// should be fixed.
			
			var marshaler = this.marshalerFactory.CreateMarshaler (entity);

			var text = value as string;
			
			return text != null
				&& marshaler.CanConvert (text);
		}


		private readonly AbstractMarshallerFactory marshalerFactory;


	}


}

