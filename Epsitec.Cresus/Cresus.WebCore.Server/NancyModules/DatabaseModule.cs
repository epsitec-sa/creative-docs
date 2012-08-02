using Epsitec.Aider.Entities;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Expressions;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;

using System.Collections.Generic;

using System.Linq;

using System.Reflection;
using System.Text.RegularExpressions;


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
			return CoreResponse.AsSuccess (DatabasesModule.GetDatabases ());
		}


		private static List<Dictionary<string, object>> GetDatabases()
		{
			// HACK This is an ugly hack and we should not initialize this from here like that, with
			// static texts and entities and whatever. We should read the menu description from a
			// configuration file or something to do it properly, so we could have different
			// configurations for different applications.

			var appDomainName = AppDomain.CurrentDomain.FriendlyName;

			if (appDomainName == "App.Aider.vshost.exe" || appDomainName == "App.Aider.exe")
			{
				return DatabasesModule.GetAiderDatabases ();
			}
			else
			{
				return DatabasesModule.GetCoreDatabases ();
			}
		}


		private static List<Dictionary<string, object>> GetAiderDatabases()
		{
			return new List<Dictionary<string, object>> ()
			{
				DatabasesModule.GetDatabaseData<AiderCountryEntity, CountryEntity> ("Countries", "Base.Country"),
				DatabasesModule.GetDatabaseData<AiderTownEntity, LocationEntity> ("Towns", "Base.Location"),
				DatabasesModule.GetDatabaseData<AiderAddressEntity, AiderAddressEntity> ("Addresses", "Data.AiderAddress"),
				DatabasesModule.GetDatabaseData<AiderHouseholdEntity, AiderHouseholdEntity> ("Households", "Data.AiderHousehold"),
				DatabasesModule.GetDatabaseData<AiderPersonEntity, AiderPersonEntity> ("Persons", "Base.AiderPerson"),
				DatabasesModule.GetDatabaseData<AiderPersonRelationshipEntity, AiderPersonRelationshipEntity> ("Relationships", "Base.AiderPersonRelationship"),
			};
		}


		private static List<Dictionary<string, object>> GetCoreDatabases()
		{
			return new List<Dictionary<string, object>> ()
			{
				DatabasesModule.GetDatabaseData<CustomerEntity, CustomerEntity> ("Clients", "Base.Customer"),
				DatabasesModule.GetDatabaseData<ArticleDefinitionEntity, ArticleDefinitionEntity> ("Articles", "Base.ArticleDefinition"),
				DatabasesModule.GetDatabaseData<PersonGenderEntity, PersonGenderEntity> ("Genres", "Base.PersonGender"),
			};
		}


		private static Dictionary<string, object> GetDatabaseData<T1, T2>(string title, string iconUri)
			where T1 : AbstractEntity, new ()
			where T2 : AbstractEntity, new ()
		{
			return new Dictionary<string, object> ()
			{
			    { "title", title },
			    { "name", Tools.TypeToString (typeof (T1)) },
			    { "cssClass", IconManager.GetCssClassName (typeof (T2), iconUri, IconSize.ThirtyTwo) },
			};
		}


		private Response GetDatabase(BusinessContext businessContext, dynamic parameters)
		{
			string databaseName = parameters.name;

			int start = Request.Query.start;
			int limit = Request.Query.limit;

			var databaseType = Tools.ParseType (databaseName);

			var total = DatabasesModule.GetEntitiesCount (businessContext, databaseType);
			var entities = DatabasesModule.GetEntities (businessContext, databaseType, start, limit)
				.Select(e => DatabasesModule.GetEntityData(businessContext, e))
				.ToList();

			var content = new Dictionary<string, object> ()
			{
				{ "total", total },
				{ "entities", entities },
			};

			return CoreResponse.AsJson (content);
		}


		private static Dictionary<string, string> GetEntityData(BusinessContext businessContext, AbstractEntity entity)
		{
			var id = Tools.GetEntityId (businessContext, entity);
			var summary = entity.GetCompactSummary ().ToSimpleText ();
			
			return new Dictionary<string, string> ()
			{
				{ "id", id },
				{ "summary", summary },
			};
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

			var entity = DatabasesModule.CreateEntity (businessContext, databaseType);
			var entityData = DatabasesModule.GetEntityData (businessContext, entity);

			return CoreResponse.AsSuccess (entityData);
		}


		private static IEnumerable<AbstractEntity> GetEntities(BusinessContext businessContext, Type entityType, int skip, int take)
		{
			var methodName = "GetEntitiesImplementation";
			var arguments = new object[] { businessContext, skip, take };

			return DatabasesModule.InvokeGenericMethod<IEnumerable<AbstractEntity>> (methodName, entityType, arguments);
		}


		private static IEnumerable<AbstractEntity> GetEntitiesImplementation<T>(BusinessContext businessContext, int skip, int take)
			where T : AbstractEntity, new()
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

			return businessContext.DataContext.GetByRequest<T> (request);
		}


		private static int GetEntitiesCount(BusinessContext businessContext, Type entityType)
		{
			var methodName = "GetEntitiesCountImplementation";
			var arguments = new object[] { businessContext };

			return DatabasesModule.InvokeGenericMethod<int> (methodName, entityType, arguments);
		}


		private static int GetEntitiesCountImplementation<T>(BusinessContext businessContext)
			where T : AbstractEntity, new()
		{
			return businessContext.DataContext.GetCount (new T ());
		}


		private static AbstractEntity CreateEntity(BusinessContext businessContext, Type entityType)
		{
			var methodName = "CreateEntityImplementation";
			var arguments = new object[] { businessContext };

			return DatabasesModule.InvokeGenericMethod<AbstractEntity> (methodName, entityType, arguments);
		}


		private static AbstractEntity CreateEntityImplementation<T>(BusinessContext businessContext)
			where T : AbstractEntity, new()
		{
			var entity = businessContext.CreateEntity<T> ();

			// NOTE Here we need to include the empty entities, otherwise we might be in the case
			// where the entity that we just have created will be empty and thus not saved and this
			// will lead the user to click like a maniac on the "create" button without noticeable
			// result other than him becoming mad :-P

			businessContext.SaveChanges (EntitySaveMode.IncludeEmpty);

			return entity;
		}


		private static T InvokeGenericMethod<T>(string methodName, Type genericArgument, object[] arguments)
		{
			var flags = BindingFlags.NonPublic | BindingFlags.Static;
			var type = typeof (DatabasesModule);
			var method = type.GetMethod (methodName, flags);
			var genericMethod = method.MakeGenericMethod (genericArgument);

			return (T) genericMethod.Invoke (null, arguments);
		}


	}


}
