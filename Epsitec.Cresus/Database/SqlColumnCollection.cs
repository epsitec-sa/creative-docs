namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbColumnCollection encapsule une collection d'instances de type DbColumn.
	/// </summary>
	public class DbColumnCollection : InternalCollectionList
	{
		public DbColumnCollection()
		{
		}
		
		
		public virtual void Add(DbColumn column)
		{
			this.List.Add (column);
		}

		public virtual void AddRange(DbColumn[] columns)
		{
			if (columns == null)
			{
				return;
			}
			
			this.List.AddRange (columns);
		}
		
		public virtual void Remove(DbColumn column)
		{
			this.List.Remove (column);
		}
		
		
		public virtual bool Contains(DbColumn column)
		{
			return this.List.Contains (column);
		}
		
		public virtual int IndexOf(DbColumn column)
		{
			return this.List.IndexOf (column);
		}
		
		public override int IndexOf(string column_name)
		{
			for (int i = 0; i < this.List.Count; i++)
			{
				DbColumn column = this.List[i] as DbColumn;
				
				if (column.Name == column_name)
				{
					return i;
				}
			}
			
			return -1;
		}
		
		
		
		public virtual DbColumn							this[int index]
		{
			get
			{
				return this.List[index] as DbColumn;
			}
		}
		
		public virtual DbColumn							this[string column_name]
		{
			get
			{
				int index = this.IndexOf (column_name);
				
				if (index >= 0)
				{
					return this[index];
				}
				
				return null;
			}
		}
	}
}
