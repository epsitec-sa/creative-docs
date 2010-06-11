using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Database
{
	

	public class DbJoin : AbstractConditionContainer
	{


		public DbJoin(DbTableColumn leftColumn, DbTableColumn rightColumn, SqlJoinCode type)
		{
			this.LeftColumn = leftColumn;
			this.RightColumn = rightColumn;

			this.Type = type;
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

		public IEnumerable<DbTableColumn> Columns
		{
			get
			{
				return new DbTableColumn[] { this.LeftColumn, this.RightColumn }.Concat (base.Columns);
			}
		}


		public void ReplaceTableColumns(System.Func<DbTableColumn, DbTableColumn> replaceOperation)
		{
			base.ReplaceTableColumns (replaceOperation);

			this.LeftColumn = replaceOperation (this.LeftColumn);
			this.RightColumn = replaceOperation (this.RightColumn);
		}


		public SqlJoin CreateSqlJoin()
		{
			SqlField leftField = SqlField.CreateAliasedName (this.LeftColumn.TableAlias, this.LeftColumn.Column.GetSqlName (), this.LeftColumn.ColumnAlias);
			SqlField rightField = SqlField.CreateAliasedName (this.RightColumn.TableAlias, this.RightColumn.Column.GetSqlName (), this.RightColumn.ColumnAlias);

			SqlJoin join = new SqlJoin (leftField, rightField, this.Type);

			if (!this.IsEmpty)
			{
				join.Conditions.Add (this.CreateSqlField ());
			}

			return join;
		}


	}


}
