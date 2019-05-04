//	Copyright © 2013-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.EditionControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Override;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Entities;

using System.Linq;
using System.Collections.Generic;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderContactViewController : SummaryViewController<AiderContactEntity>
	{
		protected override void CreateBricks(BrickWall<AiderContactEntity> wall)
        {
            //	TODO: refactor this in a set of sub-controllers, just like SummaryAiderPersonWarningViewController
            var user = AiderUserManager.Current.AuthenticatedUser;
            var contact = this.Entity;
            var household = this.Entity.Household;

            if ((contact.Person.IsNull ()) &&
                (contact.LegalPerson.IsNull ()))
            {
                wall.AddBrick ()
                    .EnableActionMenu<ActionAiderContactViewController0CreatePerson> ();
            }


            if ((user.LoginName == "epsitec") &&
                (this.CheckAndFixBrokenContact (contact)))
            {
                return;
            }

            var personTitle    = SummaryAiderContactViewController.GetPersonTitle (contact);
            var contactSummary = SummaryAiderContactViewController.GetPersonContactSummary (contact);

            //	TODO: add phone/...

            switch (this.Entity.ContactType)
            {
                case Enumerations.ContactType.Deceased:
                    if (contact.Person.IsNotNull ())
                    {
                        wall.AddBrick (x => x.Person)
                            .Title (personTitle)
                            .Icon (contact.Person.GetIconName ("Data"))
                            .Text (contactSummary)
                            .EnableActionMenu<ActionAiderPersonViewController10AddToBag> ()
                            .Attribute (BrickMode.DefaultToSummarySubView);
                    }
                    break;

                case Enumerations.ContactType.PersonHousehold:
                    if (contact.Person.IsNotNull ())
                    {
                        if ((contact.Household.IsNull ()) &&
                            (this.HasUserPowerLevel (Cresus.Core.Business.UserManagement.UserPowerLevel.Administrator)))
                        {
                            wall.AddBrick ()
                                .Text ("Personne sans ménage !?")
                                .EnableActionButton<ActionAiderContactViewController6DeleteContact> ();
                        }

                        wall.AddBrick (x => x.Person)
                            .Title (personTitle)
                            .Icon (contact.Person.GetIconName ("Data"))
                            .Text (contactSummary)
                            .EnableActionMenu<ActionAiderPersonViewController10AddToBag> ()
                            .Attribute (BrickMode.DefaultToSummarySubView);
                    }

                    if (household.IsNotNull ())
                    {
                        if (contact.Address.IsNotNull ())
                        {
                            var html = contact.Address.GetHtmlForLocationWebServices (contact.DisplayName);

                            wall.AddBrick ()
                                .Title (new FormattedText (Resources.Text ("Adresse de domicile") + html))
                                .Text (contact.Address.GetSummary ())
                                .Icon ("Data.AiderAddress")
                                .EnableActionMenu<ActionAiderContactViewController3AddAddressToBag> ()
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
                                .EnableActionMenu<ActionAiderContactViewController2AddHouseholdMembersToBag> ()
                                .EnableActionMenu<ActionAiderContactViewController4RemoveMemberFromHousehold> ().IfTrue (household.Members.Count > 1)
                                .EnableActionMenu<ActionAiderContactViewController5ChangeHeadOfHousehold> ()
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
                            .Title (personTitle)
                            .Icon (contact.Person.GetIconName ("Data"))
                            .Text (contactSummary)
                            .EnableActionMenu<ActionAiderPersonViewController10AddToBag> ()
                            .Attribute (BrickMode.DefaultToSummarySubView);
                    }
                    if (this.HasUserPowerLevel (Cresus.Core.Business.UserManagement.UserPowerLevel.Administrator))
                    {
                        wall.AddBrick ()
                            .EnableActionButton<ActionAiderContactViewController6DeleteContact> ();
                    }
                    if (contact.Address.IsNotNull ())
                    {
                        if (contact.AddressType == AddressType.Confidential)
                        {
                            if (user.CanViewConfidentialAddress ())
                            {
                                wall.AddBrick ()
                                .Title (TextFormatter.FormatText (contact.AddressType))
                                .Text (contact.Address.GetSummary ())
                                .Icon ("Data.AiderAddress")
                                .EnableActionMenu<ActionAiderContactViewController3AddAddressToBag> ()
                                .WithSpecialController (typeof (EditionAiderContactViewController1Address));
                            }
                        }
                        else
                        {
                            wall.AddBrick ()
                            .Title (TextFormatter.FormatText (contact.AddressType))
                            .Text (contact.Address.GetSummary ())
                            .Icon ("Data.AiderAddress")
                            .EnableActionMenu<ActionAiderContactViewController3AddAddressToBag> ()
                            .WithSpecialController (typeof (EditionAiderContactViewController1Address));
                        }

                    }
                    break;

                case Enumerations.ContactType.Legal:
                    if (contact.LegalPerson.IsNotNull ())
                    {
                        wall.AddBrick (x => x.LegalPerson)
                            .Attribute (BrickMode.DefaultToSummarySubView);

                        wall.AddBrick ()
                            .Title ("Adresse de base")
                            .Icon ("Data.AiderAddress")
                            .Text (x => x.LegalPerson.Address.GetSummary ())
                            .EnableActionMenu<ActionAiderContactViewController3AddAddressToBag> ()
                            .WithSpecialController (typeof (EditionAiderContactViewController1Address));
                    }

                    var contactPersonSummary = string.IsNullOrEmpty (contact.LegalPersonContactFullName)
                        ? FormattedText.Empty
                        : TextFormatter.FormatText (AiderContactEntity.GetContactTitleLongText (contact.LegalPersonContactMrMrs, contact.ContactTitle), "\n",
                        /**/                        contact.LegalPersonContactFullName);

                    wall.AddBrick ()
                        .Icon (AiderPersonEntity.GetIconName ("Data", contact.LegalPersonContactMrMrs, contact.LegalPerson.Language))
                        .Title ("Personne de contact")
                        .Text (contactPersonSummary)
                        .EnableActionMenu<ActionAiderContactViewController1AddToBag> ()
                        .WithSpecialController (typeof (EditionAiderContactViewController2LegalContact));
                    break;

                default:
                    break;
            }
        }

        private bool CheckAndFixBrokenContact(AiderContactEntity contact)
        {
            bool skip = false;

            //	Process the degenerated cases when we click on them: no name for the
            //	contact (usually this means that there is no person or fully undefined
            //	persons attached to the contact). This should never happen, but as of
            //	dec. 17 2013, the database contained about 150 of these broken contacts.

            if (contact.DisplayName == AiderContactEntity.AddressContactSuffix)
            {
                var example = new AiderContactEntity
                {
                    DisplayName = AiderContactEntity.AddressContactSuffix
                };

                var emptyContacts = this.BusinessContext.GetByExample (example).ToList ();

                foreach (var item in emptyContacts)
                {
                    AiderPersonEntity.Delete (this.BusinessContext, item.Person);
                }

                this.BusinessContext.SaveChanges (Cresus.Core.Business.LockingPolicy.KeepLock);

                skip = true;
            }
            else if (string.IsNullOrEmpty (contact.DisplayName))
            {
                var example = new AiderContactEntity
                {
                    DisplayName = ""
                };

                var emptyContacts = this.BusinessContext.GetByExample (example).ToList ();

                foreach (var item in emptyContacts)
                {
                    if (item.Person.IsNull ())
                    {
                        AiderContactEntity.Delete (this.BusinessContext, item);
                    }
                    else
                    {
                        AiderPersonEntity.Delete (this.BusinessContext, item.Person);
                    }
                }

                this.BusinessContext.SaveChanges (Cresus.Core.Business.LockingPolicy.KeepLock);

                skip = true;
            }

            return skip;
        }

        private static FormattedText GetPersonTitle(AiderContactEntity contact)
        {
            if ((contact.Person.IsNotNull ()) &&
                (contact.Person.eCH_Person.DataSource == Enumerations.DataSource.Government))
            {
                return "Personne (RCH)";
            }
            else
            {
                return "Personne";
            }
        }

        public static FormattedText GetPersonContactSummary(AiderContactEntity contact)
		{
			var user = AiderUserManager.Current.AuthenticatedUser;
			var text = contact.Person.GetCompactSummary ();

			var contactInfoPrivate      = new List<FormattedText> (text.Split (FormattedText.HtmlBreak));
			var contactInfoConfidential = new List<FormattedText> ();
			var contactInfoProf			= new List<FormattedText> ();
			var contactInfoSecondary	= new List<FormattedText> ();

			foreach (var detail in contact.Person.Contacts.Where (x => x.Address.IsNotNull ()))
			{
				var address        = detail.Address;
				var phonesAndMails = address.GetPhones ().Concat (address.GetWebEmails ()).ToList ();

				if (phonesAndMails.Count == 0)
				{
					continue;
				}

				switch (detail.AddressType)
				{
					case Enumerations.AddressType.Default:

					case Enumerations.AddressType.Other:
						contactInfoPrivate.AddRange (phonesAndMails);
						break;

					case Enumerations.AddressType.Confidential:
						if (user.CanViewConfidentialAddress ())
						{
							if (contactInfoConfidential.Count == 0)
							{
								contactInfoConfidential.Add (new FormattedText ("<hr/><b>Confidentiel:</b>"));
							}

							contactInfoConfidential.AddRange (phonesAndMails);
						}
						break;

					case Enumerations.AddressType.Professional:
						if (contactInfoProf.Count == 0)
						{
							contactInfoProf.Add (new FormattedText ("<hr/><b>Professionnel:</b>"));
						}
						contactInfoProf.AddRange (phonesAndMails);
						break;

					case Enumerations.AddressType.Secondary:
						if (contactInfoSecondary.Count == 0)
						{
							contactInfoSecondary.Add (new FormattedText ("<hr/><b>Domicile secondaire:</b>"));
						}
						contactInfoSecondary.AddRange (phonesAndMails);
						break;
				}
			}

			var all = new List<FormattedText> ();

			all.AddRange (contactInfoPrivate.Distinct ());
			all.AddRange (contactInfoProf.Distinct ());
			all.AddRange (contactInfoSecondary.Distinct ());
			all.AddRange (contactInfoConfidential.Distinct ());

			return TextFormatter.Join (FormattedText.HtmlBreak, all);
		}
	}
}
