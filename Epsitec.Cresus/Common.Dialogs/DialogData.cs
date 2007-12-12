//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Dialogs
{
	public class DialogData
	{
		public DialogData(AbstractEntity data, DialogDataMode mode)
		{
			this.originalValues = new Dictionary<EntityFieldPath, object> ();
			this.externalData = data;
			this.internalData = this.CreateProxy (this.externalData, null);
			this.mode = mode;
		}

		private AbstractEntity CreateProxy(AbstractEntity entity, EntityNodeProxy parent)
		{
			EntityContext context = entity.GetEntityContext ();
			Druid entityId = entity.GetEntityStructuredTypeId ();
			AbstractEntity copy = context.CreateEmptyEntity (entityId);

			foreach (string id in context.GetEntityFieldIds (entity))
			{
				copy.InternalSetValue (id, new EntityNodeProxy (parent, id, this, entity));
			}

			return copy;
		}


		public DialogDataMode Mode
		{
			get
			{
				return this.mode;
			}
		}

		public AbstractEntity Data
		{
			get
			{
				return this.internalData;
			}
		}


		private class EntityNodeProxy : IEntityProxy
		{
			public EntityNodeProxy(EntityNodeProxy parent, string nodeId, DialogData host, AbstractEntity externalData)
			{
				this.parent = parent;
				this.nodeId = nodeId;
				this.host = host;
				this.externalData = externalData;
			}

			#region IEntityProxy Members

			public object GetReadEntityValue(IValueStore store, string id)
			{
				object value = this.ResolveNode (id);
				
				if (this.host.mode == DialogDataMode.Isolated)
				{
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
					System.Diagnostics.Debug.Assert (this.host.originalValues.ContainsKey (this.GetFieldPath (id)));

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

			private object ResolveNode(string id)
			{
				object value;

				switch (this.externalData.InternalGetFieldRelation (id))
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
				object value = this.externalData.InternalGetValue (id);
				this.SaveOriginalValue (id, value);
				
				return value;
			}

			private object ResolveReference(string id)
			{
				AbstractEntity reference = this.externalData.InternalGetValue (id) as AbstractEntity;
				this.SaveOriginalValue (id, reference);

				return this.host.CreateProxy (reference, this);
			}

			private object ResolveCollection(string id)
			{
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

					EntityNodeProxy parent = this.parent;

					while (parent != null)
					{
						nodes.Push (parent.nodeId);
						parent = parent.parent;
					}
				}

				return nodes;
			}

			private readonly EntityNodeProxy parent;
			private readonly DialogData host;
			private readonly AbstractEntity externalData;
			private readonly string nodeId;
		}


		private readonly Dictionary<EntityFieldPath, object> originalValues;
		private AbstractEntity					externalData;
		private AbstractEntity					internalData;
		private DialogDataMode					mode;
	}
}
