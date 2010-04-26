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
			//	Incroyable système en 4 passes, pour essayer d'abord de ne mettre que des tuiles distinctes, puis de grouper petit à petit.
			for (int pass = 0; pass < 4; pass++)
			{
				this.container.Children.Clear ();
				this.CreateUITiles (person, pass > 2, pass > 1, pass > 0);

				Common.Widgets.Layouts.LayoutContext.SyncMeasure (this.container);
				Common.Widgets.Layouts.LayoutContext.SyncArrange (this.container);

				//	Calcule la hauteur des tuiles qu'on vient de générer.
				double currentHeight = 0;
				foreach (Widget widget in this.container.Children)
				{
					currentHeight += widget.ActualHeight;
					currentHeight += widget.Margins.Top;
					currentHeight += widget.Margins.Bottom;
				}

				if (currentHeight <= this.container.ActualHeight)  // assez de place ?
				{
					break;
				}
			}
		}

		private void CreateUITiles(Entities.AbstractPersonEntity person, bool groupMail, bool groupTelecom, bool groupUri)
		{
			this.person = person;
			int count;
			bool compactFollower;

			if (this.Mode == ViewControllerMode.PersonEdition)
			{
				if (person is Entities.NaturalPersonEntity)
				{
					var accessor = new EntitiesAccessors.NaturalPersonAccessor (person as Entities.NaturalPersonEntity, false);
					FrameBox container = this.CreateEditionTile (accessor, ViewControllerMode.None);

					this.CreateTextField (container, 100, "Titre", accessor.NaturalTitle, x => accessor.NaturalTitle = x, Validators.StringValidator.Validate);
					this.CreateTextField (container, 0, "Prénom", accessor.NaturalPerson.Firstname, x => accessor.NaturalPerson.Firstname = x, Validators.StringValidator.Validate);
					this.CreateTextField (container, 0, "Nom", accessor.NaturalPerson.Lastname, x => accessor.NaturalPerson.Lastname = x, Validators.StringValidator.Validate);
					this.CreateMargin (container, true);
					this.CreateTextField (container, 100, "Date de naissance", accessor.NaturalBirthDate, x => accessor.NaturalBirthDate = x, null);
				}

				if (person is Entities.LegalPersonEntity)
				{
					var accessor = new EntitiesAccessors.LegalPersonAccessor (person as Entities.LegalPersonEntity, false);
					FrameBox container = this.CreateEditionTile (accessor, ViewControllerMode.None);

					this.CreateTextField (container, 0, "Nom complet", accessor.LegalPerson.Name, x => accessor.LegalPerson.Name = x, Validators.StringValidator.Validate);
					this.CreateTextField (container, 150, "Nom court", accessor.LegalPerson.ShortName, x => accessor.LegalPerson.ShortName = x, Validators.StringValidator.Validate);
					this.CreateMargin (container, true);
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
					var accessor = new EntitiesAccessors.NaturalPersonAccessor (this.Entity, false);
					this.CreateSummaryTile (accessor, groupIndex, false, ViewControllerMode.PersonEdition);
				}

				if (this.Entity is Entities.LegalPersonEntity)
				{
					var accessor = new EntitiesAccessors.LegalPersonAccessor (this.Entity, false);
					this.CreateSummaryTile (accessor, groupIndex, false, ViewControllerMode.PersonEdition);
				}

				groupIndex++;

				//	Une tuile distincte par adresse postale.
				count = 0;
				compactFollower = false;
				foreach (Entities.AbstractContactEntity contact in this.person.Contacts)
				{
					if (contact is Entities.MailContactEntity)
					{
						var accessor = new EntitiesAccessors.MailContactAccessor(contact as Entities.MailContactEntity, groupMail);
						this.CreateSummaryTile (accessor, groupIndex, compactFollower, ViewControllerMode.GenericEdition);
						count++;
						compactFollower = groupMail;
					}
				}

				if (count == 0)
				{
					var emptyEntity = new Entities.MailContactEntity ();
					var accessor = new EntitiesAccessors.MailContactAccessor (emptyEntity, false);
					this.CreateSummaryTile (accessor, groupIndex, false, ViewControllerMode.GenericEdition);
				}

				groupIndex++;

				//	Une tuile distincte par numéro de téléphone.
				count = 0;
				compactFollower = false;
				foreach (Entities.AbstractContactEntity contact in this.person.Contacts)
				{
					if (contact is Entities.TelecomContactEntity)
					{
						var accessor = new EntitiesAccessors.TelecomContactAccessor(contact as Entities.TelecomContactEntity, groupTelecom);
						this.CreateSummaryTile (accessor, groupIndex, compactFollower, ViewControllerMode.TelecomEdition);
						count++;
						compactFollower = groupTelecom;
					}
				}

				if (count == 0)
				{
					var emptyEntity = new Entities.TelecomContactEntity ();
					var accessor = new EntitiesAccessors.TelecomContactAccessor (emptyEntity, false);
					this.CreateSummaryTile (accessor, groupIndex, false, ViewControllerMode.GenericEdition);
				}

				groupIndex++;

				//	Une tuile distincte par adresse mail.
				count = 0;
				compactFollower = false;
				foreach (Entities.AbstractContactEntity contact in this.person.Contacts)
				{
					if (contact is Entities.UriContactEntity)
					{
						var accessor = new EntitiesAccessors.UriContactAccessor (contact as Entities.UriContactEntity, groupUri);
						this.CreateSummaryTile (accessor, groupIndex, compactFollower, ViewControllerMode.UriEdition);
						count++;
						compactFollower = groupUri;
					}
				}

				if (count == 0)
				{
					var emptyEntity = new Entities.UriContactEntity ();
					var accessor = new EntitiesAccessors.UriContactAccessor (emptyEntity, false);
					this.CreateSummaryTile (accessor, groupIndex, false, ViewControllerMode.GenericEdition);
				}

				groupIndex++;
			}

			this.AdjustVisualForGroups ();
		}


		private Entities.AbstractPersonEntity person;
	}
}
