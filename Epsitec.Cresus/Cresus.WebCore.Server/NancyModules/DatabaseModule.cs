using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

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
	public class DatabaseModule : AbstractAuthenticatedModule
	{


		public DatabaseModule(CoreServer coreServer)
			: base (coreServer, "/database")
		{
			Get["/list"] = p => this.Execute (wa => this.GetDatabaseList (wa));
			Get["/definition/{name}"] = p => this.GetDatabase (p);
			Get["/get/{name}"] = p => this.Execute (wa => this.GetEntities (wa, p));
			Post["/delete"] = p => this.Execute (b => this.DeleteEntities (b));
			Post["/create/"] = p => this.Execute (b => this.CreateEntity (b));
		}


		private Response GetDatabaseList(WorkerApp workerApp)
		{
			var databases = this.CoreServer.DatabaseManager
				.GetDatabases (workerApp.UserManager)
				.Select (d => d.GetDataDictionary ())
				.ToList ();

			var content = new Dictionary<string, object> ()
			{
				{ "menu", databases },
			};

			return CoreResponse.Success (content);
		}


		private Response GetDatabase(dynamic parameters)
		{
			string databaseName = parameters.name;
			var database = this.CoreServer.DatabaseManager.GetDatabase (databaseName);

			var content = database.GetDataDictionary (this.CoreServer.Caches);

			return CoreResponse.Success (content);
		}


		private Response GetEntities(WorkerApp workerApp, dynamic parameters)
		{
			string databaseName = parameters.name;
			var database = this.CoreServer.DatabaseManager.GetDatabase (databaseName);

			int start = Request.Query.start;
			int limit = Request.Query.limit;

			var entityType = database.DataSetMetadata.DataSetEntityType;

			var session = workerApp.UserManager.ActiveSession;
			var settings = session.GetTableSettings (entityType);

			string sort = DatabaseModule.GetOptionalParameter (Request.Query.sort);
			var sorters = this.ParseSorters (database, sort);

			settings.Sort.Clear();
			settings.Sort.AddRange (sorters);

			string filter = DatabaseModule.GetOptionalParameter (Request.Query.filter);
			var entityFilter = this.ParseEntityFilter (database, filter);

			settings.Filter = entityFilter;

			session.SetTableSettings (entityType, settings);

			var dataSetGetter = workerApp.DataSetGetter;

			using (var dataSet = database.GetDataSetAccessor (dataSetGetter))
			{
				var dataContext = dataSet.IsolatedDataContext;

				var total = dataSet.GetItemCount ();
				var entities = dataSet.GetItems (start, limit)
					.Select (e => database.GetEntityData (dataContext, this.CoreServer.Caches, e))
					.ToList ();

				var content = new Dictionary<string, object> ()
				{
					{ "total", total },
					{ "entities", entities },
				};

				return CoreResponse.Success (content);
			}
		}


		private IEnumerable<ColumnRef<EntityColumnSort>> ParseSorters(Core.Databases.Database database, string sortParameter)
		{
			if (sortParameter != null)
			{
				foreach (var sorter in sortParameter.Split (";"))
				{
					var data = sorter.Split (":");

					if (data.Length == 2)
					{
						var name = this.ParseColumnName (data[0]);
						var columnId = database.Columns.First (c => c.Name == name).MetaData.Id;
						var entityColumnSort = new EntityColumnSort ()
						{
							SortOrder = this.ParseColumnSortOrder (data[1]),
						};

						yield return new ColumnRef<EntityColumnSort> (columnId, entityColumnSort);
					}
				}
			}
		}


		private string ParseColumnName(string id)
		{
			return this.CoreServer.Caches.ColumnIdCache.GetItem (id);
		}


		private ColumnSortOrder ParseColumnSortOrder(string sortOrder)
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


		private EntityFilter ParseEntityFilter(Core.Databases.Database database, string filterParameter)
		{
			var entityType = database.DataSetMetadata.DataSetEntityType;
			var entityId = EntityInfo.GetTypeId (entityType);
			var entityFilter = new EntityFilter (entityId);

			if (filterParameter != null)
			{
				var deserializer = new JavaScriptSerializer ();
				var filters = (object[]) deserializer.DeserializeObject (filterParameter);

				foreach (var filter in filters.Cast<Dictionary<string, object>> ())
				{
					var column = this.ParseColumn (database, filter);

					var columnId = column.MetaData.Id;
					var columnFilter = this.ParseColumnFilter (column, filter);
					var columnRef = new ColumnRef<EntityColumnFilter> (columnId, columnFilter);

					entityFilter.Columns.Add (columnRef);
				}
			}

			return entityFilter;
		}


		private Column ParseColumn(Core.Databases.Database database, Dictionary<string, object> filter)
		{
			var fieldId = (string) filter["field"];
			var fieldName = this.ParseColumnName (fieldId);

		    return database.Columns.First (c => c.Name == fieldName);
		}


		private EntityColumnFilter ParseColumnFilter(Column column, Dictionary<string, object> filter)
		{
			var lambda = column.LambdaExpression;
			var propertyAccessorCache = this.CoreServer.Caches.PropertyAccessorCache;
			var propertyAccessor = (AbstractStringPropertyAccessor) propertyAccessorCache.Get (lambda);

			var type = (string) filter["type"];
			var value = filter["value"];

			if (type == "list")
			{
				return this.ParseColumnSetFilter (propertyAccessor, type, value);
			}
			else
			{
				object comparison;
				if (!filter.TryGetValue ("comparison", out comparison))
				{
					comparison = "eq";
				}

				return this.ParseColumnComparisonFilter (propertyAccessor, type, comparison, value);
			}
		}


		private EntityColumnFilter ParseColumnSetFilter(AbstractStringPropertyAccessor propertyAccessor, string type, object value)
		{
			var valueArray = (object[]) value;
			var constants = valueArray.Select (v => this.ParseConstant (propertyAccessor, type, v));

			var filterExpression = new ColumnFilterSetExpression ()
			{
				Predicate = ColumnFilterSetCode.In,
			};

			foreach (var constant in constants)
			{
				filterExpression.Values.Add (constant);
			}

			return new EntityColumnFilter (filterExpression);
		}


		private EntityColumnFilter ParseColumnComparisonFilter(AbstractStringPropertyAccessor propertyAccessor, string type, object comparator, object value)
		{
			var comparison = this.ParseComparison (type, comparator);
			var constant = this.ParseConstant (propertyAccessor, type, value);

			var filterExpression = new ColumnFilterComparisonExpression ()
			{
				Comparison = comparison,
				Constant = constant,
			};

			return new EntityColumnFilter (filterExpression);
		}


		private ColumnFilterComparisonCode ParseComparison(string type, object comparator)
		{
			if (type == "string")
			{
				return ColumnFilterComparisonCode.LikeEscaped;
			}

			switch ((string) comparator)
			{
				case "eq":
					return ColumnFilterComparisonCode.Equal;
				case "nq":
					return ColumnFilterComparisonCode.NotEqual;
				case "gt":
					return ColumnFilterComparisonCode.GreaterThan;
				case "lt":
					return ColumnFilterComparisonCode.LessThan;
				default:
					throw new NotImplementedException ();
			}
		}


		private ColumnFilterConstant ParseConstant(AbstractStringPropertyAccessor propertyAccessor, string type, object value)
		{
			var convertedValue = value;

			switch (type)
			{
				case "numeric":
					switch (propertyAccessor.PropertyAccessorType)
					{
						case PropertyAccessorType.Integer:
							convertedValue = Convert.ToInt64 (convertedValue);
							break;

						case PropertyAccessorType.Decimal:
							convertedValue = Convert.ToDecimal (convertedValue);
							break;
					}
					break;

				case "string":
					convertedValue = "%" + Constant.Escape ((string) convertedValue) + "%";
					break;
			}

			convertedValue = propertyAccessor.ConvertValue (convertedValue);

			switch (propertyAccessor.PropertyAccessorType)
			{
				case PropertyAccessorType.Boolean:
					return ColumnFilterConstant.From ((bool?) convertedValue);

				case PropertyAccessorType.Date:
					return ColumnFilterConstant.From ((Date?) convertedValue);

				case PropertyAccessorType.Decimal:
					return ColumnFilterConstant.From ((decimal?) convertedValue);

				case PropertyAccessorType.Enumeration:
					return ColumnFilterConstant.From ((Enum) convertedValue);

				case PropertyAccessorType.Integer:
					if (convertedValue is short? || convertedValue is short)
					{
						return ColumnFilterConstant.From ((short?) convertedValue);
					}
					else if (convertedValue is int? || convertedValue is int)
					{
						return ColumnFilterConstant.From ((int?) convertedValue);
					}
					else if (convertedValue is long? || convertedValue is long)
					{
						return ColumnFilterConstant.From ((long?) convertedValue);
					}
					else
					{
						throw new NotImplementedException ();
					}

				case PropertyAccessorType.Text:
					return ColumnFilterConstant.From (FormattedText.CastToString (convertedValue));

				default:
					throw new NotImplementedException ();
			}
		}


		private Response DeleteEntities(BusinessContext businessContext)
		{
			string rawEntityIds = Request.Form.entityIds;
			var entityIds = rawEntityIds.Split (";");

			var success = true;

			foreach (var entityId in entityIds)
			{
				var entity = Tools.ResolveEntity (businessContext, entityId);

				using (businessContext.Bind (entity))
				{
					success = businessContext.DeleteEntity (entity);
				}

				if (!success)
				{
					break;
				}
			}

			if (success)
			{
				businessContext.SaveChanges (LockingPolicy.KeepLock);
			}

			return success
				? CoreResponse.Success ()
				: CoreResponse.Failure ();
		}


		private Response CreateEntity(BusinessContext businessContext)
		{
			string databaseName = Request.Form.databaseName;
			var database = this.CoreServer.DatabaseManager.GetDatabase (databaseName);

			var dataContext = businessContext.DataContext;

			var entity = database.CreateEntity (businessContext);
			var entityData = database.GetEntityData (dataContext, this.CoreServer.Caches, entity);

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
