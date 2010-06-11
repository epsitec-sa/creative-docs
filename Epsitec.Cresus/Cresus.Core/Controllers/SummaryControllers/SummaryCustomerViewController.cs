//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryCustomerViewController : SummaryViewController<Entities.CustomerEntity>
	{
		public SummaryCustomerViewController(string name, Entities.CustomerEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI(TileContainer container)
		{
			var containerController = new TileContainerController (this, container);
			var data = containerController.DataItems;

			this.CreateUICustomer (data);
			this.CreateUIPerson (data);
			this.CreateUISalesRepresentative (data);
			this.CreateUIDefaultAddress (data);
			this.CreateUIMailContacts (data);
			this.CreateUITelecomContacts (data);
			this.CreateUIUriContacts (data);

			containerController.GenerateTiles ();
		}


		private void CreateUICustomer(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "Customer",
					IconUri				= "Data.Customer",
					Title				= UIBuilder.FormatText ("Client"),
					CompactTitle		= UIBuilder.FormatText ("Client"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText (x.Id)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText (x.Id)),
					EntityAccessor		= this.EntityGetter,
				});
		}

		private void CreateUIPerson(SummaryDataItems data)
		{
			if (this.Entity.Person is Entities.NaturalPersonEntity)
			{
				System.Func<Entities.NaturalPersonEntity > entityGetter = () => this.Entity.Person as Entities.NaturalPersonEntity;

				data.Add (
					new SummaryData
					{
						Name				= "NaturalPerson",
						IconUri				= "Data.NaturalPerson",
						Title				= UIBuilder.FormatText ("Personne physique"),
						CompactTitle		= UIBuilder.FormatText ("Personne"),
						TextAccessor		= Accessor.Create (entityGetter, x => UIBuilder.FormatText (x.Title.Name, "\n", x.Firstname, x.Lastname, "(", x.Gender.Name, ")", "\n", x.BirthDate)),
						CompactTextAccessor = Accessor.Create (entityGetter, x => UIBuilder.FormatText (x.Title.ShortName, x.Firstname, x.Lastname)),
						EntityAccessor		= entityGetter,
					});
			}

			if (this.Entity.Person is Entities.LegalPersonEntity)
			{
				System.Func<Entities.LegalPersonEntity > entityGetter = () => this.Entity.Person as Entities.LegalPersonEntity;

				data.Add (
					new SummaryData
					{
						Name				= "LegalPerson",
						IconUri				= "Data.LegalPerson",
						Title				= UIBuilder.FormatText ("Personne morale"),
						CompactTitle		= UIBuilder.FormatText ("Personne"),
						TextAccessor		= Accessor.Create (entityGetter, x => UIBuilder.FormatText (x.Name)),
						CompactTextAccessor = Accessor.Create (entityGetter, x => UIBuilder.FormatText (x.Name)),
						EntityAccessor		= entityGetter,
					});
			}
		}

		private void CreateUISalesRepresentative(SummaryDataItems data)
		{
			if (this.Entity.SalesRepresentative == null)
			{
				data.Add (
					new SummaryData
					{
						Name		 = "SalesRepresentative",
						IconUri		 = "Data.NaturalPerson",
						Title		 = UIBuilder.FormatText ("Représentant"),
						CompactTitle = UIBuilder.FormatText ("Représentant"),
						Text		 = UIBuilder.FormatText ("<i>vide</i>")
					});
			}
			else
			{
				System.Func<Entities.NaturalPersonEntity > entityGetter = () => this.Entity.SalesRepresentative;

				data.Add (
					new SummaryData
					{
						Name				= "SalesRepresentative",
						IconUri				= "Data.NaturalPerson",
						Title				= UIBuilder.FormatText ("Représentant"),
						CompactTitle		= UIBuilder.FormatText ("Représentant"),
						TextAccessor		= Accessor.Create (entityGetter, x => UIBuilder.FormatText (x.Title.Name, "\n", x.Firstname, x.Lastname, "(", x.Gender.Name, ")", "\n", x.BirthDate)),
						CompactTextAccessor = Accessor.Create (entityGetter, x => UIBuilder.FormatText (x.Title.ShortName, x.Firstname, x.Lastname)),
						EntityAccessor		= entityGetter,
					});
			}
		}

		private void CreateUIDefaultAddress(SummaryDataItems data)
		{
			if (this.Entity.DefaultAddress == null)  // TODO: comment tester si l'adresse par défaut n'existe pas ?
			//?if (this.Entity.DefaultAddress.IsDefiningOriginalValues)  // TODO: comment tester si l'adresse par défaut n'existe pas ?
			{
				data.Add (
					new SummaryData
					{
						Name		 = "DefaultAddress",
						IconUri		 = "Data.Mail",
						Title		 = UIBuilder.FormatText ("Adresse par défaut du client"),
						CompactTitle = UIBuilder.FormatText ("Adresse par défaut"),
						Text		 = UIBuilder.FormatText ("<i>vide</i>")
					});
			}
			else
			{
				System.Func<Entities.AddressEntity > entityGetter = () => this.Entity.DefaultAddress;

				data.Add (
					new SummaryData
					{
						Name				= "DefaultAddress",
						IconUri				= "Data.Mail",
						Title				= UIBuilder.FormatText ("Adresse par défaut du client"),
						CompactTitle		= UIBuilder.FormatText ("Adresse par défaut"),
						TextAccessor		= Accessor.Create (entityGetter, x => UIBuilder.FormatText (x.Street.StreetName, "\n", x.Street.Complement, "\n", x.PostBox.Number, "\n", x.Location.Country.Code, "~-", x.Location.PostalCode, x.Location.Name)),
						CompactTextAccessor = Accessor.Create (entityGetter, x => UIBuilder.FormatText (x.Street.StreetName, "~,", x.Location.PostalCode, x.Location.Name)),
						EntityAccessor		= entityGetter,  // TODO: ce n'est pas ça...
					});
			}
		}

		private void CreateUIMailContacts(SummaryDataItems data)
		{
			var template = new CollectionTemplate<Entities.MailContactEntity> ("MailContact")
				.DefineTitle		(x => UIBuilder.FormatText ("Adresse", "(", string.Join (", ", x.Roles.Select (role => role.Name)), ")"))
				.DefineText			(x => UIBuilder.FormatText (x.LegalPerson.Name, "\n", x.LegalPerson.Complement, "\n", x.Complement, "\n", x.Address.Street.StreetName, "\n", x.Address.Street.Complement, "\n", x.Address.PostBox.Number, "\n", x.Address.Location.Country.Code, "~-", x.Address.Location.PostalCode, x.Address.Location.Name))
				.DefineCompactText	(x => UIBuilder.FormatText (x.Address.Street.StreetName, "~,", x.Address.Location.PostalCode, x.Address.Location.Name));

			data.Add (
				new SummaryData
				{
					Name		 = "MailContact",
					IconUri		 = "Data.Mail",
					Title		 = UIBuilder.FormatText ("Adresse"),
					CompactTitle = UIBuilder.FormatText ("Adresse"),
					Text		 = UIBuilder.FormatText ("<i>vide</i>")
				});

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Person.Contacts, template));
		}

		private void CreateUITelecomContacts(SummaryDataItems data)
		{
			var template = new CollectionTemplate<Entities.TelecomContactEntity> ("TelecomContact")
				.DefineTitle		(x => UIBuilder.FormatText (x.TelecomType.Name))
				.DefineText			(x => UIBuilder.FormatText (x.Number, "(", string.Join (", ", x.Roles.Select (role => role.Name)), ")"))
				.DefineCompactText  (x => UIBuilder.FormatText (x.Number, "(", x.TelecomType.Name, ")"));

			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "TelecomContact",
					IconUri		 = "Data.Telecom",
					Title		 = UIBuilder.FormatText ("Téléphone"),
					CompactTitle = UIBuilder.FormatText ("Téléphone"),
					Text		 = UIBuilder.FormatText ("<i>vide</i>")
				});

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Person.Contacts, template));
		}

		private void CreateUIUriContacts(SummaryDataItems data)
		{
			var template = new CollectionTemplate<Entities.UriContactEntity> ("UriContact", filter: x => x.UriScheme.Code == "mailto")
				.DefineText			(x => UIBuilder.FormatText (x.Uri, "(", string.Join (", ", x.Roles.Select (role => role.Name)), ")"))
				.DefineCompactText	(x => UIBuilder.FormatText (x.Uri))
				.DefineSetupItem	(x => x.UriScheme = CoreProgram.Application.Data.GetUriScheme ("mailto"));

			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "UriContact",
					IconUri		 = "Data.Uri",
					Title		 = UIBuilder.FormatText ("E-Mail"),
					CompactTitle = UIBuilder.FormatText ("E-Mail"),
					Text		 = UIBuilder.FormatText ("<i>vide</i>")
				});

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Person.Contacts, template));
		}
	}
}
