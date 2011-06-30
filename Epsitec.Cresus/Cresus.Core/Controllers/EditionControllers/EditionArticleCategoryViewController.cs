//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Bricks;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionArticleCategoryViewController : EditionViewController<Entities.ArticleCategoryEntity>
	{
#if true
		protected override void CreateBricks(BrickWall<ArticleCategoryEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.Description)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.DefaultBillingUnit)
				  .Field (x => x.DefaultPictures)
				  .Field (x => x.DefaultInputVatCode)
				  .Field (x => x.DefaultOutputVatCode)
				  .Field (x => x.VatNumber)
				  .Field (x => x.DefaultRoundingMode)
				  .Field (x => x.NeverApplyDiscount)
				  .Field (x => x.ArticleType)
				.End ()
				;
		}
#else
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Category", "Catégorie");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateWarning               (tile);
			builder.CreateTextField             (tile, 0, "Nom",                    Marshaler.Create (() => this.Entity.Name,                 x => this.Entity.Name = x));
			builder.CreateAutoCompleteTextField (tile, 0, "Code TVA à l'achat",		Marshaler.Create (() => this.Entity.DefaultInputVatCode,  x => this.Entity.DefaultInputVatCode = x),  EnumKeyValues.FromEnum<VatCode> ());
			builder.CreateAutoCompleteTextField (tile, 0, "Code TVA à la vente",	Marshaler.Create (() => this.Entity.DefaultOutputVatCode, x => this.Entity.DefaultOutputVatCode = x), EnumKeyValues.FromEnum<VatCode> ());
			builder.CreateTextField             (tile, 0, "Numéro TVA",             Marshaler.Create (() => this.Entity.VatNumber,            x => this.Entity.VatNumber = x));
			builder.CreateAutoCompleteTextField (tile, 0, "Type d'article",         Marshaler.Create (() => this.Entity.ArticleType,          x => this.Entity.ArticleType = x),          EnumKeyValues.FromEnum<ArticleType> ());
		}
#endif
	}
}
