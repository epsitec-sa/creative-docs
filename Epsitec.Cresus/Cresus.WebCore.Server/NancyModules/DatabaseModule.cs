using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

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
	public class DatabasesModule : AbstractBusinessContextModule
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


		public DatabasesModule(CoreServer coreServer)
			: base (coreServer, "/database")
		{
			Get["/list"] = p => this.Execute (b => this.GetDatabaseList (b));
			Get["/get/{name}"] = p => this.Execute (b => this.GetDatabase (b, p));
			Post["/delete"] = p => this.Execute (b => this.DeleteEntity (b));
			Post["/create/{name}"] = p => this.Execute (b => this.CreateEntity (b, p));
		}


		private Response GetDatabaseList(BusinessContext businessContext)
		{
			var content = from database in DatabasesModule.databases.Values
			              select new Dictionary<string, object> ()
			              {
			              	  { "Title", database.Title },
			              	  { "DatabaseName", database.DatabaseName },
			              	  { "CssClass", database.CssClass },
			              };

			return CoreResponse.AsSuccess (content.ToList ());
		}


		private Response GetDatabase(BusinessContext businessContext, dynamic parameters)
		{
			string databaseName = parameters.name;

			int start = Request.Query.start;
			int limit = Request.Query.limit;

			var database = DatabasesModule.databases[databaseName];

			var total = database.GetCount (businessContext);

			var entities = from entity in database.GetEntities (businessContext, start, limit)
			               let summary = entity.GetCompactSummary ().ToSimpleText ()
			               let id = Tools.GetEntityId (businessContext, entity)
			               select new
						   {
							   name = summary,
							   uniqueId = id,
						   };

			var content = new Dictionary<string, object> ()
			{
				{ "total", total },
				{ "entities", entities.ToList () },
			};

			return CoreResponse.AsJson (content);
		}


		private Response DeleteEntity(BusinessContext businessContext)
		{
			string entityId = Request.Form.entityId;

			var entity = Tools.ResolveEntity (businessContext, entityId);

			var ok = false;

			using (businessContext.Bind (entity))
			{
				ok = businessContext.DeleteEntity (entity);

				businessContext.SaveChanges ();
			}

			return ok
				? CoreResponse.AsSuccess ()
				: CoreResponse.AsError ();
		}


		private Response CreateEntity(BusinessContext businessContext, dynamic parameters)
		{
			// TODO This implementation is very simple and will only work if the entity that will be
			// created is not of an abstract type. If it is an abstract entity, probable that an
			// entity of the wrong type will be created. Probably that we should implement something
			// with the CreationControllers.

			string databaseName = parameters.name;
			var database = DatabasesModule.databases[databaseName];

			var entity = database.Create (businessContext);
			var entityId = Tools.GetEntityId (businessContext, entity);

			return CoreResponse.AsSuccess (entityId);
		}


		private readonly static Dictionary<string, Database> databases;


	}


}
