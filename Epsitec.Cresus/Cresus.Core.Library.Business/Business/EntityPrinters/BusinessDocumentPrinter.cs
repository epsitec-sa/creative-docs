//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
using Epsitec.Cresus.Core.Business.Finance;
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
	public abstract class BusinessDocumentPrinter : AbstractPrinter
	{
		protected BusinessDocumentPrinter(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
			: base (businessContext, entity, options, printingUnits)
		{
			var documentMetadata = this.businessContext.GetMasterEntity<DocumentMetadataEntity> ();
			System.Diagnostics.Debug.Assert (documentMetadata != null);
			this.documentLogic = new DocumentLogic (this.businessContext as BusinessContext, documentMetadata);

			this.columnsWithoutRightBorder = new List<TableColumnKeys> ();
		}


		protected static IEnumerable<DocumentOption> RequiredHeaderDocumentOptions
		{
			get
			{
				yield return DocumentOption.HeaderSender;

				yield return DocumentOption.HeaderLogoLeft;
				yield return DocumentOption.HeaderLogoTop;
				yield return DocumentOption.HeaderLogoWidth;
				yield return DocumentOption.HeaderLogoHeight;

				yield return DocumentOption.HeaderFromAddress;
				yield return DocumentOption.HeaderFromLeft;
				yield return DocumentOption.HeaderFromTop;
				yield return DocumentOption.HeaderFromWidth;
				yield return DocumentOption.HeaderFromHeight;
				yield return DocumentOption.HeaderFromFontSize;
				
				yield return DocumentOption.HeaderForLeft;
				yield return DocumentOption.HeaderForTop;
				yield return DocumentOption.HeaderForWidth;
				yield return DocumentOption.HeaderForHeight;
				yield return DocumentOption.HeaderForFontSize;
				
				yield return DocumentOption.HeaderNumberLeft;
				yield return DocumentOption.HeaderNumberTop;
				yield return DocumentOption.HeaderNumberWidth;
				yield return DocumentOption.HeaderNumberHeight;
				yield return DocumentOption.HeaderNumberFontSize;
				
				yield return DocumentOption.HeaderToLeft;
				yield return DocumentOption.HeaderToTop;
				yield return DocumentOption.HeaderToWidth;
				yield return DocumentOption.HeaderToHeight;
				yield return DocumentOption.HeaderToFontSize;
				
				yield return DocumentOption.HeaderLocDateLeft;
				yield return DocumentOption.HeaderLocDateTop;
				yield return DocumentOption.HeaderLocDateWidth;
				yield return DocumentOption.HeaderLocDateHeight;
				yield return DocumentOption.HeaderLocDateFontSize;
			}
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
				double dy = (this.PreviewMode == PreviewMode.ContinuousPreview) ? AbstractPrinter.ContinuousHeight : 297;

				return new Size (dx, dy);  // A4
			}
		}


		protected override Margins GetPageMargins()
		{
			double leftMargin   = this.GetOptionValue (DocumentOption.LeftMargin);
			double rightMargin  = this.GetOptionValue (DocumentOption.RightMargin);
			double topMargin    = this.GetOptionValue (DocumentOption.TopMargin);
			double bottomMargin = this.GetOptionValue (DocumentOption.BottomMargin);

			double h = this.HasPrices ? BusinessDocumentPrinter.reportHeight : 0;

			return new Margins (leftMargin, rightMargin, topMargin+h*2, h+bottomMargin);
		}


		public override FormattedText BuildSections()
		{
			base.BuildSections ();

			this.documentContainer.Clear ();

			return null;  // ok
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


		protected void BuildHeader()
		{
			//	Ajoute l'en-tête dans le document.
			var context = this.businessContext as BusinessContext;
			var settings = context.GetCachedBusinessSettings ();

			//	Génère l'image du logo de l'entreprise.
			if (this.HasOption (DocumentOption.HeaderSender) && settings.CompanyLogo.IsNotNull ())
			{
				var rect = this.GetOptionRectangle (DocumentOption.HeaderLogoLeft);

				if (!rect.IsSurfaceZero)
				{
					var imageBand = new ImageBand ();
					imageBand.Load (this.coreData, settings.CompanyLogo);
					imageBand.BuildSections (rect.Width, rect.Height, rect.Height, rect.Height);
					this.documentContainer.AddAbsolute (imageBand, rect);
				}
			}

			//	Génère l'adresse de l'entreprise.
			if (this.HasOption (DocumentOption.HeaderSender) && settings.Company.IsNotNull ())
			{
				var rect = this.GetOptionRectangle (DocumentOption.HeaderFromLeft);

				if (!rect.IsSurfaceZero)
				{
					FormattedText address = this.GetOptionText (DocumentOption.HeaderFromAddress);

					if (address.IsNullOrEmpty)
					{
						address = settings.Company.DefaultMailContact.GetSummary ();
					}

					var textBand = new TextBand ();
					textBand.LanguageId = this.LanguageId;
					textBand.Text = address;
					textBand.Font = BusinessDocumentPrinter.font;
					textBand.FontSize = this.GetOptionValue (DocumentOption.HeaderFromFontSize);
					this.documentContainer.AddAbsolute (textBand, rect);
				}
			}

			//	Génère l'adresse du client.
			{
				var rect = this.GetOptionRectangle (DocumentOption.HeaderToLeft);

				if (!rect.IsSurfaceZero)
				{
					var mailContactBand = new TextBand ();
					mailContactBand.LanguageId = this.LanguageId;
					mailContactBand.Text = this.Entity.BillToMailContact.GetSummary ();
					mailContactBand.Font = BusinessDocumentPrinter.font;
					mailContactBand.FontSize = this.GetOptionValue (DocumentOption.HeaderToFontSize);
					this.documentContainer.AddAbsolute (mailContactBand, rect);
				}
			}

			//	Génère le groupe "concerne".
			{
				var rect = this.GetOptionRectangle (DocumentOption.HeaderForLeft);
				var band = this.BuildConcerne (rect.Width);

				if (band != null && !rect.IsSurfaceZero)
				{
					this.documentContainer.AddAbsolute (band, rect);
				}
			}

			//	Génère le groupe "numéro de facture".
			{
				var rect = this.GetOptionRectangle (DocumentOption.HeaderNumberLeft);

				if (!rect.IsSurfaceZero)
				{
					var titleBand = new TextBand ();
					titleBand.LanguageId = this.LanguageId;
					titleBand.Text = this.Title;
					titleBand.BreakMode = TextBreakMode.SingleLine | TextBreakMode.Split | TextBreakMode.Ellipsis;
					titleBand.Font = font;
					titleBand.FontSize = this.GetOptionValue (DocumentOption.HeaderNumberFontSize);
					this.documentContainer.AddAbsolute (titleBand, rect);
				}
			}

			//	Génère le groupe "localité et date".
			{
				var rect = this.GetOptionRectangle (DocumentOption.HeaderLocDateLeft);

				if (!rect.IsSurfaceZero)
				{
					string date = Misc.GetDateShortDescription (this.DocumentDate);

					var location = this.DefaultLocation;
					var dateBand = new TextBand ();
					dateBand.LanguageId = this.LanguageId;
					dateBand.Text = (location == null) ? FormattedText.Concat ("Le ", date) : FormattedText.Concat (location, ", le ", date);
					dateBand.Font = font;
					dateBand.FontSize = this.GetOptionValue (DocumentOption.HeaderLocDateFontSize);
					this.documentContainer.AddAbsolute (dateBand, rect);
				}
			}
		}

		private Date DocumentDate
		{
			//	Retourne la date qui doit figurer sur le document.
			get
			{
				if (this.Entity.BillingDate.HasValue)
				{
					return this.Entity.BillingDate.Value;
				}
				else if (this.Metadata.LastModificationDate.HasValue)
				{
					return new Date (this.Metadata.LastModificationDate.Value);
				}
				else if (this.Metadata.CreationDate.HasValue)
				{
					return new Date (this.Metadata.CreationDate.Value);
				}
				else
				{
					return Date.Today;
				}
			}
		}

		private Rectangle GetOptionRectangle(DocumentOption firstOption)
		{
			int first = (int) firstOption;

			double left   = this.GetOptionValue ((DocumentOption) (first+0));  // HeaderXxxLeft
			double top    = this.GetOptionValue ((DocumentOption) (first+1));  // HeaderXxxTop
			double width  = this.GetOptionValue ((DocumentOption) (first+2));  // HeaderXxxWidth
			double height = this.GetOptionValue ((DocumentOption) (first+3));  // HeaderXxxHeight

			return new Rectangle (left, this.RequiredPageSize.Height-top-height, width, height);
		}

		private FormattedText DefaultLocation
		{
			//	Retourne la ville de l'entreprise, pour imprimer par exemple "Yverdon-les-Bains, le 30 septembre 2010".
			get
			{
				var context = this.businessContext as BusinessContext;
				var settings = context.GetCachedBusinessSettings ();

				if (settings.IsNull () ||
					settings.Company.IsNull () ||
					settings.Company.DefaultMailContact.IsNull () ||
					settings.Company.DefaultMailContact.Address.IsNull () ||
					settings.Company.DefaultMailContact.Address.Location.IsNull ())
				{
					return null;
				}
				else
				{
					return settings.Company.DefaultMailContact.Address.Location.Name;
				}
			}
		}


		protected void BuildArticles(double? verticalPosition = null)
		{
			//	Ajoute les articles dans le document.
			double y = this.GetOptionValue (DocumentOption.TableTopAfterHeader);
			this.documentContainer.CurrentVerticalPosition = verticalPosition.HasValue ? verticalPosition.Value : this.RequiredPageSize.Height-y;

			this.InitializeColumns ();

			//	Construit une fois pour toutes les accesseurs au contenu.
			var lines = this.ContentLines;
			var accessors = new List<DocumentItemAccessor> (DocumentItemAccessor.CreateAccessors (this.Metadata, this.documentLogic, this.DocumentItemAccessorMode, lines));

			//	Fusionne toutes les lignes de sous-total, pour les rendre inséparables.
			for (int i = 0; i < lines.Count; i++)
			{
				if (accessors[i].IsSubTotal)
				{
					accessors[i].TransformToSingleRow ();
				}
			}

			//	Première passe pour déterminer le nombre le lignes du tableau ainsi que
			//	les colonnes visibles.
			int rowCount = 1;  // déjà 1 pour l'en-tête (titres des colonnes)

			for (int i = 0; i < lines.Count; i++)
			{
				var contentLine = lines[i];

				if (contentLine.Line.Attributes.HasFlag (DocumentItemAttributes.Hidden) == false)
				{
					rowCount += this.InitializeLine (accessors[i], contentLine);
				}
			}

			this.HideColumns (accessors);

			//	Compte et numérote les colonnes visibles.
			this.visibleColumnCount = 0;

			foreach (var column in this.tableColumns.Values)
			{
				if (column.Visible)
				{
					column.Rank = this.visibleColumnCount++;
				}
			}

			this.FinishColumns ();

			//	Deuxième passe pour générer les colonnes et les lignes du tableau.
			this.table = new TableBand ();
			this.table.LanguageId = this.LanguageId;
			this.table.ColumnsCount = this.visibleColumnCount;
			this.table.RowsCount = rowCount;
			this.table.CellBorder = this.GetCellBorder ();
			this.table.CellMargins = new Margins (this.CellMargin);

			//	Génère une première ligne d'en-tête (titres des colonnes).
			int row = 0;

			this.InitializeHeaderTableText (this.table, row);
			this.InitializeRowAlignment (this.table, row);
			this.SetCellBorder (row, this.GetCellBorder (bottomBold: true));

			row++;

			//	Génère toutes les lignes pour les articles.
			int linePage = this.documentContainer.CurrentPage;
			double lineY = this.documentContainer.CurrentVerticalPosition;

			for (int i = 0; i < lines.Count; i++)
			{
				var contentLine = lines[i];

				if (contentLine.Line.Attributes.HasFlag (DocumentItemAttributes.Hidden) == false)
				{
					var prevLine = (i == 0            ) ? DocumentAccessorContentLine.Empty : lines[i-1];
					var nextLine = (i >= lines.Count-1) ? DocumentAccessorContentLine.Empty : lines[i+1];

					int rowUsed = this.BuildLine (row, accessors[i], prevLine, contentLine, nextLine);
					this.BuildCommonLine (row, accessors[i], contentLine);

					if (accessors[i].IsSubTotal)
					{
						//	Les lignes de sous-total ont été fusionnées avec TransformToSingleRow.
						//	Il ne reste qu'à rendre le tout inséparable.
						this.table.SetUnbreakableRow (row, true);
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

			if (fixedWidth < this.documentContainer.CurrentWidth - 10)  // reste au moins 1cm pour la colonne 'fill' ?
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

		private void BuildCommonLine(int row, DocumentItemAccessor accessor, DocumentAccessorContentLine line)
		{
			FormattedText text = null;

			if (this.HasOption (DocumentOption.LineNumber, "Group"))
			{
				text = accessor.GetContent (0, DocumentItemAccessorColumn.GroupNumber);
			}

			if (this.HasOption (DocumentOption.LineNumber, "Line"))
			{
				text = accessor.GetContent (0, DocumentItemAccessorColumn.LineNumber);
			}

			if (this.HasOption (DocumentOption.LineNumber, "Full"))
			{
				text = accessor.GetContent (0, DocumentItemAccessorColumn.FullNumber);
			}

			this.table.SetText (this.tableColumns[TableColumnKeys.LineNumber].Rank, row, text, this.GetOptionValue (DocumentOption.TableFontSize));

			var topGap = 0;  // pas d'espace supplémentaire

			if (this.HasOption (DocumentOption.GapBeforeGroup))
			{
				bool hasTopGap = !accessor.GetContent (0, DocumentItemAccessorColumn.GroupNumber).IsNullOrEmpty;  // début d'un groupe ?

				if (hasTopGap)
				{
					if (this.IsWithFrame)
					{
						topGap = 2;  // 2mm d'espace entre les groupes
					}
					else
					{
						topGap = 5;  // 5mm d'espace entre les groupes
					}
				}
			}


			var cellBorder = this.table.GetCellBorder (0, row);
			this.table.SetCellBorder (0, row, new CellBorder (cellBorder.LeftWidth, cellBorder.RightWidth, cellBorder.BottomWidth, cellBorder.TopWidth, topGap, cellBorder.Color));
		}

		protected bool BuildTitleLine(int row, DocumentItemAccessor accessor, DocumentAccessorContentLine line)
		{
			//	S'il s'agit d'une ligne de texte contenant un titre, elle est générée de façon très spéciale,
			//	en occupant toutes les colonnes (avec ColumnSpan), sauf l'éventuelle première qui contient le
			//	numéro de ligne.
			//	Retourne true s'il s'agissait d'une ligne de texte contenant un titre et qu'elle a été gérée.
			if (line.Line is TextDocumentItemEntity)
			{
				var text = accessor.GetContent (0, DocumentItemAccessorColumn.ArticleDescription);

				if (LineEngine.IsTitle (text))
				{
					TableColumnKeys firstColumn = TableColumnKeys.LineNumber;
					int span = 0;
					foreach (var pair in this.tableColumns)
					{
						var column = pair.Value;

						if (pair.Key != TableColumnKeys.LineNumber && column.Visible)
						{
							if (span == 0)
							{
								firstColumn = pair.Key;
							}

							span++;
						}
					}

					if (!this.HasOption (DocumentOption.LineNumber, "None"))
					{
						this.SetTableText (row, TableColumnKeys.LineNumber, accessor.GetContent (0, DocumentItemAccessorColumn.LineNumber));
					}

					this.table.SetAlignment (this.tableColumns[firstColumn].Rank, row, ContentAlignment.MiddleLeft);
					this.SetTableText (row, firstColumn, text);
					this.SetColumnSpan (row, firstColumn, span);

					if (this.IsWithFrame)
					{
						var c = this.table.GetCellBorder (0, row);
						this.SetCellBorder (row, new CellBorder (0, 0, 0, 0, c.TopGap, c.Color));
					}

					return true;
				}
			}

			return false;
		}


		protected virtual bool HasPrices
		{
			get
			{
				return false;
			}
		}

		protected virtual FormattedText Title
		{
			get
			{
				return InvoiceDocumentHelper.GetTitle (this.Metadata, this.Entity, null);
			}
		}

		protected virtual TableBand BuildConcerne(double width)
		{
			var text = TextFormatter.FormatText (this.Metadata.DocumentTitle);
			double firstColumnWidth = 16;

			if (text.IsNullOrEmpty || width < firstColumnWidth+10)
			{
				return null;
			}
			else
			{
				var band = new TableBand ();

				var fontSize = this.GetOptionValue (DocumentOption.HeaderForFontSize);
				firstColumnWidth *= fontSize/3;

				band.LanguageId = this.LanguageId;
				band.ColumnsCount = 2;
				band.RowsCount = 1;
				band.CellBorder = CellBorder.Empty;
				band.Font = BusinessDocumentPrinter.font;
				band.FontSize = fontSize;
				band.CellMargins = new Margins (0);
				band.SetRelativeColumWidth (0, firstColumnWidth);
				band.SetRelativeColumWidth (1, width-firstColumnWidth);
				band.SetText (0, 0, "Concerne", fontSize);
				band.SetText (1, 0, text, fontSize);
				band.SetBackground (1, 0, Color.Empty);

				return band;
			}
		}

		public IList<DocumentAccessorContentLine> ContentLines
		{
			get
			{
				return this.GetContentLines ().ToList ();
			}
		}

		protected void InvalidateContentLines()
		{
			//	TODO: vide le cache des content lines
		}

		protected virtual IEnumerable<DocumentAccessorContentLine> GetContentLines()
		{
			//	Donne nornalement toutes les lignes.
			//	Les versions dans les classes dérivées peuvent ne donner que certaines lignes,
			//	dans un ordre spécial, etc.
			return this.Entity.GetConciseLines ().Select (x => new DocumentAccessorContentLine (x));
		}


		protected virtual void InitializeColumns()
		{
		}

		protected string GetColumnDescription(TableColumnKeys columnKey)
		{
			switch (columnKey)
			{
				case TableColumnKeys.LineNumber:
					return "N°";

				case TableColumnKeys.MainQuantity:
					return "Quantité";

				case TableColumnKeys.AdditionalType:
					return "Autres quantités";

				case TableColumnKeys.ArticleId:
					return "Article";

				case TableColumnKeys.ArticleDescription:
					return "Désignation";

				case TableColumnKeys.UnitPrice:
					return this.Entity.IsExcludingTax ? "p.u. HT" : "p.u. TTC";

				case TableColumnKeys.Discount:
					return "Rabais";

				case TableColumnKeys.LinePrice:
					return this.Entity.IsExcludingTax ? "Prix HT" : "Prix TTC";

				case TableColumnKeys.Vat:
					return "TVA";

				case TableColumnKeys.Total:
					return "Total";

				default:
					return "";
			}
		}

		protected virtual DocumentItemAccessorMode DocumentItemAccessorMode
		{
			get
			{
				return DocumentItemAccessorMode.None;
			}
		}

		protected virtual int InitializeLine(DocumentItemAccessor accessor, DocumentAccessorContentLine line)
		{
			return accessor.RowsCount;
		}

		protected virtual void HideColumns(List<DocumentItemAccessor> accessors)
		{
		}

		protected virtual void FinishColumns()
		{
		}

		protected virtual int BuildLine(int row, DocumentItemAccessor accessor, DocumentAccessorContentLine prevLine, DocumentAccessorContentLine line, DocumentAccessorContentLine nextLine)
		{
			return 0;
		}


		protected static FormattedText GetQuantityAndUnit(DocumentItemAccessor accessor, int row, DocumentItemAccessorColumn quantity, DocumentItemAccessorColumn unit)
		{
			var q = accessor.GetContent (row, quantity).ToString ();
			var u = accessor.GetContent (row, unit).ToString ();

			if (string.IsNullOrEmpty (q))
			{
				return null;
			}
			else
			{
				return TextFormatter.FormatText (q, u);
			}
		}

		protected static FormattedText GetDates(DocumentItemAccessor accessor, int row, DocumentItemAccessorColumn begin, DocumentItemAccessorColumn end)
		{
			FormattedText b = accessor.GetContent (row, begin);
			FormattedText e = accessor.GetContent (row, end);

			if (!b.IsNullOrEmpty && !e.IsNullOrEmpty)
			{
				return FormattedText.Concat (b, " au ", e);
			}
			else if (!b.IsNullOrEmpty && e.IsNullOrEmpty)
			{
				return b;
			}
			else if (b.IsNullOrEmpty && !e.IsNullOrEmpty)
			{
				return FormattedText.Concat ("Au ", e);
			}
			else
			{
				return null;
			}
		}


		protected void BuildPages(int firstPage)
		{
			//	Met les numéros de page.
			double reportHeight = this.HasPrices ? BusinessDocumentPrinter.reportHeight*2 : 0;

			var margins     = this.GetPageMargins ();
			var leftBounds  = new Rectangle (margins.Left, this.RequiredPageSize.Height-margins.Top+reportHeight+1, 80, 5);
			var rightBounds = new Rectangle (this.RequiredPageSize.Width-margins.Right-80, this.RequiredPageSize.Height-margins.Top+reportHeight+1, 80, 5);

			int count = this.documentContainer.PageCount ();
			for (int page = firstPage+1; page < count; page++)  // toutes les pages, sauf la première
			{
				this.documentContainer.CurrentPage = page;

				var leftHeader = new TextBand ();
				leftHeader.LanguageId = this.LanguageId;
				leftHeader.Text = this.Title;
				leftHeader.Alignment = ContentAlignment.BottomLeft;
				leftHeader.Font = font;
				leftHeader.FontSize = this.FontSize*1.3;

				var rightHeader = new TextBand ();
				rightHeader.LanguageId = this.LanguageId;
				rightHeader.Text = FormattedText.Concat ("page ", (page-firstPage+1).ToString ());
				rightHeader.Alignment = ContentAlignment.BottomRight;
				rightHeader.Font = font;
				rightHeader.FontSize = this.FontSize;

				this.documentContainer.AddAbsolute (leftHeader, leftBounds);
				this.documentContainer.AddAbsolute (rightHeader, rightBounds);
			}
		}

		protected void BuildReportHeaders(int firstPage)
		{
			//	Met un report en haut des pages concernées, avec une répétition de la ligne
			//	d'en-tête (noms des colonnes).
			var margins = this.GetPageMargins ();
			double width = this.RequiredPageSize.Width-margins.Left-margins.Right;

			int count = this.documentContainer.PageCount ();
			for (int page = firstPage+1; page < count; page++)  // toutes les pages, sauf la première
			{
				int relativePage = page-firstPage;

				if (relativePage >= this.tableBounds.Count)
				{
					break;
				}

				this.documentContainer.CurrentPage = page;

				var table = new TableBand ();
				table.LanguageId = this.LanguageId;
				table.ColumnsCount = this.visibleColumnCount;
				table.RowsCount = 2;
				table.CellMargins = new Margins (this.CellMargin);

				this.CloneColumnWidth (table);
				this.SetCellBorder (table, 0, this.GetCellBorder (bottomBold: true));
				this.SetCellBorder (table, 1, this.GetCellBorder (bottomBold: true));

				//	Génère une première ligne d'en-tête (titres des colonnes).
				this.InitializeHeaderTableText (table, 0);
				this.InitializeRowAlignment (table, 0);

				//	Génère une deuxième ligne avec les montants à reporter.
				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, 1, "Report", this.GetOptionValue (DocumentOption.TableFontSize));

				decimal sumPT, sumTot;
				this.ComputeBottomReports (relativePage-1, out sumPT, out sumTot);
				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, 1, Misc.PriceToString (sumPT),  this.GetOptionValue (DocumentOption.TableFontSize));
				table.SetText (this.tableColumns[TableColumnKeys.Total    ].Rank, 1, Misc.PriceToString (sumTot), this.GetOptionValue (DocumentOption.TableFontSize));
				this.InitializeRowAlignment (table, 1);

				var tableBound = this.tableBounds[relativePage];
				double h = table.RequiredHeight (width);
				var bounds = new Rectangle (tableBound.Left, tableBound.Top, width, h);

				this.documentContainer.AddAbsolute (table, bounds);
			}
		}

		protected void BuildReportFooters(int firstPage)
		{
			//	Met un report en bas des pages concernées.
			var margins = this.GetPageMargins ();
			double width = this.RequiredPageSize.Width-margins.Left-margins.Right;

			int count = this.documentContainer.PageCount ();
			for (int page = firstPage; page < count-1; page++)  // toutes les pages, sauf la dernière
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
				table.LanguageId = this.LanguageId;
				table.ColumnsCount = this.visibleColumnCount;
				table.RowsCount = 1;
				table.CellMargins = new Margins (this.CellMargin);

				this.CloneColumnWidth (table);
				this.SetCellBorder (table, 0, this.GetCellBorder (topBold: true));
				this.InitializeRowAlignment (table, 0);

				table.SetText (this.tableColumns[TableColumnKeys.ArticleDescription].Rank, 0, "à reporter", this.GetOptionValue (DocumentOption.TableFontSize));

				decimal sumPT, sumTot;
				this.ComputeBottomReports (relativePage, out sumPT, out sumTot);
				table.SetText (this.tableColumns[TableColumnKeys.LinePrice].Rank, 0, Misc.PriceToString (sumPT),  this.GetOptionValue (DocumentOption.TableFontSize));
				table.SetText (this.tableColumns[TableColumnKeys.Total    ].Rank, 0, Misc.PriceToString (sumTot), this.GetOptionValue (DocumentOption.TableFontSize));

				var tableBound = this.tableBounds[relativePage];
				double h = table.RequiredHeight (width);
				var bounds = new Rectangle (tableBound.Left, tableBound.Bottom-h, width, h);

				this.documentContainer.AddAbsolute (table, bounds);
			}
		}

		private void ComputeBottomReports(int page, out decimal sumPT, out decimal sumTot)
		{
			//	Calcul les reports à montrer en bas d'une page, ou en haut de la suivante.
			sumPT  = 0;
			sumTot = 0;

			int lastRow = this.lastRowForEachSection[page];
			var lines = this.ContentLines;

			for (int row = 0; row <= lastRow; row++)
			{
				if (row == 0 ||  // en-tête ?
					row-1 >= lines.Count)
				{
					continue;
				}

				var contentLine = lines[row-1];  // -1 à cause de l'en-tête

				if (contentLine.Line is ArticleDocumentItemEntity)
				{
					var article = contentLine.Line as ArticleDocumentItemEntity;

					decimal beforeTax = article.LinePriceBeforeTax2.GetValueOrDefault ();
					decimal afterTax  = article.LinePriceAfterTax2.GetValueOrDefault ();

					sumPT  += beforeTax;
					sumTot += afterTax;
				}
			}
		}


		private void InitializeRowAlignment(TableBand table, int row)
		{
			foreach (var column in this.tableColumns.Values)
			{
				if (column.Visible)
				{
					//	Initialise l'alignement seulement s'il n'y a pas de ColumnSpan.
					//	En effet, dans ce cas l'alignement a déjà été spécifié manuellement.
					if (table.GetColumnSpan (column.Rank, row) < 2)
					{
						table.SetAlignment (column.Rank, row, column.Alignment);
					}
				}
			}
		}

		private void InitializeHeaderTableText(TableBand table, int row)
		{
			//	Génère les textes dans la première ligne d'en-tête d'un tableau.
			for (int i = 0; i < this.tableColumns.Count; i++)
			{
				var pair = this.tableColumns.ElementAt (i);
				var column = pair.Value;

				if (column.Visible)
				{
					//	Les 3 colonnes AdditionalType, AdditionalQuantity et AdditionalDate forment
					//	toujours un bloc dans l'en-tête.
					if (i <= this.tableColumns.Count-3                                              &&
						pair.Key                              == TableColumnKeys.AdditionalType     &&
						this.tableColumns.ElementAt (i+1).Key == TableColumnKeys.AdditionalQuantity &&
						this.tableColumns.ElementAt (i+2).Key == TableColumnKeys.AdditionalDate     )
					{
						table.SetColumnSpan (column.Rank, row, 3);
					}

					table.SetText (column.Rank, row, column.Title, this.GetOptionValue (DocumentOption.TableFontSize));
				}
			}
		}

		private void CloneColumnWidth(TableBand table)
		{
			//	Initialise les largeurs d'une table quelconque d'après la table principale.
			foreach (var column in this.tableColumns.Values)
			{
				if (column.Visible)
				{
					table.SetRelativeColumWidth (column.Rank, this.table.GetRelativeColumnWidth (column.Rank));
				}
			}
		}


		#region TableBand helpers
		protected FormattedText GetTableText(int row, TableColumnKeys columnKey)
		{
			if (this.GetColumnVisibility (columnKey))
			{
				return this.table.GetText (this.tableColumns[columnKey].Rank, row);
			}
			else
			{
				return FormattedText.Empty;
			}
		}

		protected void SetTableText(int row, TableColumnKeys columnKey, FormattedText text)
		{
			if (this.GetColumnVisibility (columnKey))
			{
				this.table.SetText (this.tableColumns[columnKey].Rank, row, text, this.GetOptionValue (DocumentOption.TableFontSize));
			}
		}

		protected bool GetColumnVisibility(TableColumnKeys columnKey)
		{
			if (this.tableColumns.ContainsKey (columnKey))
			{
				return this.tableColumns[columnKey].Visible;
			}
			else
			{
				return false;
			}
		}

		protected void SetColumnVisibility(TableColumnKeys columnKey, bool visibility)
		{
			if (this.tableColumns.ContainsKey (columnKey))
			{
				this.tableColumns[columnKey].Visible = visibility;
			}
		}

		protected void SetColumnSpan(int row, TableColumnKeys columnKey, int span)
		{
			if (this.tableColumns.ContainsKey (columnKey))
			{
				this.table.SetColumnSpan (this.tableColumns[columnKey].Rank, row, span);
			}
		}


		protected void IndentCellMargins(int row, TableColumnKeys columnKey, int groupIndex)
		{
			//	Indente une cellule, en augmentant sa marge gauche.
			int level = System.Math.Max (AbstractDocumentItemEntity.GetGroupLevel (groupIndex)-1, 0);
			double indent = this.GetOptionValue (DocumentOption.IndentWidth) * level;

			if (indent > 0)
			{
				int column = this.tableColumns[columnKey].Rank;

				var m1 = this.table.GetCellMargins (column, row);

				if (m1 == Margins.Zero)
				{
					m1 = this.table.CellMargins;
				}

				var m2 = new Margins (m1.Left+indent, m1.Right, m1.Top, m1.Bottom);

				this.table.SetCellMargins (column, row, m2);
			}
		}

		private void SetCellMargins(int row, bool bottomZero = false, bool topZero = false)
		{
			//	Détermine les marges supérieures et inférieures d'une ligne du tableau.
			for (int column = 0; column < this.table.ColumnsCount; column++)
			{
				var m1 = this.table.GetCellMargins (column, row);

				if (m1 == Margins.Zero)
				{
					m1 = this.table.CellMargins;
				}

				//	En cas de marge nulle, on laisse une petite valeur résiduelle de 0.2mm, ce qui est discutable.
				//	On verra à l'usage s cela est judicieux !
				var m2 = new Margins (m1.Left, m1.Right, topZero ? 0.2 : m1.Top, bottomZero ? 0.2 : m1.Bottom);

				this.table.SetCellMargins (column, row, m2);
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

		protected CellBorder GetCellBorder(bool bottomBold = false, bool topBold = false, bool bottomLess = false, bool topLess = false)
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

		protected void SetCellBorder(int row, DocumentItemAccessor accessor, int i, int count)
		{
			//	Choix des bords et des marges. Le but est de former visuellement un "groupe" pour les
			//	lignes d'un même article (par exemple).
			bool bottomLess = false;
			bool topLess    = false;
			bool bottomZero = false;
			bool topZero    = false;

			if (i > 0)
			{
				topLess = true;
				topZero = !accessor.IsEndTotal;  // on laisse les marges pour le grand total
			}

			if (i < count-1)
			{
				bottomLess = true;
				bottomZero = !accessor.IsEndTotal;  // on laisse les marges pour le grand total
			}

			this.SetCellBorder (row+i, this.GetCellBorder (bottomLess: bottomLess, topLess: topLess));
			this.SetCellMargins (row+i, bottomZero: bottomZero, topZero: topZero);
		}

		protected void SetCellBorder(int row, CellBorder value)
		{
			this.SetCellBorder (this.table, row, value);
		}

		protected void SetCellBorder(TableBand table, int row, CellBorder value)
		{
			foreach (var pair in this.tableColumns)
			{
				if (pair.Value.Visible)
				{
					this.SetCellBorder (table, pair.Key, row, value);
				}
			}
		}

		protected void SetCellBorder(TableColumnKeys columnKey, int row, CellBorder value)
		{
			this.SetCellBorder (this.table, columnKey, row, value);
		}

		protected void SetCellBorder(TableBand table, TableColumnKeys columnKey, int row, CellBorder value)
		{
			if (this.IsWithFrame)
			{
				if (this.columnsWithoutRightBorder.Contains (columnKey))
				{
					//	Plus de trait à droite.
					value = new CellBorder (value.LeftWidth, 0, value.BottomWidth, value.TopWidth, value.TopGap, value.Color);
				}

				if (this.columnsWithoutRightBorder.Contains (columnKey-1))
				{
					//	Plus de trait à gauche.
					value = new CellBorder (0, value.RightWidth, value.BottomWidth, value.TopWidth, value.TopGap, value.Color);
				}
			}

			table.SetCellBorder (this.tableColumns[columnKey].Rank, row, value);
		}


		protected static bool IsEmptyColumn(List<DocumentItemAccessor> accessors, DocumentItemAccessorColumn column)
		{
			foreach (var accessor in accessors)
			{
				for (int i = 0; i < accessor.RowsCount; i++)
				{
					var content = accessor.GetContent (i, column);

					if (!content.IsNullOrEmpty)
					{
						return false;
					}
				}
			}

			return true;
		}
		#endregion


		#region Common helpers for production documents
		protected List<ArticleGroupEntity> ProductionGroups
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

		protected bool IsArticleForProduction(AbstractDocumentItemEntity item, ArticleGroupEntity group)
		{
			//	Retourne true s'il s'agit d'une ligne qui doit figurer sur un ordre de production.
			if (item is ArticleDocumentItemEntity)
			{
				//	S'il s'agit d'un article, il doit appartenir au bon groupe.
				var article = item as ArticleDocumentItemEntity;

				return article.ArticleDefinition.ArticleGroups.Contains (group);
			}

			if (item is TextDocumentItemEntity)
			{
				//	S'il s'agit d'un texte, il doit faire partie du même GroupIndex qu'un article appartenant au bon groupe.
				if (item.Attributes.HasFlag (DocumentItemAttributes.MyEyesOnly))
				{
					if (this.productionTexts == null)  // ExtractProductionTexts pas encore effectué ?
					{
						this.ExtractProductionTexts ();
					}

					ArticleDocumentItemEntity article;
					if (this.productionTexts.TryGetValue (item as TextDocumentItemEntity, out article))
					{
						return article.ArticleDefinition.ArticleGroups.Contains (group);
					}
				}
			}

			return false;
		}

		private void ExtractProductionTexts()
		{
			//	Pour chaque ligne de texte 'MyEyesOnly' (TextDocumentItemEntity), essaie de trouver une ligne
			//	d'article (ArticleDocumentItemEntity) à laquelle l'attacher.
			//	Un algorithme arbitraire de priorités essaie d'attacher la ligne à l'article au-dessus en
			//	premier lieu. Il peut arriver de ne pas trouver d'article auquel attacher une ligne !
			this.productionTexts = new Dictionary<TextDocumentItemEntity, ArticleDocumentItemEntity> ();

			ArticleDocumentItemEntity article;

			for (int i = 0; i < this.Entity.Lines.Count; i++)
			{
				var line = this.Entity.Lines[i] as TextDocumentItemEntity;

				if (line != null && line.Attributes.HasFlag (DocumentItemAttributes.MyEyesOnly))
				{
					//	1) Cherche un article au-dessus faisant partie du même groupe.
					article = this.GetArticleAt (i, -1);
					if (article != null && article.GroupIndex == line.GroupIndex)
					{
						this.productionTexts.Add (line, article);
						continue;
					}

					//	2) Cherche un article au-dessous faisant partie du même groupe.
					article = this.GetArticleAt (i, 1);
					if (article != null && article.GroupIndex == line.GroupIndex)
					{
						this.productionTexts.Add (line, article);
						continue;
					}

					//	3) Cherche un article au-dessus.
					article = this.GetArticleAt (i, -1);
					if (article != null)
					{
						this.productionTexts.Add (line, article);
						continue;
					}

					//	4) Cherche un article au-dessous.
					article = this.GetArticleAt (i, 1);
					if (article != null)
					{
						this.productionTexts.Add (line, article);
						continue;
					}
				}
			}
		}

		private ArticleDocumentItemEntity GetArticleAt(int index, int direction)
		{
			while (true)
			{
				index += direction;

				if (index < 0 || index >= this.Entity.Lines.Count)
				{
					return null;
				}

				var article = this.Entity.Lines[index] as ArticleDocumentItemEntity;

				if (article != null)
				{
					return article;
				}
			}
		}
		#endregion


		protected void BuildLineAdditionalQuantities(int row, DocumentItemAccessor accessor, int i)
		{
			if (!this.HasOption (DocumentOption.ArticleAdditionalQuantities, "None"))  // imprime les autres quantités ?
			{
				var t = accessor.GetContent (i, DocumentItemAccessorColumn.AdditionalType);
				var q = BusinessDocumentPrinter.GetQuantityAndUnit (accessor, i, DocumentItemAccessorColumn.AdditionalQuantity, DocumentItemAccessorColumn.AdditionalUnit);
				var d = BusinessDocumentPrinter.GetDates (accessor, i, DocumentItemAccessorColumn.AdditionalBeginDate, DocumentItemAccessorColumn.AdditionalEndDate);

				if (!t.IsNullOrEmpty || !q.IsNullOrEmpty || !d.IsNullOrEmpty)
				{
					if (this.HasOption (DocumentOption.ArticleAdditionalQuantities, "Separate"))  // dans une colonne spécifique ?
					{
						//	1) "Retardé"
						//	2) "5 pces"
						//	3) "25.12.2011"
						this.SetTableText (row+i, TableColumnKeys.AdditionalType, t);
						this.SetTableText (row+i, TableColumnKeys.AdditionalQuantity, q);
						this.SetTableText (row+i, TableColumnKeys.AdditionalDate, d);
					}

					if (this.HasOption (DocumentOption.ArticleAdditionalQuantities, "ToQuantity") &&  // avec les quantités ?
						this.GetTableText (row+i, TableColumnKeys.MainQuantity).IsNullOrEmpty)  // (*)
					{
						//	"Retardé 5 pces 25.12.2011"
						this.SetTableText (row+i, TableColumnKeys.MainQuantity, TextFormatter.FormatText (t, q, d));  // très compact, peu de place
					}

					if (this.HasOption (DocumentOption.ArticleAdditionalQuantities, "ToDescription") &&  // avec les descriptions ?
						this.GetTableText (row+i, TableColumnKeys.ArticleDescription).IsNullOrEmpty)  // (*)
					{
						//	"Retardé 5 pces, le 25.12.2011"
						this.SetTableText (row+i, TableColumnKeys.ArticleDescription, TextFormatter.FormatText (t, q, "le~", d));  // assez de place à disposition
					}

					if (this.HasOption (DocumentOption.ArticleAdditionalQuantities, "ToQuantityAndDescription") &&  // réparti ?
						this.GetTableText (row+i, TableColumnKeys.MainQuantity).IsNullOrEmpty &&  // (*)
						this.GetTableText (row+i, TableColumnKeys.ArticleDescription).IsNullOrEmpty)  // (*)
					{
						//	1) "−5 pces"
						//	2) "Retardé, le 25.12.2011"
						this.SetTableText (row+i, TableColumnKeys.MainQuantity, FormattedText.Concat ("−", q));  // signe moins U2212 (pas tiret) !
						this.SetTableText (row+i, TableColumnKeys.ArticleDescription, TextFormatter.FormatText (t, ", le~", d));  // assez de place à disposition
					}

					// (*)	Si la cellule contient déjà un texte, il ne faut pas l'écraser. En effet,
					//		cette méthode est appelée après avoir rempli les contenus normaux. Les
					//		colonnes 'MainQuantity' ou 'ArticleDescription' contiennent donc déjà
					//		les informations de base, qu'il ne faut pas remplacer par des quantité
					//		additionnelles vides.
				}
			}
		}

		protected virtual DocumentItemAccessorMode GetDocumentItemAccessorMode()
		{
			var mode = DocumentItemAccessorMode.None;

			if (!this.HasOption (DocumentOption.ArticleAdditionalQuantities, "None"))
			{
				mode |= DocumentItemAccessorMode.AdditionalQuantities;
			}

			if (this.HasOption (DocumentOption.ArticleAdditionalQuantities, "Separate"))
			{
				mode |= DocumentItemAccessorMode.AdditionalQuantitiesSeparate;
			}

			return mode;
		}


		protected double PriceWidth
		{
			get
			{
				return 13 + this.CellMargin*2;  // largeur standard pour un montant ou une quantité
			}
		}

		private double CellMargin
		{
			get
			{
				return this.IsWithFrame ? 1 : 2;
			}
		}


		protected bool IsColumnsOrderQD
		{
			get
			{
				return this.HasOption (DocumentOption.ColumnsOrder, "QD");
			}
		}

		private bool IsWithLine
		{
			get
			{
				return this.HasOption (DocumentOption.LayoutFrame, "WithLine");
			}
		}

		protected bool IsWithFrame
		{
			get
			{
				return this.HasOption (DocumentOption.LayoutFrame, "WithFrame");
			}
		}


		protected BusinessDocumentEntity Entity
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

		protected DocumentMetadataEntity Metadata
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

			public bool CanPrint(AbstractEntity entity, PrintingOptionDictionary options)
			{
				return entity is DocumentMetadataEntity;
			}

			public IEnumerable<System.Type> GetSupportedEntityTypes()
			{
				yield return typeof (DocumentMetadataEntity);
			}

			public DocumentType GetDocumentType(AbstractEntity entity)
			{
				var documentMetadata = entity as DocumentMetadataEntity;

				if ((documentMetadata != null) &&
					(documentMetadata.DocumentCategory.IsNotNull ()))
				{
					return documentMetadata.DocumentCategory.DocumentType;
				}

				return DocumentType.Unknown;
			}

			public IEntityPrinter CreatePrinter(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
			{
				var documentMetadata = entity as DocumentMetadataEntity;

				switch (documentMetadata.DocumentCategory.DocumentType)
				{
					case DocumentType.SalesQuote:
						return new SalesQuoteDocumentPrinter (businessContext, entity, options, printingUnits);

					case DocumentType.OrderBooking:
						return new OrderBookingDocumentPrinter (businessContext, entity, options, printingUnits);

					case DocumentType.OrderConfirmation:
						return new OrderConfirmationDocumentPrinter (businessContext, entity, options, printingUnits);

					case DocumentType.ProductionOrder:
						return new ProductionOrderDocumentPrinter (businessContext, entity, options, printingUnits);

					case DocumentType.ProductionChecklist:
						return new ProductionChecklistDocumentPrinter (businessContext, entity, options, printingUnits);

					case DocumentType.DeliveryNote:
						return new DeliveryNoteDocumentPrinter (businessContext, entity, options, printingUnits);

					case DocumentType.Invoice:
						return new InvoiceDocumentPrinter (businessContext, entity, options, printingUnits);
				}

				return null;
			}

			public IEnumerable<DocumentOption> GetRequiredDocumentOptions(DocumentType documentType)
			{
				switch (documentType)
				{
					case DocumentType.SalesQuote:
						return SalesQuoteDocumentPrinter.RequiredDocumentOptions;

					case DocumentType.OrderBooking:
						return OrderBookingDocumentPrinter.RequiredDocumentOptions;

					case DocumentType.OrderConfirmation:
						return OrderConfirmationDocumentPrinter.RequiredDocumentOptions;

					case DocumentType.ProductionOrder:
						return ProductionOrderDocumentPrinter.RequiredDocumentOptions;

					case DocumentType.ProductionChecklist:
						return ProductionChecklistDocumentPrinter.RequiredDocumentOptions;

					case DocumentType.DeliveryNote:
						return DeliveryNoteDocumentPrinter.RequiredDocumentOptions;

					case DocumentType.Invoice:
						return InvoiceDocumentPrinter.RequiredDocumentOptions;
				}

				return null;
			}

			public IEnumerable<PageType> GetRequiredPageTypes(DocumentType documentType)
			{
				switch (documentType)
				{
					case DocumentType.SalesQuote:
						return SalesQuoteDocumentPrinter.RequiredPageTypes;

					case DocumentType.OrderBooking:
						return OrderBookingDocumentPrinter.RequiredPageTypes;

					case DocumentType.OrderConfirmation:
						return OrderConfirmationDocumentPrinter.RequiredPageTypes;

					case DocumentType.ProductionOrder:
						return ProductionOrderDocumentPrinter.RequiredPageTypes;

					case DocumentType.ProductionChecklist:
						return ProductionChecklistDocumentPrinter.RequiredPageTypes;

					case DocumentType.DeliveryNote:
						return DeliveryNoteDocumentPrinter.RequiredPageTypes;

					case DocumentType.Invoice:
						return InvoiceDocumentPrinter.RequiredPageTypes;
				}

				return null;
			}

			#endregion
		}

		#endregion


		protected static readonly Font				font = Font.GetFont ("Arial", "Regular");
		protected static readonly double			reportHeight = 7.0;

		protected readonly DocumentLogic			documentLogic;
		protected readonly List<TableColumnKeys>	columnsWithoutRightBorder;

		private TableBand							table;
		private int									visibleColumnCount;
		private int[]								lastRowForEachSection;
		private List<Rectangle>						tableBounds;
		protected Dictionary<TextDocumentItemEntity, ArticleDocumentItemEntity> productionTexts;
	}
}
