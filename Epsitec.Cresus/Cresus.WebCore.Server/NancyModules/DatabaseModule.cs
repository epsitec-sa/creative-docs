

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.WebCore.Server.CoreServer;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// Used to populate the left list and the header menu.
	/// A list of available databases is populated when the module is loaded, 
	/// and it is able to the Javascript which menu to show.
	/// It is then able to retrieve a list of entities based on the request.
	/// It is also able to add or delete an entity within the selected database.
	/// </summary>
	public class DatabasesModule : AbstractCoreSessionModule
	{


		static DatabasesModule()
		{
			DatabasesModule.databases = new Dictionary<string, Database> ();

			DatabasesModule.databases["customers"] = new Database<CustomerEntity>
			{
				Title = "Clients",
				DatabaseName = "customers",
				CSSClass = IconManager.GetCSSClassName ("Base.Customer", IconSize.ThirtyTwo)
			};

			DatabasesModule.databases["articles"] = new Database<ArticleDefinitionEntity>
			{
				Title = "Articles",
				DatabaseName = "articles",
				CSSClass = IconManager.GetCSSClassName ("Base.ArticleDefinition", IconSize.ThirtyTwo)
			};

			DatabasesModule.databases["genders"] = new Database<PersonGenderEntity>
			{
				Title = "Genres",
				DatabaseName = "genders",
				CSSClass = IconManager.GetCSSClassName ("Base.PersonGender", IconSize.ThirtyTwo)
			};
		}


		public DatabasesModule(ServerContext serverContext)
			: base (serverContext, "/database")
		{

			Get["/list"] = parameters => this.ExecuteWithCoreSession (coreSession =>
			{
				var list = new List<object> ();

				DatabasesModule.databases.ForEach (o => list.Add (o.Value));

				return Response.AsCoreSuccess (list);
			});

			Get["/{name}"] = parameters => this.ExecuteWithCoreSession(coreSession => 
			{
				var context = coreSession.GetBusinessContext ();
				var dataContext = context.DataContext;

				string databaseName = parameters.name;

				// Get all entites from the current Type
				var type = DatabasesModule.databases[databaseName].GetDatabaseType ();
				var method = typeof (BusinessContext).GetMethod ("GetAllEntities");
				var m = method.MakeGenericMethod (type);
				var o = m.Invoke (context, new object[0]);
				var enumerable = o as IEnumerable<AbstractEntity>;

				var start = (int) Request.Query.start;
				var limit = (int) Request.Query.limit;

				var list = from c in enumerable
						   let summary = c.GetCompactSummary ().ToSimpleText ()
						   // orderby summary // TODO Awefully slow !
						   select new
						   {
							   name = summary,
							   uniqueId = dataContext.GetNormalizedEntityKey (c).Value.ToString ()
						   };

				// Only take a subset of all the entities
				var subset = list.Skip (start).Take (limit).ToList ();

				var dic = new Dictionary<string, object> ();
				dic["total"] = enumerable.Count (); // For ExtJS
				dic["entities"] = subset;

				var res = Response.AsJson (dic);

				return res;
			});

			Post["/delete"] = parameters => this.ExecuteWithCoreSession(coreSession => 
			{
				var context = coreSession.GetBusinessContext ();

				string paramEntityKey = (string) Request.Form.entityId;

				var entityKey = EntityKey.Parse (paramEntityKey);
				AbstractEntity entity = context.DataContext.ResolveEntity (entityKey);

				var ok = context.DeleteEntity (entity);

				context.SaveChanges ();

				return Response.AsCoreBoolean (ok);
			});

			Post["/create"] = parameters => this.ExecuteWithCoreSession(coreSession => 
			{
				var context = coreSession.GetBusinessContext ();

				// TODO Being able to create an entity 
				// (problems with the AbstractPerson)

				return Response.AsCoreError ();
			});

		}


		private readonly static Dictionary<string, Database> databases;


		private abstract class Database
		{
			public abstract Type GetDatabaseType();
		}


		private sealed class Database<T> : Database
			where T : AbstractEntity
		{
			public string Title;
			public string DatabaseName;
			public string CSSClass;

			public override Type GetDatabaseType()
			{
				return typeof (T);
			}
		}


	}


}
