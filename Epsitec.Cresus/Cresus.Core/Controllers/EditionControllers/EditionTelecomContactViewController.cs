//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionTelecomContactViewController : EntityViewController<Entities.TelecomContactEntity>
	{
		public EditionTelecomContactViewController(string name, Entities.TelecomContactEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
#if false
			UIBuilder builder = new UIBuilder (container, this);
			
			TitleTile group;
			EditionTile tile;

			System.Diagnostics.Debug.Assert (this.Entity != null);
			var accessor = new Accessors.TelecomContactAccessor (null, this.Entity, false);

			//	Crée les tuiles.
			builder.CreateHeaderEditorTile ();

			//	Crée le contenu de la tuile d'édition.
			group = builder.CreateSummaryGroupingTile ("Data.Roles", "Rôles");

			var roleAccessor = new Accessors.RolesContactAccessor (null, accessor.TelecomContact, false)
			{
				ViewControllerMode = ViewControllerMode.RolesEdition
			};

			builder.CreateSummaryTile (group, roleAccessor);

			//	Crée le contenu de la tuile d'édition.
			group = builder.CreateSummaryGroupingTile ("Data.Type", "Type");

			var telecomTypeAccessor = new Accessors.TelecomTypeAccessor (null, accessor.TelecomContact, false)
			{
				ViewControllerMode = ViewControllerMode.TelecomTypeEdition
			};

			builder.CreateSummaryTile (group, telecomTypeAccessor);

			//	Crée le contenu de la tuile d'édition.
			group = builder.CreateEditionGroupingTile ("Data.Telecom", "Téléphone");
			tile = builder.CreateEditionTile (group, accessor);

			builder.CreateLinkButtons (tile.Container);

			builder.CreateTextField (tile.Container, 150, "Numéro de téléphone", accessor.TelecomContact.Number, x => accessor.TelecomContact.Number = x, Validators.StringValidator.Validate);
			builder.CreateTextField (tile.Container, 100, "Numéro interne", accessor.TelecomContact.Extension, x => accessor.TelecomContact.Extension = x, Validators.StringValidator.Validate);

			UI.SetInitialFocus (container);
#else
			var builder = new UIBuilder (container, this);

			builder.CreateHeaderEditorTile ();

			var person = this.Entity;
			var group = builder.CreateEditionGroupingTile ("Data.Telecom", "Téléphone");
			var roleTile = builder.CreateEditionTile (group, this.Entity);
			var typeTile = builder.CreateEditionTile (group, this.Entity);
			var mainTile = builder.CreateEditionTile (group, this.Entity);

			builder.CreateFooterEditorTile ();

			builder.CreateDetailed (roleTile, 0, "Choix du ou des rôles souhaités", true, this.Entity.Roles, null);  // TODO: remplacer 'null' par qq chose de réel
			builder.CreateDetailed (typeTile, 0, "Type du numéro de téléphone", false, this.Entity.TelecomType, null);  // TODO: remplacer 'null' par qq chose de réel

			builder.CreateTextField (mainTile.Container, 150, "Numéro de téléphone", this.Entity.Number, x => this.Entity.Number = x, Validators.StringValidator.Validate);
			builder.CreateTextField (mainTile.Container, 100, "Numéro interne", this.Entity.Extension, x => this.Entity.Extension = x, Validators.StringValidator.Validate);

			UI.SetInitialFocus (container);
#endif
		}
	}
}
