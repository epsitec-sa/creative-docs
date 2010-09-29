//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryInvoiceDocumentViewController : SummaryViewController<Entities.InvoiceDocumentEntity>
	{
		public SummaryInvoiceDocumentViewController(string name, Entities.InvoiceDocumentEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				using (var data = TileContainerController.Setup (builder))
				{
					this.CreateUIInvoice (data);
					this.CreateUIArticleLines (data);
					this.CreateUIBillings (data);
					this.CreateUIComments (data);
				}
				
				// TODO: faire en sorte que cette tuile viennent après les lignes d'article !
				this.CreateUITotalSummary (builder);
			}

			this.CreateUIPreviewPanel ();
		}


		protected override void AboutToCloseUI()
		{
			this.CloseUIPreviewPanel ();
			base.AboutToCloseUI ();
		}

		
		private void CreateUIPreviewPanel()
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
			Printers.AbstractEntityPrinter entityPrinter = Printers.AbstractEntityPrinter.CreateEntityPrinter (this.Entity);

			if (entityPrinter == null)
			{
				return;
			}

			var mainViewController = this.Orchestrator.MainViewController;
			var previewController  = mainViewController.PreviewViewController;

			entityPrinter.DefaultPrepare (Printers.DocumentType.InvoiceWithInsideESR);
			entityPrinter.IsPreview = true;
			entityPrinter.SetPrinterUnit ();
			entityPrinter.BuildSections ();

			mainViewController.SetPreviewPanelVisibility (true);

			var previewFrame = new FrameBox ()
			{
				Dock = DockStyle.Fill,
				Padding = new Margins (5),
				BackColor = adorner.ColorWindow,
			};

			var printerUnitsToolbarBox = new FrameBox
			{
				Parent = previewFrame,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 10),
			};

			var previewBox = new FrameBox
			{
				Parent = previewFrame,
				Dock = DockStyle.Fill,
			};

			var pagesToolbarBox = new FrameBox
			{
				Parent = previewFrame,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, 0, 10, 0),
			};

			this.previewerController = new Printers.PreviewerController (entityPrinter, new AbstractEntity[] { this.Entity });
			this.previewerController.CreateUI (previewBox, pagesToolbarBox, printerUnitsToolbarBox);

			previewController.Add (previewFrame);
			previewController.Updating += this.HandlePreviewPanelUpdating;
		}

		private void CloseUIPreviewPanel()
		{
			var mainViewController = this.Orchestrator.MainViewController;
			var previewController  = mainViewController.PreviewViewController;

			mainViewController.SetPreviewPanelVisibility (false);
			
			previewController.Clear ();
			previewController.Updating -= this.HandlePreviewPanelUpdating;
		}

		private void HandlePreviewPanelUpdating(object sender)
		{
			this.previewerController.Update ();
		}

		private void CreateUIInvoice(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "InvoiceDocument",
					IconUri				= "Data.InvoiceDocument",
					Title				= TextFormatter.FormatText ("Document"),
					CompactTitle		= TextFormatter.FormatText ("Document"),
					TextAccessor		= this.CreateAccessor (x => x.GetSummary ()),
					CompactTextAccessor = this.CreateAccessor (x => x.GetCompactSummary ()),
					EntityMarshaler		= this.CreateEntityMarshaler (),
				});
		}

		private void CreateUIArticleLines(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "ArticleDocumentItem",
					IconUri		 = "Data.DocumentItems",
					Title		 = TextFormatter.FormatText ("Lignes"),
					CompactTitle = TextFormatter.FormatText ("Lignes"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<AbstractDocumentItemEntity> ("ArticleDocumentItem", data.Controller, this.DataContext);

			template.DefineText (x => TextFormatter.FormatText (SummaryInvoiceDocumentViewController.GetDocumentItemSummary (x)));
			template.DefineCompactText (x => TextFormatter.FormatText (SummaryInvoiceDocumentViewController.GetDocumentItemSummary (x)));
			template.DefineCreateItem (this.CreateArticleDocumentItem);  // le bouton [+] crée une ligne d'article
			template.DefineCreateGetIndex (this.CreateArticleGetIndex);
			template.Filter = SummaryInvoiceDocumentViewController.ArticleLineFilter;

			data.Add (this.CreateCollectionAccessor (template, x => x.Lines));
		}

		private void CreateUITotalSummary(UIBuilder builder)
		{
			builder.CreateEditionTitleTile ("Data.TotalDocumentItem", "Total");
			builder.CreateSummaryTile ("TotalDocumentItem", this.Entity, GetTotalSummary (this.Entity), ViewControllerMode.Edition, 1);
		}

		private void CreateUIBillings(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "BillingDetails",
					IconUri		 = "Data.BillingDetails",
					Title		 = TextFormatter.FormatText ("Facturation"),
					CompactTitle = TextFormatter.FormatText ("Facturation"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<BillingDetailEntity> ("BillingDetails", this.BusinessContext);

			template.DefineText (x => TextFormatter.FormatText (GetBillingDetailsSummary (this.Entity, x)));
			template.DefineCompactText (x => TextFormatter.FormatText (GetBillingDetailsSummary (this.Entity, x)));

			data.Add (this.CreateCollectionAccessor (template, x => x.BillingDetails));
		}

		private void CreateUIComments(SummaryDataItems data)
		{
			SummaryControllers.Common.CreateUIComments (this.Data, data, this.EntityGetter, x => x.Comments);
		}


		private ArticleDocumentItemEntity CreateArticleDocumentItem()
		{
			//	Crée un nouvelle ligne dans la facture du type le plus courant, c'est-à-dire ArticleDocumentItemEntity.
			var article = this.DataContext.CreateEntityAndRegisterAsEmpty<ArticleDocumentItemEntity> ();

			article.Visibility = true;
			article.BeginDate  = this.Entity.CreationDate;
			article.EndDate    = this.Entity.CreationDate;

			return article;
		}

		private int CreateArticleGetIndex()
		{
			//	Retourne l'index où insérer la nouvelle ligne d'article créée avec le bouton [+].
			int index = this.Entity.Lines.Count;  // insère à la fin par défaut

			while (index > 0)
			{
				var line = this.Entity.Lines[index-1];

				if (line is TotalDocumentItemEntity ||
					line is TaxDocumentItemEntity   )
				{
					index--;  // insère avant le total
					continue;
				}

				if (ArticleDocumentItemHelper.IsFixedTax (line as ArticleDocumentItemEntity))
				{
					index--;  // insère avant les frais de port
					continue;
				}

				break;
			}

			return index;
		}

		private static bool ArticleLineFilter(AbstractDocumentItemEntity x)
		{
			return (x is TextDocumentItemEntity    ||
				    x is ArticleDocumentItemEntity ||
				    x is PriceDocumentItemEntity   );
		}


		private static FormattedText GetDocumentItemSummary(AbstractDocumentItemEntity documentItemEntity)
		{
			if (documentItemEntity is TextDocumentItemEntity)
			{
				return SummaryInvoiceDocumentViewController.GetTextDocumentItemSummary (documentItemEntity as TextDocumentItemEntity);
			}

			if (documentItemEntity is ArticleDocumentItemEntity)
			{
				return SummaryInvoiceDocumentViewController.GetArticleDocumentItemSummary (documentItemEntity as ArticleDocumentItemEntity);
			}

			if (documentItemEntity is PriceDocumentItemEntity)
			{
				return SummaryInvoiceDocumentViewController.GetPriceDocumentItemSummary (documentItemEntity as PriceDocumentItemEntity);
			}

			if (documentItemEntity is TaxDocumentItemEntity)
			{
				return SummaryInvoiceDocumentViewController.GetTaxDocumentItemSummary (documentItemEntity as TaxDocumentItemEntity);
			}

			if (documentItemEntity is TotalDocumentItemEntity)
			{
				return SummaryInvoiceDocumentViewController.GetTotalDocumentItemSummary (documentItemEntity as TotalDocumentItemEntity);
			}

			return FormattedText.Null;
		}

		private static FormattedText GetTextDocumentItemSummary(TextDocumentItemEntity x)
		{
			if (x.Text.IsNullOrEmpty)
			{
				return "<i>Texte</i>";
			}
			else
			{
				return x.Text;
			}
		}

		private static FormattedText GetArticleDocumentItemSummary(ArticleDocumentItemEntity x)
		{
			var quantity = ArticleDocumentItemHelper.GetArticleQuantityAndUnit (x);
			var desc = Misc.FirstLine (ArticleDocumentItemHelper.GetArticleDescription (x, shortDescription: true));
			var price = Misc.PriceToString (x.PrimaryLinePriceBeforeTax);

			FormattedText text = TextFormatter.FormatText (quantity, desc, price); // FormattedText.Join (" ", quantity, desc, price);

			if (text.IsNullOrEmpty)
			{
				return "<i>Article</i>";
			}
			else
			{
				return text;
			}
		}

		private static FormattedText GetPriceDocumentItemSummary(PriceDocumentItemEntity x)
		{
			var builder = new System.Text.StringBuilder ();

			builder.Append ("Sous-total ");
			builder.Append (Misc.PriceToString (x.ResultingPriceBeforeTax));

			if (x.Discount.DiscountRate.HasValue)
			{
				builder.Append (" (après rabais en %)");
			}
			else if (x.Discount.DiscountAmount.HasValue)
			{
				builder.Append (" (après rabais en francs)");
			}
			else if (x.FixedPriceAfterTax.HasValue)
			{
				builder.Append (" (montant arrêté)");
			}

			return builder.ToString ();
		}

		private static FormattedText GetTaxDocumentItemSummary(TaxDocumentItemEntity x)
		{
			var desc = x.Text;
			var tax = Misc.PriceToString (x.ResultingTax);

			var text = TextFormatter.FormatText (desc, tax);

			if (text.IsNullOrEmpty)
			{
				return "<i>TVA</i>";
			}
			else
			{
				return text;
			}
		}

		private static FormattedText GetTotalDocumentItemSummary(TotalDocumentItemEntity x)
		{
			var desc = x.TextForPrimaryPrice;

			string total;
			if (x.PrimaryPriceBeforeTax.HasValue)
			{
				total = Misc.PriceToString (x.PrimaryPriceBeforeTax);
			}
			else if (x.FixedPriceAfterTax.HasValue)
			{
				total = Misc.PriceToString (x.FixedPriceAfterTax);
			}
			else
			{
				total = Misc.PriceToString (x.PrimaryPriceAfterTax);
			}

			var text = TextFormatter.FormatText (desc, total);

			if (text.IsNullOrEmpty)
			{
				return "<i>Total</i>";
			}
			else
			{
				return text;
			}
		}


		private static FormattedText GetBillingDetailsSummary(InvoiceDocumentEntity invoiceDocument, BillingDetailEntity billingDetails)
		{
			string amount = Misc.PriceToString (billingDetails.AmountDue.Amount);
			FormattedText title = Misc.FirstLine (billingDetails.Title);
			FormattedText ratio = InvoiceDocumentHelper.GetInstalmentName (invoiceDocument, billingDetails, true);

			if (ratio.IsNullOrWhiteSpace)
			{
				return TextFormatter.FormatText (amount, title);
			}
			else
			{
				return TextFormatter.FormatText (amount, ratio, title);
			}
		}

		private static FormattedText GetTotalSummary(InvoiceDocumentEntity invoiceDocument)
		{
			string ht  = Misc.PriceToString (InvoiceDocumentHelper.GetPrimaryPriceHT  (invoiceDocument));
			string vat = Misc.PriceToString (InvoiceDocumentHelper.GetVatTotal        (invoiceDocument));
			string ttc = Misc.PriceToString (InvoiceDocumentHelper.GetPrimaryPriceTTC (invoiceDocument));
			string fix = Misc.PriceToString (InvoiceDocumentHelper.GetFixedPriceTTC   (invoiceDocument));

			return TextFormatter.FormatText ("HT~", ht, "~,", "TVA~", vat, "\n", "TTC~", ttc, "arrêté à~", fix);
		}


		protected override EditionStatus GetEditionStatus()
		{
			var entity = this.Entity;
			return EditionStatus.Valid;
		}


		private Printers.PreviewerController		previewerController;
	}
}
