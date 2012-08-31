using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Metadata;

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
			
			//	TODO: resolve EntityColumnMetadata through DataStoreMetdata
			
			var entityColumnMetaData = new EntityColumnMetadata (lambda, name);
			var entityColumnSort = new EntityColumnSort ()
			{
				SortOrder = EntityColumnSort.Convert (this.sortOrder)
			};

			return entityColumnSort.ToSortClause (entityColumnMetaData, example);
		}


		private readonly Column column;


		private readonly SortOrder sortOrder;


	}


}
