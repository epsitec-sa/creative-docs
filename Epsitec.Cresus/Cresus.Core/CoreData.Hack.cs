//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.EntityEngine;

namespace Epsitec.Cresus.Core
{
	public sealed partial class CoreData
	{
		public IEnumerable<CountryEntity> GetCountries()
		{
			//	HACK: this method will soon be replaced by repositories

			if (this.sampleCountries == null)
			{
				this.sampleCountries = new List<CountryEntity> ();
				CoreData.CreateSampleCountries (this.sampleCountries);
			}

			return this.sampleCountries;
		}

		public IEnumerable<LocationEntity> GetLocations()
		{
			//	HACK: this method will soon be replaced by repositories

			if (this.sampleLocations == null)
			{
				IEnumerable<CountryEntity> countries = this.GetCountries ();

				this.sampleLocations = new List<LocationEntity> ();
				CoreData.CreateSampleLocations (this.sampleLocations, countries);
			}

			return this.sampleLocations;
		}

		public IEnumerable<ContactRoleEntity> GetRoles()
		{
			//	HACK: this method will soon be replaced by repositories

			if (this.samplePersons == null)
			{
				this.sampleRoles = new List<ContactRoleEntity> ();
				CoreData.CreateSampleRoles (this.sampleRoles);
			}

			return this.sampleRoles;
		}

		public IEnumerable<UriSchemeEntity> GetUriSchemes()
		{
			if (this.uriSchemes == null)
            {
				this.uriSchemes = new List<UriSchemeEntity> ();
				
				var uriScheme1 = EntityContext.Current.CreateEmptyEntity<UriSchemeEntity> ();

				using (uriScheme1.DefineOriginalValues ())
				{
					uriScheme1.Code = "mailto";
					uriScheme1.Name = "Mail";
				}

				this.uriSchemes.Add (uriScheme1);
			}

			return this.uriSchemes;
		}

		public UriSchemeEntity GetUriScheme(string code)
		{
			return this.GetUriSchemes ().Where (x => x.Code == code).FirstOrDefault ();
		}

		public IEnumerable<TelecomTypeEntity> GetTelecomTypes()
		{
			if (this.telecomTypes == null)
            {
				this.telecomTypes = new List<TelecomTypeEntity> ();

				var telecomType1 = EntityContext.Current.CreateEmptyEntity<TelecomTypeEntity> ();
				var telecomType2 = EntityContext.Current.CreateEmptyEntity<TelecomTypeEntity> ();
				var telecomType3 = EntityContext.Current.CreateEmptyEntity<TelecomTypeEntity> ();

				using (telecomType1.DefineOriginalValues ())
				{
					telecomType1.Code = "fixnet";
					telecomType1.Name = "Téléphone fixe";
				}

				using (telecomType2.DefineOriginalValues ())
				{
					telecomType2.Code = "mobile";
					telecomType2.Name = "Téléphone mobile";
				}

				using (telecomType3.DefineOriginalValues ())
				{
					telecomType3.Code = "fixnet";
					telecomType3.Name = "Téléphone fixe";
				}

				this.telecomTypes.Add (telecomType1);
				this.telecomTypes.Add (telecomType2);
				this.telecomTypes.Add (telecomType3);
			}

			return this.telecomTypes;
		}

		public IEnumerable<AbstractPersonEntity> GetAbstractPersons()
		{
			//	HACK: this method will soon be replaced by repositories

			if (this.samplePersons == null)
			{
				IEnumerable<ContactRoleEntity> roles = this.GetRoles ();
				IEnumerable<LocationEntity> locations = this.GetLocations ();

				this.samplePersons = new List<AbstractPersonEntity> ();
				this.CreateSamplePersons (this.samplePersons);
			}

			return this.samplePersons;
		}


		public IEnumerable<Entities.PersonTitleEntity> GetTitles()
		{
			if (this.sampleTitles == null)
			{
				this.sampleTitles = new List<PersonTitleEntity> ();
				CoreData.CreateSampleTitles (this.sampleTitles);
			}

			return this.sampleTitles;
		}

		public IEnumerable<Entities.PersonGenderEntity> GetGenders()
		{
			if (this.sampleGenders == null)
			{
				this.sampleGenders = new List<PersonGenderEntity> ();
				CoreData.CreateSampleGenders (this.sampleGenders);
			}

			return this.sampleGenders;
		}

		private static void CreateSampleCountries(List<CountryEntity> countries)
		{
			for (int i = 0; i < CoreData.countries.Length; i+=2)
			{
				var entity = new CountryEntity ();

				entity.Code = CoreData.countries[i+0];
				entity.Name = CoreData.countries[i+1];

				countries.Add (entity);
			}
		}

