﻿//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			while (this.ForEachChange (change => change.Path.NavigateWrite (this.internalData, change.OldValue)))
			{
				//	Run until we have processed all changes.
			}
		}


		/// <summary>
		/// Walks through every change done by the user on the dialog data. This
		/// filters changes done in referenced entities if the reference itself
		/// was changed too.
		/// </summary>
		/// <param name="action">The action to apply on each change set item.</param>
		/// <returns><c>true</c> if the action was executed at least once; otherwise, <c>false</c>.</returns>
		public bool ForEachChange(System.Action<DialogDataChangeSet> action)
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

					action (change);
					executed++;
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
		private class FieldProxy : IEntityProxy
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

				this.parent = parent;
				this.nodeId = nodeId;
				this.host = host;
				this.externalData = externalData;
				this.relation = this.externalData.InternalGetFieldRelation (this.nodeId);
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
					
					this.SaveOriginalValue (id, () => value);
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

					this.SaveOriginalValue (id, () => this.ResolveField ());

					IValueStore externalStore = this.externalData;
					
					switch (this.externalData.InternalGetFieldRelation (id))
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

			public object PromoteToRealInstance()
			{
				throw new System.NotImplementedException ();
			}

			#endregion

			private AbstractEntity Wrap(AbstractEntity reference)
			{
				//	TODO: handle wrapping <null> !

				IEntityProxyProvider provider = reference;

				if (provider == null)
				{
					return null;
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
				return this.externalData.InternalGetValue (this.nodeId);
			}

			private object ResolveReference()
			{
				if (this.proxy == null)
				{
					this.CreateReferenceProxy (this.externalData.InternalGetValue (this.nodeId) as AbstractEntity);
				}

				return this.proxy;
			}

			private void CreateReferenceProxy(AbstractEntity reference)
			{
				this.proxy = this.host.CreateEntityProxy (reference, this);
				this.proxySource = reference;
			}

			private object ResolveCollection()
			{
				throw new System.NotImplementedException ();
			}

			private void SaveOriginalValue(string id, System.Func<object> valueGetter)
			{
				EntityFieldPath path = this.GetFieldPath (id);

				if (this.host.originalValues.ContainsKey (path) == false)
				{
					this.host.originalValues[path] = valueGetter ();
				}
			}

			private EntityFieldPath GetFieldPath(string id)
			{
				return EntityFieldPath.CreateRelativePath (this.GetNodeIds (null));
			}

			private IEnumerable<string> GetNodeIds(string id)
			{
				Stack<string> nodes = new Stack<string> ();

				if (id != null)
				{
					nodes.Push (id);
				}

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

			foreach (string id in context.GetEntityFieldIds (entity))
			{
				copy.InternalSetValue (id, new FieldProxy (parent, id, this, entity));
			}

			return copy;
		}


		private readonly Dictionary<EntityFieldPath, object> originalValues;
		private readonly DialogDataMode			mode;
		private readonly AbstractEntity			externalData;
		private readonly AbstractEntity			internalData;
	}
}
