using Epsitec.Common.Support.EntityEngine;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	internal sealed class EntityReferencePropertyAccessor : AbstractPropertyAccessor
	{


		public EntityReferencePropertyAccessor(LambdaExpression lambda, string id)
			: base (lambda, FieldType.EntityReference, id)
		{
		}


		public override bool CheckValue(object value)
		{
			if (value == null)
			{
				return this.Property.IsNullable;
			}

			var entity = value as AbstractEntity;

			if (entity == null)
			{
				return false;
			}

			return this.Type.IsAssignableFrom (value.GetType ());
		}


	}


}

