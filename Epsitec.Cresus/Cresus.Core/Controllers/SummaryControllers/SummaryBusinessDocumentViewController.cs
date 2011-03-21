//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Print.Controllers;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryBusinessDocumentViewController : SummaryViewController<BusinessDocumentEntity>
	{
		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIInvoice (data);
				this.CreateUIArticleLines (data);
				this.CreateUIFreightAndTaxLines (data);
				this.CreateUITotalSummary (data);
				this.CreateUIVatLines (data);
				this.CreateUIBillings (data);
				this.CreateUIComments (data);
			}

			this.CreateUIPreviewPanel ();
		}


		private void CreateUIInvoice(TileDataItems data)
		{
			var tileDataItem = new TileDataItem
			{
				Name				= "InvoiceDocument",
				IconUri				= "Data.InvoiceDocument",
				Title				= TextFormatter.FormatText ("Document"),
				CompactTitle		= TextFormatter.FormatText ("Document"),
				TextAccessor		= this.CreateAccessor (x => x.GetSummary ()),
				CompactTextAccessor = this.CreateAccessor (x => x.GetCompactSummary ()),
				EntityMarshaler		= this.CreateEntityMarshaler (),
			};

			data.Add (tileDataItem);
		}

		private void CreateUIArticleLines(TileDataItems data)
		{
			var tileDataItem = new TileDataItem
			{
				AutoGroup    = true,
				Name		 = "ArticleLines",
				IconUri		 = "Data.ArticleDocumentItem",
				Title		 = TextFormatter.FormatText ("Lignes"),
				CompactTitle = TextFormatter.FormatText ("Lignes"),
				Text		 = CollectionTemplate.DefaultEmptyText,
			};

			data.Add (tileDataItem);

			var template = new CollectionTemplate<AbstractDocumentItemEntity> ("ArticleLines", this.BusinessContext);

			template.DefineText        (x => x.GetCompactSummary ());
			template.DefineCompactText (x => x.GetCompactSummary ());
			template.DefineCreateItem (this.CreateArticleDocumentItem);  // le bouton [+] crée une ligne d'article
			template.DefineCreateGetIndex (this.CreateArticleGetIndex);
			template.Filter = SummaryBusinessDocumentViewController.ArticleLineFilter;

			data.Add (this.CreateCollectionAccessor (template, x => x.Lines));
		}

		private void CreateUIFreightAndTaxLines(TileDataItems data)
		{
			var tileDataItem = new TileDataItem
			{
				AutoGroup    = true,
				Name		 = "FreightAndTaxLines",
				IconUri		 = "Data.DocumentItems",
				Title		 = TextFormatter.FormatText ("Port, emballage et taxes"),
				CompactTitle = TextFormatter.FormatText ("Port, emballage et taxes"),
				Text		 = CollectionTemplate.DefaultEmptyText,
			};

			data.Add (tileDataItem);

			var template = new CollectionTemplate<AbstractDocumentItemEntity> ("FreightAndTaxLines", this.BusinessContext);

			template.DefineText        (x => x.GetCompactSummary ());
			template.DefineCompactText (x => x.GetCompactSummary ());
			template.DefineCreateItem (this.CreateFreightAndTaxDocumentItem);  // le bouton [+] crée une ligne d'article
			template.DefineCreateGetIndex (this.CreateFreightAndTaxGetIndex);
			template.Filter = SummaryBusinessDocumentViewController.FreightAndTaxLineFilter;

			data.Add (this.CreateCollectionAccessor (template, x => x.Lines));
		}

		private void CreateUITotalSummary(TileDataItems data)
		{
			var tileData = new TileDataItem
			{
				Name		       = "TotalDocumentItem",
				IconUri		       = "Data.TotalDocumentItem",
				Title		       = TextFormatter.FormatText ("Total"),
				CompactTitle       = TextFormatter.FormatText ("Total"),
				CreateCustomizedUI = (tile, builder) =>
				{
					builder.CreateStaticText (tile, GetTotalSummary (this.Entity).ToString ());
				}
			};

			data.Add (tileData);
		}

		private void CreateUIVatLines(TileDataItems data)
		{
			var tileDataItem = new TileDataItem
			{
				AutoGroup        = true,
				HideAddButton    = true,
				HideRemoveButton = true,
				Name		     = "VatLines",
				IconUri		     = "Data.TaxDocumentItem",
				Title		     = TextFormatter.FormatText ("Récapitulatif TVA"),
				CompactTitle     = TextFormatter.FormatText ("Récapitulatif TVA"),
				Text		     = CollectionTemplate.DefaultEmptyText,
			};

			data.Add (tileDataItem);

			var template = new CollectionTemplate<AbstractDocumentItemEntity> ("VatLines", this.BusinessContext);

			template.DefineText        (x => x.GetCompactSummary ());
			template.DefineCompactText (x => x.GetCompactSummary ());
			template.DefineCreateItem (null);
			template.DefineDeleteItem (null);
			template.Filter = SummaryBusinessDocumentViewController.VatLineFilter;

			data.Add (this.CreateCollectionAccessor (template, x => x.Lines));
		}

		private void CreateUIBillings(TileDataItems data)
		{
			var tileDataItem = new TileDataItem
			{
				AutoGroup    = true,
				Name		 = "BillingDetails",
				IconUri		 = "Data.BillingDetails",
				Title		 = TextFormatter.FormatText ("Facturation"),
				CompactTitle = TextFormatter.FormatText ("Facturation"),
				Text		 = CollectionTemplate.DefaultEmptyText,
			};

			data.Add (tileDataItem);

			var template = new CollectionTemplate<BillingDetailEntity> ("BillingDetails", this.BusinessContext);

			template.DefineText        (x => x.GetCompactSummary (this.Entity));
			template.DefineCompactText (x => x.GetCompactSummary (this.Entity));

			data.Add (this.CreateCollectionAccessor (template, x => x.BillingDetails));
		}

		private void CreateUIComments(TileDataItems data)
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


		#region Preview panel
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
			DocumentMetadataEntity metadoc = this.GetDocumentMetadata ();

			//?Business.DocumentType type = Business.DocumentType.Unknown;
			Business.DocumentType type = Business.DocumentType.Invoice;  // TODO: remplacer cette ligne par la précédente !

			if (metadoc.DocumentCategory.IsNotNull ())
			{
				type = metadoc.DocumentCategory.DocumentType;
			}

			this.previewController = new ContinuousController (this.BusinessContext, metadoc, type);
			this.previewController.CreateUI (previewFrame);

			previewController.Add (previewFrame);
			previewController.Updating += this.HandlePreviewPanelUpdating;
		}

		private DocumentMetadataEntity GetDocumentMetadata()
		{
			var meta = this.BusinessContext.GetMasterEntity<DocumentMetadataEntity> ();

			if (meta.IsNull ())
			{
				var repository = this.BusinessContext.GetRepository<DocumentMetadataEntity> ();
				var example    = new DocumentMetadataEntity ()
				{
					IsArchive = false,
					BusinessDocument = this.Entity
				};

				return repository.GetByExample (example).FirstOrDefault ();
			}
			else
			{
				return meta;
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
		#endregion


		private ContinuousController		 previewController;
	}
}