		private static void CreateSampleLocations(List<LocationEntity> locations, IEnumerable<CountryEntity> countries)
		{
			CountryEntity swiss = null;

			foreach (var country in countries)
			{
				if (country.Code == "CH")
				{
					swiss = country;
					break;
				}
			}

			for (int i = 0; i < CoreData.locations.Length; i+=2)
			{
				var entity = new LocationEntity ();

				entity.Country    = swiss;
				entity.PostalCode = CoreData.locations[i+0];
				entity.Name       = CoreData.locations[i+1];

				locations.Add (entity);
			}
		}

		private static void CreateSampleRoles(List<ContactRoleEntity> roles)
		{
			var context = EntityContext.Current;

			var role1 = context.CreateEmptyEntity<ContactRoleEntity> ();
			role1.Name = "professionnel";

			var role2 = context.CreateEmptyEntity<ContactRoleEntity> ();
			role2.Name = "commande";

			var role3 = context.CreateEmptyEntity<ContactRoleEntity> ();
			role3.Name = "livraison";

			var role4 = context.CreateEmptyEntity<ContactRoleEntity> ();
			role4.Name = "facturation";

			var role5 = context.CreateEmptyEntity<ContactRoleEntity> ();
			role5.Name = "privé";

			roles.Add (role1);
			roles.Add (role2);
			roles.Add (role3);
			roles.Add (role4);
			roles.Add (role5);
		}

		private static void CreateSampleTitles(List<PersonTitleEntity> titles)
		{
			var context = EntityContext.Current;

			var title1 = context.CreateEmptyEntity<PersonTitleEntity> ();
			var title2 = context.CreateEmptyEntity<PersonTitleEntity> ();
			var title3 = context.CreateEmptyEntity<PersonTitleEntity> ();

			using (title1.DefineOriginalValues ())
			{
				title1.Name = "Monsieur";
				title1.ShortName = "M.";
			}

			using (title2.DefineOriginalValues ())
			{
				title2.Name = "Madame";
				title2.ShortName = "Mme";
			}

			using (title3.DefineOriginalValues ())
			{
				title3.Name = "Mademoiselle";
				title3.ShortName = "Mlle";
			}

			titles.Add (title1);
			titles.Add (title2);
			titles.Add (title3);
		}

		private static void CreateSampleGenders(List<PersonGenderEntity> genders)
		{
			var context = EntityContext.Current;

			var gender1 = context.CreateEmptyEntity<PersonGenderEntity> ();
			var gender2 = context.CreateEmptyEntity<PersonGenderEntity> ();

			using (gender1.DefineOriginalValues ())
			{
				gender1.Name = "Homme";
				gender1.Code = "♂";
			}

			using (gender2.DefineOriginalValues ())
			{
				gender2.Name = "Femme";
				gender2.Code = "♀";
			}

			genders.Add (gender1);
			genders.Add (gender2);
		}

