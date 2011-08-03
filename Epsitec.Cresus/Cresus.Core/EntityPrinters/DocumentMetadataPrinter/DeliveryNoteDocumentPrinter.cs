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
	public class DeliveryNoteDocumentPrinter : AbstractDocumentMetadataPrinter
	{
		public DeliveryNoteDocumentPrinter(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
			: base (businessContext, entity, options, printingUnits)
		{
		}


		public static IEnumerable<DocumentOption> UsedDocumentOptions
		{
			get
			{
				yield return DocumentOption.Orientation;
				yield return DocumentOption.HeaderLogo;
				yield return DocumentOption.Specimen;
				yield return DocumentOption.FontSize;

				yield return DocumentOption.LeftMargin;
				yield return DocumentOption.RightMargin;
				yield return DocumentOption.TopMargin;
				yield return DocumentOption.TopMargin;

				yield return DocumentOption.LayoutFrame;
				yield return DocumentOption.GapBeforeGroup;
				yield return DocumentOption.IndentWidth;

				yield return DocumentOption.LineNumber;
				yield return DocumentOption.ArticleAdditionalQuantities;
				yield return DocumentOption.ArticleId;
				yield return DocumentOption.ColumnsOrder;
			}
		}


		public override FormattedText BuildSections()
		{
			base.BuildSections ();

			if (this.ContentLines.Count () == 0)
			{
				return "Il n'y a rien à imprimer, car le document ne contient aucune ligne.";
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


		protected override void InitializeColumns()
		{
			this.tableColumns.Clear ();

			double priceWidth = this.PriceWidth;

			if (this.IsColumnsOrderQD)
			{
				this.tableColumns.Add (TableColumnKeys.LineNumber,         new TableColumn ("N°",               priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.MainQuantity,       new TableColumn ("Livré",            priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.AdditionalType,     new TableColumn ("Autres quantités", priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.AdditionalQuantity, new TableColumn ("",                 priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.AdditionalDate,     new TableColumn ("",                 priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleId,          new TableColumn ("Article",          priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleDescription, new TableColumn ("Désignation",      0,            ContentAlignment.MiddleLeft));  // seule colonne en mode width = fill
			}																							       
			else																						       
			{																							       
				this.tableColumns.Add (TableColumnKeys.LineNumber,         new TableColumn ("N°",               priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleId,          new TableColumn ("Article",          priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleDescription, new TableColumn ("Désignation",      0,            ContentAlignment.MiddleLeft));  // seule colonne en mode width = fill
				this.tableColumns.Add (TableColumnKeys.MainQuantity,       new TableColumn ("Livré",            priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.AdditionalType,     new TableColumn ("Autres quantités", priceWidth+3, ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.AdditionalQuantity, new TableColumn ("",                 priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.AdditionalDate,     new TableColumn ("",                 priceWidth+3, ContentAlignment.MiddleLeft));
			}
		}

		protected override DocumentItemAccessorMode DocumentItemAccessorMode
		{
			get
			{
				var mode = DocumentItemAccessorMode.UseMainColumns |
						   DocumentItemAccessorMode.UseArticleName;  // le nom court suffit

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

			if (!this.HasOption (DocumentOption.ArticleAdditionalQuantities))
			{
				this.tableColumns[TableColumnKeys.AdditionalType].Visible = false;
				this.tableColumns[TableColumnKeys.AdditionalQuantity].Visible = false;
				this.tableColumns[TableColumnKeys.AdditionalDate].Visible = false;
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
					this.SetTableText (row+i, TableColumnKeys.AdditionalType, accessor.GetContent (i, DocumentItemAccessorColumn.AdditionalType));
					this.SetTableText (row+i, TableColumnKeys.AdditionalQuantity, AbstractDocumentMetadataPrinter.GetQuantityAndUnit (accessor, i, DocumentItemAccessorColumn.AdditionalQuantity, DocumentItemAccessorColumn.AdditionalUnit));
					this.SetTableText (row+i, TableColumnKeys.AdditionalDate, AbstractDocumentMetadataPrinter.GetDates (accessor, i, DocumentItemAccessorColumn.AdditionalBeginDate, DocumentItemAccessorColumn.AdditionalEndDate));
				}

				if (this.HasOption (DocumentOption.ArticleId))
				{
					this.SetTableText (row+i, TableColumnKeys.ArticleId, accessor.GetContent (i, DocumentItemAccessorColumn.ArticleId));
				}

				this.SetTableText (row+i, TableColumnKeys.ArticleDescription, accessor.GetContent (i, DocumentItemAccessorColumn.ArticleDescription));
				this.IndentCellMargins (row+i, TableColumnKeys.ArticleDescription, line.GroupIndex);

				this.SetCellBorder (row+i, this.GetCellBorder ());
			}

			int last = row+accessor.RowsCount-1;

			if (line.Line is SubTotalDocumentItemEntity ||
				line.Line is EndTotalDocumentItemEntity)
			{
				this.SetCellBorder (last, this.GetCellBorder (bottomBold: true));
			}

			return accessor.RowsCount;
		}
	}
}
