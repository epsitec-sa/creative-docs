//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionUriContactViewController : EditionViewController<Entities.UriContactEntity>
	{
		public EditionUriContactViewController(string name, Entities.UriContactEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
			UIBuilder builder = new UIBuilder (container, this);
			TitleTile group;

			System.Diagnostics.Debug.Assert (this.Entity != null);
			var accessor = new Accessors.UriContactAccessor (null, this.Entity, false);

			//	Crée les tuiles.
			builder.CreateHeaderEditorTile ();

			//	Crée le contenu de la tuile d'édition.
			group = builder.CreateSummaryGroupingTile ("Data.Roles", "Rôles");

			var roleAccessor = new Accessors.RolesContactAccessor (null, accessor.Entity, false)
			{
				ViewControllerMode = ViewControllerMode.Edition
			};

			builder.CreateSummaryTile (group, roleAccessor);

			//	Crée le contenu de la tuile d'édition.
			group = builder.CreateSummaryGroupingTile ("Data.Type", "Type");

			var uriSchemeAccessor = new Accessors.UriSchemeAccessor (null, accessor.Entity, false)
			{
				ViewControllerMode = ViewControllerMode.Edition
			};

			builder.CreateSummaryTile (group, uriSchemeAccessor);
	
			//	Crée le contenu de la tuile d'édition.
			group = builder.CreateEditionGroupingTile ("Data.Uri", "Mail");
			var tile = builder.CreateEditionTile (group, accessor);

			builder.CreateLinkButtons (tile.Container);

			builder.CreateTextField (tile.Container, 0, "Adresse mail", accessor.Entity.Uri, x => accessor.Entity.Uri = x, Validators.StringValidator.Validate);

			UI.SetInitialFocus (container);
		}
	}
}
