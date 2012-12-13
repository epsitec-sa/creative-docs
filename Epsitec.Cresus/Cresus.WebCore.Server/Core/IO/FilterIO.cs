using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.DataLayer.Expressions;

using Epsitec.Cresus.WebCore.Server.Core.Databases;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Nancy.Json;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.IO
{


	internal static class FilterIO
	{


		public static EntityFilter ParseFilter(Caches caches, Core.Databases.Database database, string filterParameter)
		{
			var entityType = database.DataSetMetadata.EntityTableMetadata.EntityType;
			var entityId = EntityInfo.GetTypeId (entityType);
			var entityFilter = new EntityFilter (entityId);

			if (filterParameter != null)
			{
				var deserializer = new JavaScriptSerializer ();
				var filters = (object[]) deserializer.DeserializeObject (filterParameter);

				foreach (var filter in filters.Cast<Dictionary<string, object>> ())
				{
					var column = FilterIO.ParseColumn (caches, database, filter);

					var columnId = column.MetaData.Id;
					var columnFilter = FilterIO.ParseColumnFilter (caches, column, filter);
					var columnRef = new ColumnRef<EntityColumnFilter> (columnId, columnFilter);

					entityFilter.Columns.Add (columnRef);
				}
			}

			return entityFilter;
		}


		private static Column ParseColumn(Caches caches, Core.Databases.Database database, Dictionary<string, object> filter)
		{
			var fieldId = (string) filter["field"];
			var fieldName = caches.ColumnIdCache.GetItem (fieldId);

			return database.Columns.First (c => c.Name == fieldName);
		}


		private static EntityColumnFilter ParseColumnFilter(Caches caches, Column column, Dictionary<string, object> filter)
		{
			var lambda = column.LambdaExpression;
			var propertyAccessorCache = caches.PropertyAccessorCache;
			var propertyAccessor = propertyAccessorCache.Get (lambda);

			var fieldType = propertyAccessor.FieldType;
			var valueType = propertyAccessor.Type;

			var type = (string) filter["type"];
			var value = filter["value"];

			if (type == "list")
			{
				return FilterIO.ParseColumnSetFilter (fieldType, valueType, type, value);
			}
			else
			{
				object comparison;
				if (!filter.TryGetValue ("comparison", out comparison))
				{
					comparison = "eq";
				}

				return FilterIO.ParseColumnComparisonFilter (fieldType, valueType, type, comparison, value);
			}
		}


		private static EntityColumnFilter ParseColumnSetFilter(FieldType fieldType, Type valueType, string type, object value)
		{
			var valueArray = (object[]) value;
			var constants = valueArray.Select (v => FilterIO.ParseConstant (fieldType, valueType, type, v));

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


		private static EntityColumnFilter ParseColumnComparisonFilter(FieldType fieldType, Type valueType, string type, object comparator, object value)
		{
			var comparison = FilterIO.ParseComparison (type, comparator);
			var constant = FilterIO.ParseConstant (fieldType, valueType, type, value);

			var filterExpression = new ColumnFilterComparisonExpression ()
			{
				Comparison = comparison,
				Constant = constant,
			};

			return new EntityColumnFilter (filterExpression);
		}


		private static ColumnFilterComparisonCode ParseComparison(string type, object comparator)
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


		private static ColumnFilterConstant ParseConstant(FieldType fieldType, Type valueType, string type, object value)
		{
			var clientValue = value;

			switch (type)
			{
				case "numeric":
					switch (fieldType)
					{
						case FieldType.Integer:
							clientValue = Convert.ToInt64 (value);
							break;

						case FieldType.Decimal:
							clientValue = Convert.ToDecimal (value);
							break;
					}
					break;

				case "string":
					clientValue = "%" + Constant.Escape ((string) clientValue) + "%";
					break;
			}

			var fieldValue = ValueConverter.ConvertClientToField (clientValue, fieldType, valueType);
			var entityValue = ValueConverter.ConvertFieldToEntity (fieldValue, fieldType, valueType);

			switch (fieldType)
			{
				case FieldType.Boolean:
					return ColumnFilterConstant.From ((bool?) entityValue);

				case FieldType.Date:
					return ColumnFilterConstant.From ((Date?) entityValue);

				case FieldType.Decimal:
					return ColumnFilterConstant.From ((decimal?) entityValue);

				case FieldType.Enumeration:
					return ColumnFilterConstant.From ((Enum) entityValue);

				case FieldType.Integer:
					if (entityValue is short? || entityValue is short)
					{
						return ColumnFilterConstant.From ((short?) entityValue);
					}
					else if (entityValue is int? || entityValue is int)
					{
						return ColumnFilterConstant.From ((int?) entityValue);
					}
					else if (entityValue is long? || entityValue is long)
					{
						return ColumnFilterConstant.From ((long?) entityValue);
					}
					else
					{
						throw new NotImplementedException ();
					}

				case FieldType.Text:
					return ColumnFilterConstant.From (FormattedText.CastToString (entityValue));

				default:
					throw new NotImplementedException ();
			}
		}



	}


}
