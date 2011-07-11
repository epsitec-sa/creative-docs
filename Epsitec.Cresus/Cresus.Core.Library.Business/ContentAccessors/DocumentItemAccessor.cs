//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Business;
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
		public DocumentItemAccessor()
		{
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
			this.SetContent (0, DocumentItemAccessorColumn.UnitPrice,          Misc.PriceToString (line.PrimaryUnitPriceBeforeTax));

			if (line.ResultingLinePriceBeforeTax.HasValue && line.ResultingLineTax1.HasValue)
			{
				decimal beforeTax = line.ResultingLinePriceBeforeTax.Value;
				decimal tax =       line.ResultingLineTax1.Value;

				this.SetContent (0, DocumentItemAccessorColumn.LinePrice, Misc.PriceToString (beforeTax));
				this.SetContent (0, DocumentItemAccessorColumn.Vat,       Misc.PriceToString (tax));
				this.SetContent (0, DocumentItemAccessorColumn.Total,     Misc.PriceToString (beforeTax+tax));
			}

			if (line.Discounts.Count != 0)
			{
				if (line.Discounts[0].DiscountRate.HasValue)
				{
					this.SetContent (0, DocumentItemAccessorColumn.Discount, Misc.PercentToString (line.Discounts[0].DiscountRate.Value));
				}

				if (line.Discounts[0].Value.HasValue)
				{
					this.SetContent (0, DocumentItemAccessorColumn.Discount, Misc.PriceToString (line.Discounts[0].Value.Value));
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
			FormattedText text = FormattedText.Concat (line.Text, " (", Misc.PriceToString (line.BaseAmount), ")");

			this.SetContent (0, DocumentItemAccessorColumn.ArticleDescription, text);
			this.SetContent (0, DocumentItemAccessorColumn.LinePrice, Misc.PriceToString (line.ResultingTax));
		}

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
			FormattedText discountText = "";

			if (discountText.IsNullOrEmpty)
			{
				discountText = "Rabais";
			}

			if (line.Discount.DiscountRate.HasValue)
			{
				existingDiscount = true;
				discountText = line.Discount.Text;

				if (discountText.IsNullOrEmpty)
				{
					discountText = "Rabais";
				}

				discountText = FormattedText.Concat (discountText, " (", Misc.PercentToString (line.Discount.DiscountRate), ")");
			}

			if (line.Discount.Value.HasValue)
			{
				existingDiscount = true;
				discountText = line.Discount.Text;

				if (discountText.IsNullOrEmpty)
				{
					discountText = "Rabais";
				}
			}

			decimal discountPrice = line.PrimaryPriceBeforeTax.Value - line.ResultingPriceBeforeTax.Value;
			decimal discountVat   = line.PrimaryTax.Value            - line.ResultingTax.Value;

			//	3) Ligne "total après rabais".
			FormattedText sumText = "Total après rabais";

			decimal sumPrice = line.ResultingPriceBeforeTax.Value;
			decimal sumVat   = line.ResultingTax.Value;

			//	Génère les lignes.
			int row = 0;

			this.SetContent (row, DocumentItemAccessorColumn.ArticleDescription, primaryText);
			this.SetContent (row, DocumentItemAccessorColumn.LinePrice,          Misc.PriceToString (primaryPrice));
			this.SetContent (row, DocumentItemAccessorColumn.Vat,                Misc.PriceToString (primaryVat));
			this.SetContent (row, DocumentItemAccessorColumn.Total,              Misc.PriceToString (primaryPrice + primaryVat));
			row++;

			if (existingDiscount || (this.mode & DocumentItemAccessorMode.ForceAllLines) != 0)
			{
				this.SetContent (row, DocumentItemAccessorColumn.ArticleDescription, discountText);
				this.SetContent (row, DocumentItemAccessorColumn.LinePrice,          Misc.PriceToString (discountPrice));
				this.SetContent (row, DocumentItemAccessorColumn.Vat,                Misc.PriceToString (discountVat));
				this.SetContent (row, DocumentItemAccessorColumn.Total,              Misc.PriceToString (discountPrice + discountVat));
				row++;
	
				this.SetContent (row, DocumentItemAccessorColumn.ArticleDescription, sumText);
				this.SetContent (row, DocumentItemAccessorColumn.LinePrice,          Misc.PriceToString (sumPrice));
				this.SetContent (row, DocumentItemAccessorColumn.Vat,                Misc.PriceToString (sumVat));
				this.SetContent (row, DocumentItemAccessorColumn.Total,              Misc.PriceToString (sumPrice + sumVat));
				row++;
			}
	
#if false
			string discount = InvoiceDocumentHelper.GetAmount (line);

			//	Colonne "Désignation":
			if (discount == null)
			{
				this.SetContent (0, DocumentItemAccessorColumn.ArticleDescription, line.TextForResultingPrice);
			}
			else
			{
				FormattedText rabais;
				if (line.Discount.DiscountRate.HasValue)
				{
					rabais = FormattedText.Concat ("Rabais ", discount);  // Rabais 20.0%
				}
				else
				{
					rabais = "Rabais";
				}

				this.SetContent (0, DocumentItemAccessorColumn.ArticleDescription, line.TextForPrimaryPrice);
				this.SetContent (1, DocumentItemAccessorColumn.ArticleDescription, rabais);
				this.SetContent (2, DocumentItemAccessorColumn.ArticleDescription, line.TextForResultingPrice);
			}

			//	Colonne "Prix HT":
			if (discount == null)
			{
				decimal v1 = line.ResultingPriceBeforeTax.GetValueOrDefault (0);
				string p1 = Misc.PriceToString (v1);

				this.SetContent (0, DocumentItemAccessorColumn.LinePrice, p1);
			}
			else
			{
				decimal v1 = line.PrimaryPriceBeforeTax.GetValueOrDefault (0);
				decimal v3 = line.ResultingPriceBeforeTax.GetValueOrDefault (0);

				this.SetContent (0, DocumentItemAccessorColumn.LinePrice, Misc.PriceToString (v1));
				this.SetContent (1, DocumentItemAccessorColumn.LinePrice, Misc.PriceToString (v3 - v1));
				this.SetContent (2, DocumentItemAccessorColumn.LinePrice, Misc.PriceToString (v3));
			}

			//	Colonne "TVA":
			if (discount == null)
			{
				decimal v1 = line.ResultingTax.GetValueOrDefault (0);
				string p1 = Misc.PriceToString (v1);

				this.SetContent (0, DocumentItemAccessorColumn.Vat, p1);
			}
			else
			{
				decimal v1 = line.PrimaryTax.GetValueOrDefault (0);
				decimal v3 = line.ResultingTax.GetValueOrDefault (0);

				this.SetContent (0, DocumentItemAccessorColumn.Vat, Misc.PriceToString (v1));
				this.SetContent (1, DocumentItemAccessorColumn.Vat, Misc.PriceToString (v3 - v1));
				this.SetContent (2, DocumentItemAccessorColumn.Vat, Misc.PriceToString (v3));
			}

			//	Colonne "Prix TTC":
			if (discount == null)
			{
				decimal v1 = line.ResultingPriceBeforeTax.GetValueOrDefault (0) + line.ResultingTax.GetValueOrDefault (0);
				string p1 = Misc.PriceToString (v1);

				this.SetContent (0, DocumentItemAccessorColumn.Total, p1);
			}
			else
			{
				decimal v1 = line.PrimaryPriceBeforeTax.GetValueOrDefault (0) + line.PrimaryTax.GetValueOrDefault (0);
				decimal v3 = line.ResultingPriceBeforeTax.GetValueOrDefault (0) + line.ResultingTax.GetValueOrDefault (0);

				this.SetContent (0, DocumentItemAccessorColumn.Total, Misc.PriceToString (v1));
				this.SetContent (1, DocumentItemAccessorColumn.Total, Misc.PriceToString (v3 - v1));
				this.SetContent (2, DocumentItemAccessorColumn.Total, Misc.PriceToString (v3));
			}
#endif
		}

		private void BuildEndTotalItem(EndTotalDocumentItemEntity line)
		{
			if (line.PriceBeforeTax.HasValue)  // ligne de total HT ?
			{
				this.SetContent (0, DocumentItemAccessorColumn.ArticleDescription, line.TextForPrice);
				this.SetContent (0, DocumentItemAccessorColumn.LinePrice, Misc.PriceToString (line.PriceBeforeTax));
			}
			else if (line.FixedPriceAfterTax.HasValue)
			{
				this.SetContent (0, DocumentItemAccessorColumn.ArticleDescription, line.TextForPrice);
				this.SetContent (1, DocumentItemAccessorColumn.ArticleDescription, line.TextForFixedPrice);

				this.SetContent (0, DocumentItemAccessorColumn.Total, Misc.PriceToString (line.PriceAfterTax));
				this.SetContent (1, DocumentItemAccessorColumn.Total, Misc.PriceToString (line.FixedPriceAfterTax));
			}
			else
			{
				this.SetContent (0, DocumentItemAccessorColumn.ArticleDescription, line.TextForPrice);
				this.SetContent (0, DocumentItemAccessorColumn.Total, Misc.PriceToString (line.PriceAfterTax));
			}
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


		private DocumentType						type;
		private DocumentItemAccessorMode			mode;
		private Dictionary<int, FormattedText>		content;
		private List<ArticleQuantityEntity>			articleQuantityEntities;
		private int									rowsCount;
	}
}
