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
using Epsitec.Cresus.Core.Library.Business.ContentAccessors;
using Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.EntityPrinters
{
	public class InvoiceDocumentMetadataPrinter : AbstractDocumentMetadataPrinter
	{
		public InvoiceDocumentMetadataPrinter(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
			: base (businessContext, entity, options, printingUnits)
		{
		}


		protected override Margins PageMargins
		{
			get
			{
				double leftMargin   = this.GetOptionValue (DocumentOption.LeftMargin, 20);
				double rightMargin  = this.GetOptionValue (DocumentOption.RightMargin, 20);
				double topMargin    = this.GetOptionValue (DocumentOption.TopMargin, 20);
				double bottomMargin = this.GetOptionValue (DocumentOption.BottomMargin, 20);

				double h = AbstractDocumentMetadataPrinter.reportHeight;

				if (this.HasIsr && this.HasOption (DocumentOption.IsrPosition, "WithInside"))
				{
					return new Margins (leftMargin, rightMargin, topMargin+h*2, h+InvoiceDocumentMetadataPrinter.marginBeforeIsr+AbstractIsrBand.DefautlSize.Height);
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

			if (!this.HasIsr || this.HasOption (DocumentOption.IsrPosition, "Without") || this.PreviewMode == Print.PreviewMode.ContinuousPreview)
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
					this.BuildIsrs (billingDetails, firstPage);

					this.documentContainer.Ending (firstPage);
					onlyTotal = true;
				}
			}
		}


		protected override void InitializeColumns()
		{
			this.tableColumns.Clear ();

			double priceWidth = 13 + this.CellMargin*2;  // largeur standard pour un montant ou une quantité

			if (this.IsColumnsOrderQD)
			{
				this.tableColumns.Add (TableColumnKeys.LineNumber,                new TableColumn ("N°",          priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.Quantity,                  new TableColumn ("Nb",          priceWidth,   ContentAlignment.MiddleLeft));

				this.tableColumns.Add (TableColumnKeys.OrderedQuantity,           new TableColumn ("Comm",        priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.OrderedDate,               new TableColumn ("Date",        priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.BilledQuantity,            new TableColumn ("Fact",        priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.BilledDate,                new TableColumn ("Date",        priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.DelayedQuantity,           new TableColumn ("Suit",        priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.DelayedDate,               new TableColumn ("Date",        priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ExpectedQuantity,          new TableColumn ("Att",         priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ExpectedDate,              new TableColumn ("Date",        priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ShippedQuantity,           new TableColumn ("Livré",       priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ShippedDate,               new TableColumn ("Date",        priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ShippedPreviouslyQuantity, new TableColumn ("Déjà",        priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ShippedPreviouslyDate,     new TableColumn ("Date",        priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.InformationQuantity,       new TableColumn ("Info",        priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.InformationDate,           new TableColumn ("Date",        priceWidth+3, ContentAlignment.MiddleLeft));

				this.tableColumns.Add (TableColumnKeys.ArticleId,                 new TableColumn ("Article",     priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleDescription,        new TableColumn ("Désignation", 0,            ContentAlignment.MiddleLeft));  // seule colonne en mode width = fill

				this.tableColumns.Add (TableColumnKeys.UnitPrice,                 new TableColumn ("p.u. HT",     priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Discount,                  new TableColumn ("Rabais",      priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.LinePrice,                 new TableColumn ("Prix HT",     priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Vat,                       new TableColumn ("TVA",         priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Total,                     new TableColumn ("Prix TTC",    priceWidth,   ContentAlignment.MiddleRight));
			}
			else
			{
				this.tableColumns.Add (TableColumnKeys.LineNumber,                new TableColumn ("N°",          priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleId,                 new TableColumn ("Article",     priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleDescription,        new TableColumn ("Désignation", 0,            ContentAlignment.MiddleLeft));  // seule colonne en mode width = fill
				this.tableColumns.Add (TableColumnKeys.Quantity,                  new TableColumn ("Nb",          priceWidth,   ContentAlignment.MiddleLeft));

				this.tableColumns.Add (TableColumnKeys.OrderedQuantity,           new TableColumn ("Comm",        priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.OrderedDate,               new TableColumn ("Date",        priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.BilledQuantity,            new TableColumn ("Fact",        priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.BilledDate,                new TableColumn ("Date",        priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.DelayedQuantity,           new TableColumn ("Suit",        priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.DelayedDate,               new TableColumn ("Date",        priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ExpectedQuantity,          new TableColumn ("Att",         priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ExpectedDate,              new TableColumn ("Date",        priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ShippedQuantity,           new TableColumn ("Livré",       priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ShippedDate,               new TableColumn ("Date",        priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ShippedPreviouslyQuantity, new TableColumn ("Déjà",        priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ShippedPreviouslyDate,     new TableColumn ("Date",        priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.InformationQuantity,       new TableColumn ("Info",        priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.InformationDate,           new TableColumn ("Date",        priceWidth+3, ContentAlignment.MiddleLeft));

				this.tableColumns.Add (TableColumnKeys.UnitPrice,                 new TableColumn ("p.u. HT",     priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Discount,                  new TableColumn ("Rabais",      priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.LinePrice,                 new TableColumn ("Prix HT",     priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Vat,                       new TableColumn ("TVA",         priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Total,                     new TableColumn ("Prix TTC",    priceWidth,   ContentAlignment.MiddleRight));
			}
		}

		protected override DocumentItemAccessorMode DocumentItemAccessorMode
		{
			get
			{
				return DocumentItemAccessorMode.ForceAllLines |
					   DocumentItemAccessorMode.DescriptionIndented |
					   DocumentItemAccessorMode.UseArticleName;  // le nom court suffit sur une facture
#if false
			var mode = DocumentItemAccessorMode.ForceAllLines |
					   DocumentItemAccessorMode.DescriptionIndented;

			if (this.DocumentType == Business.DocumentType.ProductionOrder   ||
				this.DocumentType == Business.DocumentType.DeliveryNote      ||
				this.DocumentType == Business.DocumentType.ShipmentChecklist ||
				this.DocumentType == Business.DocumentType.Invoice           ||
				this.DocumentType == Business.DocumentType.InvoiceProForma   )
			{
				//	Les ordres de productions doivent utiliser les descriptions courtes des articles.
				//	C'est une demande de Monsieur "M" !
				mode |= DocumentItemAccessorMode.UseArticleName;
			}
			else
			{
				mode |= DocumentItemAccessorMode.UseArticleBoth;
			}

			if (this.DocumentType == Business.DocumentType.ProductionOrder     ||
				this.DocumentType == Business.DocumentType.ProductionChecklist ||
				this.DocumentType == Business.DocumentType.ShipmentChecklist   )
			{
				//	Seuls les documents à usage interne incluent les lignes 'MyEyesOnly'.
				mode |= DocumentItemAccessorMode.ShowMyEyesOnly;
			}
#endif
			}
		}

		protected override int InitializeLine(DocumentItemAccessor accessor, AbstractDocumentItemEntity line, ArticleGroupEntity group)
		{
			if (line is TextDocumentItemEntity)
			{
				return this.InitializeTextLine (accessor, line as TextDocumentItemEntity);
			}

			if (line is ArticleDocumentItemEntity)
			{
				return this.InitializeArticleLine (accessor, line as ArticleDocumentItemEntity, group);
			}

			if (line is SubTotalDocumentItemEntity)
			{
				return this.InitializeSubTotalLine (accessor, line as SubTotalDocumentItemEntity);
			}

			if (line is TaxDocumentItemEntity)
			{
				return this.InitializeTaxLine (accessor, line as TaxDocumentItemEntity);
			}

			if (line is EndTotalDocumentItemEntity)
			{
				return this.InitializeEndTotalLine (accessor, line as EndTotalDocumentItemEntity);
			}

			return 0;
		}

		private int InitializeTextLine(DocumentItemAccessor accessor, TextDocumentItemEntity line)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			this.tableColumns[TableColumnKeys.ArticleDescription].Visible = true;

			return accessor.RowsCount;
		}

		private int InitializeArticleLine(DocumentItemAccessor accessor, ArticleDocumentItemEntity line, ArticleGroupEntity group)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			this.tableColumns[TableColumnKeys.ArticleId].Visible = true;
			this.tableColumns[TableColumnKeys.ArticleDescription].Visible = true;
			this.tableColumns[TableColumnKeys.Total].Visible = true;

			if (line.VatCode != Business.Finance.VatCode.None &&
				line.VatCode != Business.Finance.VatCode.Excluded &&
				line.VatCode != Business.Finance.VatCode.ZeroRated)
			{
				this.tableColumns[TableColumnKeys.Vat].Visible = true;
			}

			foreach (var quantity in line.ArticleQuantities)
			{
				this.tableColumns[TableColumnKeys.Quantity].Visible = true;
				this.tableColumns[TableColumnKeys.UnitPrice].Visible = true;
				this.tableColumns[TableColumnKeys.LinePrice].Visible = true;

				if (quantity.QuantityColumn.QuantityType == Business.ArticleQuantityType.Ordered)
				{
					this.tableColumns[TableColumnKeys.OrderedQuantity].Visible = true;
					this.tableColumns[TableColumnKeys.OrderedDate].Visible = true;
				}

				if (quantity.QuantityColumn.QuantityType == Business.ArticleQuantityType.Billed)
				{
					this.tableColumns[TableColumnKeys.BilledQuantity].Visible = true;
					this.tableColumns[TableColumnKeys.BilledDate].Visible = true;
				}

				if (quantity.QuantityColumn.QuantityType == Business.ArticleQuantityType.Delayed)
				{
					this.tableColumns[TableColumnKeys.DelayedQuantity].Visible = true;
					this.tableColumns[TableColumnKeys.DelayedDate].Visible = true;
				}

				if (quantity.QuantityColumn.QuantityType == Business.ArticleQuantityType.Expected)
				{
					this.tableColumns[TableColumnKeys.ExpectedQuantity].Visible = true;
					this.tableColumns[TableColumnKeys.ExpectedDate].Visible = true;
				}

				if (quantity.QuantityColumn.QuantityType == Business.ArticleQuantityType.Shipped)
				{
					this.tableColumns[TableColumnKeys.ShippedQuantity].Visible = true;
					this.tableColumns[TableColumnKeys.ShippedDate].Visible = true;
				}

				if (quantity.QuantityColumn.QuantityType == Business.ArticleQuantityType.ShippedPreviously)
				{
					this.tableColumns[TableColumnKeys.ShippedPreviouslyQuantity].Visible = true;
					this.tableColumns[TableColumnKeys.ShippedPreviouslyDate].Visible = true;
				}

				if (quantity.QuantityColumn.QuantityType == Business.ArticleQuantityType.Information)
				{
					this.tableColumns[TableColumnKeys.InformationQuantity].Visible = true;
					//?this.tableColumns[TableColumnKeys.InformationDate].Visible = true;
				}
			}

			if (line.Discounts.Count != 0)
			{
				this.tableColumns[TableColumnKeys.Discount].Visible = true;
			}

			return accessor.RowsCount;
		}

		private int InitializeSubTotalLine(DocumentItemAccessor accessor, SubTotalDocumentItemEntity line)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			this.tableColumns[TableColumnKeys.ArticleDescription].Visible = true;
			this.tableColumns[TableColumnKeys.LinePrice].Visible = true;
			this.tableColumns[TableColumnKeys.Vat].Visible = true;
			this.tableColumns[TableColumnKeys.Total].Visible = true;

			return accessor.RowsCount;
		}

		private int InitializeTaxLine(DocumentItemAccessor accessor, TaxDocumentItemEntity line)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			this.tableColumns[TableColumnKeys.ArticleDescription].Visible = true;
			this.tableColumns[TableColumnKeys.LinePrice].Visible = true;
			this.tableColumns[TableColumnKeys.Vat].Visible = true;
			this.tableColumns[TableColumnKeys.Total].Visible = true;

			return accessor.RowsCount;
		}

		private int InitializeEndTotalLine(DocumentItemAccessor accessor, EndTotalDocumentItemEntity line)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			this.tableColumns[TableColumnKeys.ArticleDescription].Visible = true;

			if (line.PriceBeforeTax.HasValue)  // ligne de total HT ?
			{
				this.tableColumns[TableColumnKeys.LinePrice].Visible = true;
			}
			else  // ligne de total TTC ?
			{
				this.tableColumns[TableColumnKeys.Total].Visible = true;
			}

			return accessor.RowsCount;
		}


		protected override int BuildLine(TableBand table, int row, DocumentItemAccessor accessor, AbstractDocumentItemEntity prevLine, AbstractDocumentItemEntity line, AbstractDocumentItemEntity nextLine, ArticleGroupEntity group)
		{
			if (line is TextDocumentItemEntity)
			{
				return this.BuildTextLine (table, row, accessor, line as TextDocumentItemEntity);
			}

			if (line is ArticleDocumentItemEntity)
			{
				return this.BuildArticleLine (table, row, accessor, line as ArticleDocumentItemEntity, group);
			}

			if (line is SubTotalDocumentItemEntity)
			{
				return this.BuildSubTotalLine (table, row, accessor, line as SubTotalDocumentItemEntity);
			}

			if (line is TaxDocumentItemEntity)
			{
				bool firstTax = (prevLine is TaxDocumentItemEntity);
				bool lastTax  = (nextLine is TaxDocumentItemEntity);

				return this.BuildTaxLine (table, row, accessor, line as TaxDocumentItemEntity, firstTax, lastTax);
			}

			if (line is EndTotalDocumentItemEntity)
			{
				return this.BuildEndTotalLine (table, row, accessor, line as EndTotalDocumentItemEntity);
			}

			return 0;
		}

		private int BuildTextLine(TableBand table, int row, DocumentItemAccessor accessor, TextDocumentItemEntity line)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			var text = accessor.GetContent (0, DocumentItemAccessorColumn.ArticleDescription).ApplyBold ();
			table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, text, this.FontSize);

			return accessor.RowsCount;
		}

		private int BuildArticleLine(TableBand table, int row, DocumentItemAccessor accessor, ArticleDocumentItemEntity line, ArticleGroupEntity group)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			for (int i = 0; i < accessor.RowsCount; i++)
			{
				table.SetText (this.tableColumns[TableColumnKeys.Quantity].Rank, row+i, AbstractDocumentMetadataPrinter.GetQuantityAndUnit (accessor, i, DocumentItemAccessorColumn.BilledQuantity, DocumentItemAccessorColumn.BilledUnit), this.FontSize);

				table.SetText (this.tableColumns[TableColumnKeys.OrderedQuantity].Rank, row+i, AbstractDocumentMetadataPrinter.GetQuantityAndUnit (accessor, i, DocumentItemAccessorColumn.OrderedQuantity, DocumentItemAccessorColumn.OrderedUnit), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.OrderedDate].Rank, row+i, AbstractDocumentMetadataPrinter.GetDates (accessor, i, DocumentItemAccessorColumn.OrderedBeginDate, DocumentItemAccessorColumn.OrderedEndDate), this.FontSize);

				table.SetText (this.tableColumns[TableColumnKeys.BilledQuantity].Rank, row+i, AbstractDocumentMetadataPrinter.GetQuantityAndUnit (accessor, i, DocumentItemAccessorColumn.BilledQuantity, DocumentItemAccessorColumn.BilledUnit), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.BilledDate].Rank, row+i, AbstractDocumentMetadataPrinter.GetDates (accessor, i, DocumentItemAccessorColumn.BilledBeginDate, DocumentItemAccessorColumn.BilledEndDate), this.FontSize);

				table.SetText (this.tableColumns[TableColumnKeys.DelayedQuantity].Rank, row+i, AbstractDocumentMetadataPrinter.GetQuantityAndUnit (accessor, i, DocumentItemAccessorColumn.DelayedQuantity, DocumentItemAccessorColumn.DelayedUnit), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.DelayedDate].Rank, row+i, AbstractDocumentMetadataPrinter.GetDates (accessor, i, DocumentItemAccessorColumn.DelayedBeginDate, DocumentItemAccessorColumn.DelayedEndDate), this.FontSize);

				table.SetText (this.tableColumns[TableColumnKeys.ExpectedQuantity].Rank, row+i, AbstractDocumentMetadataPrinter.GetQuantityAndUnit (accessor, i, DocumentItemAccessorColumn.ExpectedQuantity, DocumentItemAccessorColumn.ExpectedUnit), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.ExpectedDate].Rank, row+i, AbstractDocumentMetadataPrinter.GetDates (accessor, i, DocumentItemAccessorColumn.ExpectedBeginDate, DocumentItemAccessorColumn.ExpectedEndDate), this.FontSize);

				table.SetText (this.tableColumns[TableColumnKeys.ShippedQuantity].Rank, row+i, AbstractDocumentMetadataPrinter.GetQuantityAndUnit (accessor, i, DocumentItemAccessorColumn.ShippedQuantity, DocumentItemAccessorColumn.ShippedUnit), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.ShippedDate].Rank, row+i, AbstractDocumentMetadataPrinter.GetDates (accessor, i, DocumentItemAccessorColumn.ShippedBeginDate, DocumentItemAccessorColumn.ShippedEndDate), this.FontSize);

				table.SetText (this.tableColumns[TableColumnKeys.ShippedPreviouslyQuantity].Rank, row+i, AbstractDocumentMetadataPrinter.GetQuantityAndUnit (accessor, i, DocumentItemAccessorColumn.ShippedPreviouslyQuantity, DocumentItemAccessorColumn.ShippedPreviouslyUnit), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.ShippedPreviouslyDate].Rank, row+i, AbstractDocumentMetadataPrinter.GetDates (accessor, i, DocumentItemAccessorColumn.ShippedPreviouslyBeginDate, DocumentItemAccessorColumn.ShippedPreviouslyEndDate), this.FontSize);

				table.SetText (this.tableColumns[TableColumnKeys.InformationQuantity].Rank, row+i, AbstractDocumentMetadataPrinter.GetQuantityAndUnit (accessor, i, DocumentItemAccessorColumn.InformationQuantity, DocumentItemAccessorColumn.InformationUnit), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.InformationDate].Rank, row+i, AbstractDocumentMetadataPrinter.GetDates (accessor, i, DocumentItemAccessorColumn.InformationBeginDate, DocumentItemAccessorColumn.InformationEndDate), this.FontSize);
			}

			table.SetText (this.tableColumns[TableColumnKeys.ArticleId].Rank, row, accessor.GetContent (0, DocumentItemAccessorColumn.ArticleId), this.FontSize);
			table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, accessor.GetContent (0, DocumentItemAccessorColumn.ArticleDescription), this.FontSize);
			table.SetText (this.tableColumns[TableColumnKeys.UnitPrice].Rank, row, accessor.GetContent (0, DocumentItemAccessorColumn.UnitPrice), this.FontSize);

			if (line.ResultingLinePriceBeforeTax.HasValue && line.ResultingLineTax1.HasValue)
			{
				decimal beforeTax = line.ResultingLinePriceBeforeTax.Value;
				decimal tax =       line.ResultingLineTax1.Value;

				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, row, accessor.GetContent (0, DocumentItemAccessorColumn.LinePrice), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.Vat].Rank, row, accessor.GetContent (0, DocumentItemAccessorColumn.Vat), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.Total].Rank, row, accessor.GetContent (0, DocumentItemAccessorColumn.Total), this.FontSize);
			}

			if (line.Discounts.Count != 0)
			{
				if (line.Discounts[0].DiscountRate.HasValue || line.Discounts[0].Value.HasValue)
				{
					table.SetText (this.tableColumns[TableColumnKeys.Discount].Rank, row, accessor.GetContent (0, DocumentItemAccessorColumn.Discount), this.FontSize);
				}
			}

			this.TableMakeBlock (table, row, accessor.RowsCount);

			return accessor.RowsCount;
		}

		private int BuildSubTotalLine(TableBand table, int row, DocumentItemAccessor accessor, SubTotalDocumentItemEntity line)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			//  Une ligne de sous-total PriceDocumentItemEntity peut occuper 2 lignes physiques du tableau,
			//	lorsqu'il y a un rabais. Cela permet de créer un demi-espace vertical entre les lignes
			//	'Sous-total avant rabais / Rabais' et 'Sous-total après rabais'.
			for (int i = 0; i < accessor.RowsCount; i++)
			{
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row+i, accessor.GetContent (i, DocumentItemAccessorColumn.ArticleDescription), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, row+i, accessor.GetContent (i, DocumentItemAccessorColumn.LinePrice), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.Vat].Rank, row+i, accessor.GetContent (i, DocumentItemAccessorColumn.Vat), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.Total].Rank, row+i, accessor.GetContent (i, DocumentItemAccessorColumn.Total), this.FontSize);
			}

			this.TableMakeBlock (table, row, accessor.RowsCount);

			return accessor.RowsCount;
		}

		private int BuildTaxLine(TableBand table, int row, DocumentItemAccessor accessor, TaxDocumentItemEntity line, bool firstTax, bool lastTax)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row, accessor.GetContent (0, DocumentItemAccessorColumn.ArticleDescription), this.FontSize);
			table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, row, accessor.GetContent (0, DocumentItemAccessorColumn.LinePrice), this.FontSize);

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

			return accessor.RowsCount;
		}

		private int BuildEndTotalLine(TableBand table, int row, DocumentItemAccessor accessor, EndTotalDocumentItemEntity line)
		{
			//	Retourne le nombre de lignes à utiliser dans le tableau.
			for (int i = 0; i < accessor.RowsCount; i++)
			{
				var total = accessor.GetContent (i, DocumentItemAccessorColumn.Total);

				if (i == accessor.RowsCount-1 && total != null)  // dernière ligne ?
				{
					total = total.ApplyBold ();
				}

				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, row+i, accessor.GetContent (i, DocumentItemAccessorColumn.ArticleDescription), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, row+i, accessor.GetContent (i, DocumentItemAccessorColumn.LinePrice), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.Vat].Rank, row+i, accessor.GetContent (i, DocumentItemAccessorColumn.Vat), this.FontSize);
				table.SetText (this.tableColumns[TableColumnKeys.Total].Rank, row+i, total, this.FontSize);
			}

			this.TableMakeBlock (table, row, accessor.RowsCount);

			if (this.IsWithFrame)
			{
				table.SetCellBorder (row, this.GetCellBorder (topLess: true));
				table.SetCellBorder (this.tableColumns[TableColumnKeys.Total].Rank, row, new CellBorder (CellBorder.BoldWidth));
			}
			else
			{
				table.SetCellBorder (row, this.GetCellBorder (bottomBold: true, topLess: true));
			}

			return accessor.RowsCount;
		}


		private void BuildConditions(BillingDetailEntity billingDetails)
		{
			//	Met les conditions à la fin de la facture.
			FormattedText conditions = FormattedText.Join (FormattedText.HtmlBreak, billingDetails.Text, billingDetails.AmountDue.PaymentMode.Description);

			if (!conditions.IsNullOrEmpty)
			{
				var band = new TextBand ();
				band.Text = conditions;
				band.FontSize = this.FontSize;

				this.documentContainer.AddFromTop (band, 0);
			}
		}


		private void BuildIsrs(BillingDetailEntity billingDetails, int firstPage)
		{
			if (this.HasOption (DocumentOption.IsrPosition, "WithInside"))
			{
				this.BuildInsideIsrs (billingDetails, firstPage);
			}

			if (this.HasOption (DocumentOption.IsrPosition, "WithOutside"))
			{
				this.BuildOutsideIsr (billingDetails, firstPage);
			}
		}

		private void BuildInsideIsrs(BillingDetailEntity billingDetails, int firstPage)
		{
			//	Met un BVR orangé ou un BV rose en bas de chaque page.
			for (int page = firstPage; page < this.documentContainer.PageCount (); page++)
			{
				this.documentContainer.CurrentPage = page;

				this.BuildIsr (billingDetails, mackle: page != this.documentContainer.PageCount ()-1);
			}
		}

		private void BuildOutsideIsr(BillingDetailEntity billingDetails, int firstPage)
		{
			//	Met un BVR orangé ou un BV rose sur une dernière page séparée.
			var bounds = new Rectangle (Point.Zero, AbstractIsrBand.DefautlSize);

			if (this.documentContainer.PageCount () - firstPage > 1 ||
				this.documentContainer.CurrentVerticalPosition - InvoiceDocumentMetadataPrinter.marginBeforeIsr < bounds.Top ||
				this.HasPrintingUnitDefined (PageType.Single) == false)
			{
				//	On ne prépare pas une nouvelle page si on peut mettre la facture
				//	et le BV sur une seule page !
				this.documentContainer.PrepareEmptyPage (PageType.Isr);
			}

			this.BuildIsr (billingDetails);
		}

		private void BuildIsr(BillingDetailEntity billingDetails, bool mackle=false)
		{
			//	Met un BVR orangé ou un BV rose au bas de la page courante.
			AbstractIsrBand isr;

			if (this.HasOption (DocumentOption.IsrType, "Isr"))
			{
				isr = new IsrBand ();  // BVR orangé
			}
			else
			{
				isr = new IsBand ();  // BV rose
			}

			isr.PaintIsrSimulator = this.HasOption (DocumentOption.IsrFacsimile);
			isr.From = this.Entity.BillToMailContact.GetSummary ();
			isr.To = billingDetails.IsrDefinition.SubscriberAddress;
			isr.Communication = InvoiceDocumentHelper.GetTitle (this.Metadata, this.Entity, billingDetails);

			isr.Slip = new IsrSlip (billingDetails);
			isr.NotForUse = mackle;  // pour imprimer "XXXXX XX" sur un faux BVR

			var bounds = new Rectangle (Point.Zero, AbstractIsrBand.DefautlSize);
			this.documentContainer.AddAbsolute (isr, bounds);
		}


		private bool HasIsr
		{
			//	Indique s'il faut imprimer le BV. Pour cela, il faut que l'unité d'impression soit définie pour le type PageType.Isr.
			//	En mode DocumentOption.IsrPosition = "WithOutside", cela évite d'imprimer à double un BV sur l'imprimante 'Blanc'.
			get
			{
				if (this.HasOption (DocumentOption.IsrPosition, "WithInside"))
				{
					return true;
				}

				if (this.HasOption (DocumentOption.IsrPosition, "WithOutside"))
				{
					if (this.currentPrintingUnit != null)
					{
						var example = new DocumentPrintingUnitsEntity ();
						example.Code = this.currentPrintingUnit.DocumentPrintingUnitCode;

						var documentPrintingUnits = this.businessContext.DataContext.GetByExample<DocumentPrintingUnitsEntity> (example).FirstOrDefault ();

						if (documentPrintingUnits != null)
						{
							var pageTypes = documentPrintingUnits.GetPageTypes ();

							return pageTypes.Contains (PageType.Isr);
						}
					}
				}

				return false;
			}
		}


		private static readonly double		marginBeforeIsr = 10;
	}
}
