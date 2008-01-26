//	Copyright © 2007-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		public EntityContext(IStructuredTypeResolver resourceManager, EntityLoopHandlingMode loopHandlingMode)
		{
			this.resourceManager     = resourceManager;
			this.associatedThread    = System.Threading.Thread.CurrentThread;
			this.structuredTypeMap   = new Dictionary<Druid, IStructuredType> ();
			this.loopHandlingMode    = loopHandlingMode;

			this.propertyGetters = new Dictionary<string, PropertyGetter> ();
			this.propertySetters = new Dictionary<string, PropertySetter> ();

			this.dataGeneration = 1;
		}

		/// <summary>
		/// Initializes the <see cref="EntityContext"/> class.
		/// </summary>
		static EntityContext()
		{
			EntityClassResolver.Setup ();
		}


		/// <summary>
		/// Gets the active data generation.
		/// </summary>
		/// <value>The active data generation.</value>
		public long								DataGeneration
		{
			get
			{
				return this.dataGeneration;
			}
		}

		/// <summary>
		/// Gets a value indicating whether to skip constraint checking when
		/// setting a value.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if constraint checkin should be skipped; otherwise, <c>false</c>.
		/// </value>
		public bool								SkipConstraintChecking
		{
			get
			{
				return this.suspendConstraintChecking > 0;
			}
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


		/// <summary>
		/// Starts a new data generation. This increments the <see cref="DataGeneration"/>
		/// property.
		/// </summary>
		public void NewDataGeneration()
		{
			System.Threading.Interlocked.Increment (ref this.dataGeneration);
		}

		public System.IDisposable SuspendConstraintChecking()
		{
			return new SuspendConstraintCheckingHelper (this);
		}

		internal IValueStore CreateValueStore(AbstractEntity entity)
		{
			Druid typeId = entity.GetEntityStructuredTypeId ();
			IStructuredType type = this.GetStructuredType (typeId);

			if (type == null)
			{
				throw new System.NotSupportedException (string.Format ("Type {0} is not supported", typeId));
			}
			
			return new Data (type);
		}

		internal IEnumerable<string> GetEntityFieldIds(AbstractEntity entity)
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

		internal IEnumerable<StructuredTypeField> GetEntityFieldDefinitions(Druid id)
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

		internal IStructuredType GetStructuredType(Druid id)
		{
			this.EnsureCorrectThread ();

			IStructuredType type;

			if (!this.structuredTypeMap.TryGetValue (id, out type))
			{
				if (id.IsTemporary)
				{
					return null;
				}
				
				type = this.resourceManager.GetStructuredType (id);
				this.structuredTypeMap[id] = type;
			}
			
			return type;
		}

		internal IStructuredType GetStructuredType(AbstractEntity entity)
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

		internal StructuredTypeField GetStructuredTypeField(AbstractEntity entity, string fieldId)
		{
			IStructuredType     type  = this.GetStructuredType (entity);
			StructuredTypeField field = type == null ? null : type.GetField (fieldId);

			return field;
		}

		/// <summary>
		/// Defines a structured type.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="type">The type.</param>
		internal void DefineStructuredType(Druid id, IStructuredType type)
		{
			this.EnsureCorrectThread ();

			if (this.structuredTypeMap.ContainsKey (id))
			{
				throw new System.InvalidOperationException ("StructuredType cannot be redefined");
			}
			else
			{
				this.structuredTypeMap[id] = type;
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
			AbstractEntity entity = EntityClassResolver.CreateEmptyEntity (entityId);

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

		public IEnumerable<string> GetDefinedFields(AbstractEntity entity)
		{
			HashSet<string> ids = new HashSet<string> ();

			foreach (IValueStore store in entity.InternalGetValueStores ())
			{
				Data dataStore = store as Data;
				
				foreach (string id in dataStore.GetIds ())
				{
					ids.Add (id);
				}
			}

			return ids;
		}

		public void DisableCalculations(AbstractEntity entity)
		{
			if (entity != null)
			{
				entity.DisableCalculations ();
			}
		}

		public bool IsNullable(Druid entityId, string fieldId)
		{
			IStructuredType type = this.GetStructuredType (entityId);

			if (type == null)
			{
				throw new System.ArgumentException ("Entity id cannot be resolved to a type");
			}

			StructuredTypeField field = type.GetField (fieldId);

			if (field == null)
			{
				throw new System.ArgumentException ("Field cannot be resolved");
			}
			
			return this.IsNullable (field);
		}
		
		public bool IsNullable(AbstractEntity entity, string fieldId)
		{
			StructuredTypeField field = this.GetStructuredTypeField (entity, fieldId);

			return this.IsNullable (field);
		}

		private bool IsNullable(StructuredTypeField field)
		{
			if (field.IsNullable)
			{
				return true;
			}

			INullableType nullableType = field.Type as INullableType;

			if ((nullableType != null) &&
				(nullableType.IsNullable))
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// Finds the property setter used to write to the specified field.
		/// This uses an internal cache.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="id">The field id.</param>
		/// <returns>The <see cref="PropertySetter"/> delegate or <c>null</c>.</returns>
		internal PropertySetter FindPropertySetter(AbstractEntity entity, string id)
		{
			string key = string.Concat (entity.GetEntityStructuredTypeId (), ".", id);
			PropertySetter setter;
			
			if (this.propertySetters.TryGetValue (key, out setter))
			{
				return setter;
			}

			this.propertySetters[key] = setter = DynamicCodeFactory.CreatePropertySetter (this.FindPropertyInfo (entity, id));

			return setter;
		}

		/// <summary>
		/// Finds the property getter used to read from the specified field.
		/// This uses an internal cache.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="id">The field id.</param>
		/// <returns>The <see cref="PropertyGetter"/> delegate or <c>null</c>.</returns>
		internal PropertyGetter FindPropertyGetter(AbstractEntity entity, string id)
		{
			string key = string.Concat (entity.GetEntityStructuredTypeId (), ".", id);
			PropertyGetter getter;
			
			if (this.propertyGetters.TryGetValue (key, out getter))
			{
				return getter;
			}

			this.propertyGetters[key] = getter = DynamicCodeFactory.CreatePropertyGetter (this.FindPropertyInfo (entity, id));

			return getter;
		}

		/// <summary>
		/// Finds the property info for the matching property.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="id">The field id to map to a property.</param>
		/// <returns>The <see cref="System.Reflection.PropertyInfo"/> or <c>null</c>.</returns>
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
					
					if (proxy == null)
					{
						//	If the value has an attached proxy, check to see if this
						//	should be handled as a null value :

						IEntityProxyProvider provider = value as IEntityProxyProvider;

						if (provider != null)
						{
							INullable nullableProxy = provider.GetEntityProxy () as INullable;

							//	This trick is needed to properly implement proxying null
							//	values in the DialogData class.

							if ((nullableProxy != null) &&
								(nullableProxy.IsNull))
							{
								return UndefinedValue.Value;
							}
						}
						
						return value;
					}
					else
					{
						return proxy.GetReadEntityValue (this, id);
					}
				}
				else
				{
					return UndefinedValue.Value;
				}
			}

			public void SetValue(string id, object value, ValueStoreSetMode mode)
			{
				object oldValue;

				if (mode != ValueStoreSetMode.ShortCircuit)
				{
					if (this.store.TryGetValue (id, out oldValue))
					{
						IEntityProxy proxy = oldValue as IEntityProxy;

						if (proxy == null)
						{
							IEntityProxyProvider provider = oldValue as IEntityProxyProvider;

							if (provider != null)
							{
								proxy = provider.GetEntityProxy ();
							}
						}

						if ((proxy != null) &&
							(proxy.DiscardWriteEntityValue (this, id, ref value)))
						{
							return;
						}
					}
				}

				if (UndefinedValue.IsUndefinedValue (value))
				{
					this.store.Remove (id);
				}
				else if (mode == ValueStoreSetMode.ShortCircuit)
				{
					this.store[id] = value;
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

			public IEnumerable<string> GetIds()
			{
				return this.store.Keys;
			}

			IStructuredType type;
			Dictionary<string, object> store;
		}

		#endregion

		private class SuspendConstraintCheckingHelper : System.IDisposable
		{
			public SuspendConstraintCheckingHelper(EntityContext context)
			{
				this.context = context;
				System.Threading.Interlocked.Increment (ref this.context.suspendConstraintChecking);
			}

			~SuspendConstraintCheckingHelper()
			{
				throw new System.InvalidOperationException ("Caller of SuspendConstraintChecking forgot to call Dispose");
			}


			#region IDisposable Members

			public void Dispose()
			{
				System.GC.SuppressFinalize (this);
				System.Threading.Interlocked.Decrement (ref this.context.suspendConstraintChecking);
			}

			#endregion


			private readonly EntityContext context;
		}


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

		private readonly IStructuredTypeResolver resourceManager;
		private readonly System.Threading.Thread associatedThread;
		private readonly Dictionary<Druid, IStructuredType> structuredTypeMap;
		private readonly EntityLoopHandlingMode loopHandlingMode;
		private readonly Dictionary<string, PropertyGetter> propertyGetters;
		private readonly Dictionary<string, PropertySetter> propertySetters;

		private long dataGeneration;
		private int suspendConstraintChecking;
	}
}
