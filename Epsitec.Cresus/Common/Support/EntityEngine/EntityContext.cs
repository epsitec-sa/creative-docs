//	Copyright � 2007-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

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

		public EntityContext() : this (SafeResourceResolver.Instance)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityContext"/> class.
		/// </summary>
		/// <param name="resourceManager">The resource manager.</param>
		/// <param name="loopHandlingMode">The loop handling mode.</param>
		public EntityContext(SafeResourceResolver resourceResolver, EntityLoopHandlingMode loopHandlingMode = EntityLoopHandlingMode.Throw, string name = null)
		{
			this.name = name;
			this.loopHandlingMode = loopHandlingMode;

			this.suspendConstraintChecking = new InterlockedSafeCounter ();

			this.persistenceManagers = new List<IEntityPersistenceManager> ();

			this.resourceResolver = resourceResolver;

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
		/// 	<c>true</c> if constraint checking should be skipped; otherwise, <c>false</c>.
		/// </value>
		public bool								SkipConstraintChecking
		{
			get
			{
				return !this.suspendConstraintChecking.IsZero;
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
			return this.suspendConstraintChecking.Enter ();
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

		internal static IEnumerable<string> GetValueStoreDataIds(IValueStore store)
		{
			Data data = store as Data;

			if (data != null)
			{
				return data.GetIds ();
			}
			else
			{
				return EmptyEnumerable<string>.Instance;
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
			AbstractEntity entity = EntityClassFactory.CreateEmptyEntity (entityId) ?? this.CreateGenericEntity (entityId);

			entity = new SearchEntity (entity);

			entity.ReplaceEntityContext (this);

			return entity;
		}

		public static bool IsSearchEntity(AbstractEntity entity)
		{
			return entity is SearchEntity;
		}

		public AbstractEntity CreateEmptyEntity(Druid entityId)
		{
			AbstractEntity entity = EntityClassFactory.CreateEmptyEntity (entityId);
				
			if (entity == null)
			{
				entity = this.CreateGenericEntity (entityId);
			}

			entity.ReplaceEntityContext (this);

			return entity;
		}

		public T CreateEmptyEntity<T>()
			where T : AbstractEntity, new ()
		{
			T entity = new T ();

			entity.ReplaceEntityContext (this);

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

		public void DisableCalculations(AbstractEntity entity)
		{
			if (entity != null)
			{
				entity.DisableCalculations ();
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

		internal void NotifyEntityAttached(AbstractEntity entity, EntityContext oldContext)
		{
			var eventArgs = new EntityContextEventArgs (entity, oldContext, this);

			this.EntityAttached.Raise (this, eventArgs);
		}

		internal void NotifyEntityDetached(AbstractEntity entity, EntityContext newContext)
		{
			var eventArgs = new EntityContextEventArgs (entity, this, newContext);

			this.EntityDetached.Raise (this, eventArgs);
		}

		internal void NotifyEntityChanged(AbstractEntity entity, string id, object oldValue, object newValue)
		{
			if (entity.AreEventsEnabled)
			{
				var eventArgs = new EntityFieldChangedEventArgs (entity, id, oldValue, newValue);

				this.EntityChanged.Raise (this, eventArgs);
			}
		}

		public Caption GetCaption(Druid captionId)
		{
			return this.resourceResolver.GetCaption (captionId);
		}

		public StructuredType GetStructuredType(Druid entityTypeId)
		{
			return this.resourceResolver.GetStructuredType (entityTypeId);
		}

		public StructuredType GetStructuredType(AbstractEntity entity)
		{
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

		public event EventHandler<EntityContextEventArgs> EntityAttached;
		public event EventHandler<EntityContextEventArgs> EntityDetached;
		public event EventHandler<EntityFieldChangedEventArgs> EntityChanged;

		private readonly string name;
		private readonly EntityLoopHandlingMode loopHandlingMode;

		private readonly List<IEntityPersistenceManager> persistenceManagers;
		private long dataGeneration;
		private InterlockedSafeCounter suspendConstraintChecking;

		private readonly SafeResourceResolver resourceResolver;
		private readonly Dictionary<string, PropertyGetter> propertyGetters;
		private readonly Dictionary<string, PropertySetter> propertySetters;
	}
}
