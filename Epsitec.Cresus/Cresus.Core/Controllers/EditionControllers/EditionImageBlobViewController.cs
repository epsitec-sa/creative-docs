//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Bricks;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionImageBlobViewController : EditionViewController<Entities.ImageBlobEntity>
	{
#if true
		protected override void CreateBricks(BrickWall<ImageBlobEntity> wall)
		{
			wall.AddBrick ()
				// TODO: Comment mettre un texte fixe ?
				;
		}
#else
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.ImageBlob", "Image bitmap");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateStaticText (tile, 60, "<i>Aucune donnée ne peut être modifiée.</i>");
		}
#endif
	}
}
