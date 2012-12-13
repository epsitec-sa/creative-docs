using Epsitec.Cresus.Core.Metadata;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.IO
{


	internal static class SorterIO
	{


		public static IEnumerable<ColumnRef<EntityColumnSort>> ParseSorters(Caches caches, Core.Databases.Database database, string sortParameter)
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
					var name = caches.ColumnIdCache.GetItem (data[0]);
					var column = database.Columns.First (c => c.Name == name);
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
