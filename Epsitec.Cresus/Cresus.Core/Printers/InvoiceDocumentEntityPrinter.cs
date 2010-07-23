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

			type = new DocumentType ("BV", "Facture avec BV", "Facture A4 avec un bulletin de versement orange ou rose intégré au bas de chaque page.");
			AbstractEntityPrinter.DocumentTypeAddStyles (type.DocumentOptions);
			AbstractEntityPrinter.DocumentTypeAddBV     (type.DocumentOptions);
			this.DocumentTypes.Add (type);

			type = new DocumentType ("Simple", "Facture sans BV", "Facture A4 simple sans bulletin de versement.");
			AbstractEntityPrinter.DocumentTypeAddStyles      (type.DocumentOptions);
			AbstractEntityPrinter.DocumentTypeAddOrientation (type.DocumentOptions);
			AbstractEntityPrinter.DocumentTypeAddMargin      (type.DocumentOptions);
			AbstractEntityPrinter.DocumentTypeAddSpecimen    (type.DocumentOptions);
			this.DocumentTypes.Add (type);

			type = new DocumentType ("BL", "Bulletin de livraison", "Bulletin de livraison A4, sans prix.");
			AbstractEntityPrinter.DocumentTypeAddStyles      (type.DocumentOptions);
			AbstractEntityPrinter.DocumentTypeAddOrientation (type.DocumentOptions);
			AbstractEntityPrinter.DocumentTypeAddMargin      (type.DocumentOptions);
			AbstractEntityPrinter.DocumentTypeAddSpecimen    (type.DocumentOptions);
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
				if (this.HasDocumentOption ("Horizontal"))
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
				if (this.DocumentTypeSelected == "BV")
				{
					return new Margins (20, 10, 20, 10+AbstractBvBand.DefautlSize.Height);
				}
				else
				{
					return new Margins (20, 10, 20, 20);
				}
			}
		}

		public override void BuildSections()
		{
			base.BuildSections ();
			this.documentContainer.Clear ();

			if (this.DocumentTypeSelected == "BV")
			{
				this.BuildHeader ();
				this.BuildArticles ();
				this.BuildConditions ();
				this.BuildPages ();
				this.BuildBvs (bvr: this.HasDocumentOption ("BVR"));
			}

			if (this.DocumentTypeSelected == "Simple")
			{
				this.BuildHeader ();
				this.BuildArticles ();
				this.BuildConditions ();
				this.BuildPages ();
			}

			if (this.DocumentTypeSelected == "BL")
			{
				this.BuildHeader ();
				this.BuildArticles ();
				this.BuildConditions ();
				this.BuildPages ();
			}
		}

		public override void PrintCurrentPage(IPaintPort port, Rectangle bounds)
		{
			base.PrintCurrentPage (port, bounds);

			this.documentContainer.Paint (port, this.CurrentPage);
		}


		private void BuildHeader()
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

			string concerne = InvoiceDocumentHelper.GetConcerne (this.entity);
			if (!string.IsNullOrEmpty (concerne))
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
				concerneBand.SetText (1, 0, concerne);
				this.documentContainer.AddAbsolute (concerneBand, new Rectangle (20, this.PageSize.Height-67, 100, 15));
			}

			var titleBand = new TextBand ();
			titleBand.Text = InvoiceDocumentHelper.GetTitle (this.entity, this.IsBL);
			titleBand.Font = font;
			titleBand.FontSize = 5.0;
			this.documentContainer.AddAbsolute (titleBand, new Rectangle (20, this.PageSize.Height-82, 90, 10));

			string date = InvoiceDocumentHelper.GetDate (this.entity);
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
			if (this.IsModern)
			{
				double wd = 70.0;

				if (this.IsBL)
				{
					wd += 15*5;
				}

				if (this.PageSize.Width > this.PageSize.Height)  // paysage ?
				{
					wd += 80;
				}

				this.tableColumns.Add ("Desc", new TableColumn ("Désignation", wd,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add ("Nb",   new TableColumn ("Livré",       12.0, ContentAlignment.MiddleLeft));
				this.tableColumns.Add ("Suit", new TableColumn ("Suit",        12.0, ContentAlignment.MiddleLeft));
				this.tableColumns.Add ("Date", new TableColumn ("Date",        20.0, ContentAlignment.MiddleLeft));
				this.tableColumns.Add ("Rab",  new TableColumn ("Rabais",      15.0, ContentAlignment.MiddleRight));
				this.tableColumns.Add ("PU",   new TableColumn ("p.u. HT",     15.0, ContentAlignment.MiddleRight));
				this.tableColumns.Add ("PT",   new TableColumn ("Prix HT",     15.0, ContentAlignment.MiddleRight));
				this.tableColumns.Add ("TVA",  new TableColumn ("TVA",         15.0, ContentAlignment.MiddleRight));
				this.tableColumns.Add ("Tot",  new TableColumn ("Total",       15.0, ContentAlignment.MiddleRight));
			}
			else
			{
				double wd = 54.0;

				if (this.IsBL)
				{
					wd += 17*5;
				}

				if (this.PageSize.Width > this.PageSize.Height)  // paysage ?
				{
					wd += 80;
				}

				this.tableColumns.Add ("Nb",   new TableColumn ("Quantité",    17.0, ContentAlignment.MiddleLeft));
				this.tableColumns.Add ("Suit", new TableColumn ("Suit",        14.0, ContentAlignment.MiddleLeft));
				this.tableColumns.Add ("Date", new TableColumn ("Date",        22.0, ContentAlignment.MiddleLeft));
				this.tableColumns.Add ("Desc", new TableColumn ("Désignation", wd,   ContentAlignment.MiddleLeft));
				this.tableColumns.Add ("Rab",  new TableColumn ("Rabais",      17.0, ContentAlignment.MiddleRight));
				this.tableColumns.Add ("PU",   new TableColumn ("p.u. HT",     17.0, ContentAlignment.MiddleRight));
				this.tableColumns.Add ("PT",   new TableColumn ("Prix HT",     17.0, ContentAlignment.MiddleRight));
				this.tableColumns.Add ("TVA",  new TableColumn ("TVA",         17.0, ContentAlignment.MiddleRight));
				this.tableColumns.Add ("Tot",  new TableColumn ("Total",       17.0, ContentAlignment.MiddleRight));
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

					if (exist)
					{
						rowCount++;
					}
				}
			}

			if (this.IsBL)
			{
				this.tableColumns["Rab"].Visible = false;
				this.tableColumns["PU" ].Visible = false;
				this.tableColumns["PT" ].Visible = false;
				this.tableColumns["TVA"].Visible = false;
				this.tableColumns["Tot"].Visible = false;
			}

			//	Compte et numérote les colonnes visibles.
			int columnCount = 0;

			foreach (var column in this.tableColumns.Values)
			{
				if (column.Visible)
				{
					column.Rank = columnCount++;
				}
			}

			//	Deuxième passe pour générer les colonnes et les lignes du tableau.
			var table = new TableBand ();
			table.ColumnsCount = columnCount;
			table.RowsCount = rowCount;
			table.PaintFrame = this.IsModern;
			table.CellMargins = new Margins (this.IsModern ? 1 : 2);

			int row = 0;

			//	Génère une première ligne d'en-tête (titres des colonnes).
			foreach (var column in this.tableColumns.Values)
			{
				if (column.Visible)
				{
					table.SetRelativeColumWidth (column.Rank, column.Width);
					table.SetText               (column.Rank, row, column.Title);
				}
			}

			this.InitializeRowAlignment (table, row);

			row++;

			int linePage = this.documentContainer.CurrentPage;
			double lineY = this.documentContainer.CurrentVerticalPosition;

			foreach (var line in this.entity.Lines)
			{
				if (line.Visibility)
				{
					if (line is TextDocumentItemEntity)
					{
						this.BuildTextLine (table, row, line as TextDocumentItemEntity);
					}

					if (line is ArticleDocumentItemEntity)
					{
						this.BuildArticleLine (table, row, line as ArticleDocumentItemEntity);
					}

					if (line is PriceDocumentItemEntity)
					{
						this.BuildPriceLine (table, row, line as PriceDocumentItemEntity, lastLine: row == rowCount-1);
					}

					this.InitializeRowAlignment (table, row);
					row++;
				}
			}

			this.documentContainer.AddFromTop (table, 5.0);

			if (!this.IsModern)
			{
				var h = table.GetRowHeight (0);

				var line = new SurfaceBand ();
				line.Height = 0.3;
				var bounds = new Rectangle (this.PageMargins.Left, lineY-h, this.PageSize.Width-this.PageMargins.Left-this.PageMargins.Right, line.Height);

				this.documentContainer.CurrentPage = 0;
				this.documentContainer.AddAbsolute (line, bounds);
			}
		}


		private bool InitializeColumnTextLine(TextDocumentItemEntity line)
		{
			this.tableColumns["Desc"].Visible = true;
			return true;
		}

		private bool InitializeColumnArticleLine(ArticleDocumentItemEntity line)
		{
			if (this.IsBL && this.IsPort (line))
			{
				return false;
			}

			this.tableColumns["Desc"].Visible = true;
			this.tableColumns["Tot" ].Visible = true;

			if (line.VatCode != BusinessLogic.Finance.VatCode.None &&
				line.VatCode != BusinessLogic.Finance.VatCode.Excluded &&
				line.VatCode != BusinessLogic.Finance.VatCode.ZeroRated)
			{
				this.tableColumns["TVA"].Visible = true;
			}

			foreach (var quantity in line.ArticleQuantities)
			{
				if (quantity.Code == "livré")
				{
					this.tableColumns["Nb"].Visible = true;
					this.tableColumns["PU"].Visible = true;
					this.tableColumns["PT"].Visible = true;
				}

				if (quantity.Code == "suivra")
				{
					this.tableColumns["Suit"].Visible = true;
					this.tableColumns["Date"].Visible = true;
				}
			}

			if (line.Discounts.Count != 0)
			{
				this.tableColumns["Rab"].Visible = true;
			}

			return true;
		}

		private bool InitializeColumnPriceLine(PriceDocumentItemEntity line)
		{
			if (this.IsBL)
			{
				return false;
			}

			if (line.Discount.IsActive ())
			{
				this.InitializeColumnDiscountLine (line);
			}
			else
			{
				this.InitializeColumnTotalLine (line);
			}

			return true;
		}

		private void InitializeColumnDiscountLine(PriceDocumentItemEntity line)
		{
			if (line.PrimaryPriceBeforeTax.HasValue)
			{
				this.tableColumns["Rab"].Visible = true;
			}

			this.tableColumns["Desc"].Visible = true;
			this.tableColumns["PT"  ].Visible = true;
		}

		private void InitializeColumnTotalLine(PriceDocumentItemEntity line)
		{
			this.tableColumns["Desc"].Visible = true;
			this.tableColumns["TVA" ].Visible = true;
			this.tableColumns["Tot" ].Visible = true;
		}


		private void BuildTextLine(TableBand table, int row, TextDocumentItemEntity line)
		{
			string text = string.Concat ("<b>", line.Text, "</b>");
			table.SetText (this.tableColumns["Desc"].Rank, row, text);
		}

		private void BuildArticleLine(TableBand table, int row, ArticleDocumentItemEntity line)
		{
			if (this.IsBL && this.IsPort (line))
			{
				return;
			}

			string q1 = null;
			string q2 = null;
			string date = null;

			foreach (var quantity in line.ArticleQuantities)
			{
				if (quantity.Code == "livré")
				{
					q1 = Misc.FormatUnit (quantity.Quantity, quantity.Unit.Code);
				}

				if (quantity.Code == "suivra")
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
				table.SetText (this.tableColumns["Nb"].Rank, row, q1);
			}

			if (q2 != null)
			{
				table.SetText (this.tableColumns["Suit"].Rank, row, q2);
			}

			if (date != null)
			{
				table.SetText (this.tableColumns["Date"].Rank, row, date);
			}

			table.SetText (this.tableColumns["Desc"].Rank, row, description);
			table.SetText (this.tableColumns["PU"  ].Rank, row, Misc.PriceToString (line.PrimaryUnitPriceBeforeTax));

			if (line.ResultingLinePriceBeforeTax.HasValue && line.ResultingLineTax.HasValue)
			{
				decimal beforeTax = line.ResultingLinePriceBeforeTax.Value;
				decimal tax = line.ResultingLineTax.Value;

				table.SetText (this.tableColumns["PT" ].Rank, row, Misc.PriceToString (beforeTax));
				table.SetText (this.tableColumns["TVA"].Rank, row, Misc.PriceToString (tax));
				table.SetText (this.tableColumns["Tot"].Rank, row, Misc.PriceToString (beforeTax+tax));
			}

			if (line.Discounts.Count != 0)
			{
				if (line.Discounts[0].DiscountRate.HasValue)
				{
					table.SetText (this.tableColumns["Rab"].Rank, row, Misc.PercentToString (line.Discounts[0].DiscountRate.Value));
				}

				if (line.Discounts[0].DiscountAmount.HasValue)
				{
					table.SetText (this.tableColumns["Rab"].Rank, row, Misc.PriceToString (line.Discounts[0].DiscountAmount.Value));
				}
			}
		}

		private void BuildPriceLine(TableBand table, int row, PriceDocumentItemEntity line, bool lastLine)
		{
			if (this.IsBL)
			{
				return;
			}

			if (line.Discount.IsActive ())
			{
				this.BuildDiscountLine (table, row, line);
			}
			else
			{
				this.BuildTotalLine (table, row, line, lastLine);
			}
		}

		private void BuildDiscountLine(TableBand table, int row, PriceDocumentItemEntity line)
		{
			//	Génère les 2 lignes de description.
			string beforeText = line.TextForPrimaryPrice;
			if (string.IsNullOrEmpty (beforeText))
			{
				beforeText = "Total avant rabais";
			}

			string afterText = line.TextForResultingPrice;
			if (string.IsNullOrEmpty (afterText))
			{
				afterText = "Total après rabais";
			}

			string desc = string.Concat (beforeText, "<br/>", afterText);

			//	Génère les 2 lignes de prix.
			string beforePrice = null;
			if (line.PrimaryPriceBeforeTax.HasValue)
			{
				beforePrice = Misc.PriceToString (line.PrimaryPriceBeforeTax.Value);
			}

			string afterPrice = null;
			if (line.ResultingPriceBeforeTax.HasValue)
			{
				afterPrice = Misc.PriceToString (line.ResultingPriceBeforeTax.Value);
			}

			string prix = string.Concat (beforePrice, "<br/>", afterPrice);

			//	Génère les 2 lignes de rabais.
			string rabais = null;
			if (line.Discount.DiscountRate.HasValue)
			{
				rabais = string.Concat ("<br/>", Misc.PercentToString (line.Discount.DiscountRate.Value));
			}

			table.SetText (this.tableColumns["Desc"].Rank, row, desc);
			table.SetText (this.tableColumns["Rab" ].Rank, row, rabais);
			table.SetText (this.tableColumns["PT"  ].Rank, row, prix);
		}

		private void BuildTotalLine(TableBand table, int row, PriceDocumentItemEntity line, bool lastLine)
		{
			string desc = line.TextForFixedPrice;
			if (string.IsNullOrEmpty (desc))
			{
				desc = "Total arrêté";
			}

			string tva = null;
			if (line.ResultingTax.HasValue)
			{
				tva = Misc.PriceToString (line.ResultingTax.Value);
			}
			
			string total = null;
			if (line.FixedPriceAfterTax.HasValue)
			{
				total = Misc.PriceToString (line.FixedPriceAfterTax.Value);
			}

			if (lastLine)
			{
				desc  = string.Concat ("<b>", desc,  "</b>");
				tva   = string.Concat ("<b>", tva,   "</b>");
				total = string.Concat ("<b>", total, "</b>");
			}

			table.SetText (this.tableColumns["Desc"].Rank, row, desc);
			table.SetText (this.tableColumns["TVA" ].Rank, row, tva);
			table.SetText (this.tableColumns["Tot" ].Rank, row, total);

			if (lastLine)
			{
				table.SetCellBorderWidth (this.tableColumns["Tot"].Rank, row, 0.5);  // met un cadre épais
			}
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


		private void BuildConditions()
		{
			//	Met les conditions à la fin de la facture.
			if (this.IsBL)
			{
				return;
			}

			string conditions = InvoiceDocumentHelper.GetConditions (this.entity);

			if (!string.IsNullOrEmpty (conditions))
			{
				var band = new TextBand ();
				band.Text = conditions;

				this.documentContainer.AddFromTop (band, 0);
			}
		}

		private void BuildPages()
		{
			//	Met les numéros de page.
			var leftBounds  = new Rectangle (this.PageMargins.Left, this.PageSize.Height-this.PageMargins.Top+1, 80, 5);
			var rightBounds = new Rectangle (this.PageSize.Width-this.PageMargins.Right-80, this.PageSize.Height-this.PageMargins.Top+1, 80, 5);

			for (int page = 1; page < this.documentContainer.PageCount; page++)
			{
				this.documentContainer.CurrentPage = page;

				var leftHeader = new TextBand ();
				leftHeader.Text = InvoiceDocumentHelper.GetTitle (this.entity, this.IsBL);
				leftHeader.Alignment = ContentAlignment.BottomLeft;
				leftHeader.Font = font;
				leftHeader.FontSize = 4.0;

				var rightHeader = new TextBand ();
				rightHeader.Text = string.Format ("page {0}", (page+1).ToString ());
				rightHeader.Alignment = ContentAlignment.BottomRight;
				rightHeader.Font = font;
				rightHeader.FontSize = fontSize;

				this.documentContainer.AddAbsolute (leftHeader, leftBounds);
				this.documentContainer.AddAbsolute (rightHeader, rightBounds);
			}
		}

		private void BuildBvs(bool bvr)
		{
			//	Met un BVR orangé ou un BV rose en bas de chaque page.
			var bounds = new Rectangle (Point.Zero, AbstractBvBand.DefautlSize);

			for (int page = 0; page < this.documentContainer.PageCount; page++)
			{
				this.documentContainer.CurrentPage = page;

				AbstractBvBand BV;

				if (bvr)
				{
					BV = new BvrBand ();  // BVR orangé
				}
				else
				{
					BV = new BvBand ();  // BV rose
				}

				BV.PaintBvSimulator = this.HasDocumentOption ("BV.Simul");
				BV.PaintSpecimen    = this.HasDocumentOption ("BV.Spec");
				BV.From = InvoiceDocumentHelper.GetMailContact (this.entity);
				BV.To = "EPSITEC SA<br/>1400 Yverdon-les-Bains";
				BV.Communication = "En vous remerciant pour votre travail qui nous a rendu un très grand service !";

				if (page == this.documentContainer.PageCount-1)  // dernière page ?
				{
					BV.NotForUse = false;  // c'est LE vrai BV
					BV.Price = InvoiceDocumentHelper.GetAmontDue (this.entity);

					if (this.entity.BillingDetails.Count > 0)
					{
						BV.EsrCustomerNumber  = this.entity.BillingDetails[0].EsrCustomerNumber;
						BV.EsrReferenceNumber = this.entity.BillingDetails[0].EsrReferenceNumber;
					}
				}
				else  // faux BV ?
				{
					BV.NotForUse = true;  // pour imprimer "XXXXX XX"
				}

				this.documentContainer.AddAbsolute (BV, bounds);
			}
		}


		private bool IsPort(ArticleDocumentItemEntity article)
		{
			//	Retourne true s'il s'agit des frais de port.
			if (article.ArticleDefinition.IsActive () &&
				article.ArticleDefinition.ArticleCategory.IsActive ())
			{
				return article.ArticleDefinition.ArticleCategory.Name == "Ports/emballages";
			}

			return false;
		}

		private bool IsBL
		{
			get
			{
				return this.DocumentTypeSelected == "BL";
			}
		}

		private bool IsModern
		{
			get
			{
				return this.HasDocumentOption ("Modern");
			}
		}




		private static readonly Font font = Font.GetFont ("Arial", "Regular");
		private static readonly double fontSize = 3.0;
	}
}
