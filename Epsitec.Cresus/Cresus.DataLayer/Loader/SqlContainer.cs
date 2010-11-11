using Epsitec.Cresus.Database;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Loader
{
	
	
	// TODO Comment this class.
	// Marc


	internal sealed class SqlContainer
	{


		public SqlContainer(IEnumerable<SqlField> sqlTables, IEnumerable<SqlField> sqlFields, IEnumerable<SqlJoin> sqlJoins, IEnumerable<SqlFunction> sqlConditions)
		{
			this.SqlTables = sqlTables.ToList ();
			this.SqlFields = sqlFields.ToList ();
			this.SqlJoins = sqlJoins.ToList ();
			this.SqlConditions = sqlConditions.ToList ();
		}


		public IEnumerable<SqlField> SqlTables
		{
			get;
			private set;
		}


		public IEnumerable<SqlField> SqlFields
		{
			get;
			private set;
		}


		public IEnumerable<SqlJoin> SqlJoins
		{
			get;
			private set;
		}


		public IEnumerable<SqlFunction> SqlConditions
		{
			get;
			private set;
		}


		public SqlSelect BuildSqlSelect()
		{
			SqlSelect sqlSelect = new SqlSelect ();

			sqlSelect.Tables.AddRange (this.SqlTables);
			sqlSelect.Fields.AddRange (this.SqlFields);
			sqlSelect.Joins.AddRange (this.SqlJoins.Select (j => SqlField.CreateJoin (j)));
			sqlSelect.Conditions.AddRange (this.SqlConditions.Select (c => SqlField.CreateFunction (c)));

			return sqlSelect;
		}


		public SqlContainer Plus(SqlContainer that)
		{
			var newSqlTables = this.SqlTables.Concat (that.SqlTables);
			var newSqlFields = this.SqlFields.Concat (that.SqlFields);
			var newSqlJoins = this.SqlJoins.Concat (that.SqlJoins);
			var newSqlConditions = this.SqlConditions.Concat (that.SqlConditions);

			return new SqlContainer (newSqlTables, newSqlFields, newSqlJoins, newSqlConditions);
		}


		public SqlContainer PlusSqlTables(params SqlField[] sqlTables)
		{
			var newSqlTables = this.SqlTables.Concat (sqlTables);
			var newSqlFields = this.SqlFields;
			var newSqlJoins = this.SqlJoins;
			var newSqlConditions = this.SqlConditions;

			return new SqlContainer (newSqlTables, newSqlFields, newSqlJoins, newSqlConditions);
		}


		public SqlContainer PlusSqlFields(params SqlField[] sqlFields)
		{
			var newSqlTables = this.SqlTables;
			var newSqlFields = this.SqlFields.Concat (sqlFields);
			var newSqlJoins = this.SqlJoins;
			var newSqlConditions = this.SqlConditions;

			return new SqlContainer (newSqlTables, newSqlFields, newSqlJoins, newSqlConditions);
		}


		public SqlContainer PlusSqlJoins(params SqlJoin[] sqlJoins)
		{
			var newSqlTables = this.SqlTables;
			var newSqlFields = this.SqlFields;
			var newSqlJoins = this.SqlJoins.Concat (sqlJoins);
			var newSqlConditions = this.SqlConditions;

			return new SqlContainer (newSqlTables, newSqlFields, newSqlJoins, newSqlConditions);
		}


		public SqlContainer PlusSqlConditions(params SqlFunction[] sqlConditions)
		{
			var newSqlTables = this.SqlTables;
			var newSqlFields = this.SqlFields;
			var newSqlJoins = this.SqlJoins;
			var newSqlConditions = this.SqlConditions.Concat (sqlConditions);

			return new SqlContainer (newSqlTables, newSqlFields, newSqlJoins, newSqlConditions);
		}


		public static SqlContainer CreateEmpty()
		{
			var newSqlTables = new List<SqlField> ();
			var newSqlFields = new List<SqlField> ();
			var newSqlJoins = new List<SqlJoin> ();
			var newSqlConditions = new List<SqlFunction> ();

			return new SqlContainer (newSqlTables, newSqlFields, newSqlJoins, newSqlConditions);
		}


		public static SqlContainer CreateSqlTables(params SqlField[] sqlTables)
		{
			return SqlContainer.Empty.PlusSqlTables (sqlTables);
		}


		public static SqlContainer CreateSqlFields(params SqlField[] sqlFields)
		{
			return SqlContainer.Empty.PlusSqlFields (sqlFields);
		}


		public static SqlContainer CreateSqlJoins(params SqlJoin[] sqlJoins)
		{
			return SqlContainer.Empty.PlusSqlJoins (sqlJoins);
		}


		public static SqlContainer CreateSqlConditions(params SqlFunction[] sqlConditions)
		{
			return SqlContainer.Empty.PlusSqlConditions (sqlConditions);
		}


		public static SqlContainer Empty
		{
			get
			{
				return SqlContainer.empty;
			}
		}


		private static SqlContainer empty = SqlContainer.CreateEmpty ();


	}


}
