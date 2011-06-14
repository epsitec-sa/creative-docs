//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionContactGroupViewController : EditionViewController<Entities.ContactGroupEntity>
	{
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.ContactGroup", "Rôle");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateWarning   (tile);
			builder.CreateTextField (tile, 0, "Nom", Marshaler.Create (() => this.Entity.Name, x => this.Entity.Name = x));
		}
	}
}
