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
			DocumentType type;

			type = new DocumentType ("ESR", "Facture avec BV", "Facture A4 avec un bulletin de versement orange ou rose intégré au bas de chaque page.");
			type.DocumentOptionsAddInvoice (isBL: false);
			type.DocumentOptionsAddEsr      ();
			this.DocumentTypes.Add (type);

			type = new DocumentType ("Simple", "Facture sans BV", "Facture A4 simple sans bulletin de versement.");
			type.DocumentOptionsAddInvoice     (isBL: false);
			type.DocumentOptionsAddOrientation ();
			type.DocumentOptionsAddMargin      ();
			type.DocumentOptionsAddSpecimen    ();
			this.DocumentTypes.Add (type);

			type = new DocumentType ("BL", "Bulletin de livraison", "Bulletin de livraison A4, sans prix.");
			type.DocumentOptionsAddInvoice     (isBL: true);
			type.DocumentOptionsAddOrientation ();
			type.DocumentOptionsAddMargin      ();
			type.DocumentOptionsAddBL          ();
			type.DocumentOptionsAddSpecimen    ();
			this.DocumentTypes.Add (type);
		}

		public override string JobName
		{
			get
			{
				return UIBuilder.FormatText ("Facture", this.entity.IdA).ToSimpleText ();
			}
		}

		public override Size PageSize
		{
			get
			{
				if (this.HasDocumentOption ("Orientation.Horizontal"))
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
				double h = this.IsBL ? 0 : InvoiceDocumentEntityPrinter.reportHeight;

				if (this.DocumentTypeSelected == "ESR")
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

			if (this.DocumentTypeSelected == "ESR")
			{
				foreach (var billingDetails in this.entity.BillingDetails)
				{
					int firstPage = this.documentContainer.PrepareEmptyPage ();

					this.BuildHeader (billingDetails);
					this.BuildArticles ();
					this.BuildConditions (billingDetails);
					this.BuildPages (billingDetails, firstPage);
					this.BuildReportHeaders (firstPage);
					this.BuildReportFooters (firstPage);
					this.BuildEsrs (billingDetails, firstPage);
				}
			}

			if (this.DocumentTypeSelected == "Simple")
			{
				if (this.entity.BillingDetails.Count != 0)
				{
					var billingDetails = this.entity.BillingDetails[0];
					int firstPage = this.documentContainer.PrepareEmptyPage ();

					this.BuildHeader (billingDetails);
					this.BuildArticles ();
					this.BuildConditions (billingDetails);
					this.BuildPages (billingDetails, firstPage);
					this.BuildReportHeaders (firstPage);
					this.BuildReportFooters (firstPage);
				}
			}

			if (this.DocumentTypeSelected == "BL")
			{
				int firstPage = this.documentContainer.PrepareEmptyPage ();

				this.BuildHeader (null);
				this.BuildArticles ();
				this.BuildFooterBL ();
				this.BuildPages (null, firstPage);
			}
		}

		public override void PrintCurrentPage(IPaintPort port)
		{
			base.PrintCurrentPage (port);

			this.documentContainer.Paint (port, this.CurrentPage, this.IsPreview);
		}


		private void BuildHeader(BillingDetailEntity billingDetails)
		{
			//	Ajoute l'en-tête de la facture dans le document.
			var imageBand = new ImageBand ();
			imageBand.Load("logo-cresus.png");
			imageBand.BuildSections (60, 50, 50, 50);
			this.documentContainer.AddAbsolute (imageBand, new Rectangle (20, this.PageSize.Height-10-50, 60, 50));

			var textBand = new TextBand ();
			textBand.Text = "<b>Les logiciels de gestion</b>";
			textBand.Font = font;
			textBand.FontSize = 5.0;
			this.documentContainer.AddAbsolute (textBand, new Rectangle (20, this.PageSize.Height-10-imageBand.GetSectionHeight (0)-10, 80, 10));

			var mailContactBand = new TextBand ();
			mailContactBand.Text = InvoiceDocumentHelper.GetMailContact (this.entity);
			mailContactBand.Font = font;
			mailContactBand.FontSize = fontSize;
			this.documentContainer.AddAbsolute (mailContactBand, new Rectangle (120, this.PageSize.Height-57, 80, 25));

			if (!string.IsNullOrEmpty (this.entity.DocumentTitle))
			{
				var concerneBand = new TableBand ();
				concerneBand.ColumnsCount = 2;
				concerneBand.RowsCount = 1;
				concerneBand.PaintFrame = false;
				concerneBand.Font = font;
				concerneBand.FontSize = fontSize;
				concerneBand.CellMargins = new Margins (0);
				concerneBand.SetRelativeColumWidth (0, 15);
				concerneBand.SetRelativeColumWidth (1, 80);
				concerneBand.SetText (0, 0, "Concerne");
				concerneBand.SetText (1, 0, this.entity.DocumentTitle);
				this.documentContainer.AddAbsolute (concerneBand, new Rectangle (20, this.PageSize.Height-67, 100, 15));
			}

			var titleBand = new TextBand ();
			titleBand.Text = InvoiceDocumentHelper.GetTitle (this.entity, billingDetails, this.IsBL);
			titleBand.Font = font;
			titleBand.FontSize = 5.0;
			this.documentContainer.AddAbsolute (titleBand, new Rectangle (20, this.PageSize.Height-82, 90, 10));

			string date = Misc.GetDateTimeDescription (this.entity.LastModificationDate);
			var dateBand = new TextBand ();
			dateBand.Text = UIBuilder.FormatText ("Crissier, le ", date).ToString ();
			dateBand.Font = font;
			dateBand.FontSize = fontSize;
			this.documentContainer.AddAbsolute (dateBand, new Rectangle (120, this.PageSize.Height-82, 80, 10));
		}

		private void BuildArticles()
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
				this.tableColumns.Add (TableColumnKeys.Discount,           new TableColumn ("Rabais",      priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.UnitPrice,          new TableColumn ("p.u. HT",     priceWidth,   ContentAlignment.MiddleRight));
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
				this.tableColumns.Add (TableColumnKeys.Discount,           new TableColumn ("Rabais",      priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.UnitPrice,          new TableColumn ("p.u. HT",     priceWidth,   ContentAlignment.MiddleRight));
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
					bool exist = false;

					if (line is TextDocumentItemEntity)
					{
						exist = this.InitializeColumnTextLine (line as TextDocumentItemEntity);
					}

					if (line is ArticleDocumentItemEntity)
					{
						exist = this.InitializeColumnArticleLine (line as ArticleDocumentItemEntity);
					}

					if (line is PriceDocumentItemEntity)
					{
						exist = this.InitializeColumnPriceLine (line as PriceDocumentItemEntity);
					}

					if (line is TaxDocumentItemEntity)
					{
						exist = this.InitializeColumnTaxLine (line as TaxDocumentItemEntity);
					}

					if (line is TotalDocumentItemEntity)
					{
						exist = this.InitializeColumnTotalLine (line as TotalDocumentItemEntity);
					}

					if (exist)
					{
						rowCount++;
					}
				}
			}

			if (this.IsBL)
			{
				this.tableColumns[TableColumnKeys.Discount  ].Visible = false;
				this.tableColumns[TableColumnKeys.UnitPrice ].Visible = false;
				this.tableColumns[TableColumnKeys.LinePrice ].Visible = false;
				this.tableColumns[TableColumnKeys.Vat       ].Visible = false;
				this.tableColumns[TableColumnKeys.Total     ].Visible = false;
			}

			if (!this.HasDocumentOption ("Delayed"))  // n'imprime pas les articles retardés ?
			{
				this.tableColumns[TableColumnKeys.DelayedQuantity].Visible = false;
				this.tableColumns[TableColumnKeys.DelayedDate    ].Visible = false;
			}

			if (!this.HasDocumentOption ("ArticleId"))  // n'imprime pas les numéros d'article ?
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
			this.table.PaintFrame = this.IsWithFrame;
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

			row++;

			//	Génère toutes les lignes pour les articles.
			int linePage = this.documentContainer.CurrentPage;
			double lineY = this.documentContainer.CurrentVerticalPosition;

			foreach (var line in this.entity.Lines)
			{
				if (line.Visibility)
				{
					bool exist = false;

					if (line is TextDocumentItemEntity)
					{
						exist = this.BuildTextLine (this.table, row, line as TextDocumentItemEntity);
					}

					if (line is ArticleDocumentItemEntity)
					{
						exist = this.BuildArticleLine (this.table, row, line as ArticleDocumentItemEntity);
					}

					if (line is PriceDocumentItemEntity)
					{
						exist = this.BuildPriceLine (this.table, row, line as PriceDocumentItemEntity);
					}

					if (line is TaxDocumentItemEntity)
					{
						exist = this.BuildTaxLine (this.table, row, line as TaxDocumentItemEntity);
					}

					if (line is TotalDocumentItemEntity)
					{
						exist = this.BuildTotalLine (this.table, row, line as TotalDocumentItemEntity);
					}

					if (exist)
					{
						this.InitializeRowAlignment (this.table, row);
						row++;
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

			// Met un trait horizontal sous l'en-tête.
			var currentPage = this.documentContainer.CurrentPage;
			this.documentContainer.CurrentPage = 0;  // dans la première page

			var h = this.table.GetRowHeight (0);
			this.BuildSeparator (lineY-h);

			this.documentContainer.CurrentPage = currentPage;
		}


		private bool InitializeColumnTextLine(TextDocumentItemEntity line)
		{
			this.tableColumns[TableColumnKeys.ArticleDescription].Visible = true;
			return true;
		}

		private bool InitializeColumnArticleLine(ArticleDocumentItemEntity line)
		{
			if (this.IsBL && !ArticleDocumentItemHelper.IsArticleForBL (line))
			{
				return false;
			}

			if (!this.IsPrintableArticle (line))
			{
				return false;
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

			return true;
		}

		private bool InitializeColumnPriceLine(PriceDocumentItemEntity line)
		{
			if (this.IsBL)
			{
				return false;
			}

			this.tableColumns[TableColumnKeys.ArticleDescription].Visible = true;
			this.tableColumns[TableColumnKeys.LinePrice         ].Visible = true;
			this.tableColumns[TableColumnKeys.Vat               ].Visible = true;
			this.tableColumns[TableColumnKeys.Total             ].Visible = true;

			if (InvoiceDocumentHelper.HasAmount (line))
			{
				this.tableColumns[TableColumnKeys.Discount].Visible = true;
			}

			return true;
		}

		private bool InitializeColumnTaxLine(TaxDocumentItemEntity line)
		{
			if (this.IsBL)
			{
				return false;
			}

			this.tableColumns[TableColumnKeys.ArticleDescription].Visible = true;
			this.tableColumns[TableColumnKeys.LinePrice         ].Visible = true;

			return true;
		}

		private bool InitializeColumnTotalLine(TotalDocumentItemEntity line)
		{
			if (this.IsBL)
			{
				return false;
			}

			this.tableColumns[TableColumnKeys.ArticleDescription].Visible = true;
			this.tableColumns[TableColumnKeys.Discount          ].Visible = line.FixedPriceAfterTax.HasValue;
			this.tableColumns[TableColumnKeys.Total             ].Visible = true;

			return true;
		}


		private bool BuildTextLine(TableBand table, int row, TextDocumentItemEntity line)
		{
			string text = string.Concat ("<b>", line.Text, "</b>");
			table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, text);

			return true;
		}

		private bool BuildArticleLine(TableBand table, int row, ArticleDocumentItemEntity line)
		{
			if (this.IsBL && !ArticleDocumentItemHelper.IsArticleForBL (line))
			{
				return false;
			}

			if (!this.IsPrintableArticle (line))
			{
				return false;
			}

			string q1 = null;
			string q2 = null;
			string date = null;

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

			string  description = ArticleDocumentItemHelper.GetArticleDescription (line);

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

			return true;
		}

		private bool BuildPriceLine(TableBand table, int row, PriceDocumentItemEntity line)
		{
			if (this.IsBL)
			{
				return false;
			}

			string discount = InvoiceDocumentHelper.GetAmount (line);

			//	Colonne "Désignation":
			if (discount == null)
			{
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, line.TextForResultingPrice);
			}
			else
			{
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, string.Concat (line.TextForPrimaryPrice, "<br/>", line.TextForResultingPrice));
			}

			//	Colonne "Rabais":
			if (discount != null)
			{
				table.SetText (this.tableColumns[TableColumnKeys.Discount].Rank, row, string.Concat ("<br/>", discount));
			}

			//	Colonne "Prix HT":
			if (discount == null)
			{
				string p1 = Misc.PriceToString (line.ResultingPriceBeforeTax.GetValueOrDefault (0));
				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, row, p1);
			}
			else
			{
				string p1 = Misc.PriceToString (line.PrimaryPriceBeforeTax.GetValueOrDefault (0));
				string p2 = Misc.PriceToString (line.ResultingPriceBeforeTax.GetValueOrDefault (0));
				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, row, string.Concat (p1, "<br/>", p2));
			}

			//	Colonne "TVA":
			if (discount == null)
			{
				string p1 = Misc.PriceToString (line.ResultingTax.GetValueOrDefault (0));
				table.SetText (this.tableColumns[TableColumnKeys.Vat].Rank, row, p1);
			}
			else
			{
				string p1 = Misc.PriceToString (line.PrimaryTax.GetValueOrDefault (0));
				string p2 = Misc.PriceToString (line.ResultingTax.GetValueOrDefault (0));
				table.SetText (this.tableColumns[TableColumnKeys.Vat].Rank, row, string.Concat (p1, "<br/>", p2));
			}

			//	Colonne "Prix TTC":
			string total;

			if (line.FixedPriceAfterTax.HasValue)  // valeur imposée ?
			{
				total = Misc.PriceToString (line.FixedPriceAfterTax);
			}
			else
			{
				total = Misc.PriceToString (line.ResultingPriceBeforeTax.GetValueOrDefault (0) + line.ResultingTax.GetValueOrDefault (0));
			}

			table.SetText (this.tableColumns[TableColumnKeys.Total].Rank, row, string.Concat (discount == null ? "" : "<br/>", total));

			table.SetUnbreakableRow (row, true);

			return true;
		}

		private bool BuildTaxLine(TableBand table, int row, TaxDocumentItemEntity line)
		{
			if (this.IsBL)
			{
				return false;
			}

			string text = string.Concat (line.Text, " (", Misc.PriceToString (line.BaseAmount), ")");

			table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, text);
			table.SetText (this.tableColumns[TableColumnKeys.LinePrice         ].Rank, row, Misc.PriceToString (line.ResultingTax));

			return true;
		}

		private bool BuildTotalLine(TableBand table, int row, TotalDocumentItemEntity line)
		{
			if (this.IsBL)
			{
				return false;
			}

			if (line.FixedPriceAfterTax.HasValue)
			{
				string text = string.Join ("<br/>", line.TextForPrimaryPrice, line.TextForFixedPrice);
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, text);

				string discount = string.Concat ("<br/>", Misc.PriceToString (line.PrimaryPriceAfterTax - line.FixedPriceAfterTax));
				table.SetText (this.tableColumns[TableColumnKeys.Discount].Rank, row, discount);

				string total = string.Concat (Misc.PriceToString (line.PrimaryPriceAfterTax), "<br/><b>", Misc.PriceToString (line.FixedPriceAfterTax), "</b>");
				table.SetText (this.tableColumns[TableColumnKeys.Total].Rank, row, total);
			}
			else
			{
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, line.TextForPrimaryPrice);

				string total = string.Concat ("<b>", Misc.PriceToString (line.PrimaryPriceAfterTax), "</b>");
				table.SetText (this.tableColumns[TableColumnKeys.Total].Rank, row, total);
			}

			table.SetUnbreakableRow (row, true);
			table.SetCellBorderWidth (this.tableColumns[TableColumnKeys.Total].Rank, row, 0.5);

			return true;
		}


		private bool IsPrintableArticle(ArticleDocumentItemEntity line)
		{
			if (!this.HasDocumentOption ("Delayed"))  // n'imprime pas les articles retardés ?
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
			if (this.IsBL)
			{
				return;
			}

			string conditions = string.Join("<br/>", billingDetails.Title, billingDetails.AmountDue.PaymentMode.Description);

			if (!string.IsNullOrEmpty (conditions))
			{
				var band = new TextBand ();
				band.Text = conditions;

				this.documentContainer.AddFromTop (band, 0);
			}
		}

		private void BuildFooterBL()
		{
			if (this.HasDocumentOption ("BL.Signing"))
			{
				var table = new TableBand ();

				table.ColumnsCount = 2;
				table.RowsCount = 1;
				table.PaintFrame = true;
				table.Font = font;
				table.FontSize = fontSize;
				table.CellMargins = new Margins (2);
				table.SetRelativeColumWidth (0,  60);
				table.SetRelativeColumWidth (1, 100);
				table.SetText (0, 0, "Matériel reçu en bonne et due forme");
				table.SetText (1, 0, "Reçu le :<br/><br/>Par :<br/><br/>Signature :<br/><br/><br/>");
				table.SetUnbreakableRow (0, true);

				this.documentContainer.AddToBottom (table, this.PageMargins.Bottom);
			}
		}

		private void BuildPages(BillingDetailEntity billingDetails, int firstPage)
		{
			//	Met les numéros de page.
			double reportHeight = this.IsBL ? 0 : InvoiceDocumentEntityPrinter.reportHeight*2;

			var leftBounds  = new Rectangle (this.PageMargins.Left, this.PageSize.Height-this.PageMargins.Top+reportHeight+1, 80, 5);
			var rightBounds = new Rectangle (this.PageSize.Width-this.PageMargins.Right-80, this.PageSize.Height-this.PageMargins.Top+reportHeight+1, 80, 5);

			for (int page = firstPage+1; page < this.documentContainer.PageCount; page++)
			{
				this.documentContainer.CurrentPage = page;

				var leftHeader = new TextBand ();
				leftHeader.Text = InvoiceDocumentHelper.GetTitle (this.entity, billingDetails, this.IsBL);
				leftHeader.Alignment = ContentAlignment.BottomLeft;
				leftHeader.Font = font;
				leftHeader.FontSize = 4.0;

				var rightHeader = new TextBand ();
				rightHeader.Text = string.Format ("page {0}", (page-firstPage+1).ToString ());
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

			for (int page = firstPage+1; page < this.documentContainer.PageCount; page++)
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
				table.PaintFrame = this.IsWithFrame;
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

				// Met un trait horizontal sous l'en-tête.
				h = table.GetRowHeight (0);
				this.BuildSeparator (bounds.Top-h);
			}
		}

		private void BuildReportFooters(int firstPage)
		{
			//	Met un report en bas des pages concernées.
			double width = this.PageSize.Width-this.PageMargins.Left-this.PageMargins.Right;

			for (int page = firstPage; page < this.documentContainer.PageCount-1; page++)
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
				table.PaintFrame = this.IsWithFrame;
				table.CellMargins = new Margins (this.CellMargin);

				foreach (var column in this.tableColumns.Values)
				{
					if (column.Visible)
					{
						table.SetRelativeColumWidth (column.Rank, this.table.GetRelativeColumnWidth (column.Rank));
					}
				}

				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, 0, "Reporté");

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

				// Met un trait horizontal sur le report.
				this.BuildSeparator (bounds.Top);
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
			//	Met un BVR orangé ou un BV rose en bas de chaque page.
			var bounds = new Rectangle (Point.Zero, AbstractEsrBand.DefautlSize);

			for (int page = firstPage; page < this.documentContainer.PageCount; page++)
			{
				this.documentContainer.CurrentPage = page;

				AbstractEsrBand Esr;

				if (this.HasDocumentOption ("ESR"))
				{
					Esr = new EsrBand ();  // BVR orangé
				}
				else
				{
					Esr = new EsBand ();  // BV rose
				}

				Esr.PaintEsrSimulator = this.HasDocumentOption ("ESR.Simul");
				Esr.PaintSpecimen     = this.HasDocumentOption ("ESR.Specimen");
				Esr.From = InvoiceDocumentHelper.GetMailContact (this.entity);
				Esr.To = "EPSITEC SA<br/>1400 Yverdon-les-Bains";
				Esr.Communication = InvoiceDocumentHelper.GetTitle (this.entity, billingDetails, this.IsBL);

				if (page == this.documentContainer.PageCount-1)  // dernière page ?
				{
					Esr.NotForUse          = false;  // c'est LE vrai BV
					Esr.Price              = billingDetails.AmountDue.Amount;
					Esr.EsrCustomerNumber  = billingDetails.EsrCustomerNumber;
					Esr.EsrReferenceNumber = billingDetails.EsrReferenceNumber;
				}
				else  // faux BV ?
				{
					Esr.NotForUse = true;  // pour imprimer "XXXXX XX"
				}

				this.documentContainer.AddAbsolute (Esr, bounds);
			}
		}


		private void BuildSeparator(double y, double width=0.5)
		{
			//	Met un séparateur horizontal.
			var line = new SurfaceBand ()
			{
				Height = width,
			};

			var bounds = new Rectangle (this.PageMargins.Left, y-width/2, this.PageSize.Width-this.PageMargins.Left-this.PageMargins.Right, line.Height);
			this.documentContainer.AddAbsolute (line, bounds);
		}


		private bool IsBL
		{
			get
			{
				return this.DocumentTypeSelected == "BL";
			}
		}

		private bool IsColumnsOrderQD
		{
			get
			{
				return this.HasDocumentOption ("ColumnsOrderQD");
			}
		}

		private double CellMargin
		{
			get
			{
				return this.IsWithFrame ? 1 : 2;
			}
		}

		private bool IsWithFrame
		{
			get
			{
				return this.HasDocumentOption ("WithFrame");
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
