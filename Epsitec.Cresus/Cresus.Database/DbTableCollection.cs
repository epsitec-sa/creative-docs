//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 23/11/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTableCollection encapsule une collection d'instances de type DbTable.
	/// </summary>
	public class DbTableCollection : InternalCollectionList
	{
		public DbTableCollection()
		{
		}
		
		
		public virtual void Add(DbTable table)
		{
			this.List.Add (table);
		}

		public virtual void AddRange(DbTable[] tables)
		{
			if (tables == null)
			{
				return;
			}
			
			this.List.AddRange (tables);
		}
		
		public virtual void Remove(DbTable table)
		{
			this.List.Remove (table);
		}
		
		
		public virtual bool Contains(DbTable table)
		{
			return this.List.Contains (table);
		}
		
		public virtual int IndexOf(DbTable table)
		{
			return this.List.IndexOf (table);
		}
		
		public override int IndexOf(string table_name)
		{
			for (int i = 0; i < this.List.Count; i++)
			{
				DbTable table = this.List[i] as DbTable;
				
				if (table.Name == table_name)
				{
					return i;
				}
			}
			
			return -1;
		}
		
		
		public virtual DbTable			this[int index]
		{
			get
			{
				return this.List[index] as DbTable;
			}
		}
		
		public virtual DbTable			this[string table_name]
		{
			get
			{
				int index = this.IndexOf (table_name);
				
				if (index >= 0)
				{
					return this[index];
				}
				
				return null;
			}
		}
	}
}
