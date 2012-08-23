using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Expressions;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.Databases;

using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// Used to retrieve data about the databases, such as the list of defined databases, the number
	/// of entities within a database, a subset of a database or to create or delete an entity
	/// within a database.
	/// </summary>
	public class DatabasesModule : AbstractBusinessContextModule
	{


		public DatabasesModule(CoreServer coreServer)
			: base (coreServer, "/database")
		{
			Get["/list"] = p => this.GetDatabaseList ();
			Get["/columns/{name}"] = p => this.GetColumnList(p);
			Get["/get/{name}"] = p => this.Execute (b => this.GetDatabase (b, p));
			Post["/delete"] = p => this.Execute (b => this.DeleteEntities (b));
			Post["/create/"] = p => this.Execute (b => this.CreateEntity (b));
		}


		private Response GetDatabaseList()
		{
			var databases = this.CoreServer.DatabaseManager.GetDatabases ()
				.Select (d => this.GetDatabaseData (d))
				.ToList ();

			var content = new Dictionary<string, object> ()
			{
				{ "databases", databases },
			};

			return CoreResponse.Success (content);
		}


		private Dictionary<string, object> GetDatabaseData(Core.Databases.Database database)
		{
			return new Dictionary<string, object> ()
			{
			    { "title", database.Title },
			    { "name", database.Name },
			    { "cssClass", database.IconClass },
			};
		}


		private Response GetColumnList(dynamic parameters)
		{
			string databaseName = parameters.name;
			var database = this.CoreServer.DatabaseManager.GetDatabase (databaseName);

			var columns = database.Columns
				.Select (c => this.GetColumnData (c))
				.ToList ();

			var content = new Dictionary<string, object> ()
			{
				{ "columns", columns },
			};

			return CoreResponse.Success (content);
		}


		private Dictionary<string, object> GetColumnData(Column column)
		{
			return new Dictionary<string, object> ()
			{
				{ "title", column.Title },
				{ "name", column.Name },
				{ "type", this.GetColumnTypeData(column.Type) },
				{ "hidden", column.Hidden },
				{ "sortable", column.Sortable },
				{ "sortDirection", this.GetSortOrder (column.SortOrder) } 
			};
		}


		private string GetColumnTypeData(ColumnType type)
		{
			switch (type)
			{
				case ColumnType.Boolean:
					return "boolean";

				case ColumnType.Date:
					return "date";

				case ColumnType.Integer:
					return "int";

				case ColumnType.Number:
					return "float";

				case ColumnType.String:
					return "string";

				default:
					throw new NotImplementedException ();
			}
		}


		private string GetSortOrder(SortOrder? sortOrder)
		{
			switch (sortOrder)
			{
				case null:
					return null;

				case SortOrder.Ascending:
					return "ASC";

				case SortOrder.Descending:
					return "DESC";

				default:
					throw new NotImplementedException ();
			}
		}


		private Response GetDatabase(BusinessContext businessContext, dynamic parameters)
		{
			string databaseName = parameters.name;
			var database = this.CoreServer.DatabaseManager.GetDatabase (databaseName);

			int start = Request.Query.start;
			int limit = Request.Query.limit;

			string sort = DatabasesModule.GetOptionalParameter (Request.Query.sort);
			var sortCriteria = this.ParseSortCriteria (sort);

			var propertyAccessorCache = this.CoreServer.PropertyAccessorCache;

			var total = database.GetCount (businessContext);
			var entities = database.GetEntities (businessContext, sortCriteria, start, limit)
				.Select (e => database.GetEntityData (businessContext, e, propertyAccessorCache))
				.ToList ();

			var content = new Dictionary<string, object> ()
			{
				{ "total", total },
				{ "entities", entities },
			};

			return CoreResponse.Success (content);
		}


		private IEnumerable<Tuple<string, SortOrder>> ParseSortCriteria(string sortParameter)
		{
			if (sortParameter != null)
			{
				foreach (var sortCriterium in sortParameter.Split (";"))
				{
					var data = sortCriterium.Split (":");

					if (data.Length == 2)
					{
						var name = data[0];
						var direction = this.ParseSortOrder (data[1]);

						yield return Tuple.Create (name, direction);
					}
				}
			}
		}


		private SortOrder ParseSortOrder(string sortOrder)
		{
			switch (sortOrder)
			{
				case "ASC":
					return SortOrder.Ascending;

				case "DESC":
					return SortOrder.Descending;

				default:
					throw new NotImplementedException ();
			}
		}


		private Response DeleteEntities(BusinessContext businessContext)
		{
			string databaseName = Request.Form.databaseName;
			var database = this.CoreServer.DatabaseManager.GetDatabase (databaseName);

			string rawEntityIds = Request.Form.entityIds;
			var entityIds = rawEntityIds.Split (";");

			var success = true;

			foreach (var entityId in entityIds)
			{
				var entity = Tools.ResolveEntity (businessContext, entityId);

				success = database.DeleteEntity (businessContext, entity);

				if (!success)
				{
					break;
				}
			}

			if (success)
			{
				businessContext.SaveChanges ();
			}

			return success
				? CoreResponse.Success ()
				: CoreResponse.Failure ();
		}


		private Response CreateEntity(BusinessContext businessContext)
		{
			string databaseName = Request.Form.databaseName;
			var database = this.CoreServer.DatabaseManager.GetDatabase (databaseName);

			var propertyAccessorCache = this.CoreServer.PropertyAccessorCache;

			var entity = database.CreateEntity (businessContext);
			var entityData = database.GetEntityData (businessContext, entity, propertyAccessorCache);

			return CoreResponse.Success (entityData);
		}


		private static string GetOptionalParameter(dynamic parameter)
		{
			return parameter.HasValue
				? parameter.Value
				: null;
		}


	}


}
