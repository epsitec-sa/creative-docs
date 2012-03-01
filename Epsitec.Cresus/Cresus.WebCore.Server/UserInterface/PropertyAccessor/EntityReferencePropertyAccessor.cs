using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using System.Linq.Expressions;

using System;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.PropertyAccessor
{


	internal sealed class EntityReferencePropertyAccessor : AbstractPropertyAccessor
	{


		public EntityReferencePropertyAccessor(LambdaExpression lambda, string id)
			: base (lambda, id)
		{
		}


		public AbstractEntity GetEntity(AbstractEntity entity)
		{
			return (AbstractEntity) this.Getter.DynamicInvoke (entity);
		}


		public void SetEntityValue(AbstractEntity entity, AbstractEntity value)
		{
			this.Setter.DynamicInvoke (entity, value);
		}


		public override void SetValue(AbstractEntity entity, object value)
		{
			this.SetEntityValue (entity, (AbstractEntity) value);
		}


		public override bool CheckValue(AbstractEntity entity, object value)
		{
			return this.Type.IsAssignableFrom (value.GetType ())
				&& (value != null || this.Property.IsNullable);
		}


	}


}

