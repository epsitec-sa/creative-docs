using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.DataLayer.Expressions;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.Databases;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using Nancy.Json;

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
			Get["/definition/{name}"] = p => this.GetDatabase (p);
			Get["/get/{name}"] = p => this.Execute (b => this.GetEntities (b, p));
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


		private Response GetDatabase(dynamic parameters)
		{
			string databaseName = parameters.name;
			var database = this.CoreServer.DatabaseManager.GetDatabase (databaseName);

			var columns = database.Columns
				.Select (c => this.GetColumnData (c))
				.ToList ();

			var sorters = database.Sorters
				.Select (s => this.GetSorterData (s))
				.ToList ();

			var content = new Dictionary<string, object> ()
			{
				{ "columns", columns },
				{ "sorters", sorters },
			};

			return CoreResponse.Success (content);
		}


		private Dictionary<string, object> GetColumnData(Column column)
		{
			return new Dictionary<string, object> ()
			{
				{ "title", column.Title },
				{ "name", column.Name },
				{ "type", this.GetColumnTypeData(column) },
				{ "hidden", column.Hidden },
				{ "sortable", column.Sortable },
				{ "filter", this.GetFilterData (column) },
			};
		}


		private Dictionary<string, object> GetSorterData(Sorter sorter)
		{
			return new Dictionary<string, object> ()
			{
				{ "name", sorter.Column.Name },
				{ "sortDirection", this.GetSortOrderData (sorter.SortOrder) },
			};
		}


		private Dictionary<string, object> GetColumnTypeData(Column column)
		{
			var propertyAccessorType = this.GetPropertyAccessorType (column);

			var data = new Dictionary<string, object> ()
			{
				{ "type", this.GetPropertyAccessorTypeData (propertyAccessorType) },
			};

			if (propertyAccessorType == PropertyAccessorType.Enumeration)
			{
				data["enumerationName"] = Tools.TypeToString (column.LambdaExpression.ReturnType);
			}

			return data;
		}


		private PropertyAccessorType GetPropertyAccessorType(Column column)
		{
			var lambdaExpression = column.LambdaExpression;
			var propertyAccessor = this.CoreServer.PropertyAccessorCache.Get (lambdaExpression);

			return propertyAccessor.PropertyAccessorType;
		}


		private string GetPropertyAccessorTypeData(PropertyAccessorType type)
		{
			switch (type)
			{
				case PropertyAccessorType.Boolean:
					return "boolean";

				case PropertyAccessorType.Date:
					return "date";

				case PropertyAccessorType.Integer:
					return "int";

				case PropertyAccessorType.Enumeration:
					return "list";

				case PropertyAccessorType.Decimal:
					return "float";

				case PropertyAccessorType.Text:
					return "string";

				case PropertyAccessorType.EntityReference:
				case PropertyAccessorType.EntityCollection:
					throw new NotSupportedException ();

				default:
					throw new NotImplementedException ();
			}
		}


		private string GetSortOrderData(SortOrder? sortOrder)
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


		private Dictionary<string, object> GetFilterData(Column column)
		{
			var data = new Dictionary<string, object> ()
			{		
				{ "filterable", column.Filterable },
			};

			var propertyAccessorType = this.GetPropertyAccessorType (column);

			if (column.Filterable && propertyAccessorType == PropertyAccessorType.Enumeration)
			{
				data["enumerationName"] = Tools.TypeToString (column.LambdaExpression.ReturnType);
			}

			return data;
		}


		private Response GetEntities(BusinessContext businessContext, dynamic parameters)
		{
			string databaseName = parameters.name;
			var database = this.CoreServer.DatabaseManager.GetDatabase (databaseName);

			int start = Request.Query.start;
			int limit = Request.Query.limit;

			string sort = DatabasesModule.GetOptionalParameter (Request.Query.sort);
			var sorters = this.ParseSorters (database, sort);

			string filter = DatabasesModule.GetOptionalParameter (Request.Query.filter);
			var filters = this.ParseFilters (database, filter).ToList ();

			var propertyAccessorCache = this.CoreServer.PropertyAccessorCache;

			var total = database.GetCount (businessContext, filters);
			var entities = database.GetEntities (businessContext, sorters, filters, start, limit)
				.Select (e => database.GetEntityData (businessContext, e, propertyAccessorCache))
				.ToList ();

			var content = new Dictionary<string, object> ()
			{
				{ "total", total },
				{ "entities", entities },
			};

			return CoreResponse.Success (content);
		}


		private IEnumerable<Sorter> ParseSorters(Core.Databases.Database database, string sortParameter)
		{
			if (sortParameter != null)
			{
				foreach (var sorter in sortParameter.Split (";"))
				{
					var data = sorter.Split (":");

					if (data.Length == 2)
					{
						var name = data[0];
						var column = database.Columns.First (c => c.Name == name);
						var direction = this.ParseSortOrder (data[1]);

						yield return new Sorter (column, direction);
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


		private IEnumerable<Core.Databases.Filter> ParseFilters(Core.Databases.Database database, string filterParameter)
		{
			if (filterParameter != null)
			{
				var deserializer = new JavaScriptSerializer ();
				var filters = (object[]) deserializer.DeserializeObject (filterParameter);

				foreach (var filter in filters.Cast<Dictionary<string, object>> ())
				{
					yield return this.ParseFilter (database, filter);
				}
			}
		}


		private Core.Databases.Filter ParseFilter(Core.Databases.Database database, Dictionary<string, object> filter)
		{
			var column = this.ParseFilterColumn (database, filter);
			
			return this.ParseFilterCondition (column, filter);
		}

		private Column ParseFilterColumn(Core.Databases.Database database, Dictionary<string, object> filter)
		{
			var field = (string) filter["field"];

			return database.Columns.First (c => c.Name == field);
		}


		private Core.Databases.Filter ParseFilterCondition(Column column, Dictionary<string, object> filter)
		{
			var lambda = column.LambdaExpression;
			var propertyAccessorCache = this.CoreServer.PropertyAccessorCache;
			var propertyAccessor = (AbstractStringPropertyAccessor) propertyAccessorCache.Get (lambda);

			var type = (string) filter["type"];
			var value = filter["value"];

			if (type == "list")
			{
				return this.ParseSetFilter (column, propertyAccessor, value);
			}
			else
			{
				object comparison;
				if (!filter.TryGetValue ("comparison", out comparison))
				{
					comparison = "eq";
				}

				return this.ParseComparisonFilter (column, propertyAccessor, type, comparison, value);
			}
		}


		private SetFilter ParseSetFilter(Column column, AbstractStringPropertyAccessor propertyAccessor, object value)
		{
			var valueArray = (object[]) value;
			var convertedValues = valueArray.Select (v => propertyAccessor.ConvertValue (v));

			return new SetFilter (column, SetComparator.In, convertedValues);
		}


		private ComparisonFilter ParseComparisonFilter(Column column, AbstractStringPropertyAccessor propertyAccessor, string type, object comparison, object value)
		{
			var comparator = this.ParseComparator (comparison);

			switch (type)
			{
				case "numeric":
					switch (propertyAccessor.PropertyAccessorType)
					{
						case PropertyAccessorType.Integer:
							value = Convert.ToInt64 (value);
							break;

						case PropertyAccessorType.Decimal:
							value = Convert.ToDecimal (value);
							break;
					}
					break;

				case "string":
					comparator = BinaryComparator.IsLikeEscape;
					value =  "%" + Constant.Escape ((string) value) + "%";
					break;
			}

			var convertedValue = propertyAccessor.ConvertValue (value);

			return new ComparisonFilter (column, comparator, convertedValue);
		}


		private BinaryComparator ParseComparator(object comparator)
		{
			switch ((string) comparator)
			{
				case "eq":
					return BinaryComparator.IsEqual;
				case "nq":
					return BinaryComparator.IsNotEqual;
				case "gt":
					return BinaryComparator.IsGreater;
				case "lt":
					return BinaryComparator.IsLower;
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
