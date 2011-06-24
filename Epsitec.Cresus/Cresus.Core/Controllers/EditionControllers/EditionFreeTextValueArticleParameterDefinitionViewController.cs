//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionFreeTextValueArticleParameterDefinitionViewController : EditionViewController<Entities.FreeTextValueArticleParameterDefinitionEntity>
	{
#if true
		protected override void CreateBricks(Bricks.BrickWall<FreeTextValueArticleParameterDefinitionEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.Description)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.ShortText)
				  .Field (x => x.LongText)
				.End ()
				;
		}
#else
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.ArticleParameter", "Paramètre");

				this.CreateTabBook (builder);
				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateTabBook(UIBuilder builder)
		{
			Common.CreateAbstractArticleParameterTabBook (builder, this, ArticleParameterTabId.FreeText);
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile, 80, "Nom court (un ou deux caractères)", Marshaler.Create (() => this.Entity.Name, x => this.Entity.Name = x));
			builder.CreateTextField      (tile,  0, "Nom long",    Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateTextField      (tile,  0, "Texte court", Marshaler.Create (() => this.Entity.ShortText, x => this.Entity.ShortText = x));
			builder.CreateTextFieldMulti (tile, 52, "Texte long",  Marshaler.Create (() => this.Entity.LongText,  x => this.Entity.LongText = x));
		}
#endif
	}
}
