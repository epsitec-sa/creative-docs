using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.WebCore.Server.Layout.TileData;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	internal sealed class EntityReferencePropertyAccessor : AbstractPropertyAccessor
	{


		public EntityReferencePropertyAccessor(LambdaExpression lambda, string id)
			: base (lambda, id)
		{
		}


		public override PropertyAccessorType PropertyAccessorType
		{
			get
			{
				return PropertyAccessorType.EntityReference;
			}
		}


		public AbstractEntity GetEntity(AbstractEntity entity)
		{
			return (AbstractEntity) this.Getter.DynamicInvoke (entity);
		}


		public void SetEntity(AbstractEntity entity, AbstractEntity value)
		{
			this.Setter.DynamicInvoke (entity, value);
		}


		public bool CheckEntity(AbstractEntity entity, AbstractEntity value)
		{
			if (value == null)
			{
				return this.Property.IsNullable;
			}
			else
			{
				return this.Type.IsAssignableFrom (value.GetType ());
			}
		}


		public override void SetValue(AbstractEntity entity, object value)
		{
			this.SetEntity (entity, (AbstractEntity) value);
		}


		public override bool CheckValue(AbstractEntity entity, object value)
		{
			return (value == null || value is AbstractEntity)
				&& this.CheckEntity (entity, (AbstractEntity) value);
		}


	}


}

