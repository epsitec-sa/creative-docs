namespace Epsitec.Cresus.DataLayer.Expressions
{


	public sealed class SortClause
	{


		public SortClause(Field field, SortOrder sortOrder)
		{
			this.Field = field;
			this.SortOrder = sortOrder;
		}


		public Field Field
		{
			get;
			private set;
		}


		public SortOrder SortOrder
		{
			get;
			private set;
		}


	}


}
