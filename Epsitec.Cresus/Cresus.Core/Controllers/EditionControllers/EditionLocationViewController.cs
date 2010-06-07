//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionLocationViewController : EditionViewController<Entities.LocationEntity>
	{
		public EditionLocationViewController(string name, Entities.LocationEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
#if false
			UIBuilder builder = new UIBuilder (container, this);
			TitleTile group;

			var contact = this.Entity;
			var accessor = new Accessors.LocationAccessor (null, contact, false);

			//	Crée les tuiles.
			builder.CreateHeaderEditorTile ();

			//	Crée le contenu de la tuile d'édition.
			group = builder.CreateEditionGroupingTile ("Data.Mail", "Ville");
			var tile = builder.CreateEditionTile (group, accessor);

			builder.CreateTextField (tile.Container, 0, "Numéro postal", accessor.PostalCode, x => accessor.PostalCode = x, Validators.StringValidator.Validate);
			builder.CreateTextField (tile.Container, 0, "Ville", accessor.Name, x => accessor.Name = x, Validators.StringValidator.Validate);
			
			UI.SetInitialFocus (container);
#else
			var builder = new UIBuilder (container, this);

			builder.CreateHeaderEditorTile ();

			var mail = this.Entity;
			var group = builder.CreateEditionGroupingTile ("Data.Mail", "Ville");
			var tile = builder.CreateEditionTile (group, this.Entity);

			builder.CreateFooterEditorTile ();

			builder.CreateTextField (tile.Container, 0, "Numéro postal", this.Entity.PostalCode, x => this.Entity.PostalCode = x, Validators.StringValidator.Validate);
			builder.CreateTextField (tile.Container, 150, "Ville", this.Entity.Name, x => this.Entity.Name = x, Validators.StringValidator.Validate);

			UI.SetInitialFocus (container);
#endif
		}
	}
}
