using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using System;

using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor
{


	internal sealed class EntityCollectionPropertyAccessor : AbstractPropertyAccessor
	{


		public EntityCollectionPropertyAccessor(LambdaExpression lambda, string id)
			: base (lambda, FieldType.EntityCollection, id)
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


		public override void SetValue(AbstractEntity entity, object value)
		{
			var collection = (IList) this.GetValue (entity);
			var items = (IEnumerable) value;

			using (collection.SuspendNotifications ())
			{
				collection.Clear ();
				collection.AddRange (items);
			}
		}


		public override bool CheckValue(object value)
		{
			if (value == null)
			{
				return this.Property.IsNullable;
			}

			var collection = value as IEnumerable<AbstractEntity>;

			if (collection == null)	
			{
				return false;
			}

			return collection.All (e => e != null)
				&& collection.All (e => this.collectionType.IsAssignableFrom (e.GetType ()));
		}


		private readonly Type collectionType;


	}


}

