using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.DataLayer
{

	public class DataQueryJoin
	{


		public DataQueryJoin(DataQueryColumn leftColumn, DataQueryColumn rightColumn)
			: this (leftColumn, rightColumn, SqlJoinCode.Inner)
		{

		}

		public DataQueryJoin(DataQueryColumn leftColumn, DataQueryColumn rightColumn, SqlJoinCode kind)
		{
			if (leftColumn.FieldPath.IsRelative || rightColumn.FieldPath.IsRelative)
			{
				throw new System.ArgumentException ("Relative path are not allowed.");
			}
			
			this.LeftColumn = leftColumn;
			this.RightColumn = rightColumn;

			this.Kind = kind;
		}

		public DataQueryColumn LeftColumn
		{
			get;
			private set;
		}

		public DataQueryColumn RightColumn
		{
			get;
			private set;
		}

		public SqlJoinCode Kind
		{
			get;
			private set;
		}

	}

}
