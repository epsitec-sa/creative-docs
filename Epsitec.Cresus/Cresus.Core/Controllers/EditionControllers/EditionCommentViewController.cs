//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionCommentViewController : EditionViewController<Entities.CommentEntity>
	{
#if true
		protected override void CreateBricks(Bricks.BrickWall<CommentEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Title ("Texte")
				  .Field (x => x.Text).Height (120)
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
				builder.CreateEditionTitleTile ("Data.Comment", "Commentaire");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextFieldMulti (tile, 120, "Texte", Marshaler.Create (() => this.Entity.Text, x => this.Entity.Text = x));
		}
#endif
	}
}
