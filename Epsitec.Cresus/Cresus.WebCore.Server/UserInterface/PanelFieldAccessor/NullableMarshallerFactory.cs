using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types.Converters;
using Epsitec.Common.Types.Converters.Marshalers;

using System;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.PanelFieldAccessor
{


	internal sealed class NullableMarshallerFactory<TField> : AbstractMarshallerFactory
		where TField : struct
	{


		public NullableMarshallerFactory(LambdaExpression lambda, Delegate getter, Delegate setter)
			: base (lambda)
		{
			this.getterPattern = e => (TField?) getter.DynamicInvoke (e);
			this.setterPattern = (e, f) => setter.DynamicInvoke (e, f);
		}


		public override Marshaler CreateMarshaler(AbstractEntity entity)
		{
			Func<TField?> getter = () => this.getterPattern (entity);
			Action<TField?> setter = f => this.setterPattern (entity, f);

			return new NullableMarshaler<TField> (getter, setter, this.Lambda);
		}


		private readonly Func<AbstractEntity, TField?> getterPattern;


		private readonly Action<AbstractEntity, TField?> setterPattern;


	}


}

