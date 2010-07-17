//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryInvoiceDocumentViewController : SummaryViewController<Entities.InvoiceDocumentEntity>
	{
		public SummaryInvoiceDocumentViewController(string name, Entities.InvoiceDocumentEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI(TileContainer container)
		{
			var containerController = new TileContainerController (this, container);
			var data = containerController.DataItems;

			this.CreateUIInvoiceDocument (data);

			containerController.GenerateTiles ();
		}

		protected override EditionStatus GetEditionStatus()
		{
			var entity = this.Entity;
			return EditionStatus.Valid;
		}

		protected override void UpdateEmptyEntityStatus(DataLayer.DataContext context, bool isEmpty)
		{
			var entity = this.Entity;
			
			context.UpdateEmptyEntityStatus (entity, isEmpty);
		}

		private void CreateUIInvoiceDocument(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "InvoiceDocument",
					IconUri				= "Data.Document",
					Title				= UIBuilder.FormatText ("Facture"),
					CompactTitle		= UIBuilder.FormatText ("Facture"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => SummaryInvoiceDocumentViewController.GetText (x)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => UIBuilder.FormatText ("N°", x.IdA)),
					EntityMarshaler		= this.EntityMarshaler,
				});
		}


		private static FormattedText GetText(InvoiceDocumentEntity x)
		{
			string date = SummaryInvoiceDocumentViewController.GetDate (x);
			string total = Misc.DecimalToString (SummaryInvoiceDocumentViewController.GetTotal (x));

			var builder = new System.Text.StringBuilder ();

			foreach (var line in x.Lines)
			{
				if (line.Visibility)
				{
					if (line is ArticleDocumentItemEntity)
					{
						var quantity = SummaryInvoiceDocumentViewController.GetArticleQuantity (line as ArticleDocumentItemEntity);
						var desc     = SummaryInvoiceDocumentViewController.GetArticleDescription (line as ArticleDocumentItemEntity, shortDescription: true);

						builder.Append ("● ");
						builder.Append (string.Join (" ", quantity, desc));
						builder.Append ("<br/>");
					}
				}
			}
	
			return UIBuilder.FormatText ("N°", x.IdA, "/~", x.IdB, "/~", x.IdC, ", ", date, "\n", builder.ToString (), "Total: ", total);
		}


		public static string GetDate(InvoiceDocumentEntity x)
		{
			System.DateTime date;
			if (x.LastModificationDate.HasValue)
			{
				date = x.LastModificationDate.Value;
			}
			else
			{
				date = System.DateTime.Now;
			}

			return Misc.GetDateTimeDescription (date);
		}

		public static string GetMailContact(InvoiceDocumentEntity x)
		{
			string legal = "";
			string natural = "";

			if (x.BillingMailContact != null)
			{
				if (x.BillingMailContact.LegalPerson.IsActive ())
				{
					var y = x.BillingMailContact.LegalPerson;
					legal = UIBuilder.FormatText (y.Name).ToString ();
				}

				if (x.BillingMailContact.NaturalPerson.IsActive ())
				{
					var y = x.BillingMailContact.NaturalPerson;
					natural = UIBuilder.FormatText (y.Title.Name, "~\n", y.Firstname, y.Lastname).ToString ();
				}

				return UIBuilder.FormatText (legal, "~\n", natural, "~\n", x.BillingMailContact.Address.Street.StreetName, "\n", x.BillingMailContact.Address.Location.PostalCode, x.BillingMailContact.Address.Location.Name).ToString ();
			}

			return null;
		}

		public static decimal GetArticlePrice(ArticleDocumentItemEntity article)
		{
			if (article.ArticleDefinition.ArticlePrices.Count != 0)
			{
				return article.ArticleDefinition.ArticlePrices[0].ValueBeforeTax;
			}

			return 0;
		}

		public static string GetArticleQuantity(ArticleDocumentItemEntity article)
		{
			foreach (var quantity in article.ArticleQuantities)
			{
				if (quantity.Code != "suivra")
				{
					return Misc.FormatUnit (quantity.Quantity, quantity.Unit.Code);
				}
			}

			return null;
		}

		public static string GetArticleDescription(ArticleDocumentItemEntity article, bool shortDescription)
		{
			string description = shortDescription ? null : article.ArticleDefinition.LongDescription;

			if (string.IsNullOrEmpty (description))
			{
				description = article.ArticleDefinition.ShortDescription;  // description courte s'il n'existe pas de longue
			}

			return description;
		}

		public static string GetConcerne(InvoiceDocumentEntity x)
		{
			if (x.BillingDetails.Count > 0)
			{
				return x.BillingDetails[0].Title;
			}

			return null;
		}

		public static string GetConditions(InvoiceDocumentEntity x)
		{
			if (x.BillingDetails.Count > 0)
			{
				return x.BillingDetails[0].AmountDue.PaymentMode.Description;
			}

			return null;
		}

		public static decimal GetTotal(InvoiceDocumentEntity x)
		{
			if (x.BillingDetails.Count > 0)
			{
				return Misc.CentRound (x.BillingDetails[0].AmountDue.Amount);
			}

			return 0;
		}

		public static string GetTitle(InvoiceDocumentEntity x)
		{
			return UIBuilder.FormatText ("<b>Facture", x.IdA, "/~", x.IdB, "/~", x.IdC, "</b>").ToString ();
		}
	
	
	}
}
