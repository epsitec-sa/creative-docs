//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe SqlColumnCollection encapsule une collection d'instances de type SqlColumn.
	/// </summary>
	public class SqlColumnCollection : InternalCollectionList
	{
		public SqlColumnCollection()
		{
		}
		
		
		public virtual void Add(SqlColumn column)
		{
			this.List.Add (column);
			this.OnChanged ();
		}

		public virtual void AddRange(SqlColumn[] columns)
		{
			if (columns == null)
			{
				return;
			}
			
			this.List.AddRange (columns);
			this.OnChanged ();
		}
		
		public virtual void Remove(SqlColumn column)
		{
			this.List.Remove (column);
			this.OnChanged ();
		}
		
		
		public virtual bool Contains(SqlColumn column)
		{
			return this.List.Contains (column);
		}
		
		public virtual int IndexOf(SqlColumn column)
		{
			return this.List.IndexOf (column);
		}
		
		
		public override int IndexOf(string column_name)
		{
			for (int i = 0; i < this.List.Count; i++)
			{
				SqlColumn column = this.List[i] as SqlColumn;
				
				if (column.Name == column_name)
				{
					return i;
				}
			}
			
			return -1;
		}
		
		
		public virtual SqlColumn			this[int index]
		{
			get
			{
				return this.List[index] as SqlColumn;
			}
		}
		
		public virtual SqlColumn			this[string column_name]
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
