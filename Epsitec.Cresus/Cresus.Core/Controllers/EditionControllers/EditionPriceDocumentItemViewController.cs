//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionPriceDocumentItemViewController : EditionViewController<Entities.PriceDocumentItemEntity>
	{
		public EditionPriceDocumentItemViewController(string name, Entities.PriceDocumentItemEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.PriceDocumentItem", "Ligne de prix");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

#if false
			builder.CreateTextField (tile, 0, "TextForPrimaryPrice", Marshaler.Create (() => this.Entity.TextForPrimaryPrice, x => this.Entity.TextForPrimaryPrice = x));
			builder.CreateTextField (tile, 0, "TextForResultingPrice", Marshaler.Create (() => this.Entity.TextForResultingPrice, x => this.Entity.TextForResultingPrice = x));
			builder.CreateTextField (tile, 0, "TextForFixedPrice", Marshaler.Create (() => this.Entity.TextForFixedPrice, x => this.Entity.TextForFixedPrice = x));
			builder.CreateTextField (tile, 0, "TextForTax", Marshaler.Create (() => this.Entity.TextForTax, x => this.Entity.TextForTax = x));

			builder.CreateMargin (tile,	horizontalSeparator: true);

			builder.CreateTextField (tile, 0, "Discount.Description", Marshaler.Create (() => this.Entity.Discount.Description, x => this.Entity.Discount.Description = x));
			builder.CreateTextField (tile, 60, "Discount.DiscountRate", Marshaler.Create (() => this.Entity.Discount.DiscountRate, x => this.Entity.Discount.DiscountRate = x));
			builder.CreateTextField (tile, 120, "Discount.DiscountAmount", Marshaler.Create (() => this.Entity.Discount.DiscountAmount, x => this.Entity.Discount.DiscountAmount = x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			builder.CreateTextField (tile, 120, "PrimaryPriceBeforeTax", Marshaler.Create (() => this.Entity.PrimaryPriceBeforeTax, x => this.Entity.PrimaryPriceBeforeTax = x));
			builder.CreateTextField (tile, 120, "PrimaryTax", Marshaler.Create (() => this.Entity.PrimaryTax, x => this.Entity.PrimaryTax = x));
			builder.CreateTextField (tile, 120, "ResultingPriceBeforeTax", Marshaler.Create (() => this.Entity.ResultingPriceBeforeTax, x => this.Entity.ResultingPriceBeforeTax = x));
			builder.CreateTextField (tile, 120, "ResultingTax", Marshaler.Create (() => this.Entity.ResultingTax, x => this.Entity.ResultingTax = x));
			builder.CreateTextField (tile, 120, "FixedPriceBeforeTax", Marshaler.Create (() => this.Entity.FixedPriceBeforeTax, x => this.Entity.FixedPriceBeforeTax = x));
			builder.CreateTextField (tile, 120, "FixedPriceAfterTax", Marshaler.Create (() => this.Entity.FixedPriceAfterTax, x => this.Entity.FixedPriceAfterTax = x));
			builder.CreateTextField (tile, 120, "FinalPriceBeforeTax", Marshaler.Create (() => this.Entity.FinalPriceBeforeTax, x => this.Entity.FinalPriceBeforeTax = x));
			builder.CreateTextField (tile, 120, "FinalTax", Marshaler.Create (() => this.Entity.FinalTax, x => this.Entity.FinalTax = x));
#else
			if (this.Entity.Discount.IsActive ())
			{
				builder.CreateTextField (tile, 0, "TextForPrimaryPrice", Marshaler.Create (() => this.Entity.TextForPrimaryPrice, x => this.Entity.TextForPrimaryPrice = x));
				builder.CreateTextField (tile, 0, "TextForResultingPrice", Marshaler.Create (() => this.Entity.TextForResultingPrice, x => this.Entity.TextForResultingPrice = x));

				builder.CreateMargin (tile, horizontalSeparator: true);

				builder.CreateTextField (tile, 0, "Discount.Description", Marshaler.Create (() => this.Entity.Discount.Description, x => this.Entity.Discount.Description = x));
				builder.CreateTextField (tile, 60, "Discount.DiscountRate", Marshaler.Create (() => this.Entity.Discount.DiscountRate, x => this.Entity.Discount.DiscountRate = x));
				builder.CreateTextField (tile, 120, "Discount.DiscountAmount", Marshaler.Create (() => this.Entity.Discount.DiscountAmount, x => this.Entity.Discount.DiscountAmount = x));

				builder.CreateMargin (tile, horizontalSeparator: true);

				builder.CreateTextField (tile, 120, "PrimaryPriceBeforeTax", Marshaler.Create (() => this.Entity.PrimaryPriceBeforeTax, x => this.Entity.PrimaryPriceBeforeTax = x));
				builder.CreateTextField (tile, 120, "PrimaryTax", Marshaler.Create (() => this.Entity.PrimaryTax, x => this.Entity.PrimaryTax = x));
				builder.CreateTextField (tile, 120, "ResultingPriceBeforeTax", Marshaler.Create (() => this.Entity.ResultingPriceBeforeTax, x => this.Entity.ResultingPriceBeforeTax = x));
				builder.CreateTextField (tile, 120, "ResultingTax", Marshaler.Create (() => this.Entity.ResultingTax, x => this.Entity.ResultingTax = x));
				builder.CreateTextField (tile, 120, "FixedPriceBeforeTax", Marshaler.Create (() => this.Entity.FixedPriceBeforeTax, x => this.Entity.FixedPriceBeforeTax = x));
				builder.CreateTextField (tile, 120, "FixedPriceAfterTax", Marshaler.Create (() => this.Entity.FixedPriceAfterTax, x => this.Entity.FixedPriceAfterTax = x));
				builder.CreateTextField (tile, 120, "FinalPriceBeforeTax", Marshaler.Create (() => this.Entity.FinalPriceBeforeTax, x => this.Entity.FinalPriceBeforeTax = x));
				builder.CreateTextField (tile, 120, "FinalTax", Marshaler.Create (() => this.Entity.FinalTax, x => this.Entity.FinalTax = x));
			}
			else
			{
				builder.CreateTextField (tile, 0, "TextForFixedPrice", Marshaler.Create (() => this.Entity.TextForFixedPrice, x => this.Entity.TextForFixedPrice = x));
				builder.CreateTextField (tile, 0, "TextForTax", Marshaler.Create (() => this.Entity.TextForTax, x => this.Entity.TextForTax = x));

				builder.CreateMargin (tile, horizontalSeparator: true);

				builder.CreateTextField (tile, 120, "PrimaryPriceBeforeTax", Marshaler.Create (() => this.Entity.PrimaryPriceBeforeTax, x => this.Entity.PrimaryPriceBeforeTax = x));
				builder.CreateTextField (tile, 120, "PrimaryTax", Marshaler.Create (() => this.Entity.PrimaryTax, x => this.Entity.PrimaryTax = x));
				builder.CreateTextField (tile, 120, "ResultingPriceBeforeTax", Marshaler.Create (() => this.Entity.ResultingPriceBeforeTax, x => this.Entity.ResultingPriceBeforeTax = x));
				builder.CreateTextField (tile, 120, "ResultingTax", Marshaler.Create (() => this.Entity.ResultingTax, x => this.Entity.ResultingTax = x));
				builder.CreateTextField (tile, 120, "FixedPriceBeforeTax", Marshaler.Create (() => this.Entity.FixedPriceBeforeTax, x => this.Entity.FixedPriceBeforeTax = x));
				builder.CreateTextField (tile, 120, "FixedPriceAfterTax", Marshaler.Create (() => this.Entity.FixedPriceAfterTax, x => this.Entity.FixedPriceAfterTax = x));
				builder.CreateTextField (tile, 120, "FinalPriceBeforeTax", Marshaler.Create (() => this.Entity.FinalPriceBeforeTax, x => this.Entity.FinalPriceBeforeTax = x));
				builder.CreateTextField (tile, 120, "FinalTax", Marshaler.Create (() => this.Entity.FinalTax, x => this.Entity.FinalTax = x));
			}
#endif
		}


		protected override EditionStatus GetEditionStatus()
		{
			return EditionStatus.Valid;
		}
	}
}
