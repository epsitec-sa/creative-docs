using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	internal abstract class Database
	{


		public Database(string title, string name, string iconClass, IEnumerable<Column> columns, IEnumerable<Sorter> sorters)
		{
			this.title = title;
			this.name = name;
			this.iconClass = iconClass;
			this.columns = columns.ToList ();
			this.sorters = sorters.ToList ();
		}


		public string Title
		{
			get
			{
				return this.title;
			}
		}


		public string Name
		{
			get
			{
				return this.name;
			}
		}


		public string IconClass
		{
			get
			{
				return this.iconClass;
			}
		}


		public IEnumerable<Column> Columns
		{
			get
			{
				return this.columns;
			}
		}


		public IEnumerable<Sorter> Sorters
		{
			get
			{
				return this.sorters;
			}
		}


		public abstract Dictionary<string, object> GetEntityData(BusinessContext businessContext, AbstractEntity entity, PropertyAccessorCache propertyAccessorCache);


		public abstract IEnumerable<AbstractEntity> GetEntities(BusinessContext businessContext, IEnumerable<Sorter> sorters, IEnumerable<Filter> filters, int skip, int take);


		public abstract int GetCount(BusinessContext businessContext, IEnumerable<Filter> filters);


		public abstract AbstractEntity CreateEntity(BusinessContext businessContext);


		public abstract bool DeleteEntity(BusinessContext businessContext, AbstractEntity entity);


		public static Database Create(Type type, string title, string iconUri, IEnumerable<Column> columns, IEnumerable<Sorter> sorters)
		{
			var name = Tools.TypeToString (type);
			var iconClass = IconManager.GetCssClassName (type, iconUri, IconSize.ThirtyTwo);

			return Database.Create (type, title, name, iconClass, columns, sorters);
		}


		public static Database Create(Type type)
		{
			var title = "";
			var name = Tools.TypeToString (type);
			var iconClass = "";
			var columns = new Column[0];
			var sorters = new Sorter[0];

			return Database.Create (type, title, name, iconClass, columns, sorters);
		}


		private static Database Create(Type type, string title, string name, string iconClass, IEnumerable<Column> columns, IEnumerable<Sorter> sorters)
		{
			var arguments = new object[] { title, name, iconClass, columns, sorters };
			var genericType = typeof (Database<>);
			var concreteType = genericType.MakeGenericType (type);

			return (Database) Activator.CreateInstance (concreteType, arguments);
		}


		private readonly string title;


		private readonly string name;
		
		
		private readonly string iconClass;
		
		
		private readonly List<Column> columns;


		private readonly List<Sorter> sorters;


	}


}
