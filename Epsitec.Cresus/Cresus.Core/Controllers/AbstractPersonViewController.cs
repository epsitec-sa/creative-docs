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
				if (person is Entities.NaturalPersonEntity)
				{
					var accessor = new EntitiesAccessors.NaturalPersonAccessor (person as Entities.NaturalPersonEntity);
					FrameBox container = this.CreateEditionTile (this.Entity, ViewControllerMode.None, accessor.Icon, accessor.Title);

					this.CreateTextField (container, 100, "Titre", accessor.NaturalTitle, x => accessor.NaturalTitle = x, Validators.StringValidator.Validate);
					this.CreateMargin (container, 10);
					this.CreateTextField (container, 0, "Prénom", accessor.NaturalPerson.Firstname, x => accessor.NaturalPerson.Firstname = x, Validators.StringValidator.Validate);
					this.CreateTextField (container, 0, "Nom", accessor.NaturalPerson.Lastname, x => accessor.NaturalPerson.Lastname = x, Validators.StringValidator.Validate);
					
					this.CreateMargin (container, 20);
					this.CreateTextField (container, 100, "Date de naissance", accessor.NaturalBirthDate, x => accessor.NaturalBirthDate = x, null);
				}

				if (person is Entities.LegalPersonEntity)
				{
					var accessor = new EntitiesAccessors.LegalPersonAccessor (person as Entities.LegalPersonEntity);
					FrameBox container = this.CreateEditionTile (this.Entity, ViewControllerMode.None, accessor.Icon, accessor.Title);

					this.CreateTextField (container, 0, "Nom complet", accessor.LegalPerson.Name, x => accessor.LegalPerson.Name = x, Validators.StringValidator.Validate);
					this.CreateTextField (container, 150, "Nom court", accessor.LegalPerson.ShortName, x => accessor.LegalPerson.ShortName = x, Validators.StringValidator.Validate);
					this.CreateMargin (container, 20);
					this.CreateTextFieldMulti (container, 100, "Complément", accessor.LegalPerson.Complement, x => accessor.LegalPerson.Complement = x, null);
				}

				this.SetInitialFocus ();
			}
			else
			{
				int groupIndex = 0;

				//	Une première tuile pour l'identité de la personne.
				if (this.Entity is Entities.NaturalPersonEntity)
				{
					var accessor = new EntitiesAccessors.NaturalPersonAccessor (this.Entity);
					this.CreateSummaryTile (accessor.NaturalPerson, groupIndex, false, ViewControllerMode.PersonEdition, accessor.Icon, accessor.Title, accessor.Summary);
				}

				if (this.Entity is Entities.LegalPersonEntity)
				{
					var accessor = new EntitiesAccessors.LegalPersonAccessor (this.Entity);
					this.CreateSummaryTile (accessor.LegalPerson, groupIndex, false, ViewControllerMode.PersonEdition, accessor.Icon, accessor.Title, accessor.Summary);
				}

				groupIndex++;

				//	Une tuile distincte par adresse postale.
				count = 0;
				foreach (Entities.AbstractContactEntity contact in this.person.Contacts)
				{
					if (contact is Entities.MailContactEntity)
					{
						var accessor = new EntitiesAccessors.MailContactAccessor(contact as Entities.MailContactEntity);
						this.CreateSummaryTile (accessor.MailContact, groupIndex, false, ViewControllerMode.GenericEdition, accessor.Icon, accessor.Title, accessor.Summary);
						count++;
					}
				}

				if (count == 0)
				{
					var emptyEntity = new Entities.MailContactEntity ();
					var accessor = new EntitiesAccessors.MailContactAccessor (emptyEntity);
					this.CreateSummaryTile (accessor.MailContact, groupIndex, false, ViewControllerMode.GenericEdition, accessor.Icon, accessor.Title, accessor.Summary);
					//?this.CreateSummaryTile (null, groupIndex, false, ViewControllerMode.None, "Data.Mail", "Adresse", EntitySummary.emptyText);
				}

				groupIndex++;

				//	Une tuile distincte par numéro de téléphone.
				count = 0;
				compactFollower = false;
				foreach (Entities.AbstractContactEntity contact in this.person.Contacts)
				{
					if (contact is Entities.TelecomContactEntity)
					{
						var accessor = new EntitiesAccessors.TelecomContactAccessor(contact as Entities.TelecomContactEntity);
						this.CreateSummaryTile (accessor.TelecomContact, groupIndex, compactFollower, ViewControllerMode.TelecomEdition, accessor.Icon, accessor.Title, accessor.Summary);
						count++;
						compactFollower = true;
					}
				}

				if (count == 0)
				{
					var emptyEntity = new Entities.TelecomContactEntity ();
					var accessor = new EntitiesAccessors.TelecomContactAccessor (emptyEntity);
					this.CreateSummaryTile (accessor.TelecomContact, groupIndex, false, ViewControllerMode.GenericEdition, accessor.Icon, accessor.Title, accessor.Summary);
					//?this.CreateSummaryTile (null, groupIndex, false, ViewControllerMode.None, "Data.Telecom", "Téléphone", EntitySummary.emptyText);
				}

				groupIndex++;

				//	Une tuile distincte par adresse mail.
				count = 0;
				compactFollower = false;
				foreach (Entities.AbstractContactEntity contact in this.person.Contacts)
				{
					if (contact is Entities.UriContactEntity)
					{
						var accessor = new EntitiesAccessors.UriContactAccessor (contact as Entities.UriContactEntity);
						this.CreateSummaryTile (accessor.UriContact, groupIndex, compactFollower, ViewControllerMode.UriEdition, accessor.Icon, accessor.Title, accessor.Summary);
						count++;
						compactFollower = true;
					}
				}

				if (count == 0)
				{
					var emptyEntity = new Entities.UriContactEntity ();
					var accessor = new EntitiesAccessors.UriContactAccessor (emptyEntity);
					this.CreateSummaryTile (accessor.UriContact, groupIndex, false, ViewControllerMode.GenericEdition, accessor.Icon, accessor.Title, accessor.Summary);
					//?this.CreateSummaryTile (null, groupIndex, false, ViewControllerMode.None, "Data.Uri", "Mail", EntitySummary.emptyText);
				}

				groupIndex++;
			}

			this.AdjustVisualForGroups ();
		}


		private Entities.AbstractPersonEntity person;
	}
}
