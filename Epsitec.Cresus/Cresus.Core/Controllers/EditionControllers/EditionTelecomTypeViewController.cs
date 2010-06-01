//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionTelecomTypeViewController : EntityViewController<Entities.TelecomContactEntity>
	{
		public EditionTelecomTypeViewController(string name, Entities.TelecomContactEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
			UIBuilder builder = new UIBuilder (container, this);
			TitleTile group;

			var contact = this.Entity;
			var accessor = new Accessors.TelecomTypeAccessor (null, contact, false);

			//	Crée les tuiles.
			builder.CreateHeaderEditorTile ();

			group = builder.CreateEditionGroupingTile ("Data.Type", "Type");
			var tile = builder.CreateEditionTile (group, accessor);

			builder.CreateFooterEditorTile ();

			//	Crée le contenu de la tuile d'édition.
			builder.CreateCombo (tile.Container, 150, "Type du numéro de téléphone", accessor.TelecomTypeInitializer, true, false, true, accessor.TelecomType, x => accessor.TelecomType = x, null);

			UI.SetInitialFocus (container);
		}
	}
}
