//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionRolesContactViewController : EditionViewController<Entities.AbstractContactEntity>
	{
		public EditionRolesContactViewController(string name, Entities.AbstractContactEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
			UIBuilder builder = new UIBuilder (container, this);
			
			TitleTile group;

			var contact = this.Entity;
			var accessor = new Accessors.RolesContactAccessor (null, contact, false);

			//	Crée les tuiles.
			builder.CreateHeaderEditorTile ();

			group = builder.CreateEditionGroupingTile ("Data.Type", "Type");
			var tile = builder.CreateEditionTile (group, accessor);

			builder.CreateFooterEditorTile ();

			//?this.CreateLinkButtons (tile.Container);

			//	Crée le contenu de la tuile d'édition.
			builder.CreateDetailed (tile.Container, 0, "Choix du ou des rôles souhaités", true, this.Entity.Roles, accessor, null);  // TODO: remplacer null

			UI.SetInitialFocus (container);
		}
	}
}
