//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionInvoiceDocumentViewController : EditionViewController<Entities.InvoiceDocumentEntity>
	{
		public EditionInvoiceDocumentViewController(string name, Entities.InvoiceDocumentEntity entity)
			: base (name, entity)
		{
		}


		protected override EditionStatus GetEditionStatus()
		{
			if (string.IsNullOrEmpty (this.Entity.IdA) &&
				string.IsNullOrEmpty (this.Entity.IdB) &&
				string.IsNullOrEmpty (this.Entity.IdC))
			{
				return EditionStatus.Empty;
			}

			if (string.IsNullOrEmpty (this.Entity.IdA) &&
				(!string.IsNullOrEmpty (this.Entity.IdB) ||
				 !string.IsNullOrEmpty (this.Entity.IdC)))
			{
				return EditionStatus.Invalid;
			}

			return EditionStatus.Valid;
		}

		protected override void UpdateEmptyEntityStatus(DataLayer.DataContext context, bool isEmpty)
		{
			var entity = this.Entity;
			context.UpdateEmptyEntityStatus (entity, isEmpty);
		}

	
		protected override void CreateUI(TileContainer container)
		{
			this.tileContainer = container;

			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.InvoiceDocument", "Facture");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}

			//	Summary:
			var containerController = new TileContainerController (this, container);
			var data = containerController.DataItems;

			this.CreateUILines (data);
			this.CreateUIBillings (data);
			this.CreateUIComments (data);

			containerController.GenerateTiles ();
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField      (tile, 150, "Numéro de la facture", Marshaler.Create (() => this.IdA, x => this.IdA = x));
			builder.CreateTextField      (tile, 150, "Numéro externe",       Marshaler.Create (() => this.IdB, x => this.IdB = x));
			builder.CreateTextField      (tile, 150, "Numéro interne",       Marshaler.Create (() => this.IdC, x => this.IdC = x));
			builder.CreateMargin         (tile, horizontalSeparator: true);
			builder.CreateTextField      (tile,   0, "Description",          Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));

			FrameBox group = builder.CreateGroup (tile, "Total arrêté");
			             builder.CreateTextField (group, DockStyle.Left, 80, Marshaler.Create (() => this.FixedPrice, x => this.FixedPrice = x));
			var button = builder.CreateButton    (group, DockStyle.Fill, 0, "Recalculer la facture");

			button.Clicked += delegate
			{
				InvoiceDocumentHelper.UpdatePrices (this.Entity, this.DataContext);
				InvoiceDocumentHelper.UpdateDialogs (this.Entity);
				this.tileContainer.UpdateAllWidgets ();
			};
		}

		private void CreateUILines(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup          = true,
					Name		       = "DocumentItem",
					IconUri		       = "Data.DocumentItems",
					Title		       = UIBuilder.FormatText ("Lignes"),
					CompactTitle       = UIBuilder.FormatText ("Lignes"),
					Text		       = CollectionTemplate.DefaultEmptyText,
					ViewControllerMode = ViewControllerMode.Creation,  // TODO: Pourquoi ce mode n'est pas passé à SummaryData.CreateSubViewController ???
				});

			var template = new CollectionTemplate<AbstractDocumentItemEntity> ("DocumentItem", data.Controller)
				.DefineText        (x => UIBuilder.FormatText (GetDocumentItemSummary (x)))
				.DefineCompactText (x => UIBuilder.FormatText (GetDocumentItemSummary (x)));

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Lines, template));
		}

		private void CreateUIBillings(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup          = true,
					Name		       = "BillingDetails",
					IconUri		       = "Data.BillingDetails",
					Title		       = UIBuilder.FormatText ("Payements"),
					CompactTitle       = UIBuilder.FormatText ("Payements"),
					Text		       = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<BillingDetailsEntity> ("BillingDetails", data.Controller)
				.DefineText        (x => UIBuilder.FormatText (GetBillingDetailsSummary (x)))
				.DefineCompactText (x => UIBuilder.FormatText (GetBillingDetailsSummary (x)));

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.BillingDetails, template));
		}

		private void CreateUIComments(SummaryDataItems data)
		{
			SummaryControllers.Common.CreateUIComments (data, this.EntityGetter, x => x.Comments);
		}


		private static string GetDocumentItemSummary(AbstractDocumentItemEntity documentItemEntity)
		{
			if (documentItemEntity is TextDocumentItemEntity)
			{
				return GetTextDocumentItemSummary (documentItemEntity as TextDocumentItemEntity);
			}

			if (documentItemEntity is ArticleDocumentItemEntity)
			{
				return GetArticleDocumentItemSummary (documentItemEntity as ArticleDocumentItemEntity);
			}

			if (documentItemEntity is PriceDocumentItemEntity)
			{
				return GetPriceDocumentItemSummary (documentItemEntity as PriceDocumentItemEntity);
			}

			return null;			
		}

		private static string GetTextDocumentItemSummary(TextDocumentItemEntity x)
		{
			return string.Concat ("<i>Texte</i><tab/>", x.Text);
		}

		private static string GetArticleDocumentItemSummary(ArticleDocumentItemEntity x)
		{
			var quantity = ArticleDocumentItemHelper.GetArticleQuantityAndUnit (x);
			var desc = Misc.FirstLine (ArticleDocumentItemHelper.GetArticleDescription (x));

			return string.Concat ("<i>Article</i><tab/>", string.Join (" ", quantity, desc));
		}

		private static string GetPriceDocumentItemSummary(PriceDocumentItemEntity x)
		{
			if (x.Discount.IsActive ())
			{
				if (x.Discount.DiscountRate.HasValue)
				{
					return string.Concat ("<i>Prix</i><tab/>Rabais ", Misc.PercentToString (x.Discount.DiscountRate.Value));
				}
			}
			else
			{
				string desc = x.TextForFixedPrice;
				if (string.IsNullOrEmpty (desc))
				{
					desc = "Total arrêté";
				}

				string total = null;
				if (x.FixedPriceAfterTax.HasValue)
				{
					total = Misc.PriceToString (x.FixedPriceAfterTax.Value);
				}

				return string.Concat ("<i>Prix</i><tab/>", desc, " ", total);
			}

			return null;
		}


		private static string GetBillingDetailsSummary(BillingDetailsEntity billingDetailsEntity)
		{
			string amount = Misc.PriceToString (billingDetailsEntity.AmountDue.Amount);
			string title = Misc.FirstLine (billingDetailsEntity.Title);

			return string.Concat (amount, " ", title);
		}


		private string IdA
		{
			get
			{
				return this.Entity.IdA;
			}
			set
			{
				if (this.Entity.IdA != value)
				{
					this.Entity.IdA = value;
					InvoiceDocumentHelper.UpdateDialogs (this.Entity);
				}
			}
		}

		private string IdB
		{
			get
			{
				return this.Entity.IdB;
			}
			set
			{
				if (this.Entity.IdB != value)
				{
					this.Entity.IdB = value;
					InvoiceDocumentHelper.UpdateDialogs (this.Entity);
				}
			}
		}

		private string IdC
		{
			get
			{
				return this.Entity.IdC;
			}
			set
			{
				if (this.Entity.IdC != value)
				{
					this.Entity.IdC = value;
					InvoiceDocumentHelper.UpdateDialogs (this.Entity);
				}
			}
		}

		private decimal FixedPrice
		{
			get
			{
				return InvoiceDocumentHelper.GetFixedPrice (this.Entity);
			}
			set
			{
				InvoiceDocumentHelper.SetFixedPrice (this.Entity, this.DataContext, value);
				InvoiceDocumentHelper.UpdateDialogs (this.Entity);
			}
		}


		private TileContainer					tileContainer;
	}
}
