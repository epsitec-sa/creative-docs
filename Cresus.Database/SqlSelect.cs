//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD
using Epsitec.Cresus.Database.Collections;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>SqlSelect</c> class describes a SELECT command.
	/// </summary>
	public sealed class SqlSelect
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlSelect"/> class.
		/// </summary>
		public SqlSelect()
		{
			this.fields	 = new SqlFieldList ();
			this.tables	 = new SqlFieldList ();
			this.wheres	 = new SqlFieldList ();
			this.joins	 = new SqlFieldList ();
			this.orderBy = new SqlFieldList ();
		}

		
		/// <summary>
		/// Gets or sets the SELECT predicate.
		/// </summary>
		/// <value>The SELECT predicate.</value>
		public SqlSelectPredicate				Predicate
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

		/// <summary>
		/// The number of rows to remove from the result of the query.
		/// </summary>
		public int?								Skip
		{
			get
			{
				return this.skip;
			}
			set
			{
				if (value < 0)
				{
					throw new System.ArgumentException ();
				}

				this.skip = value;
			}
		}

		/// <summary>
		/// The number of rows retrieve from the result of the query.
		/// </summary>
		public int?								Take
		{
			get
			{
				return this.take;
			}
			set
			{
				if (value < 0)
				{
					throw new System.ArgumentException ();
				}
				
				this.take = value;
			}
		}

		/// <summary>
		/// Gets the fields for the columns that will be returned by the query.
		/// </summary>
		/// <value>The fields.</value>
		public SqlFieldList						Fields
		{
			get
			{
				return this.fields;
			}
		}

		/// <summary>
		/// Gets the tables.
		/// </summary>
		/// <value>The tables.</value>
		public SqlFieldList						Tables
		{
			get
			{
				return this.tables;
			}
		}

		/// <summary>
		/// Gets the conditions for the WHERE clause.
		/// </summary>
		/// <value>The conditions.</value>
		public SqlFieldList						Conditions
		{
			get
			{
				return this.wheres;
			}
		}

		/// <summary>
		/// Gets the joins.
		/// </summary>
		/// <value>The joins.</value>
		public SqlFieldList						Joins
		{
			get
			{
				return this.joins;
			}
		}
		
		/// <summary>
		/// Gets the field that are part of the ORDER BY clause of the query.
		/// </summary>
		public SqlFieldList						OrderBy
		{
			get
			{
				return this.orderBy;
			}
		}

		/// <summary>
		/// Gets the SELECT set query.
		/// </summary>
		/// <value>The SELECT set query.</value>
		public SqlSelect						SetQuery
		{
			get
			{
				return this.setQuery;
			}
		}

		/// <summary>
		/// Gets the SELECT set operation.
		/// </summary>
		/// <value>The SELECT set operation.</value>
		public SqlSelectSetOp					SetOp
		{
			get
			{
				return this.setOp;
			}
		}

		
		/// <summary>
		/// Adds the specified set query.
		/// </summary>
		/// <param name="setQuery">The SELECT set query.</param>
		/// <param name="setOp">The SELECT set operation.</param>
		public void Add(SqlSelect setQuery, SqlSelectSetOp setOp)
		{
			if (this.setQuery == null)
			{
				if (setOp == SqlSelectSetOp.None)
				{
					throw new System.ArgumentException ("Invalid set operation");
				}

				this.setQuery = setQuery;
				this.setOp    = setOp;
			}
			else
			{
				this.setQuery.Add (setQuery, setOp);
			}
		}

		
		private readonly SqlFieldList			fields;
		private readonly SqlFieldList			tables;
		private readonly SqlFieldList			wheres;
		private readonly SqlFieldList			joins;
		private readonly SqlFieldList			orderBy;

		private SqlSelectPredicate				predicate;
		private SqlSelectSetOp					setOp;
		private SqlSelect						setQuery;

		private int?							skip;
		private int?							take;
	}
}
