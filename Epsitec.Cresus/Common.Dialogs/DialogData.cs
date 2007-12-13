//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			public FieldProxy(FieldProxy parent, string nodeId, DialogData host, AbstractEntity externalData)
			{
				this.parent = parent;
				this.nodeId = nodeId;
				this.host = host;
				this.externalData = externalData;
			}

			#region IEntityProxy Members

			public object GetReadEntityValue(IValueStore store, string id)
			{
				FieldRelation relation = this.externalData.InternalGetFieldRelation (id);
				object        value    = this.ResolveField (id, relation);
				
				if ((this.host.mode == DialogDataMode.Isolated) ||
					(relation != FieldRelation.None))
				{
					this.SaveOriginalValue (id, value);
					store.SetValue (id, value);
				}
				
				return value;
			}

			public object GetWriteEntityValue(IValueStore store, string id)
			{
				return this;
			}

			public bool DiscardWriteEntityValue(IValueStore store, string id, object value)
			{
				if (this.host.mode == DialogDataMode.RealTime)
				{
					this.SaveOriginalValue (id, this.ResolveValue (id));
					this.externalData.InternalSetValue (id, value);

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

			private object ResolveField(string id, FieldRelation relation)
			{
				object value;

				switch (relation)
				{
					case FieldRelation.None:
						value = this.ResolveValue (id);
						break;

					case FieldRelation.Reference:
						value = this.ResolveReference (id);
						break;

					case FieldRelation.Collection:
						value = this.ResolveCollection (id);
						break;
					
					default:
						throw new System.NotSupportedException ();
				}
				
				return value;
			}

			private object ResolveValue(string id)
			{
				System.Diagnostics.Debug.Assert (this.nodeId == id);

				object value = this.externalData.InternalGetValue (id);
				
				return value;
			}

			private object ResolveReference(string id)
			{
				System.Diagnostics.Debug.Assert (this.nodeId == id);

				if (this.proxy == null)
				{
					AbstractEntity reference = this.externalData.InternalGetValue (id) as AbstractEntity;
					this.proxy = this.host.CreateEntityProxy (reference, this);
				}

				return this.proxy;
			}

			private object ResolveCollection(string id)
			{
				System.Diagnostics.Debug.Assert (this.nodeId == id);

				throw new System.NotImplementedException ();
			}

			private void SaveOriginalValue(string id, object value)
			{
				EntityFieldPath path = this.GetFieldPath (id);

				if (this.host.originalValues.ContainsKey (path) == false)
				{
					this.host.originalValues[path] = value;
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
			private AbstractEntity proxy;
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
