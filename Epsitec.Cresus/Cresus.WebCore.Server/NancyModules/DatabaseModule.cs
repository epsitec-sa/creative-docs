using Epsitec.Aider.Entities;

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;

using Epsitec.Cresus.WebCore.Server.CoreServer;
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

			return CoreResponse.AsSuccess (list);
		}


		private Response GetDatabase(CoreSession coreSession, dynamic parameters)
		{
			// NOTE Should we use a RequestView here, in order to maintain consistency between each
 			// calls ?

			var businessContext = coreSession.GetBusinessContext ();
			var dataContext = businessContext.DataContext;

			string databaseName = parameters.name;

			int start = Request.Query.start;
			int limit = Request.Query.limit;

			var database = DatabasesModule.databases[databaseName];

			var total = database.GetCount (dataContext);
			
			var entities = from entity in database.GetEntities (dataContext, start, limit)
			               let summary = entity.GetCompactSummary ().ToSimpleText ()
			               let id = Tools.GetEntityId (businessContext, entity)
			               select new
						   {
							   name = summary,
							   uniqueId = id,
						   };

			var content = new Dictionary<string, object> ()
			{
				{"total", total },
				{"entities", entities.ToList () },
			};

			return CoreResponse.AsJson (content);
		}


		private Response DeleteEntity(CoreSession coreSession)
		{
			var context = coreSession.GetBusinessContext ();

			string paramEntityKey = Request.Form.entityId;

			AbstractEntity entity = Tools.ResolveEntity (context, paramEntityKey);

			bool ok = false;

			using (context.Bind (entity))
			{
				ok = context.DeleteEntity (entity);

				context.SaveChanges ();
			}

			return ok
				? CoreResponse.AsSuccess ()
				: CoreResponse.AsError ();
		}


		private Response CreateEntity(CoreSession coreSession)
		{
			var context = coreSession.GetBusinessContext ();

			// TODO Being able to create an entity (problems with the AbstractPerson)

			return CoreResponse.AsError ();
		}


		private readonly static Dictionary<string, Database> databases;


		private abstract class Database
		{


			public string Title;
			public string DatabaseName;
			public string CssClass;


			public abstract int GetCount(DataContext dataContext);


			public abstract IEnumerable<AbstractEntity> GetEntities(DataContext dataContext, int skip, int take);


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
			where T : AbstractEntity, new ()
		{


			public override int GetCount(DataContext dataContext)
			{
				return dataContext.GetCount (new T ());
			}


			public override IEnumerable<AbstractEntity> GetEntities(DataContext dataContext, int skip, int take)
			{
				var example = new T ();

				var request = new DataLayer.Loader.Request ()
				{
					RootEntity = example,
					Skip = skip,
					Take = take,
				};

				request.AddSortClause
				(
					InternalField.CreateId (example),
					SortOrder.Ascending
				);

				return dataContext.GetByRequest<T> (request);
			}
		}


	}


}
