//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

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

			this.field = field;
			this.sortOrder = sortOrder;
		}


		public EntityField Field
		{
			get
			{
				return this.field;
			}
		}


		public SortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
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


		private readonly EntityField field;


		private readonly SortOrder sortOrder;


	}


}
