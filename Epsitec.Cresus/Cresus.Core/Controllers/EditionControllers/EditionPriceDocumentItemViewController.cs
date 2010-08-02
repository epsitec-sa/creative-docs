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
using Epsitec.Cresus.Core.Helpers;

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
			this.tileContainer = container;

			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.PriceDocumentItem", "Ligne de total");

				this.CreateTabBook (builder);
				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateTabBook(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin (tile, horizontalSeparator: false);

			List<string> pagesDescription = new List<string> ();
			pagesDescription.Add ("Text.Texte");
			pagesDescription.Add ("Article.Article");
			pagesDescription.Add ("Price.Total");
			this.tabBookContainer = builder.CreateTabBook (tile, pagesDescription, "Price", this.HandleTabBookAction);
		}

		private void HandleTabBookAction(string tabPageName)
		{
			if (tabPageName == "Price")
			{
				return;
			}

			Common.ChangeEditedLineEntity (this.tileContainer, this.DataContext, this.Entity, tabPageName);
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
			builder.CreateTextField  (tile, 80, "Rabais de groupe (pourcent ou montant)", Marshaler.Create (this.GetDiscount, this.SetDiscount));
			builder.CreateStaticText (tile, 16, "<b>— ou —</b>");
			builder.CreateTextField  (tile, 80, "Montant total du groupe arrêté TTC",     Marshaler.Create (this.GetFixedPriceAfterTax, this.SetFixedPriceAfterTax));
#endif
		}


		private string GetDiscount()
		{
			if (this.Entity.Discount.DiscountRate.HasValue)
			{
				return Misc.PercentToString (this.Entity.Discount.DiscountRate.Value);
			}

			if (this.Entity.Discount.DiscountAmount.HasValue)
			{
				return Misc.PriceToString (this.Entity.Discount.DiscountAmount.Value);
			}

			return null;
		}

		private void SetDiscount(string value)
		{
			value = value.Trim ();

			decimal? v = null;
			bool percent = false;

			if (value.EndsWith ("%"))
			{
				v = Misc.StringToDecimal (value.Substring (0, value.Length-1)) / 100.0M;
				percent = true;
			}
			else
			{
				v = Misc.StringToDecimal (value);
			}

			if (v == null)
			{
				this.Entity.Discount.DiscountRate   = null;
				this.Entity.Discount.DiscountAmount = null;
			}
			else
			{
				if (percent)
				{
					this.Entity.Discount.DiscountRate = v;
					this.Entity.Discount.DiscountAmount = null;
				}
				else
				{
					this.Entity.Discount.DiscountRate = null;
					this.Entity.Discount.DiscountAmount = v;
				}
			}

			if (this.Entity.Discount.DiscountRate.HasValue || this.Entity.Discount.DiscountAmount.HasValue)
			{
				this.Entity.FixedPriceAfterTax = null;
			}

			this.UpdatePrices ();
		}


		private string GetFixedPriceAfterTax()
		{
			return Misc.PriceToString (this.Entity.FixedPriceAfterTax);
		}

		private void SetFixedPriceAfterTax(string value)
		{
			this.Entity.FixedPriceAfterTax = Misc.StringToDecimal (value);

			if (this.Entity.FixedPriceAfterTax.HasValue)
			{
				this.Entity.Discount.DiscountRate   = null;
				this.Entity.Discount.DiscountAmount = null;
			}

			this.UpdatePrices ();
		}


		private void UpdatePrices()
		{
			var invoiceDocument = Common.GetParentEntity (this.tileContainer) as InvoiceDocumentEntity;

			if (invoiceDocument != null)
			{
				InvoiceDocumentHelper.UpdatePrices (invoiceDocument);
			}

			this.tileContainer.UpdateAllWidgets ();
		}

	
		protected override EditionStatus GetEditionStatus()
		{
			return EditionStatus.Valid;
		}


		private TileContainer							tileContainer;
		private Epsitec.Common.Widgets.FrameBox			tabBookContainer;
	}
}
