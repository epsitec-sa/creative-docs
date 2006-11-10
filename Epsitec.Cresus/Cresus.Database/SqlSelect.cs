//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>SqlSelect</c> class describes a SELECT command.
	/// </summary>
	public sealed class SqlSelect
	{
		public SqlSelect()
		{
		}


		public void Add(SqlSelect set_query, SqlSelectSetOp set_op)
		{
			if (this.setQuery == null)
			{
				if (set_op == SqlSelectSetOp.None)
				{
					throw new System.ArgumentException ("Invalid set operation");
				}

				this.setQuery = set_query;
				this.setOp    = set_op;
			}
			else
			{
				this.setQuery.Add (set_query, set_op);
			}
		}


		public SqlSelectPredicate Predicate
		{
			get
			{
				return this.predicate;
			}
			set
			{
				this.predicate = value;
			}
		}

		public Collections.SqlFields Fields
		{
			get
			{
				return this.fields;
			}
		}

		public Collections.SqlFields Tables
		{
			get
			{
				return this.tables;
			}
		}

		public Collections.SqlFields Conditions
		{
			get
			{
				return this.wheres;
			}
		}

		public Collections.SqlFields Joins
		{
			get
			{
				return this.joins;
			}
		}


		public SqlSelect SelectSetQuery
		{
			get
			{
				return this.setQuery;
			}
		}

		public SqlSelectSetOp SelectSetOp
		{
			get
			{
				return this.setOp;
			}
		}



		private Collections.SqlFields fields	= new Collections.SqlFields ();
		private Collections.SqlFields tables	= new Collections.SqlFields ();
		private Collections.SqlFields wheres	= new Collections.SqlFields ();
		private Collections.SqlFields joins	= new Collections.SqlFields ();

		private SqlSelectPredicate predicate	= SqlSelectPredicate.All;
		private SqlSelectSetOp setOp		= SqlSelectSetOp.None;
		private SqlSelect setQuery;
	}
}
