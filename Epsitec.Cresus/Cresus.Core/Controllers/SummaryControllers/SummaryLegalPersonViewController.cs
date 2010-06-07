//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryLegalPersonViewController : SummaryViewController<Entities.LegalPersonEntity>
	{
		public SummaryLegalPersonViewController(string name, Entities.LegalPersonEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI(TileContainer container)
		{
			var containerController = new TileContainerController (this, container);
			var data = containerController.DataItems;

			data.Add (
				new SummaryData
				{
					Name				= "LegalPerson",
					IconUri				= "Data.LegalPerson",
					Title				= UIBuilder.FormatText ("Personne morale"),
					CompactTitle		= UIBuilder.FormatText ("Personne"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText (x.Name)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText (x.Name)),
					EntityAccessor		= this.EntityGetter,
				});

			var template1 = new CollectionTemplate<Entities.MailContactEntity> ("MailContact")
				.DefineTitle (x => UIBuilder.FormatText ("Adresse", "(", string.Join (", ", x.Roles.Select (role => role.Name)), ")"))
				.DefineText (x => UIBuilder.FormatText (x.LegalPerson.Name, "\n", x.LegalPerson.Complement, "\n", x.Complement, "\n", x.Address.Street.StreetName, "\n", x.Address.Street.Complement, "\n", x.Address.PostBox.Number, "\n", x.Address.Location.Country.Code, "~-", x.Address.Location.PostalCode, x.Address.Location.Name))
				.DefineCompactText (x => UIBuilder.FormatText (x.Address.Street.StreetName, "~,", x.Address.Location.PostalCode, x.Address.Location.Name));

			var template2 = new CollectionTemplate<Entities.TelecomContactEntity> ("TelecomContact")
				.DefineTitle (x => UIBuilder.FormatText (x.TelecomType.Name))
				.DefineText (x => UIBuilder.FormatText (x.Number, "(", string.Join (", ", x.Roles.Select (role => role.Name)), ")"))
				.DefineCompactText (x => UIBuilder.FormatText (x.Number, "(", x.TelecomType.Name, ")"));

			var template3 = new CollectionTemplate<Entities.UriContactEntity> ("UriContact", filter: x => x.UriScheme.Code == "mailto")
				.DefineText (x => UIBuilder.FormatText (x.Uri, "(", string.Join (", ", x.Roles.Select (role => role.Name)), ")"))
				.DefineCompactText (x => UIBuilder.FormatText (x.Uri))
				.DefineSetupItem (x => x.UriScheme = CoreProgram.Application.Data.GetUriScheme ("mailto"));

			data.Add (
				new SummaryData
				{
					Name		 = "MailContact",
					IconUri		 = "Data.Mail",
					Title		 = UIBuilder.FormatText ("Adresse"),
					CompactTitle = UIBuilder.FormatText ("Adresse"),
					Text		 = UIBuilder.FormatText ("<i>vide</i>")
				});

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

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Contacts, template1));
			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Contacts, template2));
			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Contacts, template3));

			containerController.GenerateTiles ();
		}
	}
}
