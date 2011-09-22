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
using Epsitec.Cresus.Core.Resolvers;
using Epsitec.Cresus.Core.Library.Business.ContentAccessors;
using Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.EntityPrinters
{
	public sealed class SalesQuoteDocumentPrinter : BusinessDocumentPrinter
	{
		internal SalesQuoteDocumentPrinter(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
			: base (businessContext, entity, options, printingUnits)
		{
		}


		public static IEnumerable<DocumentOption> RequiredDocumentOptions
		{
			get
			{
				yield return DocumentOption.Orientation;

				foreach (var option in BusinessDocumentPrinter.RequiredHeaderDocumentOptions)
				{
					yield return option;
				}

				yield return DocumentOption.TableTopAfterHeader;
				yield return DocumentOption.TableFontSize;

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
			}
		}

		public static IEnumerable<PageType> RequiredPageTypes
		{
			get
			{
				yield return PageType.Single;
				yield return PageType.First;
				yield return PageType.Following;
			}
		}


		public override FormattedText BuildSections()
		{
			base.BuildSections ();

			if (this.ContentLines.Any () == false)
			{
				return new FormattedText ("Il n'y a rien à imprimer, car le document ne contient aucune ligne.");
			}

			int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

			this.BuildHeader ();
			this.BuildArticles ();
			this.BuildPages (firstPage);
			this.BuildReportHeaders (firstPage);
			this.BuildReportFooters (firstPage);

			this.documentContainer.Ending (firstPage);

			return null;  // ok
		}


		protected override bool HasPrices
		{
			get
			{
				return true;
			}
		}

		protected override void InitializeColumns()
		{
			this.tableColumns.Clear ();

			double priceWidth = this.PriceWidth;

			if (this.IsColumnsOrderQD)
			{
				this.tableColumns.Add (TableColumnKeys.LineNumber,         new TableColumn (this.GetColumnDescription (TableColumnKeys.LineNumber),         priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.MainQuantity,       new TableColumn ("Commandé",                                                     priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.AdditionalType,     new TableColumn (this.GetColumnDescription (TableColumnKeys.AdditionalType),     priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.AdditionalQuantity, new TableColumn (this.GetColumnDescription (TableColumnKeys.AdditionalQuantity), priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.AdditionalDate,     new TableColumn (this.GetColumnDescription (TableColumnKeys.AdditionalDate),     priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleId,          new TableColumn (this.GetColumnDescription (TableColumnKeys.ArticleId),          priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleDescription, new TableColumn (this.GetColumnDescription (TableColumnKeys.ArticleDescription), 0,            ContentAlignment.MiddleLeft));  // seule colonne en mode width = fill
																										        
				this.tableColumns.Add (TableColumnKeys.UnitPrice,          new TableColumn (this.GetColumnDescription (TableColumnKeys.UnitPrice),          priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Discount,           new TableColumn (this.GetColumnDescription (TableColumnKeys.Discount),           priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.LinePrice,          new TableColumn (this.GetColumnDescription (TableColumnKeys.LinePrice),          priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Vat,                new TableColumn (this.GetColumnDescription (TableColumnKeys.Vat),                priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Total,              new TableColumn (this.GetColumnDescription (TableColumnKeys.Total),              priceWidth,   ContentAlignment.MiddleRight));
			}																							        
			else																						        
			{																							        
				this.tableColumns.Add (TableColumnKeys.LineNumber,         new TableColumn (this.GetColumnDescription (TableColumnKeys.LineNumber),         priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleId,          new TableColumn (this.GetColumnDescription (TableColumnKeys.ArticleId),          priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleDescription, new TableColumn (this.GetColumnDescription (TableColumnKeys.ArticleDescription), 0,            ContentAlignment.MiddleLeft));  // seule colonne en mode width = fill
				this.tableColumns.Add (TableColumnKeys.MainQuantity,       new TableColumn ("Commandé",                                                     priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.AdditionalType,     new TableColumn (this.GetColumnDescription (TableColumnKeys.AdditionalType),     priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.AdditionalQuantity, new TableColumn (this.GetColumnDescription (TableColumnKeys.AdditionalQuantity), priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.AdditionalDate,     new TableColumn (this.GetColumnDescription (TableColumnKeys.AdditionalDate),     priceWidth+3, ContentAlignment.MiddleLeft));
																										        
				this.tableColumns.Add (TableColumnKeys.UnitPrice,          new TableColumn (this.GetColumnDescription (TableColumnKeys.UnitPrice),          priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Discount,           new TableColumn (this.GetColumnDescription (TableColumnKeys.Discount),           priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.LinePrice,          new TableColumn (this.GetColumnDescription (TableColumnKeys.LinePrice),          priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Vat,                new TableColumn (this.GetColumnDescription (TableColumnKeys.Vat),                priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Total,              new TableColumn (this.GetColumnDescription (TableColumnKeys.Total),              priceWidth,   ContentAlignment.MiddleRight));
			}
		}

		protected override DocumentItemAccessorMode DocumentItemAccessorMode
		{
			get
			{
				var mode = DocumentItemAccessorMode.Print |
						   DocumentItemAccessorMode.UseArticleBoth;

				mode |= this.GetDocumentItemAccessorMode ();

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

			if (!this.HasOption (DocumentOption.ArticleAdditionalQuantities, "Separate") ||
				BusinessDocumentPrinter.IsEmptyColumn (accessors, DocumentItemAccessorColumn.AdditionalQuantity))
			{
				this.tableColumns[TableColumnKeys.AdditionalType].Visible = false;
				this.tableColumns[TableColumnKeys.AdditionalQuantity].Visible = false;
				this.tableColumns[TableColumnKeys.AdditionalDate].Visible = false;
			}

			if (BusinessDocumentPrinter.IsEmptyColumn (accessors, DocumentItemAccessorColumn.LineDiscount))
			{
				this.tableColumns[TableColumnKeys.Discount].Visible = false;
			}

			if (BusinessDocumentPrinter.IsEmptyColumn (accessors, DocumentItemAccessorColumn.VatRate))
			{
				this.tableColumns[TableColumnKeys.Vat].Visible = false;
			}
		}

		protected override void FinishColumns()
		{
			if (this.HasOption (DocumentOption.ArticleAdditionalQuantities, "Separate"))
			{
				this.columnsWithoutRightBorder.Add (TableColumnKeys.AdditionalType);
				this.columnsWithoutRightBorder.Add (TableColumnKeys.AdditionalQuantity);
			}
		}

		protected override int BuildLine(int row, DocumentItemAccessor accessor, DocumentAccessorContentLine prevLine, DocumentAccessorContentLine line, DocumentAccessorContentLine nextLine)
		{
			if (this.BuildTitleLine (row, accessor, line))
			{
				return accessor.RowsCount;
			}

			int count = accessor.RowsCount;
			for (int i = 0; i < count; i++)
			{
				if (!this.HasOption (DocumentOption.LineNumber, "None"))
				{
					this.SetTableText (row+i, TableColumnKeys.LineNumber, accessor.GetContent (i, DocumentItemAccessorColumn.LineNumber));
				}

				this.SetTableText (row+i, TableColumnKeys.MainQuantity, BusinessDocumentPrinter.GetQuantityAndUnit (accessor, i, DocumentItemAccessorColumn.MainQuantity, DocumentItemAccessorColumn.MainUnit));

				if (this.HasOption (DocumentOption.ArticleId))
				{
					this.SetTableText (row+i, TableColumnKeys.ArticleId, accessor.GetContent (i, DocumentItemAccessorColumn.ArticleId));
				}

				this.SetTableText (row+i, TableColumnKeys.ArticleDescription, accessor.GetContent (i, DocumentItemAccessorColumn.ArticleDescription));
				this.BuildLineAdditionalQuantities (row, accessor, i);  // imprime les autres quantités
				this.IndentCellMargins (row+i, TableColumnKeys.ArticleDescription, line.GroupIndex);

				this.SetTableText (row+i, TableColumnKeys.UnitPrice, accessor.GetContent (i, DocumentItemAccessorColumn.UnitPrice));
				this.SetTableText (row+i, TableColumnKeys.Discount, accessor.GetContent (i, DocumentItemAccessorColumn.LineDiscount));
				this.SetTableText (row+i, TableColumnKeys.LinePrice, accessor.GetContent (i, DocumentItemAccessorColumn.LinePrice));
				this.SetTableText (row+i, TableColumnKeys.Vat, accessor.GetContent (i, DocumentItemAccessorColumn.VatRate));

				var total = accessor.GetContent (i, DocumentItemAccessorColumn.TotalPrice);
				if (line.Line is EndTotalDocumentItemEntity && i == accessor.RowsCount-1)
				{
					total = total.ApplyBold ();
				}
				this.SetTableText (row+i, TableColumnKeys.Total, total);

				this.SetCellBorder (row, i, count);
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
					this.SetCellBorder (TableColumnKeys.Total, last, new CellBorder (CellBorder.BoldWidth));
				}
				else
				{
					this.SetCellBorder (last, this.GetCellBorder (bottomBold: true, topLess: true));
				}
			}

			return accessor.RowsCount;
		}
	}
}
