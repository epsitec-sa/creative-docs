//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Aider.Entities;
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
					if (contact.Household.IsNotNull ())
					{
						//wall.AddBrick ()
						//    .Title (Resources.Text ("Ménage associé"))
						//    .Text (contact.Household.DisplayName)
						//    .Icon ("Data.AiderHousehold")
						//    .Attribute (BrickMode.SpecialController2);

						if (contact.Address.IsNotNull ())
						{
							wall.AddBrick ()
								.Title (Resources.Text ("Adresse de domicile"))
								.Text (contact.Address.GetSummary ())
								.Icon ("Data.AiderAddress")
								.Attribute (BrickMode.SpecialController1);
						}

						if (contact.Household.Members.Count > 1)
						{
							wall.AddBrick (x => x.Household.Members)
								.Title (Resources.Text ("Membres du ménage"))
								.Icon ("Data.AiderPersons")
								.Attribute (BrickMode.HideAddButton)
								.Attribute (BrickMode.HideRemoveButton)
								.Attribute (BrickMode.AutoGroup)
								.Template ()
									.Text (x => x.GetCompactSummary ())
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
							.Attribute (BrickMode.SpecialController1);
					}
					break;

				case Enumerations.ContactType.Legal:
					//	TODO: ...
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

				switch (detail.AddressType)
				{
					case Enumerations.AddressType.Default:
					case Enumerations.AddressType.Other:
						contactInfoPrivate = TextFormatter.FormatText (contactInfoPrivate, "\n", phone, "\n", email);
						break;

					case Enumerations.AddressType.Professional:
						contactInfoProf = TextFormatter.FormatText (contactInfoProf, "\n", phone, "\n", email);
						break;

					case Enumerations.AddressType.Secondary:
						contactInfoSecondary = TextFormatter.FormatText (contactInfoSecondary, "\n", phone, "\n", email);
						break;
				}
			}

			return TextFormatter.FormatText (contactInfoPrivate,
				"\nProfessionnel:\n~", contactInfoProf,
				"\nDomicile secondaire:\n~", contactInfoSecondary);
		}
	}
}
