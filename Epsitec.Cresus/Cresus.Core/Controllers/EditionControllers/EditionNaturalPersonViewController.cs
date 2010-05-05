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
	public class EditionNaturalPersonViewController : EntityViewController<Entities.NaturalPersonEntity>
	{
		public EditionNaturalPersonViewController(string name, Entities.NaturalPersonEntity entity)
			: base (name, entity)
		{
		}

		public override void CreateUI(Widget container)
		{
			UIBuilder builder = new UIBuilder (container, this);
			
			builder.CreateHeaderEditorTile ();

			var person = this.Entity;
			var group = builder.CreateEditionGroupingTile ("Data.NaturalPerson", "Personne physique");

			var accessor = new Accessors.NaturalPersonAccessor (null, person as Entities.NaturalPersonEntity, false);
			var tile = builder.CreateEditionTile (group, accessor);

			builder.CreateFooterEditorTile ();

			builder.CreateCombo (tile.Container, 150, "Titre", accessor.TitleInitializer, false, false, false, accessor.NaturalTitle, x => accessor.NaturalTitle = x, null);
			builder.CreateTextField (tile.Container, 0, "Prénom", accessor.Entity.Firstname, x => accessor.Entity.Firstname = x, Validators.StringValidator.Validate);
			builder.CreateTextField (tile.Container, 0, "Nom", accessor.Entity.Lastname, x => accessor.Entity.Lastname = x, Validators.StringValidator.Validate);
			builder.CreateMargin (tile.Container, true);
			builder.CreateCombo (tile.Container, 0, "Sexe", accessor.GenderInitializer, true, false, true, accessor.Gender, x => accessor.Gender = x, null);
			builder.CreateTextField (tile.Container, 75, "Date de naissance", accessor.NaturalBirthDate, x => accessor.NaturalBirthDate = x, null);

			UI.SetInitialFocus (container);
		}
	}
}
