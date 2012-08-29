using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.WebCore.Server.Layout.TileData;

using System;

using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	internal sealed class EntityCollectionPropertyAccessor : AbstractPropertyAccessor
	{


		public EntityCollectionPropertyAccessor(LambdaExpression lambda, int id)
			: base (lambda, id)
		{
			this.collectionType = lambda.ReturnType.GetGenericArguments ().Single ();
		}


		public override PropertyAccessorType PropertyAccessorType
		{
			get
			{
				return PropertyAccessorType.EntityCollection;
			}
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


		public void SetEntityCollection(AbstractEntity entity, IEnumerable<AbstractEntity> items)
		{
			var collection = this.GetCollection (entity);

			using (collection.SuspendNotifications ())
			{
				collection.Clear ();
				collection.AddRange (items);
			}
		}


		public bool CheckEntityCollection(AbstractEntity entity, IEnumerable<AbstractEntity> entities)
		{
			return entities.All (e => e != null)
				&& entities.All (e => this.collectionType.IsAssignableFrom (e.GetType ()));
		}


		public override void SetValue(AbstractEntity entity, object value)
		{
			this.SetEntityCollection (entity, (IEnumerable<AbstractEntity>) value);
		}


		public override bool CheckValue(AbstractEntity entity, object value)
		{
			return (value == null || value is IEnumerable<AbstractEntity>)
				&& this.CheckEntityCollection (entity, (IEnumerable<AbstractEntity>) value);
		}


		private readonly Type collectionType;


	}


}

