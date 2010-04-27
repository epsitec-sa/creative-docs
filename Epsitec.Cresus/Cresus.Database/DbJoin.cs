namespace Epsitec.Cresus.Database
{
	
	public class DbJoin
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

	}

}
