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
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business.Finance;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionArticlePriceViewController : EditionViewController<Entities.ArticlePriceEntity>
	{
#if true
		protected override void CreateBricks(Bricks.BrickWall<ArticlePriceEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.BeginDate).Width (150)
				  .Field (x => x.EndDate).Width (150)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.MinQuantity).Width (100)
				  .Field (x => x.MaxQuantity).Width (100)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.ValueIncludesTaxes)
				  .Field (x => x.ValueOverridesPriceGroup)
				  .Field (x => x.Value).Width (100)
				  .Field (x => x.CurrencyCode)
				  .Field (x => x.PriceGroups)
				.End ()
				;
			wall.AddBrick (x => x.PriceCalculators)
				.Template ()
				.End ()
				;
		}
#else
		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIMain (data);
				this.CreateUIPriceCalculators (data);
			}
		}


		private void CreateUIMain(TileDataItems data)
		{
			var tileData = new TileDataItem
			{
				Name            = "ArticlePrice",
				IconUri	        = "Data.ArticlePrice",
				Title	        = TextFormatter.FormatText ("Prix"),
				CompactTitle    = TextFormatter.FormatText ("Prix"),
				CreateEditionUI = (tile, builder) =>
				{
					builder.CreateTextField (tile, 150, "Du", Marshaler.Create (() => this.Entity.BeginDate, x => this.Entity.BeginDate = x));
					builder.CreateTextField (tile, 150, "Au", Marshaler.Create (() => this.Entity.EndDate, x => this.Entity.EndDate   = x));

					builder.CreateMargin (tile, horizontalSeparator: true);

					builder.CreateTextField (tile, 80, "Quantité minimale", Marshaler.Create (() => this.Entity.MinQuantity, x => this.Entity.MinQuantity = x));
					builder.CreateTextField (tile, 80, "Quantité maximale", Marshaler.Create (() => this.Entity.MaxQuantity, x => this.Entity.MaxQuantity = x));

					builder.CreateMargin (tile, horizontalSeparator: true);

					//	TODO: gérer le HT/TTC selon this.Entity.ValueIncludesTaxes
					builder.CreateTextField (tile, 150, "Prix HT", Marshaler.Create (() => this.Entity.Value, x => this.Entity.Value = x));
					builder.CreateAutoCompleteTextField (tile, 150-Library.UI.ComboButtonWidth+1, "Monnaie", Marshaler.Create (() => this.Entity.CurrencyCode, x => this.Entity.CurrencyCode = x), EnumKeyValues.FromEnum<CurrencyCode> ());
				}
			};

			data.Add (tileData);
		}

		private void CreateUIPriceCalculators(TileDataItems data)
		{
			var tileDataItem = new TileDataItem
			{
				AutoGroup    = true,
				Name		 = "PriceCalculator",
				IconUri		 = "Data.PriceCalculator",
				Title		 = TextFormatter.FormatText ("Calculateurs de prix"),
				CompactTitle = TextFormatter.FormatText ("Calculateurs de prix"),
				Text		 = CollectionTemplate.DefaultEmptyText,
			};

			data.Add (tileDataItem);

			var template = new CollectionTemplate<PriceCalculatorEntity> ("PriceCalculator", this.BusinessContext);

			template.DefineText        (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetSummary ());

			data.Add (this.CreateCollectionAccessor (template, x => x.PriceCalculators));
		}
#endif
	}
}
