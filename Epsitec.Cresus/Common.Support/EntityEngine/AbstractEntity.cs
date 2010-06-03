//	Copyright � 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>AbstractEntity</c> class is the base class used to store the
	/// data represented by entity instances.
	/// </summary>
	public abstract class AbstractEntity : IStructuredTypeProvider, IStructuredData, IEntityProxyProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractEntity"/> class.
		/// </summary>
		protected AbstractEntity()
		{
			this.entitySerialId = System.Threading.Interlocked.Increment (ref AbstractEntity.nextSerialId);
			this.context = EntityContext.Current;
		}

		/// <summary>
		/// Gets a value indicating whether this entity is currently defining
		/// its original values (see <see cref="DefineOriginalValues"/>).
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this entity is currently defining its original
		///		values; otherwise, <c>false</c>.
		/// </value>
		public bool IsDefiningOriginalValues
		{
			get
			{
				if (this.defineOriginalValuesCount > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether calculations are disabled.
		/// </summary>
		/// <value><c>true</c> if calculations are disabled; otherwise, <c>false</c>.</value>
		internal bool CalculationsDisabled
		{
			get
			{
				return this.calculationsDisabled;
			}
		}


		/// <summary>
		/// Gets the id of the <see cref="StructuredType"/> which describes
		/// this entity.
		/// </summary>
		/// <returns>The id of the <see cref="StructuredType"/>.</returns>
		public abstract Druid GetEntityStructuredTypeId();

		/// <summary>
		/// Gets the key of the <see cref="StructuredType"/> which describes
		/// this entity. This is a textual representation of the underlying
		/// DRUID.
		/// </summary>
		/// <returns>The key of the <see cref="StructuredType"/>.</returns>
		public abstract string GetEntityStructuredTypeKey();

		/// <summary>
		/// Gets the context associated with this entity.
		/// </summary>
		/// <returns>The <see cref="EntityContext"/> instance.</returns>
		public EntityContext GetEntityContext()
		{
			return this.context;
		}

		/// <summary>
		/// Gets the state of the entity data.
		/// </summary>
		/// <value>The state of the entity data.</value>
		public EntityDataState GetEntityDataState()
		{
			if (this.ModifiedValues != null)
			{
				return EntityDataState.Modified;
			}
			else
			{
				return EntityDataState.Unchanged;
			}
		}

		/// <summary>
		/// Gets the (unique) serial id for this entity.
		/// </summary>
		/// <returns>The serial id for this entity.</returns>
		public long GetEntitySerialId()
		{
			return this.entitySerialId;
		}

		/// <summary>
		/// Gets the entity data generation.
		/// </summary>
		/// <returns>The entity data generation.</returns>
		public long GetEntityDataGeneration()
		{
			return this.dataGeneration;
		}

		/// <summary>
		/// Determines whether the entity contains the specified data version.
		/// </summary>
		/// <param name="version">The version.</param>
		/// <returns>
		/// 	<c>true</c> if the entity contains the specified data version;
		///		otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsDataVersion(EntityDataVersion version)
		{
			switch (version)
			{
				case EntityDataVersion.Modified:
					return this.ModifiedValues != null;
				case EntityDataVersion.Original:
					return this.OriginalValues != null;
				default:
					throw new System.NotImplementedException ();
			}
		}

		public string Dump()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			this.Dump (buffer, 0, new HashSet<AbstractEntity> (), null, true);
			return buffer.ToString ();
		}

		public string DumpFlatData(System.Predicate<StructuredTypeField> filter)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			this.Dump (buffer, 0, new HashSet<AbstractEntity> (), filter, false);
			return buffer.ToString ();
		}

		public void ForEachField(EntityDataVersion version, System.Action<EntityFieldPath, StructuredTypeField, object> action)
		{
			this.ForEachField (version, "", action);
		}

		private void ForEachField(EntityDataVersion version, string root, System.Action<EntityFieldPath, StructuredTypeField, object> action)
		{
			IValueStore store;
			
			switch (version)
			{
				case EntityDataVersion.Modified:
					store = this.ModifiedValues;
					break;
				
				case EntityDataVersion.Original:
					store = this.OriginalValues;
					break;
				
				default:
					throw new System.NotImplementedException ();
			}
			
			if (store == null)
			{
				return;
			}

			foreach (string id in this.context.GetEntityFieldIds (this))
			{
				StructuredTypeField field = this.context.GetStructuredTypeField (this, id);
				object              value = store.GetValue (field.Id);
				EntityFieldPath     path  = string.IsNullOrEmpty (root) ? EntityFieldPath.CreateRelativePath (field.Id) : EntityFieldPath.CreateRelativePath (root, field.Id);

				if ((UndefinedValue.IsUndefinedValue (value)) ||
					(value == null))
				{
					continue;
				}

				action (path, field, value);

				switch (field.Relation)
				{
					case FieldRelation.Reference:
						AbstractEntity entity = value as AbstractEntity;
						if (entity != null)
						{
							entity.ForEachField (version, path.ToString (), action);
						}
						break;

					case FieldRelation.Collection:
						//	TODO: implement...
						break;
				}
			}
		}


		private void Dump(System.Text.StringBuilder buffer, int level, HashSet<AbstractEntity> history, System.Predicate<StructuredTypeField> filter, bool includeLabels)
		{
			string indent = new string (' ', level*2);
			string name   = "";
			
			if (history.Add (this))
			{
				ICaptionResolver manager = this.context.CaptionResolver;
				string nullValue = "<null>";
				
				foreach (string id in this.context.GetEntityFieldIds (this))
				{
					StructuredTypeField field = this.context.GetStructuredTypeField (this, id);

					if ((filter != null) &&
						(filter (field) == false))
					{
						continue;
					}

					if (includeLabels)
					{
						Caption caption = manager.GetCaption (field.CaptionId);
						name = caption.Name;
					}

					object value = this.DynamicGetField (id);
					AbstractEntity child = value as AbstractEntity;

					switch (this.InternalGetFieldRelation (id))
					{
						case FieldRelation.None:
							buffer.AppendFormat (includeLabels ? "{0}{1}: {2}\n" : "{2}\n", indent, name, value == null ? nullValue : value.ToString ());
							break;

						case FieldRelation.Reference:
							if (child == null)
							{
								buffer.AppendFormat (includeLabels ? "{0}{1}: {2}\n" : "\n", indent, name, nullValue);
							}
							else
							{
								if (includeLabels)
								{
									buffer.AppendFormat ("{0}{1}:\n", indent, name);
								}
								child.Dump (buffer, level+1, history, filter, includeLabels);
							}
							break;

						case FieldRelation.Collection:
							if (includeLabels)
							{
								buffer.AppendFormat ("{0}{1}{2}:\n", indent, name, "{");
							}
							int index = 0;
							foreach (object item in (System.Collections.IList) value)
							{
								child = item as AbstractEntity;
								if (child == null)
								{
									if (includeLabels)
									{
										buffer.AppendFormat ("{0}  {1}: {2}\n", indent, index++, nullValue);
									}
								}
								else
								{
									if (includeLabels)
									{
										buffer.AppendFormat ("{0}  {1}:\n", indent, index++);
									}
									child.Dump (buffer, level+2, history, filter, includeLabels);
								}
							}
							if (includeLabels)
							{
								buffer.AppendFormat ("{0}{1}\n", indent, "}");
							}
							break;
					}
				}
			}
			else
			{
				if (includeLabels)
				{
					buffer.AppendFormat ("{0}--> ...\n", indent);
				}
			}
		}


		/// <summary>
		/// Switches the entity into a mode which allows the caller to define
		/// the original values. Call this method in a <c>using</c> block.
		/// </summary>
		/// <returns>The object which must be disposed of to end the special
		/// data definition mode.</returns>
		public System.IDisposable DefineOriginalValues()
		{
			return new DefineOriginalValuesHelper (this);
		}

		internal void DisableCalculations()
		{
			if (this.calculationsDisabled == false)
			{
				this.calculationsDisabled = true;

				AbstractEntity that = this.Resolve ();

				if (this != that)
				{
					that.DisableCalculations ();
				}
			}
		}

		/// <summary>
		/// Resolves the specified entity to the specified type. If the specified
		/// entity is a fa�ade, this will return the real underlying entity, cast
		/// appropriately.
		/// </summary>
		/// <typeparam name="T">The type to resolve to.</typeparam>
		/// <param name="entity">The entity.</param>
		/// <returns>The resolved entity.</returns>
		public static T Resolve<T>(AbstractEntity entity) where T : AbstractEntity
		{
			return entity == null ? null : entity.Resolve () as T;
		}

		public T GetField<T>(string id)
		{
			this.AssertSimpleField (id);

			object value = this.InternalGetValue (id);

			if (UnknownValue.IsUnknownValue (value))
			{
				throw new System.NotSupportedException (string.Format ("Field {0} not supported by value store", id));
			}

			if (UndefinedValue.IsUndefinedValue (value))
			{
				return default (T);
			}
			
			return (T) value;
		}

		public IList<T> GetFieldCollection<T>(string id) where T : AbstractEntity
		{
			this.AssertCollectionField (id);
			
			object  value = this.InternalGetValue (id);
			IList<T> list = value as IList<T>;

			if (list == null)
			{
				if (UndefinedValue.IsUndefinedValue (value))
				{
					//	The value store does not (yet) contain a collection for the
					//	specified items. We have to allocate one :

					using (this.DefineOriginalValues ())
					{
						list = new EntityCollection<T> (id, this, true);
						this.InternalSetValue (id, list);
					}

					list = new EntityCollectionProxy<T> (id, this);
				}
				else
				{
					IEntityCollection collection = value as IEntityCollection;
					System.Collections.IList simpleList = value as System.Collections.IList;

					if (collection == null || simpleList == null)
					{
						throw new System.NotSupportedException (string.Format ("Field {0} uses incompatible collection type", id));
					}

					if (collection.HasCopyOnWriteState)
					{
						list = new EntityCollectionProxy<T> (id, this);
					}
					else
					{
						list = new EntityCollectionProxy<T> (simpleList);
					}
				}
			}
			else
			{
				IEntityCollection collection = value as IEntityCollection;
				
				if (collection == null)
				{
					throw new System.NotSupportedException (string.Format ("Field {0} uses incompatible collection type", id));
				}

				if (collection.HasCopyOnWriteState)
				{
					list = new EntityCollectionProxy<T> (id, this);
				}
			}

			return list;
		}

		public void SetField<T>(string id, T newValue)
		{
			T oldValue = this.GetField<T> (id);
			this.SetField<T> (id, oldValue, newValue);
		}
		
		public void SetField<T>(string id, T oldValue, T newValue)
		{
			this.GenericSetValue (id, oldValue, newValue);
		}

		/// <summary>
		/// Gets the result of the calculation. If calculations are disabled,
		/// this will simply return the stored value instead.
		/// </summary>
		/// <exception cref="System.ArgumentException">Thrown if the entity object does not derive from <see cref="AbstractEntity"/>.</exception>
		/// <typeparam name="T">The entity on which to apply the calculation.</typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="entityObject">The source entity object.</param>
		/// <param name="id">The field id on which we are going to compute a calculation.</param>
		/// <param name="func">The function which implements the calculation.</param>
		/// <param name="expr">The expression for the specified function.</param>
		/// <returns>The result of the calculation.</returns>
		public static TResult GetCalculation<T, TResult>(T entityObject, string id, System.Func<T, TResult> func, System.Linq.Expressions.LambdaExpression expr)
		{
			//	No constraints can be specified on type T, since we can pass in an
			//	interface, which is neither an AbstractEntity derived class in itself
			//	nor a reference type.

			AbstractEntity entity = entityObject as AbstractEntity;

			if (entity == null)
			{
				throw new System.ArgumentException ("Invalid entity specified; cannot resolve to AbstractEntity");
			}

			if (entity.calculationsDisabled)
			{
				return entity.GetField<TResult> (id);
			}
			else
			{
				//	The caller is expecting that we calculate the result based on
				//	the provided function; let the entity context handle the details
				//	(error handling, for instance) :

				EntityContext context = entity.GetEntityContext ();

				return context.EvaluateFunc (delegate { return func (entityObject); }, entity, typeof (T), id, expr);
			}
		}

		/// <summary>
		/// Sets the result of the calculation for the specified field. This is
		/// only acceptable if the calculations have been disabled for the
		/// specified entity.
		/// </summary>
		/// <exception cref="System.ArgumentException">Thrown if the entity object does not derive from <see cref="AbstractEntity"/>.</exception>
		/// <exception cref="System.NotSupportedException">Thrown if the entity is not in the special "disabled calculations" mode.</exception>
		/// <typeparam name="T">The entity on which to apply the calculation.</typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="entityObject">The source entity object.</param>
		/// <param name="id">The field id on which we are going to force a calculation result.</param>
		/// <param name="newValue">The new value.</param>
		public static void SetCalculation<T, TResult>(T entityObject, string id, TResult newValue)
		{
			AbstractEntity entity = entityObject as AbstractEntity;

			if (entity == null)
			{
				throw new System.ArgumentException ("Invalid entity specified; cannot resolve to AbstractEntity");
			}

			if (entity.calculationsDisabled)
			{
				entity.SetField<TResult> (id, newValue);
			}
			else
			{
				throw new System.NotSupportedException (string.Format ("Trying to modify calculation {0} for entity {1}", id, typeof (T).Name));
			}
		}

		
		internal void InternalDefineProxy(IEntityProxy proxy)
		{
			this.proxy = proxy;
		}

		internal object InternalGetValueOrFieldCollection(string id)
		{
			object value;

			switch (this.InternalGetFieldRelation (id))
			{
				case FieldRelation.None:
				case FieldRelation.Reference:
					value = this.InternalGetValue (id);
					break;

				case FieldRelation.Collection:
					value = this.InternalGetFieldCollection (id);
					break;

				default:
					throw new System.NotSupportedException ();
			}

			return value;
		}
		
		internal object InternalGetValue(string id)
		{
			object value;

			if (this.ModifiedValues != null && this.IsDefiningOriginalValues == false)
			{
				value = this.ModifiedValues.GetValue (id);

				if (this.OriginalValues != null && UndefinedValue.IsUndefinedValue (value))
				{
				    value = this.OriginalValues.GetValue (id);
				}
			}
			else if (this.OriginalValues != null)
			{
				value = this.OriginalValues.GetValue (id);
			}
			else
			{
				value = UndefinedValue.Value;
			}
			
			return value;
		}
		
		internal void InternalSetValue(string id, object value)
		{
			if (this.IsDefiningOriginalValues)
			{
				if (this.OriginalValues == null)
				{
					this.CreateOriginalValues ();
				}

				this.OriginalValues.SetValue (id, value, ValueStoreSetMode.Default);
			}
			else
			{
				if (this.ModifiedValues == null)
				{
					this.CreateModifiedValues ();
				}

				this.ModifiedValues.SetValue (id, value, ValueStoreSetMode.Default);
			}
		}

		internal FieldRelation InternalGetFieldRelation(string id)
		{
			StructuredTypeField field = this.context.GetStructuredTypeField (this, id);
			return field == null ? FieldRelation.None : field.Relation;
		}

		internal FieldSource InternalGetFieldSource(string id)
		{
			StructuredTypeField field = this.context.GetStructuredTypeField (this, id);
			return field.Source;
		}


		internal System.Collections.IList InternalGetFieldCollection(string id)
		{
			object value = this.InternalGetValue (id);
			System.Collections.IList list = value as System.Collections.IList;

			if (list == null)
			{
				if (UndefinedValue.IsUndefinedValue (value))
				{
					//	The value store does not (yet) contain a collection for the
					//	specified items. We have to allocate one :

					using (this.DefineOriginalValues ())
					{
						//StructuredTypeField field = this.context.GetStructuredTypeField (this, id);
						//AbstractEntity model = this.context.CreateEmptyEntity (field.TypeId);

						//System.Type itemType = model.GetType ();
						//System.Type genericType = typeof (EntityCollection<>);
						//System.Type collectionType = genericType.MakeGenericType (itemType);

						//list = System.Activator.CreateInstance (collectionType, id, this, true) as System.Collections.IList;
						
						//genericType = typeof (EntityCollectionProxy<>);
						//collectionType = genericType.MakeGenericType (itemType);

						//list = System.Activator.CreateInstance (collectionType, id, this) as System.Collections.IList;

						list = new EntityCollection<AbstractEntity> (id, this, true);

						this.InternalSetValue (id, list);

						list = new EntityCollectionProxy<AbstractEntity> (id, this);
					}
				}
				else
				{
					throw new System.NotSupportedException (string.Format ("Field {0} uses incompatible collection type", id));
				}
			}
			else
			{
				IEntityCollection collection = value as IEntityCollection;

				if (collection == null)
				{
					throw new System.NotSupportedException (string.Format ("Field {0} uses incompatible collection type", id));
				}

				if (collection.HasCopyOnWriteState)
				{
					System.Type itemType = collection.GetItemType ();
					System.Type genericType = typeof (EntityCollectionProxy<>);
					System.Type collectionType = genericType.MakeGenericType (itemType);

					list = System.Activator.CreateInstance (collectionType, id, this) as System.Collections.IList;
				}
			}

			return list;
		}

		internal IEnumerable<IValueStore> InternalGetValueStores()
		{
			if (this.OriginalValues != null)
			{
				yield return this.OriginalValues;
			}
			if (this.ModifiedValues != null)
			{
				yield return this.ModifiedValues;
			}
		}

		internal EntityCollection<T> CopyFieldCollection<T>(string id, EntityCollection<T> collection) where T : AbstractEntity
		{
			System.Diagnostics.Debug.Assert (this.IsDefiningOriginalValues == false);

			EntityCollection<T> copy = new EntityCollection<T> (id, this, false);

			copy.AddRange (collection);

			this.InternalSetValue (id, copy);

			return copy;
		}

		internal IStructuredTypeProvider GetStructuredTypeProvider()
		{
			return (this.OriginalValues ?? this.ModifiedValues) as IStructuredTypeProvider;
		}

		internal IStructuredType GetSyntheticStructuredType(EntityContext context)
		{
			HashSet<string> ids = new HashSet<string> ();

			context.FillValueStoreDataIds (this.OriginalValues, ids);
			context.FillValueStoreDataIds (this.ModifiedValues, ids);

			List<string> list = new List<string> (ids);
			list.Sort ();

			StructuredType type = new StructuredType (StructuredTypeClass.Entity);
			int rank = 0;

			foreach (string id in list)
			{
				StructuredTypeField field = new StructuredTypeField (id, null, Druid.Empty, rank++);
				type.Fields.Add (field);
			}

			return type;
		}

		internal void ReplaceEntityContext(EntityContext newContext)
		{
			var oldContext = this.GetEntityContext ();
			
			if (oldContext != newContext)
			{
				if (this.context != null)
				{
					this.context.NotifyEntityDetached (this);
				}

				this.context = newContext;

				if (this.context != null)
				{
					this.context.NotifyEntityAttached (this);
				}
			}
		}


		/// <summary>
		/// Gets the value for the specified field.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <returns>The value for the specified field.</returns>
		protected virtual object DynamicGetField(string id)
		{
			PropertyGetter getter = this.context.FindPropertyGetter (this, id);

			//	Caution: the getter, if it exists, expects that the calling entity
			//	is of the proper type; we cannot pass in a SearchEntity or a
			//	GenericEntity here !

			if (getter == null)
			{
				return this.GenericGetValue (id);
			}
			else
			{
				return getter (this);
			}
		}

		/// <summary>
		/// Set the value for the specified field.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void DynamicSetField(string id, object newValue)
		{
			if (UndefinedValue.IsUndefinedValue (newValue))
			{
				newValue = null;
			}

			PropertySetter setter = this.context.FindPropertySetter (this, id);

			//	Caution: the setter, if it exists, expects that the calling entity
			//	is of the proper type; we cannot pass in a SearchEntity or a
			//	GenericEntity here !

			if (setter == null)
			{
				this.GenericSetValue (id, this.InternalGetValue (id), newValue);
			}
			else
			{
				setter (this, newValue);
			}
		}

		protected virtual IValueStore OriginalValues
		{
			get
			{
				return this.originalValues;
			}
		}

		protected virtual IValueStore ModifiedValues
		{
			get
			{
				return this.modifiedValues;
			}
		}

		protected virtual void CreateOriginalValues()
		{
			AbstractEntity that = this.Resolve ();

			if (that.originalValues == null)
			{
				that.originalValues = that.context.CreateValueStore (that);
			}
			if (this != that)
			{
				this.originalValues = that.originalValues;
			}
		}

		protected virtual void CreateModifiedValues()
		{
			AbstractEntity that = this.Resolve ();

			if (that.modifiedValues == null)
			{
				that.modifiedValues = that.context.CreateValueStore (that);
			}
			if (this != that)
			{
				this.modifiedValues = that.modifiedValues;
			}
			
		}

		internal void SetModifiedValues(IValueStore values)
		{
			this.modifiedValues = values;
		}

		internal IValueStore GetModifiedValues()
		{
			if (this.ModifiedValues == null)
			{
				this.CreateModifiedValues ();
			}
			return this.ModifiedValues;
		}

		internal void SetOriginalValues(IValueStore values)
		{
			this.originalValues = values;
		}

		internal IValueStore GetOriginalValues()
		{
			if (this.OriginalValues == null)
			{
				this.CreateOriginalValues ();
			}
			return this.OriginalValues;
		}


		/// <summary>
		/// Resolves this instance; override this method if the entity is just
		/// a fa�ade.
		/// </summary>
		/// <returns>The real entity instance.</returns>
		protected virtual AbstractEntity Resolve()
		{
			return this;
		}



		/// <summary>
		/// Gets the value for the specified field, without any casting. Calls
		/// <c>InternalGetValue</c>.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <returns>The value or <c>UndefinedValue.Value</c>.</returns>
		protected object GenericGetValue(string id)
		{
			return this.InternalGetValue (id);
		}

		/// <summary>
		/// Sets the value for the specified field, without any casting. Calls
		/// <c>InternalSetValue</c>, <c>UpdateDataGeneration</c> and <c>NotifyEventHandlers</c>.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected virtual void GenericSetValue(string id, object oldValue, object newValue)
		{
			StructuredTypeField field = this.context.GetStructuredTypeField (this, id);

			if (field == null || field.Type == null)
			{
				throw new System.ArithmeticException (string.Format ("Invalid field '{0}' specified", id));
			}

			System.Diagnostics.Debug.Assert (field != null);
			System.Diagnostics.Debug.Assert (field.Relation != FieldRelation.Collection);

			bool isNullValue = field.IsNullValue (newValue);

			if (isNullValue && (field.IsNullable || context.SkipConstraintChecking))
			{
				//	The value is null and the field is nullable; this operation
				//	is valid and it will clear the field.

				this.InternalSetValue (id, null);
				this.UpdateDataGeneration ();
			}
			else
			{
				IDataConstraint constraint = field.Type as IDataConstraint;

				System.Diagnostics.Debug.Assert (constraint != null);

				if ((context.SkipConstraintChecking) ||
					(constraint.IsValidValue (newValue)))
				{
					object value;

					if (isNullValue)
					{
						value = UndefinedValue.Value;
					}
					else
					{
						value = (object) newValue;
					}

					this.InternalSetValue (id, value);
					this.UpdateDataGeneration ();
				}
				else
				{
					throw new System.ArgumentException (string.Format ("Invalid value '{0}' specified for field {1}", newValue ?? "<null>", id));
				}
			}

			this.NotifyEventHandlers (id, oldValue, newValue);
		}

		/// <summary>
		/// Notifies the event handlers of the field change. This will both
		/// notify the handlers listening for the specific field and those
		/// listening for any field change.
		/// </summary>
		/// <param name="id">The field id.</param>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		protected void NotifyEventHandlers(string id, object oldValue, object newValue)
		{
			if (this.eventHandlers != null)
			{
				System.Delegate fieldHandler;
				System.Delegate contentHandler;

				lock (this.eventHandlers)
				{
					this.eventHandlers.TryGetValue (id, out fieldHandler);
					this.eventHandlers.TryGetValue ("*", out contentHandler);
				}

				if (fieldHandler != null)
				{
					((EventHandler<DependencyPropertyChangedEventArgs>) fieldHandler) (this, new DependencyPropertyChangedEventArgs (id, oldValue, newValue));
				}
				if (contentHandler != null)
				{
					((EventHandler<DependencyPropertyChangedEventArgs>) contentHandler) (this, new DependencyPropertyChangedEventArgs (id, oldValue, newValue));
				}
			}
		}


		/// <summary>
		/// Asserts that the id identifies a simple field.
		/// </summary>
		/// <param name="id">The field id.</param>
		[System.Diagnostics.Conditional ("DEBUG")]
		protected virtual void AssertSimpleField(string id)
		{
			StructuredTypeField field = this.context.GetStructuredTypeField (this, id);

			System.Diagnostics.Debug.Assert (field != null);
			System.Diagnostics.Debug.Assert (field.Relation != FieldRelation.Collection);
		}

		/// <summary>
		/// Asserts that the id identifies a collection field.
		/// </summary>
		/// <param name="id">The field id.</param>
		[System.Diagnostics.Conditional ("DEBUG")]
		protected virtual void AssertCollectionField(string id)
		{
			StructuredTypeField field = this.context.GetStructuredTypeField (this, id);

			System.Diagnostics.Debug.Assert (field != null);
			System.Diagnostics.Debug.Assert (field.Relation == FieldRelation.Collection);
		}

		
		#region IStructuredTypeProvider Members

		IStructuredType IStructuredTypeProvider.GetStructuredType()
		{
			return this.context.GetStructuredType (this);
		}

		#endregion

		#region IStructuredData Members

		/// <summary>
		/// Attaches a listener to the specified structured value.
		/// </summary>
		/// <param name="id">The identifier of the value.</param>
		/// <param name="handler">The handler which implements the listener.</param>
		void IStructuredData.AttachListener(string id, EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			this.EnsureEventHandlers ();

			lock (this.eventHandlers)
			{
				System.Delegate handlers;

				if (this.eventHandlers.TryGetValue (id, out handlers))
				{
					this.eventHandlers[id] = System.Delegate.Combine (handlers, handler);
				}
				else
				{
					this.eventHandlers[id] = handler;
				}
			}
		}

		/// <summary>
		/// Detaches a listener from the specified structured value.
		/// </summary>
		/// <param name="id">The identifier of the value.</param>
		/// <param name="handler">The handler which implements the listener.</param>
		void IStructuredData.DetachListener(string id, EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			this.EnsureEventHandlers ();

			lock (this.eventHandlers)
			{
				System.Delegate handlers;

				if (this.eventHandlers.TryGetValue (id, out handlers))
				{
					handlers = System.Delegate.Remove (handlers, handler);

					if (handlers == null)
					{
						this.eventHandlers.Remove (id);
					}
					else
					{
						this.eventHandlers[id] = handlers;
					}
				}
			}
		}

		/// <summary>
		/// Gets the collection of identifiers used to define the structured values.
		/// </summary>
		/// <returns>The collection of identifiers.</returns>
		IEnumerable<string> IStructuredData.GetValueIds()
		{
			return this.context.GetEntityFieldIds (this);
		}

		/// <summary>
		/// Sets the value. See <see cref="ICloneableValueStore.SetValue"/> for additional
		/// details (the default mode will be used).
		/// </summary>
		/// <param name="id">The identifier of the value.</param>
		/// <param name="value">The value.</param>
		void IStructuredData.SetValue(string id, object value)
		{
			this.DynamicSetField (id, value);
		}

		#endregion

		#region IValueStore Members

		/// <summary>
		/// Gets the value for the specified identifier.
		/// </summary>
		/// <param name="id">The identifier of the value.</param>
		/// <returns>
		/// The value, or either <see cref="UndefinedValue.Value"/> if the
		/// value is currently undefined or <see cref="UnknownValue.Value"/> if the
		/// identifier does not map to a known value.
		/// </returns>
		object IValueStore.GetValue(string id)
		{
			return this.DynamicGetField (id);
		}

		/// <summary>
		/// Sets the value for the specified identifier.
		/// </summary>
		/// <param name="id">The identifier of the value.</param>
		/// <param name="value">The value to store into the structure record;
		/// specifying <see cref="UndefinedValue.Value"/> clears the value.
		/// <see cref="UnknownValue.Value"/> may not be specified as a value.</param>
		/// <param name="mode">The set mode.</param>
		void IValueStore.SetValue(string id, object value, ValueStoreSetMode mode)
		{
			this.DynamicSetField (id, value);
		}

		#endregion

		#region IEntityProxyProvider Members

		IEntityProxy IEntityProxyProvider.GetEntityProxy()
		{
			return this.proxy;
		}

		#endregion

		private void EnsureEventHandlers()
		{
			if (this.eventHandlers == null)
			{
				lock (AbstractEntity.globalExclusion)
				{
					if (this.eventHandlers == null)
					{
						this.eventHandlers = new Dictionary<string, System.Delegate> ();
					}
				}
			}
		}

		/// <summary>
		/// Updates the data generation for this entity to match the one of the
		/// associated context.
		/// </summary>
		internal void UpdateDataGeneration()
		{
			if (this.defineOriginalValuesCount == 0)
			{
				this.dataGeneration = this.context.DataGeneration;
			}
		}

		#region DefineOriginalValuesHelper Class

		/// <summary>
		/// The <c>DefineOriginalValuesHelper</c> is used by the <see cref="DefineOriginalValues"/>
		/// method to manage the end of the definition phase; instances of this class
		/// are meant to be used in a <c>using</c> block.
		/// </summary>
		private sealed class DefineOriginalValuesHelper : System.IDisposable
		{
			public DefineOriginalValuesHelper(AbstractEntity entity)
			{
				this.entity = entity;
				System.Threading.Interlocked.Increment (ref this.entity.defineOriginalValuesCount);
			}

			~DefineOriginalValuesHelper()
			{
				throw new System.InvalidOperationException ("Caller of DefineOriginalValues forgot to call Dispose");
			}

			#region IDisposable Members

			public void Dispose()
			{
				if (this.entity != null)
				{
					System.GC.SuppressFinalize (this);
					System.Threading.Interlocked.Decrement (ref this.entity.defineOriginalValuesCount);
					this.entity = null;
				}
			}

			#endregion

			AbstractEntity entity;
		}

		#endregion

		public static readonly Druid EntityStructuredTypeId = Druid.Empty;
		public static readonly string EntityStructuredTypeKey = null;
		
		private static long nextSerialId = 1;
		private static readonly object globalExclusion = new object ();
		
		private readonly long entitySerialId;
		private EntityContext context;
		private long dataGeneration;
		private int defineOriginalValuesCount;
		private bool calculationsDisabled;
		private IValueStore originalValues;
		private IValueStore modifiedValues;
		private Dictionary<string, System.Delegate> eventHandlers;
		private IEntityProxy proxy;
	}


}
