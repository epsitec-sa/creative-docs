//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Helpers;

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


		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIInvoice      (data);
				this.CreateUIArticleLines (data);
				this.CreateUIFreightLines (data);
				this.CreateUITotalLines   (data);
				this.CreateUIBillings     (data);
				this.CreateUIComments     (data);
			}
		}


		private void CreateUIInvoice(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "InvoiceDocument",
					IconUri				= "Data.InvoiceDocument",
					Title				= TextFormatter.FormatText ("Document"),
					CompactTitle		= TextFormatter.FormatText ("Document"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => InvoiceDocumentHelper.GetSummary (x)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => TextFormatter.FormatText ("N°", x.IdA)),
					EntityMarshaler		= this.EntityMarshaler,
				});
		}

		private void CreateUIArticleLines(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "ArticleDocumentItem",
					IconUri		 = "Data.DocumentItems",
					Title		 = TextFormatter.FormatText ("Lignes"),
					CompactTitle = TextFormatter.FormatText ("Lignes"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<AbstractDocumentItemEntity> ("ArticleDocumentItem", data.Controller, this.DataContext);

			template.DefineText           (x => TextFormatter.FormatText (GetDocumentItemSummary (x)));
			template.DefineCompactText    (x => TextFormatter.FormatText (GetDocumentItemSummary (x)));
			template.DefineCreateItem     (this.CreateArticleDocumentItem);  // le bouton [+] crée une ligne d'article
			template.DefineCreateGetIndex (this.CreateArticleGetIndex);
			template.Filter = ArticleLineFilter;

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Lines, template));
		}

		private void CreateUIFreightLines(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "FreightDocumentItem",
					IconUri		 = "Data.FreightDocumentItem",
					Title		 = TextFormatter.FormatText ("Port et emballage"),
					CompactTitle = TextFormatter.FormatText ("Port et emballage"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<AbstractDocumentItemEntity> ("FreightDocumentItem", data.Controller, this.DataContext);

			template.DefineText           (x => TextFormatter.FormatText (GetDocumentItemSummary (x)));
			template.DefineCompactText    (x => TextFormatter.FormatText (GetDocumentItemSummary (x)));
			template.DefineCreateItem     (this.CreateArticleDocumentItem);  // le bouton [+] crée une ligne d'article
			template.DefineCreateGetIndex (this.CreateArticleGetIndex);
			template.Filter = FreightLineFilter;

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Lines, template));
		}

		private void CreateUITotalLines(SummaryDataItems data)
		{
#if false
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "TotalDocumentItem",
					IconUri		 = "Data.TotalDocumentItem",
					Title		 = TextFormatter.FormatText ("Total"),
					CompactTitle = TextFormatter.FormatText ("Total"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<AbstractDocumentItemEntity> ("TotalDocumentItem", data.Controller, this.DataContext);

			template.DefineText        (x => TextFormatter.FormatText (GetDocumentItemSummary (x)));
			template.DefineCompactText (x => TextFormatter.FormatText (GetDocumentItemSummary (x)));
			template.Filter = TotalLineFilter;

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.Lines, template));
#else
			var builder = new UIBuilder (this);

			builder.CreateEditionTitleTile ("Data.TotalDocumentItem", "Total");
			builder.CreateSummaryTile ("Coucou", this.Entity, TextFormatter.FormatText ("Tralala"));
