//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	/// Accesseur universel permettant d'obtenir le contenu d'une ligne d'un document commercial,
	/// sous forme d'un tableau (ligne/colonne).
	/// </summary>
	public sealed class DocumentItemAccessor
	{
		public DocumentItemAccessor(DocumentMetadataEntity documentMetadataEntity, BusinessLogic businessLogic, IncrementalNumberGenerator numberGenerator)
		{
			this.documentMetadataEntity  = documentMetadataEntity;
			this.businessDocumentEntity  = documentMetadataEntity.BusinessDocument as BusinessDocumentEntity;
			this.businessLogic           = businessLogic;
			this.numberGenerator         = numberGenerator;
			this.content                 = new Dictionary<int, FormattedText> ();
			this.errors                  = new Dictionary<int, DocumentItemAccessorError> ();
			this.articleQuantityEntities = new List<ArticleQuantityEntity> ();
			
			if ((this.businessDocumentEntity.IsNotNull ()) &&
				(this.businessDocumentEntity.PriceGroup.IsNotNull ()))
			{
				this.billingMode = this.businessDocumentEntity.PriceGroup.BillingMode;
			}
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

		
		public bool BuildContent(AbstractDocumentItemEntity item, DocumentType type, DocumentItemAccessorMode mode, int? groupIndex = null)
		{
			//	Construit tout le contenu.
			//	Retourne false si le contenu est entièrement caché.
			
			DocumentItemAccessor.EnsureValidMode (mode);

			this.item = item;
			this.type = type;
			this.mode = mode;
			this.groupIndex = groupIndex.GetValueOrDefault (item.GroupIndex);

			this.content.Clear ();

			if ((this.mode.HasFlag (DocumentItemAccessorMode.ShowMyEyesOnly)) &&
				(this.item.Attributes.HasFlag (DocumentItemAttributes.MyEyesOnly)))
			{
				return false;
			}

			this.BuildTextItem (this.item as TextDocumentItemEntity);
			this.BuildArticleItem (this.item as ArticleDocumentItemEntity);
			this.BuildTaxItem (this.item as TaxDocumentItemEntity);
			this.BuildSubTotalItem (this.item as SubTotalDocumentItemEntity);
			this.BuildEndTotalItem (this.item as EndTotalDocumentItemEntity);
			this.BuildCommonItem ();

			return true;
		}

		public bool IsEmptyRow(int row)
		{
			//	Retourne true si une ligne est entièrement vide.
			foreach (var column in System.Enum.GetValues (typeof (DocumentItemAccessorColumn)))
			{
				var key = DocumentItemAccessor.GetKey (row, (DocumentItemAccessorColumn) column);
				if (this.content.ContainsKey (key))
				{
					return false;  // la ligne n'est pas vide
				}
			}

			return true;  // ligne entièrement vide
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
				return null;
			}
		}

		public DocumentItemAccessorError GetError(int row)
		{
			foreach (var column in System.Enum.GetValues (typeof (DocumentItemAccessorColumn)))
			{
				var error = this.GetError (row, (DocumentItemAccessorColumn) column);
				if (error != DocumentItemAccessorError.OK)
				{
					return error;
				}
			}

			return DocumentItemAccessorError.OK;
		}

		public DocumentItemAccessorError GetError(int row, DocumentItemAccessorColumn column)
		{
			//	Retourne l'erreurs d'une cellule.
			var key = DocumentItemAccessor.GetKey (row, column);

			if (this.errors.ContainsKey (key))
			{
				return this.errors[key];
			}
			else
			{
				return DocumentItemAccessorError.OK;
			}
		}

		public ArticleQuantityEntity GetArticleQuantityEntity(int row)
		{
			if (row < this.articleQuantityEntities.Count)
			{
				return this.articleQuantityEntities[row];
			}
			else
			{
				return null;
			}
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
			if (line.IsNull ())
			{
				return;
			}

			this.BuildArticleItemQuantities (line);

			var description = this.GetArticleItemDescription (line);
			var revenue     = this.billingMode == BillingMode.ExcludingTax ? line.TotalRevenueBeforeTax : line.TotalRevenueAfterTax;

			if (description.IsNullOrEmpty)
			{
				this.SetError (0, DocumentItemAccessorColumn.ArticleDescription, DocumentItemAccessorError.ArticleNotDefined);
			}

			this.SetContent (0, DocumentItemAccessorColumn.ArticleId,           ArticleDocumentItemHelper.GetArticleId (line));
			this.SetContent (0, DocumentItemAccessorColumn.ArticleDescription,  description);
			this.SetContent (0, DocumentItemAccessorColumn.UnitPriceBeforeTax1,  this.GetFormattedPrice (line.UnitPriceBeforeTax1));
			this.SetContent (0, DocumentItemAccessorColumn.UnitPriceAfterTax1,   this.GetFormattedPrice (line.UnitPriceAfterTax1));
			this.SetContent (0, DocumentItemAccessorColumn.LinePriceBeforeTax1,  this.GetFormattedPrice (line.LinePriceBeforeTax1));
			this.SetContent (0, DocumentItemAccessorColumn.LinePriceAfterTax1,   this.GetFormattedPrice (line.LinePriceAfterTax1));
			this.SetContent (0, DocumentItemAccessorColumn.LinePriceBeforeTax2, this.GetFormattedPrice (line.LinePriceBeforeTax2));
			this.SetContent (0, DocumentItemAccessorColumn.LinePriceAfterTax2,  this.GetFormattedPrice (line.LinePriceAfterTax2));
			this.SetContent (0, DocumentItemAccessorColumn.Revenue,             this.GetFormattedPrice (revenue));

			if (line.VatRatio == 1)
			{
				this.SetContent (0, DocumentItemAccessorColumn.VatRate, this.GetFormattedPercent (line.VatRateA));
			}
			else
			{
				//	TODO: handle multiple VAT rates
			}

			this.BuildArticleItemDiscounts (line);
		}

		private void BuildArticleItemQuantities(ArticleDocumentItemEntity line)
		{
			//	Génère les quantités.
			if (this.mode.HasFlag (DocumentItemAccessorMode.Print))
			{
				this.BuildArticleItemQuantitiesForPrint (line);
			}
			else
			{
				this.BuildArticleItemQuantitiesForEdition (line);
			}
		}
		
		private void BuildArticleItemQuantitiesForEdition(ArticleDocumentItemEntity line)
		{
			int row = 0;

			foreach (var quantityType in DocumentItemAccessor.ArticleQuantityTypes)
			{
				foreach (var quantity in line.ArticleQuantities.Where (x => x.QuantityColumn.QuantityType == quantityType).OrderBy (x => x.BeginDate))
				{
					this.articleQuantityEntities.Add (quantity);

					this.SetContent (row, DocumentItemAccessorColumn.AdditionalType, quantity.QuantityColumn.Name);
					this.SetContent (row, DocumentItemAccessorColumn.AdditionalQuantity, quantity.Quantity.ToString ());
					this.SetContent (row, DocumentItemAccessorColumn.AdditionalUnit, quantity.Unit.Name);

					if (quantity.BeginDate.HasValue)
					{
						this.SetContent (row, DocumentItemAccessorColumn.AdditionalBeginDate, quantity.BeginDate.Value.ToString ());
					}

					if (quantity.EndDate.HasValue)
					{
						this.SetContent (row, DocumentItemAccessorColumn.AdditionalEndDate, quantity.EndDate.Value.ToString ());
					}

					this.SetError (row, DocumentItemAccessorColumn.AdditionalQuantity, this.GetQuantityError (line, quantityType));

					row++;
				}
			}
		}

		private void BuildArticleItemQuantitiesForPrint(ArticleDocumentItemEntity line)
		{
			//	Utilise les colonnes MainQuantity/MainUnit
			
			//	Génère la quantité principale.
			var quantityTypes = this.businessLogic.PrintableArticleQuantityTypes;
			var mainQuantityType = ArticleQuantityType.None;
			int row = 0;

			if (quantityTypes.Any ())
			{
				mainQuantityType = quantityTypes.First ();

				var mainQuantity = line.GetQuantity (mainQuantityType);
				var mainUnitName = line.ArticleDefinition.GetBillingUnitName ();

				this.SetContent (row, DocumentItemAccessorColumn.MainQuantity, mainQuantity.ToString ());
				this.SetContent (row, DocumentItemAccessorColumn.MainUnit, mainUnitName);
				this.SetError (row, DocumentItemAccessorColumn.MainQuantity, this.GetQuantityError (line, mainQuantityType));

				row++;
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

						this.articleQuantityEntities.Add (quantity);

						this.SetContent (row, DocumentItemAccessorColumn.AdditionalType, quantity.QuantityColumn.Name);
						this.SetContent (row, DocumentItemAccessorColumn.AdditionalQuantity, quantity.Quantity.ToString ());
						this.SetContent (row, DocumentItemAccessorColumn.AdditionalUnit, quantity.Unit.Name);

						if (quantity.BeginDate.HasValue)
						{
							this.SetContent (row, DocumentItemAccessorColumn.AdditionalBeginDate, quantity.BeginDate.Value.ToString ());
						}

						if (quantity.EndDate.HasValue)
						{
							this.SetContent (row, DocumentItemAccessorColumn.AdditionalEndDate, quantity.EndDate.Value.ToString ());
						}

						this.SetError (row, DocumentItemAccessorColumn.AdditionalQuantity, this.GetQuantityError (line, quantityType));

						row++;
					}
				}
			}
		}

		private void BuildArticleItemDiscounts(ArticleDocumentItemEntity line)
		{
			int row = 0;

			foreach (var discount in line.Discounts)
			{
				if (discount.HasDiscountRate)
				{
					this.SetContent (row++, DocumentItemAccessorColumn.LineDiscount, this.GetFormattedPercent (discount.DiscountRate.Value));
				}
				else if (discount.HasValue)
				{
					this.SetContent (row++, DocumentItemAccessorColumn.LineDiscount, this.GetFormattedPrice (discount.Value.Value));
				}
			}
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
					if (!Controllers.BusinessDocumentControllers.InvoiceBusinessLogic.IsDirectInvoice (this.documentMetadataEntity) &&
						billedQuantity > shippedQuantity)
					{
						return DocumentItemAccessorError.BilledQuantitiesTooHigh;
					}
					break;
			}

			return DocumentItemAccessorError.OK;
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
				description = description.AppendLine (line.ArticleNameCache);
				description = description.AppendLine (line.ArticleDescriptionCache);
			}
			
			return description;
		}
		
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



		private void BuildTaxItem(TaxDocumentItemEntity line)
		{
			if (line.IsNull ())
			{
				return;
			}

			var text = line.Text;

			if (text.IsNullOrEmpty)
			{
				text = "TVA ({total} à {taux})";
			}

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
			this.SetContent (0, DocumentItemAccessorColumn.VatRevenue, revenue);
			this.SetContent (0, DocumentItemAccessorColumn.VatTotal, this.GetFormattedPrice (line.ResultingTax));
		}

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
				this.SetContent (row, DocumentItemAccessorColumn.SubTotal, this.GetFormattedPrice (subTotal1));
				row++;

				this.SetContent (row, DocumentItemAccessorColumn.ArticleDescription, discountText);
				this.SetContent (row, DocumentItemAccessorColumn.Revenue, this.GetFormattedPrice (subTotal2 - subTotal1));
				row++;
			}

			this.SetContent (row, DocumentItemAccessorColumn.ArticleDescription, sumText);
			this.SetContent (row, DocumentItemAccessorColumn.SubTotal, this.GetFormattedPrice (subTotal2));
		}

		private void BuildEndTotalItem(EndTotalDocumentItemEntity line)
		{
			if (line.IsNull ())
			{
				return;
			}

			this.SetContent (0, DocumentItemAccessorColumn.ArticleDescription,  line.TextForPrice.GetValueOrDefault ("Grand total"));
			this.SetContent (0, DocumentItemAccessorColumn.LinePriceBeforeTax1,  this.GetFormattedPrice (line.PriceBeforeTax));
			this.SetContent (0, DocumentItemAccessorColumn.LinePriceBeforeTax2, this.GetFormattedPrice (line.PriceAfterTax));

			if (line.FixedPriceAfterTax.HasValue)
			{
				this.SetContent (1, DocumentItemAccessorColumn.ArticleDescription,  line.TextForFixedPrice.GetValueOrDefault ("Grand total après escompte"));
				this.SetContent (1, DocumentItemAccessorColumn.LinePriceBeforeTax2, this.GetFormattedPrice (line.FixedPriceAfterTax));
			}
		}

		private void BuildCommonItem()
		{
			this.numberGenerator.PutNext (this.groupIndex);

			this.SetContent (0, DocumentItemAccessorColumn.GroupNumber, this.numberGenerator.GroupNumber);
			this.SetContent (0, DocumentItemAccessorColumn.LineNumber,  this.numberGenerator.SimpleNumber);
			this.SetContent (0, DocumentItemAccessorColumn.FullNumber,  this.numberGenerator.FullNumber);
		}


		private FormattedText GetFormattedPrice(decimal? price)
		{
			if (price.HasValue)
			{
				if (this.businessDocumentEntity == null)
				{
					return Misc.PriceToString (PriceCalculator.RoundToCents (price.Value));
				}
				else
				{
					return Misc.PriceToString (PriceCalculator.ClipPriceValue (price.Value, this.businessDocumentEntity.CurrencyCode));
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
			if (error != DocumentItemAccessorError.OK)
			{
				var key = DocumentItemAccessor.GetKey (row, column);
				this.errors[key] = error;
			}
		}

		private static void EnsureValidMode(DocumentItemAccessorMode mode)
		{
			int count = 0;

			if (mode.HasFlag (DocumentItemAccessorMode.EditArticleName))
				count++;
			if (mode.HasFlag (DocumentItemAccessorMode.EditArticleDescription))
				count++;
			if (mode.HasFlag (DocumentItemAccessorMode.UseArticleName))
				count++;
			if (mode.HasFlag (DocumentItemAccessorMode.UseArticleBoth))
				count++;

			if (count == 0)
			{
				throw new System.NotSupportedException ("No mode has been specified");
			}
			if (count > 1)
			{
				throw new System.NotSupportedException ("At most one mode may be specified");
			}

			if ((!mode.HasFlag (DocumentItemAccessorMode.Print)) &&
					(mode.HasFlag (DocumentItemAccessorMode.AdditionalQuantities)))
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


		private static readonly int identSpacePerLevel = 3;

		private readonly DocumentMetadataEntity						documentMetadataEntity;
		private readonly BusinessDocumentEntity						businessDocumentEntity;
		private readonly BillingMode								billingMode;
		private readonly BusinessLogic								businessLogic;
		private readonly IncrementalNumberGenerator					numberGenerator;
		private readonly Dictionary<int, FormattedText>				content;
		private readonly Dictionary<int, DocumentItemAccessorError>	errors;
		private readonly List<ArticleQuantityEntity>				articleQuantityEntities;

		private AbstractDocumentItemEntity							item;
		private DocumentType										type;
		private DocumentItemAccessorMode							mode;
		private int													rowsCount;
		private int													groupIndex;
		private int													lineNumber;
		private int													relativeLineNumber;
		private string												groupText;
	}
}
