//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionTelecomContactViewController : EditionViewController<Entities.TelecomContactEntity>
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
			builder.CreateEditionGroupingTile ("Data.Telecom", "Téléphone");

			this.CreateUIRoles (builder);
			this.CreateUITelecomType (builder);
			this.CreateUIPhoneNumber (builder);
			
			builder.CreateFooterEditorTile ();

			UI.SetInitialFocus (container);
#endif
		}
		private void CreateUIRoles(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var controller = new SelectionController<Entities.ContactRoleEntity>
			{
				CollectionValueGetter    = () => this.Entity.Roles,
				PossibleItemsGetter      = () => CoreProgram.Application.Data.GetRoles (),
				ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name)
			};

			builder.CreateEditionDetailedRadio (0, "Choix du ou des rôles souhaités", controller);
		}
		
		private void CreateUITelecomType(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var controller = new SelectionController<Entities.TelecomTypeEntity>
			{
				ValueGetter              = () => this.Entity.TelecomType,
				ValueSetter              = x => this.Entity.TelecomType = x,
				PossibleItemsGetter      = () => CoreProgram.Application.Data.GetTelecomTypes (),
				ToFormattedTextConverter = x => UIBuilder.FormatText (x.Name)
			};
			
			builder.CreateEditionDetailedCheck (0, "Type du numéro de téléphone", controller);
		}
		
		private void CreateUIPhoneNumber(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();
			builder.CreateTextField (tile, 150, "Numéro de téléphone", Marshaler.Create (() => this.Entity.Number,    x => this.Entity.Number = x));
			builder.CreateTextField (tile, 100, "Numéro interne",      Marshaler.Create (() => this.Entity.Extension, x => this.Entity.Extension = x));
		}
	}
}
