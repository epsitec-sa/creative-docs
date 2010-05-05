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

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public abstract class AbstractEditionPersonViewController : EntityViewController
	{
		public AbstractEditionPersonViewController(string name, AbstractEntity entity, ViewControllerMode mode)
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
			if (!this.container.IsActualGeometryValid)
			{
				Common.Widgets.Layouts.LayoutContext.SyncArrange (this.container);
			}

			for (int pass = 2; pass < 4; pass++)
			{
				this.container.Children.Clear ();  // supprime les widgets générés à la passe précédente
				this.CreateUITiles (person, pass > 2, pass > 1, pass > 0);

				Common.Widgets.Layouts.LayoutContext.SyncMeasure (this.container);

				//	Calcule la hauteur des tuiles qu'on vient de générer.
				double currentHeight = 0;
				foreach (Widget widget in this.container.Children)
				{
					currentHeight += Epsitec.Common.Widgets.Layouts.LayoutMeasure.GetHeight (widget).Desired;
					currentHeight += widget.Margins.Height;
				}

				if (currentHeight <= this.container.ActualHeight)  // assez de place ?
				{
					break;
				}
			}
		}

		private void CreateUITiles(Entities.AbstractPersonEntity person, bool groupMail, bool groupTelecom, bool groupUri)
		{
			Widgets.GroupingTile group = null;

			if (person is Entities.NaturalPersonEntity)
			{
				this.CreateHeaderEditorTile ();

				group = EntityViewController.CreateGroupingTile (this.Container, "Data.NaturalPerson", "Personne physique", true);

				var accessor = new EntitiesAccessors.NaturalPersonAccessor (null, person as Entities.NaturalPersonEntity, false);
				var tile = this.CreateEditionTile (group, accessor, ViewControllerMode.None);

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

				group = EntityViewController.CreateGroupingTile (this.Container, "Data.LegalPerson", "Personne morale", true);

				var accessor = new EntitiesAccessors.LegalPersonAccessor (null, person as Entities.LegalPersonEntity, false);
				var tile = this.CreateEditionTile (group, accessor, ViewControllerMode.None);
					
				this.CreateFooterEditorTile ();

				this.CreateTextField (tile.Container, 0, "Nom complet", accessor.LegalPerson.Name, x => accessor.LegalPerson.Name = x, Validators.StringValidator.Validate);
				this.CreateTextField (tile.Container, 150, "Nom court", accessor.LegalPerson.ShortName, x => accessor.LegalPerson.ShortName = x, Validators.StringValidator.Validate);
				this.CreateMargin (tile.Container, true);
				this.CreateTextFieldMulti (tile.Container, 100, "Complément", accessor.LegalPerson.Complement, x => accessor.LegalPerson.Complement = x, null);
			}

			UI.SetInitialFocus (this.container);
		}
	}
}
