using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Loader
{


	/// <summary>
	/// The <see cref="SqlContainer"/> is used to contain several SQL objects used to build SQL
	/// queries. It is an immutable object and two of them can be used to be combined together, kind
	/// of like you would add two 5-dimension vector together.
	/// </summary>
	internal sealed class SqlContainer
	{


		/// <summary>
		/// Builds a new <see cref="SqlContainer"/> with some given elements.
		/// </summary>
		/// <param name="sqlTables">The collection of <see cref="SqlField"/> that represent the tables of the query.</param>
		/// <param name="sqlFields">The collection of <see cref="SqlField"/> that represent the return fields of the query.</param>
		/// <param name="sqlJoins">The collection of <see cref="SqlJoin"/> that represent the joins of the query.</param>
		/// <param name="sqlConditions">The collection of <see cref="SqlFunction"/> that represent the conditions of the query.</param>
		/// <param name="sqlOrderBys">The collection of <see cref="SqlField"/> that represent the order by clause of the query.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sqlTables"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sqlFields"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sqlJoins"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sqlConditions"/> is <c>null</c>.</exception>
		public SqlContainer(IEnumerable<SqlField> sqlTables, IEnumerable<SqlField> sqlFields, IEnumerable<SqlJoin> sqlJoins, IEnumerable<SqlFunction> sqlConditions, IEnumerable<SqlField> sqlOrderBys)
		{
			sqlTables.ThrowIfNull ("sqlTables");
			sqlFields.ThrowIfNull ("sqlFields");
			sqlJoins.ThrowIfNull ("sqlJoins");
			sqlConditions.ThrowIfNull ("sqlConditions");
			sqlOrderBys.ThrowIfNull ("sqlOrderBys");

			this.SqlTables = sqlTables.ToList ();
			this.SqlFields = sqlFields.ToList ();
			this.SqlJoins = sqlJoins.ToList ();
			this.SqlConditions = sqlConditions.ToList ();
			this.SqlOrderBys = sqlOrderBys.ToList ();
		}


		/// <summary>
		/// Gets the collection of <see cref="SqlField"/> that represent the tables of the query.
		/// </summary>
		public IEnumerable<SqlField> SqlTables
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the collection of <see cref="SqlField"/> that represent the query fields of the
		/// query.
		/// </summary>
		public IEnumerable<SqlField> SqlFields
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the collection of <see cref="SqlJoin"/> that represent the joins of the query.
		/// </summary>
		public IEnumerable<SqlJoin> SqlJoins
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the collection of <see cref="SqlFunction"/> that represent the conditions of the
		/// query.
		/// </summary>
		public IEnumerable<SqlFunction> SqlConditions
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the collection of <see cref="SqlField"/> that represent the order by clause of the
		/// query.
		/// </summary>
		public IEnumerable<SqlField> SqlOrderBys
		{
			get;
			private set;
		}


		/// <summary>
		/// Builds the <see cref="SqlSelect"/> that is the query that corresponds to this instance.
		/// </summary>
		/// <returns>The <see cref="SqlSelect"/>.</returns>
		public SqlSelect BuildSqlSelect()
		{
			SqlSelect sqlSelect = new SqlSelect ();

			sqlSelect.Tables.AddRange (this.SqlTables);
			sqlSelect.Fields.AddRange (this.SqlFields);
			sqlSelect.Joins.AddRange (this.SqlJoins.Select (j => SqlField.CreateJoin (j)));
			sqlSelect.Conditions.AddRange (this.SqlConditions.Select (c => SqlField.CreateFunction (c)));
			sqlSelect.OrderBy.AddRange (this.SqlOrderBys);

			return sqlSelect;
		}


		/// <summary>
		/// Adds the elements of another <see cref="SqlContainer"/> to the ones of this instance and
		/// returns the result as a new <see cref="SqlContainer"/>.
		/// </summary>
		/// <param name="that">The other <see cref="SqlContainer"/>.</param>
		/// <returns>The resulting <see cref="SqlContainer"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="that"/> is <c>null</c>.</exception>
		public SqlContainer Plus(SqlContainer that)
		{
			that.ThrowIfNull ("that");

			var newSqlTables = this.SqlTables.Concat (that.SqlTables);
			var newSqlFields = this.SqlFields.Concat (that.SqlFields);
			var newSqlJoins = this.SqlJoins.Concat (that.SqlJoins);
			var newSqlConditions = this.SqlConditions.Concat (that.SqlConditions);
			var newSqlOrderBys = this.SqlOrderBys.Concat (that.SqlOrderBys);

			return new SqlContainer (newSqlTables, newSqlFields, newSqlJoins, newSqlConditions, newSqlOrderBys);
		}


		/// <summary>
		/// Builds a new <see cref="SqlContainer"/> whose elements are the one of this instance plus
		/// the given collection <see cref="SqlField"/> for the tables.
		/// </summary>
		/// <param name="sqlTables">The tables to add.</param>
		/// <returns>The resulting <see cref="SqlContainer"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sqlTables"/> is <c>null</c>.</exception>
		public SqlContainer PlusSqlTables(params SqlField[] sqlTables)
		{
			sqlTables.ThrowIfNull ("sqlTables");

			var newSqlTables = this.SqlTables.Concat (sqlTables);
			var newSqlFields = this.SqlFields;
			var newSqlJoins = this.SqlJoins;
			var newSqlConditions = this.SqlConditions;
			var newSqlOrderBys = this.SqlOrderBys;

			return new SqlContainer (newSqlTables, newSqlFields, newSqlJoins, newSqlConditions, newSqlOrderBys);
		}


		/// <summary>
		/// Builds a new <see cref="SqlContainer"/> whose elements are the one of this instance plus
		/// the given collection <see cref="SqlField"/> for the query fields.
		/// </summary>
		/// <param name="sqlFields">The query fields to add.</param>
		/// <returns>The resulting <see cref="SqlContainer"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sqlFields"/> is <c>null</c>.</exception>
		public SqlContainer PlusSqlFields(params SqlField[] sqlFields)
		{
			sqlFields.ThrowIfNull ("sqlFields");

			var newSqlTables = this.SqlTables;
			var newSqlFields = this.SqlFields.Concat (sqlFields);
			var newSqlJoins = this.SqlJoins;
			var newSqlConditions = this.SqlConditions;
			var newSqlOrderBys = this.SqlOrderBys;

			return new SqlContainer (newSqlTables, newSqlFields, newSqlJoins, newSqlConditions, newSqlOrderBys);
		}


		/// <summary>
		/// Builds a new <see cref="SqlContainer"/> whose elements are the one of this instance plus
		/// the given collection <see cref="SqlJoin"/> for the joins.
		/// </summary>
		/// <param name="sqlJoins">The joins to add.</param>
		/// <returns>The resulting <see cref="SqlContainer"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sqlJoins"/> is <c>null</c>.</exception>
		public SqlContainer PlusSqlJoins(params SqlJoin[] sqlJoins)
		{
			sqlJoins.ThrowIfNull ("sqlJoins");

			var newSqlTables = this.SqlTables;
			var newSqlFields = this.SqlFields;
			var newSqlJoins = this.SqlJoins.Concat (sqlJoins);
			var newSqlConditions = this.SqlConditions;
			var newSqlOrderBys = this.SqlOrderBys;

			return new SqlContainer (newSqlTables, newSqlFields, newSqlJoins, newSqlConditions, newSqlOrderBys);
		}


		/// <summary>
		/// Builds a new <see cref="SqlContainer"/> whose elements are the one of this instance plus
		/// the given collection <see cref="SqlFunction"/> for the conditions.
		/// </summary>
		/// <param name="sqlConditions">The conditions to add.</param>
		/// <returns>The resulting <see cref="SqlContainer"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sqlConditions"/> is <c>null</c>.</exception>
		public SqlContainer PlusSqlConditions(params SqlFunction[] sqlConditions)
		{
			sqlConditions.ThrowIfNull ("sqlConditions");

			var newSqlTables = this.SqlTables;
			var newSqlFields = this.SqlFields;
			var newSqlJoins = this.SqlJoins;
			var newSqlConditions = this.SqlConditions.Concat (sqlConditions);
			var newSqlOrderBys = this.SqlOrderBys;

			return new SqlContainer (newSqlTables, newSqlFields, newSqlJoins, newSqlConditions, newSqlOrderBys);
		}


		/// <summary>
		/// Builds a new <see cref="SqlContainer"/> whose elements are the one of this instance plus
		/// the given collection <see cref="SqlField"/> for the order by clause.
		/// </summary>
		/// <param name="sqlOrderBys">The fields to add in the order by clause.</param>
		/// <returns>The resulting <see cref="SqlContainer"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sqlOrderBys"/> is <c>null</c>.</exception>
		public SqlContainer PlusSqlOrderBys(params SqlField[] sqlOrderBys)
		{
			sqlOrderBys.ThrowIfNull ("sqlOrderBys");

			var newSqlTables = this.SqlTables;
			var newSqlFields = this.SqlFields;
			var newSqlJoins = this.SqlJoins;
			var newSqlConditions = this.SqlConditions;
			var newSqlOrderBys = this.SqlOrderBys.Concat (sqlOrderBys);

			return new SqlContainer (newSqlTables, newSqlFields, newSqlJoins, newSqlConditions, newSqlOrderBys);
		}


		/// <summary>
		/// Builds a new empty <see cref="SqlContainer"/>.
		/// </summary>
		/// <returns>The <see cref="SqlContainer"/>.</returns>
		public static SqlContainer CreateEmpty()
		{
			var newSqlTables = new List<SqlField> ();
			var newSqlFields = new List<SqlField> ();
			var newSqlJoins = new List<SqlJoin> ();
			var newSqlConditions = new List<SqlFunction> ();
			var newSqlOrderBys = new List<SqlField> ();

			return new SqlContainer (newSqlTables, newSqlFields, newSqlJoins, newSqlConditions, newSqlOrderBys);
		}


		/// <summary>
		/// Builds a new <see cref="SqlContainer"/> which contains only tables.
		/// </summary>
		/// <param name="sqlTables">The collection of <see cref="SqlField"/> that are the tables.</param>
		/// <returns>The new <see cref="SqlContainer"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sqlTables"/> is <c>null</c>.</exception>
		public static SqlContainer CreateSqlTables(params SqlField[] sqlTables)
		{
			sqlTables.ThrowIfNull ("sqlTables");

			return SqlContainer.Empty.PlusSqlTables (sqlTables);
		}


		/// <summary>
		/// Builds a new <see cref="SqlContainer"/> which contains only query fields.
		/// </summary>
		/// <param name="sqlFields">The collection of <see cref="SqlField"/> that are the query fields.</param>
		/// <returns>The new <see cref="SqlContainer"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sqlFields"/> is <c>null</c>.</exception>
		public static SqlContainer CreateSqlFields(params SqlField[] sqlFields)
		{
			sqlFields.ThrowIfNull ("sqlFields");

			return SqlContainer.Empty.PlusSqlFields (sqlFields);
		}


		/// <summary>
		/// Builds a new <see cref="SqlContainer"/> which contains only joins.
		/// </summary>
		/// <param name="sqlJoins">The collection of <see cref="SqlJoin"/> that are the joins.</param>
		/// <returns>The new <see cref="SqlContainer"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sqlJoins"/> is <c>null</c>.</exception>
		public static SqlContainer CreateSqlJoins(params SqlJoin[] sqlJoins)
		{
			sqlJoins.ThrowIfNull ("sqlJoins");

			return SqlContainer.Empty.PlusSqlJoins (sqlJoins);
		}


		/// <summary>
		/// Builds a new <see cref="SqlContainer"/> which contains only conditions.
		/// </summary>
		/// <param name="sqlConditions">The collection of <see cref="SqlFunction"/> that are the conditions.</param>
		/// <returns>The new <see cref="SqlContainer"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sqlConditions"/> is <c>null</c>.</exception>
		public static SqlContainer CreateSqlConditions(params SqlFunction[] sqlConditions)
		{
			sqlConditions.ThrowIfNull ("sqlConditions");

			return SqlContainer.Empty.PlusSqlConditions (sqlConditions);
		}


		/// <summary>
		/// Builds a new <see cref="SqlContainer"/> which contains only field in the order by clause.
		/// </summary>
		/// <param name="sqlOrderBys">The collection of <see cref="SqlField"/> that are in the order by clause.</param>
		/// <returns>The new <see cref="SqlContainer"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sqlOrderBys"/> is <c>null</c>.</exception>
		public static SqlContainer CreateSqlOrderBys(params SqlField[] sqlOrderBys)
		{
			sqlOrderBys.ThrowIfNull ("sqlOrderBys)");

			return SqlContainer.Empty.PlusSqlOrderBys (sqlOrderBys);
		}


		/// <summary>
		/// Gets the empty <see cref="SqlContainer"/>.
		/// </summary>
		public static SqlContainer Empty
		{
			get
			{
				return SqlContainer.empty;
			}
		}


		/// <summary>
		/// An instance of the empty <see cref="SqlContainer"/>.
		/// </summary>
		private static SqlContainer empty = SqlContainer.CreateEmpty ();


	}


}
