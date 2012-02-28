using Epsitec.Common.Support.EntityEngine;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.PropertyAccessor
{


	internal sealed class EntityReferencePropertyAccessor : AbstractPropertyAccessor
	{


		public EntityReferencePropertyAccessor(LambdaExpression lambda, string id)
			: base (lambda, id)
		{
		}


		public void SetEntityValue(AbstractEntity entity, AbstractEntity value)
		{
			this.Setter.DynamicInvoke (entity, value);
		}


		public AbstractEntity GetEntity(AbstractEntity entity)
		{
			return (AbstractEntity) this.Getter.DynamicInvoke (entity);
		}


	}


}

