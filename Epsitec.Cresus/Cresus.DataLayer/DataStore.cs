//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.DataLayer
{
	public class DataChangeEventArgs : System.EventArgs
	{
		public DataChangeEventArgs(string path, System.Data.DataRowAction action)
		{
			this.path   = path;
			this.action = action;
		}
		
		
		public string							Path
		{
			get { return this.path; }
		}
		
		public System.Data.DataRowAction		Action
		{
			get { return this.action; }
		}
		
		
		protected string						path;
		protected System.Data.DataRowAction		action;
	}
	
	public delegate void DataChangeEventHandler(object sender, DataChangeEventArgs e);
	
	/// <summary>
	/// La classe DataStore encapsule les données stockées dans
	/// un System.Data.DataSet.
	/// </summary>
	public class DataStore
	{
		public DataStore(System.Data.DataSet data_set)
		{
			this.data_set = data_set;
		}
		
		
		public object							this[string path]
		{
			get 
			{
				return this.GetRowItem (path);
			}
			set
			{
				this.SetRowItem (path, value);
			}
		}
		
		
		
		public void AttachObserver(string path, DataChangeEventHandler handler)
		{
			if (this.changed_events == null)
			{
				this.changed_events = new System.Collections.Hashtable ();
				this.changed_events[path] = handler;
			}
			else if (this.changed_events.Contains (path))
			{
				DataChangeEventHandler current_handler = this.changed_events[path] as DataChangeEventHandler;
				this.changed_events[path] = current_handler + handler;
			}
			else
			{
				this.changed_events[path] = handler;
			}
		}
		
		public void DetachObserver(string path, DataChangeEventHandler handler)
		{
			if (this.changed_events == null)
			{
				//	Rien à faire...
			}
			else if (this.changed_events.Contains (path))
			{
				DataChangeEventHandler current_handler = this.changed_events[path] as DataChangeEventHandler;
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
		
		
		protected string[] SplitPath(string path)
		{
			return path.Split ('.');
		}
		
		protected void CheckPathLength(string[] path, int min)
		{
			if (path.Length < min)
			{
				throw new System.ArgumentException (string.Format ("Path '{0}' has not enough elements, {1} expected", string.Join (".", path), min));
			}
		}
		
		
		protected System.Data.DataRow GetRow(string[] path)
		{
			this.CheckPathLength (path, 2);
			
			string table_name = path[0];
			int    row_index  = System.Int32.Parse (path[1], System.Globalization.CultureInfo.InvariantCulture);
			
			System.Data.DataTable table = this.data_set.Tables[table_name];
			System.Data.DataRow   row   = table.Rows[row_index];
			
			return row;
		}
		
		
		protected object GetRowItem(string path)
		{
			string[] elem = this.SplitPath (path);
			this.CheckPathLength (elem, 3);
			
			string col_name = elem[2];
			System.Data.DataRow row = this.GetRow (elem);
			
			return row[col_name];
		}
		
		protected void SetRowItem(string path, object value)
		{
			string[] elem = this.SplitPath (path);
			this.CheckPathLength (elem, 3);
			
			string col_name = elem[2];
			System.Data.DataRow row = this.GetRow (elem);
			
			row[col_name] = value;
			
			this.NotifyDataChanged (elem, path, System.Data.DataRowAction.Change);
		}
		
		
		protected void NotifyDataChanged(string[] elem, string path, System.Data.DataRowAction action)
		{
			if (this.changed_events == null)
			{
				return;
			}
			
			DataChangeEventArgs e = new DataChangeEventArgs (path, action);
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			for (int i = 0; i < elem.Length; i++)
			{
				if (i > 0)
				{
					buffer.Append (".");
				}
				
				buffer.Append (path[i]);
				
				this.OnDataChanged (buffer.ToString (), e);
			}
		}
		
		
		protected virtual void OnDataChanged(string path, DataChangeEventArgs e)
		{
			DataChangeEventHandler handler = this.changed_events[path] as DataChangeEventHandler;
			
			if (handler != null)
			{
				handler (this, e);
			}
		}
		
		
		
		protected System.Data.DataSet			data_set;
		protected System.Collections.Hashtable	changed_events;
	}
}
