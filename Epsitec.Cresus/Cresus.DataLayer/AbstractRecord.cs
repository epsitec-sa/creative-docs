//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.DataLayer
{
	public delegate void DataChangedHandler(AbstractRecord sender, string path);
	
	/// <summary>
	/// La classe AbstractRecord sert de base aux DataRecord et autres classes
	/// dérivées.
	/// </summary>
	public abstract class AbstractRecord
	{
		protected AbstractRecord()
		{
		}
		
		public virtual string					Name
		{
			get { return null; }
		}
				
		
		public DataState						State
		{
			get { return this.state; }
		}
		
		public AbstractRecord					Parent
		{
			get { return this.parent; }
		}
		
		
		public bool								IsUnchanged
		{
			get
			{
				return this.State == DataState.Unchanged;
			}
		}
		
		public bool								IsModified
		{
			get
			{
				switch (this.State)
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
				return this.State == DataState.Invalid;
			}
		}
		
		public bool								IsValid
		{
			get
			{
				return this.State != DataState.Invalid;
			}
		}
		
		
		internal string BuildLocalName(string name)
		{
			string this_name = this.Name;
			
			if ((name == null) || (name.Length == 0))
			{
				return this_name;
			}
			
			return (this_name == null) ? name : this_name + "." + name;
		}
		
		
		internal virtual void NotifyDataChanged(string name)
		{
			string path = this.BuildLocalName (name);
			
			if (this.parent != null)
			{
				//	Signale la modification de manière récursive, aussi à tous les
				//	contenants.
				
				this.parent.NotifyDataChanged (path);
			}
			
			if (this.changed_events != null)
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
			path = this.BuildLocalName (path);
			
			if (this.changed_events == null)
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
			path = this.BuildLocalName (path);
			
			if (this.changed_events == null)
			{
				//	Rien à faire...
			}
			else if (this.changed_events.Contains (path))
			{
				DataChangedHandler current_handler = this.changed_events[path] as DataChangedHandler;
				current_handler = current_handler - handler;
				
				if (current_handler.GetInvocationList ().Length == 0)
				{
					current_handler = null;
				}
				
				this.changed_events[path] = current_handler;
			}
			else
			{
				//	Rien à faire...
			}
		}
		
		
		public virtual void ValidateChanges()
		{
			this.SetState (DataState.Unchanged);
		}
		
		internal virtual void SetState(DataState state)
		{
			this.state = state;
		}
		
		internal virtual void SetParent(AbstractRecord parent)
		{
			this.parent = parent;
		}
		
		
		private DataState						state = DataState.Invalid;
		private AbstractRecord					parent;
		protected System.Collections.Hashtable	changed_events;
	}
}
