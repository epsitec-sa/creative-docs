//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 12/11/2003

using Epsitec.Common.Support;

namespace Epsitec.Cresus.DataLayer
{
	public delegate void DataChangedHandler(DataRecord sender, string path);
	
	/// <summary>
	/// La classe DataRecord sert de base pour DataSet, DataField et DataTable.
	/// </summary>
	public abstract class DataRecord : IDataAttributesHost
	{
		protected DataRecord()
		{
		}
		
		
		public virtual bool						IsField
		{
			get { return false; }
		}
		
		public virtual bool						IsSet
		{
			get { return false; }
		}
		
		public virtual bool						IsTable
		{
			get { return false; }
		}
		
		
		public virtual string					Name
		{
			get { return null; }
		}
				
		
		public bool								IsUnchanged
		{
			get
			{
				return this.DataState == DataState.Unchanged;
			}
		}
		
		public bool								IsModified
		{
			get
			{
				switch (this.DataState)
				{
					case DataState.Modified:
					case DataState.Added:
					case DataState.Removed:
						return true;
					default:
						return false;
				}
			}
		}
		
		public bool								IsInvalid
		{
			get
			{
				return this.DataState == DataState.Invalid;
			}
		}
		
		public bool								IsValid
		{
			get
			{
				return this.DataState != DataState.Invalid;
			}
		}
		
		
		public DataType							DataType
		{
			get { return this.type; }
		}
		
		public DataState						DataState
		{
			get { return this.state; }
		}
		
		public DataRecord						Parent
		{
			get { return this.parent; }
		}
		
		
		public string							UserLabel
		{
			get { return this.Attributes.GetAttribute (DataAttributes.TagLabel, ResourceLevel.Merged); }
		}
		
		public string							UserDescription
		{
			get { return this.Attributes.GetAttribute (DataAttributes.TagDescription, ResourceLevel.Merged); }
		}
		
		
		#region IDataAttributesHost Members
		public DataAttributes					Attributes
		{
			get
			{
				if (this.attributes == null)
				{
					this.attributes = new DataAttributes ();
				}
				
				return this.attributes;
			}
		}
		#endregion
		
		public DataRecord FindRecord(string path)
		{
			return this.FindRecord (path, DataVersion.Active);
		}
		
		public virtual DataRecord FindRecord(string path, DataVersion version)
		{
			return null;
		}
		
		public virtual void ValidateChanges()
		{
		}
		
		
		internal virtual void SetDataState(DataState state)
		{
			this.state = state;
		}
		
		internal virtual void SetParent(DataRecord parent)
		{
			this.parent = parent;
		}
		
		internal virtual void MarkAsModified()
		{
			switch (this.state)
			{
				case DataState.Unchanged:
					this.state = DataState.Modified;
					break;
				
				case DataState.Added:
				case DataState.Modified:
					break;
				
				default:
					throw new DataException (string.Format ("Illegal state {0}", this.state.ToString ()));
			}
		}
		
		internal virtual void MarkAsUnchanged()
		{
			switch (this.state)
			{
				case DataState.Unchanged:
					break;
				
				case DataState.Added:
				case DataState.Modified:
					this.state = DataState.Unchanged;
					break;
				
				case DataState.Invalid:
				case DataState.Removed:
				default:
					
					//	Ni le data set non initialisé, ni le data set supprimé ne peuvent
					//	être "validés"...
				
					throw new DataException (string.Format ("Illegal state {0}", this.state.ToString ()));
			}
		}
		
		internal virtual void NotifyDataChanged(string name)
		{
			string path;
			
			if (this.Name == null)
			{
				path = name;
			}
			else
			{
				path = this.Name + "." + name;
			}
			
			if (this.parent != null)
			{
				this.parent.NotifyDataChanged (path);
			}
			else if (this.changed_events != null)
			{
				DataChangedHandler handler = this.changed_events[path] as DataChangedHandler;
				
				if (handler != null)
				{
					handler (this, name);
				}
			}
		}
		
		
		public void AttachObserver(string path, DataChangedHandler handler)
		{
			if (this.Name != null)
			{
				path = this.Name + "." + path;
			}
			
			if (this.parent != null)
			{
				this.parent.AttachObserver (path, handler);
			}
			else if (this.changed_events == null)
			{
				this.changed_events = new System.Collections.Hashtable ();
				this.changed_events[path] = handler;
			}
			else if (this.changed_events.Contains (path))
			{
				DataChangedHandler current_handler = this.changed_events[path] as DataChangedHandler;
				this.changed_events[path] = current_handler + handler;
			}
			else
			{
				this.changed_events[path] = handler;
			}
		}
		
		public void DetachObserver(string path, DataChangedHandler handler)
		{
			if (this.Name != null)
			{
				path = this.Name + "." + path;
			}
			
			if (this.parent != null)
			{
				this.parent.DetachObserver (path, handler);
			}
			else if (this.changed_events == null)
			{
				//	Rien à faire...
			}
			else if (this.changed_events.Contains (path))
			{
				DataChangedHandler current_handler = this.changed_events[path] as DataChangedHandler;
				this.changed_events[path] = current_handler - handler;
			}
			else
			{
				//	Rien à faire...
			}
		}
		
		
		protected static string SplitPath(string path, out string path_remaining)
		{
			System.Diagnostics.Debug.Assert (path != null);
			
			int pos = path.IndexOf ('.');
			
			if (pos < 0)
			{
				path_remaining = null;
				return path;
			}
			
			path_remaining = path.Substring (pos+1);
			return path.Substring (0, pos);
		}
		
		
		protected DataType						type;
		protected DataState						state = DataState.Invalid;
		protected DataAttributes				attributes;
		protected DataRecord					parent;
		protected System.Collections.Hashtable	changed_events;
	}
}
