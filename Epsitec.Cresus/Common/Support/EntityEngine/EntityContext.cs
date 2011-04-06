//	Copyright © 2007-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityContext</c> class defines the context associated with a
	/// set of related entities. It is responsible of the value store management.
	/// </summary>
	public class EntityContext : IEntityPersistenceManager
	{
		public EntityContext()
			: this (Resources.DefaultManager, EntityLoopHandlingMode.Throw)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityContext"/> class.
		/// </summary>
		/// <param name="resourceManager">The resource manager.</param>
		/// <param name="loopHandlingMode">The loop handling mode.</param>
		public EntityContext(IStructuredTypeResolver resourceManager, EntityLoopHandlingMode loopHandlingMode, string name = null)
		{
			this.name = name;
			
			this.resourceManager     = resourceManager;
			this.associatedThread    = System.Threading.Thread.CurrentThread;
			this.structuredTypeMap   = new Dictionary<Druid, StructuredType> ();
			this.loopHandlingMode    = loopHandlingMode;
			this.persistenceManagers = new List<IEntityPersistenceManager> ();

			this.propertyGetters = new Dictionary<string, PropertyGetter> ();
			this.propertySetters = new Dictionary<string, PropertySetter> ();

			this.dataGeneration = 1;
		}

		/// <summary>
		/// Initializes the <see cref="EntityContext"/> class.
		/// </summary>
		static EntityContext()
		{
			EntityClassFactory.Setup ();
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
		/// Gets or sets the exception manager associated with this context.
		/// </summary>
		/// <value>The exception manager.</value>
		public IExceptionManager				ExceptionManager
		{
			get;
			set;
		}

		public IList<IEntityPersistenceManager> PersistenceManagers
		{
			get
			{
				return this.persistenceManagers;
			}
		}

		public ICaptionResolver					CaptionResolver
		{
			get
			{
				return this.resourceManager as ICaptionResolver ?? Resources.DefaultManager;
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
		/// Compares two entities for equality. This compares the contents of
		/// both entities; the two entities may therefore be of differing types
		/// and still return <c>true</c>.
		/// </summary>
		/// <param name="a">The first entity.</param>
		/// <param name="b">The second entity.</param>
		/// <returns></returns>
		public static bool CompareEqual(AbstractEntity a, AbstractEntity b)
		{
			if (a == b)
			{
				return true;
			}
			if ((a == null) ||
				(b == null))
			{
				return false;
			}

			//	TODO: improve equality comparison !

			string da = a.Dump ();
			string db = b.Dump ();

			return da == db;
		}

		#region IEntityPersistenceManager Members

		/// <summary>
		/// Gets the persisted id for the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>The persisted id or <c>null</c>.</returns>
		public string GetPersistedId(AbstractEntity entity)
		{
			foreach (var manager in this.persistenceManagers)
			{
				string id = manager.GetPersistedId (entity);

				if (!string.IsNullOrEmpty (id))
				{
					return id;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the persisted entity for the specified id and entity id.
		/// </summary>
		/// <param name="id">The id (identifies the instance).</param>
		/// <param name="entityId">The entity id (identifies the type).</param>
		/// <returns>The persisted entity or <c>null</c>.</returns>
		public AbstractEntity GetPersistedEntity(string id)
		{
			if (string.IsNullOrEmpty (id))
			{
				return null;
			}

			foreach (var manager in this.persistenceManagers)
			{
				AbstractEntity entity = manager.GetPersistedEntity (id);

				if (entity != null)
				{
					return entity;
				}
			}

			return null;
		}

		#endregion


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

		/// <summary>
		/// Evaluates the specified function using the associated <see cref="IExceptionManager"/>,
		/// if any.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="func">The function to call.</param>
		/// <param name="entity">The entity which provides the function.</param>
		/// <param name="entityType">The type of the entity.</param>
		/// <param name="id">The id of the field which implements the function.</param>
		/// <param name="expr">The original function encoded as an expression.</param>
		/// <returns>The result of the function evaluation.</returns>
		internal TResult EvaluateFunc<TResult>(System.Func<TResult> func, AbstractEntity entity, System.Type entityType, string id, System.Linq.Expressions.LambdaExpression expr)
		{
			IExceptionManager manager = this.ExceptionManager;

			if (manager == null)
			{
				return func ();
			}
			else
			{
				return manager.Execute (func, () => string.Format ("Entity {0} ({1}), field {2}", entityType.FullName, entity.GetEntityStructuredTypeId (), id));
			}
		}

		internal IValueStore CreateValueStore(AbstractEntity entity)
		{
			Druid typeId = entity.GetEntityStructuredTypeId ();
			StructuredType type = this.GetStructuredType (typeId);

			if (type == null)
			{
				GenericEntity genericEntity = entity as GenericEntity;

				if ((genericEntity == null) ||
					(typeId.IsValid))
				{
					throw new System.NotSupportedException (string.Format ("Type {0} is not supported", typeId));
				}
			}
			
			return new Data (type);
		}

		internal void FillValueStoreDataIds(IValueStore store, HashSet<string> ids)
		{
			Data data = store as Data;

			if (data != null)
			{
				foreach (string id in data.GetIds ())
				{
					ids.Add (id);
				}
			}
		}

		public IEnumerable<string> GetEntityFieldIds(AbstractEntity entity)
		{
			if (entity == null)
			{
				throw new System.ArgumentNullException ("entity");
			}

			StructuredType entityType = this.GetStructuredType (entity);

			if (entityType == null)
			{
				throw new System.ArgumentException ("Invalid entity; no associated StructuredType");
			}

			return entityType.GetFieldIds ();
		}

		public StructuredType GetStructuredType(Druid entityId)
		{
			this.EnsureCorrectThread ();

			StructuredType type;

			if (!this.structuredTypeMap.TryGetValue (entityId, out type))
			{
				if (entityId.IsTemporary)
				{
					return null;
				}
				
				type = this.resourceManager.GetStructuredType (entityId);
				this.structuredTypeMap[entityId] = type;
			}
			
			return type;
		}

		public StructuredType GetStructuredType(AbstractEntity entity)
		{
			this.EnsureCorrectThread ();

			IStructuredTypeProvider provider = entity.GetStructuredTypeProvider ();
			StructuredType type;

			if (provider == null)
			{
				type = this.GetStructuredType (entity.GetEntityStructuredTypeId ());
			}
			else
			{
				type = provider.GetStructuredType () as StructuredType;
			}

			if (type == null)
			{
				type = entity.GetSyntheticStructuredType (this);
			}

			return type;
		} 

		internal StructuredTypeField GetStructuredTypeField(AbstractEntity entity, string fieldId)
		{
			StructuredType      type  = this.GetStructuredType (entity);
			StructuredTypeField field = type == null ? null : type.GetField (fieldId);

			return field;
		}

		/// <summary>
		/// Defines a structured type.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="type">The type.</param>
		internal void DefineStructuredType(Druid id, StructuredType type)
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

		/// <summary>
		/// Creates an entity which can be used as a search template. The entity
		/// implements <see cref="IFieldPropertyStore"/>.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		/// <returns>The search template entity.</returns>
		public AbstractEntity CreateSearchEntity(Druid entityId)
		{
			AbstractEntity entity;

			EntityContext.Push (this);

			try
			{
				entity = EntityClassFactory.CreateEmptyEntity (entityId) ?? this.CreateGenericEntity (entityId);
				entity = new SearchEntity (entity);
			}
			finally
			{
				EntityContext.Pop ();
			}

			this.OnEntityAttached (new EntityContextEventArgs (entity, null, this));
			return entity;
		}

		public static bool IsSearchEntity(AbstractEntity entity)
		{
			return entity is SearchEntity;
		}

		public AbstractEntity CreateEmptyEntity(Druid entityId)
		{
			AbstractEntity entity;

			EntityContext.Push (this);

			try
			{
				entity = EntityClassFactory.CreateEmptyEntity (entityId);
				
				if (entity == null)
				{
					entity = this.CreateGenericEntity (entityId);
				}
			}
			finally
			{
				EntityContext.Pop ();
			}

			this.OnEntityAttached (new EntityContextEventArgs (entity, null, this));
			return entity;
		}

		public T CreateEmptyEntity<T>()
			where T : AbstractEntity, new ()
		{
			T entity;
			
			EntityContext.Push (this);
			
			try
			{
				entity = new T ();
			}
			finally
			{
				EntityContext.Pop ();
			}
			
			this.OnEntityAttached (new EntityContextEventArgs (entity, null, this));
			return entity;
		}

		/// <summary>
		/// Initializes the default values if the entity implements the <see cref="IEntityInitializer"/>
		/// interface.
		/// </summary>
		/// <param name="entity">The entity to initialize.</param>
		public static void InitializeDefaultValues(AbstractEntity entity)
		{
			var initializer = entity as IEntityInitializer;

			if (initializer != null)
            {
				using (entity.DefineOriginalValues ())
				{
					initializer.InitializeDefaultValues ();
				}
            }
		}

		public AbstractEntity CreateEntity(Druid entityId)
		{
			AbstractEntity entity = this.CreateEmptyEntity (entityId);
			this.CreateRelatedEntities (entity);
			return entity;
		}

		public T CreateEntity<T>()
			where T : AbstractEntity, new ()
		{
			T entity = this.CreateEmptyEntity<T> ();
			this.CreateRelatedEntities (entity);
			return entity;
		}


		public bool IsFieldDefined(string fieldId, AbstractEntity entity)
		{
			bool isDefined;

			StructuredType entityType = this.GetStructuredType (entity.GetEntityStructuredTypeId ());
			StructuredTypeField field = entityType.GetField (fieldId);

			object value = entity.InternalGetValueOrFieldCollection (fieldId);

			switch (field.Relation)
			{
				case FieldRelation.None:
				case FieldRelation.Reference:

					isDefined = (value != null) && (value != UndefinedValue.Value);

					break;

				case FieldRelation.Collection:

					IList values = value as IList;

					isDefined = (values != null) && (values.Count > 0);

					break;

				default:
					throw new System.NotSupportedException ();
			}

			return isDefined;
		}


		public INamedType GetFieldType(AbstractEntity entity, string id)
		{
			StructuredTypeField field = this.GetStructuredTypeField (entity, id);
			return field == null ? null : field.Type;
		}

		public System.Type GetFieldSystemType(AbstractEntity entity, string id)
		{
			INamedType type = this.GetFieldType (entity, id);
			return type == null ? null : type.SystemType;
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
			StructuredType type = this.GetStructuredType (entityId);

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
			string entityId = entity.GetEntityStructuredTypeKey ();
			string key = string.Concat (entityId, id);
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
			string entityId = entity.GetEntityStructuredTypeKey ();
			string key = string.Concat (entityId, id);
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

			StructuredType type = this.GetStructuredType (entity);
			
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

		internal void NotifyEntityAttached(AbstractEntity entity, EntityContext oldContext)
		{
			this.OnEntityAttached (new EntityContextEventArgs (entity, oldContext, this));
		}

		internal void NotifyEntityDetached(AbstractEntity entity, EntityContext newContext)
		{
			this.OnEntityDetached (new EntityContextEventArgs (entity, this, newContext));
		}

		internal void NotifyEntityChanged(AbstractEntity entity, string id, object oldValue, object newValue)
		{
			if (entity.AreEventsEnabled)
			{
				EntityFieldChangedEventArgs eventArgs = new EntityFieldChangedEventArgs (entity, id, oldValue, newValue);

				this.OnEntityChanged (eventArgs);
				entity.OnEntityChanged (eventArgs);
			}
		}

		protected virtual void OnEntityAttached(EntityContextEventArgs e)
		{
			EventHandler<EntityContextEventArgs> handler;

			lock (this.eventExclusion)
			{
				handler = this.entityAttachedEvent;
			}

			if (handler != null)
			{
				handler (this, e);
			}
		}

		protected virtual void OnEntityDetached(EntityContextEventArgs e)
		{
			EventHandler<EntityContextEventArgs> handler;

			lock (this.eventExclusion)
			{
				handler = this.entityDetachedEvent;
			}

			if (handler != null)
			{
				handler (this, e);
			}
		}

		protected virtual void OnEntityChanged(EntityFieldChangedEventArgs e)
		{
			EventHandler<EntityFieldChangedEventArgs> handler;

			lock (this.eventExclusion)
			{
				handler = this.entityChangedEvent;
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
			public Data(StructuredType type)
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

					IEntityProxy entityProxy = value as IEntityProxy;
					
					if (entityProxy == null)
					{
						//	If the value has an attached proxy, check to see if this
						//	should be handled as a null value :

						IEntityProxyProvider provider = value as IEntityProxyProvider;

						if (provider != null)
						{
							INullable nullableProxy = provider.GetEntityProxy () as INullable;

							//	This trick is needed to properly implement proxying null
							//	values in the DialogData class.

							if (nullableProxy != null && nullableProxy.IsNull)
							{
								return UndefinedValue.Value;
							}
						}

						IValueProxy fieldProxy = value as IValueProxy;

						if (fieldProxy != null)
						{
							value = fieldProxy.GetValue ();
							this.SetValue (id, value, ValueStoreSetMode.Default);
						}

						return value;
					}
					else
					{
						return entityProxy.GetReadEntityValue (this, id);
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

						if (proxy != null && proxy.DiscardWriteEntityValue (this, id, ref value))
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


			private StructuredType type;
			private Dictionary<string, object> store;

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


		public event EventHandler<EntityContextEventArgs> EntityAttached
		{
			add
			{
				lock (this.eventExclusion)
				{
					this.entityAttachedEvent += value;
				}
			}
			remove
			{
				lock (this.eventExclusion)
				{
					this.entityAttachedEvent -= value;
				}
			}
		}

		public event EventHandler<EntityContextEventArgs> EntityDetached
		{
			add
			{
				lock (this.eventExclusion)
				{
					this.entityDetachedEvent += value;
				}
			}
			remove
			{
				lock (this.eventExclusion)
				{
					this.entityDetachedEvent -= value;
				}
			}
		}

		public event EventHandler<EntityFieldChangedEventArgs> EntityChanged
		{
			add
			{
				lock (this.eventExclusion)
				{
					this.entityChangedEvent += value;
				}
			}
			remove
			{
				lock (this.eventExclusion)
				{
					this.entityChangedEvent -= value;
				}
			}
		}

		[System.ThreadStatic]
		private static EntityContext current;

		[System.ThreadStatic]
		private static Stack<EntityContext> contextStack;

		private readonly object eventExclusion = new object ();

		private EventHandler<EntityContextEventArgs> entityAttachedEvent;
		private EventHandler<EntityContextEventArgs> entityDetachedEvent;
        private EventHandler<EntityFieldChangedEventArgs> entityChangedEvent;

		private readonly IStructuredTypeResolver resourceManager;
		private readonly System.Threading.Thread associatedThread;
		private readonly Dictionary<Druid, StructuredType> structuredTypeMap;
		private readonly EntityLoopHandlingMode loopHandlingMode;
		private readonly List<IEntityPersistenceManager> persistenceManagers;
		private readonly Dictionary<string, PropertyGetter> propertyGetters;
		private readonly Dictionary<string, PropertySetter> propertySetters;
		private readonly string name;

		private long dataGeneration;
		private int suspendConstraintChecking;
	}
}
