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
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Print;
using Epsitec.Cresus.Core.Print.Bands;
using Epsitec.Cresus.Core.Print.Containers;
using Epsitec.Cresus.Core.Print.EntityPrinters;
using Epsitec.Cresus.Core.Resolvers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.EntityPrinters
{
	public class DocumentMetadataPrinter : AbstractPrinter
	{
		private DocumentMetadataPrinter(IBusinessContext businessContext, AbstractEntity entity, OptionsDictionary options, PrintingUnitsDictionary printingUnits)
			: base (businessContext, entity, options, printingUnits)
		{
		}

		public override string JobName
		{
			get
			{
				return FormattedText.Concat (InvoiceDocumentHelper.GetDocumentName (this.Metadata), " ", this.Metadata.IdA).ToSimpleText ();
			}
		}


		public override Size PreferredPageSize
		{
			get
			{
				double dx = 210;
				double dy = (this.PreviewMode == PreviewMode.ContinuousPreview) ? AbstractPrinter.continuousHeight : 297;

				return new Size (dx, dy);  // A4
			}
		}


		protected override Margins PageMargins
		{
			get
			{
				double leftMargin   = this.GetOptionValue (DocumentOption.LeftMargin,   20);
				double rightMargin  = this.GetOptionValue (DocumentOption.RightMargin,  20);
				double topMargin    = this.GetOptionValue (DocumentOption.TopMargin,    20);
				double bottomMargin = this.GetOptionValue (DocumentOption.BottomMargin, 20);

				double h = this.IsDocumentWithoutPrice ? 0 : DocumentMetadataPrinter.reportHeight;

				if (this.HasOption (DocumentOption.EsrPosition, "WithInside"))
				{
					return new Margins (leftMargin, rightMargin, topMargin+h*2, h+DocumentMetadataPrinter.marginBeforeEsr+AbstractEsrBand.DefautlSize.Height);
				}
				else
				{
					return new Margins (leftMargin, rightMargin, topMargin+h*2, h+bottomMargin);
				}
			}
		}


		public override void BuildSections()
		{
			base.BuildSections ();

			this.documentContainer.Clear ();

			if (this.DocumentType == Business.DocumentType.SalesQuote)
			{
				int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

				this.BuildHeader (null);
				this.BuildArticles ();
				this.BuildPages (null, firstPage);

				this.documentContainer.Ending (firstPage);
			}

			if (this.DocumentType == Business.DocumentType.OrderBooking)
			{
				int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

				this.BuildHeader (null);
				this.BuildArticles ();
				this.BuildFooter ();
				this.BuildPages (null, firstPage);

				this.documentContainer.Ending (firstPage);
			}

			if (this.DocumentType == Business.DocumentType.OrderConfirmation)
			{
				int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

				this.BuildHeader (null);
				this.BuildArticles ();
				this.BuildPages (null, firstPage);

				this.documentContainer.Ending (firstPage);
			}

			if (this.DocumentType == Business.DocumentType.ProductionOrder)
			{
				int documentRank = 0;
				var groups = this.GetProdGroups ();
				foreach (var group in groups)
				{
					this.documentContainer.DocumentRank = documentRank++;
					int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

					this.BuildHeader (null, group);
					this.BuildArticles (group);
					this.BuildFooter ();
					this.BuildPages (null, firstPage);

					this.documentContainer.Ending (firstPage);
				}
			}

			if (this.DocumentType == Business.DocumentType.DeliveryNote)
			{
				int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

				this.BuildHeader (null);
				this.BuildArticles ();
				this.BuildFooter ();
				this.BuildPages (null, firstPage);

				this.documentContainer.Ending (firstPage);
			}

			if (this.DocumentType == Business.DocumentType.Invoice)
			{
				if (this.HasOption (DocumentOption.EsrPosition, "Without") || this.PreviewMode == Print.PreviewMode.ContinuousPreview)
				{
					if (this.Entity.BillingDetails.Count != 0)
					{
						var billingDetails = this.Entity.BillingDetails[0];
						int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

						this.BuildHeader (billingDetails);
						this.BuildArticles ();
						this.BuildConditions (billingDetails);
						this.BuildPages (billingDetails, firstPage);
						this.BuildReportHeaders (firstPage);
						this.BuildReportFooters (firstPage);

						this.documentContainer.Ending (firstPage);
					}
				}
				else
				{
					int documentRank = 0;
					bool onlyTotal = false;
					foreach (var billingDetails in this.Entity.BillingDetails)
					{
						this.documentContainer.DocumentRank = documentRank++;
						int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

						this.BuildHeader (billingDetails);
						this.BuildArticles (onlyTotal: onlyTotal);
						this.BuildConditions (billingDetails);
						this.BuildPages (billingDetails, firstPage);
						this.BuildReportHeaders (firstPage);
						this.BuildReportFooters (firstPage);
						this.BuildEsrs (billingDetails, firstPage);

						this.documentContainer.Ending (firstPage);
						onlyTotal = true;
					}
				}
			}
		}

		public override void PrintBackgroundCurrentPage(IPaintPort port)
		{
			this.documentContainer.PaintBackground (port, this.CurrentPage, this.PreviewMode);

			base.PrintBackgroundCurrentPage (port);
		}

		public override void PrintForegroundCurrentPage(IPaintPort port)
		{
			this.documentContainer.PaintForeground (port, this.CurrentPage, this.PreviewMode);

			base.PrintForegroundCurrentPage (port);
		}


		private List<ArticleGroupEntity> GetProdGroups()
		{
			//	Retourne la liste des groupes des articles du document.
			var groups = new List<ArticleGroupEntity> ();

			foreach (var line in this.Entity.Lines)
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
			double leftMargin = this.GetOptionValue (DocumentOption.LeftMargin, 20);

			//	Ajoute l'en-tête de la facture dans le document.
			if (this.HasOption (DocumentOption.HeaderLogo))
			{
				BusinessContext context = this.businessContext as BusinessContext;
				var settings = context.GetCachedBusinessSettings ();

				if (settings.CompanyLogo.IsNotNull ())
				{
					//	Affiche l'image du logo de l'entreprise.
					var imageBand = new ImageBand ();
					imageBand.Load (this.coreData, settings.CompanyLogo);
					imageBand.BuildSections (80, 40, 40, 40);
					this.documentContainer.AddAbsolute (imageBand, new Rectangle (leftMargin, this.RequiredPageSize.Height-10-40, 80, 40));

					if (settings.Company.IsNotNull ())
					{
						if (settings.Company.Person is LegalPersonEntity)
						{
							var legalPerson = settings.Company.Person as LegalPersonEntity;

							if (!legalPerson.Complement.IsNullOrWhiteSpace)
							{
								//	Affiche le texte sous le logo de l'entreprise.
								var textBand = new TextBand ();
								textBand.Text = FormattedText.Concat ("<b>", legalPerson.Complement, "</b>");
								textBand.Font = font;
								textBand.FontSize = this.FontSize*1.6;
								this.documentContainer.AddAbsolute (textBand, new Rectangle (leftMargin, this.RequiredPageSize.Height-10-imageBand.GetSectionHeight (0)-10, 80, 10));
							}
						}
					}
				}
			}

			var mailContactBand = new TextBand ();
			mailContactBand.Text = this.Entity.BillToMailContact.GetSummary ();
			mailContactBand.Font = font;
			mailContactBand.FontSize = this.FontSize;
			this.documentContainer.AddAbsolute (mailContactBand, new Rectangle (120, this.RequiredPageSize.Height-57, 80, 25));

			//	Génère le groupe "concerne".
			{
				FormattedText title, text;
				CellBorder    cellBorder;
				Margins       margins;
				Color         color;

				if (group == null)
				{
					title      = "Concerne";
					text       = TextFormatter.FormatText (this.Metadata.DocumentTitle);
					cellBorder = CellBorder.Empty;
					margins    = new Margins (0);
					color      = Color.Empty;
				}
				else
				{
					var groupName = TextFormatter.FormatText (group.Name).ToSimpleText ();

					title      = "Atelier";
					text       = FormattedText.Concat ("<b>", string.IsNullOrWhiteSpace (groupName) ? group.Code : groupName, "</b>");
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
					band.FontSize = this.FontSize;
					band.CellMargins = margins;
					band.SetRelativeColumWidth (0, 15);
					band.SetRelativeColumWidth (1, 80);
					band.SetText (0, 0, title, this.FontSize);
					band.SetText (1, 0, text,  this.FontSize);
					band.SetBackground (1, 0, color);
					this.documentContainer.AddAbsolute (band, new Rectangle (leftMargin, this.RequiredPageSize.Height-70, 100-5, 12));
				}
			}

			var titleBand = new TextBand ();
			titleBand.Text = InvoiceDocumentHelper.GetTitle (this.Metadata, this.Entity, billingDetails);
			titleBand.Font = font;
			titleBand.FontSize = this.FontSize*1.6;
			this.documentContainer.AddAbsolute (titleBand, new Rectangle (leftMargin, this.RequiredPageSize.Height-82, 90, 10));

			string date = Misc.GetDateShortDescription (this.Entity.BillingDate);
			var dateBand = new TextBand ();
			dateBand.Text = FormattedText.Concat (DocumentMetadataPrinter.GetDefaultLocation (), ", le ", date);
			dateBand.Font = font;
			dateBand.FontSize = this.FontSize;
			this.documentContainer.AddAbsolute (dateBand, new Rectangle (120, this.RequiredPageSize.Height-82, 80, 10-2));
		}

		private void BuildArticles(ArticleGroupEntity group=null, bool onlyTotal=false)
		{
			//	Ajoute les articles dans le document.
			this.documentContainer.CurrentVerticalPosition = this.RequiredPageSize.Height-87;

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
			int firstLine = onlyTotal ? this.Entity.Lines.Count-1 : 0;
			int rowCount = 1;  // déjà 1 pour l'en-tête (titres des colonnes)

			for (int i = firstLine; i < this.Entity.Lines.Count; i++)
			{
				var line = this.Entity.Lines[i];

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

					if (line is SubTotalDocumentItemEntity)
					{
						rowUsed = this.InitializeColumnPriceLine (line as SubTotalDocumentItemEntity);
					}

					if (line is TaxDocumentItemEntity)
					{
						rowUsed = this.InitializeColumnTaxLine (line as TaxDocumentItemEntity);
					}

					if (line is EndTotalDocumentItemEntity)
					{
						rowUsed = this.InitializeColumnTotalLine (line as EndTotalDocumentItemEntity);
					}

					rowCount += rowUsed;
				}
			}

			if (this.DocumentType == Business.DocumentType.DeliveryNote)
			{
				this.tableColumns[TableColumnKeys.Discount ].Visible = false;
				this.tableColumns[TableColumnKeys.UnitPrice].Visible = false;
				this.tableColumns[TableColumnKeys.LinePrice].Visible = false;
				this.tableColumns[TableColumnKeys.Vat      ].Visible = false;
				this.tableColumns[TableColumnKeys.Total    ].Visible = false;
			}

			if (this.DocumentType == Business.DocumentType.ProductionOrder)
			{
				this.tableColumns[TableColumnKeys.DelayedQuantity].Visible = false;
				this.tableColumns[TableColumnKeys.DelayedDate    ].Visible = false;
				this.tableColumns[TableColumnKeys.Discount       ].Visible = false;
				this.tableColumns[TableColumnKeys.UnitPrice      ].Visible = false;
				this.tableColumns[TableColumnKeys.LinePrice      ].Visible = false;
				this.tableColumns[TableColumnKeys.Vat            ].Visible = false;
				this.tableColumns[TableColumnKeys.Total          ].Visible = false;
			}

			if (!this.HasOption (DocumentOption.ArticleDelayed))  // n'imprime pas les articles retardés ?
			{
				this.tableColumns[TableColumnKeys.DelayedQuantity].Visible = false;
				this.tableColumns[TableColumnKeys.DelayedDate    ].Visible = false;
			}

			if (!this.HasOption (DocumentOption.ArticleId))  // n'imprime pas les numéros d'article ?
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
					this.table.SetText (column.Rank, row, column.Title, this.FontSize);
				}
			}

			this.InitializeRowAlignment (this.table, row);
			this.table.SetCellBorder (row, this.GetCellBorder (bottomBold: true));

			row++;

			//	Génère toutes les lignes pour les articles.
			int linePage = this.documentContainer.CurrentPage;
			double lineY = this.documentContainer.CurrentVerticalPosition;

			for (int i = firstLine; i < this.Entity.Lines.Count; i++)
			{
				var line = this.Entity.Lines[i];

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

					if (line is SubTotalDocumentItemEntity)
					{
						rowUsed = this.BuildPriceLine (this.table, row, line as SubTotalDocumentItemEntity);
					}

					if (line is TaxDocumentItemEntity)
					{
						bool firstTax = (i > 0                         && !(this.Entity.Lines[i-1] is TaxDocumentItemEntity));
						bool lastTax  = (i < this.Entity.Lines.Count-1 && !(this.Entity.Lines[i+1] is TaxDocumentItemEntity));

						rowUsed = this.BuildTaxLine (this.table, row, line as TaxDocumentItemEntity, firstTax, lastTax);
					}

					if (line is EndTotalDocumentItemEntity)
					{
						rowUsed = this.BuildTotalLine (this.table, row, line as EndTotalDocumentItemEntity);
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
			if (this.DocumentType == Business.DocumentType.DeliveryNote && !ArticleDocumentItemHelper.IsArticleForBL (line))
			{
				return 0;
			}

			if (this.DocumentType == Business.DocumentType.ProductionOrder && !ArticleDocumentItemHelper.IsArticleForProd (line, group))
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

			if (line.VatCode != Business.Finance.VatCode.None &&
				line.VatCode != Business.Finance.VatCode.Excluded &&
				line.VatCode != Business.Finance.VatCode.ZeroRated)
			{
				this.tableColumns[TableColumnKeys.Vat].Visible = true;
			}

			foreach (var quantity in line.ArticleQuantities)
			{
				if (quantity.QuantityType == Business.ArticleQuantityType.Billed)
				{
					this.tableColumns[TableColumnKeys.Quantity ].Visible = true;
					this.tableColumns[TableColumnKeys.UnitPrice].Visible = true;
					this.tableColumns[TableColumnKeys.LinePrice].Visible = true;
				}

				if (quantity.QuantityType == Business.ArticleQuantityType.Delayed)
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

		private int InitializeColumnPriceLine(SubTotalDocumentItemEntity line)
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

		private int InitializeColumnTotalLine(EndTotalDocumentItemEntity line)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			if (this.IsDocumentWithoutPrice)
			{
				return 0;
			}

			this.tableColumns[TableColumnKeys.ArticleDescription].Visible = true;

			if (line.PriceBeforeTax.HasValue)  // ligne de total HT ?
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
			table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, text, this.FontSize);

			return 1;
		}

		private int BuildArticleLine(TableBand table, int row, ArticleDocumentItemEntity line, ArticleGroupEntity group)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			if (this.DocumentType == Business.DocumentType.DeliveryNote && !ArticleDocumentItemHelper.IsArticleForBL (line))
			{
				return 0;
			}

			if (this.DocumentType == Business.DocumentType.ProductionOrder && !ArticleDocumentItemHelper.IsArticleForProd (line, group))
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
				if (quantity.QuantityType == Business.ArticleQuantityType.Billed)
				{
					q1 = Misc.FormatUnit (quantity.Quantity, quantity.Unit.Code);
				}

				if (quantity.QuantityType == Business.ArticleQuantityType.Delayed)
				{
					q2.AppendLine (Misc.FormatUnit (quantity.Quantity, quantity.Unit.Code));

					if (quantity.ExpectedDate.HasValue)
					{
						date.AppendLine (quantity.ExpectedDate.Value.ToString ());
					}
				}
			}

			FormattedText description = ArticleDocumentItemHelper.GetArticleDescription (line, replaceTags: true, shortDescription: this.DocumentType == Business.DocumentType.ProductionOrder);

			if (q1 != null)
			{
				table.SetText (this.tableColumns[TableColumnKeys.Quantity].Rank, row, q1, this.FontSize);
			}

			if (q2 != null)
			{
				table.SetText (this.tableColumns[TableColumnKeys.DelayedQuantity].Rank, row, q2, this.FontSize);
			}

			if (date != null)
			{
				table.SetText (this.tableColumns[TableColumnKeys.DelayedDate].Rank, row, date, this.FontSize);
			}

			table.SetText (this.tableColumns[TableColumnKeys.ArticleId         ].Rank, row, ArticleDocumentItemHelper.GetArticleId (line),       this.FontSize);
			table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, description,                                         this.FontSize);
			table.SetText (this.tableColumns[TableColumnKeys.UnitPrice         ].Rank, row, Misc.PriceToString (line.PrimaryUnitPriceBeforeTax), this.FontSize);

			if (line.ResultingLinePriceBeforeTax.HasValue && line.ResultingLineTax1.HasValue)
			{
				decimal beforeTax = line.ResultingLinePriceBeforeTax.Value;
				decimal tax =       line.ResultingLineTax1.Value;

				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, row, Misc.PriceToString (beforeTax),     this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.Vat      ].Rank, row, Misc.PriceToString (tax),           this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.Total    ].Rank, row, Misc.PriceToString (beforeTax+tax), this.FontSize);
			}

			if (line.Discounts.Count != 0)
			{
				if (line.Discounts[0].DiscountRate.HasValue)
				{
					table.SetText (this.tableColumns[TableColumnKeys.Discount].Rank, row, Misc.PercentToString (line.Discounts[0].DiscountRate.Value), this.FontSize);
				}

				if (line.Discounts[0].Value.HasValue)
				{
					table.SetText (this.tableColumns[TableColumnKeys.Discount].Rank, row, Misc.PriceToString (line.Discounts[0].Value.Value), this.FontSize);
				}
			}

			return 1;
		}

		private int BuildPriceLine(TableBand table, int row, SubTotalDocumentItemEntity line)
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
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, line.TextForResultingPrice, this.FontSize);
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
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row+0, text,                       this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row+1, line.TextForResultingPrice, this.FontSize);
			}

			//	Colonne "Prix HT":
			if (discount == null)
			{
				decimal v1 = line.ResultingPriceBeforeTax.GetValueOrDefault (0);
				string p1 = Misc.PriceToString (v1);

				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, row, p1, this.FontSize);
			}
			else
			{
				decimal v1 = line.PrimaryPriceBeforeTax.GetValueOrDefault (0);
				decimal v3 = line.ResultingPriceBeforeTax.GetValueOrDefault (0);

				string p1 = Misc.PriceToString (v1);
				string p2 = Misc.PriceToString (v3 - v1);
				string p3 = Misc.PriceToString (v3);

				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, row+0, FormattedText.Concat (p1, FormattedText.HtmlBreak, p2), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, row+1, p3,                                                     this.FontSize);
			}

			//	Colonne "TVA":
			if (discount == null)
			{
				decimal v1 = line.ResultingTax.GetValueOrDefault (0);
				string p1 = Misc.PriceToString (v1);

				table.SetText (this.tableColumns[TableColumnKeys.Vat].Rank, row, p1, this.FontSize);
			}
			else
			{
				decimal v1 = line.PrimaryTax.GetValueOrDefault (0);
				decimal v3 = line.ResultingTax.GetValueOrDefault (0);

				string p1 = Misc.PriceToString (v1);
				string p2 = Misc.PriceToString (v3 - v1);
				string p3 = Misc.PriceToString (v3);

				table.SetText (this.tableColumns[TableColumnKeys.Vat].Rank, row+0, FormattedText.Concat (p1, FormattedText.HtmlBreak, p2), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.Vat].Rank, row+1, p3,                                                     this.FontSize);
			}

			//	Colonne "Prix TTC":
			if (discount == null)
			{
				decimal v1 = line.ResultingPriceBeforeTax.GetValueOrDefault (0) + line.ResultingTax.GetValueOrDefault (0);
				string p1 = Misc.PriceToString (v1);

				table.SetText (this.tableColumns[TableColumnKeys.Total].Rank, row, p1, this.FontSize);
			}
			else
			{
				decimal v1 = line.PrimaryPriceBeforeTax.GetValueOrDefault (0) + line.PrimaryTax.GetValueOrDefault (0);
				decimal v3 = line.ResultingPriceBeforeTax.GetValueOrDefault (0) + line.ResultingTax.GetValueOrDefault (0);

				string p1 = Misc.PriceToString (v1);
				string p2 = Misc.PriceToString (v3 - v1);
				string p3 = Misc.PriceToString (v3);

				table.SetText (this.tableColumns[TableColumnKeys.Total].Rank, row+0, FormattedText.Concat (p1, FormattedText.HtmlBreak, p2), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.Total].Rank, row+1, p3,                                                     this.FontSize);
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

			table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, text,                                   this.FontSize);
			table.SetText (this.tableColumns[TableColumnKeys.LinePrice         ].Rank, row, Misc.PriceToString (line.ResultingTax), this.FontSize);

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

		private int BuildTotalLine(TableBand table, int row, EndTotalDocumentItemEntity line)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			if (this.IsDocumentWithoutPrice)
			{
				return 0;
			}

			if (line.PriceBeforeTax.HasValue)  // ligne de total HT ?
			{
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, line.TextForPrice,                        this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.LinePrice         ].Rank, row, Misc.PriceToString (line.PriceBeforeTax), this.FontSize);
			}
			else if (line.FixedPriceAfterTax.HasValue)
			{
				FormattedText text = FormattedText.Join (FormattedText.HtmlBreak, line.TextForPrice, line.TextForFixedPrice);
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, text, this.FontSize);

				FormattedText total = FormattedText.Concat ("<b>", Misc.PriceToString (line.PriceAfterTax), "</b><br/><b><i>", Misc.PriceToString (line.FixedPriceAfterTax), "</i></b>");
				table.SetText (this.tableColumns[TableColumnKeys.Total].Rank, row, total, this.FontSize);
			}
			else
			{
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, line.TextForPrice, this.FontSize);

				FormattedText total = FormattedText.Concat ("<b>", Misc.PriceToString (line.PriceAfterTax), "</b>");
				table.SetText (this.tableColumns[TableColumnKeys.Total].Rank, row, total, this.FontSize);
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
			if (!this.HasOption (DocumentOption.ArticleDelayed))  // n'imprime pas les articles retardés ?
			{
				foreach (var quantity in line.ArticleQuantities)
				{
					if (quantity.QuantityType == Business.ArticleQuantityType.Billed)
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

			FormattedText conditions = FormattedText.Join (FormattedText.HtmlBreak, billingDetails.Text, billingDetails.AmountDue.PaymentMode.Description);

			if (!conditions.IsNullOrEmpty)
			{
				var band = new TextBand ();
				band.Text = conditions;
				band.FontSize = this.FontSize;

				this.documentContainer.AddFromTop (band, 0);
			}
		}

		private void BuildFooter()
		{
			if (this.HasOption (DocumentOption.Signing))
			{
				if (this.DocumentType == Business.DocumentType.DeliveryNote)
				{
					var table = new TableBand ();

					table.ColumnsCount = 2;
					table.RowsCount = 1;
					table.CellBorder = CellBorder.Default;
					table.Font = font;
					table.FontSize = this.FontSize;
					table.CellMargins = new Margins (2);
					table.SetRelativeColumWidth (0, 60);
					table.SetRelativeColumWidth (1, 100);
					table.SetText (0, 0, new FormattedText ("Matériel reçu en bonne et due forme"), this.FontSize);
					table.SetText (1, 0, new FormattedText ("Reçu le :<br/><br/>Par :<br/><br/>Signature :<br/><br/><br/>"), this.FontSize);
					table.SetUnbreakableRow (0, true);

					this.documentContainer.AddToBottom (table, this.PageMargins.Bottom);
				}

				if (this.DocumentType == Business.DocumentType.ProductionOrder)
				{
					var table = new TableBand ();

					table.ColumnsCount = 2;
					table.RowsCount = 1;
					table.CellBorder = CellBorder.Default;
					table.Font = font;
					table.FontSize = this.FontSize;
					table.CellMargins = new Margins (2);
					table.SetRelativeColumWidth (0, 60);
					table.SetRelativeColumWidth (1, 100);
					table.SetText (0, 0, new FormattedText ("Matériel produit en totalité"), this.FontSize);
					table.SetText (1, 0, new FormattedText ("Terminé le :<br/><br/>Par :<br/><br/>Signature :<br/><br/><br/>"), this.FontSize);
					table.SetUnbreakableRow (0, true);

					this.documentContainer.AddToBottom (table, this.PageMargins.Bottom);
				}

				if (this.DocumentType == Business.DocumentType.OrderBooking)
				{
					var table = new TableBand ();

					table.ColumnsCount = 2;
					table.RowsCount = 1;
					table.CellBorder = CellBorder.Default;
					table.Font = font;
					table.FontSize = this.FontSize;
					table.CellMargins = new Margins (2);
					table.SetRelativeColumWidth (0, 60);
					table.SetRelativeColumWidth (1, 100);
					table.SetText (0, 0, new FormattedText ("Bon pour commande"), this.FontSize);
					table.SetText (1, 0, new FormattedText ("Lieu et date :<br/><br/>Signature :<br/><br/><br/>"), this.FontSize);
					table.SetUnbreakableRow (0, true);

					this.documentContainer.AddToBottom (table, this.PageMargins.Bottom);
				}
			}
		}

		private void BuildPages(BillingDetailEntity billingDetails, int firstPage)
		{
			//	Met les numéros de page.
			double reportHeight = this.IsDocumentWithoutPrice ? 0 : DocumentMetadataPrinter.reportHeight*2;

			var leftBounds  = new Rectangle (this.PageMargins.Left, this.RequiredPageSize.Height-this.PageMargins.Top+reportHeight+1, 80, 5);
			var rightBounds = new Rectangle (this.RequiredPageSize.Width-this.PageMargins.Right-80, this.RequiredPageSize.Height-this.PageMargins.Top+reportHeight+1, 80, 5);

			for (int page = firstPage+1; page < this.documentContainer.PageCount (); page++)
			{
				this.documentContainer.CurrentPage = page;

				var leftHeader = new TextBand ();
				leftHeader.Text = InvoiceDocumentHelper.GetTitle (this.Metadata, this.Entity, billingDetails);
				leftHeader.Alignment = ContentAlignment.BottomLeft;
				leftHeader.Font = font;
				leftHeader.FontSize = this.FontSize*1.3;

				var rightHeader = new TextBand ();
				rightHeader.Text = FormattedText.Concat ("page ", (page-firstPage+1).ToString ());
				rightHeader.Alignment = ContentAlignment.BottomRight;
				rightHeader.Font = font;
				rightHeader.FontSize = this.FontSize;

				this.documentContainer.AddAbsolute (leftHeader, leftBounds);
				this.documentContainer.AddAbsolute (rightHeader, rightBounds);
			}
		}

		private void BuildReportHeaders(int firstPage)
		{
			//	Met un report en haut des pages concernées, avec une répétition de la ligne
			//	d'en-tête (noms des colonnes).
			double width = this.RequiredPageSize.Width-this.PageMargins.Left-this.PageMargins.Right;

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
						table.SetText (column.Rank, 0, column.Title, this.FontSize);
					}
				}

				//	Génère une deuxième ligne avec les montants à reporter.
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, 1, "Report", this.FontSize);

				decimal sumPT, sumTva, sumTot;
				this.ComputeBottomReports (relativePage-1, out sumPT, out sumTva, out sumTot);
				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, 1, Misc.PriceToString (sumPT),  this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.Vat      ].Rank, 1, Misc.PriceToString (sumTva), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.Total    ].Rank, 1, Misc.PriceToString (sumTot), this.FontSize);

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
			double width = this.RequiredPageSize.Width-this.PageMargins.Left-this.PageMargins.Right;

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

				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, 0, "à reporter", this.FontSize);

				decimal sumPT, sumTva, sumTot;
				this.ComputeBottomReports (relativePage, out sumPT, out sumTva, out sumTot);
				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, 0, Misc.PriceToString (sumPT),  this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.Vat      ].Rank, 0, Misc.PriceToString (sumTva), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.Total    ].Rank, 0, Misc.PriceToString (sumTot), this.FontSize);

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

				AbstractDocumentItemEntity item = this.Entity.Lines[row-1];  // -1 à cause de l'en-tête

				if (item is ArticleDocumentItemEntity)
				{
					var article = item as ArticleDocumentItemEntity;

					decimal beforeTax = article.ResultingLinePriceBeforeTax.GetValueOrDefault (0);
					decimal tax1 =      article.ResultingLineTax1          .GetValueOrDefault (0);
					decimal tax2 =      article.ResultingLineTax2          .GetValueOrDefault (0);

					sumPT  += beforeTax;
					sumTva += tax1 + tax2;
					sumTot += beforeTax+tax1+tax2;
				}
			}
		}


		private void BuildEsrs(BillingDetailEntity billingDetails, int firstPage)
		{
			if (this.HasOption (DocumentOption.EsrPosition, "WithInside"))
			{
				this.BuildInsideEsrs (billingDetails, firstPage);
			}

			if (this.HasOption (DocumentOption.EsrPosition, "WithOutside"))
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
			var bounds = new Rectangle (Point.Zero, AbstractEsrBand.DefautlSize);

			if (this.documentContainer.PageCount () - firstPage > 1 ||
				this.documentContainer.CurrentVerticalPosition - DocumentMetadataPrinter.marginBeforeEsr < bounds.Top ||
				this.HasPrintingUnitDefined (PageType.Single) == false)
			{
				//	On ne prépare pas une nouvelle page si on peut mettre la facture
				//	et le BV sur une seule page !
				this.documentContainer.PrepareEmptyPage (PageType.Esr);
			}

			this.BuildEsr (billingDetails);
		}

		private void BuildEsr(BillingDetailEntity billingDetails, bool mackle=false)
		{
			//	Met un BVR orangé ou un BV rose au bas de la page courante.
			AbstractEsrBand isr;

			if (this.HasOption (DocumentOption.EsrType, "Esr"))
			{
				isr = new EsrBand ();  // BVR orangé
			}
			else
			{
				isr = new EsBand ();  // BV rose
			}

			isr.PaintEsrSimulator = this.HasOption (DocumentOption.EsrFacsimile);
			isr.From = this.Entity.BillToMailContact.GetSummary ();
			isr.To = billingDetails.IsrDefinition.SubscriberAddress;
			isr.Communication = InvoiceDocumentHelper.GetTitle (this.Metadata, this.Entity, billingDetails);

			isr.Slip = new IsrSlip (billingDetails);
			isr.NotForUse = mackle;  // pour imprimer "XXXXX XX" sur un faux BVR

			var bounds = new Rectangle (Point.Zero, AbstractEsrBand.DefautlSize);
			this.documentContainer.AddAbsolute (isr, bounds);
		}


		private bool IsDocumentWithoutPrice
		{
			get
			{
				return this.DocumentType == Business.DocumentType.DeliveryNote ||
					   this.DocumentType == Business.DocumentType.ProductionOrder;
			}
		}

		private bool IsColumnsOrderQD
		{
			get
			{
				return this.HasOption (DocumentOption.ColumnsOrder, "QD");
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
				return this.HasOption (DocumentOption.LayoutFrame, "WithLine");
			}
		}

		private bool IsWithFrame
		{
			get
			{
				return this.HasOption (DocumentOption.LayoutFrame, "WithFrame");
			}
		}


		private DocumentType DocumentType
		{
			get
			{
				if (this.Metadata.DocumentCategory == null)
				{
					return Business.DocumentType.None;
				}
				else
				{
#if true
					if (this.Metadata.DocumentCategory.DocumentType == Business.DocumentType.None)
					{
						return Business.DocumentType.Invoice;  // TODO: Hask à supprimer dès que possible !
					}
#endif

					return this.Metadata.DocumentCategory.DocumentType;
				}
			}
		}

		private BusinessDocumentEntity Entity
		{
			get
			{
				var metadata = this.Metadata;

				if (metadata.IsNull ())
				{
					return null;
				}
				else
				{
					return metadata.BusinessDocument as BusinessDocumentEntity;
				}
			}
		}

		private DocumentMetadataEntity Metadata
		{
			get
			{
				return this.entity as DocumentMetadataEntity;
			}
		}


		#region Factory Class

		private class Factory : IEntityPrinterFactory
		{
			#region IEntityPrinterFactory Members

			public bool CanPrint(AbstractEntity entity, OptionsDictionary options)
			{
				return entity is DocumentMetadataEntity;
			}

			public AbstractPrinter CreatePrinter(IBusinessContext businessContext, AbstractEntity entity, OptionsDictionary options, PrintingUnitsDictionary printingUnits)
			{
				return new DocumentMetadataPrinter (businessContext, entity, options, printingUnits);
			}

			#endregion
		}

		#endregion


		private static readonly Font		font = Font.GetFont ("Arial", "Regular");
		private static readonly double		reportHeight = 7.0;
		private static readonly double		marginBeforeEsr = 10;

		private TableBand					table;
		private int							visibleColumnCount;
		private int[]						lastRowForEachSection;
		private List<Rectangle>				tableBounds;
	}
}
