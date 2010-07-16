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
			this.columns = new Dictionary<string, TableColumn> ();
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
				return new Size (pageWidth, pageHeight);  // A4 vertical
			}
		}

		public override Margins PageMargins
		{
			get
			{
				return new Margins (20, 10, 10, AbstractBvBand.DefautlSize.Height+10);
			}
		}

		public override void BuildSections()
		{
			this.BuildHeader ();
			this.BuildArticles ();
			this.BuildBvs (bvr: false);
		}

		public override void PrintCurrentPage(IPaintPort port, Rectangle bounds)
		{
			this.documentContainer.Paint (port, this.CurrentPage);
		}


		private void BuildHeader()
		{
			//	Ajoute l'en-tête de la facture dans le document.
			var imageBand = new ImageBand ();
			imageBand.Load("logo-cresus.png");
			imageBand.BuildSections (60, 50, 50, 50);
			this.documentContainer.AddAbsolute (imageBand, new Rectangle (20, 297-10-50, 60, 50));

			var textBand = new TextBand ();
			textBand.Text = "<b>Les logiciels de gestion</b>";
			textBand.Font = font;
			textBand.FontSize = 5.0;
			this.documentContainer.AddAbsolute (textBand, new Rectangle (20, 297-10-imageBand.GetSectionHeight (0)-10, 80, 10));

			var mailContactBand = new TextBand ();
			mailContactBand.Text = this.GetMailContact ();
			mailContactBand.Font = font;
			mailContactBand.FontSize = fontSize;
			this.documentContainer.AddAbsolute (mailContactBand, new Rectangle (120, 240, 80, 25));

			string concerne = this.GetConcerne ();
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
				this.documentContainer.AddAbsolute (concerneBand, new Rectangle (20, 230, 100, 15));
			}

			var titleBand = new TextBand ();
			titleBand.Text = UIBuilder.FormatText ("<b>Facture", this.entity.IdA, "/~", this.entity.IdB, "/~", this.entity.IdC, "</b>").ToString ();
			titleBand.Font = font;
			titleBand.FontSize = 5.0;
			this.documentContainer.AddAbsolute (titleBand, new Rectangle (20, 215, 90, 10));

			string date = this.GetDate ();
			var dateBand = new TextBand ();
			dateBand.Text = UIBuilder.FormatText ("Crissier, le ", date).ToString ();
			dateBand.Font = font;
			dateBand.FontSize = fontSize;
			this.documentContainer.AddAbsolute (dateBand, new Rectangle (120, 215, 80, 10));
		}

		private void BuildArticles()
		{
			//	Ajoute les articles dans le document.
			this.documentContainer.CurrentVerticalPosition = 210;

			this.columns.Clear ();
			this.columns.Add ("Desc", new TableColumn ("Désignation", 70.0, ContentAlignment.MiddleLeft));
			this.columns.Add ("Nb",   new TableColumn ("Livré",       15.0, ContentAlignment.MiddleLeft));
			this.columns.Add ("Suit", new TableColumn ("Suit",        15.0, ContentAlignment.MiddleLeft));
			this.columns.Add ("Date", new TableColumn ("Date",        20.0, ContentAlignment.MiddleCenter));
			this.columns.Add ("Rab",  new TableColumn ("Rabais",      15.0, ContentAlignment.MiddleRight));
			this.columns.Add ("PU",   new TableColumn ("p.u. HT",     15.0, ContentAlignment.MiddleRight));
			this.columns.Add ("PT",   new TableColumn ("Prix HT",     15.0, ContentAlignment.MiddleRight));
			this.columns.Add ("TVA",  new TableColumn ("TVA",         15.0, ContentAlignment.MiddleRight));
			this.columns.Add ("Tot",  new TableColumn ("Total",       15.0, ContentAlignment.MiddleRight));

			//	Première passe pour déterminer le nombre le lignes du tableau de la facture
			//	ainsi que les colonnes visibles.
			int rowCount = 1;  // déjà 1 pour l'en-tête (titres des colonnes)

			foreach (var line in this.entity.Lines)
			{
				if (line.Visibility)
				{
					if (line is TextDocumentItemEntity)
					{
						this.InitializeColumnTextLine (line as TextDocumentItemEntity);
					}

					if (line is ArticleDocumentItemEntity)
					{
						this.InitializeColumnArticleLine (line as ArticleDocumentItemEntity);
					}

					if (line is PriceDocumentItemEntity)
					{
						this.InitializeColumnPriceLine (line as PriceDocumentItemEntity);
					}

					rowCount++;
				}
			}

			//	Compte et numérote les colonnes visibles.
			int columnCount = 0;

			foreach (var column in this.columns.Values)
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

			int row = 0;

			//	Génère une première ligne d'en-tête (titres des colonnes).
			foreach (var column in this.columns.Values)
			{
				if (column.Visible)
				{
					table.SetRelativeColumWidth (column.Rank, column.Width);
					table.SetText               (column.Rank, row, column.Title);
				}
			}

			this.InitializeTableAlignment (table, row);

			row++;

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

					this.InitializeTableAlignment (table, row);
					row++;
				}
			}

			this.documentContainer.AddFromTop (table, 5.0);
		}


		private void InitializeColumnTextLine(TextDocumentItemEntity line)
		{
			this.columns["Desc"].Visible = true;
		}

		private void InitializeColumnArticleLine(ArticleDocumentItemEntity line)
		{
			this.columns["Desc"].Visible = true;
			this.columns["Tot" ].Visible = true;

			if (line.VatCode != BusinessLogic.Finance.VatCode.None &&
				line.VatCode != BusinessLogic.Finance.VatCode.Excluded &&
				line.VatCode != BusinessLogic.Finance.VatCode.ZeroRated)
			{
				this.columns["TVA"].Visible = true;
			}

			foreach (var quantity in line.ArticleQuantities)
			{
				if (quantity.Code == "suivra")
				{
					this.columns["Suit"].Visible = true;
					this.columns["Date"].Visible = true;
				}
				else
				{
					this.columns["Nb"].Visible = true;
					this.columns["PU"].Visible = true;
					this.columns["PT"].Visible = true;
				}
			}
		}

		private void InitializeColumnPriceLine(PriceDocumentItemEntity line)
		{
			this.columns["Desc"].Visible = true;
			this.columns["Tot" ].Visible = true;
			this.columns["PU"  ].Visible = true;
			this.columns["PT"  ].Visible = true;

			if (line.Discount.IsActive ())
			{
				this.columns["Rab"].Visible = true;
			}
		}


		private void BuildTextLine(TableBand table, int row, TextDocumentItemEntity line)
		{
			string text = string.Concat ("<b>", line.Text, "</b>");
			table.SetText (this.columns["Desc"].Rank, row, text);
		}

		private void BuildArticleLine(TableBand table, int row, ArticleDocumentItemEntity line)
		{
			string q1 = null;
			string q2 = null;
			string date = null;

			foreach (var quantity in line.ArticleQuantities)
			{
				if (quantity.Code == "suivra")
				{
					q2 = Misc.AppendLine (q2, Misc.FormatUnit (quantity.Quantity, quantity.Unit.Name));

					if (quantity.ExpectedDate.HasValue)
					{
						date = Misc.AppendLine(date, quantity.ExpectedDate.Value.ToString ());
					}
				}
				else
				{
					q1 = Misc.FormatUnit (quantity.Quantity, quantity.Unit.Name);
				}
			}

			decimal price       = this.GetArticlePrice       (line);
			string  description = this.GetArticleDescription (line);

			if (q1 != null)
			{
				table.SetText (this.columns["Nb"].Rank, row, q1);
			}

			if (q2 != null)
			{
				table.SetText (this.columns["Suit"].Rank, row, q2);
			}

			if (date != null)
			{
				table.SetText (this.columns["Date"].Rank, row, date);
			}

			table.SetText (this.columns["Desc"].Rank, row, description);
			table.SetText (this.columns["PU"  ].Rank, row, Misc.DecimalToString (line.PrimaryUnitPriceBeforeTax));

			if (line.ResultingLinePriceBeforeTax.HasValue && line.ResultingLineTax.HasValue)
			{
				decimal beforeTax = line.ResultingLinePriceBeforeTax.Value;
				decimal tax = line.ResultingLineTax.Value;

				table.SetText (this.columns["PT" ].Rank, row, Misc.DecimalToString (beforeTax));
				table.SetText (this.columns["TVA"].Rank, row, Misc.DecimalToString (tax));
				table.SetText (this.columns["Tot"].Rank, row, Misc.DecimalToString (beforeTax+tax));
			}

			this.InitializeTableAlignment (table, row);
		}

		private void BuildPriceLine(TableBand table, int row, PriceDocumentItemEntity line, bool lastLine)
		{
			decimal p1 = Misc.CentRound (line.PrimaryPriceBeforeTax.Value);
			decimal p2 = Misc.CentRound (line.ResultingPriceBeforeTax.Value);

			string price = Misc.DecimalToString (p2);

			if (lastLine)
			{
				price = string.Concat ("<b>", price, "</b>");
			}

			table.SetText (this.columns["Desc"].Rank, row, line.TextForPrimaryPrice);
			table.SetText (this.columns["PU"  ].Rank, row, Misc.DecimalToString (p1));
			table.SetText (this.columns["PT"  ].Rank, row, price);

			if (lastLine)
			{
				table.SetCellBorderWidth (this.columns["Tot"].Rank, row, 0.5);  // met un cadre épais
			}
		}


		private void InitializeTableAlignment(TableBand table, int row)
		{
			foreach (var column in this.columns.Values)
			{
				if (column.Visible)
				{
					table.SetAlignment (column.Rank, row, column.Alignment);
				}
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

				BV.PaintBvSimulator = true;
				BV.From = this.GetMailContact ();
				BV.To = "EPSITEC SA<br/>1400 Yverdon-les-Bains";
				BV.Communication = "En vous remerciant pour votre travail qui nous a rendu un très grand service !";

				if (page == this.documentContainer.PageCount-1)  // dernière page ?
				{
					BV.NotForUse = false;  // c'est LE vrai BV
					BV.Price = this.GetTotal ();

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


		private string GetDate()
		{
			System.DateTime date;
			if (this.entity.LastModificationDate.HasValue)
			{
				date = this.entity.LastModificationDate.Value;
			}
			else
			{
				date = System.DateTime.Now;
			}

			return Misc.GetDateTimeDescription (date);
		}

		private string GetMailContact()
		{
			string legal = "";
			string natural = "";

			if (this.entity.BillingMailContact != null)
			{
				if (this.entity.BillingMailContact.LegalPerson.IsActive ())
				{
					var x = this.entity.BillingMailContact.LegalPerson;
					legal = UIBuilder.FormatText (x.Name).ToString ();
				}

				if (this.entity.BillingMailContact.NaturalPerson.IsActive ())
				{
					var x = this.entity.BillingMailContact.NaturalPerson;
					natural = UIBuilder.FormatText (x.Title.Name, "~\n", x.Firstname, x.Lastname).ToString ();
				}

				return UIBuilder.FormatText (legal, "~\n", natural, "~\n", this.entity.BillingMailContact.Address.Street.StreetName, "\n", this.entity.BillingMailContact.Address.Location.PostalCode, this.entity.BillingMailContact.Address.Location.Name).ToString ();
			}

			return null;
		}

		private decimal GetArticlePrice(ArticleDocumentItemEntity article)
		{
			if (article.ArticleDefinition.ArticlePrices.Count != 0)
			{
				return article.ArticleDefinition.ArticlePrices[0].ValueBeforeTax;
			}

			return 0;
		}

		private string GetArticleDescription(ArticleDocumentItemEntity article)
		{
			string description = article.ArticleDefinition.LongDescription;

			if (string.IsNullOrEmpty (description))
			{
				description = article.ArticleDefinition.ShortDescription;  // description courte s'il n'existe pas de longue
			}

			return description;
		}

		private string GetConcerne()
		{
			if (this.entity.BillingDetails.Count > 0)
			{
				return this.entity.BillingDetails[0].Title;
			}

			return null;
		}

		private decimal GetTotal()
		{
			if (this.entity.BillingDetails.Count > 0)
			{
				return Misc.CentRound (this.entity.BillingDetails[0].AmountDue.Amount);
			}

			return 0;
		}



		private class TableColumn
		{
			public TableColumn(string title, double width, ContentAlignment alignment)
			{
				this.Title     = title;
				this.Width     = width;
				this.Alignment = alignment;
				this.Rank      = -1;
				this.Visible   = false;
			}

			public string			Title;
			public double			Width;
			public ContentAlignment	Alignment;
			public int				Rank;
			public bool				Visible;
		}



		private static readonly double pageWidth  = 210;
		private static readonly double pageHeight = 297;  // A4 vertical

		private static readonly Font font = Font.GetFont ("Arial", "Regular");
		private static readonly double fontSize = 3.0;

		private Dictionary<string, TableColumn> columns;
	}
}
