//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
		/// Gets the fields for the columns.
		/// </summary>
		/// <value>The fields.</value>
		public Collections.SqlFields			Fields
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
		public Collections.SqlFields			Tables
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
		public Collections.SqlFields			Conditions
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
		public Collections.SqlFields			Joins
		{
			get
			{
				return this.joins;
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

		private Collections.SqlFields			fields	= new Collections.SqlFields ();
		private Collections.SqlFields			tables	= new Collections.SqlFields ();
		private Collections.SqlFields			wheres	= new Collections.SqlFields ();
		private Collections.SqlFields			joins	= new Collections.SqlFields ();

		private SqlSelectPredicate				predicate;
		private SqlSelectSetOp					setOp;
		private SqlSelect						setQuery;
	}
}
