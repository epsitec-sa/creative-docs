using Epsitec.Cresus.Core.Metadata;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Core.IO
{


	using Database = Core.Databases.Database;


	internal static class SorterIO
	{


		public static IEnumerable<ColumnRef<EntityColumnSort>> ParseSorters(Caches caches, Database database, string sortParameter)
		{
			if (string.IsNullOrEmpty (sortParameter))
			{
				yield break;
			}

			foreach (var sorter in sortParameter.Split (';'))
			{
				var data = sorter.Split (':');

				if (data.Length == 2)
				{
					var column = ColumnIO.ParseColumn (caches, database, data[0]);
					var columnId = column.MetaData.Id;

					var entityColumnSort = new EntityColumnSort ()
					{
						SortOrder = SorterIO.ParseColumnSortOrder (data[1]),
					};

					yield return new ColumnRef<EntityColumnSort> (columnId, entityColumnSort);
				}
			}
		}

		private static ColumnSortOrder ParseColumnSortOrder(string sortOrder)
		{
			switch (sortOrder)
			{
				case "ASC":
					return ColumnSortOrder.Ascending;

				case "DESC":
					return ColumnSortOrder.Descending;

				default:
					throw new NotImplementedException ();
			}
		}


	}


}
