//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{

	public class InvoiceDocumentEntityPrinter : AbstractEntityPrinter<InvoiceDocumentEntity>
	{
		public InvoiceDocumentEntityPrinter(InvoiceDocumentEntity entity)
			: base (entity)
		{
			DocumentTypeDefinition type;

			type = new DocumentTypeDefinition (DocumentType.Offer, "Offre", "Offre pour le client.");
			type.AddDocumentOptionInvoice ();
			type.AddDocumentOptionOrientation ();
			type.AddDocumentOptionMargin ();
			type.AddDocumentOptionSpecimen ();
			type.AddPrinterBase ();
			this.DocumentTypes.Add (type);

			type = new DocumentTypeDefinition (DocumentType.Order, "Commande", "Commande pour le client.");
			type.AddDocumentOptionInvoice ();
			type.AddDocumentOptionOrientation ();
			type.AddDocumentOptionMargin ();
			type.AddDocumentOptionCommande ();
			type.AddDocumentOptionSpecimen ();
			type.AddPrinterBase ();
			this.DocumentTypes.Add (type);

			type = new DocumentTypeDefinition (DocumentType.OrderAcknowledge, "Confirmation de commande", "Confirmation de commande pour le client.");
			type.AddDocumentOptionInvoice ();
			type.AddDocumentOptionOrientation ();
			type.AddDocumentOptionMargin ();
			type.AddDocumentOptionSpecimen ();
			type.AddPrinterBase ();
			this.DocumentTypes.Add (type);

			type = new DocumentTypeDefinition (DocumentType.ProductionOrder, "Ordres de production", "Ordres de production, pour chaque atelier.");
			type.AddDocumentOptionInvoice ();
			type.AddDocumentOptionOrientation ();
			type.AddDocumentOptionMargin ();
			type.AddDocumentOptionProd ();
			type.AddDocumentOptionSpecimen ();
			type.AddPrinterBase ();
			this.DocumentTypes.Add (type);

			type = new DocumentTypeDefinition (DocumentType.BL, "Bulletin de livraison", "Bulletin de livraison, sans prix.");
			type.AddDocumentOptionInvoice ();
			type.AddDocumentOptionOrientation ();
			type.AddDocumentOptionMargin ();
			type.AddDocumentOptionBL ();
			type.AddDocumentOptionSpecimen ();
			type.AddPrinterBase ();
			this.DocumentTypes.Add (type);

			type = new DocumentTypeDefinition (DocumentType.InvoiceWithInsideESR, "Facture avec BV intégré", "Facture avec un bulletin de versement intégré au bas de chaque page.");
			type.AddDocumentOptionInvoice ();
			type.AddDocumentOptionEsr ();
			type.AddPrinterBase ();
			this.DocumentTypes.Add (type);

			type = new DocumentTypeDefinition (DocumentType.InvoiceWithOutsideESR, "Facture avec BV séparé", "Facture avec un bulletin de versement imprimé sur une page séparée.");
			type.AddDocumentOptionInvoice ();
			type.AddDocumentOptionEsr ();
			type.AddPrinterBase ();
			type.AddPrinterEsr ();
			this.DocumentTypes.Add (type);

			type = new DocumentTypeDefinition (DocumentType.InvoiceWithoutESR, "Facture sans BV", "Facture simple sans bulletin de versement.");
			type.AddDocumentOptionInvoice ();
			type.AddDocumentOptionOrientation ();
			type.AddDocumentOptionMargin ();
			type.AddDocumentOptionSpecimen ();
			type.AddPrinterBase ();
			this.DocumentTypes.Add (type);
		}

		public override string JobName
		{
			get
			{
				return FormattedText.Concat ("Facture ", this.entity.IdA).ToSimpleText ();
			}
		}

		public override Size PageSize
		{
			get
			{
				if (this.EntityPrintingSettings.HasDocumentOption ("Orientation.Horizontal"))
				{
					return new Size (297, 210);  // A4 horizontal
				}
				else
				{
					return new Size (210, 297);  // A4 vertical
				}
			}
		}

		public override Margins PageMargins
		{
			get
			{
				double h = this.IsDocumentWithoutPrice ? 0 : InvoiceDocumentEntityPrinter.reportHeight;

				if (this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.InvoiceWithInsideESR)
				{
					return new Margins (20, 10, 20+h*2, h+10+AbstractEsrBand.DefautlSize.Height);
				}
				else
				{
					return new Margins (20, 10, 20+h*2, h+20);
				}
			}
		}

		public override void BuildSections()
		{
			base.BuildSections ();
			this.documentContainer.Clear ();

			if (this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.Offer)
			{
				int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

				this.BuildHeader (null);
				this.BuildArticles ();
				this.BuildPages (null, firstPage);
			}

			if (this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.Order)
			{
				int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

				this.BuildHeader (null);
				this.BuildArticles ();
				this.BuildFooter ();
				this.BuildPages (null, firstPage);
			}

			if (this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.OrderAcknowledge)
			{
				int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

				this.BuildHeader (null);
				this.BuildArticles ();
				this.BuildPages (null, firstPage);
			}

			if (this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.ProductionOrder)
			{
				var groups = this.GetProdGroups ();
				foreach (var group in groups)
				{
					int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

					this.BuildHeader (null, group);
					this.BuildArticles (group);
					this.BuildFooter ();
					this.BuildPages (null, firstPage);
				}
			}

			if (this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.BL)
			{
				int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

				this.BuildHeader (null);
				this.BuildArticles ();
				this.BuildFooter ();
				this.BuildPages (null, firstPage);
			}

			if (this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.InvoiceWithInsideESR ||
				this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.InvoiceWithOutsideESR)
			{
				foreach (var billingDetails in this.entity.BillingDetails)
				{
					int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

					this.BuildHeader (billingDetails);
					this.BuildArticles ();
					this.BuildConditions (billingDetails);
					this.BuildPages (billingDetails, firstPage);
					this.BuildReportHeaders (firstPage);
					this.BuildReportFooters (firstPage);
					this.BuildEsrs (billingDetails, firstPage);
				}
			}

			if (this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.InvoiceWithoutESR)
			{
				if (this.entity.BillingDetails.Count != 0)
				{
					var billingDetails = this.entity.BillingDetails[0];
					int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

					this.BuildHeader (billingDetails);
					this.BuildArticles ();
					this.BuildConditions (billingDetails);
					this.BuildPages (billingDetails, firstPage);
					this.BuildReportHeaders (firstPage);
					this.BuildReportFooters (firstPage);
				}
			}
		}

		public override void PrintCurrentPage(IPaintPort port)
		{
			base.PrintCurrentPage (port);

			this.documentContainer.Paint (port, this.CurrentPage, this.IsPreview);
		}


		private List<ArticleGroupEntity> GetProdGroups()
		{
			//	Retourne la liste des groupes des articles du document.
			var groups = new List<ArticleGroupEntity> ();

			foreach (var line in this.entity.Lines)
			{
				if (line is ArticleDocumentItemEntity)
				{
					var article = line as ArticleDocumentItemEntity;

					foreach (var group in article.ArticleDefinition.ArticleGroups)
					{
						if (!groups.Contains (group))
						{
							groups.Add (group);
						}
					}
				}
			}

			return groups;
		}


		private void BuildHeader(BillingDetailEntity billingDetails, ArticleGroupEntity group=null)
		{
			//	Ajoute l'en-tête de la facture dans le document.
			var imageBand = new ImageBand ();
			imageBand.Load ("logo-cresus.png");
			imageBand.BuildSections (60, 50, 50, 50);
			this.documentContainer.AddAbsolute (imageBand, new Rectangle (20, this.PageSize.Height-10-50, 60, 50));

			var textBand = new TextBand ();
			textBand.Text = FormattedText.Concat ("<b>", "Les logiciels de gestion", "</b>");
			textBand.Font = font;
			textBand.FontSize = 5.0;
			this.documentContainer.AddAbsolute (textBand, new Rectangle (20, this.PageSize.Height-10-imageBand.GetSectionHeight (0)-10, 80, 10));

			var mailContactBand = new TextBand ();
			mailContactBand.Text = InvoiceDocumentHelper.GetMailContact (this.entity);
			mailContactBand.Font = font;
			mailContactBand.FontSize = fontSize;
			this.documentContainer.AddAbsolute (mailContactBand, new Rectangle (120, this.PageSize.Height-57, 80, 25));

			//	Génère le groupe "concerne".
			{
				FormattedText title, text;
				CellBorder    cellBorder;
				Margins       margins;
				Color         color;

				if (group == null)
				{
					title      = "Concerne";
					text       = this.entity.DocumentTitle;
					cellBorder = CellBorder.Empty;
					margins    = new Margins (0);
					color      = Color.Empty;
				}
				else
				{
					title      = "Atelier";
					text       = FormattedText.Concat ("<b>", group.Name.IsNullOrWhiteSpace ? group.Code : group.Name, "</b>");
					cellBorder = CellBorder.Default;
					margins    = new Margins (1);
					color      = Color.FromBrightness (0.9);
				}

				if (!text.IsNullOrEmpty)
				{
					var band = new TableBand ();
					band.ColumnsCount = 2;
					band.RowsCount = 1;
					band.CellBorder = cellBorder;
					band.Font = font;
					band.FontSize = fontSize;
					band.CellMargins = margins;
					band.SetRelativeColumWidth (0, 15);
					band.SetRelativeColumWidth (1, 80);
					band.SetText (0, 0, title);
					band.SetText (1, 0, text);
					band.SetBackground (1, 0, color);
					this.documentContainer.AddAbsolute (band, new Rectangle (20, this.PageSize.Height-67, 100-5, 15));
				}
			}

			var titleBand = new TextBand ();
			titleBand.Text = InvoiceDocumentHelper.GetTitle (this.entity, billingDetails, this.EntityPrintingSettings.DocumentTypeEnumSelected);
			titleBand.Font = font;
			titleBand.FontSize = 5.0;
			this.documentContainer.AddAbsolute (titleBand, new Rectangle (20, this.PageSize.Height-82, 90, 10));

			string date = Misc.GetDateTimeDescription (this.entity.LastModificationDate);
			var dateBand = new TextBand ();
			dateBand.Text = FormattedText.Concat ("Crissier, le ", date);
			dateBand.Font = font;
			dateBand.FontSize = fontSize;
			this.documentContainer.AddAbsolute (dateBand, new Rectangle (120, this.PageSize.Height-82, 80, 10));
		}

		private void BuildArticles(ArticleGroupEntity group=null)
		{
			//	Ajoute les articles dans le document.
			this.documentContainer.CurrentVerticalPosition = this.PageSize.Height-87;

			this.tableColumns.Clear ();

			double priceWidth = 13 + this.CellMargin*2;  // largeur standard pour un montant ou une quantité

			if (this.IsColumnsOrderQD)
			{
				this.tableColumns.Add (TableColumnKeys.Quantity,           new TableColumn ("?",           priceWidth,   ContentAlignment.MiddleLeft));  // "Quantité" ou "Livré"
				this.tableColumns.Add (TableColumnKeys.DelayedQuantity,    new TableColumn ("Suit",        priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.DelayedDate,        new TableColumn ("Date",        priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleId,          new TableColumn ("Article",     priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleDescription, new TableColumn ("Désignation", 0,            ContentAlignment.MiddleLeft));  // seule colonne en mode width = fill
				this.tableColumns.Add (TableColumnKeys.UnitPrice,          new TableColumn ("p.u. HT",     priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Discount,           new TableColumn ("Rabais",      priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.LinePrice,          new TableColumn ("Prix HT",     priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Vat,                new TableColumn ("TVA",         priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Total,              new TableColumn ("Prix TTC",    priceWidth,   ContentAlignment.MiddleRight));
			}
			else
			{
				this.tableColumns.Add (TableColumnKeys.ArticleId,          new TableColumn ("Article",     priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleDescription, new TableColumn ("Désignation", 0,            ContentAlignment.MiddleLeft));  // seule colonne en mode width = fill
				this.tableColumns.Add (TableColumnKeys.Quantity,           new TableColumn ("?",           priceWidth,   ContentAlignment.MiddleLeft));  // "Quantité" ou "Livré"
				this.tableColumns.Add (TableColumnKeys.DelayedQuantity,    new TableColumn ("Suit",        priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.DelayedDate,        new TableColumn ("Date",        priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.UnitPrice,          new TableColumn ("p.u. HT",     priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Discount,           new TableColumn ("Rabais",      priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.LinePrice,          new TableColumn ("Prix HT",     priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Vat,                new TableColumn ("TVA",         priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Total,              new TableColumn ("Prix TTC",    priceWidth,   ContentAlignment.MiddleRight));
			}

			//	Première passe pour déterminer le nombre le lignes du tableau de la facture
			//	ainsi que les colonnes visibles.
			int rowCount = 1;  // déjà 1 pour l'en-tête (titres des colonnes)

			foreach (var line in this.entity.Lines)
			{
				if (line.Visibility)
				{
					int rowUsed = 0;

					if (line is TextDocumentItemEntity)
					{
						rowUsed = this.InitializeColumnTextLine (line as TextDocumentItemEntity);
					}

					if (line is ArticleDocumentItemEntity)
					{
						rowUsed = this.InitializeColumnArticleLine (line as ArticleDocumentItemEntity, group);
					}

					if (line is PriceDocumentItemEntity)
					{
						rowUsed = this.InitializeColumnPriceLine (line as PriceDocumentItemEntity);
					}

					if (line is TaxDocumentItemEntity)
					{
						rowUsed = this.InitializeColumnTaxLine (line as TaxDocumentItemEntity);
					}

					if (line is TotalDocumentItemEntity)
					{
						rowUsed = this.InitializeColumnTotalLine (line as TotalDocumentItemEntity);
					}

					rowCount += rowUsed;
				}
			}

			if (this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.BL)
			{
				this.tableColumns[TableColumnKeys.Discount ].Visible = false;
				this.tableColumns[TableColumnKeys.UnitPrice].Visible = false;
				this.tableColumns[TableColumnKeys.LinePrice].Visible = false;
				this.tableColumns[TableColumnKeys.Vat      ].Visible = false;
				this.tableColumns[TableColumnKeys.Total    ].Visible = false;
			}

			if (this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.ProductionOrder)
			{
				this.tableColumns[TableColumnKeys.DelayedQuantity].Visible = false;
				this.tableColumns[TableColumnKeys.DelayedDate    ].Visible = false;
				this.tableColumns[TableColumnKeys.Discount       ].Visible = false;
				this.tableColumns[TableColumnKeys.UnitPrice      ].Visible = false;
				this.tableColumns[TableColumnKeys.LinePrice      ].Visible = false;
				this.tableColumns[TableColumnKeys.Vat            ].Visible = false;
				this.tableColumns[TableColumnKeys.Total          ].Visible = false;
			}

			if (!this.EntityPrintingSettings.HasDocumentOption ("Delayed"))  // n'imprime pas les articles retardés ?
			{
				this.tableColumns[TableColumnKeys.DelayedQuantity].Visible = false;
				this.tableColumns[TableColumnKeys.DelayedDate    ].Visible = false;
			}

			if (!this.EntityPrintingSettings.HasDocumentOption ("ArticleId"))  // n'imprime pas les numéros d'article ?
			{
				this.tableColumns[TableColumnKeys.ArticleId].Visible = false;
			}

			//	Compte et numérote les colonnes visibles.
			this.visibleColumnCount = 0;

			foreach (var column in this.tableColumns.Values)
			{
				if (column.Visible)
				{
					column.Rank = this.visibleColumnCount++;
				}
			}

			//	Deuxième passe pour générer les colonnes et les lignes du tableau.
			this.table = new TableBand ();
			this.table.ColumnsCount = this.visibleColumnCount;
			this.table.RowsCount = rowCount;
			this.table.CellBorder = this.GetCellBorder ();
			this.table.CellMargins = new Margins (this.CellMargin);

			//	Détermine le nom de la colonne TableColumnKeys.Quantity.
			if (this.tableColumns[TableColumnKeys.DelayedQuantity].Visible)  // colonne quantité visible ?
			{
				this.tableColumns[TableColumnKeys.Quantity].Title = "Livré";  // affiche "Livré", "Suit", "Date"
			}
			else
			{
				this.tableColumns[TableColumnKeys.Quantity].Title = "Quantité";  // affiche "Quantité"
			}

			//	Génère une première ligne d'en-tête (titres des colonnes).
			int row = 0;

			foreach (var column in this.tableColumns.Values)
			{
				if (column.Visible)
				{
					this.table.SetText (column.Rank, row, column.Title);
				}
			}

			this.InitializeRowAlignment (this.table, row);
			this.table.SetCellBorder (row, this.GetCellBorder (bottomBold: true));

			row++;

			//	Génère toutes les lignes pour les articles.
			int linePage = this.documentContainer.CurrentPage;
			double lineY = this.documentContainer.CurrentVerticalPosition;

			for (int i = 0; i < this.entity.Lines.Count; i++)
			{
				var line = this.entity.Lines[i];

				if (line.Visibility)
				{
					int rowUsed = 0;

					if (line is TextDocumentItemEntity)
					{
						rowUsed = this.BuildTextLine (this.table, row, line as TextDocumentItemEntity);
					}

					if (line is ArticleDocumentItemEntity)
					{
						rowUsed = this.BuildArticleLine (this.table, row, line as ArticleDocumentItemEntity, group);
					}

					if (line is PriceDocumentItemEntity)
					{
						rowUsed = this.BuildPriceLine (this.table, row, line as PriceDocumentItemEntity);
					}

					if (line is TaxDocumentItemEntity)
					{
						bool firstTax = (i > 0                         && !(this.entity.Lines[i-1] is TaxDocumentItemEntity));
						bool lastTax  = (i < this.entity.Lines.Count-1 && !(this.entity.Lines[i+1] is TaxDocumentItemEntity));

						rowUsed = this.BuildTaxLine (this.table, row, line as TaxDocumentItemEntity, firstTax, lastTax);
					}

					if (line is TotalDocumentItemEntity)
					{
						rowUsed = this.BuildTotalLine (this.table, row, line as TotalDocumentItemEntity);
					}

					if (rowUsed != 0)
					{
						for (int j=row; j<row+rowUsed; j++)
						{
							this.InitializeRowAlignment (this.table, j);
						}

						row += rowUsed;
					}
				}
			}

			//	Détermine les largeurs des colonnes.
			double fixedWidth = 0;
			double[] columnWidths = new double[this.visibleColumnCount];
			foreach (var column in this.tableColumns.Values)
			{
				if (column.Visible)
				{
					if (column.Width != 0)  // pas la seule colonne en mode width = fill ?
					{
						double columnWidth = this.table.RequiredColumnWidth (column.Rank) + this.CellMargin*2;

						columnWidths[column.Rank] = columnWidth;
						fixedWidth += columnWidth;
					}
				}
			}

			if (fixedWidth < this.documentContainer.CurrentWidth - 50)  // reste au moins 5cm pour la colonne 'fill' ?
			{
				//	Initialise les largeurs en fonction des contenus réels des colonnes.
				foreach (var column in this.tableColumns.Values)
				{
					if (column.Visible)
					{
						if (column.Width != 0)  // pas la seule colonne en mode width = fill ?
						{
							column.Width = columnWidths[column.Rank];
						}
					}
				}
			}
			else
			{
				//	Initialise les largeurs d'après les estimations initiales.
				fixedWidth = 0;
				foreach (var column in this.tableColumns.Values)
				{
					if (column.Visible)
					{
						if (column.Width != 0)  // pas la seule colonne en mode width = fill ?
						{
							fixedWidth += column.Width;
						}
					}
				}
			}

			foreach (var column in this.tableColumns.Values)
			{
				if (column.Visible)
				{
					double columnWidth = column.Width;

					if (columnWidth == 0)  // seule colonne en mode width = fill ?
					{
						columnWidth = this.documentContainer.CurrentWidth - fixedWidth;  // utilise la largeur restante
					}

					this.table.SetRelativeColumWidth (column.Rank, columnWidth);
				}
			}

			//	Met la grande table dans le document.
			this.tableBounds = this.documentContainer.AddFromTop (this.table, 5.0);

			this.lastRowForEachSection = this.table.GetLastRowForEachSection ();
		}


		private int InitializeColumnTextLine(TextDocumentItemEntity line)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			this.tableColumns[TableColumnKeys.ArticleDescription].Visible = true;
			return 1;
		}

		private int InitializeColumnArticleLine(ArticleDocumentItemEntity line, ArticleGroupEntity group)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			if (this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.BL && !ArticleDocumentItemHelper.IsArticleForBL (line))
			{
				return 0;
			}

			if (this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.ProductionOrder && !ArticleDocumentItemHelper.IsArticleForProd (line, group))
			{
				return 0;
			}

			if (!this.IsPrintableArticle (line))
			{
				return 0;
			}

			this.tableColumns[TableColumnKeys.ArticleId         ].Visible = true;
			this.tableColumns[TableColumnKeys.ArticleDescription].Visible = true;
			this.tableColumns[TableColumnKeys.Total             ].Visible = true;

			if (line.VatCode != BusinessLogic.Finance.VatCode.None &&
				line.VatCode != BusinessLogic.Finance.VatCode.Excluded &&
				line.VatCode != BusinessLogic.Finance.VatCode.ZeroRated)
			{
				this.tableColumns[TableColumnKeys.Vat].Visible = true;
			}

			foreach (var quantity in line.ArticleQuantities)
			{
				if (quantity.QuantityType == BusinessLogic.ArticleQuantityType.Billed)
				{
					this.tableColumns[TableColumnKeys.Quantity ].Visible = true;
					this.tableColumns[TableColumnKeys.UnitPrice].Visible = true;
					this.tableColumns[TableColumnKeys.LinePrice].Visible = true;
				}

				if (quantity.QuantityType == BusinessLogic.ArticleQuantityType.Delayed)
				{
					this.tableColumns[TableColumnKeys.DelayedQuantity].Visible = true;
					this.tableColumns[TableColumnKeys.DelayedDate    ].Visible = true;
				}
			}

			if (line.Discounts.Count != 0)
			{
				this.tableColumns[TableColumnKeys.Discount].Visible = true;
			}

			return 1;
		}

		private int InitializeColumnPriceLine(PriceDocumentItemEntity line)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			if (this.IsDocumentWithoutPrice)
			{
				return 0;
			}

			this.tableColumns[TableColumnKeys.ArticleDescription].Visible = true;
			this.tableColumns[TableColumnKeys.LinePrice         ].Visible = true;
			this.tableColumns[TableColumnKeys.Vat               ].Visible = true;
			this.tableColumns[TableColumnKeys.Total             ].Visible = true;

			return InvoiceDocumentHelper.HasAmount (line) ? 2 : 1;  // 2 fausses lignes s'il y a un rabais
		}

		private int InitializeColumnTaxLine(TaxDocumentItemEntity line)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			if (this.IsDocumentWithoutPrice)
			{
				return 0;
			}

			this.tableColumns[TableColumnKeys.ArticleDescription].Visible = true;
			this.tableColumns[TableColumnKeys.LinePrice         ].Visible = true;
			this.tableColumns[TableColumnKeys.Vat               ].Visible = true;
			this.tableColumns[TableColumnKeys.Total             ].Visible = true;

			return 1;
		}

		private int InitializeColumnTotalLine(TotalDocumentItemEntity line)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			if (this.IsDocumentWithoutPrice)
			{
				return 0;
			}

			this.tableColumns[TableColumnKeys.ArticleDescription].Visible = true;

			if (line.PrimaryPriceBeforeTax.HasValue)  // ligne de total HT ?
			{
				this.tableColumns[TableColumnKeys.LinePrice].Visible = true;
			}
			else  // ligne de total TTC ?
			{
				this.tableColumns[TableColumnKeys.Total].Visible = true;
			}

			return 1;
		}


		private int BuildTextLine(TableBand table, int row, TextDocumentItemEntity line)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			var text = FormattedText.Concat ("<b>", line.Text, "</b>");
			table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, text);

			return 1;
		}

		private int BuildArticleLine(TableBand table, int row, ArticleDocumentItemEntity line, ArticleGroupEntity group)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			if (this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.BL && !ArticleDocumentItemHelper.IsArticleForBL (line))
			{
				return 0;
			}

			if (this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.ProductionOrder && !ArticleDocumentItemHelper.IsArticleForProd (line, group))
			{
				return 0;
			}

			if (!this.IsPrintableArticle (line))
			{
				return 0;
			}

			FormattedText q1   = FormattedText.Null;
			FormattedText q2   = FormattedText.Null;
			FormattedText date = FormattedText.Null;

			foreach (var quantity in line.ArticleQuantities)
			{
				if (quantity.QuantityType == BusinessLogic.ArticleQuantityType.Billed)
				{
					q1 = Misc.FormatUnit (quantity.Quantity, quantity.Unit.Code);
				}

				if (quantity.QuantityType == BusinessLogic.ArticleQuantityType.Delayed)
				{
					q2 = Misc.AppendLine (q2, Misc.FormatUnit (quantity.Quantity, quantity.Unit.Code));

					if (quantity.ExpectedDate.HasValue)
					{
						date = Misc.AppendLine(date, quantity.ExpectedDate.Value.ToString ());
					}
				}
			}

			FormattedText description = ArticleDocumentItemHelper.GetArticleDescription (line, replaceTags: true, shortDescription: this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.ProductionOrder);

			if (q1 != null)
			{
				table.SetText (this.tableColumns[TableColumnKeys.Quantity].Rank, row, q1);
			}

			if (q2 != null)
			{
				table.SetText (this.tableColumns[TableColumnKeys.DelayedQuantity].Rank, row, q2);
			}

			if (date != null)
			{
				table.SetText (this.tableColumns[TableColumnKeys.DelayedDate].Rank, row, date);
			}

			table.SetText (this.tableColumns[TableColumnKeys.ArticleId         ].Rank, row, ArticleDocumentItemHelper.GetArticleId (line));
			table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, description);
			table.SetText (this.tableColumns[TableColumnKeys.UnitPrice         ].Rank, row, Misc.PriceToString (line.PrimaryUnitPriceBeforeTax));

			if (line.ResultingLinePriceBeforeTax.HasValue && line.ResultingLineTax.HasValue)
			{
				decimal beforeTax = line.ResultingLinePriceBeforeTax.Value;
				decimal tax =       line.ResultingLineTax.Value;

				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, row, Misc.PriceToString (beforeTax));
				table.SetText (this.tableColumns[TableColumnKeys.Vat      ].Rank, row, Misc.PriceToString (tax));
				table.SetText (this.tableColumns[TableColumnKeys.Total    ].Rank, row, Misc.PriceToString (beforeTax+tax));
			}

			if (line.Discounts.Count != 0)
			{
				if (line.Discounts[0].DiscountRate.HasValue)
				{
					table.SetText (this.tableColumns[TableColumnKeys.Discount].Rank, row, Misc.PercentToString (line.Discounts[0].DiscountRate.Value));
				}

				if (line.Discounts[0].DiscountAmount.HasValue)
				{
					table.SetText (this.tableColumns[TableColumnKeys.Discount].Rank, row, Misc.PriceToString (line.Discounts[0].DiscountAmount.Value));
				}
			}

			return 1;
		}

		private int BuildPriceLine(TableBand table, int row, PriceDocumentItemEntity line)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			//  Une ligne de sous-total PriceDocumentItemEntity peut occuper 2 lignes physiques du tableau,
			//	lorsqu'il y a un rabais. Cela permet de créer un demi-espace vertical entre les lignes
			//	'Sous-total avant rabais / Rabais' et 'Sous-total après rabais'.
			if (this.IsDocumentWithoutPrice)
			{
				return 0;
			}

			string discount = InvoiceDocumentHelper.GetAmount (line);

			//	Colonne "Désignation":
			if (discount == null)
			{
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, line.TextForResultingPrice);
			}
			else
			{
				FormattedText rabais;
				if (line.Discount.DiscountRate.HasValue)
				{
					rabais = FormattedText.Concat ("Rabais ", discount);  // Rabais 20.0%
				}
				else
				{
					rabais = "Rabais";
				}

				FormattedText text = FormattedText.Concat (line.TextForPrimaryPrice, FormattedText.HtmlBreak, rabais);
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row+0, text);
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row+1, line.TextForResultingPrice);
			}

			//	Colonne "Prix HT":
			if (discount == null)
			{
				decimal v1 = line.ResultingPriceBeforeTax.GetValueOrDefault (0);
				string p1 = Misc.PriceToString (v1);

				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, row, p1);
			}
			else
			{
				decimal v1 = line.PrimaryPriceBeforeTax.GetValueOrDefault (0);
				decimal v3 = line.ResultingPriceBeforeTax.GetValueOrDefault (0);

				string p1 = Misc.PriceToString (v1);
				string p2 = Misc.PriceToString (v3 - v1);
				string p3 = Misc.PriceToString (v3);

				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, row+0, FormattedText.Concat (p1, FormattedText.HtmlBreak, p2));
				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, row+1, p3);
			}

			//	Colonne "TVA":
			if (discount == null)
			{
				decimal v1 = line.ResultingTax.GetValueOrDefault (0);
				string p1 = Misc.PriceToString (v1);

				table.SetText (this.tableColumns[TableColumnKeys.Vat].Rank, row, p1);
			}
			else
			{
				decimal v1 = line.PrimaryTax.GetValueOrDefault (0);
				decimal v3 = line.ResultingTax.GetValueOrDefault (0);

				string p1 = Misc.PriceToString (v1);
				string p2 = Misc.PriceToString (v3 - v1);
				string p3 = Misc.PriceToString (v3);

				table.SetText (this.tableColumns[TableColumnKeys.Vat].Rank, row+0, FormattedText.Concat (p1, FormattedText.HtmlBreak, p2));
				table.SetText (this.tableColumns[TableColumnKeys.Vat].Rank, row+1, p3);
			}

			//	Colonne "Prix TTC":
			if (discount == null)
			{
				decimal v1 = line.ResultingPriceBeforeTax.GetValueOrDefault (0) + line.ResultingTax.GetValueOrDefault (0);
				string p1 = Misc.PriceToString (v1);

				table.SetText (this.tableColumns[TableColumnKeys.Total].Rank, row, p1);
			}
			else
			{
				decimal v1 = line.PrimaryPriceBeforeTax.GetValueOrDefault (0) + line.PrimaryTax.GetValueOrDefault (0);
				decimal v3 = line.ResultingPriceBeforeTax.GetValueOrDefault (0) + line.ResultingTax.GetValueOrDefault (0);

				string p1 = Misc.PriceToString (v1);
				string p2 = Misc.PriceToString (v3 - v1);
				string p3 = Misc.PriceToString (v3);

				table.SetText (this.tableColumns[TableColumnKeys.Total].Rank, row+0, FormattedText.Concat (p1, FormattedText.HtmlBreak, p2));
				table.SetText (this.tableColumns[TableColumnKeys.Total].Rank, row+1, p3);
			}

			if (discount == null)
			{
				table.SetCellBorder (row, this.GetCellBorder (bottomBold: true));
			}
			else
			{
				table.SetUnbreakableRow (row, true);

				table.SetCellBorder (row+0, this.GetCellBorder (bottomLess: true));  // pas de trait horizontal entre les 2 fausses lignes
				table.SetCellBorder (row+1, this.GetCellBorder (topLess: true, bottomBold: true));

				table.SetCellMargins (row+0, this.GetCellMargins (bottomForce: 0.5));
				table.SetCellMargins (row+1, this.GetCellMargins (topForce:    0.5));  // un demi-espace entre les 2 fausses lignes
			}

			return InvoiceDocumentHelper.HasAmount (line) ? 2 : 1;  // 2 fausses lignes s'il y a un rabais
		}

		private int BuildTaxLine(TableBand table, int row, TaxDocumentItemEntity line, bool firstTax, bool lastTax)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			if (this.IsDocumentWithoutPrice)
			{
				return 0;
			}

			FormattedText text = FormattedText.Concat (line.Text, " (", Misc.PriceToString (line.BaseAmount), ")");

			table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, text);
			table.SetText (this.tableColumns[TableColumnKeys.LinePrice         ].Rank, row, Misc.PriceToString (line.ResultingTax));

			table.SetCellBorder (row, this.GetCellBorder (bottomLess: true, topLess: true));

			// Adapte les marges comme suit:
			// Seule la première taxe a une marge supérieure normale.
			// Seule la dernière taxe a une marge inférieure normale.
			// Les taxes sont donc serrées entre elles.
			var margins = table.CellMargins;

			if (!firstTax)
			{
				margins.Top = 0;
			}

			if (!lastTax)
			{
				margins.Bottom = 0;
			}

			if (!firstTax || !lastTax)
			{
				table.SetCellMargins (row, margins);
			}

			return 1;
		}

		private int BuildTotalLine(TableBand table, int row, TotalDocumentItemEntity line)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			if (this.IsDocumentWithoutPrice)
			{
				return 0;
			}

			if (line.PrimaryPriceBeforeTax.HasValue)  // ligne de total HT ?
			{
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, line.TextForPrimaryPrice);
				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, row, Misc.PriceToString (line.PrimaryPriceBeforeTax));
			}
			else if (line.FixedPriceAfterTax.HasValue)
			{
				FormattedText text = FormattedText.Join (FormattedText.HtmlBreak, line.TextForPrimaryPrice, line.TextForFixedPrice);
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, text);

				FormattedText total = FormattedText.Concat ("<b>", Misc.PriceToString (line.PrimaryPriceAfterTax), "</b><br/><b><i>", Misc.PriceToString (line.FixedPriceAfterTax), "</i></b>");
				table.SetText (this.tableColumns[TableColumnKeys.Total].Rank, row, total);
			}
			else
			{
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, line.TextForPrimaryPrice);

				FormattedText total = FormattedText.Concat ("<b>", Misc.PriceToString (line.PrimaryPriceAfterTax), "</b>");
				table.SetText (this.tableColumns[TableColumnKeys.Total].Rank, row, total);
			}

			table.SetUnbreakableRow (row, true);

			if (this.IsWithFrame)
			{
				table.SetCellBorder (row, this.GetCellBorder (topLess: true));
				table.SetCellBorder (this.tableColumns[TableColumnKeys.Total].Rank, row, new CellBorder (CellBorder.BoldWidth));
			}
			else
			{
				table.SetCellBorder (row, this.GetCellBorder (bottomBold: true, topLess: true));
			}

			return 1;
		}


		private bool IsPrintableArticle(ArticleDocumentItemEntity line)
		{
			if (!this.EntityPrintingSettings.HasDocumentOption ("Delayed"))  // n'imprime pas les articles retardés ?
			{
				foreach (var quantity in line.ArticleQuantities)
				{
					if (quantity.QuantityType == BusinessLogic.ArticleQuantityType.Billed)
					{
						return true;
					}
				}

				return false;
			}

			return true;
		}


		private void InitializeRowAlignment(TableBand table, int row)
		{
			foreach (var column in this.tableColumns.Values)
			{
				if (column.Visible)
				{
					table.SetAlignment (column.Rank, row, column.Alignment);
				}
			}
		}


		private void BuildConditions(BillingDetailEntity billingDetails)
		{
			//	Met les conditions à la fin de la facture.
			if (this.IsDocumentWithoutPrice)
			{
				return;
			}

			FormattedText conditions = FormattedText.Join (FormattedText.HtmlBreak, billingDetails.Title, billingDetails.AmountDue.PaymentMode.Description);

			if (!conditions.IsNullOrEmpty)
			{
				var band = new TextBand ();
				band.Text = conditions;

				this.documentContainer.AddFromTop (band, 0);
			}
		}

		private void BuildFooter()
		{
			if (this.EntityPrintingSettings.HasDocumentOption ("BL.Signing"))
			{
				var table = new TableBand ();

				table.ColumnsCount = 2;
				table.RowsCount = 1;
				table.CellBorder = CellBorder.Default;
				table.Font = font;
				table.FontSize = fontSize;
				table.CellMargins = new Margins (2);
				table.SetRelativeColumWidth (0,  60);
				table.SetRelativeColumWidth (1, 100);
				table.SetText (0, 0, new FormattedText ("Matériel reçu en bonne et due forme"));
				table.SetText (1, 0, new FormattedText ("Reçu le :<br/><br/>Par :<br/><br/>Signature :<br/><br/><br/>"));
				table.SetUnbreakableRow (0, true);

				this.documentContainer.AddToBottom (table, this.PageMargins.Bottom);
			}

			if (this.EntityPrintingSettings.HasDocumentOption ("Prod.Signing"))
			{
				var table = new TableBand ();

				table.ColumnsCount = 2;
				table.RowsCount = 1;
				table.CellBorder = CellBorder.Default;
				table.Font = font;
				table.FontSize = fontSize;
				table.CellMargins = new Margins (2);
				table.SetRelativeColumWidth (0, 60);
				table.SetRelativeColumWidth (1, 100);
				table.SetText (0, 0, new FormattedText ("Matériel produit en totalité"));
				table.SetText (1, 0, new FormattedText ("Terminé le :<br/><br/>Par :<br/><br/>Signature :<br/><br/><br/>"));
				table.SetUnbreakableRow (0, true);

				this.documentContainer.AddToBottom (table, this.PageMargins.Bottom);
			}

			if (this.EntityPrintingSettings.HasDocumentOption ("Commande.Signing"))
			{
				var table = new TableBand ();

				table.ColumnsCount = 2;
				table.RowsCount = 1;
				table.CellBorder = CellBorder.Default;
				table.Font = font;
				table.FontSize = fontSize;
				table.CellMargins = new Margins (2);
				table.SetRelativeColumWidth (0, 60);
				table.SetRelativeColumWidth (1, 100);
				table.SetText (0, 0, new FormattedText ("Bon pour commande"));
				table.SetText (1, 0, new FormattedText ("Lieu et date :<br/><br/>Signature :<br/><br/><br/>"));
				table.SetUnbreakableRow (0, true);

				this.documentContainer.AddToBottom (table, this.PageMargins.Bottom);
			}
		}

		private void BuildPages(BillingDetailEntity billingDetails, int firstPage)
		{
			//	Met les numéros de page.
			double reportHeight = this.IsDocumentWithoutPrice ? 0 : InvoiceDocumentEntityPrinter.reportHeight*2;

			var leftBounds  = new Rectangle (this.PageMargins.Left, this.PageSize.Height-this.PageMargins.Top+reportHeight+1, 80, 5);
			var rightBounds = new Rectangle (this.PageSize.Width-this.PageMargins.Right-80, this.PageSize.Height-this.PageMargins.Top+reportHeight+1, 80, 5);

			for (int page = firstPage+1; page < this.documentContainer.PageCount (); page++)
			{
				this.documentContainer.CurrentPage = page;

				var leftHeader = new TextBand ();
				leftHeader.Text = InvoiceDocumentHelper.GetTitle (this.entity, billingDetails, this.EntityPrintingSettings.DocumentTypeEnumSelected);
				leftHeader.Alignment = ContentAlignment.BottomLeft;
				leftHeader.Font = font;
				leftHeader.FontSize = 4.0;

				var rightHeader = new TextBand ();
				rightHeader.Text = FormattedText.Concat ("page ", (page-firstPage+1).ToString ());
				rightHeader.Alignment = ContentAlignment.BottomRight;
				rightHeader.Font = font;
				rightHeader.FontSize = fontSize;

				this.documentContainer.AddAbsolute (leftHeader, leftBounds);
				this.documentContainer.AddAbsolute (rightHeader, rightBounds);
			}
		}

		private void BuildReportHeaders(int firstPage)
		{
			//	Met un report en haut des pages concernées, avec une répétition de la ligne
			//	d'en-tête (noms des colonnes).
			double width = this.PageSize.Width-this.PageMargins.Left-this.PageMargins.Right;

			for (int page = firstPage+1; page < this.documentContainer.PageCount (); page++)
			{
				int relativePage = page-firstPage;

				if (relativePage >= this.tableBounds.Count)
				{
					break;
				}

				this.documentContainer.CurrentPage = page;

				var table = new TableBand ();
				table.ColumnsCount = this.visibleColumnCount;
				table.RowsCount = 2;
				table.CellBorder = this.GetCellBorder (bottomBold: true);
				table.CellMargins = new Margins (this.CellMargin);

				//	Génère une première ligne d'en-tête (titres des colonnes).
				foreach (var column in this.tableColumns.Values)
				{
					if (column.Visible)
					{
						table.SetRelativeColumWidth (column.Rank, this.table.GetRelativeColumnWidth (column.Rank));
						table.SetText (column.Rank, 0, column.Title);
					}
				}

				//	Génère une deuxième ligne avec les montants à reporter.
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, 1, "Report");

				decimal sumPT, sumTva, sumTot;
				this.ComputeBottomReports (relativePage-1, out sumPT, out sumTva, out sumTot);
				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, 1, Misc.PriceToString (sumPT));
				table.SetText (this.tableColumns[TableColumnKeys.Vat      ].Rank, 1, Misc.PriceToString (sumTva));
				table.SetText (this.tableColumns[TableColumnKeys.Total    ].Rank, 1, Misc.PriceToString (sumTot));

				this.InitializeRowAlignment (table, 0);
				this.InitializeRowAlignment (table, 1);

				var tableBound = this.tableBounds[relativePage];
				double h = table.RequiredHeight (width);
				var bounds = new Rectangle (tableBound.Left, tableBound.Top, width, h);

				this.documentContainer.AddAbsolute (table, bounds);
			}
		}

		private void BuildReportFooters(int firstPage)
		{
			//	Met un report en bas des pages concernées.
			double width = this.PageSize.Width-this.PageMargins.Left-this.PageMargins.Right;

			for (int page = firstPage; page < this.documentContainer.PageCount ()-1; page++)
			{
				int relativePage = page-firstPage;

				if (relativePage >= this.tableBounds.Count-1)
				{
					//	S'il n'y a pas de tableau dans la page suivante, il est inutile de mettre
					//	un report au bas de celle-çi.
					break;
				}

				this.documentContainer.CurrentPage = page;

				var table = new TableBand ();
				table.ColumnsCount = this.visibleColumnCount;
				table.RowsCount = 1;
				table.CellBorder = this.GetCellBorder (topBold: true);
				table.CellMargins = new Margins (this.CellMargin);

				foreach (var column in this.tableColumns.Values)
				{
					if (column.Visible)
					{
						table.SetRelativeColumWidth (column.Rank, this.table.GetRelativeColumnWidth (column.Rank));
					}
				}

				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, 0, "à reporter");

				decimal sumPT, sumTva, sumTot;
				this.ComputeBottomReports (relativePage, out sumPT, out sumTva, out sumTot);
				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, 0, Misc.PriceToString (sumPT));
				table.SetText (this.tableColumns[TableColumnKeys.Vat      ].Rank, 0, Misc.PriceToString (sumTva));
				table.SetText (this.tableColumns[TableColumnKeys.Total    ].Rank, 0, Misc.PriceToString (sumTot));

				this.InitializeRowAlignment (table, 0);

				var tableBound = this.tableBounds[relativePage];
				double h = table.RequiredHeight (width);
				var bounds = new Rectangle (tableBound.Left, tableBound.Bottom-h, width, h);

				this.documentContainer.AddAbsolute (table, bounds);
			}
		}

		private void ComputeBottomReports(int page, out decimal sumPT, out decimal sumTva, out decimal sumTot)
		{
			//	Calcul les reports à montrer en bas d'une page, ou en haut de la suivante.
			sumPT  = 0;
			sumTva = 0;
			sumTot = 0;

			int lastRow = this.lastRowForEachSection[page];

			for (int row = 0; row <= lastRow; row++)
			{
				if (row == 0)  // en-tête ?
				{
					continue;
				}

				AbstractDocumentItemEntity item = this.entity.Lines[row-1];  // -1 à cause de l'en-tête

				if (item is ArticleDocumentItemEntity)
				{
					var article = item as ArticleDocumentItemEntity;

					decimal beforeTax = article.ResultingLinePriceBeforeTax.GetValueOrDefault (0);
					decimal tax =       article.ResultingLineTax           .GetValueOrDefault (0);

					sumPT  += beforeTax;
					sumTva += tax;
					sumTot += beforeTax+tax;
				}
			}
		}


		private void BuildEsrs(BillingDetailEntity billingDetails, int firstPage)
		{
			if (this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.InvoiceWithInsideESR)
			{
				this.BuildInsideEsrs (billingDetails, firstPage);
			}

			if (this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.InvoiceWithOutsideESR)
			{
				this.BuildOutsideEsr (billingDetails, firstPage);
			}
		}

		private void BuildInsideEsrs(BillingDetailEntity billingDetails, int firstPage)
		{
			//	Met un BVR orangé ou un BV rose en bas de chaque page.
			for (int page = firstPage; page < this.documentContainer.PageCount (); page++)
			{
				this.documentContainer.CurrentPage = page;

				this.BuildEsr (billingDetails, mackle: page != this.documentContainer.PageCount ()-1);
			}
		}

		private void BuildOutsideEsr(BillingDetailEntity billingDetails, int firstPage)
		{
			//	Met un BVR orangé ou un BV rose sur une dernière page séparée.
			this.documentContainer.PrepareEmptyPage (PageType.ESR);

			this.BuildEsr (billingDetails);
		}

		private void BuildEsr(BillingDetailEntity billingDetails, bool mackle=false)
		{
			//	Met un BVR orangé ou un BV rose au bas de la page courante.
			AbstractEsrBand Esr;

			if (this.EntityPrintingSettings.HasDocumentOption ("ESR"))
			{
				Esr = new EsrBand ();  // BVR orangé
			}
			else
			{
				Esr = new EsBand ();  // BV rose
			}

			Esr.PaintEsrSimulator = this.EntityPrintingSettings.HasDocumentOption ("ESR.Simul");
			Esr.PaintSpecimen     = this.EntityPrintingSettings.HasDocumentOption ("ESR.Specimen");
			Esr.From = InvoiceDocumentHelper.GetMailContact (this.entity);
			Esr.To = new FormattedText ("EPSITEC SA<br/>1400 Yverdon-les-Bains");
			Esr.Communication = InvoiceDocumentHelper.GetTitle (this.entity, billingDetails, this.EntityPrintingSettings.DocumentTypeEnumSelected);

			if (mackle)  // faux BV ?
			{
				Esr.NotForUse = true;  // pour imprimer "XXXXX XX"
			}
			else  // vrai BV ?
			{
				Esr.NotForUse          = false;  // c'est LE vrai BV
				Esr.Price              = billingDetails.AmountDue.Amount;
				Esr.EsrCustomerNumber  = billingDetails.EsrCustomerNumber;
				Esr.EsrReferenceNumber = billingDetails.EsrReferenceNumber;
			}

			var bounds = new Rectangle (Point.Zero, AbstractEsrBand.DefautlSize);
			this.documentContainer.AddAbsolute (Esr, bounds);
		}


		private bool IsDocumentWithoutPrice
		{
			get
			{
				return this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.BL ||
					   this.EntityPrintingSettings.DocumentTypeEnumSelected == DocumentType.ProductionOrder;
			}
		}

		private bool IsColumnsOrderQD
		{
			get
			{
				return this.EntityPrintingSettings.HasDocumentOption ("ColumnsOrderQD");
			}
		}

		private double CellMargin
		{
			get
			{
				return this.IsWithFrame ? 1 : 2;
			}
		}

		private Margins GetCellMargins(double? bottomForce = null, double? topForce = null)
		{
			//	Retourne les marges à utiliser pour une ligne entière.
			var margins = this.table.CellMargins;

			if (bottomForce.HasValue)
			{
				margins.Bottom = bottomForce.Value;
			}

			if (topForce.HasValue)
			{
				margins.Top = topForce.Value;
			}

			return margins;
		}

		private CellBorder GetCellBorder(bool bottomBold = false, bool topBold = false, bool bottomLess = false, bool topLess = false)
		{
			//	Retourne les bordures à utiliser pour une ligne entière.
			double leftWidth   = 0;
			double rightWidth  = 0;
			double bottomWidth = 0;
			double topWidth    = 0;

			//	Initialise pour le style choisi.
			if (this.IsWithFrame)
			{
				leftWidth   = CellBorder.NormalWidth;
				rightWidth  = CellBorder.NormalWidth;
				bottomWidth = CellBorder.NormalWidth;
				topWidth    = CellBorder.NormalWidth;
			}
			else if (this.IsWithLine)
			{
				bottomWidth = CellBorder.NormalWidth;
			}

			//	Ajoute ou enlève, selon les exceptions.
			if (bottomBold)
			{
				bottomWidth = CellBorder.BoldWidth;
			}

			if (topBold)
			{
				topWidth = CellBorder.BoldWidth;
			}

			if (bottomLess)
			{
				bottomWidth = 0;
			}

			if (topLess)  // :-O
			{
				topWidth = 0;
			}

			return new CellBorder (leftWidth, rightWidth, bottomWidth, topWidth);
		}

		private bool IsWithLine
		{
			get
			{
				return this.EntityPrintingSettings.HasDocumentOption ("WithLine");
			}
		}

		private bool IsWithFrame
		{
			get
			{
				return this.EntityPrintingSettings.HasDocumentOption ("WithFrame");
			}
		}


		private static readonly Font font = Font.GetFont ("Arial", "Regular");
		private static readonly double fontSize = 3.0;
		private static readonly double reportHeight = 7.0;

		private TableBand			table;
		private int					visibleColumnCount;
		private int[]				lastRowForEachSection;
		private List<Rectangle>		tableBounds;
	}
}
