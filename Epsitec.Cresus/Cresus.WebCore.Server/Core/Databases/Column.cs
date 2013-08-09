using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.WebCore.Server.Core.IO;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using System;

using System.Collections.Generic;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	/// <summary>
	/// The Column class is used to represent a column within a Database. It contains method used
	/// to serialze it to the javascript client and to obtain entity values.
	/// </summary>
	internal sealed class Column
	{


		public Column(EntityColumnMetadata metadata)
		{
			this.metadata = metadata;
		}


		public EntityColumnMetadata MetaData
		{
			get
			{
				return this.metadata;
			}
		}


		public string Title
		{
			get
			{
				return this.metadata.GetColumnTitle ().ToString ();
			}
		}


		public string Name
		{
			get
			{
				return this.metadata.Path.FieldPath;
			}
		}


		public bool Hidden
		{
			get
			{
				return this.metadata.DefaultDisplay.Mode != ColumnDisplayMode.Visible;
			}
		}


		public int? Width
		{
			get
			{
				return this.metadata.DefaultWidth;
			}
		}


		public bool Sortable
		{
			get
			{
				return this.metadata.CanSort;
			}
		}


		public bool Filterable
		{
			get
			{
				return this.metadata.CanFilter;
			}
		}


		public LambdaExpression LambdaExpression
		{
			get
			{
				return this.metadata.Expression;
			}
		}


		public object GetColumnData(DataContext dataContext, Caches caches, AbstractEntity entity)
		{
			var propertyAccessor = caches.PropertyAccessorCache.Get (this.LambdaExpression);
			var value = propertyAccessor.GetValue (entity);

			return FieldIO.ConvertToClient (dataContext, value, propertyAccessor.FieldType);
		}


		public Dictionary<string, object> GetDataDictionary(Caches caches)
		{
			return new Dictionary<string, object> ()
			{
				{ "title", this.Title },
				{ "name", this.GetId (caches) },
				{ "type", this.GetColumnTypeData (caches) },
				{ "hidden", this.Hidden },
				{ "width", this.Width },
				{ "sortable", this.Sortable },
				{ "filter", this.GetFilterData (caches) },
			};
		}


		public string GetId(Caches caches)
		{
			return caches.ColumnIdCache.GetId (this.Name);
		}


		private Dictionary<string, object> GetColumnTypeData(Caches caches)
		{
			var fieldType = this.GetFieldType (caches);

			var data = new Dictionary<string, object> ()
			{
				{ "type", this.GetFieldTypeData (fieldType) },
			};

			if (fieldType == FieldType.Enumeration)
			{
				data["enumerationName"] = caches.TypeCache.GetId (this.LambdaExpression.ReturnType);
			}

			return data;
		}


		private FieldType GetFieldType(Caches caches)
		{
			var lambdaExpression = this.LambdaExpression;
			var propertyAccessor = caches.PropertyAccessorCache.Get (lambdaExpression);

			return propertyAccessor.FieldType;
		}


		private string GetFieldTypeData(FieldType type)
		{
			switch (type)
			{
				case FieldType.Boolean:
					return "boolean";

				case FieldType.Date:
					return "date";

				case FieldType.Integer:
					return "int";

				case FieldType.Enumeration:
					return "list";

				case FieldType.Decimal:
					return "float";

				case FieldType.Text:
					return "string";

				case FieldType.Time:
					return "time";

				case FieldType.EntityReference:
				case FieldType.EntityCollection:
					throw new NotSupportedException ();

				default:
					throw new NotImplementedException ();
			}
		}


		private Dictionary<string, object> GetFilterData(Caches caches)
		{
			var data = new Dictionary<string, object> ()
			{		
				{ "filterable", this.Filterable },
			};

			var fieldType = this.GetFieldType (caches);

			if (this.Filterable && fieldType == FieldType.Enumeration)
			{
				data["enumerationName"] = caches.TypeCache.GetId (this.LambdaExpression.ReturnType);
			}

			return data;
		}


		private readonly EntityColumnMetadata metadata;


	}


}
