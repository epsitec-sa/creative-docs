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
			var builder = new UIBuilder (container, this);

			builder.CreateHeaderEditorTile ();
			builder.CreateEditionGroupingTile ("Data.Telecom", "Téléphone");

			this.CreateUIRoles (builder);
			this.CreateUITelecomType (builder);
			this.CreateUIPhoneNumber (builder);
			
			builder.CreateFooterEditorTile ();

			UI.SetInitialFocus (container);
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
