using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using System;

using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.UserInterface.PropertyAccessor
{


	internal sealed class EntityCollectionPropertyAccessor : AbstractPropertyAccessor
	{


		public EntityCollectionPropertyAccessor(LambdaExpression lambda, string id)
			: base (lambda, id)
		{
			this.collectionType = lambda.ReturnType.GetGenericArguments ().Single ();
		}


		public Type CollectionType
		{
			get
			{
				return this.collectionType;
			}
		}


		public IList GetCollection(AbstractEntity entity)
		{
			return (IList) this.Getter.DynamicInvoke (entity);
		}


		public void SetCollection(AbstractEntity entity, IEnumerable<AbstractEntity> items)
		{
			var collection = this.GetCollection (entity);

			using (collection.SuspendNotifications ())
			{
				collection.Clear ();
				collection.AddRange (items);
			}
		}


		private readonly Type collectionType;


	}


}

