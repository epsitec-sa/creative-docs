using Epsitec.Aider.Entities;

using Epsitec.Common.Support.EntityEngine;

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
			// HACK This is an ugly hack and we should not initialize this from here like that, with
			// static texts and entities and whatever. We should read the menu description from a
			// configuration file or something to do it properly, so we could have different
			// configurations for different applications.

			DatabasesModule.databases = new Dictionary<string, Database> ();

			if (System.AppDomain.CurrentDomain.FriendlyName == "App.Aider.vshost.exe" || System.AppDomain.CurrentDomain.FriendlyName == "App.Aider.exe")
			{
				DatabasesModule.databases["countries"] = new Database<AiderCountryEntity>
				{
					Title = "Countries",
					DatabaseName = "countries",
					CssClass = IconManager.GetCssClassName (typeof (CountryEntity), "Base.Country", IconSize.ThirtyTwo)
				};

				DatabasesModule.databases["towns"] = new Database<AiderTownEntity>
				{
					Title = "Towns",
					DatabaseName = "towns",
					CssClass = IconManager.GetCssClassName (typeof (LocationEntity), "Base.Location", IconSize.ThirtyTwo)
				};

				DatabasesModule.databases["addresses"] = new Database<AiderAddressEntity>
				{
					Title = "Addresses",
					DatabaseName = "addresses",
					CssClass = IconManager.GetCssClassName (typeof (AiderAddressEntity), "Data.AiderAddress", IconSize.ThirtyTwo)
				};

				DatabasesModule.databases["households"] = new Database<AiderHouseholdEntity>
				{
					Title = "Households",
					DatabaseName = "households",
					CssClass = IconManager.GetCssClassName (typeof (AiderHouseholdEntity), "Data.AiderHousehold", IconSize.ThirtyTwo)
				};

				DatabasesModule.databases["persons"] = new Database<AiderPersonEntity>
				{
					Title = "Persons",
					DatabaseName = "persons",
					CssClass = IconManager.GetCssClassName (typeof (AiderPersonEntity), "Base.AiderPerson", IconSize.ThirtyTwo)
				};

				DatabasesModule.databases["relationships"] = new Database<AiderPersonRelationshipEntity>
				{
					Title = "Relationships",
					DatabaseName = "relationships",
					CssClass = IconManager.GetCssClassName (typeof (AiderPersonRelationshipEntity), "Base.AiderPersonRelationship", IconSize.ThirtyTwo)
				};
			}
			else
			{
				DatabasesModule.databases["customers"] = new Database<CustomerEntity>
				{
					Title = "Clients",
					DatabaseName = "customers",
					CssClass = IconManager.GetCssClassName (typeof (CustomerEntity), "Base.Customer", IconSize.ThirtyTwo)
				};

				DatabasesModule.databases["articles"] = new Database<ArticleDefinitionEntity>
				{
					Title = "Articles",
					DatabaseName = "articles",
					CssClass = IconManager.GetCssClassName (typeof (ArticleDefinitionEntity), "Base.ArticleDefinition", IconSize.ThirtyTwo)
				};

				DatabasesModule.databases["genders"] = new Database<PersonGenderEntity>
				{
					Title = "Genres",
					DatabaseName = "genders",
					CssClass = IconManager.GetCssClassName (typeof (PersonGenderEntity), "Base.PersonGender", IconSize.ThirtyTwo)
				};
			}
		}


		public DatabasesModule(ServerContext serverContext)
			: base (serverContext, "/database")
		{
			Get["/list"] = p => this.ExecuteWithCoreSession (cs => this.GetDatabaseList (cs));
			Get["/{name}"] = p => this.ExecuteWithCoreSession (cs => this.GetDatabase (cs, p));
			Post["/delete"] = p => this.ExecuteWithCoreSession (cs => this.DeleteEntity (cs));
			Post["/create"] = p => this.ExecuteWithCoreSession (cs => this.CreateEntity (cs));
		}


		private Response GetDatabaseList(CoreSession coreSession)
		{
			var list = DatabasesModule.databases.Values.Select (d => d.ToDictionary ()).ToList ();

			return Response.AsCoreSuccess (list);
		}


		private Response GetDatabase(CoreSession coreSession, dynamic parameters)
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
		}


		private Response DeleteEntity(CoreSession coreSession)
		{
			var context = coreSession.GetBusinessContext ();

			string paramEntityKey = (string) Request.Form.entityId;

			var entityKey = EntityKey.Parse (paramEntityKey);
			AbstractEntity entity = context.DataContext.ResolveEntity (entityKey);

			bool ok = false;

			using (context.Bind(entity))
			{
				ok = context.DeleteEntity (entity);	
			}

			context.SaveChanges ();

			return Response.AsCoreBoolean (ok);
		}


		private Response CreateEntity(CoreSession coreSession)
		{
			var context = coreSession.GetBusinessContext ();

			// TODO Being able to create an entity (problems with the AbstractPerson)

			return Response.AsCoreError ();
		}


		private readonly static Dictionary<string, Database> databases;


		private abstract class Database
		{


			public string Title;
			public string DatabaseName;
			public string CssClass;


			public abstract Type GetDatabaseType();


			public Dictionary<string, object> ToDictionary()
			{
				return new Dictionary<string, object> ()
				{
					{ "Title", this.Title },
					{ "DatabaseName", this.DatabaseName },
					{ "CssClass", this.CssClass },
				};
			}

		}


		private sealed class Database<T> : Database
			where T : AbstractEntity
		{


			public override Type GetDatabaseType()
			{
				return typeof (T);
			}


		}


	}


}
