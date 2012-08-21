using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Expressions;

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
			yield return Database.Create<AiderCountryEntity, CountryEntity>
			(
				title: "Countries",
				iconUri: "Base.Country",
				columns: new List<Column> ()
				{
					Column.Create<AiderCountryEntity, string>
					(
						title: "Name",
						name: "Name",
						type: ColumnType.String,
						hidden: false,
						sortable: true,
						sortOrder: SortOrder.Descending,
						lambdaExpression: x => x.Name
					),
					Column.Create<AiderCountryEntity, string>
					(
						title: "Code",
						name: "Code",
						type: ColumnType.String,
						hidden: false,
						sortable: false,
						sortOrder: null,
						lambdaExpression: x => x.IsoCode
					),
				}
			);

			yield return Database.Create<AiderTownEntity, LocationEntity>
			(
				title: "Towns",
				iconUri: "Base.Location",
				columns: new List<Column> ()
				{
					Column.Create<AiderTownEntity, string>
					(
						title: "Name",
						name: "Name",
						type: ColumnType.String,
						hidden: false,
						sortable: true,
						sortOrder: null,
						lambdaExpression: x => x.Name
					),
					Column.Create<AiderTownEntity, string>
					(
						title: "ZipCode",
						name: "ZipCode",
						type: ColumnType.String,
						hidden: false,
						sortable: true,
						sortOrder: null,
						lambdaExpression: x => x.ZipCode
					),
					Column.Create<AiderTownEntity, string>
					(
						title: "CountryName",
						name: "CountryName",
						type: ColumnType.String,
						hidden: false,
						sortable: true,
						sortOrder: null,
						lambdaExpression: x => x.Country.Name
					),
					Column.Create<AiderTownEntity, string>
					(
						title: "CountryCode",
						name: "CountryCode",
						type: ColumnType.String,
						hidden: true,
						sortable: true,
						sortOrder: null,
						lambdaExpression: x => x.Country.IsoCode
					),
				}
			);

			yield return Database.Create<AiderAddressEntity, AiderAddressEntity>
			(
				title: "Addresses",
				iconUri: "Data.AiderAddress",
				columns: new List<Column> ()
			);

			yield return Database.Create<AiderHouseholdEntity, AiderHouseholdEntity>
			(
				title: "Households",
				iconUri: "Data.AiderHousehold",
				columns: new List<Column> ()
			);

			yield return Database.Create<AiderPersonEntity, AiderPersonEntity>
			(
				title: "Persons",
				iconUri: "Base.AiderPerson",
				columns: new List<Column> ()
				{
					Column.Create<AiderPersonEntity, string>
					(
						title: "FirstName",
						name: "FirstName",
						type: ColumnType.String,
						hidden: false,
						sortable: true,
						sortOrder: null,
						lambdaExpression: x => x.eCH_Person.PersonFirstNames
					),
					Column.Create<AiderPersonEntity, string>
					(
						title: "LastName",
						name: "LastName",
						type: ColumnType.String,
						hidden: false,
						sortable: true,
						sortOrder: null,
						lambdaExpression: x => x.eCH_Person.PersonOfficialName
					),
				}
			);

			yield return Database.Create<AiderPersonRelationshipEntity, AiderPersonRelationshipEntity>
			(
				title: "Relationships",
				iconUri: "Base.AiderPersonRelationship",
				columns: new List<Column> ()
			);
		}


		private IEnumerable<Database> CreateCoreDatabases()
		{
			yield return Database.Create<CustomerEntity, CustomerEntity> 
			(
				title: "Clients",
				iconUri: "Base.Customer",
				columns: new List<Column> ()
			);

			yield return Database.Create<ArticleDefinitionEntity, ArticleDefinitionEntity> 
			(
				title: "Articles",
				iconUri: "Base.ArticleDefinition",
				columns: new List<Column> ()
			);

			yield return Database.Create<PersonGenderEntity, PersonGenderEntity> 
			(
				title: "Genres",
				iconUri: "Base.PersonGender",
				columns: new List<Column> ()
			);
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
