//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Collections
{
	/// <summary>
	/// La classe Collections.SqlFields encapsule une collection d'instances de type SqlField.
	/// SqlField.
	/// </summary>
	public class SqlFields : AbstractList
	{
		public SqlFields()
		{
		}
		
		
		public virtual SqlField					this[int index]
		{
			get
			{
				return this.List[index] as SqlField;
			}
		}
		
		public virtual SqlField					this[string field_alias]
		{
			get
			{
				int index = this.IndexOf (field_alias);
				
				if (index >= 0)
				{
					return this[index];
				}
				
				return null;
			}
		}
		
		
		public virtual void Add(SqlField field)
		{
			this.InternalAdd (field);
		}

		public virtual void Add(string alias, SqlField field)
		{
			field.Alias = alias;
			this.InternalAdd (field);
		}
		
		public virtual void Add(SqlField field, SqlFieldOrder order)
		{
			field.Order = order;
			this.InternalAdd (field);
		}
		
		public virtual void Add(string alias, SqlField field, SqlFieldOrder order)
		{
			field.Alias = alias;
			field.Order = order;
			this.InternalAdd (field);
		}
		
		public virtual void Add(SqlFunction sql_function)
		{
			SqlField field = SqlField.CreateFunction (sql_function);
			this.InternalAdd (field);
		}

		public virtual void Add(string alias, SqlFunction sql_function)
		{
			SqlField field = SqlField.CreateFunction (sql_function);
			field.Alias = alias;
			this.InternalAdd (field);
		}
		
		public virtual void Add(SqlAggregate sql_aggregate)
		{
			SqlField field = SqlField.CreateAggregate (sql_aggregate);
			this.InternalAdd (field);
		}

		public virtual void Add(string alias, SqlAggregate sql_aggregate)
		{
			SqlField field = SqlField.CreateAggregate (sql_aggregate);
			field.Alias = alias;
			this.InternalAdd (field);
		}
		
		public virtual void Add(SqlJoin sql_join)
		{
			SqlField field = SqlField.CreateJoin (sql_join);
			this.InternalAdd (field);
		}
		
		public virtual void AddRange(SqlField[] fields)
		{
			this.InternalAddRange (fields);
		}
		
		public virtual void AddRange(SqlFields fields)
		{
			this.InternalAddRange (fields);
		}
		
		public virtual void Remove(SqlField field)
		{
			this.InternalRemove (field);
		}
		
		
		public virtual bool Contains(SqlField field)
		{
			return this.List.Contains (field);
		}
		
		public virtual int IndexOf(SqlField field)
		{
			return this.List.IndexOf (field);
		}
		
		
		public override int IndexOf(string field_alias)
		{
			for (int i = 0; i < this.List.Count; i++)
			{
				SqlField field = this.List[i] as SqlField;
				
				if (field.Alias == field_alias)
				{
					return i;
				}
			}
			
			return -1;
		}
		
		
		public virtual SqlField Merge(SqlFunctionType op)
		{
			//	Produit une expression du genre ((A op B) op (C op D)) en générant
			//	un arbre aussi balancé que possible :
			
			SqlField[] fields = new SqlField[this.list.Count];
			this.list.CopyTo (fields);
			
			return SqlFields.Merge (op, fields);
		}
		
		
		protected static SqlField Merge(SqlFunctionType op, SqlField[] fields)
		{
			int n = fields.Length;
			
			switch (n)
			{
				case 0: return null;
				case 1: return fields[0];
				case 2: return SqlField.CreateFunction (new SqlFunction (op, fields[0], fields[1]));
			}
			
			SqlField[] h1 = new SqlField[n/2];
			SqlField[] h2 = new SqlField[n-n/2];
			
			System.Array.Copy (fields,   0, h1, 0, n/2);
			System.Array.Copy (fields, n/2, h2, 0, n-n/2);
			
			SqlField a = SqlFields.Merge (op, h1);
			SqlField b = SqlFields.Merge (op, h2);
			
			return SqlField.CreateFunction (new SqlFunction (op, a, b));
		}
	}
}
