using Epsitec.Cresus.WebCore.Server.Core.Databases;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.IO
{


	using Database = Core.Databases.Database;


	internal static class ColumnIO
	{


		public static IEnumerable<Column> ParseColumns(Caches caches, Database database, string rawColums)
		{
			if (string.IsNullOrEmpty (rawColums))
			{
				return Enumerable.Empty<Column> ();
			}

			return rawColums
				.Split (';')
				.Select (id => ColumnIO.ParseColumn (caches, database, id));
		}


		public static Column ParseColumn(Caches caches, Database database, string id)
		{
			var name = caches.ColumnIdCache.GetItem (id);

			return database.Columns.First (c => c.Name == name);
		}


	}


}
