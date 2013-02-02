﻿using Epsitec.Aider.Data.ECh;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Tools;

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Library.UI;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections.Generic;

using System.IO;

using System.Linq;


namespace Aider.Tests.Vs
{


	[TestClass]
	public sealed class UnitTestEChDataImporter
	{


		[TestMethod]
		public void Test()
		{
			var hack = new Epsitec.Data.Platform.Entities.MatchStreetEntity ();

			CoreData.ForceDatabaseCreationRequest = true;

			var lines = CoreContext.ReadCoreContextSettingsFile ().ToList ();
			CoreContext.ParseOptionalSettingsFile (lines);
			CoreContext.StartAsInteractive ();
			Services.Initialize ();

			using (var app = new CoreApplication ())
			{
				app.SetupApplication ();

				var coreDataManager = new CoreDataManager (app.Data);
				
				var inputFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\eerv-2011-11-29.xml");
				var eChReportedPersons = EChDataLoader.Load (inputFile);
				EChDataImporter.Import (coreDataManager, eChReportedPersons);

				Services.ShutDown ();
			}
		}


		[TestMethod]
		public void GenerateDatabase()
		{
			var hack = new Epsitec.Data.Platform.Entities.MatchStreetEntity ();

			CoreData.ForceDatabaseCreationRequest = true;

			var lines = CoreContext.ReadCoreContextSettingsFile ().ToList ();
			CoreContext.ParseOptionalSettingsFile (lines);
			CoreContext.StartAsInteractive ();
			Services.Initialize ();

			using (var app = new CoreApplication ())
			{
				app.SetupApplication ();

				var coreDataManager = new CoreDataManager (app.Data);
				
				var inputFile = new FileInfo (@"S:\Epsitec.Cresus\App.Aider\Samples\eerv-2011-11-29.xml");
				var eChReportedPersons = EChDataLoader.Load (inputFile, 50).ToList ();
				
				EChDataImporter.Import (coreDataManager, eChReportedPersons);

				this.AddStuffToDatabase (app.Data);

				Services.ShutDown ();
			}

			File.Copy (@"C:\ProgramData\Epsitec\Firebird Databases\UTD_AIDER.FIREBIRD", @"C:\ProgramData\Epsitec\Firebird Databases\AIDER.FIREBIRD", true);
		}


		private void AddStuffToDatabase(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var countries = this.GenerateCountries (businessContext).ToList ();
				var towns = this.GenerateTowns (businessContext, countries).ToList ();

				foreach (var aiderPerson in businessContext.GetAllEntities<AiderPersonEntity> ())
				{
					this.AddStuffToPerson (businessContext, towns, aiderPerson);
				}
			}
		}


		private IEnumerable<AiderCountryEntity> GenerateCountries(BusinessContext businessContext)
		{
			yield return this.GenerateCountry (businessContext, "Italie", "IT");
			yield return this.GenerateCountry (businessContext, "France", "FR");
			yield return this.GenerateCountry (businessContext, "Allemagne", "DE");
			yield return this.GenerateCountry (businessContext, "Autriche", "AU");
		}


		private AiderCountryEntity GenerateCountry(BusinessContext businessContext, string name, string isoCode)
		{
			var country = businessContext.CreateEntity<AiderCountryEntity> ();

			country.Name = name;
			country.IsoCode = isoCode;

			return country;
		}


		private IEnumerable<AiderTownEntity> GenerateTowns(BusinessContext businessContext, IEnumerable<AiderCountryEntity> countries)
		{
			foreach (var country in countries)
			{
				foreach (var town in this.GenerateTowns (businessContext, country))
				{
					yield return town;
				}
			}
		}


		private IEnumerable<AiderTownEntity> GenerateTowns(BusinessContext businessContext, AiderCountryEntity country)
		{
			yield return this.GenerateTown (businessContext, country, "Mouette-les-Bains", "1234");
			yield return this.GenerateTown (businessContext, country, "Canard-ville", "4321");
			yield return this.GenerateTown (businessContext, country, "Vers-chez-les-Foulques", "5678");
			yield return this.GenerateTown (businessContext, country, "Goéland-sur-mer", "8765");
			yield return this.GenerateTown (businessContext, country, "Pinguin-les-Mottes", "2468");
		}


		private AiderTownEntity GenerateTown(BusinessContext businessContext, AiderCountryEntity country, string name, string zipCode)
		{
			var town = businessContext.CreateEntity<AiderTownEntity> ();

			town.Country = country;
			town.Name = name;
			town.ZipCode = zipCode;

			return town;
		}


