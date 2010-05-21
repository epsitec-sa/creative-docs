//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryNaturalPersonViewController : SummaryAbstractPersonViewController<Entities.NaturalPersonEntity>
	{
		public SummaryNaturalPersonViewController(string name, Entities.NaturalPersonEntity entity)
			: base (name, entity)
		{
		}


		public override void CreateUI(Common.Widgets.Widget container)
		{
#if false
			base.CreateUI (container);
#else

			var builder = new UIBuilder (container, this);
			var items   = new List<SummaryData> ();


			items.Add (
				new SummaryData
				{
					Rank				= 1000,
					Name				= "NaturalPerson",
					IconUri				= "Data.NaturalPerson",
					Title				= new FormattedText ("Personne physique"),
					CompactTitle		= new FormattedText ("Personne"),
					TextAccessor		= Accessor.Create (this.Entity, x => UIBuilder.FormatText (x.Title.Name, "\n", x.Firstname, x.Lastname, "(", x.Gender.Name, ")", "\n", x.BirthDate)),
					CompactTextAccessor = Accessor.Create (this.Entity, x => UIBuilder.FormatText (x.Title.ShortName, x.Firstname, x.Lastname)),
					EntityAccessor		= () => this.Entity,
				});


			var template1 = new CollectionTemplate<Entities.MailContactEntity> ("MailContact")
				.DefineTitle		(x => UIBuilder.FormatText ("Adresse", "(", string.Join (", ", x.Roles.Select (role => role.Name)), ")"))
				.DefineText			(x => UIBuilder.FormatText (x.LegalPerson.Name, "\n", x.LegalPerson.Complement, "\n", x.Complement, "\n", x.Address.Street.StreetName, "\n", x.Address.Street.Complement, "\n", x.Address.PostBox.Number, "\n", x.Address.Location.Country.Code, "~-", x.Address.Location.PostalCode, x.Address.Location.Name))
				.DefineCompactText	(x => UIBuilder.FormatText (x.Address.Street.StreetName, "~,", x.Address.Location.PostalCode, x.Address.Location.Name));

			var template2 = new CollectionTemplate<Entities.TelecomContactEntity> ("TelecomContact")
				.DefineTitle		(x => UIBuilder.FormatText (x.TelecomType.Name))
				.DefineText			(x => UIBuilder.FormatText (x.Number, "(", string.Join (", ", x.Roles.Select (role => role.Name)), ")"))
				.DefineCompactText  (x => UIBuilder.FormatText (x.Number, "(", x.TelecomType.Name, ")"));

			var template3 = new CollectionTemplate<Entities.UriContactEntity> ("UriContact", x => x.UriScheme.Code == "mailto")
				.DefineText			(x => UIBuilder.FormatText (x.Uri, "(", string.Join (", ", x.Roles.Select (role => role.Name)), ")"))
				.DefineCompactText	(x => UIBuilder.FormatText (x.Uri));

			var accessor1 = CollectionAccessor.Create (this.Entity, x => x.Contacts, template1);
			var accessor2 = CollectionAccessor.Create (this.Entity, x => x.Contacts, template2);
			var accessor3 = CollectionAccessor.Create (this.Entity, x => x.Contacts, template3);

			items.Add (
				new SummaryData
				{
					Rank		 = 2000,
					Name		 = "MailContact.0",
					IconUri		 = "Data.Mail",
					Title		 = new FormattedText ("Adresse"),
					CompactTitle = new FormattedText ("Adresse"),
					Text		 = new FormattedText ("<i>vide</i>")
				});

			items.Add (
				new SummaryData
				{
					Rank		 = 3000,
					AutoGroup    = true,
					Name		 = "TelecomContact.0",
					IconUri		 = "Data.Telecom",
					Title		 = new FormattedText ("Téléphone"),
					CompactTitle = new FormattedText ("Téléphone"),
					Text		 = new FormattedText ("<i>vide</i>")
				});

			items.Add (
				new SummaryData
				{
					Rank		 = 4000,
					AutoGroup    = true,
					Name		 = "UriContact.0",
					IconUri		 = "Data.Uri",
					Title		 = new FormattedText ("E-Mail"),
					CompactTitle = new FormattedText ("E-Mail"),
					Text		 = new FormattedText ("<i>vide</i>")
				});

			items.AddRange (accessor1.Resolve ((name, index) => CollectionAccessor.FindTemplate (items, name, index)));
			items.AddRange (accessor2.Resolve ((name, index) => CollectionAccessor.FindTemplate (items, name, index)));
			items.AddRange (accessor3.Resolve ((name, index) => CollectionAccessor.FindTemplate (items, name, index)));

			builder.MapDataToTiles (items);
#endif
		}

		protected override void CreatePersonTile(UIBuilder builder)
		{
			var group = builder.CreateSummaryGroupingTile ("Data.NaturalPerson", "Personne physique");

			var accessor = new Accessors.NaturalPersonAccessor (this.Entity)
			{
				ViewControllerMode = ViewControllerMode.Edition
			};

			builder.CreateSummaryTile (group, accessor);
		}
	}
}
