//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 12/11/2003

using Epsitec.Common.Support;

namespace Epsitec.Cresus.DataLayer
{
	public delegate void DataChangedHandler(DataRecord sender, string path);
	
	/// <summary>
	/// La classe DataRecord sert de base pour DataSet, DataField et DataTable.
	/// </summary>
	public abstract class DataRecord
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
			get { return this.data_type; }
		}
		
		public DataState						DataState
		{
			get { return this.data_state; }
		}
		
		public string							DataLabel
		{
			get { return this.GetAttribute ("label", ResourceLevel.Merged); }
		}
		
		public string							DataDescription
		{
			get { return this.GetAttribute ("descr", ResourceLevel.Merged); }
		}
		
		public DataRecord						Parent
		{
			get { return this.parent; }
		}
		
		
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
			this.data_state = state;
		}
		
		internal virtual void SetParent(DataRecord parent)
		{
			this.parent = parent;
		}
		
		internal virtual void MarkAsModified()
		{
			switch (this.data_state)
			{
				case DataState.Unchanged:
					this.data_state = DataState.Modified;
					break;
				
				case DataState.Added:
				case DataState.Modified:
					break;
				
				default:
					throw new DataException (string.Format ("Illegal state {0}", this.data_state.ToString ()));
			}
		}
		
		internal virtual void MarkAsUnchanged()
		{
			switch (this.data_state)
			{
				case DataState.Unchanged:
					break;
				
				case DataState.Added:
				case DataState.Modified:
					this.data_state = DataState.Unchanged;
					break;
				
				case DataState.Invalid:
				case DataState.Removed:
				default:
					
					//	Ni le data set non initialisé, ni le data set supprimé ne peuvent
					//	être "validés"...
				
					throw new DataException (string.Format ("Illegal state {0}", this.data_state.ToString ()));
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
			else if (this.data_changed_events != null)
			{
				DataChangedHandler handler = this.data_changed_events[path] as DataChangedHandler;
				
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
			else if (this.data_changed_events == null)
			{
				this.data_changed_events = new System.Collections.Hashtable ();
				this.data_changed_events[path] = handler;
			}
			else if (this.data_changed_events.Contains (path))
			{
				DataChangedHandler current_handler = this.data_changed_events[path] as DataChangedHandler;
				this.data_changed_events[path] = current_handler + handler;
			}
			else
			{
				this.data_changed_events[path] = handler;
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
			else if (this.data_changed_events == null)
			{
				//	Rien à faire...
			}
			else if (this.data_changed_events.Contains (path))
			{
				DataChangedHandler current_handler = this.data_changed_events[path] as DataChangedHandler;
				this.data_changed_events[path] = current_handler - handler;
			}
			else
			{
				//	Rien à faire...
			}
		}
		
		
		public string GetAttribute(string name, ResourceLevel level)
		{
			if (this.attributes == null)
			{
				return null;
			}
			
			string find;
			
			switch (level)
			{
				case ResourceLevel.Default:		find = name; break;
				case ResourceLevel.Customised:	find = DbTools.BuildCompositeName (name, Resources.CustomisedSuffix);	break;
				case ResourceLevel.Localised:	find = DbTools.BuildCompositeName (name, Resources.LocalisedSuffix);	break;
				
				case ResourceLevel.Merged:
					
					//	Cas spécial: on veut trouver automatiquement l'attribut le meilleur dans
					//	ce contexte; commence par chercher la variante personnalisée, puis la
					//	variante localisée, pour enfin chercher la variante de base.
					
					find = DbTools.BuildCompositeName (name, Resources.CustomisedSuffix);
					if (this.attributes.Contains (find)) return this.attributes[find] as string;
					
					find = DbTools.BuildCompositeName (name, Resources.LocalisedSuffix);
					if (this.attributes.Contains (find)) return this.attributes[find] as string;
					
					find = name;
					break;
				
				default:
					throw new ResourceException ("Invalid ResourceLevel");
			}
			
			return (this.attributes.Contains (find)) ? this.attributes[find] as string : null;
		}
		
		public void SetAttribute(string name, string value, ResourceLevel level)
		{
			if (this.attributes == null)
			{
				this.attributes = new System.Collections.Hashtable ();
			}
			
			switch (level)
			{
				case ResourceLevel.Default:		break;
				case ResourceLevel.Customised:	name = DbTools.BuildCompositeName (name, Resources.CustomisedSuffix);	break;
				case ResourceLevel.Localised:	name = DbTools.BuildCompositeName (name, Resources.LocalisedSuffix);	break;
				
				default:
					throw new ResourceException ("Invalid ResourceLevel");
			}
			
			this.attributes[name] = value;
		}
		
		
		protected string SplitPath(string path, out string path_remaining)
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
		
		
		protected DataType						data_type;
		protected DataState						data_state = DataState.Invalid;
		protected DataRecord					parent;
		protected System.Collections.Hashtable	data_changed_events;
		protected System.Collections.Hashtable	attributes;
	}
}
