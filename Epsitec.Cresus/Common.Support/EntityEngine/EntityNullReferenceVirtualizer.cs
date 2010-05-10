//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			if (!EntityNullReferenceVirtualizer.IsPatchedEntity (entity))
			{
				entity.SetModifiedValues (new Store (entity.GetModifiedValues (), entity));
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
		/// Determines whether the specified entity was patched using <see cref="PatchNullReferences"/>
		/// and still is unchanged.
		/// </summary>
		/// <param name="entity">The entity to check.</param>
		/// <returns>
		/// 	<c>true</c> if the specified entity is a patched and still unchanged entity; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsPatchedEntityStillUnchanged(AbstractEntity entity)
		{
			if (entity.InternalGetValueStores ().Select (store => store is Store).Cast<Store> ().All (store => store.ReadOnly))
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// The <c>Store</c> class implements the <c>GetValue</c> and <c>SetValue</c> accessors used
		/// by the entity class to access its internal data store; <c>Store.GetValue</c> handles undefined
		/// references by instantiating empty entities on the fly, whereas <c>Store.SetValue</c> on such
		/// an empty entity will transform it into a real entity. Warning: avanced magic is going on here.
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
				this.ParentStore = parentStore;
				this.FieldIdInParentStore = fieldIdInParentStore;
			}

			

			public bool ReadOnly
			{
				get
				{
					return this.ParentStore != null;
				}
			}

			private Store ParentStore
			{
				get;
				set;
			}

			private string FieldIdInParentStore
			{
				get;
				set;
			}

			#region IValueStore Members

			public object GetValue(string id)
			{
				object value = this.realStore.GetValue (id);

				if (UndefinedValue.IsUndefinedValue (value))
				{
					if (this.values.TryGetValue (id, out value))
					{
						return value;
					}
					else
					{
						return this.CreateEmptyEntityForField (id);
					}
				}

				return value;
			}

			public void SetValue(string id, object value, ValueStoreSetMode mode)
			{
				this.RealizeEntity ();
				this.ReplaceValue (id, value, mode);
			}

			#endregion

			private void ReplaceValue(string id, object value, ValueStoreSetMode mode)
			{
				this.realStore.SetValue (id, value, mode);
				this.values.Remove (id);
			}

			private void RealizeEntity()
			{
				if (this.ReadOnly)
				{
					this.ParentStore.SetLiveEntity (this.FieldIdInParentStore, this.entity);
					this.ParentStore = null;

					this.entity.ReplaceEntityContext (EntityContext.Current);
				}
			}

			private void SetLiveEntity(string fieldId, AbstractEntity entity)
			{
				//	Make sure this is a real entity, before recording the entity into the specified
				//	reference field:

				this.RealizeEntity ();
				this.ReplaceValue (fieldId, entity, ValueStoreSetMode.Default);
				
				this.entity.UpdateDataGeneration ();
			}

			
			private static void PatchNullReferences(AbstractEntity entity, Store parentStore, string id)
			{
				var store = new Store (entity.GetModifiedValues (), entity, parentStore, id);
				entity.SetModifiedValues (store);
			}

			private AbstractEntity CreateEmptyEntityForField(string id)
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

				var info = type.GetField (id);

				if (info.Relation == FieldRelation.Reference)
				{
					var entity = EntityNullReferenceVirtualizer.CreateEmptyEntity (info.TypeId);

					if (entity != null)
					{
						Store.PatchNullReferences (entity, this, id);
						this.values.Add (id, entity);
					}
				}

				return null;
			}

			private readonly IValueStore realStore;
			private readonly Dictionary<string, object> values;
			private readonly AbstractEntity entity;
		}

		private static AbstractEntity CreateEmptyEntity(Druid druid)
		{
			var context = EntityNullReferenceVirtualizer.GetEmptyEntitiesContext ();
			return context.CreateEmptyEntity (druid);
		}

		private static EntityContext GetEmptyEntitiesContext()
		{
			var context = EntityNullReferenceVirtualizer.emptyEntitiesContext;

			if (EntityNullReferenceVirtualizer.emptyEntitiesContext == null)
			{
				EntityNullReferenceVirtualizer.emptyEntitiesContext = new EntityContext (Resources.DefaultManager, EntityLoopHandlingMode.Throw);
			}

			return EntityNullReferenceVirtualizer.emptyEntitiesContext;
		}

		[System.ThreadStatic]
		private static EntityContext emptyEntitiesContext;
	}
}
