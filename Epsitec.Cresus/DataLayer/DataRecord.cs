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
		
		
		public virtual DataType					DataType
		{
			get { return this.data_type; }
		}
		
		
		public virtual DataRecord FindRecord(string path)
		{
			return null;
		}
		
		
		protected virtual string SplitPath(string path, out string path_remaining)
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
	}
}
