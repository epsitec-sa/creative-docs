//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 23/11/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTypeCollection encapsule une collection d'instances de type DbType.
	/// </summary>
	public class DbTypeCollection : InternalCollectionList
	{
		public DbTypeCollection()
		{
		}
		
		
		public virtual void Add(DbType type)
		{
			this.List.Add (type);
			this.OnChanged ();
		}

		public virtual void AddRange(DbType[] types)
		{
			if (types == null)
			{
				return;
			}
			
			this.List.AddRange (types);
			this.OnChanged ();
		}
		
		public virtual void Remove(DbType type)
		{
			this.List.Remove (type);
			this.OnChanged ();
		}
		
		
		public virtual bool Contains(DbType type)
		{
			return this.List.Contains (type);
		}
		
		public virtual int IndexOf(DbType type)
		{
			return this.List.IndexOf (type);
		}
		
		public override int IndexOf(string type_name)
		{
			for (int i = 0; i < this.List.Count; i++)
			{
				DbType type = this.List[i] as DbType;
				
				if (type.Name == type_name)
				{
					return i;
				}
			}
			
			return -1;
		}
		
		
		public virtual DbType				this[int index]
		{
			get
			{
				return this.List[index] as DbType;
			}
		}
		
		public virtual DbType				this[string type_name]
		{
			get
			{
				int index = this.IndexOf (type_name);
				
				if (index >= 0)
				{
					return this[index];
				}
				
				return null;
			}
		}
	}
}
