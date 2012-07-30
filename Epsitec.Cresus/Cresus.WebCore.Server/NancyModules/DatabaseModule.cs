using Epsitec.Aider.Entities;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.WebCore.Server.Core;
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
	public class DatabasesModule : AbstractBusinessContextModule
	{


		static DatabasesModule()
		{
			// HACK This is an ugly hack and we should not initialize this from here like that, with
			// static texts and entities and whatever. We should read the menu description from a
			// configuration file or something to do it properly, so we could have different
			// configurations for different applications.

			DatabasesModule.databases = new Dictionary<Type, Database> ();

			if (AppDomain.CurrentDomain.FriendlyName == "App.Aider.vshost.exe" || AppDomain.CurrentDomain.FriendlyName == "App.Aider.exe")
			{
				DatabasesModule.SetupDatabase<AiderCountryEntity> ("Countries", "Base.Country", typeof (CountryEntity));
				DatabasesModule.SetupDatabase<AiderTownEntity> ("Towns", "Base.Location", typeof (LocationEntity));
				DatabasesModule.SetupDatabase<AiderAddressEntity> ("Addresses", "Data.AiderAddress", typeof (AiderAddressEntity));
				DatabasesModule.SetupDatabase<AiderHouseholdEntity> ("Households", "Data.AiderHousehold", typeof (AiderHouseholdEntity));
				DatabasesModule.SetupDatabase<AiderPersonEntity> ("Persons", "Base.AiderPerson", typeof (AiderPersonEntity));
				DatabasesModule.SetupDatabase<AiderPersonRelationshipEntity> ("Relationships", "Base.AiderPersonRelationship", typeof (AiderPersonRelationshipEntity));
			}
			else
			{
				DatabasesModule.SetupDatabase<CustomerEntity> ("Clients", "Base.Customer", typeof (CustomerEntity));
				DatabasesModule.SetupDatabase<ArticleDefinitionEntity> ("Articles", "Base.ArticleDefinition", typeof (ArticleDefinitionEntity));
				DatabasesModule.SetupDatabase<PersonGenderEntity> ("Genres", "Base.PersonGender", typeof (PersonGenderEntity));
			}
		}


		private static void SetupDatabase<T>(string title, string iconUri, Type iconType) 
			where T : AbstractEntity, new ()
		{
			DatabasesModule.databases[typeof (T)] = new Database<T>
			{
				Title = title,
				CssClass = IconManager.GetCssClassName (iconType, iconUri, IconSize.ThirtyTwo)
			};
		}


		public DatabasesModule(CoreServer coreServer)
			: base (coreServer, "/database")
		{
			Get["/list"] = p => this.GetDatabaseList ();
			Get["/get/{name}"] = p => this.Execute (b => this.GetDatabase (b, p));
			Post["/delete"] = p => this.Execute (b => this.DeleteEntities (b));
			Post["/create/{name}"] = p => this.Execute (b => this.CreateEntity (b, p));
		}


		private Response GetDatabaseList()
		{
			var content = from database in DatabasesModule.databases.Values
			              select new Dictionary<string, object> ()
			              {
			              	  { "title", database.Title },
			              	  { "name", database.Name },
			              	  { "cssClass", database.CssClass },
			              };

			return CoreResponse.AsSuccess (content.ToList ());
		}


		private Response GetDatabase(BusinessContext businessContext, dynamic parameters)
		{
			string databaseName = parameters.name;

			int start = Request.Query.start;
			int limit = Request.Query.limit;

			var databaseType = Tools.ParseType (databaseName);
			var database = DatabasesModule.databases[databaseType];

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


		private Response DeleteEntities(BusinessContext businessContext)
		{
			string rawEntityIds = Request.Form.entityIds;
			var entityIds = rawEntityIds.Split (";");

			var sucess = true;

			foreach (var entityId in entityIds)
			{
				var entity = Tools.ResolveEntity (businessContext, entityId);

				using (businessContext.Bind (entity))
				{
					sucess = businessContext.DeleteEntity (entity);
				}

				if (!sucess)
				{
					break;
				}
			}

			if (sucess)
			{
				businessContext.SaveChanges ();
			}
			
			return sucess
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
			var databaseType = Tools.ParseType (databaseName);
			var database = DatabasesModule.databases[databaseType];

			var entity = database.Create (businessContext);
			var entityId = Tools.GetEntityId (businessContext, entity);

			return CoreResponse.AsSuccess (entityId);
		}


		private readonly static Dictionary<Type, Database> databases;


	}


}