		private void CreateSamplePersons(List<AbstractPersonEntity> persons)
		{
			var context = EntityContext.Current;

			var role1 = this.GetRoles ().Where (x => x.Name == "facturation").First ();
			var role2 = this.GetRoles ().Where (x => x.Name == "professionnel").First ();
			var role3 = this.GetRoles ().Where (x => x.Name == "privé").First ();

			var telecomType1 = this.GetTelecomTypes ().Where (x => x.Code == "fixnet").First ();
			var telecomType2 = this.GetTelecomTypes ().Where (x => x.Code == "mobile").First ();
			var telecomType3 = this.GetTelecomTypes ().Where (x => x.Code == "fixnet").First ();
			
			var uriScheme1 = this.GetUriSchemes ().Where (x => x.Code == "mailto").First ();
			
			var yverdon = this.GetLocations ().Where (x => x.PostalCode == "1400").First ();

			var street1 = context.CreateEmptyEntity<StreetEntity> ();
			var street2 = context.CreateEmptyEntity<StreetEntity> ();
			var postbox1 = context.CreateEmptyEntity<PostBoxEntity> ();
			var address1 = context.CreateEmptyEntity<AddressEntity> ();
			var address2 = context.CreateEmptyEntity<AddressEntity> ();
			var comment1 = context.CreateEmptyEntity<CommentEntity> ();
			var contact1 = context.CreateEmptyEntity<MailContactEntity> ();
			var contact2 = context.CreateEmptyEntity<MailContactEntity> ();
			var title1 = this.GetTitles ().Where (x => x.ShortName == "M.").First ();
			var person1 = context.CreateEmptyEntity<NaturalPersonEntity> ();
			var person2 = context.CreateEmptyEntity<NaturalPersonEntity> ();
			var enterprise = context.CreateEmptyEntity<LegalPersonEntity> ();
			
			using (street1.DefineOriginalValues ())
			{
				street1.StreetName = "Ch. du Fontenay 3";
				street1.Complement = "2ème étage";
			}

			using (street2.DefineOriginalValues ())
			{
				street2.StreetName = "Ch. du Fontenay 6";
			}

			using (postbox1.DefineOriginalValues ())
			{
				postbox1.Number = "Case postale 1234";
			}

			using (address1.DefineOriginalValues ())
			{
				address1.Location = yverdon;
				address1.Street = street1;
				address1.PostBox = postbox1;
			}

			using (address2.DefineOriginalValues ())
			{
				address2.Location = yverdon;
				address2.Street = street2;
			}

			using (comment1.DefineOriginalValues ())
			{
				comment1.Text = "Bureaux ouverts de 9h-12h et 14h-16h30";
			}

			using (contact1.DefineOriginalValues ())
			{
				contact1.Address = address1;
				contact1.Complement = "Direction";
				contact1.Comments.Add (comment1);
				contact1.Roles.Add (role1);
				contact1.LegalPerson = enterprise;
				contact1.NaturalPerson = person1;
			}

			using (contact2.DefineOriginalValues ())
			{
				contact2.Address = address2;
				contact2.Roles.Add (role3);
				contact2.NaturalPerson = person1;
			}

			var telecom1 = context.CreateEmptyEntity<TelecomContactEntity> ();
			var telecom2 = context.CreateEmptyEntity<TelecomContactEntity> ();
			var telecom3 = context.CreateEmptyEntity<TelecomContactEntity> ();

			using (telecom1.DefineOriginalValues ())
			{
				telecom1.TelecomType = telecomType1;
				telecom1.LegalPerson = enterprise;
				telecom1.Number = "+41 848 27 37 87";
				telecom1.Roles.Add (role1);
				telecom1.Roles.Add (role2);
			}

			using (telecom2.DefineOriginalValues ())
			{
				telecom2.TelecomType = telecomType2;
				telecom2.NaturalPerson = person1;
				telecom2.Number = "+41 79 555 55 55";
				telecom2.Roles.Add (role3);
			}

			using (telecom3.DefineOriginalValues ())
			{
				telecom3.TelecomType = telecomType3;
				telecom3.LegalPerson = enterprise;
				telecom3.NaturalPerson = person1;
				telecom3.Number = "+41 24 425 08 30";
				telecom3.Roles.Add (role2);
			}


			var uri1 = context.CreateEmptyEntity<UriContactEntity> ();
			var uri2 = context.CreateEmptyEntity<UriContactEntity> ();
			var uri3 = context.CreateEmptyEntity<UriContactEntity> ();

			using (uri1.DefineOriginalValues ())
			{
				uri1.LegalPerson = enterprise;
				uri1.Uri = "epsitec@epsitec.ch";
				uri1.UriScheme = uriScheme1;
				uri1.Roles.Add (role2);
				uri1.Roles.Add (role3);
			}

			using (enterprise.DefineOriginalValues ())
			{
				enterprise.Complement = "Logiciels de gestion Crésus";
				enterprise.Name = "Epsitec SA";
				enterprise.Contacts.Add (contact1);
				enterprise.Contacts.Add (telecom3);
				enterprise.Contacts.Add (uri1);
			}


			using (uri2.DefineOriginalValues ())
			{
				uri2.LegalPerson = enterprise;
				uri2.Uri = "arnaud@epsitec.ch";
				uri2.UriScheme = uriScheme1;
				uri2.Roles.Add (role3);
			}

			using (uri3.DefineOriginalValues ())
			{
				uri3.LegalPerson = enterprise;
				uri3.Uri = "perre.arnaud@opac.ch";
				uri3.UriScheme = uriScheme1;
				uri3.Roles.Add (role3);
			}

			using (person1.DefineOriginalValues ())
			{
				person1.BirthDate = new Common.Types.Date (day: 11, month: 2, year: 1972);
				person1.Firstname = "Pierre";
				person1.Lastname = "Arnaud";
				person1.Title = title1;
				person1.Contacts.Add (contact1);
				person1.Contacts.Add (contact2);
				person1.Contacts.Add (telecom1);
				person1.Contacts.Add (telecom2);
				person1.Contacts.Add (telecom3);
				person1.Contacts.Add (uri1);
				person1.Contacts.Add (uri2);
				person1.Contacts.Add (uri3);
			}

			using (person2.DefineOriginalValues ())
			{
				person2.Firstname = "Daniel";
				person2.Lastname  = "Roux";
			}

			persons.Add (person1);
			persons.Add (person2);
			persons.Add (enterprise);
		}

		List<CountryEntity> sampleCountries;
		List<LocationEntity> sampleLocations;
		List<ContactRoleEntity> sampleRoles;
		List<AbstractPersonEntity> samplePersons;
		List<TelecomTypeEntity> telecomTypes;
		List<UriSchemeEntity> uriSchemes;
		List<PersonTitleEntity> sampleTitles;
		List<PersonGenderEntity> sampleGenders;
	}
}
