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
				return new Margins (20, 15, 10, BvBand.DefautlSize.Height+10);
			}
		}

		public override void BuildSections()
		{
			this.BuildHeader ();
			this.BuildArticles ();
			this.BuildBvs ();
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

			//	Première passe pour déterminer le nombre le lignes du tableau de la facture.
			int rowCount = 1;  // déjà 1 pour l'en-tête

			foreach (var line in this.entity.Lines)
			{
				if (line.Visibility)
				{
					rowCount++;
				}
			}

			//	Deuxième passe pour générer les lignes du tableau.
			var table = new TableBand ();
			table.ColumnsCount = 5;
			table.RowsCount = rowCount;

			table.SetRelativeColumWidth (0, 15.0);
			table.SetRelativeColumWidth (1, 12.0);
			table.SetRelativeColumWidth (2, 90.0);
			table.SetRelativeColumWidth (3, 20.0);
			table.SetRelativeColumWidth (4, 20.0);

			int row = 0;

			table.SetText (0, row, "Quantité");
			table.SetText (1, row, "Unité");
			table.SetText (2, row, "Désignation");
			table.SetText (3, row, "Prix unitaire");
			table.SetText (4, row, "Total");

			table.SetAlignment (0, row, ContentAlignment.MiddleRight);
			table.SetAlignment (3, row, ContentAlignment.MiddleRight);
			table.SetAlignment (4, row, ContentAlignment.MiddleRight);

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
						this.BuildPriceLine (table, row, line as PriceDocumentItemEntity);
					}

					row++;
				}
			}

			this.documentContainer.AddFromTop (table, 5.0);
		}

		private void BuildTextLine(TableBand table, int row, TextDocumentItemEntity line)
		{
			table.SetText (2, row, line.Text);
		}

		private void BuildArticleLine(TableBand table, int row, ArticleDocumentItemEntity line)
		{
			decimal quantity    = this.GetArticleQuantity    (line);
			string  unit        = this.GetArticleUnit        (line);
			decimal price       = this.GetArticlePrice       (line);
			string  description = this.GetArticleDescription (line);

			table.SetText (0, row, quantity.ToString ());
			table.SetText (1, row, unit);
			table.SetText (2, row, description);
			table.SetText (3, row, price.ToString ());
			table.SetText (4, row, (quantity*price).ToString ());

			table.SetAlignment (0, row, ContentAlignment.MiddleRight);
			table.SetAlignment (3, row, ContentAlignment.MiddleRight);
			table.SetAlignment (4, row, ContentAlignment.MiddleRight);
		}

		private void BuildPriceLine(TableBand table, int row, PriceDocumentItemEntity line)
		{
			table.SetText (2, row, line.TextForPrimaryPrice);
			table.SetText (3, row, line.PrimaryPriceBeforeTax.ToString ());
			table.SetText (4, row, line.ResultingPriceBeforeTax.ToString ());

			table.SetAlignment (3, row, ContentAlignment.MiddleRight);
			table.SetAlignment (4, row, ContentAlignment.MiddleRight);
		}


		private void BuildBvs()
		{
			//	Met un BV en bas de chaque page.
			var bounds = new Rectangle (Point.Zero, BvBand.DefautlSize);

			for (int page = 0; page < this.documentContainer.PageCount; page++)
			{
				this.documentContainer.CurrentPage = page;

				var BV = new BvBand ();

				BV.PaintBvSimulator = true;
				BV.From = this.GetMailContact ();
				BV.To = "EPSITEC SA<br/>1400 Yverdon-les-Bains";

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

			return Misc.GetDateTimeShortDescription (date);
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

		private decimal GetArticleQuantity(ArticleDocumentItemEntity article)
		{
			if (article.ArticleQuantities.Count != 0)
			{
				decimal quantity = article.ArticleQuantities[0].Quantity;

				if (quantity == 0.0M)  // les frais de port ont une quantité nulle !
				{
					quantity = 1.0M;
				}

				return quantity;
			}

			return 0;
		}

		private string GetArticleUnit(ArticleDocumentItemEntity article)
		{
			if (article.ArticleQuantities.Count != 0)
			{
				return article.ArticleQuantities[0].Unit.Name;
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
				return this.entity.BillingDetails[0].AmountDue.Amount;
			}

			return 0;
		}



		private static readonly double pageWidth  = 210;
		private static readonly double pageHeight = 297;  // A4 vertical

		private static readonly Font font = Font.GetFont ("Arial", "Regular");
		private static readonly double fontSize = 3.0;
	}
}
