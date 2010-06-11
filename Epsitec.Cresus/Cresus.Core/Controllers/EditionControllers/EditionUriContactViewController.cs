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
	public class EditionUriContactViewController : EditionViewController<Entities.UriContactEntity>
	{
		public EditionUriContactViewController(string name, Entities.UriContactEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
			var builder = new UIBuilder (container, this);

			builder.CreateHeaderEditorTile ();
			builder.CreateEditionGroupingTile ("Data.Uri", "E-Mail");

			this.CreateUIRoles (builder);
			this.CreateUIMail (builder);

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

			builder.CreateEditionDetailedCheck (0, "Choix du ou des rôles souhaités", controller);
		}

		private void CreateUIMail(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();
			builder.CreateTextField (tile, 0, "Adresse mail", Marshaler.Create (() => this.Entity.Uri, x => this.Entity.Uri = x));
		}
	}
}
