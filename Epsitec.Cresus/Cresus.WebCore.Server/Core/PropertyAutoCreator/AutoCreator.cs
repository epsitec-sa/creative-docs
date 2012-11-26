using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using System;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAutoCreator
{
	
	
	internal sealed class AutoCreator
	{


		public AutoCreator(LambdaExpression lambda, string id)
		{
			this.id = id;
			this.propertyAccessor = new EntityReferencePropertyAccessor (lambda, "");
		}


		public string Id
		{
			get
			{
				return this.id;
			}
		}


		public AbstractEntity Execute(BusinessContext businessContext, AbstractEntity entity)
		{
			var childType = this.propertyAccessor.Type;
			var childTypeId = EntityInfo.GetTypeId (childType);

			var child = (AbstractEntity) this.propertyAccessor.GetValue (entity);

			if (child.IsNull ())
			{
				child = businessContext.CreateEntity (childTypeId);

				propertyAccessor.SetValue (entity, child);
			}

			return child;
		}


		private readonly string id;


		private readonly EntityReferencePropertyAccessor propertyAccessor;



	}


}
