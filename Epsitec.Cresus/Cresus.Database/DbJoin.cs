using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Database
{
	

	public class DbJoin
	{


		public DbJoin(DbTableColumn leftColumn, DbTableColumn rightColumn, SqlJoinCode type)
		{
			this.LeftColumn = leftColumn;
			this.RightColumn = rightColumn;

			this.Type = type;

			this.Conditions = new DbConditionContainer ();
		}


		public DbTableColumn LeftColumn
		{
			get;
			private set;
		}


		public DbTableColumn RightColumn
		{
			get;
			private set;
		}


		public SqlJoinCode Type
		{
			get;
			private set;
		}


		public DbConditionContainer Conditions
		{
			get;
			set;
		}


		internal IEnumerable<DbTableColumn> Columns
		{
			get
			{
				return new DbTableColumn[] { this.LeftColumn, this.RightColumn }.Concat (this.Conditions.Columns);
			}
		}


		internal void ReplaceTableColumns(System.Func<DbTableColumn, DbTableColumn> replaceOperation)
		{
			this.Conditions.ReplaceTableColumns (replaceOperation);

			this.LeftColumn = replaceOperation (this.LeftColumn);
			this.RightColumn = replaceOperation (this.RightColumn);
		}


		internal SqlJoin CreateSqlJoin()
		{
			SqlField leftField = SqlField.CreateAliasedName (this.LeftColumn.TableAlias, this.LeftColumn.Column.GetSqlName (), this.LeftColumn.ColumnAlias);
			SqlField rightField = SqlField.CreateAliasedName (this.RightColumn.TableAlias, this.RightColumn.Column.GetSqlName (), this.RightColumn.ColumnAlias);

			SqlJoin join = new SqlJoin (leftField, rightField, this.Type);

			if (!this.Conditions.IsEmpty)
			{
				join.Conditions.Add (this.Conditions.CreateSqlField ());
			}

			return join;
		}


	}


}
