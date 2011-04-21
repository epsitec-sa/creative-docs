//	Copyright © 2007-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections;
using System.Collections.Generic;

using System.Linq;

using System.Threading;


namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>AbstractEntity</c> class is the base class used to store the
	/// data represented by entity instances.
	/// </summary>
	[System.Diagnostics.DebuggerDisplay ("{DebuggerDisplayValue}")]
	public abstract class AbstractEntity : IStructuredTypeProvider, IStructuredData, IEntityProxyProvider, IReadOnly
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractEntity"/> class.
		/// </summary>
		/// <remarks>
		/// This class is partially thread safe. It has been made as thread safe as required by the
		/// usage as readonly DataContext and readonly entities as cache that are accessed by
		/// several threads at once. This has several implications.
		/// 
		/// All members which are not used in this case such as the implementation of
		/// IStructuredData and of IValueStore are not thread safe.
		/// 
		/// All members that should throw an exception when called on a readonly entity are not
		/// thread safe. Hummm not totally true. That is how you should consider them because in the
		/// case where we want thread safety, the entity is in readonly mode and thus is not
		/// supposed to be modified. So in theory you shouldn't be calling these method in the first
		/// place. However, I made some of them thread safe in order to ensure that whatever the
		/// case, we throw if we must throw the ReadOnlyException or we proceed if we are in the
		/// special mode with readonly checks disabled. I believe that the complete thread safety on
		/// these memebers is overkill, but I felt not confident enought to leave them without some
		/// locking mechanisme that will ensure that we will do the right thing. I hope that by
		/// doing this, I will make it easier to maintain this already hard to maintain code with
		/// regard to thread safety.
		/// 
		/// The members that might modify the internal state of the entity are locked in exclusive
		/// mode. They are the following : 
		/// - GetField
		/// - GetFieldCollection
		/// - IsFieldDefined
		/// - IsFieldNotEmpty
		/// - InternalGetFieldValueOrCollection
		/// - InternalGetValue
		/// - InternalSetValue
		/// - InternalGetFieldCollection
		/// - CreateOriginalValues
		/// - CreateModifiedValues
		/// - SetOriginalValues
		/// - GetOriginalValues
		/// - SetModifiedValues
		/// - GetModifiedValues
		/// - GenericSetValue
		/// - UpdateDataGeneration
		/// - ResetDataGeneration
		/// 
		/// 
		/// The members that don't modify the internal state of the entity are locked in shared
		/// mode. They are the following:
		/// - GetEntityContext
		/// - GetEntityDataGeneration
		/// - GetStructuredTypeProvider
		/// - InternalGetValueStores
		/// - ModifiedValues
		/// - OriginalValues
		/// - NotifyContextEventHandlers
		/// - AssertNotReadOnly
		/// 
		/// The members which return an IDisposable helper object used to modify the way the entity
		/// state require an exclusive access to avoid the situation where one thread calls one such
		/// member while another is doing something with the same entity. Therefore, the exclusive
		/// lock is acquired in the member call and is released when the IDisposable helper object
		/// returned is disposed by the caller. These member are the following:
		/// - DefineOriginalValues
		/// - UseSilentUpdates
		/// - DisableEvents
		/// - DisableReadOnlyChecks
		/// 
		/// Other members are not locked either because they are not supposed to be used in
		/// Cresus.Core or because they are thread safe because they rely on immutable data or because
		/// I know that they are used only in members or in cases where the shared or exclusive lock
		/// must have been acquired before (at least now when I write this comment).
		/// 
		/// Also, we have a special case with the GetFieldCollection and the InternalGetFieldCollection
		/// methods. The problem is that we need to make sure that the collection is not modified
		/// by a thread while another thread is reading it. To ensure that, if we know that the
		/// collection can be accessed by more that one thread, we wrap it inside a
		/// ThreadSafeReadOnlyListProxy (which one depends if the caller expects to get a IList{T}
		/// or an IList back). Those classes are simple wrappers that basically lock their entity in
		/// write mode and forward the call to the underlying collection. Any operation which is not
		/// readonly throws a ReadOnlyException because as the entity is supposed to be in read only
		/// mode, the underlying collection should also throw. The only situation when we don't wrap
		/// the collection is when we already have suspended the readonly checks. In this case, we
		/// know that the entity is already locked by the thread. That means that we trust the caller
		/// not to keep the reference to the collection after he has re enabled the readonly checks.
		/// 
		/// In order to lock the entity, we need to acquire the lock on the associated DataContext. I
		/// wish I could find a solution where we would have one lock per entity, but it turned out
		/// that this solution would be prone to dead locks. Thread1 does something with EntityA
		/// while Thread2 does something with DataContextA. Then the action on EntityA by thread1
		/// requires a proxy resolution so it waits on the lock owned by Thread2. Then the action
		/// on DataContextA by Thread2 requires to to a modifiation on EntityA so it waits on the
		/// lock owned by Thread1. Bang, deadlock! This situation can happen quite often, say because
		/// there is a proxy resolution on a thread while there is a DataContext.Reload() on another.
		/// The only way to avoid those deadlocks is to acquire a lock on the DataContext which
		/// prevents any other thread to do anything that would cause a deadlock. We use two
		/// System.Func{System.IDisposable} in order to avoid mutual dependencies between this
		/// project and the DataLayer project, instead of relying on an interface for instance.
		/// </remarks>
		protected AbstractEntity()
		{
			this.entitySerialId = Interlocked.Increment (ref AbstractEntity.nextSerialId);

			this.context = AbstractEntity.defaultContext;

			this.defineOriginalValues = new InterlockedSafeCounter ();
			this.silentUpdates = new InterlockedSafeCounter ();
			this.disableEvents = new InterlockedSafeCounter ();
			this.disableReadOnlyChecks = new InterlockedSafeCounter ();

			this.eventLock = new object ();

			this.IsReadOnly = false;
		}

		#region IReadOnly Members

		public bool IsReadOnly
		{
			get;
			private set;
		}

		#endregion
	
		/// <summary>
		/// Gets a value indicating whether this entity is currently defining
		/// its original values (see <see cref="DefineOriginalValues"/>).
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this entity is currently defining its original
		///		values; otherwise, <c>false</c>.
		/// </value>
		internal bool IsDefiningOriginalValues
		{
			get
			{
				return !this.defineOriginalValues.IsZero;
			}
		}

		private bool IsUpdateSilent
		{
			get
			{
				return !this.silentUpdates.IsZero;
			}
		}

		internal bool AreEventsEnabled
		{
			get
			{
				return this.disableEvents.IsZero;
			}
		}

		private bool ReadOnlyChecksEnabled
		{
			get
			{
				return this.disableReadOnlyChecks.IsZero;
			}
		}

		/// <summary>
		/// Retourne un résumé complet de l'entité.
		/// </summary>
		/// <returns></returns>
		public virtual FormattedText GetSummary()
		{
			return FormattedText.Empty;
		}

		/// <summary>
		/// Retourne un résumé court (d'une seule ligne) de l'entité.
		/// </summary>
		/// <returns></returns>
		public virtual FormattedText GetCompactSummary()
		{
			return FormattedText.Empty;
		}

		/// <summary>
		/// Retourne la liste des mots-clé de l'entité, en vue d'une recherche.
		/// </summary>
		/// <returns></returns>
		public virtual string[] GetEntityKeywords()
		{
			return null;
		}

		public bool IsEntityEmpty
		{
			get
			{
				return this.GetEntityStatus ().HasFlag (EntityStatus.Empty);
			}
		}

		public bool IsEntityValid
		{
			get
			{
				return this.GetEntityStatus ().HasFlag (EntityStatus.Valid);
			}
		}

		public virtual EntityStatus GetEntityStatus()
		{
			return EntityStatus.None;
		}

		/// <summary>
		/// Freezes this instance and makes it read-only. A read-only instance may not be thawed
		/// (it stays frozen forever).
		/// </summary>
		public virtual void Freeze()
		{
			this.IsReadOnly = true;
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
		/// Gets the value which should be displayed by the debugger when the entity is
		/// inspected in the variables window (or data tip, etc.).
		/// </summary>
		/// <value>The debugger display value.</value>
		internal string DebuggerDisplayValue
		{
			get
			{
#if true
				return this.GetType ().Name;
#else
				var buffer = new System.Text.StringBuilder ();
				this.Dump (buffer, topLevelOnly: true, skipUndefinedFields: true);
				return string.Concat (this.GetType ().Name, ": ", string.Join (", ", buffer.ToString ().Split (new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries)));
#endif
			}
		}

		/// <summary>
		/// The id of the DataContext that manages this instance.
		/// </summary>
		internal long? DataContextId
		{
			get;
			set;
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
			using (this.LockRead ())
			{
				return this.context;
			}
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
			using (this.LockRead ())
			{
				return this.dataGeneration;
			}
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

		public string Dump()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			this.Dump (buffer, includeLabels: true);
			return buffer.ToString ();
		}

		public string DumpFlatData(System.Predicate<StructuredTypeField> filter = null)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			this.Dump (buffer, filter: filter);
			return buffer.ToString ();
		}

		private void Dump(System.Text.StringBuilder buffer, int level = 0, HashSet<AbstractEntity> history = null, System.Predicate<StructuredTypeField> filter = null, bool includeLabels = false, bool topLevelOnly = false, bool skipUndefinedFields = false)
		{
			if (history == null)
            {
				history = new HashSet<AbstractEntity> ();
            }

			string indent = new string (' ', level*2);
			string name   = "";
			
			if (history.Add (this))
			{
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
						Caption caption = this.context.GetCaption (field.CaptionId);
						name = caption.Name;
					}

					object value = this.DynamicGetField (id);
					AbstractEntity child = value as AbstractEntity;

					if (skipUndefinedFields)
					{
						if ((value == null) ||
							(value == UndefinedValue.Value))
						{
							continue;
						}
					}

					switch (this.InternalGetFieldRelation (id))
					{
						case FieldRelation.None:
							buffer.AppendFormat (includeLabels ? "{0}{1}: {2}\n" : "{2}\n", indent, name, value == null ? nullValue : value.ToString ());
							break;

						case FieldRelation.Reference:
							if (topLevelOnly)
							{
								continue;
							}

							if ((child == null) ||
								(EntityNullReferenceVirtualizer.IsNullEntity (child)))
							{
								buffer.AppendFormat (includeLabels ? "{0}{1}: {2}\n" : "\n", indent, name, nullValue);
							}
							else
							{
								if (includeLabels)
								{
									buffer.AppendFormat ("{0}{1}:\n", indent, name);
								}
								child.Dump (buffer, level+1, history, filter, includeLabels, topLevelOnly, skipUndefinedFields);
							}
							break;

						case FieldRelation.Collection:
							if (topLevelOnly)
							{
								continue;
							}

							if (includeLabels)
							{
								buffer.AppendFormat ("{0}{1}{2}:\n", indent, name, "{");
							}
							int index = 0;
							foreach (object item in (System.Collections.IList) value)
							{
								child = item as AbstractEntity;
								if ((child == null) ||
									(EntityNullReferenceVirtualizer.IsNullEntity (child)))
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
									child.Dump (buffer, level+2, history, filter, includeLabels, topLevelOnly, skipUndefinedFields);
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
			var d1 = this.LockWrite () ?? EmptyDisposable.Instance;
			var d2 = this.silentUpdates.Enter ();
			var d3 = this.defineOriginalValues.Enter ();

			return DisposableWrapper.Combine (d3, d2, d1);
		}

		public System.IDisposable UseSilentUpdates()
		{
			var d1 = this.LockWrite () ?? EmptyDisposable.Instance;
			var d2 = this.silentUpdates.Enter ();

			return DisposableWrapper.Combine (d2, d1);
		}


		public System.IDisposable DisableEvents()
		{
			var d1 = this.LockWrite () ?? EmptyDisposable.Instance;
			var d2 = this.disableEvents.Enter ();

			return DisposableWrapper.Combine (d2, d1);
		}

		public System.IDisposable DisableReadOnlyChecks()
		{
			var d1 = this.LockWrite () ?? EmptyDisposable.Instance;
			var d2 = this.disableReadOnlyChecks.Enter ();

			return DisposableWrapper.Combine (d2, d1);
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
		/// entity is a façade, this will return the real underlying entity, cast
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

			if ((UndefinedValue.IsUndefinedValue (value)) ||
				(value == null))
			{
				//	If T is a non-nullable value type, we must return its default value
				//	instead. A FormattedText, for instance, might be stored as <null> in
				//	the entity, its equivalent is the default FormattedText instance.

				return default (T);
			}
			
			return (T) value;
		}

		public IList<T> GetFieldCollection<T>(string id) where T : AbstractEntity
		{
			this.AssertCollectionField (id);

			using (this.LockWrite ())
			{
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
							list = new EntityCollection<T> (id, this, copyOnWrite: true);
							this.InternalSetValue (id, list, ValueStoreSetMode.InitialCollection);
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

				if (this.MustAcquireCollectionLock ())
				{
					list = new ThreadSafeReadOnlyListProxy<T> (this, list);
				}

				return list;
			}
		}

		public void SetField<T>(string id, T newValue)
		{
			using (this.LockWrite ())
			{
				this.AssertIsNotReadOnly ();

				T oldValue = this.GetField<T> (id);
				this.SetField<T> (id, oldValue, newValue);
			}
		}
		
		public void SetField<T>(string id, T oldValue, T newValue)
		{
			using (this.LockWrite ())
			{
				this.AssertIsNotReadOnly ();

				this.GenericSetValue (id, oldValue, newValue);
			}
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

		/// <summary>
		/// Tells whether the field given by <paramref name="id"/> is defined.
		/// </summary>
		/// <remarks>
		/// The only way for a field to be undefined (for this method) is if the underlying value
		/// stores don't stored any value for it, or if the value stored is
		/// <see cref="Undefined.Value"/>. In any other case (even a <c>null</c> value), the field is
		/// considered as defined for this method.
		/// </remarks>
		/// <param name="id">The id of the field.</param>
		/// <returns><c>true</c> if the field is defined, <c>false</c> if it isn't.</returns>
		public bool IsFieldDefined(string id)
		{
			object value = this.InternalGetValue (id);

			return !UndefinedValue.IsUndefinedValue (value);
		}

		/// <summary>
		/// Tells whether the field given by <paramref name="fieldId"/> does contain a non empty
		/// value.
		/// </summary>
		/// <remarks>
		/// A non empty value for value and reference field means that the value must not be the
		/// undefined value or null. A non empty value for collection fields means that the value
		/// must be a collection with at least one item.
		/// </remarks>
		/// <param name="id">The id of the field.</param>
		/// <returns><c>true</c> if the field is defined, <c>false</c> if it isn't.</returns>
		internal bool IsFieldNotEmpty(string fieldId)
		{
			bool isNotEmpty;

			StructuredTypeField field = this.context.GetStructuredTypeField (this, fieldId);

			object value = this.InternalGetValueOrFieldCollection (fieldId);

			switch (field.Relation)
			{
				case FieldRelation.None:
				case FieldRelation.Reference:

					isNotEmpty = (value != null) && !UndefinedValue.IsUndefinedValue(value);

					break;

				case FieldRelation.Collection:

					IList values = value as IList;

					isNotEmpty = (values != null) && (values.Count > 0);

					break;

				default:
					throw new System.NotSupportedException ();
			}

			return isNotEmpty;
		}

		private object InternalGetValueOrFieldCollection(string id)
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
			using (this.LockWrite ())
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
		}

		internal void InternalSetValue(string id, object value, ValueStoreSetMode mode = ValueStoreSetMode.Default)
		{
			using (this.LockWrite ())
			{
				if (this.IsDefiningOriginalValues)
				{
					if (this.OriginalValues == null)
					{
						this.CreateOriginalValues ();
					}

					this.OriginalValues.SetValue (id, value, mode);
				}
				else
				{
					if (this.ModifiedValues == null)
					{
						this.CreateModifiedValues ();
					}

					this.ModifiedValues.SetValue (id, value, mode);
				}
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
			using (this.LockWrite ())
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
							// TODO It would be nice to instantiate a EntityCollection<> with the proper
							// type and not AbstractEntity. Below was the way it was done before. A good
							// way of doing it would be to add a method for that in every concrete class
							// of entities.
							// Marc

							//StructuredTypeField field = this.context.GetStructuredTypeField (this, id);
							//AbstractEntity model = this.context.CreateEmptyEntity (field.TypeId);

							//System.Type itemType = model.GetType ();
							//System.Type genericType = typeof (EntityCollection<>);
							//System.Type collectionType = genericType.MakeGenericType (itemType);

							//list = System.Activator.CreateInstance (collectionType, id, this, true) as System.Collections.IList;

							//genericType = typeof (EntityCollectionProxy<>);
							//collectionType = genericType.MakeGenericType (itemType);

							//list = System.Activator.CreateInstance (collectionType, id, this) as System.Collections.IList;

							list = new EntityCollection<AbstractEntity> (id, this, copyOnWrite: true);
							this.InternalSetValue (id, list, ValueStoreSetMode.InitialCollection);
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

				if (this.MustAcquireCollectionLock ())
				{
					list = new ThreadSafeReadOnlyListProxy (this, list);
				}

				return list;
			}
		}

		internal EntityCollection<T> CopyFieldCollection<T>(string id, EntityCollection<T> collection) where T : AbstractEntity
		{
			System.Diagnostics.Debug.Assert (this.IsDefiningOriginalValues == false);

			EntityCollection<T> copy = new EntityCollection<T> (id, this, copyOnWrite: false);

			using (copy.DisableNotifications ())
			{
				copy.AddRange (collection);
			}

			this.InternalSetValue (id, copy);

			return copy;
		}
		
		#region IStructuredTypeProvider Members

		IStructuredType IStructuredTypeProvider.GetStructuredType()
		{
			return this.context.GetStructuredType (this);
		}

		#endregion

		internal IStructuredTypeProvider GetStructuredTypeProvider()
		{
			using (this.LockRead ())
			{
				return (this.OriginalValues ?? this.ModifiedValues) as IStructuredTypeProvider;
			}
		}

		internal StructuredType GetSyntheticStructuredType(EntityContext context)
		{
			var ids = EntityContext.GetValueStoreDataIds (this.originalValues)
				   .Concat (EntityContext.GetValueStoreDataIds (this.modifiedValues))
				   .Distinct ()
				   .OrderBy (id => id)
				   .ToList ();

			StructuredType type = new StructuredType (StructuredTypeClass.Entity);
			int rank = 0;

			foreach (string id in ids)
			{
				StructuredTypeField field = new StructuredTypeField (id, null, Druid.Empty, rank++);
				type.Fields.Add (field);
			}

			return type;
		}

		internal void AssignEntityContext(EntityContext newContext)
		{
			using (this.LockWrite ())
			{
				var oldContext = this.GetEntityContext ();

				if (oldContext != newContext)
				{
					if (this.context != null)
					{
						this.context.NotifyEntityDetached (this, newContext);
					}

					this.context = newContext;

					if (this.context != null)
					{
						this.context.NotifyEntityAttached (this, oldContext);
					}
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
				using (this.LockRead ())
				{
					return this.originalValues;
				}
			}
		}

		protected virtual IValueStore ModifiedValues
		{
			get
			{
				using (this.LockRead ())
				{
					return this.modifiedValues;
				}
			}
		}

		internal IEnumerable<IValueStore> InternalGetValueStores()
		{
			using (this.LockRead ())
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
		}

		protected virtual void CreateOriginalValues()
		{
			using (this.LockWrite ())
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
		}

		protected virtual void CreateModifiedValues()
		{
			using (this.LockWrite ())
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
		}

		internal IValueStore GetOriginalValues()
		{
			using (this.LockWrite ())
			{
				if (this.OriginalValues == null)
				{
					this.CreateOriginalValues ();
				}

				return this.OriginalValues;
			}
		}

		internal void SetOriginalValues(IValueStore values)
		{
			using (this.LockWrite ())
			{
				this.originalValues = values;
			}
		}

		internal IValueStore GetModifiedValues()
		{
			using (this.LockWrite ())
			{
				if (this.ModifiedValues == null)
				{
					this.CreateModifiedValues ();
				}

				return this.ModifiedValues;
			}
		}

		internal void SetModifiedValues(IValueStore values)
		{
			using (this.LockWrite ())
			{
				this.modifiedValues = values;
			}
		}

		/// <summary>
		/// Resolves this instance; override this method if the entity is just
		/// a façade.
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
			using (this.LockWrite ())
			{
				this.AssertIsNotReadOnly ();

				StructuredTypeField field = this.context.GetStructuredTypeField (this, id);

				if (field == null || field.Type == null)
				{
					throw new System.ArithmeticException (string.Format ("Invalid field '{0}' specified", id));
				}

				System.Diagnostics.Debug.Assert (field != null);
				System.Diagnostics.Debug.Assert (field.Relation != FieldRelation.Collection);

				bool isNullValue = field.IsNullValue (newValue);

				if (isNullValue && (field.IsNullable || this.context.SkipConstraintChecking))
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

					if ((this.context.SkipConstraintChecking) ||
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
				this.NotifyContextEventHandlers (id, oldValue, newValue);
			}
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

		protected void NotifyContextEventHandlers(string id, object oldValue, object newValue)
		{
			using (this.LockRead ())
			{
				if (this.context != null)
				{
					this.context.NotifyEntityChanged (this, id, oldValue, newValue);
				}
			}
		}

		internal void AssertIsNotReadOnly()
		{
			using (this.LockRead())
			{
				if (this.ReadOnlyChecksEnabled)
				{
					IReadOnlyExtensions.ThrowIfReadOnly (this);
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
		
		#region IStructuredData Members

		/// <summary>
		/// Attaches a listener to the specified structured value.
		/// </summary>
		/// <param name="id">The identifier of the value.</param>
		/// <param name="handler">The handler which implements the listener.</param>
		void IStructuredData.AttachListener(string id, EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			this.EnsureEventHandlers ();

			lock (this.eventLock)
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

			lock (this.eventLock)
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
			this.AssertIsNotReadOnly ();

			this.DynamicSetField (id, value);
		}

		#endregion

		private void EnsureEventHandlers()
		{
			if (this.eventHandlers == null)
			{
				lock (this.eventLock)
				{
					if (this.eventHandlers == null)
					{
						this.eventHandlers = new Dictionary<string, System.Delegate> ();
					}
				}
			}
		}

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
			this.AssertIsNotReadOnly ();

			this.DynamicSetField (id, value);
		}

		#endregion

		internal void InternalDefineProxy(IEntityProxy proxy)
		{
			this.proxy = proxy;
		}

		#region IEntityProxyProvider Members

		IEntityProxy IEntityProxyProvider.GetEntityProxy()
		{
			return this.proxy;
		}

		#endregion

		/// <summary>
		/// Updates the data generation for this entity to match the one of the
		/// associated context.
		/// </summary>
		internal void UpdateDataGeneration()
		{
			using (this.LockWrite ())
			{
				if (!this.IsUpdateSilent)
				{
					this.dataGeneration = this.context.DataGeneration;
				}
			}
		}

		/// <summary>
		/// Resets the data generation for this entity to zero.
		/// </summary>
		internal void ResetDataGeneration()
		{
			using (this.LockWrite ())
			{
				this.dataGeneration = 0;
			}
		}

		/// <summary>
		/// Updates the data generation for this entity to match the one of the
		/// associated context, and notifies the <see cref="EntityContext"/>
		/// about the change.
		/// </summary>
		internal void UpdateDataGenerationAndNotifyEntityContextAboutChange()
		{
			this.UpdateDataGeneration ();

			this.NotifyEventHandlers ("*", null, null);
			this.NotifyContextEventHandlers ("*", null, null);
		}

		internal void SetModifiedValuesAsOriginalValues()
		{
			IValueStore originalValues = this.GetOriginalValues ();
			IValueStore modifiedValues = this.GetModifiedValues ();

			IEnumerable<string> fieldIds = this.context.GetEntityFieldIds (this);

			using (this.UseSilentUpdates ())
			{
				foreach (Druid fieldId in fieldIds.Select (id => Druid.Parse (id)))
				{
					AbstractEntity.SetModifiedValueAsOriginalValue (originalValues, modifiedValues, fieldId);
				}
			}
		}

		private static void SetModifiedValueAsOriginalValue(IValueStore originalValues, IValueStore modifiedValues, Druid fieldId)
		{
			string fieldName     = fieldId.ToResourceId ();
			object modifiedValue = modifiedValues.GetValue (fieldName);

			if (!UndefinedValue.IsUndefinedValue (modifiedValue))
			{
				if (modifiedValue is IEntityCollection)
				{
					((IEntityCollection) modifiedValue).ResetCopyOnWrite ();
				}
				
				originalValues.SetValue (fieldName, modifiedValue, ValueStoreSetMode.ShortCircuit);
				modifiedValues.SetValue (fieldName, UndefinedValue.Value, ValueStoreSetMode.ShortCircuit);
			}
		}

		internal void ResetValueStores()
		{
			IEnumerable<string> fieldIds = this.context.GetEntityFieldIds (this);

			using (this.UseSilentUpdates ())
			{
				foreach (Druid fieldId in fieldIds.Select (id => Druid.Parse (id)))
				{
					this.ResetValue (fieldId);
				}
			}
		}

		private void ResetValue(Druid fieldId)
		{
			string fieldName = fieldId.ToResourceId ();

			IValueStore originalValues = this.GetOriginalValues ();
			IValueStore modifiedValues = this.GetModifiedValues ();

			originalValues.SetValue (fieldName, UndefinedValue.Value, ValueStoreSetMode.ShortCircuit);
			modifiedValues.SetValue (fieldName, UndefinedValue.Value, ValueStoreSetMode.ShortCircuit);
		}

		internal void DefineLockFunctions(System.Func<System.IDisposable> readLockFunction, System.Func<System.IDisposable> writeLockFunction)
		{
			this.readLockFunction = readLockFunction;
			this.writeLockFunction = writeLockFunction;
		}

		internal System.IDisposable LockRead()
		{
			return this.IsReadOnly && this.readLockFunction != null
				? this.readLockFunction ()
				: null;
		}

		internal System.IDisposable LockWrite()
		{
			return this.IsReadOnly && this.writeLockFunction != null
				? this.writeLockFunction ()
				: null;
		}

		private bool MustAcquireCollectionLock()
		{
			return this.IsReadOnly
				&& this.readLockFunction != null
				&& this.writeLockFunction != null
				&& this.ReadOnlyChecksEnabled;
		}

		private readonly InterlockedSafeCounter silentUpdates;
		private readonly InterlockedSafeCounter defineOriginalValues;
		private readonly InterlockedSafeCounter disableEvents;
		private readonly InterlockedSafeCounter disableReadOnlyChecks;

		private readonly long entitySerialId;
		private readonly object eventLock;

        private System.Func<System.IDisposable> readLockFunction;
		private System.Func<System.IDisposable> writeLockFunction;

		private EntityContext context;
		private long dataGeneration;
		
		private bool calculationsDisabled;
		private IValueStore originalValues;
		private IValueStore modifiedValues;
		private Dictionary<string, System.Delegate> eventHandlers;
		
		private IEntityProxy proxy;

		static AbstractEntity()
		{
			AbstractEntity.defaultContext = new EntityContext ();
			AbstractEntity.lockTimeOut = System.TimeSpan.FromSeconds (15);
		}

		private static long nextSerialId = 1;

		private static readonly EntityContext defaultContext;

		private static readonly System.TimeSpan lockTimeOut;
		
	}
}