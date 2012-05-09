using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>SortClause</c> class defines a part of an ordering for the result of requests.
	/// </summary>
	public sealed class SortClause
	{


		public SortClause(EntityField field, SortOrder sortOrder)
		{
			field.ThrowIfNull ("field");

			this.Field = field;
			this.SortOrder = sortOrder;
		}


		public EntityField Field
		{
			get;
			private set;
		}


		public SortOrder SortOrder
		{
			get;
			private set;
		}


		internal SqlField CreateSqlField(SqlFieldBuilder builder)
		{
			var sqlField = this.Field.CreateSqlField (builder);

			sqlField.SortOrder = EnumConverter.ToSqlSortOrder (this.SortOrder);

			return sqlField;
		}


		internal void CheckField(FieldChecker checker)
		{
			this.Field.CheckField (checker);
		}


	}


}
