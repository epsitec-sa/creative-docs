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


		/// <remarks>
		/// The collection returned by this method is not the original one but a copy. Therefore,
		/// you should not modify it and expect the modifications to be propagated to the real
		/// collection.
		/// </remarks>
		public IList<AbstractEntity> GetEntityCollection(AbstractEntity entity)
		{
			return this.GetCollection (entity).Cast<AbstractEntity> ().ToList ();
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


		public override void SetValue(AbstractEntity entity, object value)
		{
			this.SetCollection (entity, (IEnumerable<AbstractEntity>) value);
		}


		public override bool CheckValue(AbstractEntity entity, object value)
		{
			var list = value as IEnumerable<AbstractEntity>;

			return list != null
				&& list.All (e => e != null)
				&& list.All (e => this.collectionType.IsAssignableFrom (e.GetType ()));
		}


		private readonly Type collectionType;


	}


}

