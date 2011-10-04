//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business.Finance;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionArticleAccountingDefinitionViewController : EditionViewController<Entities.ArticleAccountingDefinitionEntity>
	{
#if true
		protected override void CreateBricks(BrickWall<ArticleAccountingDefinitionEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Name)
				  .Field (x => x.Description)
				  .Field (x => x.AccountingOperation)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.BeginDate)
				  .Field (x => x.EndDate)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.TransactionBookAccount)
				  .Field (x => x.VatBookAccount)
				  .Field (x => x.DiscountBookAccount)
				  .Field (x => x.RoundingBookAccount)
				.End ()
				;
		}
#else
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.ArticleAccountingDefinition", "Comptabilisation");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile, 150, "Du", Marshaler.Create (() => this.Entity.BeginDate, x => this.Entity.BeginDate = x));
			builder.CreateTextField (tile, 150, "Au", Marshaler.Create (() => this.Entity.EndDate,   x => this.Entity.EndDate   = x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateAccountEditor (tile, "Compte pour les ventes",                Marshaler.Create (() => this.Entity.SellingBookAccount,          x => this.Entity.SellingBookAccount = x));
			builder.CreateAccountEditor (tile, "Compte pour les rabais sur les ventes", Marshaler.Create (() => this.Entity.SellingDiscountBookAccount,  x => this.Entity.SellingDiscountBookAccount = x));

			builder.CreateMargin (tile, horizontalSeparator: true);
			
			builder.CreateAccountEditor (tile, "Compte pour les achats",                Marshaler.Create (() => this.Entity.PurchaseBookAccount,         x => this.Entity.PurchaseBookAccount = x));
			builder.CreateAccountEditor (tile, "Compte pour les rabais sur les achats", Marshaler.Create (() => this.Entity.PurchaseDiscountBookAccount, x => this.Entity.PurchaseDiscountBookAccount = x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateAutoCompleteTextField (tile, 150-Library.UI.ComboButtonWidth+1, "Monnaie", Marshaler.Create (() => this.Entity.CurrencyCode, x => this.Entity.CurrencyCode = x), EnumKeyValues.FromEnum<CurrencyCode> ());
		}
#endif
	}
}
