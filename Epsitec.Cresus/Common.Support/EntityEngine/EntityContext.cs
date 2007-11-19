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
			: this (Resources.DefaultManager, EntityLoopHandlingMode.Throw)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityContext"/> class.
		/// </summary>
		/// <param name="resourceManager">The resource manager.</param>
		/// <param name="loopHandlingMode">The loop handling mode.</param>
		public EntityContext(ResourceManager resourceManager, EntityLoopHandlingMode loopHandlingMode)
		{
			this.resourceManager     = resourceManager;
			this.resourceManagerPool = this.resourceManager.Pool;
			this.associatedThread    = System.Threading.Thread.CurrentThread;
			this.structuredTypeMap   = new Dictionary<Druid, IStructuredType> ();
			this.loopHandlingMode    = loopHandlingMode;
		}

		/// <summary>
		/// Initializes the <see cref="EntityContext"/> class.
		/// </summary>
		static EntityContext()
		{
			EntityResolver.Setup ();
		}


		/// <summary>
		/// Gets the current, thread specific, entity context.
		/// </summary>
		/// <value>The entity context for this thread.</value>
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

		/// <summary>
		/// Pushes the current context on an internal stack and makes the
		/// specified context the new current context.
		/// </summary>
		/// <param name="context">The context.</param>
		public static void Push(EntityContext context)
		{
			if (EntityContext.contextStack == null)
			{
				EntityContext.contextStack = new Stack<EntityContext> ();
			}

			EntityContext.contextStack.Push (EntityContext.current);
			EntityContext.current = context;
		}

		/// <summary>
		/// Pops the current context from an internal stack. See <see cref="Push"/>.
		/// </summary>
		public static void Pop()
		{
			EntityContext.current = EntityContext.contextStack.Pop ();
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

		public IEnumerable<StructuredTypeField> GetEntityFieldDefinitions(Druid id)
		{
			IStructuredType entityType = this.GetStructuredType (id);

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

		public Druid GetBaseEntityId(Druid id)
		{
			Druid baseId = id;

			while (baseId.IsValid)
			{
				StructuredType type = this.GetStructuredType (baseId) as StructuredType;

				System.Diagnostics.Debug.Assert (type != null);
				System.Diagnostics.Debug.Assert (type.CaptionId == baseId);
				
				id = baseId;
				
				baseId = type.BaseTypeId;
			}

			return id;
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


		internal PropertySetter FindPropertySetter(AbstractEntity entity, string id)
		{
			System.Reflection.PropertyInfo propertyInfo = this.FindPropertyInfo (entity, id);
			return DynamicCodeFactory.CreatePropertySetter (propertyInfo);
		}

		internal PropertyGetter FindPropertyGetter(AbstractEntity entity, string id)
		{
			System.Reflection.PropertyInfo propertyInfo = this.FindPropertyInfo (entity, id);
			return DynamicCodeFactory.CreatePropertyGetter (propertyInfo);
		}

		private System.Reflection.PropertyInfo FindPropertyInfo(AbstractEntity entity, string id)
		{
			System.Type entityType = entity.GetType ();
			System.Reflection.PropertyInfo[] propertyInfos = entityType.GetProperties (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

			foreach (System.Reflection.PropertyInfo propertyInfo in propertyInfos)
			{
				foreach (EntityFieldAttribute attribute in propertyInfo.GetCustomAttributes (typeof (EntityFieldAttribute), true))
				{
					if (attribute.FieldId == id)
					{
						//	We have found the property which maps to the specified
						//	field :

						return propertyInfo;
					}
				}
			}

			return null;
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
					AbstractEntity child;

					if (parents.Contains (entityId))
					{
						child = this.HandleGraphLoop (parents, entityId);
					}
					else
					{
						child = this.CreateChildEntity (parents, entityId);
					}

					if (child != null)
					{
						entity.SetField<AbstractEntity> (id, null, child);
					}
				}
			}

			parents.Pop ();
		}

		/// <summary>
		/// Creates the child entity, with all its child references properly
		/// initialized.
		/// </summary>
		/// <param name="parents">A stack of parents.</param>
		/// <param name="entityId">The entity id.</param>
		/// <returns>The child entity.</returns>
		private AbstractEntity CreateChildEntity(Stack<Druid> parents, Druid entityId)
		{
			AbstractEntity child = this.CreateEmptyEntity (entityId);

			using (child.DefineOriginalValues ())
			{
				this.CreateRelatedEntities (child, parents);
			}

			return child;
		}

		/// <summary>
		/// A loop has been detected in the graph. Decide what to do, based on
		/// the loop handling mode. This will either throw or skip the field.
		/// </summary>
		/// <param name="parents">A stack of parents.</param>
		/// <param name="entityId">The entity id.</param>
		/// <returns>The child entity (always <c>null</c>).</returns>
		private AbstractEntity HandleGraphLoop(Stack<Druid> parents, Druid entityId)
		{
			if (this.loopHandlingMode == EntityLoopHandlingMode.Throw)
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
			else
			{
				System.Diagnostics.Debug.Assert (this.loopHandlingMode == EntityLoopHandlingMode.Skip);

				return null;
			}
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
					//	If the value in the store is a proxy, let it resolve to
					//	another value, if needed :

					IEntityProxy proxy = value as IEntityProxy;
					return proxy == null ? value : proxy.GetReadEntityValue (this, id);
				}
				else
				{
					return UndefinedValue.Value;
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
					//	If the value to be stored is a proxy, let it resolve to
					//	another value, if needed :

					IEntityProxy proxy = value as IEntityProxy;
					this.store[id] = proxy == null ? value : proxy.GetWriteEntityValue (this, id);
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

		[System.ThreadStatic]
		private static Stack<EntityContext> contextStack;

		private readonly object eventExclusion = new object ();

		private EventHandler<EntityEventArgs> entityCreatedEvent;

		private readonly ResourceManagerPool resourceManagerPool;
		private readonly ResourceManager resourceManager;
		private readonly System.Threading.Thread associatedThread;
		private readonly Dictionary<Druid, IStructuredType> structuredTypeMap;
		private readonly EntityLoopHandlingMode loopHandlingMode;
	}
}
