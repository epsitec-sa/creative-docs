//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Collections
{
	/// <summary>
	/// La classe Collections.SqlFields encapsule une collection d'instances de type SqlField.
	/// SqlField.
	/// </summary>
	public class SqlFields : GenericList<SqlField>
	{
		public SqlFields()
		{
		}
		
		
		public virtual void Add(string alias, SqlField field)
		{
			field.Alias = alias;
			this.Add (field);
		}
		
		public virtual void Add(SqlField field, SqlFieldOrder order)
		{
			field.Order = order;
			this.Add (field);
		}
		
		public virtual void Add(string alias, SqlField field, SqlFieldOrder order)
		{
			field.Alias = alias;
			field.Order = order;
			this.Add (field);
		}
		
		public virtual void Add(SqlFunction sql_function)
		{
			SqlField field = SqlField.CreateFunction (sql_function);
			this.Add (field);
		}

		public virtual void Add(string alias, SqlFunction sql_function)
		{
			SqlField field = SqlField.CreateFunction (sql_function);
			field.Alias = alias;
			this.Add (field);
		}
		
		public virtual void Add(SqlAggregate sql_aggregate)
		{
			SqlField field = SqlField.CreateAggregate (sql_aggregate);
			this.Add (field);
		}

		public virtual void Add(string alias, SqlAggregate sql_aggregate)
		{
			SqlField field = SqlField.CreateAggregate (sql_aggregate);
			field.Alias = alias;
			this.Add (field);
		}
		
		public virtual void Add(SqlJoin sql_join)
		{
			SqlField field = SqlField.CreateJoin (sql_join);
			this.Add (field);
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
