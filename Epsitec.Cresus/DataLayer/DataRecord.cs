//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.DataLayer
{
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
		
		
		internal virtual void SetDataState(DataState state)
		{
			this.data_state = state;
		}
		
		internal virtual void SetParent(DataRecord parent)
		{
			this.parent = parent;
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
	}
}
