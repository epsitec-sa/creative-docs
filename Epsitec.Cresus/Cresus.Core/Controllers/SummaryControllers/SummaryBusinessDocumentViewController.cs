﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public class SummaryBusinessDocumentViewController : SummaryViewController<BusinessDocumentEntity>
	{
		public SummaryBusinessDocumentViewController(string name, BusinessDocumentEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI()
		{
			using (var builder = UIBuilder.Create (this))
			{
				using (var data = TileContainerController.Setup (builder))
				{
					this.CreateUIInvoice (data);
					this.CreateUIArticleLines (data);
					this.CreateUIFreightAndTaxLines (data);
					this.CreateUIVatLines (data);
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
			//	Crée le conteneur.
			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;

			var previewFrame = new FrameBox
			{
				Dock      = DockStyle.Fill,
				Padding   = new Margins (5),
				BackColor = adorner.ColorWindow,
			};

			var mainViewController = this.Orchestrator.MainViewController;
			var previewController  = mainViewController.PreviewViewController;

			mainViewController.SetPreviewPanelVisibility (true);

			//	Crée le contrôleur.
			DocumentMetadataEntity metadoc = this.GetMetadoc ();

			this.previewController = new Printers.ContinuousController (this.Data, metadoc, Printers.DocumentType.InvoiceWithInsideESR);
			this.previewController.CreateUI (previewFrame);

			previewController.Add (previewFrame);
			previewController.Updating += this.HandlePreviewPanelUpdating;
		}

		private DocumentMetadataEntity GetMetadoc()
		{
			var metadoc = this.DataContext.GetEntitiesOfType<DocumentMetadataEntity> ().FirstOrDefault ();

			if (metadoc == null)
			{
				DocumentMetadataEntity example = new DocumentMetadataEntity ();
				example.BusinessDocument = this.Entity;
				example.IsArchive = false;

				return this.DataContext.GetByExample<DocumentMetadataEntity> (example).FirstOrDefault ();
			}
			else
			{
				return metadoc;
			}
		}

		private void CloseUIPreviewPanel()
		{
			var mainViewController = this.Orchestrator.MainViewController;
			var previewController  = mainViewController.PreviewViewController;

			mainViewController.SetPreviewPanelVisibility (false);

			if (this.previewController != null)
			{
				this.previewController.CloseUI ();
				previewController.Clear ();
				previewController.Updating -= this.HandlePreviewPanelUpdating;
			}
		}

		private void HandlePreviewPanelUpdating(object sender)
		{
			previewController.Update ();
		}

		private void CreateUIInvoice(SummaryDataItems data)
		{
			data.Add (
				new SummaryDataItem
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
				new SummaryDataItem
				{
					AutoGroup    = true,
					Name		 = "ArticleLines",
					IconUri		 = "Data.DocumentItems",
					Title		 = TextFormatter.FormatText ("Lignes"),
					CompactTitle = TextFormatter.FormatText ("Lignes"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<AbstractDocumentItemEntity> ("ArticleLines", data.Controller, this.DataContext);

			template.DefineText        (x => x.GetCompactSummary ());
			template.DefineCompactText (x => x.GetCompactSummary ());
			template.DefineCreateItem (this.CreateArticleDocumentItem);  // le bouton [+] crée une ligne d'article
			template.DefineCreateGetIndex (this.CreateArticleGetIndex);
			template.Filter = SummaryBusinessDocumentViewController.ArticleLineFilter;

			data.Add (this.CreateCollectionAccessor (template, x => x.Lines));
		}

		private void CreateUIFreightAndTaxLines(SummaryDataItems data)
		{
			data.Add (
				new SummaryDataItem
				{
					AutoGroup    = true,
					Name		 = "FreightAndTaxLines",
					IconUri		 = "Data.DocumentItems",
					Title		 = TextFormatter.FormatText ("Port, emballage et taxes"),
					CompactTitle = TextFormatter.FormatText ("Port, emballage et taxes"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<AbstractDocumentItemEntity> ("FreightAndTaxLines", data.Controller, this.DataContext);

			template.DefineText (x => x.GetCompactSummary ());
			template.DefineCompactText (x => x.GetCompactSummary ());
			template.DefineCreateItem (this.CreateFreightAndTaxDocumentItem);  // le bouton [+] crée une ligne d'article
			template.DefineCreateGetIndex (this.CreateFreightAndTaxGetIndex);
			template.Filter = SummaryBusinessDocumentViewController.FreightAndTaxLineFilter;

			data.Add (this.CreateCollectionAccessor (template, x => x.Lines));
		}

		private void CreateUIVatLines(SummaryDataItems data)
		{
			data.Add (
				new SummaryDataItem
				{
					AutoGroup    = true,
					Name		 = "VatLines",
					IconUri		 = "Data.DocumentItems",
					Title		 = TextFormatter.FormatText ("Récapitulatif TVA"),
					CompactTitle = TextFormatter.FormatText ("Récapitulatif TVA"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<AbstractDocumentItemEntity> ("VatLines", data.Controller, this.DataContext);

			template.DefineText (x => x.GetCompactSummary ());
			template.DefineCompactText (x => x.GetCompactSummary ());
			template.DefineCreateItem (null);
			template.DefineDeleteItem (null);
			template.Filter = SummaryBusinessDocumentViewController.VatLineFilter;

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
				new SummaryDataItem
				{
					AutoGroup    = true,
					Name		 = "BillingDetails",
					IconUri		 = "Data.BillingDetails",
					Title		 = TextFormatter.FormatText ("Facturation"),
					CompactTitle = TextFormatter.FormatText ("Facturation"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<BillingDetailEntity> ("BillingDetails", this.BusinessContext);

			template.DefineText        (x => x.GetCompactSummary (this.Entity));
			template.DefineCompactText (x => x.GetCompactSummary (this.Entity));

			data.Add (this.CreateCollectionAccessor (template, x => x.BillingDetails));
		}

		private void CreateUIComments(SummaryDataItems data)
		{
			//SummaryControllers.Common.CreateUIComments (this.BusinessContext, data, this.EntityGetter, x => x.Comments);
		}


		private ArticleDocumentItemEntity CreateFreightAndTaxDocumentItem()
		{
			var article = this.DataContext.CreateEntityAndRegisterAsEmpty<ArticleDocumentItemEntity> ();

			article.Visibility = true;
			article.BeginDate  = this.Entity.BillingDate;
			article.EndDate    = this.Entity.BillingDate;
			article.GroupLevel = 0;

			return article;
		}

		private ArticleDocumentItemEntity CreateArticleDocumentItem()
		{
			//	Crée un nouvelle ligne dans la facture du type le plus courant, c'est-à-dire ArticleDocumentItemEntity.
			var article = this.DataContext.CreateEntityAndRegisterAsEmpty<ArticleDocumentItemEntity> ();

			article.Visibility = true;
			article.BeginDate  = this.Entity.BillingDate;
			article.EndDate    = this.Entity.BillingDate;
			article.GroupLevel = 1;

			return article;
		}

		private int CreateArticleGetIndex()
		{
			//	Retourne l'index où insérer la nouvelle ligne d'article créée avec le bouton [+].
			int index = this.Entity.Lines.Count;  // insère à la fin par défaut

			while (--index >= 0)
			{
				var line = this.Entity.Lines[index];

				if ((line.GroupLevel > 0) &&
					(! (line is SubTotalDocumentItemEntity)))
				{
					return index+1;
				}
			}

			return 0;
		}

		private int CreateFreightAndTaxGetIndex()
		{
			int index = this.CreateArticleGetIndex ();
			int count = this.Entity.Lines.Count;

			while (index < count)
			{
				var line = this.Entity.Lines[index];

				if ((line is TaxDocumentItemEntity) ||
					(line is SubTotalDocumentItemEntity) ||
					(line is EndTotalDocumentItemEntity))
				{
					break;
				}

				index++;
			}

			return index;
		}

		private static bool FreightAndTaxLineFilter(AbstractDocumentItemEntity x)
		{
			if (x.GroupLevel != 0)
			{
				return false;
			}

			return x is TextDocumentItemEntity
				|| x is ArticleDocumentItemEntity;
		}

		private static bool VatLineFilter(AbstractDocumentItemEntity x)
		{
			return x is TaxDocumentItemEntity;
		}

		private static bool ArticleLineFilter(AbstractDocumentItemEntity x)
		{
			if (x.GroupLevel < 1)
			{
				return false;
			}

			return x is TextDocumentItemEntity
				|| x is ArticleDocumentItemEntity
				|| x is SubTotalDocumentItemEntity;
		}


		private static FormattedText GetTotalSummary(BusinessDocumentEntity invoiceDocument)
		{
			string ht  = Misc.PriceToString (InvoiceDocumentHelper.GetPrimaryPriceHT  (invoiceDocument));
			string vat = Misc.PriceToString (InvoiceDocumentHelper.GetVatTotal        (invoiceDocument));
			string ttc = Misc.PriceToString (InvoiceDocumentHelper.GetPrimaryPriceTTC (invoiceDocument));
			string fix = Misc.PriceToString (InvoiceDocumentHelper.GetFixedPriceTTC   (invoiceDocument));

			return TextFormatter.FormatText ("HT~", ht, "~,", "TVA~", vat, "\n", "TTC~", ttc, "arrêté à~", fix);
		}


		private Printers.ContinuousController		 previewController;
	}
}
