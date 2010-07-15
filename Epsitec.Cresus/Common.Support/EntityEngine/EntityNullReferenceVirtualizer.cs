﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityNullReferenceVirtualizer</c> provides the mechanism used to
	/// automatically generate empty entities when null references are dereferenced.
	/// </summary>
	public static class EntityNullReferenceVirtualizer
	{
		/// <summary>
		/// Patches the null references in the specified entity.
		/// </summary>
		/// <typeparam name="T">An entity type.</typeparam>
		/// <param name="entity">The entity which will be patched.</param>
		public static void PatchNullReferences<T>(T entity) where T : AbstractEntity
		{
			if (entity == null)
            {
				throw new System.NullReferenceException ();
            }

			if (!EntityNullReferenceVirtualizer.IsPatchedEntity (entity))
			{
//-				entity.SetModifiedValues (new Store (entity.GetModifiedValues (), entity));
				entity.SetOriginalValues (new Store (entity.GetOriginalValues (), entity));
			}
		}

		/// <summary>
		/// Determines whether the specified entity was patched using <see cref="PatchNullReferences"/>.
		/// </summary>
		/// <param name="entity">The entity to check.</param>
		/// <returns>
		/// 	<c>true</c> if the specified entity was patched; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsPatchedEntity(AbstractEntity entity)
		{
			if (entity == null)
            {
				return false;
            }
			
			if (entity.InternalGetValueStores ().Any (store => store is Store))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Determines whether the specified entity is null or is an empty virtualized entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>
		/// 	<c>true</c> if the specified entity is null or is an empty virtualized entity; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsNullEntity(AbstractEntity entity)
		{
			if (entity == null)
            {
				return true;
            }

			if (entity.GetEntityContext () is EmptyEntityContext)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Determines whether the specified context is an empty entity context.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>
		/// 	<c>true</c> if the specified context is an empty entity context; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsEmptyEntityContext(EntityContext context)
		{
			return context is EmptyEntityContext;
		}

		/// <summary>
		/// Determines whether the specified entity was patched using <see cref="PatchNullReferences"/>
		/// and still is unchanged.
		/// </summary>
		/// <param name="entity">The entity to check.</param>
		/// <returns>
		/// 	<c>true</c> if the specified entity is a patched and still unchanged entity; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsPatchedEntityStillUnchanged(AbstractEntity entity)
		{
			if (entity == null)
			{
				return false;
			}

			var stores = entity.InternalGetValueStores ().Where (store => store is Store).Cast<Store> ().ToArray ();

			if ((stores.Length > 0) &&
				(stores.All (store => store.IsReadOnly)))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Unwraps the entity. If it maps to a null entity, then return <c>null</c>.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="entity">The entity.</param>
		/// <returns><c>null</c> if the entity maps to a null entity; otherwise, the entity.</returns>
		public static T UnwrapNullEntity<T>(this T entity)
			where T : AbstractEntity
		{
			if (EntityNullReferenceVirtualizer.IsNullEntity (entity))
			{
				return null;
			}
			else
			{
				return entity;
			}
		}

		/// <summary>
		/// Wraps the entity. If it is <c>null</c>, then create a virtualized empty entity.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="entity">The entity.</param>
		/// <returns>The wrapped entity.</returns>
		public static T WrapNullEntity<T>(this T entity)
			where T : AbstractEntity, new ()
		{
			if (entity == null)
			{
				return EntityNullReferenceVirtualizer.CreateEmptyEntity<T> ();
			}
			else
			{
				return entity;
			}
		}

		/// <summary>
		/// Creates an empty entity which will be considered equal to a null reference
		/// when calling <see cref="IsNullEntity"/>.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <returns>The empty entity created the same way as the automatically generated entity produced by the virtualizer.</returns>
		public static T CreateEmptyEntity<T>()
			where T : AbstractEntity, new ()
		{
			var context = EntityNullReferenceVirtualizer.GetEmptyEntitiesContext ();
			var entity  = context.CreateEmptyEntity<T> ();

			EntityNullReferenceVirtualizer.PatchNullReferences (entity);
			
			return entity;
		}


		/// <summary>
		/// Creates an empty entity attached to a dedicated context.
		/// </summary>
		/// <param name="druid">The druid.</param>
		/// <returns>The empty entity.</returns>
		private static AbstractEntity CreateEmptyEntity(Druid druid)
		{
			var context = EntityNullReferenceVirtualizer.GetEmptyEntitiesContext ();
			return context.CreateEmptyEntity (druid);
		}

		/// <summary>
		/// Gets the context used to create empty entities.
		/// </summary>
		/// <returns>The <see cref="EntityContext"/>.</returns>
		private static EntityContext GetEmptyEntitiesContext()
		{
			var context = EntityNullReferenceVirtualizer.emptyEntitiesContext;

			if (EntityNullReferenceVirtualizer.emptyEntitiesContext == null)
			{
				EntityNullReferenceVirtualizer.emptyEntitiesContext = new EmptyEntityContext ();
			}

			return EntityNullReferenceVirtualizer.emptyEntitiesContext;
		}

		#region EmptyEntityContext class

		/// <summary>
		/// The <c>EmptyEntityContext</c> class will be used when creating empty entities.
		/// This allows the virtualizer to identify an empty entity very easily by just
		/// checking if it belongs to this context, or not.
		/// </summary>
		private class EmptyEntityContext : EntityContext
		{
			public EmptyEntityContext()
				: base (Resources.DefaultManager, EntityLoopHandlingMode.Throw, "EmptyEntities")
			{
			}
		}

		#endregion

		#region Store Class

		/// <summary>
		/// The <c>Store</c> class implements the <c>GetValue</c> and <c>SetValue</c> accessors used
		/// by the entity class to access its internal data store; <c>Store.GetValue</c> handles undefined
		/// references by instantiating empty entities on the fly, whereas <c>Store.SetValue</c> on such
		/// an empty entity will transform it into a real entity. Warning: advanced magic is going on here.
		/// </summary>
		class Store : IValueStore
		{
			public Store(IValueStore realStore, AbstractEntity entity)
			{
				this.realStore = realStore;
				this.values = new Dictionary<string, object> ();
				this.entity = entity;
			}

			public Store(IValueStore realStore, AbstractEntity entity, Store parentStore, string fieldIdInParentStore)
				: this (realStore, entity)
			{
				this.parentStore = parentStore;
				this.fieldIdInParentStore = fieldIdInParentStore;
				this.isReadOnly = true;
			}
			
			public bool IsReadOnly
			{
				get
				{
					return this.isReadOnly;
				}
			}

			#region IValueStore Members


			public object GetValue(string id)
			{
				object value = this.realStore.GetValue (id);

				if ((UndefinedValue.IsUndefinedValue (value)) ||
					(value == null))
				{
					object outValue;

					if (this.values.TryGetValue (id, out outValue))
					{
						return outValue;
					}
					else
					{
						return this.CreateEmptyEntityForUndefinedField (id, value);
					}
				}

				if (value is AbstractEntity)
				{
					var entity = value as AbstractEntity;
					EntityNullReferenceVirtualizer.PatchNullReferences (entity);
				}
				else if (value is EntityCollection)
				{
					var collection = value as EntityCollection;
					collection.EnableEntityNullReferenceVirtualizer ();
				}

				return value;
			}


			public void SetValue(string id, object value, ValueStoreSetMode mode)
			{
				this.TranformNullEntityInfoLiveEntity ();
				this.ReplaceValue (id, value, mode);
			}

			
			#endregion

			private void TranformNullEntityInfoLiveEntity()
			{
				if (this.IsReadOnly)
				{
					this.parentStore.SetLiveEntity (this.fieldIdInParentStore, this.entity);
					this.isReadOnly = false;

					this.entity.ReplaceEntityContext (EntityContext.Current);
				}
			}

			private void SetLiveEntity(string fieldId, AbstractEntity entity)
			{
				//	Make sure this is a real entity, before recording the entity into the specified
				//	reference field:

				this.TranformNullEntityInfoLiveEntity ();
				this.ReplaceValue (fieldId, entity, ValueStoreSetMode.Default);
				
				this.entity.UpdateDataGeneration ();
			}

			private void ReplaceValue(string id, object value, ValueStoreSetMode mode)
			{
				this.realStore.SetValue (id, value, mode);
				this.values.Remove (id);
			}

			private object CreateEmptyEntityForUndefinedField(string id, object defaultValue)
			{
				var info = this.GetStructuredTypeField (id);

				if (info == null)
				{
					return defaultValue;
				}

				if (info.Relation == FieldRelation.Reference)
				{
					var entity = EntityNullReferenceVirtualizer.CreateEmptyEntity (info.TypeId);

					if (entity != null)
					{
						Store.PatchNullReferences (entity, this, id);
						this.values.Add (id, entity);
					}

					return entity;
				}

				return defaultValue;
			}

			private StructuredTypeField GetStructuredTypeField(string id)
			{
				var provider = this.realStore as IStructuredTypeProvider;

				if (provider == null)
				{
					return null;
				}

				var type = provider.GetStructuredType () as StructuredType;

				if (type == null)
				{
					return null;
				}

				return type.GetField (id);
			}

			private static void PatchNullReferences(AbstractEntity entity, Store parentStore, string id)
			{
				var store = new Store (entity.GetModifiedValues (), entity, parentStore, id);
				entity.SetModifiedValues (store);
			}

			private readonly IValueStore				realStore;
			private readonly Dictionary<string, object>	values;
			private readonly AbstractEntity				entity;
			private readonly Store						parentStore;
			private readonly string						fieldIdInParentStore;
			private bool								isReadOnly;
		}

		#endregion

		[System.ThreadStatic]
		private static EntityContext emptyEntitiesContext;
	}
}
