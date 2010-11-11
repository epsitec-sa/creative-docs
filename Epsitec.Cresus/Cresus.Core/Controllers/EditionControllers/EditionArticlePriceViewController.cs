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
	public class EditionArticlePriceViewController : EditionViewController<Entities.ArticlePriceEntity>
	{
		public EditionArticlePriceViewController(string name, Entities.ArticlePriceEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.ArticlePrice", "Prix");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile, 150, "Du", Marshaler.Create (() => this.Entity.BeginDate, x => this.Entity.BeginDate = x));
			builder.CreateTextField (tile, 150, "Au", Marshaler.Create (() => this.Entity.EndDate, x => this.Entity.EndDate = x));
			
			builder.CreateMargin (tile, horizontalSeparator: true);
			
			//	TODO: gérer le HT/TTC selon this.Entity.ValueIncludesTaxes
			builder.CreateTextField (tile, 150, "Prix HT", Marshaler.Create (() => this.Entity.Value, x => this.Entity.Value = x));
			builder.CreateAutoCompleteTextField (tile, 150-UIBuilder.ComboButtonWidth+1, "Monnaie", Marshaler.Create (() => this.Entity.CurrencyCode, x => this.Entity.CurrencyCode = x), Business.Enumerations.GetAllPossibleCurrencyCodes (), x => TextFormatter.FormatText (x.Values[0], "-", x.Values[1]));
			
			builder.CreateMargin (tile, horizontalSeparator: true);
			
			builder.CreateTextField (tile, 80, "Quantité minimale", Marshaler.Create (() => this.Entity.MinQuantity, x => this.Entity.MinQuantity = x));
			builder.CreateTextField (tile, 80, "Quantité maximale", Marshaler.Create (() => this.Entity.MaxQuantity, x => this.Entity.MaxQuantity = x));
		}
	}
}
