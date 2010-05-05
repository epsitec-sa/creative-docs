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
	public class EditionNaturalPersonViewController : EntityViewController
	{
		public EditionNaturalPersonViewController(string name, AbstractEntity entity, ViewControllerMode mode)
			: base (name, entity, mode)
		{
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			this.container = container;

			System.Diagnostics.Debug.Assert (this.Entity != null);
			var person = this.Entity as Entities.NaturalPersonEntity;
			System.Diagnostics.Debug.Assert (person != null);

			this.CreateHeaderEditorTile ();

			var group = EntityViewController.CreateGroupingTile (this.Container, "Data.NaturalPerson", "Personne physique", true);

			var accessor = new EntitiesAccessors.NaturalPersonAccessor (null, person as Entities.NaturalPersonEntity, false);
			var tile = this.CreateEditionTile (group, accessor, ViewControllerMode.None);

			this.CreateFooterEditorTile ();

			this.CreateCombo (tile.Container, 150, "Titre", accessor.TitleInitializer, false, false, false, accessor.NaturalTitle, x => accessor.NaturalTitle = x, null);
			this.CreateTextField (tile.Container, 0, "Prénom", accessor.NaturalPerson.Firstname, x => accessor.NaturalPerson.Firstname = x, Validators.StringValidator.Validate);
			this.CreateTextField (tile.Container, 0, "Nom", accessor.NaturalPerson.Lastname, x => accessor.NaturalPerson.Lastname = x, Validators.StringValidator.Validate);
			this.CreateMargin (tile.Container, true);
			this.CreateCombo (tile.Container, 0, "Sexe", accessor.GenderInitializer, true, false, true, accessor.Gender, x => accessor.Gender = x, null);
			this.CreateTextField (tile.Container, 75, "Date de naissance", accessor.NaturalBirthDate, x => accessor.NaturalBirthDate = x, null);

			UI.SetInitialFocus (this.container);
		}
	}
}
