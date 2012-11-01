using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using System;

using System.Collections.Generic;

using System.Linq;

using System.Linq.Expressions;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


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


		public object GetColumnData(Caches caches, AbstractEntity entity)
		{
			var propertyAccessor = caches.PropertyAccessorCache.Get (this.LambdaExpression);		
			var stringPropertyAccessor = (AbstractStringPropertyAccessor) propertyAccessor;

			return stringPropertyAccessor.GetValue (entity);
		}


		public Dictionary<string, object> GetDataDictionary(Caches caches)
		{
			return new Dictionary<string, object> ()
			{
				{ "title", this.Title },
				{ "name", this.GetId (caches) },
				{ "type", this.GetColumnTypeData (caches) },
				{ "hidden", this.Hidden },
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
			var propertyAccessorType = this.GetPropertyAccessorType (caches);

			var data = new Dictionary<string, object> ()
			{
				{ "type", this.GetPropertyAccessorTypeData (propertyAccessorType) },
			};

			if (propertyAccessorType == PropertyAccessorType.Enumeration)
			{
				data["enumerationName"] = Tools.TypeToString (this.LambdaExpression.ReturnType);
			}

			return data;
		}


		private PropertyAccessorType GetPropertyAccessorType(Caches caches)
		{
			var lambdaExpression = this.LambdaExpression;
			var propertyAccessor = caches.PropertyAccessorCache.Get (lambdaExpression);

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


		private Dictionary<string, object> GetFilterData(Caches caches)
		{
			var data = new Dictionary<string, object> ()
			{		
				{ "filterable", this.Filterable },
			};

			var propertyAccessorType = this.GetPropertyAccessorType (caches);

			if (this.Filterable && propertyAccessorType == PropertyAccessorType.Enumeration)
			{
				data["enumerationName"] = Tools.TypeToString (this.LambdaExpression.ReturnType);
			}

			return data;
		}


		private readonly EntityColumnMetadata metadata;


	}


}
