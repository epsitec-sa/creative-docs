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
				return new Margins (20, 15, 10, 110);
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
			var image = new ImageBand ();
			image.Load("logo-cresus.png");
			image.BuildSections (60, 50, 50, 50);
			this.documentContainer.AddAbsolute (image, new Rectangle (20, 297-10-50, 60, 50));

			var mailContact = new TextBand ();
			mailContact.Text = this.MailContact;
			mailContact.Font = font;
			mailContact.FontSize = fontSize;
			this.documentContainer.AddAbsolute (mailContact, new Rectangle (120, 240, 80, 25));

			string textConcerne = this.Concerne;
			if (!string.IsNullOrEmpty (textConcerne))
			{
				var concerne = new TableBand ();
				concerne.ColumnsCount = 2;
				concerne.RowsCount = 1;
				concerne.PaintFrame = false;
				concerne.Font = font;
				concerne.FontSize = fontSize;
				concerne.CellMargins = new Margins (0);
				concerne.SetRelativeColumWidth (0, 15);
				concerne.SetRelativeColumWidth (1, 80);
				concerne.SetText (0, 0, "Concerne");
				concerne.SetText (1, 0, textConcerne);
				this.documentContainer.AddAbsolute (concerne, new Rectangle (20, 230, 100, 15));
			}

			var title = new TextBand ();
			title.Text = UIBuilder.FormatText ("<b>Facture", this.entity.IdA, "/~", this.entity.IdB, "</b>").ToString ();
			title.Font = font;
			title.FontSize = 5.0;
			this.documentContainer.AddAbsolute (title, new Rectangle (20, 215, 90, 10));

			System.DateTime dt;
			if (this.entity.LastModificationDate.HasValue)
			{
				dt = this.entity.LastModificationDate.Value;
			}
			else
			{
				dt = System.DateTime.Now;
			}

			var date = new TextBand ();
			date.Text = UIBuilder.FormatText ("Crissier, le ", Misc.GetDateTimeShortDescription (dt)).ToString ();
			date.Font = font;
			date.FontSize = fontSize;
			this.documentContainer.AddAbsolute (date, new Rectangle (120, 215, 80, 10));
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
						this.BuildLineText (table, row, line as TextDocumentItemEntity);
					}

					if (line is ArticleDocumentItemEntity)
					{
						this.BuildLineArticle (table, row, line as ArticleDocumentItemEntity);
					}

					if (line is PriceDocumentItemEntity)
					{
						this.BuildLinePrice (table, row, line as PriceDocumentItemEntity);
					}

					row++;
				}
			}

			this.documentContainer.AddFromTop (table, 5.0);
		}

		private void BuildLineText(TableBand table, int row, TextDocumentItemEntity line)
		{
			table.SetText (2, row, line.Text);
		}

		private void BuildLineArticle(TableBand table, int row, ArticleDocumentItemEntity line)
		{
			decimal quantity = this.ArticleQuantity (line);
			decimal price    = this.ArticlePrice (line);

			if (quantity == 0.0M)  // les frais de port ont une quantité nulle !
			{
				quantity = 1.0M;
			}

			string description = line.ArticleDefinition.LongDescription;
			if (string.IsNullOrEmpty (description))
			{
				description = line.ArticleDefinition.ShortDescription;
			}

			table.SetText (0, row, quantity.ToString ());
			table.SetText (1, row, this.ArticleUnit (line));
			table.SetText (2, row, description);
			table.SetText (3, row, price.ToString ());
			table.SetText (4, row, (quantity*price).ToString ());

			table.SetAlignment (0, row, ContentAlignment.MiddleRight);
			table.SetAlignment (3, row, ContentAlignment.MiddleRight);
			table.SetAlignment (4, row, ContentAlignment.MiddleRight);
		}

		private void BuildLinePrice(TableBand table, int row, PriceDocumentItemEntity line)
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
				BV.From = this.MailContact;
				BV.To = "EPSITEC SA<br/>1400 Yverdon-les-Bains";

				if (page == this.documentContainer.PageCount-1)  // dernière page ?
				{
					BV.NotForUse = false;  // c'est LE vrai BV
					BV.Price = this.Total;

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


		private string MailContact
		{
			get
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
		}

		private decimal ArticleQuantity(ArticleDocumentItemEntity article)
		{
			if (article.ArticleQuantities.Count != 0)
			{
				return article.ArticleQuantities[0].Quantity;
			}

			return 0;
		}

		private string ArticleUnit(ArticleDocumentItemEntity article)
		{
			if (article.ArticleQuantities.Count != 0)
			{
				return article.ArticleQuantities[0].Unit.Name;
			}

			return null;
		}

		private decimal ArticlePrice(ArticleDocumentItemEntity article)
		{
			if (article.ArticleDefinition.ArticlePrices.Count != 0)
			{
				return article.ArticleDefinition.ArticlePrices[0].ValueBeforeTax;
			}

			return 0;
		}

		private string Concerne
		{
			get
			{
				if (this.entity.BillingDetails.Count > 0)
				{
					return this.entity.BillingDetails[0].Title;
				}

				return null;
			}
		}

		private decimal Total
		{
			get
			{
				if (this.entity.BillingDetails.Count > 0)
				{
					return this.entity.BillingDetails[0].AmountDue.Amount;
				}

				return 0;
			}
		}



		private static readonly double pageWidth  = 210;
		private static readonly double pageHeight = 297;  // A4 vertical

		private static readonly Font font = Font.GetFont ("Arial", "Regular");
		private static readonly double fontSize = 3.0;
	}
}
