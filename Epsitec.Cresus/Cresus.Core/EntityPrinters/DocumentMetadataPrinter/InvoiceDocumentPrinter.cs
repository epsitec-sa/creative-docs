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
	public class InvoiceDocumentPrinter : AbstractDocumentMetadataPrinter
	{
		public InvoiceDocumentPrinter(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
			: base (businessContext, entity, options, printingUnits)
		{
		}


		public static IEnumerable<DocumentOption> RequiredDocumentOptions
		{
			get
			{
				yield return DocumentOption.Orientation;
				yield return DocumentOption.HeaderLogo;
				yield return DocumentOption.HeaderLogoLeft;
				yield return DocumentOption.HeaderLogoTop;
				yield return DocumentOption.HeaderLogoWidth;
				yield return DocumentOption.HeaderLogoHeight;
				yield return DocumentOption.HeaderFromLeft;
				yield return DocumentOption.HeaderFromTop;
				yield return DocumentOption.HeaderFromWidth;
				yield return DocumentOption.HeaderFromHeight;
				yield return DocumentOption.HeaderForLeft;
				yield return DocumentOption.HeaderForTop;
				yield return DocumentOption.HeaderForWidth;
				yield return DocumentOption.HeaderForHeight;
				yield return DocumentOption.HeaderNumberLeft;
				yield return DocumentOption.HeaderNumberTop;
				yield return DocumentOption.HeaderNumberWidth;
				yield return DocumentOption.HeaderNumberHeight;
				yield return DocumentOption.HeaderToLeft;
				yield return DocumentOption.HeaderToTop;
				yield return DocumentOption.HeaderToWidth;
				yield return DocumentOption.HeaderToHeight;
				yield return DocumentOption.HeaderLocDateLeft;
				yield return DocumentOption.HeaderLocDateTop;
				yield return DocumentOption.HeaderLocDateWidth;
				yield return DocumentOption.HeaderLocDateHeight;
				yield return DocumentOption.TableTopAfterHeader;

				yield return DocumentOption.Specimen;
				yield return DocumentOption.FontSize;

				yield return DocumentOption.LeftMargin;
				yield return DocumentOption.RightMargin;
				yield return DocumentOption.TopMargin;
				yield return DocumentOption.BottomMargin;

				yield return DocumentOption.LayoutFrame;
				yield return DocumentOption.GapBeforeGroup;
				yield return DocumentOption.IndentWidth;

				yield return DocumentOption.LineNumber;
				yield return DocumentOption.ArticleAdditionalQuantities;
				yield return DocumentOption.ArticleId;
				yield return DocumentOption.ColumnsOrder;

				yield return DocumentOption.IsrPosition;
				yield return DocumentOption.IsrType;
				yield return DocumentOption.IsrFacsimile;
			}
		}

		public static IEnumerable<PageType> RequiredPageTypes
		{
			get
			{
				yield return PageType.Single;
				yield return PageType.First;
				yield return PageType.Following;

				yield return PageType.Isr;
			}
		}


		protected override Margins PageMargins
		{
			get
			{
				double leftMargin   = this.GetOptionValue (DocumentOption.LeftMargin);
				double rightMargin  = this.GetOptionValue (DocumentOption.RightMargin);
				double topMargin    = this.GetOptionValue (DocumentOption.TopMargin);
				double bottomMargin = this.GetOptionValue (DocumentOption.BottomMargin);

				double h = AbstractDocumentMetadataPrinter.reportHeight;

				if (this.HasIsr && this.HasOption (DocumentOption.IsrPosition, "WithInside"))
				{
					return new Margins (leftMargin, rightMargin, topMargin+h*2, h+InvoiceDocumentPrinter.marginBeforeIsr+AbstractIsrBand.DefautlSize.Height);
				}
				else
				{
					return new Margins (leftMargin, rightMargin, topMargin+h*2, h+bottomMargin);
				}
			}
		}


		public override FormattedText BuildSections()
		{
			base.BuildSections ();

			if (this.Entity.PaymentTransactions.Count == 0)
			{
				return "Il n'y a rien à imprimer, car la facture ne contient aucune donnée de facturation.";
			}

			this.onlyTotal = false;

			if (!this.HasIsr || this.HasOption (DocumentOption.IsrPosition, "Without") || this.PreviewMode == Print.PreviewMode.ContinuousPreview)
			{
				if (this.Entity.PaymentTransactions.Count != 0)
				{
					this.paymentTransactionEntity = this.Entity.PaymentTransactions[0];
					int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

					this.BuildHeader ();
					this.BuildArticles ();
					this.BuildConditions ();
					this.BuildPages (firstPage);
					this.BuildReportHeaders (firstPage);
					this.BuildReportFooters (firstPage);

					this.documentContainer.Ending (firstPage);
				}
			}
			else
			{
				int documentRank = 0;
				foreach (var billingDetails in this.Entity.PaymentTransactions)
				{
					this.paymentTransactionEntity = billingDetails;
					this.documentContainer.DocumentRank = documentRank++;
					int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

					this.BuildHeader ();
					this.BuildArticles ();
					this.BuildConditions ();
					this.BuildPages (firstPage);
					this.BuildReportHeaders (firstPage);
					this.BuildReportFooters (firstPage);
					this.BuildIsrs (firstPage);

					this.documentContainer.Ending (firstPage);
					this.onlyTotal = true;
				}
			}

			return null;  // ok
		}


		protected override bool HasPrices
		{
			get
			{
				return true;
			}
		}

		protected override FormattedText Title
		{
			get
			{
				return InvoiceDocumentHelper.GetTitle (this.Metadata, this.Entity, this.paymentTransactionEntity);
			}
		}

		protected override IEnumerable<ContentLine> ContentLines
		{
			get
			{
				if (this.onlyTotal)
				{
					//	Ne donne que la dernière ligne qui est celle du grand total.
					var totalLine = this.Entity.Lines[this.Entity.Lines.Count-1];
					yield return new ContentLine (totalLine);
				}
				else
				{
					//	Donne normalement toutes les lignes.
					foreach (var line in this.Entity.ConciseLines)
					{
						yield return new ContentLine (line);
					}
				}
			}
		}

		protected override void InitializeColumns()
		{
			this.tableColumns.Clear ();

			double priceWidth = this.PriceWidth;

			if (this.IsColumnsOrderQD)
			{
				this.tableColumns.Add (TableColumnKeys.LineNumber,         new TableColumn ("N°",               priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.MainQuantity,       new TableColumn ("Facturé",          priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.AdditionalType,     new TableColumn ("Autres quantités", priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.AdditionalQuantity, new TableColumn ("",                 priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.AdditionalDate,     new TableColumn ("",                 priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleId,          new TableColumn ("Article",          priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleDescription, new TableColumn ("Désignation",      0,            ContentAlignment.MiddleLeft));  // seule colonne en mode width = fill
																										        
				this.tableColumns.Add (TableColumnKeys.UnitPrice,          new TableColumn ("p.u. HT",          priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Discount,           new TableColumn ("Rabais",           priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.LinePrice,          new TableColumn ("Prix HT",          priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Vat,                new TableColumn ("TVA",              priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Total,              new TableColumn ("Prix TTC",         priceWidth,   ContentAlignment.MiddleRight));
			}																							        
			else																						        
			{																							        
				this.tableColumns.Add (TableColumnKeys.LineNumber,         new TableColumn ("N°",               priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleId,          new TableColumn ("Article",          priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleDescription, new TableColumn ("Désignation",      0,            ContentAlignment.MiddleLeft));  // seule colonne en mode width = fill
				this.tableColumns.Add (TableColumnKeys.MainQuantity,       new TableColumn ("Facturé",          priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.AdditionalType,     new TableColumn ("Autres quantités", priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.AdditionalQuantity, new TableColumn ("",                 priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.AdditionalDate,     new TableColumn ("",                 priceWidth+3, ContentAlignment.MiddleLeft));
																										        
				this.tableColumns.Add (TableColumnKeys.UnitPrice,          new TableColumn ("p.u. HT",          priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Discount,           new TableColumn ("Rabais",           priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.LinePrice,          new TableColumn ("Prix HT",          priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Vat,                new TableColumn ("TVA",              priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Total,              new TableColumn ("Prix TTC",         priceWidth,   ContentAlignment.MiddleRight));
			}
		}

		protected override DocumentItemAccessorMode DocumentItemAccessorMode
		{
			get
			{
				var mode = DocumentItemAccessorMode.Print |
						   DocumentItemAccessorMode.UseArticleName;  // le nom court suffit sur une facture

				if (this.HasOption (DocumentOption.ArticleAdditionalQuantities))  // imprime les autres quantités ?
				{
					mode |= DocumentItemAccessorMode.AdditionalQuantities;
				}

				return mode;
			}
		}

		protected override void HideColumns(List<DocumentItemAccessor> accessors)
		{
			if (this.HasOption (DocumentOption.LineNumber, "None"))
			{
				this.tableColumns[TableColumnKeys.LineNumber].Visible = false;
			}

			if (!this.HasOption (DocumentOption.ArticleId))
			{
				this.tableColumns[TableColumnKeys.ArticleId].Visible = false;
			}

			if (!this.HasOption (DocumentOption.ArticleAdditionalQuantities) ||
				AbstractDocumentMetadataPrinter.IsEmptyColumn (accessors, DocumentItemAccessorColumn.AdditionalQuantity))
			{
				this.tableColumns[TableColumnKeys.AdditionalType].Visible = false;
				this.tableColumns[TableColumnKeys.AdditionalQuantity].Visible = false;
				this.tableColumns[TableColumnKeys.AdditionalDate].Visible = false;
			}

			if (AbstractDocumentMetadataPrinter.IsEmptyColumn (accessors, DocumentItemAccessorColumn.Discount))
			{
				this.tableColumns[TableColumnKeys.Discount].Visible = false;
			}

			if (AbstractDocumentMetadataPrinter.IsEmptyColumn (accessors, DocumentItemAccessorColumn.Vat))
			{
				this.tableColumns[TableColumnKeys.Vat].Visible = false;
			}
		}

		protected override void FinishColumns()
		{
			if (this.HasOption (DocumentOption.ArticleAdditionalQuantities))
			{
				this.columnsWithoutRightBorder.Add (TableColumnKeys.AdditionalType);
				this.columnsWithoutRightBorder.Add (TableColumnKeys.AdditionalQuantity);
			}
		}

		protected override int BuildLine(int row, DocumentItemAccessor accessor, ContentLine prevLine, ContentLine line, ContentLine nextLine)
		{
			if (this.BuildTitleLine (row, accessor, line))
			{
				return accessor.RowsCount;
			}

			for (int i = 0; i < accessor.RowsCount; i++)
			{
				if (!this.HasOption (DocumentOption.LineNumber, "None"))
				{
					this.SetTableText (row+i, TableColumnKeys.LineNumber, accessor.GetContent (i, DocumentItemAccessorColumn.LineNumber));
				}

				this.SetTableText (row+i, TableColumnKeys.MainQuantity, AbstractDocumentMetadataPrinter.GetQuantityAndUnit (accessor, i, DocumentItemAccessorColumn.MainQuantity, DocumentItemAccessorColumn.MainUnit));

				if (this.HasOption (DocumentOption.ArticleAdditionalQuantities))  // imprime les autres quantités ?
				{
					this.SetTableText (row+i, TableColumnKeys.AdditionalType,     accessor.GetContent (i, DocumentItemAccessorColumn.AdditionalType));
					this.SetTableText (row+i, TableColumnKeys.AdditionalQuantity, AbstractDocumentMetadataPrinter.GetQuantityAndUnit (accessor, i, DocumentItemAccessorColumn.AdditionalQuantity, DocumentItemAccessorColumn.AdditionalUnit));
					this.SetTableText (row+i, TableColumnKeys.AdditionalDate,     AbstractDocumentMetadataPrinter.GetDates (accessor, i, DocumentItemAccessorColumn.AdditionalBeginDate, DocumentItemAccessorColumn.AdditionalEndDate));
				}

				if (this.HasOption (DocumentOption.ArticleId))
				{
					this.SetTableText (row+i, TableColumnKeys.ArticleId, accessor.GetContent (i, DocumentItemAccessorColumn.ArticleId));
				}

				this.SetTableText (row+i, TableColumnKeys.ArticleDescription, accessor.GetContent (i, DocumentItemAccessorColumn.ArticleDescription));

				this.SetTableText (row+i, TableColumnKeys.UnitPrice, accessor.GetContent (i, DocumentItemAccessorColumn.UnitPrice));
				this.SetTableText (row+i, TableColumnKeys.Discount,  accessor.GetContent (i, DocumentItemAccessorColumn.Discount));
				this.SetTableText (row+i, TableColumnKeys.LinePrice, accessor.GetContent (i, DocumentItemAccessorColumn.LinePrice));
				this.SetTableText (row+i, TableColumnKeys.Vat,       accessor.GetContent (i, DocumentItemAccessorColumn.Vat));

				var total = accessor.GetContent (i, DocumentItemAccessorColumn.Total);
				if (line.Line is EndTotalDocumentItemEntity && i == accessor.RowsCount-1)
				{
					total = total.ApplyBold ();
				}
				this.SetTableText (row+i, TableColumnKeys.Total, total);

				this.SetCellBorder (row+i, this.GetCellBorder ());
			}

			int last = row+accessor.RowsCount-1;

			if (line.Line is SubTotalDocumentItemEntity)
			{
				this.SetCellBorder (last, this.GetCellBorder (bottomBold: true));
			}

			if (line.Line is EndTotalDocumentItemEntity)
			{
				if (this.IsWithFrame)
				{
					if (!this.onlyTotal)
					{
						this.SetCellBorder (last, this.GetCellBorder (topLess: true));
					}

					this.SetCellBorder (TableColumnKeys.Total, last, new CellBorder (CellBorder.BoldWidth));
				}
				else
				{
					this.SetCellBorder (last, this.GetCellBorder (bottomBold: true, topLess: true));
				}
			}

			return accessor.RowsCount;
		}



		private void BuildConditions()
		{
			//	Met les conditions à la fin de la facture.
			FormattedText conditions = FormattedText.Join (FormattedText.HtmlBreak, this.paymentTransactionEntity.Text, this.paymentTransactionEntity.PaymentDetail.PaymentCategory.Description);

			if (!conditions.IsNullOrEmpty)
			{
				var band = new TextBand ();
				band.Text = conditions;
				band.FontSize = this.FontSize;

				this.documentContainer.AddFromTop (band, 0);
			}
		}


		private void BuildIsrs(int firstPage)
		{
			if (this.HasOption (DocumentOption.IsrPosition, "WithInside"))
			{
				this.BuildInsideIsrs (firstPage);
			}

			if (this.HasOption (DocumentOption.IsrPosition, "WithOutside"))
			{
				this.BuildOutsideIsr (firstPage);
			}
		}

		private void BuildInsideIsrs(int firstPage)
		{
			//	Met un BVR orangé ou un BV rose en bas de chaque page.
			for (int page = firstPage; page < this.documentContainer.PageCount (); page++)
			{
				this.documentContainer.CurrentPage = page;

				this.BuildIsr (mackle: page != this.documentContainer.PageCount ()-1);
			}
		}

		private void BuildOutsideIsr(int firstPage)
		{
			//	Met un BVR orangé ou un BV rose sur une dernière page séparée.
			var bounds = new Rectangle (Point.Zero, AbstractIsrBand.DefautlSize);

			if (this.documentContainer.PageCount () - firstPage > 1 ||
				this.documentContainer.CurrentVerticalPosition - InvoiceDocumentPrinter.marginBeforeIsr < bounds.Top ||
				this.HasPrintingUnitDefined (PageType.Single) == false)
			{
				//	On ne prépare pas une nouvelle page si on peut mettre la facture
				//	et le BV sur une seule page !
				this.documentContainer.PrepareEmptyPage (PageType.Isr);
			}

			this.BuildIsr ();
		}

		private void BuildIsr(bool mackle=false)
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
			isr.To = this.paymentTransactionEntity.PaymentDetail.PaymentCategory.IsrDefinition.SubscriberAddress;
			isr.Communication = InvoiceDocumentHelper.GetTitle (this.Metadata, this.Entity, this.paymentTransactionEntity);

			isr.Slip = new IsrSlip (this.paymentTransactionEntity);
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

		private bool						onlyTotal;
		private PaymentTransactionEntity	paymentTransactionEntity;
	}
}
