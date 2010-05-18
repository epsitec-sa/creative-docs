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
	public class EditionUriContactViewController : EntityViewController<Entities.UriContactEntity>
	{
		public EditionUriContactViewController(string name, Entities.UriContactEntity entity)
			: base (name, entity)
		{
		}

		public override void CreateUI(Widget container)
		{
			UIBuilder builder = new UIBuilder (container, this);
			GroupingTile group;

			System.Diagnostics.Debug.Assert (this.Entity != null);
			var accessor = new Accessors.UriContactAccessor (null, this.Entity, false);

			//	Crée les tuiles.
			builder.CreateHeaderEditorTile ();

			//	Crée le contenu de la tuile d'édition.
			group = builder.CreateSummaryGroupingTile ("Data.Roles", "Rôles");

			var roleAccessor = new Accessors.RolesContactAccessor (null, accessor.Entity, false)
			{
				ViewControllerMode = ViewControllerMode.RolesEdition
			};

			builder.CreateSummaryTile (group, roleAccessor);

			//	Crée le contenu de la tuile d'édition.
			group = builder.CreateSummaryGroupingTile ("Data.Type", "Type");

			var uriSchemeAccessor = new Accessors.UriSchemeAccessor (null, accessor.Entity, false)
			{
				ViewControllerMode = ViewControllerMode.UriSchemeEdition
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
