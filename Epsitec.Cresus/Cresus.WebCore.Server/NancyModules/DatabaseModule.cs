using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.Databases;

using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Collections;


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


		private Response GetDatabase(BusinessContext businessContext, dynamic parameters)
		{
			string databaseName = parameters.name;
			var database = this.CoreServer.DatabaseManager.GetDatabase (databaseName);

			int start = Request.Query.start;
			int limit = Request.Query.limit;

			var total = database.GetCount (businessContext);
			var entities = database.GetEntities (businessContext, start, limit)
				.Select (e => database.GetEntityData (businessContext, e))
				.ToList ();

			var content = new Dictionary<string, object> ()
			{
				{ "total", total },
				{ "entities", entities },
			};

			return CoreResponse.Success (content);
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
			// TODO This implementation is very simple and will only work if the entity that will be
			// created is not of an abstract type. If it is an abstract entity, probable that an
			// entity of the wrong type will be created. Probably that we should implement something
			// with the CreationControllers.

			string databaseName = Request.Form.databaseName;
			var database = this.CoreServer.DatabaseManager.GetDatabase (databaseName);

			var entity = database.CreateEntity (businessContext);
			var entityData = database.GetEntityData (businessContext, entity);

			return CoreResponse.Success (entityData);
		}


	}


}
