using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Entities;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	internal sealed class DatabaseManager
	{


		public DatabaseManager()
		{
			this.databases = this.CreateDatabases ();
		}


		private Dictionary<string, Database> CreateDatabases()
		{
			// HACK This is an ugly hack and we should not initialize this from here like that, with
			// static texts and entities and whatever. We should read the menu description from a
			// configuration file or something to do it properly, so we could have different
			// configurations for different applications.

			IEnumerable<Database> databases;

			var appDomainName = AppDomain.CurrentDomain.FriendlyName;

			if (appDomainName == "App.Aider.vshost.exe" || appDomainName == "App.Aider.exe")
			{
				databases = this.CreateAiderDatabases ();
			}
			else
			{
				databases = this.CreateCoreDatabases ();
			}

			return databases.ToDictionary (d => d.Name);
		}


		private IEnumerable<Database> CreateAiderDatabases()
		{
			yield return Database.Create<AiderCountryEntity, CountryEntity> ("Countries", "Base.Country");
			yield return Database.Create<AiderTownEntity, LocationEntity> ("Towns", "Base.Location");
			yield return Database.Create<AiderAddressEntity, AiderAddressEntity> ("Addresses", "Data.AiderAddress");
			yield return Database.Create<AiderHouseholdEntity, AiderHouseholdEntity> ("Households", "Data.AiderHousehold");
			yield return Database.Create<AiderPersonEntity, AiderPersonEntity> ("Persons", "Base.AiderPerson");
			yield return Database.Create<AiderPersonRelationshipEntity, AiderPersonRelationshipEntity> ("Relationships", "Base.AiderPersonRelationship");
		}


		private IEnumerable<Database> CreateCoreDatabases()
		{
			yield return Database.Create<CustomerEntity, CustomerEntity> ("Clients", "Base.Customer");
			yield return Database.Create<ArticleDefinitionEntity, ArticleDefinitionEntity> ("Articles", "Base.ArticleDefinition");
			yield return Database.Create<PersonGenderEntity, PersonGenderEntity> ("Genres", "Base.PersonGender");
		}


		public IEnumerable<Database> GetDatabases()
		{
			return this.databases.Values;
		}


		public Database GetDatabase(string name)
		{
			Database database;

			var exists = this.databases.TryGetValue (name, out database);

			if (!exists)
			{
				var type = Tools.ParseType (name);

				database = Database.Create (type);
			}

			return database;
		}


		private readonly Dictionary<string, Database> databases;


	}


}
