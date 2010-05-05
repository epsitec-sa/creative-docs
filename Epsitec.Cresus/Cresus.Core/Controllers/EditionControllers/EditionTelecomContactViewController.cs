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
	public class EditionTelecomContactViewController : EntityViewController<Entities.TelecomContactEntity>
	{
		public EditionTelecomContactViewController(string name, Entities.TelecomContactEntity entity)
			: base (name, entity)
		{
		}

		public override void CreateUI(Widget container)
		{
			UIBuilder builder = new UIBuilder (container);
			
			Widgets.GroupingTile group;
			Widgets.EditionTile tile;

			System.Diagnostics.Debug.Assert (this.Entity != null);
			var accessor = new EntitiesAccessors.TelecomContactAccessor (null, this.Entity, false);

			//	Crée les tuiles.
			builder.CreateHeaderEditorTile ();

			//	Crée le contenu de la tuile d'édition.
			group = builder.CreateGroupingTile ("Data.Roles", "Rôles", false);

			var roleAccessor = new EntitiesAccessors.RolesContactAccessor (null, accessor.TelecomContact, false)
			{
				ViewControllerMode = ViewControllerMode.RolesEdition
			};

			builder.CreateSummaryTile (group, roleAccessor, this);

			//	Crée le contenu de la tuile d'édition.
			group = builder.CreateGroupingTile ("Data.Type", "Type", false);

			var telecomTypeAccessor = new EntitiesAccessors.TelecomTypeAccessor (null, accessor.TelecomContact, false)
			{
				ViewControllerMode = ViewControllerMode.TelecomTypeEdition
			};

			builder.CreateSummaryTile (group, telecomTypeAccessor, this);

			//	Crée le contenu de la tuile d'édition.
			group = builder.CreateGroupingTile ("Data.Telecom", "Téléphone", true);
			tile = builder.CreateEditionTile (group, accessor, this);

			builder.CreateLinkButtons (tile.Container);

			builder.CreateTextField (tile.Container, 150, "Numéro de téléphone", accessor.TelecomContact.Number, x => accessor.TelecomContact.Number = x, Validators.StringValidator.Validate);
			builder.CreateTextField (tile.Container, 100, "Numéro interne", accessor.TelecomContact.Extension, x => accessor.TelecomContact.Extension = x, Validators.StringValidator.Validate);

			UI.SetInitialFocus (container);
		}
	}
}
