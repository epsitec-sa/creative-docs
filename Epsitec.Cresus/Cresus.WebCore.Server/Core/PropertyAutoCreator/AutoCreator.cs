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


		public AutoCreator(LambdaExpression lambda, int id)
		{
			this.id = id;
			this.autoCreator = this.GetAutoCreator (lambda);
		}


		public int Id
		{
			get
			{
				return this.id;
			}
		}


		public AbstractEntity Execute(BusinessContext businessContext, AbstractEntity entity)
		{
			return this.autoCreator (businessContext, entity);
		}


		private Func<BusinessContext, AbstractEntity, AbstractEntity> GetAutoCreator(LambdaExpression lambda)
		{
			var accessor = new EntityReferencePropertyAccessor (lambda, 0);

			var childType = accessor.Type;
			var childTypeId = EntityInfo.GetTypeId (childType);

			return (b, e) =>
			{
				var child = accessor.GetEntity (e);

				if (child.IsNull ())
				{
					child = b.CreateEntity (childTypeId);

					accessor.SetEntity (e, child);
				}

				return child;
			};
		}


		private readonly int id;


		private readonly Func<BusinessContext, AbstractEntity, AbstractEntity> autoCreator;



	}


}
