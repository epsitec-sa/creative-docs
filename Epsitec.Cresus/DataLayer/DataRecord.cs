//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 03/11/2003

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
					
					//	Ni le data set non initialis�, ni le data set supprim� ne peuvent
					//	�tre "valid�s"...
				
					throw new DataException (string.Format ("Illegal state {0}", this.data_state.ToString ()));
			}
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
