namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbColumnCollection encapsule une collection d'instances de type DbColumn.
	/// </summary>
	public class DbColumnCollection : InternalCollectionBase
	{
		public DbColumnCollection()
		{
		}
		
		public override System.Collections.ArrayList	List
		{
			get { return this.column_list; }
		}
		
		
		public void Add(DbColumn column)
		{
			//	TODO: � compl�ter
		}

		public void AddRange(DbColumn[] columns)
		{
			if (columns == null)
			{
				return;
			}
			//	TODO: � compl�ter
		}
		
		public void Remove(DbColumn column)
		{
			//	TODO: � compl�ter
		}
		
		public void Remove(int index)
		{
			//	TODO: � compl�ter
		}
		
		public void Remove(string column_name)
		{
			//	TODO: � compl�ter
		}
		
		public bool Contains(DbColumn column)
		{
			//	TODO: � compl�ter
			return false;
		}
		
		public bool Contains(string column_name)
		{
			//	TODO: � compl�ter
			return false;
		}
		
		public int IndexOf(DbColumn column)
		{
			//	TODO: � compl�ter
			return -1;
		}
		
		public int IndexOf(string column_name)
		{
			//	TODO: � compl�ter
			return -1;
		}
		
		public void Clear()
		{
			//	TODO: � compl�ter
		}
		
		
		public virtual DbColumn							this[int index]
		{
			get
			{
				//	TODO: � compl�ter
				return null;
			}
		}
		
		public virtual DbColumn							this[string column_name]
		{
			get
			{
				//	TODO: � compl�ter
				return null;
			}
		}
		
		
		protected System.Collections.ArrayList	column_list			= new System.Collections.ArrayList ();
		protected System.Collections.Hashtable	column_from_name	= new System.Collections.Hashtable ();
	}
}
