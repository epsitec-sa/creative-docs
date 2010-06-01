//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionUriSchemeViewController : EntityViewController<Entities.UriContactEntity>
	{
		public EditionUriSchemeViewController(string name, Entities.UriContactEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
			UIBuilder builder = new UIBuilder (container, this);
			TitleTile group;

			var contact = this.Entity;
			var accessor = new Accessors.UriSchemeAccessor (null, contact, false);

			//	Crée les tuiles.
			builder.CreateHeaderEditorTile ();

			group = builder.CreateEditionGroupingTile ("Data.Type", "Type");
			var tile = builder.CreateEditionTile (group, accessor);

			builder.CreateFooterEditorTile ();

			//	Crée le contenu de la tuile d'édition.
			builder.CreateCombo (tile.Container, 100, "Type du moyen de contact", accessor.UriSchemeInitializer, true, false, true, accessor.UriScheme, x => accessor.UriScheme = x, null);

			UI.SetInitialFocus (container);
		}
	}
}
