//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionArticleGroupViewController : EditionViewController<Entities.ArticleGroupEntity>
	{
#if true
		protected override void CreateBricks(BrickWall<ArticleGroupEntity> wall)
		{
			wall.AddBrick ()
				.GlobalWarning ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.Description)
				.End ()
				;
		}
#else
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Group", "Groupe d'article");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateWarning   (tile);
			builder.CreateTextField (tile, 0, "Nom court", Marshaler.Create (() => this.Entity.Name,        x => this.Entity.Name        = x));
			builder.CreateTextField (tile, 0, "Nom long",  Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));
		}
#endif
	}
}
