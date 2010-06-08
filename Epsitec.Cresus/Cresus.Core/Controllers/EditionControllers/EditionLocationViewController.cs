//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionLocationViewController : EditionViewController<Entities.LocationEntity>
	{
		public EditionLocationViewController(string name, Entities.LocationEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
			var builder = new UIBuilder (container, this);

			builder.CreateHeaderEditorTile ();
			builder.CreateEditionGroupingTile ("Data.Mail", "Ville");

			builder.CreateFooterEditorTile ();

			this.CreateUIMain (builder);

			builder.CreateFooterEditorTile ();

			UI.SetInitialFocus (container);
		}


		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateWarning   (tile);
			builder.CreateTextField (tile, 50, "Numéro postal", Marshaler.Create (() => this.Entity.PostalCode, x => this.Entity.PostalCode = x));
			builder.CreateTextField (tile,  0, "Ville",         Marshaler.Create (() => this.Entity.Name,       x => this.Entity.Name = x));
		}
	}
}
