using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	internal abstract class Database
	{


		public Database(string title, string name, string iconClass)
		{
			this.title = title;
			this.name = name;
			this.iconClass = iconClass;
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


		public abstract Dictionary<string, object> GetEntityData(BusinessContext businessContext, AbstractEntity entity);


		public abstract IEnumerable<AbstractEntity> GetEntities(BusinessContext businessContext, int skip, int take);


		public abstract int GetCount(BusinessContext businessContext);


		public abstract AbstractEntity CreateEntity(BusinessContext businessContext);


		public abstract bool DeleteEntity(BusinessContext businessContext, AbstractEntity entity);


		public static Database Create<T1, T2>(string title, string iconUri)
			where T1 : AbstractEntity, new ()
			where T2 : AbstractEntity, new ()
		{
			var name = Tools.TypeToString (typeof (T1));
			var iconClass = IconManager.GetCssClassName (typeof (T2), iconUri, IconSize.ThirtyTwo);

			return new Database<T1> (title, name, iconClass);
		}


		public static Database Create(Type type)
		{
			var title = "";
			var name = Tools.TypeToString (type);
			var iconClass = "";
			var arguments = new object[] { title, name, iconClass };
			
			var genericType = typeof (Database<>);
			var concreteType = genericType.MakeGenericType (type);

			return (Database) Activator.CreateInstance (concreteType, arguments);
		}


		private readonly string title;


		private readonly string name;
		
		
		private readonly string iconClass;


	}


}
