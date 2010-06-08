﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionLegalPersonViewController : EditionViewController<Entities.LegalPersonEntity>
	{
		public EditionLegalPersonViewController(string name, Entities.LegalPersonEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
			var builder = new UIBuilder (container, this);

			builder.CreateHeaderEditorTile ();

			var mail = this.Entity;
			var group = builder.CreateEditionGroupingTile ("Data.LegalPerson", "Personne morale");
			var tile = builder.CreateEditionTile (group, this.Entity);

			builder.CreateFooterEditorTile ();

			builder.CreateTextField (tile, 0, "Nom complet", this.Entity.Name, x => this.Entity.Name = x, Validators.StringValidator.Validate);
			builder.CreateTextField (tile, 150, "Nom court", this.Entity.ShortName, x => this.Entity.ShortName = x, Validators.StringValidator.Validate);
			builder.CreateMargin (tile, true);
			builder.CreateTextFieldMulti (tile, 100, "Complément", this.Entity.Complement, x => this.Entity.Complement = x, Validators.StringValidator.Validate);

			UI.SetInitialFocus (container);
		}
	}
}