#endif
		}

		private void CreateUIBillings(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "BillingDetails",
					IconUri		 = "Data.BillingDetails",
					Title		 = TextFormatter.FormatText ("Facturation"),
					CompactTitle = TextFormatter.FormatText ("Facturation"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<BillingDetailEntity> ("BillingDetails", data.Controller, this.DataContext);
			
			template.DefineText        (x => TextFormatter.FormatText (GetBillingDetailsSummary (this.Entity, x)));
			template.DefineCompactText (x => TextFormatter.FormatText (GetBillingDetailsSummary (this.Entity, x)));
			template.DefineSetupItem   (SetupBillingDetails);

			data.Add (CollectionAccessor.Create (this.EntityGetter, x => x.BillingDetails, template));
		}

		private void CreateUIComments(SummaryDataItems data)
		{
			SummaryControllers.Common.CreateUIComments (this.DataContext, data, this.EntityGetter, x => x.Comments);
		}


		private ArticleDocumentItemEntity CreateArticleDocumentItem()
		{
			//	Crée un nouvelle ligne dans la facture du type le plus courant, c'est-à-dire ArticleDocumentItemEntity.
			var article = this.DataContext.CreateEmptyEntity<ArticleDocumentItemEntity> ();

			article.Visibility = true;
			article.BeginDate  = this.Entity.CreationDate;
			article.EndDate    = this.Entity.CreationDate;

			return article;
		}

		private int CreateArticleGetIndex()
		{
			//	Retourne l'index où insérer la nouvelle ligne d'article créée avec le bouton [+].
			int index = this.Entity.Lines.Count;  // insère à la fin par défaut

			while (index > 0)
			{
				var line = this.Entity.Lines[index-1];

				if (line is TotalDocumentItemEntity ||
					line is TaxDocumentItemEntity   )
				{
					index--;  // insère avant le total
					continue;
				}

				if (ArticleDocumentItemHelper.IsFixedTax (line as ArticleDocumentItemEntity))
				{
					index--;  // insère avant les frais de port
					continue;
				}

				break;
			}

			return index;
		}

		private static bool ArticleLineFilter(AbstractDocumentItemEntity x)
		{
			return (x is TextDocumentItemEntity    ||
				    x is ArticleDocumentItemEntity ||
				    x is PriceDocumentItemEntity) &&
					!FreightLineFilter (x);
		}

		private static bool FreightLineFilter(AbstractDocumentItemEntity x)
		{
			if (x is ArticleDocumentItemEntity)
			{
				var article = x as ArticleDocumentItemEntity;

				return article.ArticleDefinition.ArticleCategory.ArticleType == BusinessLogic.ArticleType.Freight;
			}

			return false;
		}

		private static bool TotalLineFilter(AbstractDocumentItemEntity x)
		{
			return x is TotalDocumentItemEntity;
		}


		private static string GetDocumentItemSummary(AbstractDocumentItemEntity documentItemEntity)
		{
			if (documentItemEntity is TextDocumentItemEntity)
			{
				return GetTextDocumentItemSummary (documentItemEntity as TextDocumentItemEntity);
			}

			if (documentItemEntity is ArticleDocumentItemEntity)
			{
				return GetArticleDocumentItemSummary (documentItemEntity as ArticleDocumentItemEntity);
			}

			if (documentItemEntity is PriceDocumentItemEntity)
			{
				return GetPriceDocumentItemSummary (documentItemEntity as PriceDocumentItemEntity);
			}

			if (documentItemEntity is TaxDocumentItemEntity)
			{
				return GetTaxDocumentItemSummary (documentItemEntity as TaxDocumentItemEntity);
			}

			if (documentItemEntity is TotalDocumentItemEntity)
			{
				return GetTotalDocumentItemSummary (documentItemEntity as TotalDocumentItemEntity);
			}

			return null;
		}

		private static string GetTextDocumentItemSummary(TextDocumentItemEntity x)
		{
			if (string.IsNullOrEmpty (x.Text))
			{
				return "<i>Texte</i>";
			}
			else
			{
				return x.Text;
			}
		}

		private static string GetArticleDocumentItemSummary(ArticleDocumentItemEntity x)
		{
			var quantity = ArticleDocumentItemHelper.GetArticleQuantityAndUnit (x);
			var desc = Misc.FirstLine (ArticleDocumentItemHelper.GetArticleDescription (x));
			var price = Misc.PriceToString (x.PrimaryLinePriceBeforeTax);

			string text = string.Join (" ", quantity, desc, price);

			if (string.IsNullOrEmpty (text))
			{
				return "<i>Article</i>";
			}
			else
			{
				return text;
			}
		}

		private static string GetPriceDocumentItemSummary(PriceDocumentItemEntity x)
		{
			var builder = new System.Text.StringBuilder ();

			builder.Append ("Sous-total ");
			builder.Append (Misc.PriceToString (x.ResultingPriceBeforeTax));

			if (x.Discount.DiscountRate.HasValue)
			{
				builder.Append (" (après rabais en %)");
			}
			else if (x.Discount.DiscountAmount.HasValue)
			{
				builder.Append (" (après rabais en francs)");
			}
			else if (x.FixedPriceAfterTax.HasValue)
			{
				builder.Append (" (montant arrêté)");
			}

			return builder.ToString ();
		}

		private static string GetTaxDocumentItemSummary(TaxDocumentItemEntity x)
		{
			var desc = x.Text;
			var tax = Misc.PriceToString (x.ResultingTax);

			string text = string.Join (" ", desc, tax);

			if (string.IsNullOrEmpty (text))
			{
				return "<i>TVA</i>";
			}
			else
			{
				return text;
			}
		}

		private static string GetTotalDocumentItemSummary(TotalDocumentItemEntity x)
		{
			var desc = x.TextForPrimaryPrice;

			string total;
			if (x.PrimaryPriceBeforeTax.HasValue)
			{
				total = Misc.PriceToString (x.PrimaryPriceBeforeTax);
			}
			else if (x.FixedPriceAfterTax.HasValue)
			{
				total = Misc.PriceToString (x.FixedPriceAfterTax);
			}
			else
			{
				total = Misc.PriceToString (x.PrimaryPriceAfterTax);
			}

			string text = string.Join (" ", desc, total);

			if (string.IsNullOrEmpty (text))
			{
				return "<i>Total</i>";
			}
			else
			{
				return text;
			}
		}


		private static string GetBillingDetailsSummary(InvoiceDocumentEntity invoiceDocument, BillingDetailEntity billingDetails)
		{
			string amount = Misc.PriceToString (billingDetails.AmountDue.Amount);
			string title = Misc.FirstLine (billingDetails.Title);
			string ratio = InvoiceDocumentHelper.GetInstalmentName (invoiceDocument, billingDetails, true, false);

			if (ratio == null)
			{
				return TextFormatter.FormatText (amount, title).ToString ();
			}
			else
			{
				return TextFormatter.FormatText (amount, ratio, title).ToString ();
			}
		}

		private static void SetupBillingDetails(BillingDetailEntity billingDetails)
		{
			var date = Date.Today;

			billingDetails.AmountDue.Date = date;
			billingDetails.Title = string.Format ("Votre commande du {0}", Misc.GetDateTimeDescription (date.ToDateTime ()));
			billingDetails.EsrCustomerNumber = "01-69444-3";  // compte BVR
			billingDetails.EsrReferenceNumber = "96 13070 01000 02173 50356 73892";  // n° de réf BVR lié
			// TODO: Trouver ces 2 dernières informations de façon plus générale !
		}



		protected override EditionStatus GetEditionStatus()
		{
			var entity = this.Entity;
			return EditionStatus.Valid;
		}



		[ControllerSubType (1)]
		class SummaryInvoiceDocumentViewControllerForTotal : SummaryInvoiceDocumentViewController
		{
			public SummaryInvoiceDocumentViewControllerForTotal(string name, Entities.InvoiceDocumentEntity entity)
				: base (name, entity)
			{
			}

			protected override void CreateUI()
			{
				using (var data = TileContainerController.Setup (this))
				{
					this.CreateUIMain (data);
				}
			}

			private void CreateUIMain(SummaryDataItems data)
			{
				data.Add (
					new SummaryData
					{
						Name				= "InvoiceDocument",
						IconUri				= "Data.InvoiceDocument",
						Title				= TextFormatter.FormatText ("Document"),
						CompactTitle		= TextFormatter.FormatText ("Document"),
						TextAccessor		= Accessor.Create (this.EntityGetter, x => InvoiceDocumentHelper.GetSummary (x)),
						CompactTextAccessor = Accessor.Create (this.EntityGetter, x => TextFormatter.FormatText ("N°", x.IdA)),
						EntityMarshaler		= this.EntityMarshaler,
					});
			}
		}


	}
}
