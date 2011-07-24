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
	public abstract class AbstractDocumentMetadataPrinter : AbstractPrinter
	{
		public AbstractDocumentMetadataPrinter(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
			: base (businessContext, entity, options, printingUnits)
		{
			var documentMetadata = this.businessContext.GetMasterEntity<DocumentMetadataEntity> ();
			System.Diagnostics.Debug.Assert (documentMetadata != null);
			this.businessLogic = new BusinessLogic (this.businessContext as BusinessContext, documentMetadata);
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


		protected override Margins PageMargins
		{
			get
			{
				double leftMargin   = this.GetOptionValue (DocumentOption.LeftMargin,   20);
				double rightMargin  = this.GetOptionValue (DocumentOption.RightMargin,  20);
				double topMargin    = this.GetOptionValue (DocumentOption.TopMargin,    20);
				double bottomMargin = this.GetOptionValue (DocumentOption.BottomMargin, 20);

				double h = this.IsDocumentWithoutPrice ? 0 : AbstractDocumentMetadataPrinter.reportHeight;

				return new Margins (leftMargin, rightMargin, topMargin+h*2, h+bottomMargin);
			}
		}


		public override void BuildSections()
		{
			base.BuildSections ();

			this.documentContainer.Clear ();
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


		// TODO: Supprimer !!!
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


		protected void BuildHeader(BillingDetailEntity billingDetails, ArticleGroupEntity group=null)
		{
			double leftMargin = this.GetOptionValue (DocumentOption.LeftMargin, 20);

			//	Ajoute l'en-tête dans le document.
			if (this.HasOption (DocumentOption.HeaderLogo))
			{
				var context = this.businessContext as BusinessContext;
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

			//	Génère l'adresse du client.
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
			var location = this.DefaultLocation;
			var dateBand = new TextBand ();
			dateBand.Text = (location == null) ? FormattedText.Concat ("Le ", date) : FormattedText.Concat (location, ", le ", date);
			dateBand.Font = font;
			dateBand.FontSize = this.FontSize;
			this.documentContainer.AddAbsolute (dateBand, new Rectangle (120, this.RequiredPageSize.Height-82, 80, 10-2));
		}

		private FormattedText DefaultLocation
		{
			//	Retourne la ville de l'entreprise, pour imprimer par exemple "Yverdon-les-Bains, le 30 septembre 2010".
			get
			{
				var context = this.businessContext as BusinessContext;
				var settings = context.GetCachedBusinessSettings ();

				if (settings == null ||
					settings.Company == null ||
					settings.Company.DefaultAddress == null ||
					settings.Company.DefaultAddress.Location == null)
				{
					return null;
				}
				else
				{
					return settings.Company.DefaultAddress.Location.Name;
				}
			}
		}


		protected void BuildArticles(ArticleGroupEntity group=null)
		{
			//	Ajoute les articles dans le document.
			this.documentContainer.CurrentVerticalPosition = this.RequiredPageSize.Height-87;

			this.InitializeColumns ();

			//	Construit une fois pour toutes les accesseurs au contenu.
			var accessors = new List<DocumentItemAccessor> ();
			var numberGenerator = new IncrementalNumberGenerator ();

			for (int i = 0; i < this.ContentLines.Count (); i++)
			{
				var contentLine = this.ContentLines.ElementAt (i);

				var accessor = new DocumentItemAccessor (this.Entity, this.businessLogic, numberGenerator);
				accessor.BuildContent (contentLine.Line, this.DocumentType, this.DocumentItemAccessorMode, contentLine.GroupIndex);

				accessors.Add (accessor);
			}

			//	Première passe pour déterminer le nombre le lignes du tableau aunsi que
			//	les colonnes visibles.
			int rowCount = 1;  // déjà 1 pour l'en-tête (titres des colonnes)

			for (int i = 0; i < this.ContentLines.Count (); i++)
			{
				var contentLine = this.ContentLines.ElementAt (i);

				if (contentLine.Line.Attributes.HasFlag (DocumentItemAttributes.Hidden) == false)
				{
					rowCount += this.InitializeLine (accessors[i], contentLine, group);
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

			//	Deuxième passe pour générer les colonnes et les lignes du tableau.
			this.table = new TableBand ();
			this.table.ColumnsCount = this.visibleColumnCount;
			this.table.RowsCount = rowCount;
			this.table.CellBorder = this.GetCellBorder ();
			this.table.CellMargins = new Margins (this.CellMargin);

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

			for (int i = 0; i < this.ContentLines.Count (); i++)
			{
				var contentLine = this.ContentLines.ElementAt (i);

				if (contentLine.Line.Attributes.HasFlag (DocumentItemAttributes.Hidden) == false)
				{
					var prevLine = (i == 0) ? null : this.ContentLines.ElementAt (i-1);
					var nextLine = (i >= this.ContentLines.Count ()-1) ? null : this.ContentLines.ElementAt (i+1);

					int rowUsed = this.BuildLine (row, accessors[i], prevLine, contentLine, nextLine);
					this.BuildCommonLine (this.table, row, accessors[i], contentLine);

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

			this.BuildFinish ();

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

		private void BuildCommonLine(TableBand table, int row, DocumentItemAccessor accessor, ContentLine line)
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

			table.SetText (this.tableColumns[TableColumnKeys.LineNumber].Rank, row, text, this.FontSize);

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

			var cellBorder = table.GetCellBorder (0, row);
			table.SetCellBorder (0, row, new CellBorder (cellBorder.LeftWidth, cellBorder.RightWidth, cellBorder.BottomWidth, cellBorder.TopWidth, topGap, cellBorder.Color));
		}


		protected virtual IEnumerable<ContentLine> ContentLines
		{
			get
			{
				//	Donne nornalement toutes les lignes.
				foreach (var line in this.Entity.Lines)
				{
					yield return new ContentLine (line);
				}
			}
		}

		#region ContentLine
		protected class ContentLine
		{
			public ContentLine(AbstractDocumentItemEntity line)
			{
				this.Line = line;
				this.GroupIndex = line.GroupIndex;
			}

			public ContentLine(AbstractDocumentItemEntity line, int groupIndex)
			{
				this.Line = line;
				this.GroupIndex = groupIndex;
			}

			public AbstractDocumentItemEntity Line
			{
				get;
				internal set;
			}

			public int GroupIndex
			{
				get;
				internal set;
			}
		}
		#endregion

		protected virtual void InitializeColumns()
		{
		}

		protected virtual DocumentItemAccessorMode DocumentItemAccessorMode
		{
			get
			{
				return DocumentItemAccessorMode.None;
			}
		}

		protected virtual int InitializeLine(DocumentItemAccessor accessor, ContentLine line, ArticleGroupEntity group)
		{
			return accessor.RowsCount;
		}

		protected virtual void HideColumns(List<DocumentItemAccessor> accessors)
		{
		}

		protected virtual int BuildLine(int row, DocumentItemAccessor accessor, ContentLine prevLine, ContentLine line, ContentLine nextLine)
		{
			return 0;
		}

		protected virtual void BuildFinish()
		{
		}


		protected void SetTableText(int row, TableColumnKeys column, FormattedText text)
		{
			if (this.GetColumnVisibility (column))
			{
				this.table.SetText (this.tableColumns[column].Rank, row, text, this.FontSize);
			}
		}

		protected bool GetColumnVisibility(TableColumnKeys column)
		{
			if (this.tableColumns.ContainsKey (column))
			{
				return this.tableColumns[column].Visible;
			}
			else
			{
				return false;
			}
		}

		protected void SetColumnVisibility(TableColumnKeys column, bool visibility)
		{
			if (this.tableColumns.ContainsKey (column))
			{
				this.tableColumns[column].Visible = visibility;
			}
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
				return Misc.FormatUnit (decimal.Parse (q), u);
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


		private bool IsPrintableArticle(ArticleDocumentItemEntity line)
		{
#if false
			if (!this.HasOption (DocumentOption.ArticleDelayed))  // n'imprime pas les articles retardés ?
			{
				foreach (var quantity in line.ArticleQuantities)
				{
					if (quantity.QuantityColumn.QuantityType == Business.ArticleQuantityType.Billed)
					{
						return true;
					}
				}

				return false;
			}
#endif

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

				if (this.DocumentType == Business.DocumentType.ProductionOrder ||
					this.DocumentType == Business.DocumentType.ProductionChecklist)
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

				if (this.DocumentType == Business.DocumentType.OrderConfirmation)
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
					table.SetText (0, 0, new FormattedText ("Confirmation de commande"), this.FontSize);
					table.SetText (1, 0, new FormattedText ("Lieu et date :<br/><br/>Signature :<br/><br/><br/>"), this.FontSize);
					table.SetUnbreakableRow (0, true);

					this.documentContainer.AddToBottom (table, this.PageMargins.Bottom);
				}
			}
		}

		protected void BuildPages(BillingDetailEntity billingDetails, int firstPage)
		{
			//	Met les numéros de page.
			double reportHeight = this.IsDocumentWithoutPrice ? 0 : AbstractDocumentMetadataPrinter.reportHeight*2;

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

		protected void BuildReportHeaders(int firstPage)
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

		protected void BuildReportFooters(int firstPage)
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

				var contentLine = this.ContentLines.ElementAt (row-1);  // -1 à cause de l'en-tête

				if (contentLine.Line is ArticleDocumentItemEntity)
				{
					var article = contentLine.Line as ArticleDocumentItemEntity;

					decimal beforeTax = article.ResultingLinePriceBeforeTax.GetValueOrDefault (0);
					decimal tax1 =      article.ResultingLineTax1          .GetValueOrDefault (0);
					decimal tax2 =      article.ResultingLineTax2          .GetValueOrDefault (0);

					sumPT  += beforeTax;
					sumTva += tax1 + tax2;
					sumTot += beforeTax+tax1+tax2;
				}
			}
		}


		private bool IsDocumentWithoutPrice
		{
			get
			{
				return this.DocumentType == Business.DocumentType.DeliveryNote ||
					   this.DocumentType == Business.DocumentType.ShipmentChecklist ||
					   this.DocumentType == Business.DocumentType.ProductionOrder ||
					   this.DocumentType == Business.DocumentType.ProductionChecklist;
			}
		}

		protected bool IsColumnsOrderQD
		{
			get
			{
				return this.HasOption (DocumentOption.ColumnsOrder, "QD");
			}
		}

		protected double CellMargin
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

		protected void SetCellBorder(int column, int row, CellBorder value)
		{
			this.table.SetCellBorder (column, row, value);
		}

		protected void SetCellBorder(int row, CellBorder value)
		{
			this.table.SetCellBorder (row, value);
		}


		protected void TableMakeBlock(TableBand table, int row, int rowsCount)
		{
			if (rowsCount == 1)
			{
				table.SetCellBorder (row, this.GetCellBorder (bottomBold: true));
			}
			else
			{
				for (int i = 0; i < rowsCount; i++)
				{
					bool first = (i == 0);
					bool last  = (i == rowsCount-1);

					if (!last)
					{
						table.SetUnbreakableRow (row+i, true);
					}

					bool topBold    = first;
					bool bottomBold = last;
					bool topLess    = !first;
					bool bottomLess = !last;

					double? topForce    = null;
					double? bottomForce = null;

					if (!first)
					{
						topForce = 0.5;
					}

					if (!last)
					{
						bottomForce = 0.5;
					}

					table.SetCellBorder (row+i, this.GetCellBorder (bottomBold, topBold, bottomLess, topLess));
					table.SetCellMargins (row+i, this.GetCellMargins (bottomForce, topForce));
				}
			}
		}

		protected void RemoveRightBorder(TableColumnKeys column)
		{
			if (this.IsWithFrame)
			{
				var columns = this.tableColumns.Where (x => x.Value.Visible).ToArray ();

				for (int row = 0; row < this.table.RowsCount; row++)
				{
					double dimmed = (row == 0) ? 0 : 0.01;  // laisse un trait hyper-fin si on n'est pas dans l'en-tête

					for (int i = 0; i < columns.Count ()-1; i++)
					{
						if (columns[i].Key == column)
						{
							CellBorder b1, b2;

							b1 = this.table.GetCellBorder (i, row);
							if (b1.IsEmpty)
							{
								b1 = this.table.CellBorder;
							}
							b2 = new CellBorder (b1.LeftWidth, dimmed, b1.BottomWidth, b1.TopWidth, b1.TopGap, b1.Color);
							this.table.SetCellBorder (i, row, b2);

							b1 = this.table.GetCellBorder (i+1, row);
							if (b1.IsEmpty)
							{
								b1 = this.table.CellBorder;
							}
							b2 = new CellBorder (0, b1.RightWidth, b1.BottomWidth, b1.TopWidth, b1.TopGap, b1.Color);
							this.table.SetCellBorder (i+1, row, b2);

							break;
						}
					}

				}
			}
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
					return this.Metadata.DocumentCategory.DocumentType;
				}
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

			public AbstractPrinter CreatePrinter(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
			{
				var documentMetadata = entity as DocumentMetadataEntity;

				switch (documentMetadata.DocumentCategory.DocumentType)
				{
					case Business.DocumentType.Invoice:
						return new InvoiceDocumentMetadataPrinter (businessContext, entity, options, printingUnits);
				}

				return null;
			}

			#endregion
		}

		#endregion


		private static readonly Font		font = Font.GetFont ("Arial", "Regular");
		protected static readonly double	reportHeight = 7.0;

		protected readonly BusinessLogic	businessLogic;

		private TableBand					table;
		private int							visibleColumnCount;
		private int[]						lastRowForEachSection;
		private List<Rectangle>				tableBounds;
	}
}
