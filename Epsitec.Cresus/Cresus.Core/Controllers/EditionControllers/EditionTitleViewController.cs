//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionTitleViewController : EditionViewController<Entities.PersonTitleEntity>
	{
		public EditionTitleViewController(string name, Entities.PersonTitleEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
			var builder = new UIBuilder (container, this);

			builder.CreateHeaderEditorTile ();

			var mail = this.Entity;
			var group = builder.CreateEditionGroupingTile ("Data.NaturalPerson", "Titre");
			var tile = builder.CreateEditionTile (group, this.Entity);

			builder.CreateFooterEditorTile ();

			builder.CreateTextField (tile.Container, 0, "Titre", this.Entity.Name, x => this.Entity.Name = x, Validators.StringValidator.Validate);

			UI.SetInitialFocus (container);
		}
	}
}
