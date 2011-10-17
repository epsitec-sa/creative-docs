//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Library.Business.ContentAccessors
{
	/// <summary>
	/// The <c>DocumentItemAccessor</c> is used to build the visual content of a line in a
	/// business document. The content is represented as an array of rows and columns. It
	/// will be used to fill the UI used for the interactive edition of the business document.
	/// It will also be used by the various document printers to produce their layout.
	/// </summary>
	public sealed class DocumentItemAccessor
	{
		private DocumentItemAccessor(DocumentMetadataEntity documentMetadata, DocumentLogic documentLogic, IncrementalNumberGenerator numberGenerator,
			DocumentItemAccessorMode mode, string languageId, AbstractDocumentItemEntity item, int groupIndex)
		{
			this.content           = new Dictionary<int, FormattedText> ();
			this.errors            = new Dictionary<int, DocumentItemAccessorError> ();
			this.articleQuantities = new List<ArticleQuantityEntity> ();

			this.documentMetadata  = documentMetadata;
			this.businessDocument  = documentMetadata.BusinessDocument as BusinessDocumentEntity;
			this.documentLogic     = documentLogic;
			this.numberGenerator   = numberGenerator;
			this.item              = item;
			this.groupIndex        = groupIndex;
			this.mode              = mode;
			this.languageId        = languageId;
			
			if (this.businessDocument.IsNotNull () &&
				this.businessDocument.PriceGroup.IsNotNull ())
			{
				this.billingMode = this.businessDocument.PriceGroup.BillingMode;
			}

			if (this.documentMetadata.IsNotNull () &&
				this.documentMetadata.DocumentCategory.IsNotNull ())
			{
				this.type = this.documentMetadata.DocumentCategory.DocumentType;
			}

			//	Construit tout le contenu.
			if (this.mode.HasFlag (DocumentItemAccessorMode.ShowMyEyesOnly) &&
				this.item.Attributes.HasFlag (DocumentItemAttributes.MyEyesOnly))
			{
				//	Rien à faire...
			}
			else
			{
				this.BuildItem ();
			}
		}


		public void TransformToSingleRow()
		{
			//	Appond les contenus de toutes les lignes dans la première, puis ne conserve que la première ligne.
			//	La première ligne contient alors des textes multi-lignes, et il devient facile de rendre le tout
			//	inséparable (TableBand.SetUnbreakableRow).
			foreach (DocumentItemAccessorColumn column in System.Enum.GetValues (typeof (DocumentItemAccessorColumn)))
			{
				var merged = FormattedText.Empty;
				bool empty = true;

				for (int row = 0; row < this.rowsCount; row++)
				{
					var line = this.GetContent (row, column);
					merged = merged.AppendLine (line);

					if (!line.IsNullOrEmpty)
					{
						empty = false;
					}
				}

				if (!empty)
				{
					this.SetContent (0, column, merged);
				}

				for (int row = 1; row < this.rowsCount; row++)
				{
					var key = DocumentItemAccessor.GetKey (row, column);

					if (this.content.ContainsKey (key))
					{
						this.content.Remove (key);
					}
				}
			}

			this.rowsCount = 1;
		}

		
		public int								RowsCount
		{
			get
			{
				return this.rowsCount;
			}
		}

		public int								GroupIndex
		{
			get
			{
				return this.groupIndex;
			}
		}

		public AbstractDocumentItemEntity		Item
		{
			get
			{
				return this.item;
			}
		}


		public bool								IsText
		{
			get
			{
				return this.item is TextDocumentItemEntity;
			}
		}

		public bool								IsArticle
		{
			get
			{
				return this.item is ArticleDocumentItemEntity;
			}
		}

		public bool								IsTax
		{
			get
			{
				return this.item is TaxDocumentItemEntity;
			}
		}

		public bool								IsSubTotal
		{
			get
			{
				return this.item is SubTotalDocumentItemEntity;
			}
		}

		public bool								IsEndTotal
		{
			get
			{
				return this.item is EndTotalDocumentItemEntity;
			}
		}

		
		public static IEnumerable<DocumentItemAccessor> CreateAccessors(DocumentMetadataEntity documentMetadata, DocumentLogic documentLogic, DocumentItemAccessorMode mode, string languageId, IEnumerable<DocumentAccessorContentLine> lines)
		{
			DocumentItemAccessor.EnsureValidMode (mode);
			
			var numberGenerator = new IncrementalNumberGenerator ();

			foreach (var line in lines)
			{
				yield return new DocumentItemAccessor (documentMetadata, documentLogic, numberGenerator, mode, languageId, line.Line, line.GroupIndex);
			}
		}


		public FormattedText GetContent(int row, DocumentItemAccessorColumn column)
		{
			//	Retourne le contenu d'une cellule.
			var key = DocumentItemAccessor.GetKey (row, column);

			if (this.content.ContainsKey (key))
			{
				return this.content[key];
			}
			else
			{
				return FormattedText.Null;
			}
		}

		public DocumentItemAccessorError GetError(int row)
		{
			foreach (var column in EnumType.GetAllEnumValues<DocumentItemAccessorColumn> ())
			{
				var error = this.GetError (row, column);
				
				if (error != DocumentItemAccessorError.None)
				{
					return error;
				}
			}

			return DocumentItemAccessorError.None;
		}

		public DocumentItemAccessorError GetError(int row, DocumentItemAccessorColumn column)
		{
			//	Retourne l'erreurs d'une cellule.
			var key = DocumentItemAccessor.GetKey (row, column);

			if (this.errors.ContainsKey (key))
			{
				return this.errors[key];
			}

			return DocumentItemAccessorError.None;
		}

		public ArticleQuantityEntity GetArticleQuantity(int row)
		{
			if (row > 0)
			{
				if (row < 1+this.discountRowCount)
				{
					return null;  // on est sur une ligne de rabais
				}

				row -= this.discountRowCount;  // saute les lignes de rabais
			}

			if (row < this.articleQuantities.Count)
			{
				return this.articleQuantities[row];
			}

			return null;
		}



		private void BuildItem()
		{
			this.discountRowCount = 0;

			this.BuildTextItem     (this.item as TextDocumentItemEntity);
			this.BuildArticleItem  (this.item as ArticleDocumentItemEntity);
			this.BuildTaxItem      (this.item as TaxDocumentItemEntity);
			this.BuildSubTotalItem (this.item as SubTotalDocumentItemEntity);
			this.BuildEndTotalItem (this.item as EndTotalDocumentItemEntity);
			this.BuildCommonItem ();
		}
		
		private void BuildTextItem(TextDocumentItemEntity line)
		{
			if (line.IsNull ())
			{
				return;
			}

			var text = line.Text;

			if (text.IsNullOrEmpty)
			{
				this.SetError (0, DocumentItemAccessorColumn.ArticleDescription, DocumentItemAccessorError.TextNotDefined);
				this.SetContent (0, DocumentItemAccessorColumn.ArticleDescription, " ");
			}
			else
			{
				this.SetContent (0, DocumentItemAccessorColumn.ArticleDescription, text);
			}
		}

		private void BuildArticleItem(ArticleDocumentItemEntity line)
		{
			//	Un article peut être composé des lignes suivantes:
			//		Désignation de l'article, avec la quantité Ordered.
			//		0..n lignes de rabais (n = discountRowCount)
			//		0..n lignes de quantités différées
			if (line.IsNull ())
			{
				return;
			}

			var description = this.GetArticleItemDescription (line);

			if (description.IsNullOrEmpty)
			{
				this.SetError (0, DocumentItemAccessorColumn.ArticleDescription, DocumentItemAccessorError.ArticleNotDefined);
			}

			int row = 0;

			this.SetContent (0, DocumentItemAccessorColumn.ArticleId,          ArticleDocumentItemHelper.GetArticleId (line));
			this.SetContent (0, DocumentItemAccessorColumn.ArticleDescription, description);

			switch (this.billingMode)
			{
				case BillingMode.ExcludingTax:
					row = this.BuildArticleItemPrice (line, row, DocumentItemAccessorColumn.UnitPrice, line.UnitPriceBeforeTax1, DiscountPolicy.OnUnitPrice);
					row = this.BuildArticleItemPrice (line, row, DocumentItemAccessorColumn.LinePrice, line.LinePriceBeforeTax1, DiscountPolicy.OnLinePrice);
					row = this.BuildArticleItemQuantities (line, row);
					this.SetContent (row, DocumentItemAccessorColumn.TotalPrice, this.GetFormattedPrice (line.TotalRevenueBeforeTax));
					break;

				case BillingMode.IncludingTax:
					row = this.BuildArticleItemPrice (line, row, DocumentItemAccessorColumn.UnitPrice, line.UnitPriceAfterTax1, DiscountPolicy.OnUnitPrice);
					row = this.BuildArticleItemPrice (line, row, DocumentItemAccessorColumn.LinePrice, line.LinePriceAfterTax1, DiscountPolicy.OnLinePrice);
					row = this.BuildArticleItemQuantities (line, row);
					this.SetContent (row, DocumentItemAccessorColumn.TotalPrice, this.GetFormattedPrice (line.TotalRevenueAfterTax));
					break;
			}
			
			if (line.VatRatio == 1)
			{
				this.SetContent (row, DocumentItemAccessorColumn.VatRate, this.GetFormattedPercent (line.VatRateA));
			}
			else
			{
				//	TODO: handle multiple VAT rates
			}
		}

		private int BuildArticleItemPrice(ArticleDocumentItemEntity line, int row, DocumentItemAccessorColumn column, decimal? price, DiscountPolicy policy)
		{
			if (!this.mode.HasFlag (DocumentItemAccessorMode.NoPrices))
			{
				this.SetContent (row, column, this.GetFormattedPrice (price));

				foreach (var discount in line.Discounts)
				{
					if (discount.DiscountPolicy == policy)
					{
						row = this.BuildArticleItemDiscount (row, column, discount);
					}
				}
			}

			return row;
		}

		private int BuildArticleItemDiscount(int row, DocumentItemAccessorColumn column, PriceDiscountEntity discount)
		{
			if (discount.HasDiscountRate)
			{
				row++;
				this.SetContent (row, DocumentItemAccessorColumn.ArticleDescription, discount.Text);
				this.SetContent (row, column, this.GetFormattedPercent (-discount.DiscountRate.Value));

				this.discountRowCount++;
			}
			
			if (discount.HasValue)
			{
				row++;
				this.SetContent (row, DocumentItemAccessorColumn.ArticleDescription, discount.Text);
				this.SetContent (row, column, this.GetFormattedPrice (-discount.Value.Value));

				this.discountRowCount++;
			}
			
			return row;
		}
			
		private int BuildArticleItemQuantities(ArticleDocumentItemEntity line, int row)
		{
			//	Génère les quantités.
			this.billingRow = row;

			if (row > 0 ||
				(!this.mode.HasFlag (DocumentItemAccessorMode.AdditionalQuantitiesSeparate) &&
				 !this.mode.HasFlag (DocumentItemAccessorMode.MainQuantityOnTop)))
			{
				//	If there is more than the single article row, this means that we have some discounts
				//	and we don't want the quantities on the same lines, so start emitting quantities after
				//	the last discount.
				row++;
			}

			if (this.mode.HasFlag (DocumentItemAccessorMode.Print))
			{
				this.BuildArticleItemQuantitiesForPrint (line, row);
			}
			else
			{
				this.BuildArticleItemQuantitiesForEdition (line, row);
			}

			return this.billingRow;
		}

		private void BuildArticleItemQuantitiesForEdition(ArticleDocumentItemEntity line, int row)
		{
			foreach (var quantityType in DocumentItemAccessor.ArticleQuantityTypes)
			{
				foreach (var quantity in line.ArticleQuantities.Where (x => x.QuantityColumn.QuantityType == quantityType).OrderBy (x => x.BeginDate))
				{
					row = this.AddArticleQuantity (line, row, quantityType, quantity);
				}
			}
		}

		private void BuildArticleItemQuantitiesForPrint(ArticleDocumentItemEntity line, int row)
		{
			//	Utilise les colonnes MainQuantity/MainUnit
			var quantityTypes = this.documentLogic.GetPrintableArticleQuantityTypes ();
			var mainQuantityType = ArticleQuantityType.None;
			decimal mainQuantity = 0;
			var mainUnitName = FormattedText.Empty;

			if (quantityTypes.Any ())
			{
				mainQuantityType = quantityTypes.First ();
				mainQuantity = line.GetQuantity (mainQuantityType);
				mainUnitName = line.ArticleDefinition.GetBillingUnitName ();
			}

			var orderedQuantity = line.GetQuantity (ArticleQuantityType.Ordered);
			var orderedUnitName = line.ArticleDefinition.GetBillingUnitName ();

			int billingQuantityRow;

			if (orderedQuantity == mainQuantity && orderedUnitName == mainUnitName)
			{
				billingQuantityRow = 0;
			}
			else
			{
				billingQuantityRow = row++;
				this.billingRow = billingQuantityRow;
			}

			//	Génère la quantité principale.
			if (quantityTypes.Any ())
			{
				var unit = ArticleQuantityEntity.GetUnitNameForQuantity (mainQuantity, mainUnitName);

				this.SetContent (billingQuantityRow, DocumentItemAccessorColumn.MainQuantity, TextFormatter.FormatText (mainQuantity));
				this.SetContent (billingQuantityRow, DocumentItemAccessorColumn.MainUnit, unit);
				this.SetError (billingQuantityRow, DocumentItemAccessorColumn.MainQuantity, this.GetQuantityError (line, mainQuantityType));
			}

			//	Génère la quantité commandée, pour les factures, dans la première ligne de l'article (qui existe déjà).
			{
				var unit = ArticleQuantityEntity.GetUnitNameForQuantity (orderedQuantity, orderedUnitName);

				this.SetContent (0, DocumentItemAccessorColumn.OrderedQuantity, TextFormatter.FormatText (orderedQuantity));
				this.SetContent (0, DocumentItemAccessorColumn.OrderedUnit, unit);
			}

			//	Génère les autres quantités (sans la principale).
			if (this.mode.HasFlag (DocumentItemAccessorMode.AdditionalQuantities))  // met les quantités additionnelles ?
			{
				// Sur un document imprimé, les autres quantités sont toujours sur des lignes à part.
				foreach (var quantityType in quantityTypes)
				{
					foreach (var quantity in line.ArticleQuantities.Where (x => x.QuantityColumn.QuantityType == quantityType && x.QuantityColumn.QuantityType != mainQuantityType).OrderBy (x => x.BeginDate))
					{
						//	Une ligne "déjà livré" avec une quantité nulle est omise.
						if (quantityType == ArticleQuantityType.ShippedPreviously && quantity.Quantity == 0)
						{
							continue;
						}

						row = this.AddArticleQuantity (line, row, quantityType, quantity);
					}
				}
			}
		}

		private int AddArticleQuantity(ArticleDocumentItemEntity line, int row, ArticleQuantityType quantityType, ArticleQuantityEntity quantity)
		{
			int nextRow = row+1;

			if (quantity.QuantityColumn.QuantityType == ArticleQuantityType.Ordered)
			{
				//	The ordered quantity will always be generated on the first row.
				if (row != 0)
				{
					nextRow = row;
					row     = 0;
				}
			}

			this.articleQuantities.Add (quantity);

			var unit = ArticleQuantityEntity.GetUnitNameForQuantity (quantity.Quantity, quantity.Unit.Name);

			this.SetContent (row, DocumentItemAccessorColumn.AdditionalType,     TextFormatter.FormatText (quantity.QuantityColumn.Name));
			this.SetContent (row, DocumentItemAccessorColumn.AdditionalQuantity, TextFormatter.FormatText (quantity.Quantity));
			this.SetContent (row, DocumentItemAccessorColumn.AdditionalUnit,     unit);

			if (quantity.BeginDate.HasValue)
			{
				this.SetContent (row, DocumentItemAccessorColumn.AdditionalBeginDate, TextFormatter.FormatText (quantity.BeginDate.Value));
			}

			if (quantity.EndDate.HasValue)
			{
				this.SetContent (row, DocumentItemAccessorColumn.AdditionalEndDate, TextFormatter.FormatText (quantity.EndDate.Value));
			}

			this.SetError (row, DocumentItemAccessorColumn.AdditionalQuantity, this.GetQuantityError (line, quantityType));

			if (quantity.QuantityColumn.QuantityType == ArticleQuantityType.Billed)
			{
				this.billingRow = row;
			}

			return nextRow;
		}

		private void BuildTaxItem(TaxDocumentItemEntity line)
		{
			if (line.IsNull ())
			{
				return;
			}

			var text = line.Text.GetValueOrDefault ("TVA ({total} à {taux})");

			FormattedText revenue = this.GetFormattedPrice (line.TotalRevenue);
			FormattedText vatRate = Misc.PercentToString (line.VatRate);

			foreach (var pattern in DocumentItemAccessor.TotalRevenuePatterns)
			{
				text = text.Replace (pattern, revenue, System.StringComparison.OrdinalIgnoreCase);
			}

			foreach (var pattern in DocumentItemAccessor.VatRatePatterns)
			{
				text = text.Replace (pattern, vatRate, System.StringComparison.OrdinalIgnoreCase);
			}

			this.SetContent (0, DocumentItemAccessorColumn.ArticleDescription, text);
			this.SetContent (0, DocumentItemAccessorColumn.VatRate, this.GetFormattedPercent (line.VatRate));

			switch (this.billingMode)
			{
				case BillingMode.ExcludingTax:
					this.SetContent (0, DocumentItemAccessorColumn.LinePrice, revenue);
					this.SetContent (0, DocumentItemAccessorColumn.VatInfo, this.GetFormattedPercent (line.VatRate));
					this.SetContent (0, DocumentItemAccessorColumn.TotalPrice, this.GetFormattedPrice (line.ResultingTax));
					break;

				case BillingMode.IncludingTax:
					this.SetContent (0, DocumentItemAccessorColumn.VatInfo, this.GetFormattedPrice (line.ResultingTax));
					break;
			}
		}

		private void BuildSubTotalItem(SubTotalDocumentItemEntity line)
		{
			if ((line.IsNull ()) ||
				(this.billingMode == BillingMode.None))
			{
				return;
			}

			//	1) Ligne "sous-total".
			var primaryText = line.TextForPrimaryPrice.GetValueOrDefault ("Sous-total");

			//	2) Ligne "rabais".
			var  discountText = line.TextForDiscount.GetValueOrDefault ("Rabais");

			bool hasDiscount = false;

			if (line.Discount.IsNotNull ())
			{
				if (line.Discount.HasDiscountRate)
				{
					discountText = FormattedText.Concat (discountText, " (", this.GetFormattedPercent (line.Discount.DiscountRate), ")");
					hasDiscount  = true;
				}
				else if (line.Discount.HasValue)
				{
					discountText = FormattedText.Concat (discountText, this.billingMode == BillingMode.ExcludingTax ? " HT" : " TTC");
					hasDiscount  = true;
				}
			}

			//	3) Ligne "total après rabais".
			var sumText = line.TextForResultingPrice.GetValueOrDefault ("Total après rabais");

			var subTotal1 = this.billingMode == BillingMode.ExcludingTax ? line.PriceBeforeTax1 : line.PriceAfterTax1;
			var subTotal2 = this.billingMode == BillingMode.ExcludingTax ? line.PriceBeforeTax2 : line.PriceAfterTax2;

			int row = 0;

			if (hasDiscount)
			{
				this.SetContent (row, DocumentItemAccessorColumn.ArticleDescription, primaryText);
				this.SetContent (row, DocumentItemAccessorColumn.TotalPrice, this.GetFormattedPrice (subTotal1));
				row++;

				this.SetContent (row, DocumentItemAccessorColumn.ArticleDescription, discountText);
				this.SetContent (row, DocumentItemAccessorColumn.TotalPrice, this.GetFormattedPrice (subTotal2 - subTotal1));
				row++;
			}

			this.SetContent (row, DocumentItemAccessorColumn.ArticleDescription, sumText);
			this.SetContent (row, DocumentItemAccessorColumn.TotalPrice, this.GetFormattedPrice (subTotal2));
		}

		private void BuildEndTotalItem(EndTotalDocumentItemEntity line)
		{
			if (line.IsNull ())
			{
				return;
			}

			int row = 0;

			if (line.TotalRounding.HasValue)
			{
				this.SetContent (row, DocumentItemAccessorColumn.ArticleDescription, line.TextForPrice.GetValueOrDefault ("Arrondi"));
				this.SetContent (row, DocumentItemAccessorColumn.TotalPrice, this.GetFormattedPrice (line.TotalRounding));

				row++;
			}

			this.SetContent (row, DocumentItemAccessorColumn.ArticleDescription, line.TextForPrice.GetValueOrDefault ("Grand total"));
			this.SetContent (row, DocumentItemAccessorColumn.TotalPrice, this.GetFormattedPrice (line.PriceAfterTax));

			row++;

			if (line.FixedPriceAfterTax.HasValue)
			{
				this.SetContent (row, DocumentItemAccessorColumn.ArticleDescription, line.TextForFixedPrice.GetValueOrDefault ("Grand total après escompte"));
				this.SetContent (row, DocumentItemAccessorColumn.TotalPrice, this.GetFormattedPrice (line.FixedPriceAfterTax));
			}
		}

		private void BuildCommonItem()
		{
			this.numberGenerator.PutNext (this.groupIndex);

			this.SetContent (0, DocumentItemAccessorColumn.GroupNumber, this.numberGenerator.GroupNumber);
			this.SetContent (0, DocumentItemAccessorColumn.LineNumber, this.numberGenerator.SimpleNumber);
			this.SetContent (0, DocumentItemAccessorColumn.FullNumber, this.numberGenerator.FullNumber);
		}
		
		
		private DocumentItemAccessorError GetQuantityError(ArticleDocumentItemEntity line, ArticleQuantityType quantityType)
		{
			//	Génère une éventuelle erreur sur les quantités.
			decimal orderedQuantity = 0;
			decimal delayedQuantity = 0;
			decimal expectedQuantity = 0;
			decimal shippedQuantity = 0;
			decimal shippedPreviouslyQuantity = 0;
			decimal billedQuantity = 0;

			var articleDefinition = line.ArticleDefinition;
			
			foreach (var quantity in line.ArticleQuantities)
			{
				decimal value = articleDefinition.ConvertToBillingUnit (quantity.Quantity, quantity.Unit);

				switch (quantity.QuantityColumn.QuantityType)
				{
					case ArticleQuantityType.Ordered:
						orderedQuantity += value;
						break;

					case ArticleQuantityType.Delayed:
						delayedQuantity += value;
						break;

					case ArticleQuantityType.Expected:
						expectedQuantity += value;
						break;

					case ArticleQuantityType.Shipped:
						shippedQuantity += value;
						break;

					case ArticleQuantityType.ShippedPreviously:
						shippedPreviouslyQuantity += value;
						break;

					case ArticleQuantityType.Billed:
						billedQuantity += value;
						break;
				}
			}

			switch (quantityType)
			{
				case ArticleQuantityType.Ordered:
				case ArticleQuantityType.Delayed:
				case ArticleQuantityType.Expected:
					if (orderedQuantity < delayedQuantity+expectedQuantity)
					{
						return DocumentItemAccessorError.AdditionalQuantitiesTooHigh;
					}
					break;

				case ArticleQuantityType.Shipped:
					if (shippedQuantity > orderedQuantity-delayedQuantity-expectedQuantity-shippedPreviouslyQuantity)
					{
						return DocumentItemAccessorError.ShippedQuantitiesTooHigh;
					}
					break;

				case ArticleQuantityType.ShippedPreviously:
					break;

				case ArticleQuantityType.Billed:
					if (!Controllers.BusinessDocumentControllers.InvoiceDocumentLogic.IsDirectInvoice (this.documentMetadata) &&
						billedQuantity > shippedQuantity)
					{
						return DocumentItemAccessorError.BilledQuantitiesTooHigh;
					}
					break;
			}

			return DocumentItemAccessorError.None;
		}

		private FormattedText GetArticleItemDescription(ArticleDocumentItemEntity line)
		{
			//	Génère la description.
			var description = FormattedText.Empty;

			if (this.mode.HasFlag (DocumentItemAccessorMode.EditArticleName))
			{
				description = line.ArticleNameCache;
			}
			else if (this.mode.HasFlag (DocumentItemAccessorMode.EditArticleDescription))
			{
				description = line.ArticleDescriptionCache;
			}
			else if (this.mode.HasFlag (DocumentItemAccessorMode.UseArticleName))
			{
				description = line.ArticleNameCache.GetValueOrDefault (line.ArticleDescriptionCache);
			}
			else if (this.mode.HasFlag (DocumentItemAccessorMode.UseArticleBoth))
			{
				description = description.AppendLineIfNotNull (line.ArticleNameCache);
				description = description.AppendLineIfNotNull (line.ArticleDescriptionCache);
			}
			
			return description;
		}
		
#if false
		private decimal GetArticleItemTotalTax(ArticleDocumentItemEntity line)
		{
			//	...pour l'instant plus utilisé...

			decimal afterTax   = line.LinePriceAfterTax2.GetValueOrDefault ();
			decimal afterTaxA  = afterTax * (0 + line.VatRatio);
			decimal afterTaxB  = afterTax * (1 - line.VatRatio);
			decimal beforeTaxA = afterTaxA / (1 + line.VatRateA);
			decimal beforeTaxB = afterTaxB / (1 + line.VatRateB);
			decimal tax        = beforeTaxA * line.VatRateA + beforeTaxB * line.VatRateB;

			return tax;
		}
#endif

		private static readonly ArticleQuantityType[] ArticleQuantityTypes =
		{
			ArticleQuantityType.Ordered,
			ArticleQuantityType.Delayed,
			ArticleQuantityType.Expected,
			ArticleQuantityType.Shipped,
			ArticleQuantityType.ShippedPreviously,
			ArticleQuantityType.Billed,
			ArticleQuantityType.Information,
		};
		
		private readonly static string[] TotalRevenuePatterns =
		{
			"{0}",
			"{Total}",
			"{Prix}",
			"{Price}",
			"{Base}",
			"{Amount}",
			"{Revenue}",
		};

		private readonly static string[] VatRatePatterns =
		{
			"{1}",
			"{Taux}",
			"{TVA}",
			"{Rate}",
			"{VAT}",
		};


		private FormattedText GetFormattedPrice(decimal? price)
		{
			if (price.HasValue)
			{
				if (this.businessDocument == null)
				{
					return Misc.PriceToString (PriceCalculator.RoundToCents (price.Value));
				}
				else
				{
					return Misc.PriceToString (PriceCalculator.ClipPriceValue (price.Value, this.businessDocument.CurrencyCode));
				}
			}

			return FormattedText.Empty;
		}

		private FormattedText GetFormattedPercent(decimal? percent)
		{
			if (percent == null)
			{
				return null;
			}

			percent = PriceCalculator.ClipTaxRateValue (percent.Value);
			return Misc.PercentToString (percent);
		}


		private void SetContent(int row, DocumentItemAccessorColumn column, FormattedText text)
		{
			//	Modifie le contenu d'une cellule.
			text = TextFormatter.GetMonolingualText (text, this.languageId);

			if (text != null && !text.IsNullOrEmpty)
			{
				var key = DocumentItemAccessor.GetKey (row, column);
				this.content[key] = text;

				this.rowsCount = System.Math.Max (this.rowsCount, row+1);
			}
		}

		private void SetError(int row, DocumentItemAccessorColumn column, DocumentItemAccessorError error)
		{
			//	Modifie l'erreur d'une cellule.
			if (error != DocumentItemAccessorError.None)
			{
				var key = DocumentItemAccessor.GetKey (row, column);
				this.errors[key] = error;
			}
		}

		private static void EnsureValidMode(DocumentItemAccessorMode mode)
		{
			int count = 0;

			if (mode.HasFlag (DocumentItemAccessorMode.EditArticleName))
			{
				count++;
			}
			if (mode.HasFlag (DocumentItemAccessorMode.EditArticleDescription))
			{
				count++;
			}
			if (mode.HasFlag (DocumentItemAccessorMode.UseArticleName))
			{
				count++;
			}
			if (mode.HasFlag (DocumentItemAccessorMode.UseArticleBoth))
			{
				count++;
			}

			if (count == 0)
			{
				throw new System.NotSupportedException ("No mode has been specified");
			}
			if (count > 1)
			{
				throw new System.NotSupportedException ("At most one mode may be specified");
			}

			if (!mode.HasFlag (DocumentItemAccessorMode.Print) &&
				mode.HasFlag (DocumentItemAccessorMode.AdditionalQuantities))
			{
				throw new System.NotSupportedException ("Invalid mode combination");
			}

			if (!mode.HasFlag (DocumentItemAccessorMode.Print) &&
				mode.HasFlag (DocumentItemAccessorMode.AdditionalQuantitiesSeparate))
			{
				throw new System.NotSupportedException ("Invalid mode combination");
			}

			if (!mode.HasFlag (DocumentItemAccessorMode.AdditionalQuantities) &&
				mode.HasFlag (DocumentItemAccessorMode.AdditionalQuantitiesSeparate))
			{
				throw new System.NotSupportedException ("Invalid mode combination");
			}

			if (!mode.HasFlag (DocumentItemAccessorMode.Print) &&
				mode.HasFlag (DocumentItemAccessorMode.MainQuantityOnTop))
			{
				throw new System.NotSupportedException ("Invalid mode combination");
			}
		}

		private static int GetKey(int row, DocumentItemAccessorColumn column)
		{
			//	Retourne une clé unique qui mixe la ligne et la colonne.
			//	On suppose qu'il n'y aura jamais plus de 1000 colonnes !
			return row*1000 + (int) column;
		}


		public static FormattedText GetErrorDescription(DocumentItemAccessorError error)
		{
			switch (error)
			{
				case DocumentItemAccessorError.Unknown:
					return "erreur";

				case DocumentItemAccessorError.TextNotDefined:
				case DocumentItemAccessorError.ArticleNotDefined:
					return "pas encore défini";

				case DocumentItemAccessorError.AdditionalQuantitiesTooHigh:
					return "les quantités additionnelles dépassent la quantité commandée";

				case DocumentItemAccessorError.ShippedQuantitiesTooHigh:
					return "la quantité livrée est trop grande";

				case DocumentItemAccessorError.BilledQuantitiesTooHigh:
					return "la quantité facturée est trop grande";

				default:
					return null;  // ok
			}
		}


		private readonly DocumentMetadataEntity						documentMetadata;
		private readonly BusinessDocumentEntity						businessDocument;
		private readonly DocumentType								type;
		private readonly BillingMode								billingMode;
		private readonly DocumentLogic								documentLogic;
		private readonly IncrementalNumberGenerator					numberGenerator;
		private readonly Dictionary<int, FormattedText>				content;
		private readonly Dictionary<int, DocumentItemAccessorError>	errors;
		private readonly List<ArticleQuantityEntity>				articleQuantities;
		private readonly AbstractDocumentItemEntity					item;
		private readonly DocumentItemAccessorMode					mode;
		private readonly string										languageId;
		private readonly int										groupIndex;
		
		private int													rowsCount;
		private int													billingRow;
		private int													discountRowCount;
	}
}
