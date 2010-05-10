//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	public static class EntityNullReferenceVirtualizer
	{
		public static void Virtualize<T>(T entity) where T : AbstractEntity
		{
			if (!EntityNullReferenceVirtualizer.IsVirtualizedEntity (entity))
			{
				entity.SetModifiedValues (new Store (entity.GetModifiedValues ()) { Entity = entity });
				entity.SetOriginalValues (new Store (entity.GetOriginalValues ()) { Entity = entity });
			}
		}

		private static void VirtualizeEntity(AbstractEntity entity, Store parentStore, string id)
		{
			var store = new Store (entity.GetModifiedValues ())
			{
				Entity = entity,
				ParentStore = parentStore,
				FieldIdInParentStore = id,
			};

			entity.SetModifiedValues (store);
		}

		class Store : IValueStore
		{
			public Store(IValueStore realStore)
			{
				this.realStore = realStore;
				this.values = new Dictionary<string, object> ();
			}

			public AbstractEntity Entity
			{
				get;
				set;
			}

			public bool ReadOnly
			{
				get
				{
					return this.ParentStore != null;
				}
			}

			public Store ParentStore
			{
				get;
				set;
			}

			public string FieldIdInParentStore
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

					var entity = this.CreateEntityForField (id, Store.CreateAlwaysEmptyEntity);
					
					if (entity != null)
					{
						EntityNullReferenceVirtualizer.VirtualizeEntity (entity, this, id);
						this.values.Add (id, entity);

						value = entity;
					}
				}

				return value;
			}

			public void SetValue(string id, object value, ValueStoreSetMode mode)
			{
				this.RealizeEntity ();
				
				this.realStore.SetValue (id, value, mode);
				this.values.Remove (id);
			}

			private static AbstractEntity CreateAlwaysEmptyEntity(Druid druid)
			{
				EntityContext.Push (EntityNullReferenceVirtualizer.emptyEntitiesContext);
				
				try
				{
					return EntityClassFactory.CreateEmptyEntity (druid);
				}
				finally
				{
					EntityContext.Pop ();
				}
			}

			private AbstractEntity CreateEntityForField(string id, System.Func<Druid, AbstractEntity> creator)
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
					return creator (info.TypeId);
				}

				return null;
			}

			private void RealizeEntity()
			{
				if (this.ReadOnly)
				{
					var entity  = this.Entity;
					var parent  = this.ParentStore;
					var fieldId = this.FieldIdInParentStore;

					parent.RealizeEntity ();
					parent.realStore.SetValue (fieldId, entity, ValueStoreSetMode.Default);
					parent.values.Remove (fieldId);
					parent.Entity.UpdateDataGeneration ();
					
					this.ParentStore = null;
					
					entity.ReplaceEntityContext (EntityContext.Current);
				}
			}

			#endregion

			private readonly IValueStore realStore;
			private readonly Dictionary<string, object> values;
		}

		public static bool IsVirtualizedEntity(AbstractEntity entity)
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

		public static bool IsVirtualizedReadOnlyEntity(AbstractEntity entity)
		{
			if (entity.InternalGetValueStores ().Any (store => store is Store && ((Store)store).ReadOnly))
			{
				return true;
			}
			else
			{
				return false;
			}
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
