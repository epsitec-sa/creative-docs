//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Library.Business.ContentAccessors
{
	/// <summary>
	/// Accesseur universel permettant d'obtenir le contenu d'une ligne d'un document commercial,
	/// sous forme d'un tableau (ligne/colonne).
	/// </summary>
	public class DocumentItemAccessor
	{
		public DocumentItemAccessor(BusinessDocumentEntity businessDocumentEntity)
		{
			this.businessDocumentEntity = businessDocumentEntity;

			this.content = new Dictionary<int, FormattedText> ();
			this.articleQuantityEntities = new List<ArticleQuantityEntity> ();
		}

		public void BuildContent(AbstractDocumentItemEntity item, DocumentType type, DocumentItemAccessorMode mode)
		{
			//	Construit tout le contenu.
			this.type = type;
			this.mode = mode;

			this.content.Clear ();

			if (item is TextDocumentItemEntity)
			{
				this.BuildTextItem (item as TextDocumentItemEntity);
			}

			if (item is ArticleDocumentItemEntity)
			{
				this.BuildArticleItem (item as ArticleDocumentItemEntity);
			}

			if (item is TaxDocumentItemEntity)
			{
				this.BuildTaxItem (item as TaxDocumentItemEntity);
			}

			if (item is SubTotalDocumentItemEntity)
			{
				this.BuildSubTotalItem (item as SubTotalDocumentItemEntity);
			}

			if (item is EndTotalDocumentItemEntity)
			{
				this.BuildEndTotalItem (item as EndTotalDocumentItemEntity);
			}
		}

		public int RowsCount
		{
			//	Retourne le nombre total de lignes.
			get
			{
				return this.rowsCount;
			}
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
			this.SetContent (0, DocumentItemAccessorColumn.ArticleDescription, line.Text);
		}

		private void BuildArticleItem(ArticleDocumentItemEntity line)
		{
			int row = 0;
			for (int i = 0; i < DocumentItemAccessor.articleItemTypes.Length; i++)
			{
				if ((this.mode & DocumentItemAccessorMode.SpecialQuantitiesToDistinctLines) == 0)  // colonnes distinctes ?
				{
					row = 0;
				}

				foreach (var quantity in line.ArticleQuantities.Where (x => x.QuantityColumn.QuantityType == DocumentItemAccessor.articleItemTypes[i]))
				{
					this.articleQuantityEntities.Add (quantity);

					DocumentItemAccessorColumn quantityColumn, unitColumn, beginDateColumn, endDateColumn;

					if ((this.mode & DocumentItemAccessorMode.SpecialQuantitiesToDistinctLines) == 0)  // colonnes distinctes ?
					{
						quantityColumn  = DocumentItemAccessor.articleItemQuantityColumns[i];
						unitColumn      = DocumentItemAccessor.articleItemUnitColumns[i];
						beginDateColumn = DocumentItemAccessor.articleItemBeginDateColumns[i];
						endDateColumn   = DocumentItemAccessor.articleItemEndDateColumns[i];
					}
					else  // lignes distinctes ?
					{
						this.SetContent (row, DocumentItemAccessorColumn.UniqueType, quantity.QuantityColumn.Name);

						quantityColumn  = DocumentItemAccessorColumn.UniqueQuantity;
						unitColumn      = DocumentItemAccessorColumn.UniqueUnit;
						beginDateColumn = DocumentItemAccessorColumn.UniqueBeginDate;
						endDateColumn   = DocumentItemAccessorColumn.UniqueEndDate;
					}

					this.SetContent (row, quantityColumn, quantity.Quantity.ToString ());
					this.SetContent (row, unitColumn, quantity.Unit.Name);

					if (quantity.BeginDate.HasValue)
					{
						this.SetContent (row, beginDateColumn, quantity.BeginDate.Value.ToString ());
					}

					if (quantity.EndDate.HasValue)
					{
						this.SetContent (row, endDateColumn, quantity.EndDate.Value.ToString ());
					}

					row++;
				}
			}

			FormattedText description = ArticleDocumentItemHelper.GetArticleDescription (line, replaceTags: true, shortDescription: this.type == DocumentType.ProductionOrder);

			this.SetContent (0, DocumentItemAccessorColumn.ArticleId,          ArticleDocumentItemHelper.GetArticleId (line));
			this.SetContent (0, DocumentItemAccessorColumn.ArticleDescription, description);
			this.SetContent (0, DocumentItemAccessorColumn.UnitPrice,          this.GetFormattedPrice (line.PrimaryUnitPriceBeforeTax));

			if (line.ResultingLinePriceBeforeTax.HasValue && line.ResultingLineTax1.HasValue)
			{
				decimal beforeTax = line.ResultingLinePriceBeforeTax.Value;
				decimal tax =       line.ResultingLineTax1.Value;

				this.SetContent (0, DocumentItemAccessorColumn.LinePrice, this.GetFormattedPrice (beforeTax));
				this.SetContent (0, DocumentItemAccessorColumn.Vat,       this.GetFormattedPrice (tax));
				this.SetContent (0, DocumentItemAccessorColumn.Total,     this.GetFormattedPrice (beforeTax+tax));
			}

			if (line.Discounts.Count != 0)
			{
				if (line.Discounts[0].DiscountRate.HasValue)
				{
					this.SetContent (0, DocumentItemAccessorColumn.Discount, this.GetFormattedPercent (line.Discounts[0].DiscountRate.Value));
				}

				if (line.Discounts[0].Value.HasValue)
				{
					this.SetContent (0, DocumentItemAccessorColumn.Discount, this.GetFormattedPrice (line.Discounts[0].Value.Value));
				}
			}
		}

		#region Static tables
		static DocumentItemAccessor()
		{
			System.Diagnostics.Debug.Assert (DocumentItemAccessor.articleItemTypes.Length == DocumentItemAccessor.articleItemQuantityColumns.Length);
			System.Diagnostics.Debug.Assert (DocumentItemAccessor.articleItemTypes.Length == DocumentItemAccessor.articleItemUnitColumns.Length);
			System.Diagnostics.Debug.Assert (DocumentItemAccessor.articleItemTypes.Length == DocumentItemAccessor.articleItemBeginDateColumns.Length);
			System.Diagnostics.Debug.Assert (DocumentItemAccessor.articleItemTypes.Length == DocumentItemAccessor.articleItemEndDateColumns.Length);
		}

		static ArticleQuantityType[] articleItemTypes =
			{
				ArticleQuantityType.Ordered,
				ArticleQuantityType.Billed,
				ArticleQuantityType.Delayed,
				ArticleQuantityType.Expected,
				ArticleQuantityType.Shipped,
				ArticleQuantityType.ShippedPreviously,
				ArticleQuantityType.Information,
			};

		static DocumentItemAccessorColumn[] articleItemQuantityColumns =
			{
				DocumentItemAccessorColumn.OrderedQuantity,
				DocumentItemAccessorColumn.BilledQuantity,
				DocumentItemAccessorColumn.DelayedQuantity,
				DocumentItemAccessorColumn.ExpectedQuantity,
				DocumentItemAccessorColumn.ShippedQuantity,
				DocumentItemAccessorColumn.ShippedPreviouslyQuantity,
				DocumentItemAccessorColumn.InformationQuantity,
			};

		static DocumentItemAccessorColumn[] articleItemUnitColumns =
			{
				DocumentItemAccessorColumn.OrderedUnit,
				DocumentItemAccessorColumn.BilledUnit,
				DocumentItemAccessorColumn.DelayedUnit,
				DocumentItemAccessorColumn.ExpectedUnit,
				DocumentItemAccessorColumn.ShippedUnit,
				DocumentItemAccessorColumn.ShippedPreviouslyUnit,
				DocumentItemAccessorColumn.InformationUnit,
			};

		static DocumentItemAccessorColumn[] articleItemBeginDateColumns =
			{
				DocumentItemAccessorColumn.OrderedBeginDate,
				DocumentItemAccessorColumn.BilledBeginDate,
				DocumentItemAccessorColumn.DelayedBeginDate,
				DocumentItemAccessorColumn.ExpectedBeginDate,
				DocumentItemAccessorColumn.ShippedBeginDate,
				DocumentItemAccessorColumn.ShippedPreviouslyBeginDate,
				DocumentItemAccessorColumn.InformationBeginDate,
			};

		static DocumentItemAccessorColumn[] articleItemEndDateColumns =
			{
				DocumentItemAccessorColumn.OrderedEndDate,
				DocumentItemAccessorColumn.BilledEndDate,
				DocumentItemAccessorColumn.DelayedEndDate,
				DocumentItemAccessorColumn.ExpectedEndDate,
				DocumentItemAccessorColumn.ShippedEndDate,
				DocumentItemAccessorColumn.ShippedPreviouslyEndDate,
				DocumentItemAccessorColumn.InformationEndDate,
			};
		#endregion

		private void BuildTaxItem(TaxDocumentItemEntity line)
		{
			var text = line.Text;

			if (text.IsNullOrEmpty)
			{
				text = "TVA ({total} à {taux})";
			}

			foreach (var pattern in DocumentItemAccessor.baseAmountList)
			{
				text = text.ToString ().Replace (pattern, this.GetFormattedPrice (line.BaseAmount).ToString ());
			}

			foreach (var pattern in DocumentItemAccessor.rateList)
			{
				text = text.ToString ().Replace (pattern, Misc.PercentToString (line.Rate).ToString ());
			}

			this.SetContent (0, DocumentItemAccessorColumn.ArticleDescription, text);
			this.SetContent (0, DocumentItemAccessorColumn.Vat, this.GetFormattedPrice (line.ResultingTax));
		}

		private static string[] baseAmountList =
		{
			"{total}",
			"{Total}",
			"{prix}",
			"{Prix}",
			"{price}",
			"{Price}",
			"{base}",
			"{Base}",
			"{amount}",
			"{Amount}",
			"{baseamount}",
			"{BaseAmount}",
		};

		private static string[] rateList =
		{
			"{taux}",
			"{Taux}",
			"{tva}",
			"{Tva}",
			"{TVA}",
			"{rate}",
			"{Rate}",
			"{vat}",
			"{Vat}",
			"{VAT}",
		};

		private void BuildSubTotalItem(SubTotalDocumentItemEntity line)
		{
			//	1) Ligne "sous-total".
			FormattedText primaryText = line.TextForPrimaryPrice;

			if (primaryText.IsNullOrEmpty)
			{
				primaryText = "Sous-total";
			}

			decimal primaryPrice = line.PrimaryPriceBeforeTax.GetValueOrDefault (0);
			decimal primaryVat   = line.PrimaryTax.GetValueOrDefault (0);

			//	2) Ligne "rabais".
			bool existingDiscount = false;
			FormattedText discountText = line.TextForDiscount;

			if (discountText.IsNullOrEmpty)
			{
				discountText = "Rabais";
			}

			if (line.Discount.DiscountRate.HasValue && line.Discount.DiscountRate.Value != 0)
			{
				existingDiscount = true;
				discountText = FormattedText.Concat (discountText, " (", this.GetFormattedPercent (line.Discount.DiscountRate), ")");
			}
			else if (line.Discount.Value.HasValue && line.Discount.Value != 0)
			{
				existingDiscount = true;
			}

			decimal discountPrice = line.PrimaryPriceBeforeTax.Value - line.ResultingPriceBeforeTax.Value;
			decimal discountVat   = line.PrimaryTax.Value            - line.ResultingTax.Value;

			//	3) Ligne "total après rabais".
			FormattedText sumText = line.TextForResultingPrice;
			
			if (sumText.IsNullOrEmpty)
			{
				sumText = "Total après rabais";
			}

			decimal sumPrice = line.ResultingPriceBeforeTax.Value;
			decimal sumVat   = line.ResultingTax.Value;

			//	Génère les lignes.
			int row = 0;

			this.SetContent (row, DocumentItemAccessorColumn.ArticleDescription, primaryText);
			this.SetContent (row, DocumentItemAccessorColumn.LinePrice,          this.GetFormattedPrice (primaryPrice));
			this.SetContent (row, DocumentItemAccessorColumn.Vat,                this.GetFormattedPrice (primaryVat));
			this.SetContent (row, DocumentItemAccessorColumn.Total,              this.GetFormattedPrice (primaryPrice + primaryVat));
			row++;

			if (existingDiscount || (this.mode & DocumentItemAccessorMode.ForceAllLines) != 0)
			{
				this.SetContent (row, DocumentItemAccessorColumn.ArticleDescription, discountText);
				this.SetContent (row, DocumentItemAccessorColumn.LinePrice,          this.GetFormattedPrice (discountPrice));
				this.SetContent (row, DocumentItemAccessorColumn.Vat,                this.GetFormattedPrice (discountVat));
				this.SetContent (row, DocumentItemAccessorColumn.Total,              this.GetFormattedPrice (discountPrice + discountVat));
				row++;
	
				this.SetContent (row, DocumentItemAccessorColumn.ArticleDescription, sumText);
				this.SetContent (row, DocumentItemAccessorColumn.LinePrice,          this.GetFormattedPrice (sumPrice));
				this.SetContent (row, DocumentItemAccessorColumn.Vat,                this.GetFormattedPrice (sumVat));
				this.SetContent (row, DocumentItemAccessorColumn.Total,              this.GetFormattedPrice (sumPrice + sumVat));
				row++;
			}
		}

		private void BuildEndTotalItem(EndTotalDocumentItemEntity line)
		{
			{
				var text = line.TextForPrice;

				if (text.IsNullOrEmpty)
				{
					text = "Grand total";
				}

				this.SetContent (0, DocumentItemAccessorColumn.ArticleDescription, text);
				this.SetContent (0, DocumentItemAccessorColumn.LinePrice, this.GetFormattedPrice (line.PriceBeforeTax));
				this.SetContent (0, DocumentItemAccessorColumn.Total, this.GetFormattedPrice (line.PriceAfterTax));
			}

			if (line.FixedPriceAfterTax.HasValue)
			{
				var text = line.TextForFixedPrice;

				if (text.IsNullOrEmpty)
				{
					text = "Grand total arrêté";
				}

				this.SetContent (1, DocumentItemAccessorColumn.ArticleDescription, text);
				this.SetContent (1, DocumentItemAccessorColumn.Total, this.GetFormattedPrice (line.FixedPriceAfterTax));
			}
		}


		private FormattedText GetFormattedPrice(decimal? price)
		{
			if (price == null)
			{
				return null;
			}

			if (this.businessDocumentEntity == null)
			{
				price = PriceCalculator.RoundToCents (price.Value);
			}
			else
			{
				price = PriceCalculator.ClipPriceValue (price.Value, this.businessDocumentEntity.CurrencyCode);
			}

			return Misc.PriceToString (price);
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


		private static int GetKey(int row, DocumentItemAccessorColumn column)
		{
			//	Retourne une clé unique qui mixe la ligne et la colonne.
			//	On suppose qu'il n'y aura jamais plus de 1000 colonnes !
			return row*1000 + (int) column;
		}


		private readonly BusinessDocumentEntity			businessDocumentEntity;
		private readonly Dictionary<int, FormattedText>	content;
		private readonly List<ArticleQuantityEntity>	articleQuantityEntities;

		private DocumentType							type;
		private DocumentItemAccessorMode				mode;
		private int										rowsCount;
	}
}
