using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using System;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.PanelFieldAccessor
{


	internal sealed class StringPanelFieldAccessor : AbstractPanelFieldAccessor
	{


		public StringPanelFieldAccessor(LambdaExpression lambda, string id)
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


		private readonly AbstractMarshallerFactory marshalerFactory;


	}


}