		private void AddStuffToPerson(BusinessContext businessContext, List<AiderTownEntity> towns, AiderPersonEntity aiderPerson)
		{
			var nbAddresses = dice.Next (0, 5);

			for (int i = 0; i < nbAddresses; i++)
			{
				var contactType = this.GetRandomEnum<AddressType> ();
				var contact = AiderContactEntity.Create (businessContext, aiderPerson, contactType);

				this.GenerateAddress (contact.Address, towns, false);
			}

			if (dice.NextDouble () > 0.1)
			{
				aiderPerson.MrMrs = this.GetHonorific (aiderPerson);
			}

			if (dice.NextDouble () > 0.8)
			{
				aiderPerson.eCH_Person.PersonDateOfDeath = this.GetRandomDate ();
			}

			if (dice.NextDouble () > 0.1)
			{
				aiderPerson.Confession = this.GetRandomEnum<PersonConfession> ();
			}

			if (dice.NextDouble () > 0.5)
			{
				aiderPerson.Title = this.GetRandomTitle (aiderPerson.eCH_Person.PersonSex);
			}

			if (dice.NextDouble () > 0.5)
			{
				aiderPerson.Profession = this.GetRandomProfession ();
			}

			aiderPerson.Language = this.GetRandomEnum<Language> ();
		}


		private PersonMrMrs GetHonorific(AiderPersonEntity aiderPerson)
		{
			switch (aiderPerson.eCH_Person.PersonSex)
			{
				case PersonSex.Female:

					if (dice.NextDouble () > 0.5)
					{
						return PersonMrMrs.Madame;
					}
					else
					{
						return PersonMrMrs.Mademoiselle;
					}

				case PersonSex.Male:
					return PersonMrMrs.Monsieur;

				case PersonSex.Unknown:
					return PersonMrMrs.None;

				default:
					throw new NotImplementedException ();
			}
		}


		private void GenerateAddress(AiderAddressEntity address, List<AiderTownEntity> towns, bool forceMailAddress)
		{
			bool isEmpty = true;

			while (isEmpty)
			{
				if (forceMailAddress || dice.NextDouble () > 0.5)
				{
					var streetPrefix = this.GetRandomElement ("Rue du", "Chemin du", "Place du", "Avenue du");
					var street = this.GetRandomElement ("pécheur barbu", "marin chauve", "bateau ivre", "gallion espagnol", "petit navire");

					address.Street = streetPrefix + " " + street;
					address.HouseNumber = dice.Next (1, 1000);
					address.Town = towns.GetRandomElement ();

					isEmpty = false;
				}

				if (dice.NextDouble () > 0.5)
				{
					address.Phone1 = this.GetRandomPhone ();

					isEmpty = false;
				}

				if (dice.NextDouble () > 0.5)
				{
					address.Phone2 = this.GetRandomPhone ();

					isEmpty = false;
				}

				if (dice.NextDouble () > 0.5)
				{
					address.Mobile = this.GetRandomPhone ();

					isEmpty = false;
				}

				if (dice.NextDouble () > 0.5)
				{
					address.Fax = this.GetRandomPhone ();

					isEmpty = false;
				}

				if (dice.NextDouble () > 0.5)
				{
					var id = this.GetRandomElement ("fred", "albert", "georges", "tom", "brad", "angelina");
					var hostname = this.GetRandomHostname ();

					var email = id + "@" + hostname;

					address.Email = email;

					isEmpty = false;
				}

				if (dice.NextDouble () > 0.5)
				{
					var website = "http://www." + this.GetRandomHostname ();

					address.Web = website;

					isEmpty = false;
				}
			}
		}


		private string GetRandomHostname()
		{
			var domain = this.GetRandomElement ("coucou", "blabla", "bonbon", "salut");
			var root = this.GetRandomElement ("com", "org", "ch", "fr", "it", "de", "au");

			return domain + "." + root;
		}



		private string GetRandomPhone()
		{
			var part1 = dice.Next (100, 1000).ToString ("000");
			var part2 = dice.Next (0, 1000).ToString ("000");
			var part3 = dice.Next (0, 100).ToString ("00");
			var part4 = dice.Next (0, 100).ToString ("00");

			return part1 + " " + part2 + " " + part3 + " " + part4;
		}


		private Date GetRandomDate()
		{
			Date? date = null;

			while (!date.HasValue)
			{
				int day = dice.Next (1, 32);
				int month = dice.Next (1, 12);
				int year = dice.Next (1900, 2100);

				try
				{
					date = new Date (year, month, day);
				}
				catch
				{
				}
			}

			return date.Value;
		}


		private string GetRandomTitle(PersonSex sex)
		{
			switch (sex)
			{
				case PersonSex.Female:
					return this.GetRandomElement ("Juge", "Ministre", "Mère");

				case PersonSex.Male:
					return this.GetRandomElement ("Pasteur", "Docteur", "Capitaine");

				case PersonSex.Unknown:
					return this.GetRandomElement ("Directeur", "Pasteur", "Ministre");

				default:
					throw new NotImplementedException ();
			}
		}


		private string GetRandomProfession()
		{
			return this.GetRandomElement ("Boucher", "Boulanger", "Informaticien", "Facteur", "Pasteur");
		}


		private string GetRandomElement(params string[] elements)
		{
			return new List<string> (elements).GetRandomElement ();
		}


		private T GetRandomEnum<T>()
		{
			return Enum.GetValues (typeof (T)).Cast<T> ().ToList ().GetRandomElement ();
		}


		private readonly Random dice = new Random ();


	}


}
