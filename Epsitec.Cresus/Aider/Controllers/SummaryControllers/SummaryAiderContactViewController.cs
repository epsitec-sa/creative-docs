//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Aider.Controllers.EditionControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Entities;

using System.Linq;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderContactViewController : SummaryViewController<AiderContactEntity>
	{
		protected override void CreateBricks(BrickWall<AiderContactEntity> wall)
		{
			var contact = this.Entity;
			var household = this.Entity.Household;

			if ((contact.Person.IsNull ()) &&
				(contact.LegalPerson.IsNull ()))
			{
				wall.AddBrick ()
					.EnableAction (0);
			}

			FormattedText contactSummary = this.GetPersonContactSummary (contact);

			//	TODO: add phone/...

			switch (this.Entity.ContactType)
			{
				case Enumerations.ContactType.PersonHousehold:
					if (contact.Person.IsNotNull ())
					{
						wall.AddBrick (x => x.Person)
							.Icon (contact.Person.GetIconName ("Data"))
							.Text (contactSummary)
							.Attribute (BrickMode.DefaultToSummarySubView);
					}
					
					if (household.IsNotNull ())
					{
						if (contact.Address.IsNotNull ())
						{
							wall.AddBrick ()
								.Title (Resources.Text ("Adresse de domicile"))
								.Text (contact.Address.GetSummary ())
								.Icon ("Data.AiderAddress")
								.WithSpecialController (typeof (EditionAiderContactViewController1Address));
						}

						if (household.Members.Count > 1)
						{
							wall.AddBrick (x => x.Household.Members)
								.Title (Resources.Text ("Membres du ménage"))
								.Icon ("Data.AiderPersons")
								.Attribute (BrickMode.HideAddButton)
								.Attribute (BrickMode.HideRemoveButton)
								.Attribute (BrickMode.AutoGroup)
								.Template ()
									.Text (x => x.GetCompactSummary (household))
								.End ()
								.Attribute (BrickMode.DefaultToSummarySubView);
						}
					}
					break;

				case Enumerations.ContactType.PersonAddress:
					if (contact.Person.IsNotNull ())
					{
						wall.AddBrick (x => x.Person)
							.Icon (contact.Person.GetIconName ("Data"))
							.Text (contactSummary)
							.Attribute (BrickMode.DefaultToSummarySubView);
					}
					if (contact.Address.IsNotNull ())
					{
						wall.AddBrick ()
							.Title (TextFormatter.FormatText (contact.AddressType))
							.Text (contact.Address.GetSummary ())
							.Icon ("Data.AiderAddress")
							.WithSpecialController (typeof (EditionAiderContactViewController1Address));
					}
					break;

				case Enumerations.ContactType.Legal:
					if (contact.LegalPerson.IsNotNull ())
					{
						wall.AddBrick (x => x.LegalPerson)
							/*.Icon ("Data.LegalPerson")*/;

						wall.AddBrick ()
							.Title ("Adresse de base")
							.Icon ("Data.AiderAddress")
							.Text (x => x.LegalPerson.Address.GetSummary ())
							.WithSpecialController (typeof (EditionAiderContactViewController1Address));
					}
					
					var contactPersonSummary = string.IsNullOrEmpty (contact.PersonFullName)
						? FormattedText.Empty
						: TextFormatter.FormatText (contact.PersonMrMrs.GetShortText (), contact.PersonFullName);

					wall.AddBrick ()
						.Icon (AiderPersonEntity.GetIconName ("Data", contact.PersonMrMrs, contact.LegalPerson.Language))
						.Title ("Personne de contact")
						.Text (contactPersonSummary)
						.WithSpecialController (typeof (EditionAiderContactViewController2LegalContact));
					
					if ((contact.Address.IsNotNull ()) &&
						(contact.Address != contact.LegalPerson.Address))
					{
						wall.AddBrick ()
							.Title ("Adresse de contact")
							.Text (contact.Address.GetSummary ())
							.Icon ("Data.AiderAddress")
							.WithSpecialController (typeof (EditionAiderContactViewController1Address));
					}
					break;

				default:
					break;
			}
		}

		private FormattedText GetPersonContactSummary(AiderContactEntity contact)
		{
			var text = contact.Person.GetCompactSummary ();

			var contactInfoPrivate   = text;
			var contactInfoProf      = FormattedText.Empty;
			var contactInfoSecondary = FormattedText.Empty;

			foreach (var detail in contact.Person.Contacts.Where (x => x.Address.IsNotNull ()))
			{
				var address = detail.Address;
				var phone   = address.GetPhoneSummary ();
				var email   = address.GetWebEmailSummary ();

				if ((phone.IsNullOrEmpty ()) &&
					(email.IsNullOrEmpty ()))
				{
					continue;
				}

				switch (detail.AddressType)
				{
					case Enumerations.AddressType.Default:
					case Enumerations.AddressType.Other:
						contactInfoPrivate = TextFormatter.FormatText (contactInfoPrivate, "\n", phone, "\n", email);
						break;

					case Enumerations.AddressType.Professional:
						if (contactInfoProf.IsNullOrEmpty ())
						{
							contactInfoProf = new FormattedText ("<hr/><b>Professionnel:</b>");
						}
						contactInfoProf = TextFormatter.FormatText (contactInfoProf, "\n", phone, "\n", email);
						break;

					case Enumerations.AddressType.Secondary:
						if (contactInfoSecondary.IsNullOrEmpty ())
						{
							contactInfoSecondary = new FormattedText ("<hr/><b>Domicile secondaire:</b>");
						}
						contactInfoSecondary = TextFormatter.FormatText (contactInfoSecondary, "\n", phone, "\n", email);
						break;
				}
			}

			return TextFormatter.FormatText (contactInfoPrivate, "\n", contactInfoProf, "\n", contactInfoSecondary);
		}
	}
}
