//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.TabIds;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionPriceDocumentItemViewController : EditionViewController<Entities.SubTotalDocumentItemEntity>
	{
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.PriceDocumentItem", "Ligne de sous-total");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateStaticText (tile, 70, "Cette ligne effectue un sous-total de tous les articles précédents. Vous pouvez spécifier ci-dessous un rabais facultatif pour l'ensemble du sous-groupe.");

			builder.CreateTextField (tile, 80, false, "Rabais (pourcent ou montant)", Marshaler.Create (() => this.GetDiscount (), this.SetDiscount));
			builder.CreateStaticText (tile, 16, "<b>— ou —</b>");
		}


		private string GetDiscount()
		{
			if (this.Entity.Discount.DiscountRate.HasValue)
			{
				return Misc.PercentToString (this.Entity.Discount.DiscountRate.Value);
			}

			if (this.Entity.Discount.Value.HasValue)
			{
				return Misc.PriceToString (this.Entity.Discount.Value.Value);
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
				this.Entity.Discount.Value = null;
			}
			else
			{
				if (percent)
				{
					this.Entity.Discount.DiscountRate = v;
					this.Entity.Discount.Value = null;
				}
				else
				{
					this.Entity.Discount.DiscountRate = null;
					this.Entity.Discount.Value = v;
				}
			}

			this.UpdatePrices ();
		}


		private void UpdatePrices()
		{
#if false
			var invoiceDocument = Common.GetParentEntity (this.TileContainer) as BusinessDocumentEntity;

			if (invoiceDocument != null)
			{
				InvoiceDocumentHelper.UpdatePrices (invoiceDocument, this.DataContext);
			}

			this.TileContainer.UpdateAllWidgets ();
#endif
		}
	}
}
