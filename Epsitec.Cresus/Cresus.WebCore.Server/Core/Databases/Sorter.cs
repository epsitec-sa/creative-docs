using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Data.Metadata;

using Epsitec.Cresus.DataLayer.Expressions;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	internal sealed class Sorter
	{


		public Sorter(Column column, SortOrder sortOrder)
		{
			this.column = column;
			this.sortOrder = sortOrder;
		}


		public Column Column
		{
			get
			{
				return this.column;
			}
		}


		public SortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}


		public SortClause ToSortClause(AbstractEntity example)
		{
			var lambda = column.LambdaExpression;
			var name = column.Name;
			var entityDataColumn = new EntityColumnMetadata (lambda, name, this.sortOrder);

			return entityDataColumn.ToSortClause (example);
		}


		private readonly Column column;


		private readonly SortOrder sortOrder;


	}


}
