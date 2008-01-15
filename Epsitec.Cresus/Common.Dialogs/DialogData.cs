//	Copyright © 2007-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>DialogData</c> class describes data associated with a dialog.
	/// The data is stored in an entity graph and the <c>DialogData</c> class
	/// manages change tracking.
	/// </summary>
	public class DialogData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DialogData"/> class.
		/// </summary>
		/// <param name="data">The dialog data.</param>
		/// <param name="mode">The dialog data mode.</param>
		public DialogData(AbstractEntity data, DialogDataMode mode)
		{
			this.originalValues = new Dictionary<EntityFieldPath, object> ();
			this.mode = mode;
			this.externalData = data;
			this.internalData = this.CreateEntityProxy (this.externalData, null);
		}


		/// <summary>
		/// Gets the dialog data mode.
		/// </summary>
		/// <value>The dialog data mode.</value>
		public DialogDataMode Mode
		{
			get
			{
				return this.mode;
			}
		}

		/// <summary>
		/// Gets the (live) dialog data.
		/// </summary>
		/// <value>The dialog data.</value>
		public AbstractEntity Data
		{
			get
			{
				return this.internalData;
			}
		}

		/// <summary>
		/// Gets the changes. The result is sorted based on the field path of
		/// every change set item.
		/// </summary>
		/// <value>The changes.</value>
		public IEnumerable<DialogDataChangeSet> Changes
		{
			get
			{
				List<EntityFieldPath> paths = new List<EntityFieldPath> (this.originalValues.Keys);

				paths.Sort ();

				foreach (EntityFieldPath path in paths)
				{
					object oldValue = this.originalValues[path];
					object newValue = path.NavigateRead (this.internalData);

					yield return new DialogDataChangeSet (path, oldValue, newValue);
				}
			}
		}

		/// <summary>
		/// Applies the changes to the original dialog data.
		/// </summary>
		public void ApplyChanges()
		{
			this.ApplyChanges (this.externalData);
		}

		/// <summary>
		/// Applies the changes to the specified data.
		/// </summary>
		/// <param name="data">The data.</param>
		public void ApplyChanges(AbstractEntity data)
		{
			this.ForEachChange (change => change.Path.NavigateWrite (data, change.NewValue));
		}

		/// <summary>
		/// Reverts the changes. This will restore the dialog data fields which
		/// were modified by the user, since the dialog data was first created.
		/// </summary>
		public void RevertChanges()
		{
			using (this.internalData.GetEntityContext ().SuspendConstraintChecking ())
			{
				while (this.ForEachChange (change => change.Path.NavigateWrite (this.internalData, change.OldValue)))
				{
					//	Run until we have processed all changes.
				}
			}
		}


		/// <summary>
		/// Binds the dialog data to the specified user interface.
		/// </summary>
		/// <param name="panel">The user interface panel.</param>
		public void BindToUserInterface(UI.Panel panel)
		{
			if (panel != null)
			{
				IStructuredData data = this.Data;
				UI.DataSource source = panel.DataSource;

				source.SetDataSource (UI.DataSource.DataName, data);
			}
		}

		/// <summary>
		/// Unbinds the dialog data from the specified user interface.
		/// </summary>
		/// <param name="panel">The user interface panel.</param>
		public void UnbindFromUserInterface(UI.Panel panel)
		{
			if (panel != null)
			{
				UI.DataSource source = panel.DataSource;

				source.SetDataSource (UI.DataSource.DataName, null);
			}
		}


		/// <summary>
		/// Walks through every change done by the user on the dialog data. This
		/// filters changes done in referenced entities if the reference itself
		/// was changed too.
		/// </summary>
		/// <param name="action">The action to apply on each change set item.</param>
		/// <returns><c>true</c> if the action was executed at least once; otherwise, <c>false</c>.</returns>
		public bool ForEachChange(System.Predicate<DialogDataChangeSet> action)
		{
			List<string> skipList = new List<string> ();
			int executed = 0;

			foreach (DialogDataChangeSet change in this.Changes)
			{
				if (change.DifferentValues)
				{
					bool ignore = false;
					string path = change.Path.ToString ();

					foreach (string skipPrefix in skipList)
					{
						if (path.StartsWith (skipPrefix))
						{
							ignore = true;
							break;
						}
					}

					if (ignore)
					{
						continue;
					}

					skipList.Add (path);

					if (action (change))
					{
						executed++;
					}
				}
			}

			return executed > 0;
		}


		#region Private FieldProxy Class

		/// <summary>
		/// The <c>FieldProxy</c> class is used to spy on every read or write
		/// done on fields from an entity, which is used to track changes in
		/// an entity graph.
		/// </summary>
		internal class FieldProxy : IEntityProxy, INullable
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="FieldProxy"/> class.
			/// </summary>
			/// <param name="parent">The parent proxy.</param>
			/// <param name="nodeId">The node id.</param>
			/// <param name="host">The <see cref="DialogData"/> hosting this proxy.</param>
			/// <param name="externalData">The external data.</param>
			public FieldProxy(FieldProxy parent, string nodeId, DialogData host, AbstractEntity externalData)
			{
				System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (nodeId));
				System.Diagnostics.Debug.Assert (host != null);
				System.Diagnostics.Debug.Assert (externalData != null);
				System.Diagnostics.Debug.Assert (host.mode != DialogDataMode.Transparent);

				EntityContext context = externalData.GetEntityContext ();
				StructuredTypeField field = context.GetStructuredTypeField (externalData, nodeId);

				this.parent = parent;
				this.nodeId = nodeId;
				this.host = host;
				this.externalData = externalData;
				this.relation = field.Relation;
				this.options  = field.Options;
			}


			/// <summary>
			/// Gets the field relation.
			/// </summary>
			/// <value>The field relation.</value>
			public FieldRelation FieldRelation
			{
				get
				{
					return this.relation;
				}
			}

			/// <summary>
			/// Gets the field options.
			/// </summary>
			/// <value>The field options.</value>
			public FieldOptions FieldOptions
			{
				get
				{
					return this.options;
				}
			}

			/// <summary>
			/// Gets the parent of this field, i.e. the field which refers to
			/// this field's containing entity.
			/// </summary>
			/// <value>The parent.</value>
			public FieldProxy Parent
			{
				get
				{
					return this.parent;
				}
			}

			/// <summary>
			/// Gets the dialog data.
			/// </summary>
			/// <value>The dialog data.</value>
			public DialogData DialogData
			{
				get
				{
					return this.host;
				}
			}

			
			public EntityFieldPath GetFieldPath()
			{
				return EntityFieldPath.CreateRelativePath (this.GetNodeIds ());
			}


			/// <summary>
			/// Unwraps the value. This gets the source data for a value, if it
			/// is represented by a <c>FieldProxy</c>. Otherwise, it simply
			/// returns the value, as is.
			/// </summary>
			/// <param name="value">The value.</param>
			/// <returns>The unwrapped value.</returns>
			public static object Unwrap(object value)
			{
				IEntityProxyProvider provider = value as IEntityProxyProvider;
				
				if (provider != null)
				{
					FieldProxy proxy = provider.GetEntityProxy () as FieldProxy;

					if (proxy != null)
					{
						//	We have found a proxy, so get the source data used to initialize it;
						//	this is the "unwrapped" data.

						System.Diagnostics.Debug.Assert (proxy.proxy == value);
						System.Diagnostics.Debug.Assert (proxy.proxySource != null);

						value = proxy.proxySource;
					}
				}

				return value;
			}
			

			#region IEntityProxy Members

			/// <summary>
			/// Gets the real instance to be used when reading on this proxy.
			/// </summary>
			/// <param name="store">The value store.</param>
			/// <param name="id">The value id.</param>
			/// <returns>The real instance to be used.</returns>
			public object GetReadEntityValue(IValueStore store, string id)
			{
				System.Diagnostics.Debug.Assert (this.nodeId == id);
				
				object value = this.ResolveField ();

				if ((this.host.mode == DialogDataMode.Isolated) ||
					(this.relation != FieldRelation.None))
				{
					//	Record the original value for this node, if we have never done
					//	so previously, then overwrite the value in the data field.

					this.SaveOriginalValue (() => value);

					if ((value == null) &&
						(this.relation == FieldRelation.Reference) &&
						((this.options & FieldOptions.Nullable) == 0))
					{
						//	We have just come across a null value in a reference that
						//	was not defined to be nullable. Let's replace it with an
						//	empty entity instead.

						EntityContext context = this.externalData.GetEntityContext ();
						StructuredTypeField field = context.GetStructuredTypeField (this.externalData, this.nodeId);
						AbstractEntity entity = context.CreateEmptyEntity (field.TypeId);

						value = this.Wrap (entity);
					}

					store.SetValue (id, value, ValueStoreSetMode.ShortCircuit);
				}
				
				return value;
			}

			/// <summary>
			/// This is a no-op.
			/// </summary>
			/// <param name="store">The value store.</param>
			/// <param name="id">The value id.</param>
			/// <returns>The real instance to be used.</returns>
			public object GetWriteEntityValue(IValueStore store, string id)
			{
				System.Diagnostics.Debug.Assert (this.nodeId == id);
				
				return this;
			}

			/// <summary>
			/// Checks if the write to the specified entity value should proceed
			/// normally or be discarded completely.
			/// </summary>
			/// <param name="internalStore">The value store.</param>
			/// <param name="id">The value id.</param>
			/// <param name="value">The value.</param>
			/// <returns>
			/// 	<c>true</c> if the value should be discarded; otherwise, <c>false</c>.
			/// </returns>
			public bool DiscardWriteEntityValue(IValueStore internalStore, string id, ref object value)
			{
				System.Diagnostics.Debug.Assert (this.nodeId == id);

				if (this.host.mode == DialogDataMode.RealTime)
				{
					//	The value store is about to overwrite our value with the specified
					//	new value.

					this.SaveOriginalValue (() => this.ResolveField ());

					IValueStore externalStore = this.externalData;
					
					switch (this.relation)
					{
						case FieldRelation.None:
							//	Update the external data field and keep our proxy in the
							//	internal data field :

							externalStore.SetValue (id, value, ValueStoreSetMode.ShortCircuit);
							break;

						case FieldRelation.Reference:
							//	Update the external data field to point to the unwrapped
							//	value and replace the internal data field with the new,
							//	wrapped, value.

							externalStore.SetValue (id, FieldProxy.Unwrap (value), ValueStoreSetMode.ShortCircuit);
							internalStore.SetValue (id, this.Wrap (value as AbstractEntity), ValueStoreSetMode.ShortCircuit);
							break;

						default:
							throw new System.NotSupportedException ();
					}

					return true;
				}
				else
				{
					return false;
				}
			}

			/// <summary>
			/// Promotes the proxy to its real instance.
			/// </summary>
			/// <returns>The real instance.</returns>
			public object PromoteToRealInstance()
			{
				throw new System.NotImplementedException ();
			}

			#endregion

			#region INullable Members

			/// <summary>
			/// Gets a value indicating whether this proxy represents a null value.
			/// </summary>
			/// <value><c>true</c> if the value is null; otherwise, <c>false</c>.</value>
			public bool IsNull
			{
				get
				{
					return (this.proxy != null) && (this.proxySource == null);
				}
			}

			#endregion
			
			/// <summary>
			/// Wraps the specified reference within a proxy. This will create a
			/// copy of the entity, attached to a <c>FieldProxy</c> to manage all
			/// field accesses (both read and write).
			/// </summary>
			/// <param name="reference">The reference data.</param>
			/// <returns>The wrapped data.</returns>
			private AbstractEntity Wrap(AbstractEntity reference)
			{
				IEntityProxyProvider provider = reference;

				if (provider == null)
				{
					//	Wrapping a null entity requires some work; we have to create
					//	a fake entity in order to attach a proxy to it and then rely
					//	on IValueStore.GetValue to properly handle the fact that the
					//	entity should be handled as a null.

					EntityContext context = this.externalData.GetEntityContext ();
					IStructuredType type = context.GetStructuredType (this.externalData);

					FieldProxy proxy = new FieldProxy (this.parent, this.nodeId, this.host, this.externalData);
					
					proxy.proxy = this.host.CreateNullProxy (proxy, context, type.GetField (this.nodeId).TypeId);
					
					return proxy.proxy;
				}
				else
				{
					FieldProxy proxy = provider.GetEntityProxy () as FieldProxy;

					if (proxy == null)
					{
						proxy = new FieldProxy (this.parent, this.nodeId, this.host, this.externalData);
						proxy.CreateReferenceProxy (reference);
					}

					return proxy.proxy;
				}
			}

			private object ResolveField()
			{
				switch (this.relation)
				{
					case FieldRelation.None:
						return this.ResolveValue ();
					
					case FieldRelation.Reference:
						return this.ResolveReference ();
					
					case FieldRelation.Collection:
						return this.ResolveCollection ();

					default:
						throw new System.NotSupportedException ();
				}
			}

			private object ResolveValue()
			{
				IValueStore store = this.externalData;

				return store.GetValue (this.nodeId);
			}

			private object ResolveReference()
			{
				if (this.proxy == null)
				{
					IValueStore store = this.externalData;
					AbstractEntity data = store.GetValue (this.nodeId) as AbstractEntity;

					if (data == null)
					{
						//	We do not create a proxy when resolving a null entity.
					}
					else
					{
						this.CreateReferenceProxy (data);
					}
				}

				return this.proxy;
			}

			private void CreateReferenceProxy(AbstractEntity reference)
			{
				System.Diagnostics.Debug.Assert (reference != null);

				this.proxy = this.host.CreateEntityProxy (reference, this);
				this.proxySource = reference;
			}

			private object ResolveCollection()
			{
				throw new System.NotImplementedException ();
			}

			private void SaveOriginalValue(System.Func<object> valueGetter)
			{
				EntityFieldPath path = this.GetFieldPath ();

				if (this.host.originalValues.ContainsKey (path) == false)
				{
					this.host.originalValues[path] = valueGetter ();
				}
			}

			private IEnumerable<string> GetNodeIds()
			{
				Stack<string> nodes = new Stack<string> ();

				if (this.nodeId != null)
				{
					nodes.Push (this.nodeId);

					FieldProxy parent = this.parent;

					while (parent != null)
					{
						nodes.Push (parent.nodeId);
						parent = parent.parent;
					}
				}

				return nodes;
			}

			private readonly FieldProxy parent;
			private readonly DialogData host;
			private readonly AbstractEntity externalData;
			private readonly string nodeId;
			private readonly FieldRelation relation;
			private readonly FieldOptions options;
			private AbstractEntity proxy;
			private AbstractEntity proxySource;
		}

		#endregion

		/// <summary>
		/// Creates an entity filled with field proxies, which mirrors the
		/// specified source entity.
		/// </summary>
		/// <param name="entity">The source entity.</param>
		/// <param name="parent">The parent proxy (if any).</param>
		/// <returns>The mirroring entity.</returns>
		private AbstractEntity CreateEntityProxy(AbstractEntity entity, FieldProxy parent)
		{
			if (this.mode == DialogDataMode.Transparent)
			{
				//	When the transparent data mode is active, there is no need
				//	to create a proxy -- just use the source entity itself !

				return entity;
			}

			EntityContext context = entity.GetEntityContext ();
			Druid entityId = entity.GetEntityStructuredTypeId ();
			AbstractEntity copy = context.CreateEmptyEntity (entityId);
			
			copy.InternalDefineProxy (parent);

			if ((parent != null) &&
				((parent.FieldOptions & FieldOptions.PrivateRelation) == 0))
			{
				//	If this entity is accessed through a shared relation, we have
				//	to disable all calculations so that the user can type in values
				//	in them to build a search query :

				context.DisableCalculations (copy);
			}

			foreach (string id in context.GetEntityFieldIds (entity))
			{
				copy.InternalSetValue (id, new FieldProxy (parent, id, this, entity));
			}

			return copy;
		}

		private AbstractEntity CreateNullProxy(FieldProxy parent, EntityContext context, Druid entityId)
		{
			if (this.mode == DialogDataMode.Transparent)
			{
				//	When the transparent data mode is active, there is no need
				//	to create a proxy -- just use the source entity itself !

				return null;
			}

			AbstractEntity copy = context.CreateEmptyEntity (entityId);

			copy.InternalDefineProxy (parent);

			return copy;
		}


		private readonly Dictionary<EntityFieldPath, object> originalValues;
		private readonly DialogDataMode			mode;
		private readonly AbstractEntity			externalData;
		private readonly AbstractEntity			internalData;
	}
}
