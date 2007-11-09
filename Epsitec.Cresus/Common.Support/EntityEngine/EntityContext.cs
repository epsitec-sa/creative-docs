//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityContext</c> class defines the context associated with a
	/// set of related entities. It is responsible of the value store management.
	/// </summary>
	public class EntityContext
	{
		private EntityContext()
		{
			this.resourceManager     = Resources.DefaultManager;
			this.resourceManagerPool = this.resourceManager.Pool;
			this.associatedThread    = System.Threading.Thread.CurrentThread;
			this.structuredTypeMap   = new Dictionary<Druid, IStructuredType> ();
		}

		static EntityContext()
		{
			EntityResolver.Setup ();
		}
		
		
		public static EntityContext				Current
		{
			get
			{
				if (EntityContext.current == null)
				{
					EntityContext.current = new EntityContext ();
				}
				
				return EntityContext.current;
			}
		}

		public IValueStore CreateValueStore(AbstractEntity entity)
		{
			Druid typeId = entity.GetEntityStructuredTypeId ();
			IStructuredType type = this.GetStructuredType (typeId);

			if (type == null)
			{
				throw new System.NotSupportedException (string.Format ("Type {0} is not supported", typeId));
			}
			
			return new Data (type);
		}


		public IEnumerable<string> GetEntityFieldIds(AbstractEntity entity)
		{
			if (entity == null)
			{
				throw new System.ArgumentNullException ("entity");
			}

			IStructuredType entityType = this.GetStructuredType (entity);

			if (entityType == null)
			{
				throw new System.ArgumentException ("Invalid entity; no associted IStructuredType");
			}

			return entityType.GetFieldIds ();
		}

		public IEnumerable<StructuredTypeField> GetEntityFieldDefinitions(AbstractEntity entity)
		{
			if (entity == null)
			{
				throw new System.ArgumentNullException ("entity");
			}

			IStructuredType entityType = this.GetStructuredType (entity);

			if (entityType == null)
			{
				throw new System.ArgumentException ("Invalid entity; no associted IStructuredType");
			}

			foreach (string fieldId in entityType.GetFieldIds ())
			{
				yield return entityType.GetField (fieldId);
			}
		}

		public IStructuredType GetStructuredType(Druid id)
		{
			this.EnsureCorrectThread ();

			IStructuredType type;

			if (!this.structuredTypeMap.TryGetValue (id, out type))
			{
				Caption caption = this.resourceManager.GetCaption (id);
				type = TypeRosetta.GetTypeObject (caption) as IStructuredType;
				this.structuredTypeMap[id] = type;
			}
			
			return type;
		}

		public IStructuredType GetStructuredType(AbstractEntity entity)
		{
			this.EnsureCorrectThread ();

			IStructuredTypeProvider provider = entity.GetStructuredTypeProvider ();

			if (provider == null)
			{
				return this.GetStructuredType (entity.GetEntityStructuredTypeId ());
			}
			else
			{
				return provider.GetStructuredType ();
			}
		}

		public AbstractEntity CreateEmptyEntity(Druid entityId)
		{
			AbstractEntity entity = EntityResolver.CreateEmptyEntity (entityId);

			if (entity == null)
			{
				entity = this.CreateGenericEntity (entityId);
			}

			this.OnEntityCreated (new EntityEventArgs (entity));
			return entity;
		}

		public T CreateEmptyEntity<T>() where T : AbstractEntity, new ()
		{
			T entity = new T ();
			this.OnEntityCreated (new EntityEventArgs (entity));
			return entity;
		}

		public AbstractEntity CreateEntity(Druid entityId)
		{
			AbstractEntity entity = this.CreateEmptyEntity (entityId);
			this.CreateRelatedEntities (entity);
			return entity;
		}

		public T CreateEntity<T>() where T : AbstractEntity, new ()
		{
			T entity = this.CreateEmptyEntity<T> ();
			this.CreateRelatedEntities (entity);
			return entity;
		}


		private AbstractEntity CreateGenericEntity(Druid entityId)
		{
			return new GenericEntity (entityId);
		}

		private void CreateRelatedEntities(AbstractEntity entity)
		{
			using (entity.DefineOriginalValues ())
			{
				this.CreateRelatedEntities (entity, new Stack<Druid> ());
			}
		}

		private void CreateRelatedEntities(AbstractEntity entity, Stack<Druid> parents)
		{
			parents.Push (entity.GetEntityStructuredTypeId ());

			IStructuredType type = this.GetStructuredType (entity);
			
			foreach (string id in type.GetFieldIds ())
			{
				StructuredTypeField field = type.GetField (id);

				if ((field.Relation == FieldRelation.Reference) &&
					(field.IsNullable == false))
				{
					Druid entityId = field.TypeId;

					if (parents.Contains (entityId))
					{
						System.Text.StringBuilder cycle = new System.Text.StringBuilder ();
						Druid[] stack = parents.ToArray ();

						for (int i = stack.Length; i > 0; i--)
						{
							if (cycle.Length > 0)
							{
								cycle.Append (" > ");
							}
							cycle.Append (stack[i-1]);
						}

						cycle.Append (" > ");
						cycle.Append (entityId);
						
						throw new System.InvalidOperationException ("Cyclic dependency : " + cycle);
					}
					
					AbstractEntity child = this.CreateEmptyEntity (field.TypeId);

					using (child.DefineOriginalValues ())
					{
						this.CreateRelatedEntities (child, parents);
					}

					entity.SetField<AbstractEntity> (id, null, child);
				}
			}

			parents.Pop ();
		}
		
		protected virtual void OnEntityCreated(EntityEventArgs e)
		{
			EventHandler<EntityEventArgs> handler;

			lock (this.eventExclusion)
			{
				handler = this.entityCreatedEvent;
			}

			if (handler != null)
			{
				handler (this, e);
			}
		}
		
		protected void EnsureCorrectThread()
		{
			if (this.associatedThread == System.Threading.Thread.CurrentThread)
			{
				return;
			}

			throw new System.InvalidOperationException ("Invalid thread calling into EntityContext");
		}

		#region Private Data Class

		/// <summary>
		/// The <c>Data</c> class is used to store the data contained in the
		/// entity objects.
		/// </summary>
		private class Data : IValueStore, IStructuredTypeProvider
		{
			public Data(IStructuredType type)
			{
				this.type  = type;
				this.store = new Dictionary<string, object> ();
			}

			#region IValueStore Members

			public object GetValue(string id)
			{
				object value;
				
				if (this.store.TryGetValue (id, out value))
				{
					return value;
				}
				else
				{
					return UndefinedValue.Instance;
				}
			}

			public void SetValue(string id, object value)
			{
				if (UndefinedValue.IsUndefinedValue (value))
				{
					this.store.Remove (id);
				}
				else
				{
					this.store[id] = value;
				}
			}

			#endregion

			#region IStructuredTypeProvider Members

			public IStructuredType GetStructuredType()
			{
				return this.type;
			}

			#endregion

			IStructuredType type;
			Dictionary<string, object> store;
		}

		#endregion

		public event EventHandler<EntityEventArgs> EntityCreated
		{
			add
			{
				lock (this.eventExclusion)
				{
					this.entityCreatedEvent += value;
				}
			}
			remove
			{
				lock (this.eventExclusion)
				{
					this.entityCreatedEvent -= value;
				}
			}
		}

		[System.ThreadStatic]
		private static EntityContext current;

		private readonly object eventExclusion = new object ();

		private EventHandler<EntityEventArgs> entityCreatedEvent;

		private readonly ResourceManagerPool resourceManagerPool;
		private readonly ResourceManager resourceManager;
		private readonly System.Threading.Thread associatedThread;
		private readonly Dictionary<Druid, IStructuredType> structuredTypeMap;
	}
}
