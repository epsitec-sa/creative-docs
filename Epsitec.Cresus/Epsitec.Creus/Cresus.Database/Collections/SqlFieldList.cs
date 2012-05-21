//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Collections
{
	/// <summary>
	/// The <c>Collections.SqlFieldList</c> class manages a list of <c>SqlField</c> items.
	/// </summary>
	public sealed class SqlFieldList : GenericList<SqlField>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlFieldList"/> class.
		/// </summary>
		public SqlFieldList()
		{
		}

		/// <summary>
		/// Adds the specified field with a given alias.
		/// </summary>
		/// <param name="alias">The alias.</param>
		/// <param name="field">The field.</param>
		public void Add(string alias, SqlField field)
		{
			field = field.Clone ();
			field.Alias = alias;
			
			this.Add (field);
		}

		/// <summary>
		/// Adds the specified field with a given SQL ordering.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="order">The order.</param>
		public void Add(SqlField field, SqlSortOrder order)
		{
			field = field.Clone ();
			field.SortOrder = order;
			
			this.Add (field);
		}

		/// <summary>
		/// Adds the specified field with a given alias and SQL ordering.
		/// </summary>
		/// <param name="alias">The alias.</param>
		/// <param name="field">The field.</param>
		/// <param name="order">The order.</param>
		public void Add(string alias, SqlField field, SqlSortOrder order)
		{
			field = field.Clone ();
			field.Alias = alias;
			field.SortOrder = order;
			
			this.Add (field);
		}

		/// <summary>
		/// Adds the specified SQL function.
		/// </summary>
		/// <param name="sqlFunction">The SQL function.</param>
		public void Add(SqlFunction sqlFunction)
		{
			SqlField field = SqlField.CreateFunction (sqlFunction);
			this.Add (field);
		}

		/// <summary>
		/// Adds the specified SQL function with a given alias.
		/// </summary>
		/// <param name="alias">The alias.</param>
		/// <param name="sqlFunction">The SQL function.</param>
		public void Add(string alias, SqlFunction sqlFunction)
		{
			SqlField field = SqlField.CreateFunction (sqlFunction);
			field.Alias = alias;
			this.Add (field);
		}

		/// <summary>
		/// Adds the specified SQL aggregate.
		/// </summary>
		/// <param name="sqlAggregate">The SQL aggregate.</param>
		public void Add(SqlAggregate sqlAggregate)
		{
			SqlField field = SqlField.CreateAggregate (sqlAggregate);
			this.Add (field);
		}

		/// <summary>
		/// Adds the specified SQL aggregate with a given alias.
		/// </summary>
		/// <param name="alias">The alias.</param>
		/// <param name="sqlAggregate">The SQL aggregate.</param>
		public void Add(string alias, SqlAggregate sqlAggregate)
		{
			SqlField field = SqlField.CreateAggregate (sqlAggregate);
			field.Alias = alias;
			this.Add (field);
		}

		/// <summary>
		/// Adds the specified SQL join.
		/// </summary>
		/// <param name="sqlJoin">The SQL join.</param>
		public void Add(SqlJoin sqlJoin)
		{
			SqlField field = SqlField.CreateJoin (sqlJoin);
			this.Add (field);
		}

		/// <summary>
		/// Merges all fields by generating the SQL functions defined by a
		/// given SQL operation. This will produce a tree encoding ((A op B)
		/// op (C op D)), for instance.
		/// </summary>
		/// <param name="op">The SQL operation.</param>
		/// <returns>A SQL field object encoding a function applied to all items.</returns>
		public SqlField Merge(SqlFunctionCode op)
		{
			return SqlFieldList.Merge (op, this.ToArray ());
		}
		
		
		private static SqlField Merge(SqlFunctionCode op, SqlField[] fields)
		{
			int n = fields.Length;
			
			switch (n)
			{
				case 0: return null;
				case 1: return fields[0].Clone ();
				case 2: return SqlField.CreateFunction (new SqlFunction (op, fields[0].Clone (), fields[1].Clone ()));
			}
			
			SqlField[] h1 = new SqlField[n/2];
			SqlField[] h2 = new SqlField[n-h1.Length];
			
			System.Array.Copy (fields,   0, h1, 0, n/2);
			System.Array.Copy (fields, n/2, h2, 0, n-n/2);
			
			SqlField a = SqlFieldList.Merge (op, h1);
			SqlField b = SqlFieldList.Merge (op, h2);
			
			return SqlField.CreateFunction (new SqlFunction (op, a, b));
		}
	}
}
