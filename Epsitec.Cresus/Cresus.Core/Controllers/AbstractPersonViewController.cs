//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers
{
	public abstract class AbstractPersonViewController : EntityViewController
	{
		public AbstractPersonViewController(string name)
			: base (name)
		{
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
		}


		protected void CreateUITiles(Entities.AbstractPersonEntity person)
		{
			this.person = person;
			int count;
			bool compactFollower;

			if (this.Mode == ViewControllerMode.PersonEdition)
			{
				FrameBox container = this.CreateEditionTile (this.Entity, ViewControllerMode.None, EntitySummary.GetIcon (this.person), EntitySummary.GetTitle (this.person));

				if (person is Entities.NaturalPersonEntity)
				{
					var naturalPerson = person as Entities.NaturalPersonEntity;

					this.CreateTextField (container, "Titre", this.NaturalTitle, x => this.NaturalTitle = x, Validators.StringValidator.Validate);
					this.CreateMargin (container, 10);
					this.CreateTextField (container, "Prénom", naturalPerson.Firstname, x => naturalPerson.Firstname = x, Validators.StringValidator.Validate);
					this.CreateTextField (container, "Nom",    naturalPerson.Lastname,  x => naturalPerson.Lastname = x,  Validators.StringValidator.Validate);
					
					this.CreateMargin (container, 20);
					this.CreateTextField (container, "Date de naissance", this.NaturalBirthDate, x => this.NaturalBirthDate = x, null);
				}

				if (person is Entities.LegalPersonEntity)
				{
					var legalPerson = person as Entities.LegalPersonEntity;

					this.CreateTextField (container, "Nom complet", legalPerson.Name,       x => legalPerson.Name = x,       Validators.StringValidator.Validate);
					this.CreateTextField (container, "Nom court",   legalPerson.ShortName,  x => legalPerson.ShortName = x,  Validators.StringValidator.Validate);
					this.CreateMargin (container, 20);
					this.CreateTextFieldMulti (container, "Complément",  100, legalPerson.Complement, x => legalPerson.Complement = x, null);
				}
			}
			else
			{
				//	Une première tuile pour l'identité de la personne.
				this.CreateSummaryTile (this.Entity, false, ViewControllerMode.PersonEdition, EntitySummary.GetIcon (this.person), EntitySummary.GetTitle (this.person), EntitySummary.GetPersonSummary (person));

				//	Une tuile distincte par adresse postale.
				this.CreateSeparator (4);

				count = 0;
				foreach (Entities.AbstractContactEntity contact in this.person.Contacts)
				{
					if (contact is Entities.MailContactEntity)
					{
						var mailContact = contact as Entities.MailContactEntity;
						this.CreateSummaryTile (mailContact, false, ViewControllerMode.GenericEdition, EntitySummary.GetIcon (mailContact), EntitySummary.GetTitle (mailContact), EntitySummary.GetMailSummary (mailContact));
						count++;
					}
				}

				if (count == 0)
				{
					this.CreateSummaryTile (null, false, ViewControllerMode.None, "Data.Mail", "Adresse", EntitySummary.emptyText);
				}

				//	Une tuile distincte par numéro de téléphone.
				this.CreateSeparator (4);

				count = 0;
				compactFollower = false;
				foreach (Entities.AbstractContactEntity contact in this.person.Contacts)
				{
					if (contact is Entities.TelecomContactEntity)
					{
						var telecomContact = contact as Entities.TelecomContactEntity;
						this.CreateSummaryTile (telecomContact, compactFollower, ViewControllerMode.TelecomEdition, EntitySummary.GetIcon (telecomContact), EntitySummary.GetTitle (telecomContact), EntitySummary.GetTelecomSummary (telecomContact));
						count++;
						compactFollower = true;
					}
				}

				if (count == 0)
				{
					this.CreateSummaryTile (null, false, ViewControllerMode.None, "Data.Telecom", "Téléphone", EntitySummary.emptyText);
				}

				//	Une tuile distincte par adresse mail.
				this.CreateSeparator (4);

				count = 0;
				compactFollower = false;
				foreach (Entities.AbstractContactEntity contact in this.person.Contacts)
				{
					if (contact is Entities.UriContactEntity)
					{
						var uriContact = contact as Entities.UriContactEntity;
						this.CreateSummaryTile (uriContact, compactFollower, ViewControllerMode.UriEdition, EntitySummary.GetIcon (uriContact), EntitySummary.GetTitle (uriContact), EntitySummary.GetUriSummary (uriContact));
						count++;
						compactFollower = true;
					}
				}

				if (count == 0)
				{
					this.CreateSummaryTile (null, false, ViewControllerMode.None, "Data.Uri", "Mail", EntitySummary.emptyText);
				}
			}

			this.AdjustLastTile ();
		}


		private string NaturalTitle
		{
			get
			{
				var naturalPerson = this.person as Entities.NaturalPersonEntity;

				if (naturalPerson != null && naturalPerson.Title != null)
				{
					return naturalPerson.Title.Name;
				}
				else
				{
					return null;
				}
			}
			set
			{
				var naturalPerson = this.person as Entities.NaturalPersonEntity;

				if (naturalPerson == null)
				{
					return;
				}

				if (naturalPerson.Title == null)
				{
					naturalPerson.Title = new Entities.PersonTitleEntity ();
				}

				naturalPerson.Title.Name = value;
			}
		}

		private string NaturalBirthDate
		{
			get
			{
				var naturalPerson = this.person as Entities.NaturalPersonEntity;

				if (naturalPerson != null)
				{
					return naturalPerson.BirthDate.ToString();
				}
				else
				{
					return null;
				}
			}
			set
			{
				var naturalPerson = this.person as Entities.NaturalPersonEntity;

				if (naturalPerson == null)
				{
					return;
				}

				//?naturalPerson.BirthDate = Date.FromString(value);  // TODO:
			}
		}


		private Entities.AbstractPersonEntity person;
	}
}
