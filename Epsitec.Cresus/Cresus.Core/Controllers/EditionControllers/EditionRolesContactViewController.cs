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

using Epsitec.Cresus.Core.Widgets.Tiles;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionRolesContactViewController : EntityViewController<Entities.AbstractContactEntity>
	{
		public EditionRolesContactViewController(string name, Entities.AbstractContactEntity entity)
			: base (name, entity)
		{
		}

		public override void CreateUI(Widget container)
		{
			UIBuilder builder = new UIBuilder (container, this);
			
			GroupingTile group;

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
