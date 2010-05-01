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
		public AbstractPersonViewController(string name, AbstractEntity entity, ViewControllerMode mode)
			: base (name, entity, mode)
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
			//	Subtil système en 4 passes, pour essayer d'abord de ne mettre que des tuiles distinctes, puis de grouper petit à petit.
			//	En cas de manque de place, ou groupe d'abord les mails, puis les téléphones, puis finalement les adresses.
			for (int pass = 0; pass < 4; pass++)
			{
				this.container.Children.Clear ();  // supprime les widgets générés à la passe précédente
				this.CreateUITiles (person, pass > 2, pass > 1, pass > 0);

				//	TODO: Je serais tranquille si Pierre vérifiait ceci !
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
			int count;
			bool compactFollower;

			if (this.Mode == ViewControllerMode.PersonEdition)
			{
				if (person is Entities.NaturalPersonEntity)
				{
					this.CreateHeaderEditorTile ();

					var accessor = new EntitiesAccessors.NaturalPersonAccessor (null, person as Entities.NaturalPersonEntity, false);
					Widgets.AbstractTile tile = this.CreateEditionTile (accessor, ViewControllerMode.None);

					this.CreateFooterEditorTile ();

					this.CreateCombo (tile.Container, 150, "Titre", accessor.TitleInitializer, false, false, false, accessor.NaturalTitle, x => accessor.NaturalTitle = x, null);
					this.CreateTextField (tile.Container, 0, "Prénom", accessor.NaturalPerson.Firstname, x => accessor.NaturalPerson.Firstname = x, Validators.StringValidator.Validate);
					this.CreateTextField (tile.Container, 0, "Nom", accessor.NaturalPerson.Lastname, x => accessor.NaturalPerson.Lastname = x, Validators.StringValidator.Validate);
					this.CreateMargin (tile.Container, true);
					this.CreateCombo (tile.Container, 0, "Sexe", accessor.GenderInitializer, true, false, true, accessor.Gender, x => accessor.Gender = x, null);
					this.CreateTextField (tile.Container, 75, "Date de naissance", accessor.NaturalBirthDate, x => accessor.NaturalBirthDate = x, null);
				}

				if (person is Entities.LegalPersonEntity)
				{
					this.CreateHeaderEditorTile ();

					var accessor = new EntitiesAccessors.LegalPersonAccessor (null, person as Entities.LegalPersonEntity, false);
					Widgets.AbstractTile tile = this.CreateEditionTile (accessor, ViewControllerMode.None);
					
					this.CreateFooterEditorTile ();

					this.CreateTextField (tile.Container, 0, "Nom complet", accessor.LegalPerson.Name, x => accessor.LegalPerson.Name = x, Validators.StringValidator.Validate);
					this.CreateTextField (tile.Container, 150, "Nom court", accessor.LegalPerson.ShortName, x => accessor.LegalPerson.ShortName = x, Validators.StringValidator.Validate);
					this.CreateMargin (tile.Container, true);
					this.CreateTextFieldMulti (tile.Container, 100, "Complément", accessor.LegalPerson.Complement, x => accessor.LegalPerson.Complement = x, null);
				}

				this.SetInitialFocus ();
			}
			else
			{
				this.CreateHeaderEditorTile ();

				int groupIndex = 0;

				//	Une première tuile pour l'identité de la personne.
				if (this.Entity is Entities.NaturalPersonEntity)
				{
					var accessor = new EntitiesAccessors.NaturalPersonAccessor (null, this.Entity, false);
					this.CreateSummaryTile (accessor, groupIndex, false, false, false, ViewControllerMode.PersonEdition);
				}

				if (this.Entity is Entities.LegalPersonEntity)
				{
					var accessor = new EntitiesAccessors.LegalPersonAccessor (null, this.Entity, false);
					this.CreateSummaryTile (accessor, groupIndex, false, false, false, ViewControllerMode.PersonEdition);
				}

				groupIndex++;

				//	Une tuile distincte par adresse postale.
				count = 0;
				compactFollower = false;

				foreach (Entities.AbstractContactEntity contact in person.Contacts)
				{
					if (contact is Entities.MailContactEntity)
					{
						var accessor = new EntitiesAccessors.MailContactAccessor (person.Contacts, contact as Entities.MailContactEntity, groupMail);
						this.CreateSummaryTile (accessor, groupIndex, compactFollower, true, false, ViewControllerMode.GenericEdition);

						count++;
						compactFollower = groupMail;
					}
				}

				if (count == 0)
				{
					var emptyEntity = new Entities.MailContactEntity ();
					person.Contacts.Add (emptyEntity);

					var accessor = new EntitiesAccessors.MailContactAccessor (person.Contacts, emptyEntity, false);
					this.CreateSummaryTile (accessor, groupIndex, false, false, false, ViewControllerMode.GenericEdition);
				}

				groupIndex++;

				//	Une tuile distincte par numéro de téléphone.
				count = 0;
				compactFollower = false;

				foreach (Entities.AbstractContactEntity contact in person.Contacts)
				{
					if (contact is Entities.TelecomContactEntity)
					{
						var accessor = new EntitiesAccessors.TelecomContactAccessor (person.Contacts, contact as Entities.TelecomContactEntity, groupTelecom);
						this.CreateSummaryTile (accessor, groupIndex, compactFollower, true, false, ViewControllerMode.TelecomEdition);

						count++;
						compactFollower = groupTelecom;
					}
				}

				if (count == 0)
				{
					var emptyEntity = new Entities.TelecomContactEntity ();
					person.Contacts.Add (emptyEntity);

					var accessor = new EntitiesAccessors.TelecomContactAccessor (person.Contacts, emptyEntity, false);
					this.CreateSummaryTile (accessor, groupIndex, false, false, false, ViewControllerMode.GenericEdition);
				}

				groupIndex++;

				//	Une tuile distincte par adresse mail.
				count = 0;
				compactFollower = false;

				foreach (Entities.AbstractContactEntity contact in person.Contacts)
				{
					if (contact is Entities.UriContactEntity)
					{
						var accessor = new EntitiesAccessors.UriContactAccessor (person.Contacts, contact as Entities.UriContactEntity, groupUri);
						this.CreateSummaryTile (accessor, groupIndex, compactFollower, true, false, ViewControllerMode.UriEdition);

						count++;
						compactFollower = groupUri;
					}
				}

				if (count == 0)
				{
					var emptyEntity = new Entities.UriContactEntity ();
					person.Contacts.Add (emptyEntity);

					var accessor = new EntitiesAccessors.UriContactAccessor (person.Contacts, emptyEntity, false);
					this.CreateSummaryTile (accessor, groupIndex, false, false, false, ViewControllerMode.GenericEdition);
				}

				groupIndex++;

				this.CreateFooterEditorTile ();
			}

			this.AdjustVisualForGroups ();
		}
	}
}
