﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Bricks;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionCountryViewController : EditionViewController<Entities.CountryEntity>
	{
#if true
		protected override void CreateBricks(BrickWall<CountryEntity> wall)
		{
			wall.AddBrick ()
				.GlobalWarning ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.CountryCode).Width (60)
				  .Field (x => x.IsPreferred)
				.End ()
				;
		}
#endif

#if false
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Country", "Pays");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateWarning   (tile);
			builder.CreateTextField (tile,  0, "Pays",                    Marshaler.Create (() => this.Entity.Name, x => this.Entity.Name = x));
			builder.CreateTextField (tile, 80, "Code ISO à deux lettres", Marshaler.Create (() => this.Entity.CountryCode, x => this.Entity.CountryCode = x));
		}
#endif
	}
}
