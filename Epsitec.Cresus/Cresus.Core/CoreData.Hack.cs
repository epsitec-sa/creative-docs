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
		public IEnumerable<AbstractPersonEntity> GetSamplePersons()
		{
			//	HACK: this method will soon be replaced by repositories

			if (this.samplePersons == null)
			{
				this.samplePersons = new List<AbstractPersonEntity> ();
				CoreData.CreateSamplePersons (this.samplePersons);
			}

			return this.samplePersons;
		}

		private static void CreateSamplePersons(List<AbstractPersonEntity> persons)
		{
			var context = EntityContext.Current;
			var country = context.CreateEmptyEntity<CountryEntity> ();
			
			country.Code = "CH";
			country.Name = "Suisse";

			var location1 = context.CreateEmptyEntity<LocationEntity> ();

			location1.Country = country;
			location1.Name = "Yverdon-les-Bains";
			location1.PostalCode = "1400";
			location1.Region = null;

			var street1 = context.CreateEmptyEntity<StreetEntity> ();

			street1.StreetName = "Ch. du Fontenay 3";
			street1.Complement = "2ème étage";

			var postbox1 = context.CreateEmptyEntity<PostBoxEntity> ();

			postbox1.Number = "Case postale 1234";

			var address1 = context.CreateEmptyEntity<AddressEntity> ();

			address1.Location = location1;
			address1.Street = street1;
			address1.PostBox = postbox1;


			var comment1 = context.CreateEmptyEntity<CommentEntity> ();

			comment1.Text = "Bureaux ouverts de 9h-12h et 14h-16h30";

			var role1 = context.CreateEmptyEntity<ContactRoleEntity> ();

			role1.Name = "facturation";

			var role2 = context.CreateEmptyEntity<ContactRoleEntity> ();

			role2.Name = "professionnel";

			var role3 = context.CreateEmptyEntity<ContactRoleEntity> ();

			role3.Name = "privé";

			var contact1 = context.CreateEmptyEntity<MailContactEntity> ();

			var enterprise = context.CreateEmptyEntity<LegalPersonEntity> ();

			enterprise.Complement = "Logiciels de gestion Crésus";
			enterprise.Name = "Epsitec SA";
			enterprise.Contacts.Add (contact1);

			var title1 = context.CreateEmptyEntity<PersonTitleEntity> ();
			
			title1.Name = "Monsieur";
			title1.ShortName = "M.";

			var person1 = context.CreateEmptyEntity<NaturalPersonEntity> ();

			person1.BirthDate = new Common.Types.Date (day: 11, month: 2, year: 1972);
			person1.Firstname = "Pierre";
			person1.Lastname = "Arnaud";
			person1.Title = title1;
			person1.Contacts.Add (contact1);
			
			contact1.Address = address1;
			contact1.Complement = "Direction";
			contact1.Comments.Add (comment1);
            contact1.Roles.Add (role1);
			contact1.LegalPerson = enterprise;
            contact1.NaturalPerson = person1;

			var telecomType1 = context.CreateEmptyEntity<TelecomTypeEntity> ();

			telecomType1.Code = "fixnet";
			telecomType1.Name = "Téléphone fixe";

			var telecomType2 = context.CreateEmptyEntity<TelecomTypeEntity> ();

			telecomType2.Code = "mobile";
			telecomType2.Name = "Téléphone mobile";

			var telecom1 = context.CreateEmptyEntity<TelecomContactEntity> ();

			telecom1.TelecomType = telecomType1;
			telecom1.LegalPerson = enterprise;
			telecom1.Number = "+41 848 27 37 87";
			telecom1.Roles.Add (role1);
			telecom1.Roles.Add (role2);

			var telecom2 = context.CreateEmptyEntity<TelecomContactEntity> ();

			telecom2.TelecomType = telecomType2;
			telecom2.NaturalPerson = person1;
			telecom2.Number = "+41 79 555 55 55";
			telecom2.Roles.Add (role3);

			person1.Contacts.Add (telecom2);

			var telecom3 = context.CreateEmptyEntity<TelecomContactEntity> ();

			telecom3.TelecomType = telecomType1;
			telecom3.LegalPerson = enterprise;
			telecom3.NaturalPerson = person1;
			telecom3.Number = "+41 24 425 08 30";
			telecom3.Roles.Add (role2);

			enterprise.Contacts.Add (telecom3);
			person1.Contacts.Add (telecom3);
			
			persons.Add (person1);
			persons.Add (enterprise);
		}

		List<AbstractPersonEntity> samplePersons;
	}
}
