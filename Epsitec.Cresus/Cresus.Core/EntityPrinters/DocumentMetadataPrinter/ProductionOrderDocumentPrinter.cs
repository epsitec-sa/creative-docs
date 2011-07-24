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
	public class ProductionOrderDocumentPrinter : AbstractDocumentMetadataPrinter
	{
		public ProductionOrderDocumentPrinter(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
			: base (businessContext, entity, options, printingUnits)
		{
		}


		public override FormattedText BuildSections()
		{
			base.BuildSections ();

			this.documentRank = 0;

			var groups = this.ProductionGroups;

			if (groups.Count == 0)
			{
				return "Les conditions suivantes doivent être remplies pour pouvoir imprimer ce document:<br/><br/>" +
					   "1) Il doit y avoir au moins un article utilisant la catégorie \"Marchandises\".<br/>" +
					   "2) Cet article doit faire partie d'un groupe.";
			}
			else
			{
				foreach (var group in groups)
				{
					this.currentGroup = group;

					this.documentContainer.DocumentRank = this.documentRank++;
					int firstPage = this.documentContainer.PrepareEmptyPage (PageType.First);

					this.BuildHeader (null);
					this.BuildArticles ();
					this.BuildFooter ();
					this.BuildPages (null, firstPage);

					this.documentContainer.Ending (firstPage);
				}

				return null;  // ok
			}
		}


		protected override TableBand BuildConcerne()
		{
			var band = new TableBand ();

			band.ColumnsCount = 2;
			band.RowsCount = 1;
			band.CellBorder = CellBorder.Default;
			band.Font = AbstractDocumentMetadataPrinter.font;
			band.FontSize = this.FontSize;
			band.CellMargins = new Margins (1);
			band.SetRelativeColumWidth (0, 15);
			band.SetRelativeColumWidth (1, 80);
			band.SetText (0, 0, "Atelier", this.FontSize);
			band.SetText (1, 0, FormattedText.FromSimpleText (this.currentGroup.Name.ToSimpleText ()).ApplyBold (), this.FontSize);
			band.SetBackground (1, 0, Color.FromBrightness (0.9));

			return band;
		}

		protected override IEnumerable<ContentLine> ContentLines
		{
			get
			{
				//	Donne les lignes du groupe de production en cours.
				foreach (var line in this.Entity.Lines.Where (x => ProductionOrderDocumentPrinter.IsArticleForProduction (x, this.currentGroup)))
				{
					yield return new ContentLine (line, this.documentRank);
				}
			}
		}

		protected override void InitializeColumns()
		{
			this.tableColumns.Clear ();

			double priceWidth = this.PriceWidth;

			if (this.IsColumnsOrderQD)
			{
				this.tableColumns.Add (TableColumnKeys.LineNumber,         new TableColumn ("N°",          priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.MainQuantity,       new TableColumn ("Quantité",    priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.ArticleId,          new TableColumn ("Article",     priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleDescription, new TableColumn ("Désignation", 0,            ContentAlignment.MiddleLeft));  // seule colonne en mode width = fill
				this.tableColumns.Add (TableColumnKeys.Total,              new TableColumn ("Visa",        priceWidth,   ContentAlignment.MiddleLeft));
			}
			else
			{
				this.tableColumns.Add (TableColumnKeys.LineNumber,         new TableColumn ("N°",          priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleId,          new TableColumn ("Article",     priceWidth,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add (TableColumnKeys.ArticleDescription, new TableColumn ("Désignation", 0,            ContentAlignment.MiddleLeft));  // seule colonne en mode width = fill
				this.tableColumns.Add (TableColumnKeys.MainQuantity,       new TableColumn ("Quantité",    priceWidth,   ContentAlignment.MiddleRight));
				this.tableColumns.Add (TableColumnKeys.Total,              new TableColumn ("Visa",        priceWidth,   ContentAlignment.MiddleLeft));
			}
		}

		protected override DocumentItemAccessorMode DocumentItemAccessorMode
		{
			get
			{
				var mode = DocumentItemAccessorMode.UseMainColumns |
						   DocumentItemAccessorMode.DescriptionIndented |
						   DocumentItemAccessorMode.UseArticleName;  // le nom court suffit

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
		}

		protected override int BuildLine(int row, DocumentItemAccessor accessor, ContentLine prevLine, ContentLine line, ContentLine nextLine)
		{
			for (int i = 0; i < accessor.RowsCount; i++)
			{
				if (!this.HasOption (DocumentOption.LineNumber, "None"))
				{
					this.SetTableText (row+i, TableColumnKeys.LineNumber, accessor.GetContent (i, DocumentItemAccessorColumn.LineNumber));
				}

				this.SetTableText (row+i, TableColumnKeys.MainQuantity, AbstractDocumentMetadataPrinter.GetQuantityAndUnit (accessor, i, DocumentItemAccessorColumn.MainQuantity, DocumentItemAccessorColumn.MainUnit));

				if (this.HasOption (DocumentOption.ArticleId))
				{
					this.SetTableText (row+i, TableColumnKeys.ArticleId, accessor.GetContent (i, DocumentItemAccessorColumn.ArticleId));
				}

				this.SetTableText (row+i, TableColumnKeys.ArticleDescription, accessor.GetContent (i, DocumentItemAccessorColumn.ArticleDescription));
			}

			this.SetTableText (row, TableColumnKeys.Total, new string (' ', 30));  // Visa

			return accessor.RowsCount;
		}


		private void BuildFooter()
		{
			if (this.HasOption (DocumentOption.Signing))
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
		}


		private List<ArticleGroupEntity> ProductionGroups
		{
			//	Retourne la liste des groupes des articles du document.
			get
			{
				var groups = new List<ArticleGroupEntity> ();

				foreach (var line in this.Entity.Lines)
				{
					if (line is ArticleDocumentItemEntity)
					{
						var article = line as ArticleDocumentItemEntity;

						if (article.ArticleDefinition.ArticleCategory.ArticleType == ArticleType.Goods)  // marchandises ?
						{
							foreach (var group in article.ArticleDefinition.ArticleGroups)
							{
								if (!groups.Contains (group))
								{
									groups.Add (group);
								}
							}
						}
					}
				}

				return groups;
			}
		}

		private static bool IsArticleForProduction(AbstractDocumentItemEntity item, ArticleGroupEntity group)
		{
			//	Retourne true s'il s'agit d'un article qui doit figurer sur un ordre de production.
			if (item is ArticleDocumentItemEntity)
			{
				var article = item as ArticleDocumentItemEntity;

				return article.ArticleDefinition.ArticleGroups.Contains (group);
			}

			return false;
		}


		private ArticleGroupEntity				currentGroup;
		private int								documentRank;
	}
}
